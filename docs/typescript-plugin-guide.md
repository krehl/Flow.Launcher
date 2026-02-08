# Creating a TypeScript Plugin for Flow Launcher

This guide walks through building a Flow Launcher plugin using TypeScript, covering both the legacy V1 protocol and the modern V2 protocol.

## Overview

Flow Launcher plugins written in TypeScript communicate with the launcher via JSON-RPC. There are two protocol versions:

| Feature | V1 (`TypeScript`) | V2 (`TypeScript_V2`) |
|---|---|---|
| Communication | Command-line args + stdout | Persistent stdin/stdout stream |
| Message format | Single JSON blob per invocation | Header-delimited JSON-RPC |
| Process model | New process per request | Long-running process |
| Flow Launcher API | Via JSON response actions | Direct bidirectional RPC calls |

For new plugins, **V2 is recommended** when you need persistent state or want to call Flow Launcher APIs directly. V1 is simpler and works well for stateless plugins.

## Prerequisites

- Flow Launcher installed (it bundles Node.js v16.18.0 automatically)
- Node.js and npm installed locally for development
- TypeScript (`npm install -g typescript`)

## Project Structure

```
Flow.Launcher.Plugin.MyPlugin/
├── .github/
│   └── workflows/
│       └── publish-release.yml   # CI: builds & publishes releases
├── src/
│   └── main.ts                   # TypeScript source
├── dist/
│   └── main.js                   # Compiled JS (build output)
├── Images/
│   └── icon.png                  # Plugin icon
├── node_modules/                 # Dependencies (included in release zip)
├── plugin.json                   # Plugin manifest (required)
├── package.json                  # Node.js project config
├── tsconfig.json                 # TypeScript config
└── SettingsTemplate.yaml         # Optional: plugin settings UI
```

## Step 1: Initialize the Project

```bash
mkdir Flow.Launcher.Plugin.MyPlugin
cd Flow.Launcher.Plugin.MyPlugin
npm init -y
npm install typescript @types/node --save-dev
```

## Step 2: Configure TypeScript

Create `tsconfig.json`:

```json
{
  "compilerOptions": {
    "strict": true,
    "module": "commonjs",
    "target": "es6",
    "esModuleInterop": true,
    "moduleResolution": "node",
    "rootDir": "./src",
    "outDir": "./dist",
    "sourceMap": true,
    "skipLibCheck": true,
    "forceConsistentCasingInFileNames": true
  },
  "exclude": ["node_modules"]
}
```

## Step 3: Create plugin.json

This manifest tells Flow Launcher how to load your plugin. It must be in the plugin root directory.

```json
{
  "$schema": "https://www.flowlauncher.com/schemas/plugin.schema.json",
  "ID": "YOUR-UNIQUE-32-BIT-UUID",
  "ActionKeyword": "myplugin",
  "Name": "My Plugin",
  "Description": "A TypeScript plugin for Flow Launcher",
  "Author": "Your Name",
  "Version": "1.0.0",
  "Language": "TypeScript",
  "Website": "https://github.com/you/Flow.Launcher.Plugin.MyPlugin",
  "ExecuteFileName": "./dist/main.js",
  "IcoPath": "Images\\icon.png"
}
```

**Key fields:**

| Field | Description |
|---|---|
| `ID` | A unique UUID. Generate one at https://www.uuidgenerator.net/ |
| `ActionKeyword` | The keyword that triggers your plugin. Use `*` for global (no keyword needed) |
| `Language` | `TypeScript` for V1, `TypeScript_V2` for V2 |
| `ExecuteFileName` | Path to the compiled JS entry point |
| `IcoPath` | Relative path to the plugin icon |

## Step 4: Write the Plugin (V1 Protocol)

The V1 protocol is the simpler approach. Flow Launcher passes a JSON-RPC request as a command-line argument, and your plugin writes a JSON response to stdout.

Create `src/main.ts`:

```typescript
// Parse the JSON-RPC request from command-line args
const request: { method: string; parameters: string[]; settings: Record<string, string> } =
  JSON.parse(process.argv[2]);

const { method, parameters, settings } = request;

if (method === "query") {
  const searchQuery = parameters[0] || "";

  const results = [
    {
      Title: `Hello from TypeScript!`,
      Subtitle: `You searched: ${searchQuery}`,
      IcoPath: "Images\\icon.png",
      Score: 100,
      JsonRPCAction: {
        method: "open_url",
        parameters: ["https://github.com/Flow-Launcher/Flow.Launcher"],
        dontHideAfterAction: false,
      },
    },
  ];

  // Write the response to stdout
  console.log(JSON.stringify({ result: results }));
}

if (method === "open_url") {
  const url = parameters[0];
  // Use dynamic import for ESM-only packages, or require for CommonJS
  const open = require("./node_modules/open");
  open(url);
}
```

