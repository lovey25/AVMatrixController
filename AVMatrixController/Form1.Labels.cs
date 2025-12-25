using MaterialSkin.Controls;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows.Forms;

namespace AVMatrixController
{
    public partial class Form1
    {
        private const string LabelSettingsFile = "labels.json";
        private List<string> customInputLabels = new();
        private List<string> customOutputLabels = new();

        private const string LegacyPresetsFile = "presets.json";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadAppStateAndApply();
            AttachLabelHandlers();
            HookPresetCardRewriter();

            btnSavePreset.Click -= BtnSavePreset_Click;
            btnSavePreset.Click += BtnSavePresetUnified_Click;

            btnSettings.Click -= BtnSettings_Click;
            btnSettings.Click += BtnSettingsUnified_Click;
        }

        private void LoadAppStateAndApply()
        {
            var state = ReadAppState();

            deviceIp = state.DeviceIp;
            devicePort = state.DevicePort;
            customInputLabels = state.InputLabels ?? new List<string>();
            customOutputLabels = state.OutputLabels ?? new List<string>();
            presets = state.Presets ?? new List<Preset>();

            EnsureLabelDefaults();
            ApplyCustomLabels();
            RefreshPresetPanel();
        }

        private void BtnSavePresetUnified_Click(object? sender, EventArgs e)
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
                SaveAppState();
                RefreshPresetPanel();

                var logMessage = $"[{DateTime.Now:HH:mm:ss}] 프리셋 저장\n";
                logMessage += $"  이름: {preset.Name}\n";
                logMessage += $"  설정: 입력 {preset.Input} → 출력 {string.Join(", ", preset.Outputs)}\n\n";
                AppendLog(logMessage);

