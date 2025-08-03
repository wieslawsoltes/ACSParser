# MCP (Model Context Protocol) Support for AcsEditor

The AcsEditor now includes MCP support, allowing AI assistants to interact with animations through standardized tools. This enables natural language commands for animation selection and management.

## Features

The MCP server provides the following tools:

### Animation Management Tools

1. **ListAnimations** - List all available animations in the current ACS file
2. **SelectAnimationByIndex** - Select an animation by its index (0-based)
3. **SelectAnimationByName** - Select an animation by name (case-insensitive partial match)
4. **GetAnimationDetails** - Get detailed information about a specific animation
5. **GetFileInfo** - Get information about the currently loaded ACS file

## Setup Instructions

### Prerequisites

- .NET 8.0 or later
- An MCP-compatible AI client (Claude Desktop, VS Code with GitHub Copilot, etc.)

### For Claude Desktop

1. Build the AcsEditor project:
   ```bash
   dotnet build
   ```

2. Add the following configuration to your Claude Desktop MCP settings file:
   
   **On macOS:** `~/Library/Application Support/Claude/claude_desktop_config.json`
   **On Windows:** `%APPDATA%\Claude\claude_desktop_config.json`

   ```json
   {
     "mcpServers": {
       "acs-editor": {
         "command": "dotnet",
         "args": [
           "run",
           "--project",
           "/path/to/ACSParser/AcsEditor/AcsEditor.csproj"
         ]
       }
     }
   }
   ```

3. Replace `/path/to/ACSParser/AcsEditor/AcsEditor.csproj` with the actual path to your project.

4. Restart Claude Desktop.

### For VS Code with GitHub Copilot

1. Create or update `.vscode/mcp.json` in your workspace:
   ```json
   {
     "inputs": [],
     "servers": {
       "AcsEditor": {
         "type": "stdio",
         "command": "dotnet",
         "args": [
           "run",
           "--project",
           "/path/to/ACSParser/AcsEditor/AcsEditor.csproj"
         ]
       }
     }
   }
   ```

2. Open GitHub Copilot in Agent mode and you should see the AcsEditor tools available.

## Usage Examples

Once configured, you can use natural language commands with your AI assistant:

### Basic Commands

- "List all available animations"
- "Show me the animations in this file"
- "Select the first animation"
- "Choose the animation named 'Idle'"
- "Get details about animation 2"
- "What animations are available?"

### Advanced Commands

- "Select the animation that contains 'wave' in its name"
- "Show me information about the current ACS file"
- "Select animation index 1 and tell me about its frames"
- "Find and select the longest animation"

## How It Works

1. **MCP Server**: The AcsEditor runs an MCP server using the stdio transport protocol
2. **Tool Registration**: Animation management functions are automatically registered as MCP tools
3. **Context Sharing**: When you load an ACS file, the MCP service is updated with the file context
4. **AI Integration**: AI assistants can call these tools to interact with your animations
5. **UI Updates**: When animations are selected via MCP, the UI automatically updates to reflect the selection

## Testing

You can test the MCP functionality using the built-in test client:

```csharp
// In your code or a test console app
await McpTestClient.RunTestAsync();
```

This will demonstrate all the available MCP tools with sample data.

## Troubleshooting

### Common Issues

1. **MCP Server Not Starting**
   - Check that all NuGet packages are properly installed
   - Ensure .NET 8.0 or later is installed
   - Check the console output for error messages

2. **AI Assistant Can't Find Tools**
   - Verify the MCP configuration file path and syntax
   - Ensure the project path in the configuration is correct
   - Restart your AI client after configuration changes

3. **Tools Not Working**
   - Make sure an ACS file is loaded first
   - Check that the MCP service was properly initialized
   - Look for error messages in the application console

### Debug Mode

The MCP server logs to stderr by default. You can monitor these logs to debug issues:

```bash
# Run with verbose logging
dotnet run --project AcsEditor.csproj 2> mcp_debug.log
```

## Architecture

The MCP implementation consists of:

- **McpAnimationService**: Contains the MCP tool implementations
- **McpServerHost**: Manages the MCP server lifecycle
- **MainView Integration**: Connects the MCP server with the UI

The server runs alongside the Avalonia UI application and communicates with AI clients through the stdio transport protocol.

## Contributing

To add new MCP tools:

1. Add methods to `McpAnimationService` with the `[McpServerTool]` attribute
2. Include descriptive `[Description]` attributes for AI context
3. Update this README with the new functionality
4. Test with the `McpTestClient`

## Security Considerations

- The MCP server only exposes read and selection operations
- No file system access or modification capabilities are exposed
- All operations are scoped to the currently loaded ACS file
- The server runs locally and doesn't expose network endpoints