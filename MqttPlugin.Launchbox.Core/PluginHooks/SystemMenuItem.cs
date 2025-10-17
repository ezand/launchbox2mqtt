using Unbroken.LaunchBox.Plugins;

namespace MqttPlugin.Core.PluginInterfaces
{
    public class SystemMenuItem : ISystemMenuItemPlugin
    {
        public SystemMenuItem()
        {
            Logger.Info("SystemMenuItem constructor called");
        }

        public string Caption => "MQTT configuration";

        public System.Drawing.Image? IconImage => null;

        public bool ShowInLaunchBox => true;

        public bool ShowInBigBox => false;

        public bool AllowInBigBoxWhenLocked => false;

        public void OnSelected()
        {
            Logger.Info("MQTT Configuration menu item selected");
            Logger.ShowDiagnostics();
        }
    }
}