### V1 Result Object Shape

Each result in the `result` array can have these properties:

```typescript
interface Result {
  Title: string;              // Required: main display text
  Subtitle?: string;          // Additional details shown below title
  IcoPath?: string;           // Icon path relative to plugin folder
  Score?: number;             // Priority (0-100, higher = shown first)
  TitleToolTip?: string;      // Hover text for title
  SubTitleToolTip?: string;   // Hover text for subtitle
  ContextData?: any[];        // Data passed to context_menu method
  JsonRPCAction?: {
    method: string;           // Method to call when user selects this result
    parameters: any[];        // Parameters for that method
    dontHideAfterAction?: boolean;
  };
}
```

### V1 Calling Flow Launcher APIs

In V1, you invoke Flow Launcher APIs by returning them as the response method:

```typescript
// Change the query in the search box
console.log(JSON.stringify({
  method: "Flow.Launcher.ChangeQuery",
  parameters: ["new query", true],
}));

// Copy text to clipboard
console.log(JSON.stringify({
  method: "Flow.Launcher.CopyToClipboard",
  parameters: ["text to copy"],
}));

// Show a message
console.log(JSON.stringify({
  method: "Flow.Launcher.ShowMsg",
  parameters: ["Title", "Subtitle", "Images\\icon.png"],
}));
```

## Step 5: Write the Plugin (V2 Protocol)

The V2 protocol uses a persistent process with header-delimited JSON-RPC over stdin/stdout (via the StreamJsonRpc library). This enables bidirectional communication: Flow Launcher calls your methods, and you can call Flow Launcher APIs directly.

Set `"Language": "TypeScript_V2"` in your `plugin.json`.

For V2 plugins, you need to implement a JSON-RPC server over stdin/stdout using the header-delimited message format. Install `vscode-jsonrpc` as a dependency:

```bash
npm install vscode-jsonrpc
```

Create `src/main.ts`:

```typescript
import {
  createMessageConnection,
  StreamMessageReader,
  StreamMessageWriter,
  RequestType,
  NotificationType,
} from "vscode-jsonrpc/node";

// Create a JSON-RPC connection over stdin/stdout
const connection = createMessageConnection(
  new StreamMessageReader(process.stdin),
  new StreamMessageWriter(process.stdout)
);

// Store context from initialization
let pluginContext: any = null;

// --- Methods Flow Launcher will call on your plugin ---

// Called once when the plugin is loaded
connection.onRequest(new RequestType<any, void, void>("initialize"), (context) => {
  pluginContext = context;
});

// Called every time the user types in the search box (with your action keyword)
connection.onRequest(
  new RequestType<[any, Record<string, string>], any, void>("query"),
  ([query, settings]) => {
    const search = query.Search || "";

    return {
      result: [
        {
          Title: `Hello from TypeScript V2!`,
          Subtitle: `You searched: ${search}`,
          IcoPath: "Images\\icon.png",
          Score: 100,
          JsonRPCAction: {
            method: "handle_selection",
            parameters: ["https://github.com/Flow-Launcher/Flow.Launcher"],
            dontHideAfterAction: false,
          },
        },
      ],
    };
  }
);

// Called when a user selects a result
connection.onRequest(
  new RequestType<string[], { hide: boolean }, void>("handle_selection"),
  (params) => {
    const url = params[0];
    require("open")(url);
    return { hide: true };
  }
);

// Called when the user opens the context menu on a result
connection.onRequest(
  new RequestType<any, any, void>("context_menu"),
  (contextData) => {
    return {
      result: [
        {
          Title: "Copy to clipboard",
          IcoPath: "Images\\icon.png",
          JsonRPCAction: {
            method: "copy_result",
            parameters: [contextData],
          },
        },
      ],
    };
  }
);

// --- Flow Launcher APIs you can call (V2 only) ---

// Example: call Flow Launcher's CopyToClipboard API
connection.onRequest(
  new RequestType<string[], { hide: boolean }, void>("copy_result"),
  async (params) => {
    await connection.sendRequest("CopyToClipboard", [params[0]]);
    return { hide: true };
  }
);

// Start listening
connection.listen();
```

### V2 Available Flow Launcher API Methods

In V2, your plugin can call these methods on the Flow Launcher host:

