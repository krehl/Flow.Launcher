# Flow.Launcher Plugin Template (JavaScript)

A complete JavaScript/Node.js plugin template for Flow.Launcher with examples of common features.

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

- Node.js 14+ (recommended: latest LTS version)
- npm (comes with Node.js)

## Quick Start

### 1. Install Dependencies

```bash
npm install
```

### 2. Generate a Unique ID

Run in PowerShell:
```powershell
[guid]::NewGuid().ToString("N").ToUpper()
```

Or use Node.js:
```javascript
console.log(require('crypto').randomUUID().replace(/-/g, '').toUpperCase());
```

### 3. Update plugin.json

Replace `GENERATE-YOUR-UNIQUE-GUID-HERE` with your generated GUID and update:
- `ActionKeyword`: Your plugin's keyword (e.g., "mp" for MyPlugin)
- `Name`: Your plugin name
- `Description`: What your plugin does
- `Author`: Your name
- `Website`: Your repository URL

### 4. Update package.json

Update the following fields:
- `name`: Your plugin package name
- `description`: What your plugin does
- `author`: Your name

### 5. Test

Copy the entire plugin folder to:
```
%APPDATA%\FlowLauncher\Plugins\YourPluginName-1.0.0\
```

Restart Flow.Launcher or press `F5` to reload plugins.

### 6. Use in Flow.Launcher

Type your action keyword (e.g., `jstemplate`) followed by your query.

## Project Structure

```
JavaScriptTemplate/
├── main.js              # Main plugin logic
├── plugin.json          # Plugin metadata
├── package.json         # Node.js package info
├── settings.json        # Plugin settings
├── Images/
│   └── icon.png        # Plugin icon (add your own)
└── README.md
```

## Development

### Main Plugin Class

The `PluginTemplate` class extends `Flow` and implements these methods:

- **query(query)**: Handle user queries and return results
- **context_menu(data)**: Provide context menu items (right-click)
- **Custom methods**: Any method can be called via JsonRPCAction

### Example: Adding a New Result

```javascript
query(query) {
    const results = [];
    
    results.push({
        Title: "My Result",
        SubTitle: "Description",
        IcoPath: "Images/icon.png",
        JsonRPCAction: {
            method: "myCustomAction",
            parameters: ["param1", "param2"]
        }
    });
    
    return results;
}

myCustomAction(param1, param2) {
    // This method will be called when the result is selected
    this.showMsg("Title", `Got: ${param1}, ${param2}`);
}
```

### Example: Async Operations

For long-running operations, use async/await:

```javascript
async fetchDataAsync() {
    const response = await fetch('https://api.example.com/data');
    return await response.json();
}

query(query) {
    // Note: Flow.Launcher helper handles async automatically
    const results = [];
    
    // For async operations, you can use promises
    this.fetchDataAsync().then(data => {
        // Process data
    }).catch(error => {
        console.error(error);
    });
    
    return results;
}
```

### Example: Using External APIs

```javascript
const axios = require('axios');

async query(query) {
    try {
        const response = await axios.get(`https://api.example.com/search?q=${query}`);
        const data = response.data;
        
        const results = data.map(item => ({
            Title: item.title,
            SubTitle: item.description,
            IcoPath: "Images/icon.png"
        }));
        
        return results;
    } catch (error) {
        return [{
            Title: "Error",
            SubTitle: error.message,
            IcoPath: "Images/icon.png"
        }];
    }
}
```

## Settings

Settings are stored in `settings.json` and automatically loaded when the plugin starts.

To update settings from your plugin:

```javascript
const newSettings = {
    enableExampleFeature: false,
    maxResults: 20
};
this.saveSettings(newSettings);
```

## API Methods

The Flow helper provides several useful methods:

```javascript
// Show message dialog
this.showMsg("Title", "Message");

// Copy to clipboard
this.copyText("text");

// Change query
this.changeQuery("new query", true);

// Hide Flow.Launcher
this.hideApp();

// Show Flow.Launcher
this.showApp();
```

## Adding Dependencies

To add external npm packages:

```bash
npm install package-name --save
```

Then require them in your code:

```javascript
const packageName = require('package-name');
```

## Adding an Icon

Add a 256x256 PNG icon to `Images/icon.png`. This will be your plugin's icon in Flow.Launcher.

## Debugging

To debug your plugin:

1. Add console.log statements (output will appear in Flow.Launcher logs)
2. Check Flow.Launcher logs: Type `open log location` in Flow.Launcher
3. Use Node.js debugging tools

Example logging:

```javascript
query(query) {
    console.log('Query received:', query);
    // ...
}
```

## TypeScript Support

To use TypeScript instead of JavaScript:

1. Install TypeScript:
```bash
npm install --save-dev typescript @types/node
```

2. Create `tsconfig.json`:
```json
{
  "compilerOptions": {
    "target": "ES2020",
    "module": "commonjs",
    "outDir": "./dist",
    "strict": true,
    "esModuleInterop": true
  },
  "include": ["*.ts"]
}
```

3. Rename `main.js` to `main.ts` and update `plugin.json`:
```json
{
  "Language": "typescript",
  "ExecuteFileName": "main.ts"
}
```

## Publishing

1. Create a GitHub repository
2. Push your plugin code
3. Create a release with a ZIP file containing:
   - `main.js`
   - `plugin.json`
   - `package.json`
   - `node_modules/` folder (or list in package.json)
   - `Images/` folder
   - Any other necessary files
4. Submit to [Flow.Launcher Plugin Store](https://github.com/Flow-Launcher/Flow.Launcher.PluginsManifest)

**Note**: Some developers prefer to exclude `node_modules/` and have users run `npm install`. If you do this, document it clearly in your README.

## Documentation

See the [Plugin Development Guide](../../PLUGIN_DEVELOPMENT.md) for detailed documentation.

## Tips

- Use modern JavaScript features (ES6+)
- Handle errors gracefully with try-catch
- Provide meaningful error messages
- Test with different queries
- Keep the plugin responsive
- Use async/await for I/O operations
- Cache data when appropriate
- Minimize dependencies to reduce plugin size

## Common Issues

### Plugin not loading
- Ensure Node.js is installed and in your PATH
- Check `plugin.json` syntax
- Verify `npm install` completed successfully

### Module not found errors
- Run `npm install` in the plugin directory
- Check that the module is listed in `package.json`

### Plugin crashes
- Check Flow.Launcher logs for error messages
- Add error handling around risky operations
- Test the plugin outside Flow.Launcher with `node main.js`
