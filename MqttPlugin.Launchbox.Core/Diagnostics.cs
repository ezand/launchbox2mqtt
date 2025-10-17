namespace MqttPlugin.Launchbox.Core
{
    public static class Diagnostics
    {
        public static string GetDiagnostics()
        {
            var diagnostics = new System.Text.StringBuilder();
            diagnostics.AppendLine($"=== MQTT Plugin Diagnostics [{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ===");
            diagnostics.AppendLine($"Log Path: {Logger.LogFilePath}");
            diagnostics.AppendLine($"BaseDirectory: {AppDomain.CurrentDomain.BaseDirectory}");

            if (MQTT.Details != null)
            {
                diagnostics.AppendLine($"MQTT ClientId: {MQTT.Details.ClientId}");
                diagnostics.AppendLine($"MQTT Username: {MQTT.Details.Credentials?.GetUserName(MQTT.Details)}");
                diagnostics.AppendLine($"MQTT URL: {MQTT.GetBrokerUrl()}");
            }
            else
            {
                diagnostics.AppendLine("MQTT Details: Not initialized");
            }

            diagnostics.AppendLine($"MQTT Broker Connected: {MQTT.IsConnected}");
            diagnostics.AppendLine();
            return diagnostics.ToString();
        }
    }
}