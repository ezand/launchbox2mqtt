
using System.Diagnostics;
using Unbroken.LaunchBox.Plugins;

namespace MqttPlugin.Core.PluginInterfaces
{
    public class SystemMenuItem : ISystemMenuItemPlugin
    {
        public string Caption => "MQTT Plugin - YAY!";

        public System.Drawing.Image IconImage => null;

        public bool ShowInLaunchBox => true;

        public bool ShowInBigBox => false;

        public bool AllowInBigBoxWhenLocked => false;

        public void OnSelected()
        {
            Debug.WriteLine("YAY!");
        }
    }
}