using MQTTnet;
using MqttPlugin.Core;
using MqttPlugin.Launchbox.Core.Services;
using Unbroken.LaunchBox.Plugins;

namespace MqttPlugin.Launchbox.Core
{
    public static class MQTT
    {
        private static IMqttClient? client;
        private static MqttClientOptions? clientOptions;
        private static readonly object lockObject = new();

        private static void EnsureInitialized()
        {
            if (client != null)
                return;

            lock (lockObject)
            {
                if (client != null)
                    return;

                var config = ConfigManager.LoadConfig();
                var mqttFactory = new MqttClientFactory();
                client = mqttFactory.CreateMqttClient();

                var optionsBuilder = new MqttClientOptionsBuilder()
                    .WithClientId($"launchbox2mqtt_{Unbroken.LaunchBox.State.MachineId}")
                    .WithTcpServer(config.Host, config.Port);

                if (!string.IsNullOrWhiteSpace(config.Username))
                {
                    var password = ConfigManager.DecryptPassword(config.EncryptedPassword);
                    optionsBuilder.WithCredentials(config.Username, password);
                }

                clientOptions = optionsBuilder.Build();

                // Fire-and-forget connection - logs errors internally
                _ = ConnectAsync();
            }
        }

        private static async Task ConnectAsync()
        {
            if (client == null || clientOptions == null)
                return;

            try
            {
                await client.ConnectAsync(clientOptions, CancellationToken.None);
                Logger.Info("Connected to MQTT broker");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to connect to MQTT broker: {ex.Message}");
            }
        }

        public static void ReloadConfig()
        {
            lock (lockObject)
            {
                if (client != null && client.IsConnected)
                {
                    _ = client.DisconnectAsync();
                }

                client = null;
                clientOptions = null;

                EnsureInitialized();
            }
        }

        public static bool IsConnected
        {
            get
            {
                EnsureInitialized();
                return client?.IsConnected ?? false;
            }
        }

        public static MqttClientOptions? Details
        {
            get
            {
                EnsureInitialized();
                return clientOptions;
            }
        }

        public static string? GetBrokerUrl()
        {
            EnsureInitialized();
            if (clientOptions?.ChannelOptions is MqttClientTcpOptions tcpOptions)
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
            EnsureInitialized();
            if (client?.IsConnected == true)
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