| Method | Parameters | Description |
|---|---|---|
| `ChangeQuery` | `(query, requery?)` | Update the search box |
| `CopyToClipboard` | `(text, directCopy?, showNotification?)` | Copy text |
| `ShellRun` | `(cmd, filename?)` | Run a shell command |
| `ShowMsg` | `(title, subtitle?, iconPath?)` | Show a notification |
| `OpenUrl` | `(url, inPrivate?)` | Open a URL in browser |
| `OpenDirectory` | `(path, fileNameOrPath?)` | Open a folder |
| `RestartApp` | none | Restart Flow Launcher |
| `SavePluginSettings` | none | Persist plugin settings |
| `ReloadAllPluginDataAsync` | none | Reload all plugins |
| `FuzzySearch` | `(query, stringToCompare)` | Use Flow's fuzzy matching |
| `HttpGetStringAsync` | `(url)` | HTTP GET returning string |
| `HttpGetStreamAsync` | `(url)` | HTTP GET returning stream |
| `HttpDownloadAsync` | `(url, filePath)` | Download a file |
| `LogDebug/Info/Warn/Error` | `(className, message)` | Write to Flow's log |
| `StartLoadingBar` / `StopLoadingBar` | none | Toggle loading indicator |
| `ShowMainWindow` / `HideMainWindow` | none | Toggle Flow window |
| `BackToQueryResults` | none | Navigate back to results |

## Step 6: Build

Add build scripts to `package.json`:

```json
{
  "scripts": {
    "build": "tsc",
    "dev": "tsc --watch"
  }
}
```

Build with:

```bash
npm run build
```

## Step 7: Test Locally

1. Build your plugin: `npm run build`
2. Copy or symlink your entire plugin folder into Flow Launcher's plugin directory:
   ```
   %APPDATA%\FlowLauncher\Plugins\Flow.Launcher.Plugin.MyPlugin\
   ```
3. Restart Flow Launcher (type `restart flow launcher` in the search box)
4. Type your action keyword to test

## Step 8: Plugin Settings (Optional)

Create a `SettingsTemplate.yaml` in your plugin root to define a settings UI:

```yaml
body:
  - type: input
    attributes:
      name: apiKey
      label: API Key
      description: Enter your API key
      defaultValue: ""
  - type: dropdown
    attributes:
      name: resultCount
      label: Number of results
      description: How many results to show
      defaultValue: "10"
      options:
        - "5"
        - "10"
        - "20"
  - type: checkbox
    attributes:
      name: openInBrowser
      label: Open in browser
      description: Open results in the default browser
      defaultValue: "true"
```

Settings are passed to your plugin in every `query` call:
- **V1:** `request.settings` in the parsed command-line argument
- **V2:** Second parameter of the `query` method

## Step 9: Set Up GitHub Actions for Releases

Users should not have to run `npm install` themselves. Use GitHub Actions to bundle `node_modules` in your release.

Create `.github/workflows/publish-release.yml`:

```yaml
name: Publish Release

on:
  workflow_dispatch:
  push:
    branches: [main]
    paths-ignore:
      - .github/workflows/*

jobs:
  publish:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: "16"

      - name: Install dependencies
        run: npm install

      - name: Build
        run: npm run build

      - name: Get version
        id: version
        run: |
          echo "version=$(cat plugin.json | python -c "import sys,json; print(json.load(sys.stdin)['Version'])")" >> $GITHUB_OUTPUT

      - name: Package
        run: |
          zip -r "Flow.Launcher.Plugin.MyPlugin.zip" . \
            -x ".git/*" ".github/*" "src/*" "tsconfig.json" ".eslintrc" ".prettierrc" "*.ts"

      - name: Publish
        uses: softprops/action-gh-release@v1
        with:
          files: "Flow.Launcher.Plugin.MyPlugin.zip"
          tag_name: "v${{ steps.version.outputs.version }}"
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

## Step 10: Publish to the Plugin Store

When ready, submit your plugin to the [Flow Launcher Plugin Manifest](https://github.com/Flow-Launcher/Flow.Launcher.PluginsManifest) repository by following the instructions in its README.

## Reference Projects

- **Official Hello World sample:** https://github.com/Flow-Launcher/Flow.Launcher.Plugin.HelloWorldNodeJS
- **Community plugin template (TypeScript):** https://github.com/Joehoel/flow-launcher-plugin-template-node
- **NPM Search plugin:** https://github.com/gabrielcarloto/flow-search-npm
- **Discord Timestamps plugin:** https://github.com/Jessuhh/discord-timestamps-flowlauncher-plugin

## Relevant Source Code in Flow.Launcher

For deeper understanding, these files in the Flow.Launcher codebase are relevant:

- `Flow.Launcher.Plugin/AllowedLanguage.cs` - Supported language identifiers
- `Flow.Launcher.Core/Plugin/NodePluginV2.cs` - V2 Node.js plugin process management
- `Flow.Launcher.Core/Plugin/JsonRPCPluginV2.cs` - V2 JSON-RPC protocol implementation
- `Flow.Launcher.Core/Plugin/JsonPRCModel.cs` - JSON-RPC data models
- `Flow.Launcher.Core/Plugin/JsonRPCV2Models/JsonRPCPublicAPI.cs` - APIs exposed to V2 plugins
- `Flow.Launcher.Core/ExternalPlugins/Environments/TypeScriptV2Environment.cs` - TypeScript V2 environment setup
