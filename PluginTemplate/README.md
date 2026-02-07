# Flow.Launcher Plugin Template

This directory contains templates to help you quickly start developing Flow.Launcher plugins.

## Available Templates

### C# Plugin Template

A complete C# plugin template with:
- Basic plugin structure
- Settings implementation
- Context menu example
- Internationalization support

**See**: `CSharpTemplate/`

### Python Plugin Template

A simple Python plugin template with:
- JSON-RPC communication
- Basic query handling
- Example actions

**See**: `PythonTemplate/`

### JavaScript Plugin Template

A Node.js/JavaScript plugin template with:
- Flow helper library integration
- Modern ES6+ syntax
- Example results

**See**: `JavaScriptTemplate/`

## Quick Start

1. **Choose a template** based on your preferred language
2. **Copy the template** to your development directory
3. **Rename** the plugin to match your plugin name
4. **Update** `plugin.json` with your plugin details:
   - Generate a unique ID (GUID)
   - Set your action keyword
   - Update author, description, etc.
5. **Implement** your plugin logic
6. **Test** by copying to Flow.Launcher's plugins directory
7. **Publish** following the [Plugin Development Guide](../PLUGIN_DEVELOPMENT.md#publishing-your-plugin)

## Plugin Location

When testing, copy your plugin to:
- **Roaming**: `%APPDATA%\FlowLauncher\Plugins\YourPluginName-<version>`
- **Portable**: `%localappdata%\FlowLauncher\app-<version>\UserData\Plugins\YourPluginName-<version>`

Type `flow user data` in Flow.Launcher to find your user data folder.

## Documentation

For detailed instructions, see the [Plugin Development Guide](../PLUGIN_DEVELOPMENT.md).

## Need Help?

- [Official Documentation](https://www.flowlauncher.com/docs/)
- [Community Discord](https://discord.gg/AvgAQgh)
- [GitHub Discussions](https://github.com/Flow-Launcher/Flow.Launcher/discussions)
