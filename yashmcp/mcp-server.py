from fastmcp import FastMCP

# Initialize the server with a name
mcp = FastMCP("demo_server")

# Use the @mcp.tool decorator to define a tool
@mcp.tool
def add(a: int, b: int) -> int:
    """Adds two numbers and returns the result."""
    return a + b

# Run the server
if __name__ == "__main__":
    mcp.run()