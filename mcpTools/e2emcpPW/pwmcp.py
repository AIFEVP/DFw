import os
from haystack.components.agents import Agent
from haystack.dataclasses import ChatMessage
from haystack_integrations.components.generators.google_genai import GoogleGenAIChatGenerator
from haystack_integrations.tools.mcp import MCPToolset, StreamableHttpServerInfo
from haystack.components.generators.utils import print_streaming_chunk
from haystack.utils import Secret

# --- 1. Configure the Playwright Toolset ---
# Define the connection info for the running Playwright MCP server
# Note: Use the /mcp endpoint and the correct port (8931 from the setup above)
SERVER_PORT = 8931
server_info = StreamableHttpServerInfo(url=f"http://localhost:{SERVER_PORT}/mcp")

# Create the MCPToolset. We explicitly select browser automation tools.
toolset = MCPToolset(
    server_info=server_info,
    tool_names=[
        "browser_navigate",               # For going to a URL
        "browser_snapshot", # For giving the LLM the DOM context
        "browser_click",                  # For clicking on elements
        "browser_fill_form"               # For filling out forms
    ]
)

print(os.environ.get("GEMINI_KEY"))

# --- 2. Configure the LLM Generator (Gemini) ---
# The Agent requires a Chat Generator that supports tool/function calling
chat_generator = GoogleGenAIChatGenerator(
    model="gemini-2.5-flash",
    api_key=Secret.from_env_var("GEMINI_KEY")
)

# Define a system message to guide the LLM's behavior
SYSTEM_PROMPT = """
You are an expert web automation agent. Your goal is to use the provided browser tools to fulfill the user's request.
1. Always start by using the 'browser_navigate' tool to go to the specified URL.
2. After navigation, use 'browser_get_accessibility_snapshot' to inspect the page.
3. Use the other tools ('browser_click', 'browser_fill_form') iteratively to complete the task.
4. Only when the task is complete, provide a final text response to the user.
"""

# --- 3. Build the Haystack Agent ---
browser_agent = Agent(
    chat_generator=chat_generator,
    tools=toolset.tools,
    system_prompt=SYSTEM_PROMPT,
    streaming_callback=print_streaming_chunk, # Optional: to see the agent's thought process
    exit_conditions=["text"] # Exit when the LLM replies with a final text message
)


LOGIN_URL = "https://practicetestautomation.com/practice-test-login/"
# --- 4. Run the Agent ---
print("--- Starting Haystack Browser Agent ---")
user_query = ChatMessage.from_user(
    f"1. Navigate to the login page at '{LOGIN_URL}'. "
    "2. Log in using the username 'student' and the password 'Password123' "
    "by filling the respective form fields and clicking the 'Submit' button. "
    "3. Once the page redirects to the secure area, find the element that contains the success message, "
    "which starts with the text 'Congratulations!'. "
    "4. Return ONLY the full text content of that success message element."
)

result = browser_agent.run(messages=[user_query])

# The final answer is in the last message of the result's replies
final_reply = result["messages"][-1].text
print("\n--- Final Agent Response ------------------------")
# print(final_reply)
# Note: Remember to stop the Playwright MCP server process when done (e.g., using 'pkill -f playwright')

# Assuming 'result' is the dictionary returned by browser_agent.run(...)

print("\n--- Agent Execution Steps (The Generated Test) ---")
step_counter = 1

from haystack.dataclasses import ToolCall, TextContent # Make sure these are imported

# Assuming 'result' is the dictionary returned by browser_agent.run(...)

print("\n--- Agent Execution Steps (The Generated Test) ---")
step_counter = 1

for message in result["messages"]:
    # Check the structured content list
    content_parts = message._content 

    # Skip messages with no structured content
    if not content_parts:
        continue
    
    # Check the first content part's class
    first_part = content_parts[0]

    # 1. Look for the Agent's Tool Call (The Action)
    if message.role.value == 'assistant' and isinstance(first_part, ToolCall):
        
        tool_call = first_part # It IS the ToolCall object
        tool_name = tool_call.tool_name
        args = tool_call.arguments

        print(f"[{step_counter}] 🤖 AGENT ACTION: **{tool_name}**")
        print(f"    - Args: {args}")
        step_counter += 1

    # 2. Look for the Tool's Response (The Observation/Verification)
    elif message.role.value == 'tool':
        # Safely extract tool name from the origin attribute
        tool_name = message._content[0].origin.tool_name if message._content and hasattr(message._content[0], 'origin') else "UNKNOWN TOOL"
        print(f"   ✅ TOOL OBSERVATION: Tool call for **{tool_name}** successful. (Check full transcript for details)")

    # 3. Stop when the final text is reached
    # The final message is from the assistant and contains simple text content
    elif message.role.value == 'assistant' and isinstance(first_part, TextContent):
        print(f"[{step_counter}] 🏁 FINAL VERIFICATION: Test passed.")
        print(f"    - Final Result: {message.text}")
        break