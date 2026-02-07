namespace Flow.Launcher.Plugin.PluginTemplate
{
    /// <summary>
    /// Plugin settings class
    /// This will be automatically saved/loaded as JSON
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Example boolean setting
        /// </summary>
        public bool EnableExampleFeature { get; set; } = true;

        /// <summary>
        /// Example string setting
        /// </summary>
        public string ExampleText { get; set; } = "Default text";

        /// <summary>
        /// Example integer setting
        /// </summary>
        public int MaxResults { get; set; } = 10;
    }
}
