using MQTTnet;
using MqttPlugin.Core;

namespace MqttPlugin.Launchbox.Core
{
    public static class MQTT
    {
        private static readonly IMqttClient client;
        private static readonly MqttClientOptions clientOptions;

        static MQTT()
        {
            var mqttFactory = new MqttClientFactory();
            client = mqttFactory.CreateMqttClient();

            clientOptions = new MqttClientOptionsBuilder()
                .WithClientId($"launchbox2mqtt_{Guid.NewGuid()}")
                .WithTcpServer("192.168.144.212", 1883)
                .WithCredentials("mosquitto", "mosquitto")
                .Build();

            // Fire-and-forget connection - logs errors internally
            _ = client.ConnectAsync(clientOptions, CancellationToken.None);
        }

        public static bool IsConnected => client.IsConnected;
        public static MqttClientOptions Details => clientOptions;

        public static string? GetBrokerUrl()
        {
            if (clientOptions.ChannelOptions is MqttClientTcpOptions tcpOptions)
            {
                if (tcpOptions.RemoteEndpoint is System.Net.IPEndPoint ipEndpoint)
                {
                    return $"tcp://{ipEndpoint.Address}:{ipEndpoint.Port}";
                }
                // Fallback for DnsEndPoint or other endpoint types
                return $"tcp://{tcpOptions.RemoteEndpoint}";
            }
            return null;
        }

        public static void Publish(string topic, string message, bool retain = false)
        {
            if (client.IsConnected)
            {
                try
                {
                    client.PublishStringAsync(topic, message, retain: retain);
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
