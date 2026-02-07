# Flow.Launcher Plugin Development Guide

Welcome to the Flow.Launcher plugin development guide! This document will help you create your own plugins for Flow.Launcher in multiple programming languages.

## Table of Contents

- [Overview](#overview)
- [Plugin Architecture](#plugin-architecture)
- [Supported Languages](#supported-languages)
- [Getting Started](#getting-started)
  - [C# Plugins](#c-plugins)
  - [F# Plugins](#f-plugins)
  - [Python Plugins](#python-plugins)
  - [JavaScript/TypeScript Plugins](#javascripttypescript-plugins)
  - [Executable Plugins](#executable-plugins)
- [Plugin Structure](#plugin-structure)
- [Core Concepts](#core-concepts)
- [API Reference](#api-reference)
- [Publishing Your Plugin](#publishing-your-plugin)
- [Examples](#examples)

## Overview

Flow.Launcher uses a modular plugin architecture that allows developers to extend its functionality using multiple programming languages. Plugins can be:

- **Built-in plugins**: Shipped with Flow.Launcher (located in `/Plugins/` directory)
- **Third-party plugins**: Installed from the Plugin Store or manually

## Plugin Architecture

Flow.Launcher supports two main plugin execution models:

### 1. Compiled Plugins (.NET)
- **Languages**: C#, F#
- **Execution**: Direct assembly loading via reflection
- **Performance**: Fast, runs in the same process
- **Use case**: Complex plugins requiring high performance

### 2. JSON-RPC Plugins (External)
- **Languages**: Python, JavaScript, TypeScript, or any executable
- **Execution**: Inter-process communication via JSON-RPC protocol
- **Performance**: Slightly slower due to IPC overhead
- **Use case**: Simple to moderate plugins, or when you prefer languages other than .NET

## Supported Languages

| Language | Type | Framework | Documentation |
|----------|------|-----------|---------------|
| **C#** | Compiled | .NET 9 | [C# Plugin Guide](https://www.flowlauncher.com/docs/#/develop-dotnet-plugins) |
| **F#** | Compiled | .NET 9 | [F# Example](https://github.com/Flow-Launcher/plugin-samples/tree/master/HelloWorldFSharp) |
| **Python** | JSON-RPC | Python 3+ | [Python Plugin Guide](https://www.flowlauncher.com/docs/#/py-develop-plugins) |
| **JavaScript** | JSON-RPC | Node.js | [Node.js Plugin Guide](https://www.flowlauncher.com/docs/#/nodejs-develop-plugins) |
| **TypeScript** | JSON-RPC | Node.js | [Node.js Plugin Guide](https://www.flowlauncher.com/docs/#/nodejs-develop-plugins) |
| **Any Executable** | JSON-RPC | Any | [Executable Plugin Guide](https://www.flowlauncher.com/docs/#/plugin-dev) |

## Getting Started

### C# Plugins

#### Prerequisites
- .NET 9 SDK
- Visual Studio 2022 (v17.12+) or any C# IDE

#### Step 1: Create a New Project

```bash
dotnet new classlib -n Flow.Launcher.Plugin.MyPlugin
cd Flow.Launcher.Plugin.MyPlugin
```

#### Step 2: Add Flow.Launcher.Plugin NuGet Package

```bash
dotnet add package Flow.Launcher.Plugin
```

#### Step 3: Create the Main Plugin Class

```csharp
using System.Collections.Generic;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.MyPlugin
{
    public class Main : IPlugin
    {
        private PluginInitContext _context;

        public void Init(PluginInitContext context)
        {
            _context = context;
        }

        public List<Result> Query(Query query)
        {
            var results = new List<Result>();

            results.Add(new Result
            {
                Title = "Hello World!",
                SubTitle = "This is my first plugin",
                IcoPath = "Images/icon.png",
                Action = e =>
                {
                    _context.API.ShowMsg("Hello", "Hello from my plugin!");
                    return true;
                }
            });

            return results;
        }
    }
}
```

#### Step 4: Create plugin.json

Create a `plugin.json` file in your project root:

```json
{
  "ID": "YOUR-UNIQUE-GUID-HERE",
  "ActionKeyword": "mp",
  "Name": "MyPlugin",
  "Description": "My first Flow.Launcher plugin",
  "Author": "Your Name",
  "Version": "1.0.0",
  "Language": "csharp",
  "Website": "https://github.com/yourusername/yourplugin",
  "ExecuteFileName": "Flow.Launcher.Plugin.MyPlugin.dll",
  "IcoPath": "Images/icon.png"
}
```

**Generate a unique GUID**: Use an online GUID generator or run `[guid]::NewGuid().ToString("N").ToUpper()` in PowerShell.

#### Step 5: Build and Test

```bash
dotnet build
```

Copy the output (usually in `bin/Debug/net9.0/`) to Flow.Launcher's plugin directory:
- Default: `%APPDATA%\FlowLauncher\Plugins\YourPluginName-<version>`

### F# Plugins

F# plugins work similarly to C# plugins. The main difference is the syntax:

```fsharp
namespace Flow.Launcher.Plugin.MyPlugin

open Flow.Launcher.Plugin
open System.Collections.Generic

type Main() =
    let mutable context : PluginInitContext = null
    
    interface IPlugin with
        member this.Init(c: PluginInitContext) =
            context <- c
        
        member this.Query(query: Query) =
            let result = Result()
            result.Title <- "Hello from F#!"
            result.SubTitle <- "F# is awesome"
            result.Action <- fun _ -> 
                context.API.ShowMsg("Hello", "Hello from F# plugin!")
                true
            
            [result] :> IList<Result>
```

### Python Plugins

#### Prerequisites
- Python 3.6 or higher
- `flowlauncher` Python package (optional, but recommended)

#### Step 1: Install the Flow.Launcher Python Module

```bash
pip install flowlauncher
```

#### Step 2: Create Your Plugin Script

Create `main.py`:

```python
from flowlauncher import FlowLauncher

class MyPlugin(FlowLauncher):
    def query(self, query):
        results = []
        
        results.append({
            "Title": "Hello World!",
            "SubTitle": f"You searched for: {query}",
            "IcoPath": "Images/icon.png",
            "JsonRPCAction": {
                "method": "show_message",
                "parameters": ["Hello!", "This is a Python plugin"]
            }
        })
        
        return results
    
    def show_message(self, title, message):
        self.api.show_msg(title, message)

if __name__ == "__main__":
    MyPlugin()
```

#### Step 3: Create plugin.json

```json
{
  "ID": "YOUR-UNIQUE-GUID-HERE",
  "ActionKeyword": "mp",
  "Name": "MyPythonPlugin",
  "Description": "My first Python plugin",
  "Author": "Your Name",
  "Version": "1.0.0",
  "Language": "python",
  "Website": "https://github.com/yourusername/yourplugin",
  "ExecuteFileName": "main.py",
  "IcoPath": "Images/icon.png"
}
```

#### Step 4: Create requirements.txt (Optional)

```txt
flowlauncher
```

### JavaScript/TypeScript Plugins

#### Prerequisites
- Node.js 14+ and npm

#### Step 1: Initialize Your Plugin

```bash
mkdir Flow.Launcher.Plugin.MyPlugin
cd Flow.Launcher.Plugin.MyPlugin
npm init -y
```

#### Step 2: Install Dependencies

```bash
npm install flow-launcher-helper
```

#### Step 3: Create Your Plugin (JavaScript)

Create `main.js`:

```javascript
const { Flow } = require('flow-launcher-helper');

class MyPlugin extends Flow {
    query(query) {
        const results = [];

        results.push({
            Title: "Hello World!",
            SubTitle: `You searched for: ${query}`,
            IcoPath: "Images/icon.png",
            JsonRPCAction: {
                method: "showMessage",
                parameters: ["Hello!", "This is a JavaScript plugin"]
            }
        });

        return results;
    }

    showMessage(title, message) {
        this.showMsg(title, message);
    }
}

const plugin = new MyPlugin();
plugin.run();
```

#### Step 4: Create plugin.json

```json
{
  "ID": "YOUR-UNIQUE-GUID-HERE",
  "ActionKeyword": "mp",
  "Name": "MyJSPlugin",
  "Description": "My first JavaScript plugin",
  "Author": "Your Name",
  "Version": "1.0.0",
  "Language": "javascript",
  "Website": "https://github.com/yourusername/yourplugin",
  "ExecuteFileName": "main.js",
  "IcoPath": "Images/icon.png"
}
```

### Executable Plugins

Any executable that can communicate via JSON-RPC over stdin/stdout can be a plugin. This includes programs written in Go, Rust, Java, etc.

The executable must:
1. Read JSON-RPC requests from stdin
2. Write JSON-RPC responses to stdout
3. Implement the `query` method at minimum

Example JSON-RPC request:
```json
{
  "method": "query",
  "parameters": ["your search query"]
}
```

Example JSON-RPC response:
```json
{
  "result": [
    {
      "Title": "Result Title",
      "SubTitle": "Result subtitle",
      "IcoPath": "Images/icon.png"
    }
  ]
}
```

## Plugin Structure

A typical plugin directory structure:

```
Flow.Launcher.Plugin.MyPlugin/
├── plugin.json              # Plugin metadata (required)
├── Main.cs/main.py/main.js  # Plugin entry point (required)
├── Images/
│   └── icon.png            # Plugin icon (recommended)
├── Languages/              # Internationalization (optional)
│   ├── en.xaml
│   └── zh-cn.xaml
├── Settings.cs             # Plugin settings (optional)
└── README.md               # Plugin documentation (recommended)
```

## Core Concepts

### Plugin Types

1. **System Plugin** (`ActionKeyword: "*"`)
   - Activated without a keyword
   - Always runs for every query
   - Example: Calculator, Program launcher

2. **User Plugin** (Custom `ActionKeyword`)
   - Requires a keyword to activate (e.g., "web", "calc", "todo")
   - Only runs when the query starts with the keyword
   - Example: WebSearch (keyword: "web"), GitHub (keyword: "gh")

### Plugin Metadata (plugin.json)

| Field | Required | Description |
|-------|----------|-------------|
| `ID` | Yes | Unique GUID identifier |
| `ActionKeyword` | Yes | Trigger keyword ("*" for system plugin) |
| `Name` | Yes | Display name |
| `Description` | Yes | Short description |
| `Author` | Yes | Plugin author |
| `Version` | Yes | Semantic version (e.g., "1.0.0") |
| `Language` | Yes | One of: csharp, fsharp, python, javascript, typescript, executable |
| `ExecuteFileName` | Yes | Main DLL or script file |
| `IcoPath` | No | Path to plugin icon |
| `Website` | No | Plugin homepage or repository |

### Query Object

The `Query` object passed to your plugin contains:

- `Search`: The full search query
- `Terms`: Query split by spaces
- `RawQuery`: Original query including action keyword
- `ActionKeyword`: The keyword used to trigger the plugin

### Result Object

Results returned by your plugin should include:

| Field | Required | Description |
|-------|----------|-------------|
| `Title` | Yes | Main text displayed |
| `SubTitle` | No | Secondary text |
| `IcoPath` | No | Path to result icon |
| `Action` | No | Callback function when result is selected |
| `Score` | No | Result ranking (higher = better) |
| `AutoCompleteText` | No | Text to autocomplete when Tab is pressed |
| `CopyText` | No | Text to copy when Ctrl+C is pressed |
| `ContextData` | No | Additional data for context menu |
| `TitleHighlightData` | No | Highlight matching characters in title |
| `SubTitleHighlightData` | No | Highlight matching characters in subtitle |

## API Reference

The `IPublicAPI` interface provides methods to interact with Flow.Launcher:

### Common Methods

```csharp
// Show a message box
API.ShowMsg(string title, string message);

// Change the current query
API.ChangeQuery(string query, bool requery = false);

// Copy text to clipboard
API.CopyToClipboard(string text);

// Run a shell command
API.ShellRun(string command);

// Open a file or URL
API.OpenAppUri(Uri uri);

// Save plugin settings
API.SavePluginSettings<T>();

// Load plugin settings
T API.LoadSettingJsonStorage<T>();

// Reload all plugin data
API.ReloadAllPluginData();

// Get current theme
Theme API.GetCurrentTheme();
```

### Advanced Interfaces

Your plugin can implement additional interfaces for more functionality:

- `IPluginI18n`: Internationalization support
- `ISettingProvider`: Provide a settings UI
- `IContextMenu`: Add context menu items
- `IAsyncPlugin`: Async query support (for long-running operations)
- `IReloadable`: Handle plugin reload events

#### Example: Settings Provider

```csharp
public class Main : IPlugin, ISettingProvider
{
    private PluginInitContext _context;
    private Settings _settings;

    public void Init(PluginInitContext context)
    {
        _context = context;
        _settings = context.API.LoadSettingJsonStorage<Settings>();
    }

    public Control CreateSettingPanel()
    {
        return new MySettingsControl(_settings);
    }

    public List<Result> Query(Query query)
    {
        // Use _settings here
        return results;
    }
}
```

#### Example: Context Menu

```csharp
public class Main : IPlugin, IContextMenu
{
    public List<Result> LoadContextMenus(Result selectedResult)
    {
        return new List<Result>
        {
            new Result
            {
                Title = "Copy to Clipboard",
                Action = _ => 
                {
                    Clipboard.SetText(selectedResult.Title);
                    return true;
                }
            }
        };
    }

    // ... other methods
}
```

## Publishing Your Plugin

### 1. Prepare Your Plugin

- Ensure `plugin.json` is complete and accurate
- Add a `README.md` with usage instructions
- Add a plugin icon (recommended size: 256x256px PNG)
- Test thoroughly on different systems

### 2. Create a GitHub Repository

Push your plugin to a public GitHub repository with the following structure:

```
YourPluginRepo/
├── plugin.json
├── Main.cs/main.py/main.js
├── Images/
├── README.md
└── LICENSE (optional)
```

### 3. Submit to Plugin Store

1. Fork the [Flow.Launcher.PluginsManifest](https://github.com/Flow-Launcher/Flow.Launcher.PluginsManifest) repository
2. Add your plugin entry to the manifest JSON file
3. Create a pull request

Plugin manifest entry example:

```json
{
  "ID": "YOUR-UNIQUE-GUID",
  "Name": "MyPlugin",
  "Description": "My awesome plugin",
  "Author": "Your Name",
  "Version": "1.0.0",
  "Language": "csharp",
  "Website": "https://github.com/yourusername/yourplugin",
  "UrlDownload": "https://github.com/yourusername/yourplugin/releases/latest/download/MyPlugin.zip",
  "UrlSourceCode": "https://github.com/yourusername/yourplugin",
  "IcoPath": "Images\\icon.png",
  "ActionKeyword": "mp"
}
```

### 4. Create a Release

Create a release on GitHub with a ZIP file containing:
- All necessary files (DLLs, scripts, images, etc.)
- plugin.json
- No development files (source code, .csproj, etc.)

## Examples

### Built-in Plugin Examples

Flow.Launcher includes several built-in plugins that serve as great examples:

1. **Calculator** (`/Plugins/Flow.Launcher.Plugin.Calculator/`)
   - Simple C# plugin
   - System plugin (ActionKeyword: "*")
   - Good example of basic plugin structure

2. **Explorer** (`/Plugins/Flow.Launcher.Plugin.Explorer/`)
   - File/folder navigation
   - Settings implementation
   - Context menu implementation

3. **WebSearch** (`/Plugins/Flow.Launcher.Plugin.WebSearch/`)
   - Web search functionality
   - Multiple search engines
   - Settings with XAML UI

### Community Examples

- [Flow.Launcher Plugin Samples](https://github.com/Flow-Launcher/plugin-samples)
- [SpotifyPremium Plugin](https://github.com/fow5040/Flow.Launcher.Plugin.SpotifyPremium) - C# example
- [Steam Search](https://github.com/Garulf/Steam-Search) - Python example
- [GitHub Plugin](https://github.com/JohnTheGr8/Flow.Plugin.Github) - C# with API calls
- [Window Walker](https://github.com/taooceros/Flow.Plugin.WindowWalker) - Advanced C# plugin

## Additional Resources

- **Official Documentation**: https://www.flowlauncher.com/docs/
- **Plugin API Reference**: https://www.flowlauncher.com/docs/#/API-Reference/Flow.Launcher.Plugin
- **Plugin Store**: https://flowlauncher.com/plugins/
- **Community Discord**: https://discord.gg/AvgAQgh
- **Plugin Samples Repository**: https://github.com/Flow-Launcher/plugin-samples

## Tips and Best Practices

1. **Keep it Simple**: Start with a basic plugin and add features incrementally
2. **Use Async When Needed**: Implement `IAsyncPlugin` for long-running operations
3. **Handle Errors Gracefully**: Catch exceptions and return empty results rather than crashing
4. **Provide Clear Results**: Use descriptive titles and subtitles
5. **Optimize Performance**: Cache data when possible, especially for system plugins
6. **Test Thoroughly**: Test with different queries, edge cases, and on different systems
7. **Document Well**: Provide clear README and inline comments
8. **Use Icons**: A good icon makes your plugin more recognizable
9. **Follow Naming Conventions**: Use `Flow.Launcher.Plugin.YourPluginName` for .NET plugins
10. **Version Properly**: Use semantic versioning (MAJOR.MINOR.PATCH)

## Troubleshooting

### Plugin Not Loading

- Check `plugin.json` syntax (use a JSON validator)
- Ensure all required fields are present
- Verify the `ExecuteFileName` matches your main file
- Check Flow.Launcher logs: Type `open log location` in Flow.Launcher

### Plugin Not Showing Results

- Ensure your plugin returns a `List<Result>` (not null)
- Check the ActionKeyword is correct in `plugin.json`
- Verify the `Init` method is being called
- Add logging to debug your plugin logic

### Python Plugin Issues

- Ensure Python is in your PATH
- Install required packages: `pip install flowlauncher`
- Check Python version compatibility (3.6+)

### JavaScript/TypeScript Issues

- Ensure Node.js is installed and in PATH
- Install dependencies: `npm install`
- Check for syntax errors in your script

## Getting Help

If you need help:

1. Check the [official documentation](https://www.flowlauncher.com/docs/)
2. Search existing [GitHub issues](https://github.com/Flow-Launcher/Flow.Launcher/issues)
3. Ask in the [Q&A discussions](https://github.com/Flow-Launcher/Flow.Launcher/discussions/categories/q-a)
4. Join the [Discord community](https://discord.gg/AvgAQgh)

Happy plugin development! 🚀
