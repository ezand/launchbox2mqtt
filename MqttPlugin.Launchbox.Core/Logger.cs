using System.Diagnostics;
using MqttPlugin.Launchbox.Core;

namespace MqttPlugin.Core;

public static class Logger
{
    private static readonly string LogFilePath;
    private static readonly object LockObject = new();
    private static string? InitError = null;

    static Logger()
    {
        try
        {
            // AppDomain.CurrentDomain.BaseDirectory points to LaunchBox/Core
            // Go up one level to LaunchBox root, then into Logs
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var launchBoxRoot = Directory.GetParent(Directory.GetParent(baseDir)?.FullName).FullName ?? baseDir;
            var logsDir = Path.Combine(launchBoxRoot, "Logs");
            Directory.CreateDirectory(logsDir);
            LogFilePath = Path.Combine(logsDir, "launchbox-mqtt-debug.log");

            // Test write to verify we can actually log
            try
            {
                File.AppendAllText(LogFilePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Logger initialized at: {LogFilePath}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                InitError = $"Failed to write to log: {ex.Message}";
            }
        }
        catch (Exception ex)
        {
            InitError = $"Failed to initialize logger: {ex.Message}";
            LogFilePath = "launchbox-mqtt-debug.log";
        }
    }

    public static void Info(string message)
    {
        Log("INFO", message);
    }

    public static void Error(string message, Exception? ex = null)
    {
        var fullMessage = ex != null ? $"{message}: {ex.Message}\n{ex.StackTrace}" : message;
        Log("ERROR", fullMessage);
    }

    public static void Debug(string message)
    {
        Log("DEBUG", message);
    }

    public static void ShowDiagnostics()
    {
        // Write comprehensive diagnostics to a dedicated file
        var diagPath = Path.Combine(Path.GetDirectoryName(LogFilePath) ?? ".", "diagnostics.txt");
        var diagnostics = new System.Text.StringBuilder();

        diagnostics.AppendLine($"=== Logger Diagnostics [{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ===");
        diagnostics.AppendLine($"Log Path: {LogFilePath}");
        diagnostics.AppendLine($"BaseDirectory: {AppDomain.CurrentDomain.BaseDirectory}");
        diagnostics.AppendLine($"File Exists: {File.Exists(LogFilePath)}");
        diagnostics.AppendLine($"Init Error: {InitError ?? "None"}");
        diagnostics.AppendLine($"MQTT Broker Connected: {MQTT.IsConnected}");
        diagnostics.AppendLine();

        try
        {
            File.AppendAllText(LogFilePath, $"[TEST] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} Manual test write{Environment.NewLine}");
            diagnostics.AppendLine("Test write: SUCCESS");
        }
        catch (Exception ex)
        {
            diagnostics.AppendLine($"Test write FAILED: {ex.Message}");
            diagnostics.AppendLine($"Stack trace: {ex.StackTrace}");
        }

        try
        {
            File.WriteAllText(diagPath, diagnostics.ToString());
        }
        catch
        {
            // Last resort - write to temp
            try
            {
                var tempPath = Path.Combine(Path.GetTempPath(), "launchbox-mqtt-diagnostics.txt");
                File.WriteAllText(tempPath, diagnostics.ToString());
            }
            catch
            {
                // Nowhere to write
            }
        }
    }

    private static void Log(string level, string message)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var logEntry = $"[{timestamp}] [{level}] {message}";

        // Output to Debug (visible in DebugView or Visual Studio)
        System.Diagnostics.Debug.WriteLine(logEntry);

        // Write to file
        try
        {
            lock (LockObject)
            {
                File.AppendAllText(LogFilePath, logEntry + Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            // If logging fails, try writing error to temp directory as last resort
            try
            {
                var errorPath = Path.Combine(Path.GetTempPath(), "launchbox-mqtt-error.txt");
                var errorInfo = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Failed to write log:\n" +
                               $"Error: {ex.Message}\n" +
                               $"Log path: {LogFilePath}\n" +
                               $"Message attempted: {logEntry}\n\n";
                File.AppendAllText(errorPath, errorInfo);
            }
            catch
            {
                // Truly silent failure - nothing we can do
            }
        }
    }
}
