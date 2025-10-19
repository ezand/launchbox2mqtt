using System.Windows.Forms;
using MqttPlugin.Launchbox.Core;
using MqttPlugin.Launchbox.Core.Services;
using MqttPlugin.Launchbox.Core.UI;
using Unbroken.LaunchBox.Plugins;

namespace MqttPlugin.Core.PluginInterfaces
{
    public class SystemMenuItem : ISystemMenuItemPlugin
    {
        public SystemMenuItem()
        {
            Logger.Info("SystemMenuItem constructor called");
        }

        public string Caption
        {
            get
            {
                return "MQTT Configuration";
            }
        }

        public Image? IconImage => null;

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