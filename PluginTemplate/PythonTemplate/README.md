# Flow.Launcher Plugin Template (Python)

A complete Python plugin template for Flow.Launcher with examples of common features.

## Features

- ✅ Basic query handling
- ✅ JSON-RPC actions
- ✅ Context menu (right-click) support
- ✅ Settings persistence
- ✅ Autocomplete example
- ✅ Copy to clipboard
- ✅ Open URLs
- ✅ Multiple result examples

## Prerequisites

- Python 3.6 or higher
- pip (Python package manager)

## Quick Start

### 1. Install Dependencies

```bash
pip install -r requirements.txt
```

Or manually:
```bash
pip install flowlauncher
```

### 2. Generate a Unique ID

Run in PowerShell:
```powershell
[guid]::NewGuid().ToString("N").ToUpper()
```

Or use Python:
```python
import uuid
print(uuid.uuid4().hex.upper())
```

### 3. Update plugin.json

Replace `GENERATE-YOUR-UNIQUE-GUID-HERE` with your generated GUID and update:
- `ActionKeyword`: Your plugin's keyword (e.g., "mp" for MyPlugin)
- `Name`: Your plugin name
- `Description`: What your plugin does
- `Author`: Your name
- `Website`: Your repository URL

### 4. Test

Copy the entire plugin folder to:
```
%APPDATA%\FlowLauncher\Plugins\YourPluginName-1.0.0\
```

Restart Flow.Launcher or press `F5` to reload plugins.

### 5. Use in Flow.Launcher

Type your action keyword (e.g., `pytemplate`) followed by your query.

## Project Structure

```
PythonTemplate/
├── main.py              # Main plugin logic
├── plugin.json          # Plugin metadata
├── requirements.txt     # Python dependencies
├── settings.json        # Plugin settings
├── Images/
│   └── icon.png        # Plugin icon (add your own)
└── README.md
```

## Development

### Main Plugin Class

The `PluginTemplate` class extends `FlowLauncher` and implements these methods:

- **query(query)**: Handle user queries and return results
- **context_menu(data)**: Provide context menu items (right-click)
- **Custom methods**: Any method can be called via JsonRPCAction

### Example: Adding a New Result

```python
def query(self, query: str) -> list:
    results = []
    
    results.append({
        "Title": "My Result",
        "SubTitle": "Description",
        "IcoPath": "Images/icon.png",
        "JsonRPCAction": {
            "method": "my_custom_action",
            "parameters": ["param1", "param2"]
        }
    })
    
    return results

def my_custom_action(self, param1: str, param2: str):
    """This method will be called when the result is selected"""
    self.api.show_msg("Title", f"Got: {param1}, {param2}")
```

### Example: Async Operations

For long-running operations, use async/await:

```python
import asyncio

async def fetch_data_async(self):
    # Simulate async operation
    await asyncio.sleep(1)
    return "data"

def query(self, query: str) -> list:
    # Run async operation
    data = asyncio.run(self.fetch_data_async())
    
    return [{
        "Title": data,
        "SubTitle": "Async result",
        "IcoPath": "Images/icon.png"
    }]
```

### Example: Using External APIs

```python
import requests

def query(self, query: str) -> list:
    try:
        response = requests.get(f"https://api.example.com/search?q={query}")
        data = response.json()
        
        results = []
        for item in data:
            results.append({
                "Title": item["title"],
                "SubTitle": item["description"],
                "IcoPath": "Images/icon.png"
            })
        
        return results
    except Exception as e:
        return [{
            "Title": "Error",
            "SubTitle": str(e),
            "IcoPath": "Images/icon.png"
        }]
```

## Settings

Settings are stored in `settings.json` and automatically loaded when the plugin starts.

To update settings from your plugin:

```python
new_settings = {
    "enable_example_feature": False,
    "max_results": 20
}
self.save_settings(new_settings)
```

## API Methods

The `self.api` object provides access to Flow.Launcher's API:

```python
# Show message dialog
self.api.show_msg("Title", "Message")

# Copy to clipboard
self.api.copy_to_clipboard("text")

# Change query
self.api.change_query("new query", requery=True)

# Hide/show Flow.Launcher
self.api.hide_app()
self.api.show_app()
```

## Adding an Icon

Add a 256x256 PNG icon to `Images/icon.png`. This will be your plugin's icon in Flow.Launcher.

## Debugging

To debug your plugin, you can:

1. Add print statements (output will appear in Flow.Launcher logs)
2. Use Python's logging module
3. Check Flow.Launcher logs: Type `open log location` in Flow.Launcher

Example logging:

```python
import logging

logging.basicConfig(level=logging.DEBUG)
logger = logging.getLogger(__name__)

def query(self, query: str) -> list:
    logger.debug(f"Query received: {query}")
    # ...
```

## Publishing

1. Create a GitHub repository
2. Push your plugin code
3. Create a release with a ZIP file containing:
   - `main.py`
   - `plugin.json`
   - `requirements.txt`
   - `Images/` folder
   - Any other necessary files
4. Submit to [Flow.Launcher Plugin Store](https://github.com/Flow-Launcher/Flow.Launcher.PluginsManifest)

## Documentation

See the [Plugin Development Guide](../../PLUGIN_DEVELOPMENT.md) for detailed documentation.

## Tips

- Use type hints for better code clarity
- Handle exceptions gracefully
- Provide meaningful error messages
- Test with different queries
- Keep the plugin responsive (avoid blocking operations)
- Use async for I/O operations
- Cache data when appropriate
