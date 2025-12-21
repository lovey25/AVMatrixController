using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AVMatrixController
{
    public partial class Form1 : MaterialForm
    {
        private int? selectedInput = null;
        private HashSet<int> selectedOutputs = new HashSet<int>();
        private List<MaterialButton> inputButtons = new List<MaterialButton>();
        private List<MaterialButton> outputButtons = new List<MaterialButton>();

        private string deviceIp = "192.168.1.200";
        private int devicePort = 7000;
        private const string SettingsFile = "settings.json";
        private const string PresetsFile = "presets.json";

        private List<Preset> presets = new List<Preset>();
        private UdpClient? udpClient;
        private System.Windows.Forms.Timer? toastTimer;

        public Form1()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Blue600, Primary.Blue700,
                Primary.Blue200, Accent.LightBlue200,
                TextShade.WHITE
            );

            InitializeControls();
            LoadSettings();
            LoadPresets();
            InitializeUdpClient();

            this.Resize += Form1_Resize;
            
            AppendLog($"[{DateTime.Now:HH:mm:ss}] AV Matrix Controller 시작\n");
            AppendLog($"  버전: 1.0.0\n");
            AppendLog($"  장치 IP: {deviceIp}:{devicePort}\n");
            AppendLog($"  로드된 프리셋: {presets.Count}개\n\n");
        }

        private void Form1_Resize(object? sender, EventArgs e)
        {
            RepositionInputButtons();
            RepositionOutputButtons();
            CenterToastPanel();
        }

        private void InitializeControls()
        {
            CreateInputButtons();
            CreateOutputButtons();
            AttachEventHandlers();
        }

        private void CreateInputButtons()
        {
            panelInputs.Controls.Clear();
            inputButtons.Clear();

            for (int i = 1; i <= 8; i++)
            {
                var btn = new MaterialButton
                {
                    Text = $"입력 {i}",
                    Size = new Size(85, 60),
                    Tag = i,
                    Type = MaterialButton.MaterialButtonType.Outlined,
                    Density = MaterialButton.MaterialButtonDensity.Default,
                    UseAccentColor = false,
                    AutoSize = false
                };
                btn.Click += InputButton_Click;
                inputButtons.Add(btn);
                panelInputs.Controls.Add(btn);
            }
            RepositionInputButtons();
        }

        private void CreateOutputButtons()
        {
            panelOutputs.Controls.Clear();
            outputButtons.Clear();

            for (int i = 1; i <= 8; i++)
            {
                var btn = new MaterialButton
                {
                    Text = $"출력 {i}",
                    Size = new Size(85, 60),
                    Tag = i,
                    Type = MaterialButton.MaterialButtonType.Outlined,
                    Density = MaterialButton.MaterialButtonDensity.Default,
                    UseAccentColor = false,
                    AutoSize = false
                };
                btn.Click += OutputButton_Click;
                outputButtons.Add(btn);
                panelOutputs.Controls.Add(btn);
            }
            RepositionOutputButtons();
        }

        private void RepositionInputButtons()
        {
            if (inputButtons.Count == 0) return;

            int panelWidth = panelInputs.Width;
            int buttonCount = inputButtons.Count;
            int spacing = 10;
            int totalSpacing = spacing * (buttonCount - 1);
            int buttonWidth = (panelWidth - totalSpacing - 20) / buttonCount;

            buttonWidth = Math.Max(70, Math.Min(buttonWidth, 150));

            int totalWidth = (buttonWidth * buttonCount) + (spacing * (buttonCount - 1));
            int startX = (panelWidth - totalWidth) / 2;

            panelInputs.SuspendLayout();
            for (int i = 0; i < inputButtons.Count; i++)
            {
                inputButtons[i].Size = new Size(buttonWidth, 60);
                inputButtons[i].Location = new Point(startX + (i * (buttonWidth + spacing)), 10);
                inputButtons[i].Invalidate();
            }
            panelInputs.ResumeLayout(true);
        }

        private void RepositionOutputButtons()
        {
            if (outputButtons.Count == 0) return;

            int panelWidth = panelOutputs.Width;
            int buttonCount = outputButtons.Count;
            int spacing = 10;
            int totalSpacing = spacing * (buttonCount - 1);
            int buttonWidth = (panelWidth - totalSpacing - 20) / buttonCount;

            buttonWidth = Math.Max(70, Math.Min(buttonWidth, 150));

            int totalWidth = (buttonWidth * buttonCount) + (spacing * (buttonCount - 1));
            int startX = (panelWidth - totalWidth) / 2;

            panelOutputs.SuspendLayout();
            for (int i = 0; i < outputButtons.Count; i++)
            {
                outputButtons[i].Size = new Size(buttonWidth, 60);
                outputButtons[i].Location = new Point(startX + (i * (buttonWidth + spacing)), 10);
                outputButtons[i].Invalidate();
            }
            panelOutputs.ResumeLayout(true);
        }

        private void CenterToastPanel()
        {
            if (panelToast != null && lblToast != null)
            {
                panelToast.Location = new Point((this.ClientSize.Width - panelToast.Width) / 2, 75);
                lblToast.Location = new Point((this.ClientSize.Width - lblToast.Width) / 2, 85);
            }
        }

        private void AttachEventHandlers()
        {
            btnApply.Click += BtnApply_Click;
            btnAllOutputs.Click += BtnAllOutputs_Click;
            btnSettings.Click += BtnSettings_Click;
            btnGetStatus.Click += BtnGetStatus_Click;
            btnGetLcdStatus.Click += BtnGetLcdStatus_Click;
            btnSearchDevice.Click += BtnSearchDevice_Click;
            btnSavePreset.Click += BtnSavePreset_Click;
            btnClearLog.Click += BtnClearLog_Click;
        }

        private void InputButton_Click(object? sender, EventArgs e)
        {
            if (sender is MaterialButton btn && btn.Tag is int input)
            {
                selectedInput = input;
                UpdateInputButtonStates();
            }
        }

        private void OutputButton_Click(object? sender, EventArgs e)
        {
            if (sender is MaterialButton btn && btn.Tag is int output)
            {
                if (ModifierKeys.HasFlag(Keys.Control))
                {
                    if (selectedOutputs.Contains(output))
                        selectedOutputs.Remove(output);
                    else
                        selectedOutputs.Add(output);
                }
                else
                {
                    selectedOutputs.Clear();
                    selectedOutputs.Add(output);
                }
                UpdateOutputButtonStates();
            }
        }

        private void UpdateInputButtonStates()
        {
            foreach (var btn in inputButtons)
            {
                if (btn.Tag is int input && input == selectedInput)
                {
                    btn.Type = MaterialButton.MaterialButtonType.Contained;
                    btn.UseAccentColor = true;
                }
                else
                {
                    btn.Type = MaterialButton.MaterialButtonType.Outlined;
                    btn.UseAccentColor = false;
                }
            }
        }

        private void UpdateOutputButtonStates()
        {
            foreach (var btn in outputButtons)
            {
                if (btn.Tag is int output && selectedOutputs.Contains(output))
                {
                    btn.Type = MaterialButton.MaterialButtonType.Contained;
                    btn.UseAccentColor = true;
                }
                else
                {
                    btn.Type = MaterialButton.MaterialButtonType.Outlined;
                    btn.UseAccentColor = false;
                }
            }
        }

        private async void BtnApply_Click(object? sender, EventArgs e)
        {
            if (!selectedInput.HasValue)
            {
                ShowToast("입력을 선택해주세요.", ToastType.Error);
                return;
            }

            if (selectedOutputs.Count == 0)
            {
                ShowToast("출력을 선택해주세요.", ToastType.Error);
                return;
            }

            await ApplyRouting(selectedInput.Value, selectedOutputs.ToList());
        }

        private async void BtnAllOutputs_Click(object? sender, EventArgs e)
        {
            if (!selectedInput.HasValue)
            {
                ShowToast("입력을 선택해주세요.", ToastType.Error);
                return;
            }

            var allOutputs = Enumerable.Range(1, 8).ToList();
            await ApplyRouting(selectedInput.Value, allOutputs);
        }

        private async Task ApplyRouting(int input, List<int> outputs)
        {
            try
            {
                string command = MatrixProtocol.CreateRoutingCommand(input, outputs);
                byte[] commandBytes = MatrixProtocol.CreateAsciiCommandBytes(command);
                
                string outputList = string.Join(", ", outputs);
                AppendLog($"[{DateTime.Now:HH:mm:ss}] 라우팅 명령 전송 중...\n");
                AppendLog($"  입력: {input} → 출력: {outputList}\n");
                AppendLog($"  명령어: {command}\n");
                
                await SendCommandAsync(commandBytes);
                
                AppendLog($"  ✓ 전송 완료\n\n");
                ShowToast($"✓ 입력 {input} → 출력 {outputList}로 전환되었습니다.", ToastType.Success);
            }
            catch (Exception ex)
            {
                var errorLog = $"[{DateTime.Now:HH:mm:ss}] 라우팅 실패: {ex.Message}\n";
                errorLog += $"  상세 정보: {ex.GetType().Name}\n\n";
                AppendLog(errorLog);
                ShowToast($"✗ 오류: {ex.Message}", ToastType.Error);
            }
        }

        private void BtnSettings_Click(object? sender, EventArgs e)
        {
            using var settingsDialog = new SettingsDialog(deviceIp, devicePort);
            if (settingsDialog.ShowDialog() == DialogResult.OK)
            {
                string oldIp = deviceIp;
                int oldPort = devicePort;
                
                deviceIp = settingsDialog.DeviceIp;
                devicePort = settingsDialog.DevicePort;
                SaveSettings();
                
                var logMessage = $"[{DateTime.Now:HH:mm:ss}] 장치 설정 변경\n";
                logMessage += $"  IP: {oldIp} → {deviceIp}\n";
                logMessage += $"  포트: {oldPort} → {devicePort}\n\n";
                AppendLog(logMessage);
                
                ShowToast("✓ 설정이 저장되었습니다.", ToastType.Info);
            }
        }

        private async void BtnGetStatus_Click(object? sender, EventArgs e)
        {
            try
            {
                AppendLog($"[{DateTime.Now:HH:mm:ss}] 장치 상태 조회 시작...\n");
                
                byte[] command = MatrixProtocol.CreateReadStatusPacket();
                byte[] response = await SendCommandWithResponseAsync(command);

                var status = MatrixProtocol.ParseStatusResponse(response);
                if (status.Success)
                {
                    DisplayDeviceStatus(status);
                    ShowToast("✓ 장치 상태를 조회했습니다.", ToastType.Success);
                }
                else
                {
                    var errorLog = $"[{DateTime.Now:HH:mm:ss}] 장치 상태 조회 실패: 응답 상태 코드 0x{status.Status:X2}\n\n";
                    AppendLog(errorLog);
                    ShowToast("✗ 장치 상태 조회 실패", ToastType.Error);
                }
            }
            catch (Exception ex)
            {
                var errorLog = $"[{DateTime.Now:HH:mm:ss}] 장치 상태 조회 오류: {ex.Message}\n";
                errorLog += $"  상세 정보: {ex.GetType().Name}\n\n";
                AppendLog(errorLog);
                ShowToast($"✗ 오류: {ex.Message}", ToastType.Error);
            }
        }

        private async void BtnGetLcdStatus_Click(object? sender, EventArgs e)
        {
            try
            {
                AppendLog($"[{DateTime.Now:HH:mm:ss}] LCD 상태 조회 시작...\n");
                
                byte[] command = MatrixProtocol.CreateReadLcdStatusPacket();
                byte[] response = await SendCommandWithResponseAsync(command);

                var status = MatrixProtocol.ParseLcdStatusResponse(response);
                if (status.Success)
                {
                    DisplayLcdStatus(status);
                    ShowToast("✓ LCD 상태를 조회했습니다.", ToastType.Success);
                }
                else
                {
                    var errorLog = $"[{DateTime.Now:HH:mm:ss}] LCD 상태 조회 실패: 응답 상태 코드 0x{status.Status:X2}\n\n";
                    AppendLog(errorLog);
                    ShowToast("✗ LCD 상태 조회 실패", ToastType.Error);
                }
            }
            catch (Exception ex)
            {
                var errorLog = $"[{DateTime.Now:HH:mm:ss}] LCD 상태 조회 오류: {ex.Message}\n";
                errorLog += $"  상세 정보: {ex.GetType().Name}\n\n";
                AppendLog(errorLog);
                ShowToast($"✗ 오류: {ex.Message}", ToastType.Error);
            }
        }

        private async void BtnSearchDevice_Click(object? sender, EventArgs e)
        {
            try
            {
                AppendLog($"[{DateTime.Now:HH:mm:ss}] 장치 검색 시작 (브로드캐스트)...\n");
                
                byte[] searchCommand = MatrixProtocol.CreateSearchPacket();
                var udpSearch = new UdpClient();
                udpSearch.EnableBroadcast = true;

                var broadcastEp = new IPEndPoint(IPAddress.Broadcast, 7000);
                await udpSearch.SendAsync(searchCommand, searchCommand.Length, broadcastEp);

                AppendLog($"  브로드캐스트 전송 완료, 응답 대기 중... (타임아웃: 3초)\n");
                
                udpSearch.Client.ReceiveTimeout = 3000;
                var result = await udpSearch.ReceiveAsync();

                var searchResult = MatrixProtocol.ParseSearchResponse(result.Buffer);

                var sb = new StringBuilder();
                sb.AppendLine($"[{DateTime.Now:HH:mm:ss}] 장치 검색 완료");
                sb.AppendLine("┌─────────────────────────────────┐");
                sb.AppendLine("│       장치 검색 결과            │");
                sb.AppendLine("├─────────────────────────────────┤");
                sb.AppendLine($"│ 📍 IP: {result.RemoteEndPoint.Address,-20}│");
                sb.AppendLine($"│ 🔧 Device Type: 0x{searchResult.DeviceType:X2}          │");
                sb.AppendLine($"│ 📦 Model: {searchResult.ModelName,-18}│");
                sb.AppendLine("└─────────────────────────────────┘");
                sb.AppendLine();
                sb.AppendLine($"RAW HEX: {BitConverter.ToString(result.Buffer)}");
                sb.AppendLine();

                AppendLog(sb.ToString());

                ShowToast("✓ 장치 검색 응답을 수신했습니다.", ToastType.Success);
                udpSearch.Close();
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
            {
                var errorLog = $"[{DateTime.Now:HH:mm:ss}] 장치 검색 타임아웃: 응답이 없습니다.\n";
                errorLog += $"  네트워크 연결 및 장치 전원을 확인하세요.\n\n";
                AppendLog(errorLog);
                ShowToast("✗ 검색 타임아웃: 응답 없음", ToastType.Error);
            }
            catch (Exception ex)
            {
                var errorLog = $"[{DateTime.Now:HH:mm:ss}] 장치 검색 실패: {ex.Message}\n";
                errorLog += $"  상세 정보: {ex.GetType().Name}\n\n";
                AppendLog(errorLog);
                ShowToast($"✗ 검색 실패: {ex.Message}", ToastType.Error);
            }
        }

        private void BtnSavePreset_Click(object? sender, EventArgs e)
        {
            if (!selectedInput.HasValue || selectedOutputs.Count == 0)
            {
                ShowToast("입력과 출력을 선택해주세요.", ToastType.Error);
                return;
            }

            using var nameDialog = new PresetNameDialog();
            if (nameDialog.ShowDialog() == DialogResult.OK)
            {
                var preset = new Preset
                {
                    Name = nameDialog.PresetName,
                    Input = selectedInput.Value,
                    Outputs = selectedOutputs.ToList(),
                    CreatedDate = DateTime.Now
                };

                presets.Add(preset);
                SavePresets();
                RefreshPresetPanel();
                
                var logMessage = $"[{DateTime.Now:HH:mm:ss}] 프리셋 저장\n";
                logMessage += $"  이름: {preset.Name}\n";
                logMessage += $"  설정: 입력 {preset.Input} → 출력 {string.Join(", ", preset.Outputs)}\n\n";
                AppendLog(logMessage);
                
                ShowToast($"✓ 프리셋 '{preset.Name}'이 저장되었습니다.", ToastType.Info);
            }
        }

        private void RefreshPresetPanel()
        {
            panelPresets.Controls.Clear();
            int x = 0;

            foreach (var preset in presets)
            {
                var presetCard = new Panel
                {
                    Size = new Size(180, 80),
                    Location = new Point(x, 5),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.FromArgb(240, 240, 240)
                };

                var lblName = new Label
                {
                    Text = preset.Name,
                    Location = new Point(5, 5),
                    Size = new Size(170, 20),
                    Font = new Font("Roboto", 10, FontStyle.Bold)
                };

                var lblDetails = new Label
                {
                    Text = $"입력 {preset.Input} → 출력 {string.Join(",", preset.Outputs)}",
                    Location = new Point(5, 28),
                    Size = new Size(170, 20),
                    Font = new Font("Roboto", 8)
                };

                var btnApplyPreset = new MaterialButton
                {
                    Text = "적용",
                    Size = new Size(80, 25),
                    Location = new Point(5, 50),
                    Type = MaterialButton.MaterialButtonType.Text,
                    Tag = preset
                };
                btnApplyPreset.Click += async (s, e) => await ApplyPreset(preset);

                var btnDeletePreset = new MaterialButton
                {
                    Text = "삭제",
                    Size = new Size(80, 25),
                    Location = new Point(90, 50),
                    Type = MaterialButton.MaterialButtonType.Text,
                    Tag = preset
                };
                btnDeletePreset.Click += (s, e) => DeletePreset(preset);

                presetCard.Controls.Add(lblName);
                presetCard.Controls.Add(lblDetails);
                presetCard.Controls.Add(btnApplyPreset);
                presetCard.Controls.Add(btnDeletePreset);
                panelPresets.Controls.Add(presetCard);

                x += 190;
            }
        }

        private async Task ApplyPreset(Preset preset)
        {
            var logMessage = $"[{DateTime.Now:HH:mm:ss}] 프리셋 적용\n";
            logMessage += $"  이름: {preset.Name}\n";
            logMessage += $"  설정: 입력 {preset.Input} → 출력 {string.Join(", ", preset.Outputs)}\n\n";
            AppendLog(logMessage);
            
            selectedInput = preset.Input;
            selectedOutputs = new HashSet<int>(preset.Outputs);
            UpdateInputButtonStates();
            UpdateOutputButtonStates();
            await ApplyRouting(preset.Input, preset.Outputs);
        }

        private void DeletePreset(Preset preset)
        {
            var logMessage = $"[{DateTime.Now:HH:mm:ss}] 프리셋 삭제\n";
            logMessage += $"  이름: {preset.Name}\n\n";
            AppendLog(logMessage);
            
            presets.Remove(preset);
            SavePresets();
            RefreshPresetPanel();
            ShowToast($"✓ 프리셋 '{preset.Name}'이 삭제되었습니다.", ToastType.Info);
        }

        private void DisplayDeviceStatus(DeviceStatusResponse status)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"[{DateTime.Now:HH:mm:ss}] 장치 상태 조회");
            sb.AppendLine("┌─────────────────────────────────┐");
            sb.AppendLine("│         장치 정보               │");
            sb.AppendLine("├─────────────────────────────────┤");
            sb.AppendLine($"│ 🏷 장치명: {status.DeviceName,-18}│");
            sb.AppendLine($"│ 🌐 IP 모드: {status.IpMode,-18}│");
            sb.AppendLine("│ 📺 출력 채널: 8 채널              │");
            sb.AppendLine("│                                 │");
            sb.AppendLine("│ 출력 ↔ 입력 매핑               │");

            for (int i = 0; i < 8; i++)
            {
                sb.AppendLine($"│ OUT{i + 1} ← IN{status.OutputStatus[i]}                │");
            }

            sb.AppendLine("└─────────────────────────────────┘");
            sb.AppendLine();

            AppendLog(sb.ToString());
        }

        private void DisplayLcdStatus(LcdStatusResponse status)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"[{DateTime.Now:HH:mm:ss}] LCD 상태 조회");
            sb.AppendLine("┌─────────────────────────────────┐");
            sb.AppendLine("│         LCD 상태                │");
            sb.AppendLine("├─────────────────────────────────┤");
            sb.AppendLine($"│ 💡 백라이트: {status.GetBacklightTimeDescription(),-17}│");
            sb.AppendLine($"│ ☀ 밝기: {status.Brightness}%                    │");
            sb.AppendLine("└─────────────────────────────────┘");
            sb.AppendLine();

            AppendLog(sb.ToString());
        }

        private void AppendLog(string logMessage)
        {
            if (!txtDeviceInfo.Visible)
            {
                txtDeviceInfo.Visible = true;
                lblDeviceInfo.Visible = true;
                btnClearLog.Visible = true;
            }

            if (txtDeviceInfo.Text.Length > 10000)
            {
                txtDeviceInfo.Text = txtDeviceInfo.Text.Substring(txtDeviceInfo.Text.Length - 5000);
            }

            txtDeviceInfo.AppendText(logMessage);
            txtDeviceInfo.SelectionStart = txtDeviceInfo.Text.Length;
            txtDeviceInfo.ScrollToCaret();
        }

        private void BtnClearLog_Click(object? sender, EventArgs e)
        {
            txtDeviceInfo.Clear();
            var logMessage = $"[{DateTime.Now:HH:mm:ss}] 로그가 지워졌습니다.\n\n";
            txtDeviceInfo.AppendText(logMessage);
        }

        private void InitializeUdpClient()
        {
            try
            {
                udpClient = new UdpClient();
            }
            catch (Exception ex)
            {
                ShowToast($"UDP 클라이언트 초기화 실패: {ex.Message}", ToastType.Error);
            }
        }

        private async Task SendCommandAsync(byte[] command)
        {
            if (udpClient == null)
            {
                AppendLog($"[{DateTime.Now:HH:mm:ss}] 오류: UDP 클라이언트가 초기화되지 않았습니다.\n\n");
                return;
            }

            var endpoint = new IPEndPoint(IPAddress.Parse(deviceIp), devicePort);
            await udpClient.SendAsync(command, command.Length, endpoint);
        }

        private async Task<byte[]> SendCommandWithResponseAsync(byte[] command)
        {
            var tempClient = new UdpClient();
            var endpoint = new IPEndPoint(IPAddress.Parse(deviceIp), devicePort);

            AppendLog($"  목적지: {deviceIp}:{devicePort}\n");
            AppendLog($"  전송 패킷 크기: {command.Length} bytes\n");
            AppendLog($"  HEX: {BitConverter.ToString(command)}\n");
            
            await tempClient.SendAsync(command, command.Length, endpoint);
            tempClient.Client.ReceiveTimeout = 3000;

            var result = await tempClient.ReceiveAsync();
            
            AppendLog($"  수신 패킷 크기: {result.Buffer.Length} bytes\n");
            AppendLog($"  발신지: {result.RemoteEndPoint}\n");
            
            tempClient.Close();

            return result.Buffer;
        }

        private void ShowToast(string message, ToastType type)
        {
            lblToast.Text = message;

            switch (type)
            {
                case ToastType.Success:
                    panelToast.BackColor = Color.FromArgb(76, 175, 80);
                    break;
                case ToastType.Error:
                    panelToast.BackColor = Color.FromArgb(244, 67, 54);
                    break;
                case ToastType.Info:
                    panelToast.BackColor = Color.FromArgb(33, 150, 243);
                    break;
            }

            CenterToastPanel();

            panelToast.Visible = true;
            lblToast.Visible = true;
            panelToast.BringToFront();
            lblToast.BringToFront();

            toastTimer?.Stop();
            toastTimer = new System.Windows.Forms.Timer { Interval = 3000 };
            toastTimer.Tick += (s, e) =>
            {
                panelToast.Visible = false;
                lblToast.Visible = false;
                toastTimer.Stop();
            };
            toastTimer.Start();
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    var json = File.ReadAllText(SettingsFile);
                    var settings = JsonSerializer.Deserialize<Settings>(json);
                    if (settings != null)
                    {
                        deviceIp = settings.DeviceIp;
                        devicePort = settings.DevicePort;
                    }
                }
            }
            catch { }
        }

        private void SaveSettings()
        {
            try
            {
                var settings = new Settings { DeviceIp = deviceIp, DevicePort = devicePort };
                var json = JsonSerializer.Serialize(settings);
                File.WriteAllText(SettingsFile, json);
            }
            catch { }
        }

        private void LoadPresets()
        {
            try
            {
                if (File.Exists(PresetsFile))
                {
                    var json = File.ReadAllText(PresetsFile);
                    var loadedPresets = JsonSerializer.Deserialize<List<Preset>>(json);
                    if (loadedPresets != null)
                    {
                        presets = loadedPresets;
                        RefreshPresetPanel();
                    }
                }
            }
            catch { }
        }

        private void SavePresets()
        {
            try
            {
                var json = JsonSerializer.Serialize(presets);
                File.WriteAllText(PresetsFile, json);
            }
            catch { }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            udpClient?.Close();
            base.OnFormClosing(e);
        }
    }

    public class Preset
    {
        public string Name { get; set; } = "";
        public int Input { get; set; }
        public List<int> Outputs { get; set; } = new();
        public DateTime CreatedDate { get; set; }
    }

    public class Settings
    {
        public string DeviceIp { get; set; } = "192.168.1.200";
        public int DevicePort { get; set; } = 7000;
    }

    public enum ToastType
    {
        Success,
        Error,
        Info
    }
}