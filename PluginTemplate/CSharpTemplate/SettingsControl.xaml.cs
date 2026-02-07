using System.Windows;
using System.Windows.Controls;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.PluginTemplate
{
    /// <summary>
    /// Settings control for the plugin
    /// This will be shown in Flow.Launcher settings
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        private readonly PluginInitContext _context;
        private readonly Settings _settings;

        public SettingsControl(PluginInitContext context, Settings settings)
        {
            _context = context;
            _settings = settings;
            InitializeComponent();
            DataContext = settings;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Save settings
            _context.API.SavePluginSettings();
            MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
