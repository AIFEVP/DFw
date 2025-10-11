import { Client } from '@modelcontextprotocol/sdk/client/index.js';
import { StdioClientTransport } from '@modelcontextprotocol/sdk/client/stdio.js';

// The command and arguments to launch the @playwright/mcp server as a child process.
const serverCommand = 'npx';
const serverArgs = [
    '-y', 
    '@playwright/mcp@latest', 
    '--isolated', // Use a clean browser context for this run
    '--headless'   // Run the browser without a visible UI
];

async function runPlaywrightMCPClient() {
    console.log(`Starting Playwright MCP server with command: ${serverCommand} ${serverArgs.join(' ')}`);

    // 1. Create a transport mechanism (StdioClientTransport is common for local/CLI-based clients)
    const transport = new StdioClientTransport({
        command: serverCommand,
        args: serverArgs,
    });

    // 2. Create the MCP client instance
    const client = new Client(
        { name: "my-node-client", version: "1.0.0" },
        { capabilities: {} } 
    );

    try {
        // 3. Connect the client to the server
        await client.connect(transport);
        console.log("Client connected to Playwright MCP server.");

        // 4. Discover the tools the server exposes
        const listToolsResponse = await client.listTools({});
        const navigateTool = listToolsResponse.tools.find(t => t.name === 'browser_navigate');

        if (navigateTool) {
            console.log(`Tool found: ${navigateTool.name}`);

            // 5. Call a tool exposed by the Playwright MCP server (e.g., 'browser_navigate')
            console.log("Calling tool: browser_navigate to navigate to Playwright's website...");
            const toolCallResponse = await client.callTool({
                name: 'browser_navigate',
                arguments: {
                    url: 'https://playwright.dev/',
                }
            });

            console.log("Navigation successful. Tool response:");
            // Log the structured content from the tool response (usually a snapshot of the page)
            if (toolCallResponse.structuredContent) {
                 // In a real AI client, this structured content (page snapshot) would be sent to the LLM
                 console.log("Received structured content (snapshot summary).");
            }

            // 6. You could follow up with another tool call, like 'browser_get_title' or 'browser_wait_for'
            // ...
        } else {
            console.log("browser_navigate tool not found.");
        }

    } catch (error) {
        console.error("An error occurred during MCP communication:", error);
    } finally {
        // 7. Close the connection and shut down the server process
        await client.close();
        console.log("Client connection closed and server shut down.");
    }
}

runPlaywrightMCPClient();