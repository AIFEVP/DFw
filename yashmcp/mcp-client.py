import asyncio
from fastmcp import Client

# Create a client instance, pointing to the server file
client = Client("mcp-server.py")

async def call_tool_and_print():
    """
    Connects to the server, calls a tool, and prints the result.
    """
    async with client:
        # Call the 'greet' tool with the argument 'Ford'
        result = await client.call_tool("add", {"a": 1, "b": 10})
        print(f"Tool call result: {result}")

# Run the asynchronous function
if __name__ == "__main__":
    asyncio.run(call_tool_and_print())