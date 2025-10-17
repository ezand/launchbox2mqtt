using Unbroken.LaunchBox.Plugins;

namespace MqttPlugin.Core.PluginInterfaces
{
    public class SystemEvents : ISystemEventsPlugin
    {
        public void OnEventRaised(string eventType)
        {
            Logger.Info($"Event raised: {eventType}");
        }
    }
}
