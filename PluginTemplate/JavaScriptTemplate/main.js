/**
 * Flow.Launcher Plugin Template (JavaScript)
 * 
 * This template demonstrates how to create a JavaScript/Node.js plugin for Flow.Launcher.
 * It includes examples of:
 * - Basic query handling
 * - JSON-RPC actions
 * - Settings management
 * - Context menus
 */

const { Flow } = require('flow-launcher-helper');
const fs = require('fs');
const path = require('path');

class PluginTemplate extends Flow {
    constructor() {
        super();
        this.settings = this.loadSettings();
    }

    /**
     * Handle user queries and return results
     * @param {string} query - The search query entered by the user
     * @returns {Array} Array of result objects
     */
    query(query) {
        const results = [];

        // Example 1: Simple result
        results.push({
            Title: "Hello from JavaScript!",
            SubTitle: `You searched for: ${query}`,
            IcoPath: "Images/icon.png",
            JsonRPCAction: {
                method: "showMessage",
                parameters: ["Hello!", `You searched for: ${query}`]
            }
        });

        // Example 2: Result with autocomplete
        if (query) {
            results.push({
                Title: "Press Tab to autocomplete",
                SubTitle: "This demonstrates autocomplete functionality",
                IcoPath: "Images/icon.png",
                AutoCompleteText: `${this.settings.actionKeyword || 'jstemplate'} autocomplete example`
            });
        }

        // Example 3: Result that copies text
        results.push({
            Title: "Copy this text",
            SubTitle: "Press Ctrl+C or Enter to copy",
            IcoPath: "Images/icon.png",
            CopyText: "This text will be copied!",
            JsonRPCAction: {
                method: "copyToClipboard",
                parameters: ["This text will be copied!"]
            }
        });

        // Example 4: Result that opens URL
        results.push({
            Title: "Open Flow.Launcher website",
            SubTitle: "Press Enter to open in browser",
            IcoPath: "Images/icon.png",
            JsonRPCAction: {
                method: "openUrl",
                parameters: ["https://www.flowlauncher.com"]
            }
        });

        // Example 5: Result with settings check
        if (this.settings.enableExampleFeature) {
            results.push({
                Title: "Example feature enabled!",
                SubTitle: "This shows when the setting is enabled",
                IcoPath: "Images/icon.png",
                JsonRPCAction: {
                    method: "showMessage",
                    parameters: ["Settings", "Example feature is enabled!"]
                }
            });
        }

        // Example 6: Result with context menu
        results.push({
            Title: "Right-click for options",
            SubTitle: "This result has a context menu",
            IcoPath: "Images/icon.png",
            ContextData: "example_context",
            JsonRPCAction: {
                method: "showMessage",
                parameters: ["Info", "You selected the main result"]
            }
        });

        return results;
    }

    /**
     * Handle context menu (right-click menu) for results
     * @param {string} data - The ContextData from the selected result
     * @returns {Array} Array of context menu items
     */
    context_menu(data) {
        if (data === "example_context") {
            return [
                {
                    Title: "Copy Title",
                    SubTitle: "Copy the result title",
                    IcoPath: "Images/icon.png",
                    JsonRPCAction: {
                        method: "copyToClipboard",
                        parameters: ["Right-click for options"]
                    }
                },
                {
                    Title: "Show Details",
                    SubTitle: "Show more information",
                    IcoPath: "Images/icon.png",
                    JsonRPCAction: {
                        method: "showMessage",
                        parameters: ["Details", "This is the context menu!"]
                    }
                }
            ];
        }
        return [];
    }

    // ========== Custom Actions ==========

    /**
     * Show a message dialog
     * @param {string} title - Dialog title
     * @param {string} message - Dialog message
     */
    showMessage(title, message) {
        this.showMsg(title, message);
    }

    /**
     * Copy text to clipboard
     * @param {string} text - Text to copy
     */
    copyToClipboard(text) {
        this.copyText(text);
    }

    /**
     * Open URL in default browser
     * @param {string} url - URL to open
     */
    openUrl(url) {
        const { exec } = require('child_process');
        exec(`start ${url}`);
    }

    // ========== Settings Management ==========

    /**
     * Load plugin settings from settings.json
     * @returns {Object} Settings object
     */
    loadSettings() {
        const settingsPath = path.join(__dirname, 'settings.json');
        
        try {
            if (fs.existsSync(settingsPath)) {
                const data = fs.readFileSync(settingsPath, 'utf8');
                return JSON.parse(data);
            }
        } catch (error) {
            console.error('Error loading settings:', error);
        }
        
        // Return default settings
        return {
            enableExampleFeature: true,
            maxResults: 10,
            exampleText: "Default text"
        };
    }

    /**
     * Save plugin settings to settings.json
     * @param {Object} settings - Settings to save
     */
    saveSettings(settings) {
        const settingsPath = path.join(__dirname, 'settings.json');
        
        try {
            fs.writeFileSync(settingsPath, JSON.stringify(settings, null, 2));
            this.settings = settings;
        } catch (error) {
            console.error('Error saving settings:', error);
        }
    }
}

// Run the plugin
const plugin = new PluginTemplate();
plugin.run();
