namespace MqttPlugin.Launchbox.Core
{
    public static class EventUtils
    {
        public static void UnloadContentEvent()
        {
            MQTT.Publish("launchbox/emulator/loaded", "off");
            MQTT.Publish("launchbox/emulator/running", "off");
            MQTT.Publish("launchbox/emulator", "");

            MQTT.Publish("launchbox/content/loaded", "off");
            MQTT.Publish("launchbox/content/running", "off");
            MQTT.Publish("launchbox/content", "");
        }
    }
}