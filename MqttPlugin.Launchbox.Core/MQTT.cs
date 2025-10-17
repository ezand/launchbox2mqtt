using MQTTnet;
using MqttPlugin.Core;

namespace MqttPlugin.Launchbox.Core
{
    public static class MQTT
    {
        private static readonly IMqttClient client;

        static MQTT()
        {
            var mqttFactory = new MqttClientFactory();
            client = mqttFactory.CreateMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("10.193.5.9", 1883)
                .WithCredentials("mosquitto", "mosquitto")
                .Build();

            client.ConnectAsync(mqttClientOptions, CancellationToken.None);
        }

        public static bool IsConnected => client.IsConnected;

        public static void Publish(string topic, string message)
        {
            if (client.IsConnected)
            {
                try
                {
                    client.PublishStringAsync(topic, message);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to publish message: {ex.Message}");
                }
            }
            else
            {
                Logger.Debug("Not connected to MQTT broker");
            }
        }
    }
};
