using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MqttPlugin.Launchbox.Core.Models;

namespace MqttPlugin.Launchbox.Core.Services;

public static class ConfigManager
{
    private static readonly string ConfigFileName = "config.json";

    public static string GetConfigFilePath()
    {
        var pluginFolder = Path.GetDirectoryName(typeof(ConfigManager).Assembly.Location) ?? "";
        return Path.Combine(pluginFolder, ConfigFileName);
    }

    public static MqttConfig LoadConfig()
    {
        var configPath = GetConfigFilePath();

        if (!File.Exists(configPath))
        {
            Logger.Info($"MQTT config file not found at {configPath}, using defaults");
            return new MqttConfig();
        }

        try
        {
            var json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<MqttConfig>(json);
            return config ?? new MqttConfig();
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to load MQTT config: {ex.Message}");
            return new MqttConfig();
        }
    }

    public static bool SaveConfig(MqttConfig config)
    {
        try
        {
            var configPath = GetConfigFilePath();
            var directory = Path.GetDirectoryName(configPath);

            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(configPath, json);
            Logger.Info($"MQTT config saved to {configPath}");
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to save MQTT config: {ex.Message}");
            return false;
        }
    }

    public static string? EncryptPassword(string? password)
    {
        if (string.IsNullOrEmpty(password))
            return null;

        try
        {
            var data = Encoding.UTF8.GetBytes(password);
            var encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to encrypt password: {ex.Message}");
            return null;
        }
    }

    public static string? DecryptPassword(string? encryptedPassword)
    {
        if (string.IsNullOrEmpty(encryptedPassword))
            return null;

        try
        {
            var encrypted = Convert.FromBase64String(encryptedPassword);
            var data = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(data);
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to decrypt password: {ex.Message}");
            return null;
        }
    }
}
