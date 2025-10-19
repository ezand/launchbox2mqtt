namespace MqttPlugin.Launchbox.Core.Models;

public class MqttConfig
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 1883;
    public string? Username { get; set; }
    public string? EncryptedPassword { get; set; }
}
