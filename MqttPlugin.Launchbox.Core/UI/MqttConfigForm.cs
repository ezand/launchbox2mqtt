using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using MqttPlugin.Launchbox.Core.Models;
using MqttPlugin.Launchbox.Core.Services;
using MQTTnet;

namespace MqttPlugin.Launchbox.Core.UI;

public class MqttConfigForm : Form
{
    private readonly PictureBox _bannerPictureBox;
    private readonly TextBox _hostTextBox;
    private readonly TextBox _portTextBox;
    private readonly TextBox _usernameTextBox;
    private readonly TextBox _passwordTextBox;
    private readonly Button _testButton;
    private readonly Button _saveButton;
    private readonly Button _cancelButton;
    private readonly Label _statusLabel;

    private MqttConfig _config;

    public MqttConfigForm(MqttConfig config)
    {
        _config = config;

        Text = "MQTT Broker Configuration";
        Width = 400;
        Height = 450;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterScreen;
        MaximizeBox = false;
        MinimizeBox = false;

        // Load and add banner
        _bannerPictureBox = new PictureBox
        {
            Left = 0,
            Top = 0,
            Width = ClientSize.Width,
            Height = 0,
            SizeMode = PictureBoxSizeMode.Zoom,
            Dock = DockStyle.Top
        };

        var bannerImage = LoadBannerImage();
        if (bannerImage != null)
        {
            _bannerPictureBox.Image = bannerImage;
            var aspectRatio = (double)bannerImage.Height / bannerImage.Width;
            _bannerPictureBox.Height = (int)(ClientSize.Width * aspectRatio);
        }

        Controls.Add(_bannerPictureBox);

        var yPos = _bannerPictureBox.Height + 20;
        var labelWidth = 80;
        var textBoxWidth = 250;
        var xLabel = 20;
        var xTextBox = xLabel + labelWidth + 10;

        // Host
        var hostLabel = new Label
        {
            Text = "Host:",
            Left = xLabel,
            Top = yPos,
            Width = labelWidth
        };
        _hostTextBox = new TextBox
        {
            Left = xTextBox,
            Top = yPos,
            Width = textBoxWidth,
            Text = config.Host
        };
        Controls.Add(hostLabel);
        Controls.Add(_hostTextBox);
        yPos += 35;

        // Port
        var portLabel = new Label
        {
            Text = "Port:",
            Left = xLabel,
            Top = yPos,
            Width = labelWidth
        };
        _portTextBox = new TextBox
        {
            Left = xTextBox,
            Top = yPos,
            Width = textBoxWidth,
            Text = config.Port.ToString()
        };
        Controls.Add(portLabel);
        Controls.Add(_portTextBox);
        yPos += 35;

        // Username
        var usernameLabel = new Label
        {
            Text = "Username:",
            Left = xLabel,
            Top = yPos,
            Width = labelWidth
        };
        _usernameTextBox = new TextBox
        {
            Left = xTextBox,
            Top = yPos,
            Width = textBoxWidth,
            Text = config.Username ?? ""
        };
        Controls.Add(usernameLabel);
        Controls.Add(_usernameTextBox);
        yPos += 35;

        // Password
        var passwordLabel = new Label
        {
            Text = "Password:",
            Left = xLabel,
            Top = yPos,
            Width = labelWidth
        };
        _passwordTextBox = new TextBox
        {
            Left = xTextBox,
            Top = yPos,
            Width = textBoxWidth,
            UseSystemPasswordChar = true,
            Text = ConfigManager.DecryptPassword(config.EncryptedPassword) ?? ""
        };
        Controls.Add(passwordLabel);
        Controls.Add(_passwordTextBox);
        yPos += 35;

        // Status label
        _statusLabel = new Label
        {
            Left = xLabel,
            Top = yPos,
            Width = textBoxWidth + labelWidth + 10,
            Height = 20,
            Text = ""
        };
        Controls.Add(_statusLabel);
        yPos += 30;

        // Buttons
        _testButton = new Button
        {
            Text = "Test Connection",
            Left = xLabel,
            Top = yPos,
            Width = 120
        };
        _testButton.Click += async (s, e) => await TestConnectionAsync();
        Controls.Add(_testButton);

        _saveButton = new Button
        {
            Text = "Save",
            Left = xTextBox + textBoxWidth - 160,
            Top = yPos,
            Width = 75
        };
        _saveButton.Click += SaveButton_Click;
        Controls.Add(_saveButton);

        _cancelButton = new Button
        {
            Text = "Cancel",
            Left = xTextBox + textBoxWidth - 75,
            Top = yPos,
            Width = 75
        };
        _cancelButton.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
        Controls.Add(_cancelButton);
    }

    private static Image? LoadBannerImage()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "MqttPlugin.Launchbox.Core.resources.launchbox2mqtt_banner.png";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                Logger.Info($"Loaded banner image from embedded resource: {resourceName}");
                return Image.FromStream(stream);
            }
            else
            {
                Logger.Info($"Embedded resource not found: {resourceName}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to load banner image: {ex.Message}", ex);
            return null;
        }
    }

    private async Task TestConnectionAsync()
    {
        _statusLabel.Text = "Testing connection...";
        _statusLabel.ForeColor = System.Drawing.Color.Blue;
        _testButton.Enabled = false;

        try
        {
            if (!int.TryParse(_portTextBox.Text, out var port))
            {
                _statusLabel.Text = "Invalid port number";
                _statusLabel.ForeColor = System.Drawing.Color.Red;
                return;
            }

            var factory = new MqttClientFactory();
            using var mqttClient = factory.CreateMqttClient();

            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(_hostTextBox.Text, port)
                .WithClientId($"LaunchBoxTest_{Guid.NewGuid()}")
                .WithCleanSession();

            if (!string.IsNullOrWhiteSpace(_usernameTextBox.Text))
            {
                optionsBuilder.WithCredentials(_usernameTextBox.Text, _passwordTextBox.Text);
            }

            var options = optionsBuilder.Build();

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var result = await mqttClient.ConnectAsync(options, cts.Token);

            if (result.ResultCode == MqttClientConnectResultCode.Success)
            {
                _statusLabel.Text = "Connection successful!";
                _statusLabel.ForeColor = System.Drawing.Color.Green;
                await mqttClient.DisconnectAsync();
            }
            else
            {
                _statusLabel.Text = $"Connection failed: {result.ResultCode}";
                _statusLabel.ForeColor = System.Drawing.Color.Red;
            }
        }
        catch (Exception ex)
        {
            _statusLabel.Text = $"Error: {ex.Message}";
            _statusLabel.ForeColor = System.Drawing.Color.Red;
            Logger.Error($"MQTT test connection failed: {ex.Message}");
        }
        finally
        {
            _testButton.Enabled = true;
        }
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        if (!int.TryParse(_portTextBox.Text, out var port))
        {
            MessageBox.Show("Invalid port number", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (string.IsNullOrWhiteSpace(_hostTextBox.Text))
        {
            MessageBox.Show("Host cannot be empty", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _config.Host = _hostTextBox.Text;
        _config.Port = port;
        _config.Username = string.IsNullOrWhiteSpace(_usernameTextBox.Text) ? null : _usernameTextBox.Text;
        _config.EncryptedPassword = ConfigManager.EncryptPassword(_passwordTextBox.Text);

        if (ConfigManager.SaveConfig(_config))
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            MessageBox.Show("Failed to save configuration", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
