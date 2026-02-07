#!/usr/bin/env python3
# -*- coding: utf-8 -*-

"""
Flow.Launcher Plugin Template (Python)

This template demonstrates how to create a Python plugin for Flow.Launcher.
It includes examples of:
- Basic query handling
- JSON-RPC actions
- Settings management
- Context menus
"""

from flowlauncher import FlowLauncher
import json
import os


class PluginTemplate(FlowLauncher):
    """
    Main plugin class that extends FlowLauncher
    """

    def __init__(self):
        """Initialize the plugin"""
        super().__init__()
        self.settings = self.load_settings()

    def query(self, query: str) -> list:
        """
        Handle user queries and return results
        
        Args:
            query: The search query entered by the user
            
        Returns:
            List of result dictionaries
        """
        results = []

        # Example 1: Simple result
        results.append({
            "Title": "Hello from Python!",
            "SubTitle": f"You searched for: {query}",
            "IcoPath": "Images/icon.png",
            "JsonRPCAction": {
                "method": "show_message",
                "parameters": ["Hello!", f"You searched for: {query}"]
            }
        })

        # Example 2: Result with autocomplete
        if query:
            results.append({
                "Title": "Press Tab to autocomplete",
                "SubTitle": "This demonstrates autocomplete functionality",
                "IcoPath": "Images/icon.png",
                "AutoCompleteText": f"{self.query_action_keyword} autocomplete example"
            })

        # Example 3: Result that copies text
        results.append({
            "Title": "Copy this text",
            "SubTitle": "Press Ctrl+C or Enter to copy",
            "IcoPath": "Images/icon.png",
            "CopyText": "This text will be copied!",
            "JsonRPCAction": {
                "method": "copy_to_clipboard",
                "parameters": ["This text will be copied!"]
            }
        })

        # Example 4: Result that opens URL
        results.append({
            "Title": "Open Flow.Launcher website",
            "SubTitle": "Press Enter to open in browser",
            "IcoPath": "Images/icon.png",
            "JsonRPCAction": {
                "method": "open_url",
                "parameters": ["https://www.flowlauncher.com"]
            }
        })

        # Example 5: Result with settings check
        if self.settings.get("enable_example_feature", True):
            results.append({
                "Title": "Example feature enabled!",
                "SubTitle": "This shows when the setting is enabled",
                "IcoPath": "Images/icon.png",
                "JsonRPCAction": {
                    "method": "show_message",
                    "parameters": ["Settings", "Example feature is enabled!"]
                }
            })

        # Example 6: Result with context menu
        results.append({
            "Title": "Right-click for options",
            "SubTitle": "This result has a context menu",
            "IcoPath": "Images/icon.png",
            "ContextData": "example_context",
            "JsonRPCAction": {
                "method": "show_message",
                "parameters": ["Info", "You selected the main result"]
            }
        })

        return results

    def context_menu(self, data: str) -> list:
        """
        Handle context menu (right-click menu) for results
        
        Args:
            data: The ContextData from the selected result
            
        Returns:
            List of context menu items
        """
        if data == "example_context":
            return [
                {
                    "Title": "Copy Title",
                    "SubTitle": "Copy the result title",
                    "IcoPath": "Images/icon.png",
                    "JsonRPCAction": {
                        "method": "copy_to_clipboard",
                        "parameters": ["Right-click for options"]
                    }
                },
                {
                    "Title": "Show Details",
                    "SubTitle": "Show more information",
                    "IcoPath": "Images/icon.png",
                    "JsonRPCAction": {
                        "method": "show_message",
                        "parameters": ["Details", "This is the context menu!"]
                    }
                }
            ]
        return []

    # ========== Custom Actions ==========

    def show_message(self, title: str, message: str):
        """Show a message dialog"""
        self.api.show_msg(title, message)

    def copy_to_clipboard(self, text: str):
        """Copy text to clipboard"""
        self.api.copy_to_clipboard(text)

    def open_url(self, url: str):
        """Open URL in default browser"""
        import webbrowser
        webbrowser.open(url)

    # ========== Settings Management ==========

    def load_settings(self) -> dict:
        """Load plugin settings from settings.json"""
        settings_path = os.path.join(
            os.path.dirname(__file__),
            "settings.json"
        )
        
        if os.path.exists(settings_path):
            try:
                with open(settings_path, 'r', encoding='utf-8') as f:
                    return json.load(f)
            except Exception as e:
                print(f"Error loading settings: {e}")
        
        # Return default settings
        return {
            "enable_example_feature": True,
            "max_results": 10,
            "example_text": "Default text"
        }

    def save_settings(self, settings: dict):
        """Save plugin settings to settings.json"""
        settings_path = os.path.join(
            os.path.dirname(__file__),
            "settings.json"
        )
        
        try:
            with open(settings_path, 'w', encoding='utf-8') as f:
                json.dump(settings, f, indent=2)
            self.settings = settings
        except Exception as e:
            print(f"Error saving settings: {e}")


if __name__ == "__main__":
    # Run the plugin
    PluginTemplate()
