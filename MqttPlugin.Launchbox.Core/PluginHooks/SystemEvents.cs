using MqttPlugin.Launchbox.Core;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;
using System.Text.Json;

namespace MqttPlugin.Core.PluginInterfaces
{
    public class SystemEvents : ISystemEventsPlugin
    {
        public void OnEventRaised(string eventType)
        {
            Logger.Info($"Event raised: {eventType}");
            switch (eventType)
            {
                case SystemEventTypes.LaunchBoxStartupCompleted:
                    MQTT.Publish("launchbox/running", "on");
                    MQTT.Publish("launchbox/details", JsonSerializer.Serialize(new Dictionary<string, string> {
                        { "version", Unbroken.LaunchBox.State.Version },
                        { "id", Unbroken.LaunchBox.State.Id.ToString() },
                        { "machineId", Unbroken.LaunchBox.State.MachineId },
                    }), true);
                    break;
                case SystemEventTypes.BigBoxStartupCompleted:
                    MQTT.Publish("launchbox/bigbox/running", "on");
                    break;
                case SystemEventTypes.BigBoxLocked:
                    MQTT.Publish("launchbox/bigbox/locked", "on");
                    break;
                case SystemEventTypes.BigBoxUnlocked:
                    MQTT.Publish("launchbox/bigbox/locked", "off");
                    break;
                case SystemEventTypes.BigBoxShutdownBeginning:
                    MQTT.Publish("launchbox/bigbox/locked", "off");
                    MQTT.Publish("launchbox/bigbox/running", "off");
                    break;
                case SystemEventTypes.LaunchBoxShutdownBeginning:
                    EventUtils.UnloadContentEvent();
                    MQTT.Publish("launchbox/bigbox/locked", "off");
                    MQTT.Publish("launchbox/bigbox/running", "off");
                    MQTT.Publish("launchbox/running", "off");
                    break;
                default:
                    MQTT.Publish("launchbox/system/event", eventType);
                    break;
            }
        }
    }
}
