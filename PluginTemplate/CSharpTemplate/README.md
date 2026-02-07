# Flow.Launcher Plugin Template (C#)

A complete C# plugin template for Flow.Launcher with examples of common features.

## Features

- ✅ Basic query handling
- ✅ Settings UI with XAML
- ✅ Context menu (right-click) support
- ✅ Autocomplete example
- ✅ Copy to clipboard functionality
- ✅ Settings persistence
- ✅ Multiple result examples

## Quick Start

### 1. Generate a Unique ID

Run in PowerShell:
```powershell
[guid]::NewGuid().ToString("N").ToUpper()
```

Or use an online GUID generator.

### 2. Update plugin.json

Replace `GENERATE-YOUR-UNIQUE-GUID-HERE` with your generated GUID and update:
- `ActionKeyword`: Your plugin's keyword (e.g., "mp" for MyPlugin)
- `Name`: Your plugin name
- `Description`: What your plugin does
- `Author`: Your name
- `Website`: Your repository URL
- `ExecuteFileName`: Should match your DLL name

### 3. Rename Files

Rename the namespace and files from `PluginTemplate` to your plugin name:
- `Flow.Launcher.Plugin.PluginTemplate.csproj` → `Flow.Launcher.Plugin.YourPlugin.csproj`
- Update namespace in all `.cs` files
- Update `ExecuteFileName` in `plugin.json`

### 4. Build

```bash
dotnet build
```

### 5. Test

Copy the output directory (`bin/Debug/net9.0/`) to:
```
%APPDATA%\FlowLauncher\Plugins\YourPluginName-1.0.0\
```

Restart Flow.Launcher or press `F5` to reload plugins.

### 6. Develop

Implement your plugin logic in `Main.cs`:

- **Query()**: Handle user queries and return results
- **CreateSettingPanel()**: Customize your settings UI
- **LoadContextMenus()**: Add context menu items

## Project Structure

```
CSharpTemplate/
├── Main.cs                     # Main plugin logic
├── Settings.cs                 # Settings model
├── SettingsControl.xaml        # Settings UI
├── SettingsControl.xaml.cs     # Settings UI code-behind
├── plugin.json                 # Plugin metadata
├── Flow.Launcher.Plugin.PluginTemplate.csproj
├── Images/
│   └── icon.png               # Plugin icon (add your own)
└── README.md
```

## Adding an Icon

Add a 256x256 PNG icon to `Images/icon.png`. This will be your plugin's icon in Flow.Launcher.

## Next Steps

1. Remove example results in `Main.cs` and implement your own logic
2. Customize `Settings.cs` with your plugin's settings
3. Update `SettingsControl.xaml` to match your settings
4. Test thoroughly
5. Publish to GitHub
6. Submit to [Flow.Launcher Plugin Store](https://github.com/Flow-Launcher/Flow.Launcher.PluginsManifest)

## Documentation

See the [Plugin Development Guide](../../PLUGIN_DEVELOPMENT.md) for detailed documentation.

## Example Code

### Simple Result
```csharp
results.Add(new Result
{
    Title = "My Result",
    SubTitle = "Description",
    IcoPath = "Images/icon.png",
    Action = e =>
    {
        _context.API.ShowMsg("Title", "Message");
        return true; // true = hide Flow.Launcher window
    }
});
```

### Async Operation
```csharp
public class Main : IAsyncPlugin
{
    public async Task<List<Result>> QueryAsync(Query query, CancellationToken token)
    {
        var data = await FetchDataAsync();
        return CreateResults(data);
    }
}
```

### Open URL
```csharp
Action = e =>
{
    _context.API.OpenAppUri(new Uri("https://example.com"));
    return true;
}
```

## Tips

- Use async/await for long-running operations
- Cache data to improve performance
- Handle exceptions gracefully
- Provide meaningful error messages
- Test with different queries