                ShowToast($"? 프리셋 '{preset.Name}'이 저장되었습니다.", ToastType.Info);
            }
        }

        private void BtnSettingsUnified_Click(object? sender, EventArgs e)
        {
            using var settingsDialog = new SettingsDialog(deviceIp, devicePort);
            if (settingsDialog.ShowDialog() == DialogResult.OK)
            {
                string oldIp = deviceIp;
                int oldPort = devicePort;

                deviceIp = settingsDialog.DeviceIp;
                devicePort = settingsDialog.DevicePort;
                SaveAppState();

                var logMessage = $"[{DateTime.Now:HH:mm:ss}] 장치 설정 변경\n";
                logMessage += $"  IP: {oldIp} → {deviceIp}\n";
                logMessage += $"  포트: {oldPort} → {devicePort}\n\n";
                AppendLog(logMessage);

                ShowToast("? 설정이 저장되었습니다.", ToastType.Info);
            }
        }

        private void ConfirmAndDeletePreset(Preset preset)
        {
            var confirm = MessageBox.Show($"프리셋 '{preset.Name}'을 삭제하시겠습니까?", "삭제 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            var logMessage = $"[{DateTime.Now:HH:mm:ss}] 프리셋 삭제\n";
            logMessage += $"  이름: {preset.Name}\n\n";
            AppendLog(logMessage);

            presets.Remove(preset);
            SaveAppState();
            RefreshPresetPanel();
            ShowToast($"? 프리셋 '{preset.Name}'이 삭제되었습니다.", ToastType.Info);
        }

        private void EditPresetFromCard(Preset preset)
        {
            var newName = Interaction.InputBox("프리셋 이름을 수정하세요.", "프리셋 수정", preset.Name).Trim();
            if (string.IsNullOrWhiteSpace(newName)) return;

            preset.Name = newName;
            SaveAppState();
            RefreshPresetPanel();
            ShowToast($"? 프리셋 '{preset.Name}'이 수정되었습니다.", ToastType.Info);
        }

        private void LoadLabelSettings()
        {
            var state = ReadAppState();
            customInputLabels = state.InputLabels ?? new List<string>();
            customOutputLabels = state.OutputLabels ?? new List<string>();
            EnsureLabelDefaults();
        }

        private void SaveLabelSettings()
        {
            EnsureLabelDefaults();
            SaveAppState();
        }

        private AppState ReadAppState()
        {
            var state = new AppState();
            try
            {
                if (File.Exists(SettingsFile))
                {
                    var json = File.ReadAllText(SettingsFile);
                    try
                    {
                        var loaded = JsonSerializer.Deserialize<AppState>(json);
                        if (loaded != null) state = loaded;
                    }
                    catch
                    {
                        var legacySettings = JsonSerializer.Deserialize<Settings>(json);
                        if (legacySettings != null)
                        {
                            state.DeviceIp = legacySettings.DeviceIp;
                            state.DevicePort = legacySettings.DevicePort;
                        }
                    }
                }

                if ((state.InputLabels == null || state.InputLabels.Count == 0) && File.Exists(LabelSettingsFile))
                {
                    var json = File.ReadAllText(LabelSettingsFile);
                    var labels = JsonSerializer.Deserialize<LabelSettings>(json);
                    if (labels != null)
                    {
                        state.InputLabels = labels.InputLabels ?? new List<string>();
                        state.OutputLabels = labels.OutputLabels ?? new List<string>();
                    }
                }

                if ((state.Presets == null || state.Presets.Count == 0) && File.Exists(LegacyPresetsFile))
                {
                    var json = File.ReadAllText(LegacyPresetsFile);
                    var legacyPresets = JsonSerializer.Deserialize<List<Preset>>(json);
                    if (legacyPresets != null)
                    {
                        state.Presets = legacyPresets;
                    }
                }
            }
            catch { }

            return EnsureStateDefaults(state);
        }

        private void SaveAppState()
        {
            try
            {
                var state = new AppState
                {
                    DeviceIp = deviceIp,
                    DevicePort = devicePort,
                    InputLabels = customInputLabels,
                    OutputLabels = customOutputLabels,
                    Presets = presets
                };
                state = EnsureStateDefaults(state);
                var json = JsonSerializer.Serialize(state);
                File.WriteAllText(SettingsFile, json);
            }
            catch { }
        }

        private AppState EnsureStateDefaults(AppState state)
        {
            state ??= new AppState();
            state.InputLabels ??= new List<string>();
            state.OutputLabels ??= new List<string>();
            for (int i = state.InputLabels.Count; i < 8; i++) state.InputLabels.Add($"입력 {i + 1}");
            for (int i = state.OutputLabels.Count; i < 8; i++) state.OutputLabels.Add($"출력 {i + 1}");
            if (state.InputLabels.Count > 8) state.InputLabels = state.InputLabels.Take(8).ToList();
            if (state.OutputLabels.Count > 8) state.OutputLabels = state.OutputLabels.Take(8).ToList();
            state.Presets ??= new List<Preset>();
            return state;
        }

        private void InputLabel_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (sender is MaterialButton btn && btn.Tag is int input)
            {
                var newName = Interaction.InputBox($"입력 {input} 이름을 입력하세요.", "입력 이름 변경", customInputLabels[input - 1]).Trim();
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    customInputLabels[input - 1] = newName;
                    ApplyCustomLabels();
                    SaveLabelSettings();
                    ShowToast("? 입력 이름이 변경되었습니다.", ToastType.Info);
                }
            }
        }

        private void OutputLabel_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (sender is MaterialButton btn && btn.Tag is int output)
            {
                var newName = Interaction.InputBox($"출력 {output} 이름을 입력하세요.", "출력 이름 변경", customOutputLabels[output - 1]).Trim();
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    customOutputLabels[output - 1] = newName;
                    ApplyCustomLabels();
                    SaveLabelSettings();
                    ShowToast("? 출력 이름이 변경되었습니다.", ToastType.Info);
                }
            }
        }

        private void HookPresetCardRewriter()
        {
            panelPresets.ControlAdded -= PanelPresets_ControlAdded;
            panelPresets.ControlAdded += PanelPresets_ControlAdded;
            RewritePresetCards();
        }

        private void PanelPresets_ControlAdded(object? sender, ControlEventArgs e)
        {
            if (e.Control is Panel card)
            {
                UpgradePresetCard(card);
            }
        }

        private void RewritePresetCards()
        {
            foreach (var card in panelPresets.Controls.OfType<Panel>())
            {
                UpgradePresetCard(card);
            }
        }

        private void UpgradePresetCard(Panel presetCard)
        {
            var preset = presetCard.Controls
                .OfType<MaterialButton>()
                .Select(b => b.Tag as Preset)
                .FirstOrDefault(p => p != null);

            if (preset == null) return;

            var applyButton = presetCard.Controls
                .OfType<MaterialButton>()
                .FirstOrDefault(b => b.Text == "적용" && ReferenceEquals(b.Tag, preset));
            if (applyButton != null)
            {
                presetCard.Controls.Remove(applyButton);
                applyButton.Dispose();
            }

            var inputLabelText = GetInputLabel(preset.Input);
            var outputLabelText = string.Join(", ", preset.Outputs.Select(GetOutputLabel));

            var detailLabel = presetCard.Controls
                .OfType<Label>()
                .FirstOrDefault(l => l.Text.Contains("→") || l.Text.Contains("입력"));
            if (detailLabel != null)
            {
                detailLabel.Text = $"{inputLabelText} → {outputLabelText}";
            }

            presetCard.Tag = preset;
            presetCard.Cursor = Cursors.Hand;
            presetCard.Click -= PresetCard_Click;
            presetCard.MouseUp -= PresetCard_MouseUp;
            presetCard.MouseUp += PresetCard_MouseUp;

            foreach (var label in presetCard.Controls.OfType<Label>())
            {
                label.Tag = preset;
                label.Cursor = Cursors.Hand;
                label.Click -= PresetCard_Click;
                label.MouseUp -= PresetCard_MouseUp;
                label.MouseUp += PresetCard_MouseUp;
            }

            foreach (var button in presetCard.Controls.OfType<MaterialButton>().Where(b => b.Text == "삭제"))
            {
                button.Tag = preset;
                ClearClickHandlers(button);
                button.Click += (s, e) => ConfirmAndDeletePreset(preset);
            }
        }

        private void ClearClickHandlers(Control control)
        {
            var eventsProp = typeof(Component).GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
            if (eventsProp == null) return;

            var eventList = eventsProp.GetValue(control) as EventHandlerList;
            if (eventList == null) return;

            var clickField = typeof(Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);
            var clickKey = clickField?.GetValue(control);
            if (clickKey != null)
            {
                var handler = eventList[clickKey];
                if (handler != null) eventList.RemoveHandler(clickKey, handler);
            }
        }

        private async void PresetCard_Click(object? sender, EventArgs e)
        {
            if (Control.MouseButtons == MouseButtons.Right) return;
            if (sender is Control ctrl && ctrl.Tag is Preset preset)
            {
                await ApplyPreset(preset);
            }
        }

        private void PresetCard_MouseUp(object? sender, MouseEventArgs e)
        {
            if (sender is Control ctrl && ctrl.Tag is Preset preset)
            {
                if (e.Button == MouseButtons.Right)
                {
                    EditPresetFromCard(preset);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    _ = ApplyPreset(preset);
                }
            }
        }

        private void AttachLabelHandlers()
        {
            foreach (var btn in inputButtons)
            {
                btn.MouseUp -= InputLabel_MouseUp;
                btn.MouseUp += InputLabel_MouseUp;
            }

            foreach (var btn in outputButtons)
            {
                btn.MouseUp -= OutputLabel_MouseUp;
                btn.MouseUp += OutputLabel_MouseUp;
            }
        }

        private void ApplyCustomLabels()
        {
            EnsureLabelDefaults();

            for (int i = 0; i < inputButtons.Count && i < customInputLabels.Count; i++)
            {
                inputButtons[i].Text = customInputLabels[i];
            }

            for (int i = 0; i < outputButtons.Count && i < customOutputLabels.Count; i++)
            {
                outputButtons[i].Text = customOutputLabels[i];
            }

            RepositionInputButtons();
            RepositionOutputButtons();
        }

        private void EnsureLabelDefaults()
        {
            if (customInputLabels == null) customInputLabels = new List<string>();
            if (customOutputLabels == null) customOutputLabels = new List<string>();

            for (int i = customInputLabels.Count; i < 8; i++)
            {
                customInputLabels.Add($"입력 {i + 1}");
            }

            for (int i = customOutputLabels.Count; i < 8; i++)
            {
                customOutputLabels.Add($"출력 {i + 1}");
            }

            if (customInputLabels.Count > 8) customInputLabels = customInputLabels.Take(8).ToList();
            if (customOutputLabels.Count > 8) customOutputLabels = customOutputLabels.Take(8).ToList();
        }

        private string GetInputLabel(int inputIndex)
        {
            EnsureLabelDefaults();
            if (inputIndex >= 1 && inputIndex <= customInputLabels.Count)
                return customInputLabels[inputIndex - 1];
            return $"입력 {inputIndex}";
        }

        private string GetOutputLabel(int outputIndex)
        {
            EnsureLabelDefaults();
            if (outputIndex >= 1 && outputIndex <= customOutputLabels.Count)
                return customOutputLabels[outputIndex - 1];
            return $"출력 {outputIndex}";
        }
    }

    public class LabelSettings
    {
        public List<string> InputLabels { get; set; } = new();
        public List<string> OutputLabels { get; set; } = new();
    }

    public class AppState
    {
        public string DeviceIp { get; set; } = "192.168.1.200";
        public int DevicePort { get; set; } = 7000;
        public List<string> InputLabels { get; set; } = new();
        public List<string> OutputLabels { get; set; } = new();
        public List<Preset> Presets { get; set; } = new();
    }
}
