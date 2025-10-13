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
# final_reply = result["final_response"][-1].content
print("\n--- Final Agent Response ---")
print(result)

# Note: Remember to stop the Playwright MCP server process when done (e.g., using 'pkill -f playwright')