using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using MqttPlugin.Launchbox.Core;
using MqttPlugin.Launchbox.Core.Services;
using MqttPlugin.Launchbox.Core.UI;
using Unbroken.LaunchBox.Plugins;

namespace MqttPlugin.Core.PluginInterfaces
{
    public class SystemMenuItem : ISystemMenuItemPlugin
    {
        private Image? _iconImage;

        public SystemMenuItem()
        {
            Logger.Info("SystemMenuItem constructor called");
            LoadIcon();
        }

        public string Caption
        {
            get
            {
                return "MQTT Configuration";
            }
        }

        public Image? IconImage => _iconImage;

        private void LoadIcon()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "MqttPlugin.Launchbox.Core.resources.mqtt.png";

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    _iconImage = Image.FromStream(stream);
                    Logger.Info($"Loaded menu icon from embedded resource: {resourceName}");
                }
                else
                {
                    Logger.Info($"Embedded resource not found: {resourceName}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to load menu icon: {ex.Message}", ex);
            }
        }

        public bool ShowInLaunchBox => true;

        public bool ShowInBigBox => false;

        public bool AllowInBigBoxWhenLocked => false;

        public void OnSelected()
        {
            Logger.Info("MQTT Configuration menu item selected");

            try
            {
                var config = ConfigManager.LoadConfig();
                var form = new MqttConfigForm(config);
                var result = form.ShowDialog();

                if (result == DialogResult.OK)
                {
                    Logger.Info("MQTT configuration updated, reloading connection");
                    MQTT.ReloadConfig();
                }

                form.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error showing MQTT config dialog: {ex.Message}", ex);
                MessageBox.Show(
                    $"Error opening configuration dialog:\n\n{ex.Message}\n\nCheck log file for details.",
                    "MQTT Configuration Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}