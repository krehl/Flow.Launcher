using System.Collections.Generic;
using System.Windows.Controls;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.PluginTemplate
{
    /// <summary>
    /// Main plugin class that implements IPlugin interface.
    /// Additional interfaces can be implemented for more functionality:
    /// - IPluginI18n: Internationalization support
    /// - ISettingProvider: Provide a settings UI
    /// - IContextMenu: Add context menu items
    /// - IAsyncPlugin: Async query support
    /// </summary>
    public class Main : IPlugin, ISettingProvider, IContextMenu
    {
        private PluginInitContext _context;
        private Settings _settings;

        /// <summary>
        /// Initialize plugin with context
        /// Called when plugin is loaded
        /// </summary>
        public void Init(PluginInitContext context)
        {
            _context = context;
            
            // Load plugin settings
            _settings = context.API.LoadSettingJsonStorage<Settings>();
        }

        /// <summary>
        /// Query method called when user types in Flow.Launcher
        /// </summary>
        /// <param name="query">User's search query</param>
        /// <returns>List of results to display</returns>
        public List<Result> Query(Query query)
        {
            var results = new List<Result>();

            // Example: Simple greeting result
            results.Add(new Result
            {
                Title = "Hello World!",
                SubTitle = $"You searched for: {query.Search}",
                IcoPath = "Images/icon.png",
                Action = e =>
                {
                    // Action when user selects this result
                    _context.API.ShowMsg("Hello!", $"You searched for: {query.Search}");
                    return true;
                }
            });

            // Example: Result with autocomplete
            if (!string.IsNullOrEmpty(query.Search))
            {
                results.Add(new Result
                {
                    Title = "Press Tab to autocomplete",
                    SubTitle = "This demonstrates autocomplete functionality",
                    IcoPath = "Images/icon.png",
                    AutoCompleteText = $"{query.ActionKeyword} autocomplete example",
                    Action = e =>
                    {
                        _context.API.ChangeQuery($"{query.ActionKeyword} autocomplete example", true);
                        return false; // Don't hide Flow.Launcher window
                    }
                });
            }

            // Example: Result that copies text
            results.Add(new Result
            {
                Title = "Copy this text",
                SubTitle = "Press Ctrl+C to copy or Enter to copy and close",
                IcoPath = "Images/icon.png",
                CopyText = "This text will be copied!",
                Action = e =>
                {
                    _context.API.CopyToClipboard("This text will be copied!");
                    return true; // Hide Flow.Launcher window
                }
            });

            // Example: Result with settings check
            if (_settings.EnableExampleFeature)
            {
                results.Add(new Result
                {
                    Title = "Example feature enabled!",
                    SubTitle = "This result only shows when the setting is enabled",
                    IcoPath = "Images/icon.png",
                    Action = e =>
                    {
                        _context.API.ShowMsg("Settings", "Example feature is enabled in settings!");
                        return true;
                    }
                });
            }

            return results;
        }

        /// <summary>
        /// Provide settings UI for the plugin
        /// </summary>
        public Control CreateSettingPanel()
        {
            return new SettingsControl(_context, _settings);
        }

        /// <summary>
        /// Load context menu for a selected result (right-click menu)
        /// </summary>
        public List<Result> LoadContextMenus(Result selectedResult)
        {
            var contextMenus = new List<Result>();

            contextMenus.Add(new Result
            {
                Title = "Copy Title",
                SubTitle = "Copy the result title to clipboard",
                IcoPath = "Images/icon.png",
                Action = e =>
                {
                    _context.API.CopyToClipboard(selectedResult.Title);
                    return true;
                }
            });

            contextMenus.Add(new Result
            {
                Title = "Show Details",
                SubTitle = "Show more information about this result",
                IcoPath = "Images/icon.png",
                Action = e =>
                {
                    _context.API.ShowMsg("Details", $"Title: {selectedResult.Title}\nSubTitle: {selectedResult.SubTitle}");
                    return true;
                }
            });

            return contextMenus;
        }
    }
}
