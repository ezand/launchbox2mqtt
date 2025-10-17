using System.Text.Json;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;

namespace MqttPlugin.Launchbox.Core
{
    public class GameLaunchingEvent : IGameLaunchingPlugin
    {
        public void OnBeforeGameLaunching(IGame game, IAdditionalApplication app, IEmulator emulator)
        {
            MQTT.Publish("launchbox/emulator/loaded", "on");
            MQTT.Publish("launchbox/content/loaded", "on");
        }

        public void OnAfterGameLaunched(IGame game, IAdditionalApplication app, IEmulator emulator)
        {
            Logger.Info($"Game launched: {game.Title} on emulator {emulator.Title}");

            MQTT.Publish("launchbox/emulator/running", "on");
            MQTT.Publish("launchbox/emulator", emulator.Title);
            MQTT.Publish("launchbox/emulator/last_loaded", emulator.Title, true);
            MQTT.Publish("launchbox/emulator/details", JsonSerializer.Serialize(emulator), true);

            MQTT.Publish("launchbox/content/running", "on");
            MQTT.Publish("launchbox/content/content", game.Title);
            MQTT.Publish("launchbox/content/last_played", game.Title, true);
            MQTT.Publish("launchbox/content/details", JsonSerializer.Serialize(game), true);
        }

        public void OnGameExited()
        {
            Logger.Info("Event exited");
            EventUtils.UnloadContentEvent();
        }
    }
}