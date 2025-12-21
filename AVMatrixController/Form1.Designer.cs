using MaterialSkin.Controls;
using System.Windows.Forms;

namespace AVMatrixController
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            panelInputs = new Panel();
            lblInputs = new MaterialLabel();
            panelOutputs = new Panel();
            lblOutputs = new MaterialLabel();
            btnApply = new MaterialButton();
            btnAllOutputs = new MaterialButton();
            btnSettings = new MaterialButton();
            btnGetStatus = new MaterialButton();
            btnGetLcdStatus = new MaterialButton();
            btnSearchDevice = new MaterialButton();
            panelPresets = new Panel();
            lblPresets = new MaterialLabel();
            btnSavePreset = new MaterialButton();
            panelDeviceInfo = new Panel();
            txtDeviceInfo = new RichTextBox();
            lblDeviceInfo = new MaterialLabel();
            panelToast = new Panel();
            lblToast = new MaterialLabel();
            panelButtons = new FlowLayoutPanel();
            btnClearLog = new MaterialButton();
            panelButtons.SuspendLayout();
            SuspendLayout();
            // 
            // panelInputs
            // 
            panelInputs.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelInputs.Location = new Point(20, 100);
            panelInputs.Name = "panelInputs";
            panelInputs.Size = new Size(760, 80);
            panelInputs.TabIndex = 0;
            // 
            // lblInputs
            // 
            lblInputs.AutoSize = true;
            lblInputs.Depth = 0;
            lblInputs.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblInputs.Location = new Point(20, 75);
            lblInputs.MouseState = MaterialSkin.MouseState.HOVER;
            lblInputs.Name = "lblInputs";
            lblInputs.Size = new Size(25, 19);
            lblInputs.TabIndex = 1;
            lblInputs.Text = "입력";
            // 
            // panelOutputs
            // 
            panelOutputs.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelOutputs.Location = new Point(20, 205);
            panelOutputs.Name = "panelOutputs";
            panelOutputs.Size = new Size(760, 80);
            panelOutputs.TabIndex = 2;
            // 
            // lblOutputs
            // 
            lblOutputs.AutoSize = true;
            lblOutputs.Depth = 0;
            lblOutputs.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblOutputs.Location = new Point(20, 183);
            lblOutputs.MouseState = MaterialSkin.MouseState.HOVER;
            lblOutputs.Name = "lblOutputs";
            lblOutputs.Size = new Size(25, 19);
            lblOutputs.TabIndex = 3;
            lblOutputs.Text = "출력";
            // 
            // btnApply
            // 
            btnApply.AutoSize = false;
            btnApply.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnApply.Density = MaterialButton.MaterialButtonDensity.Default;
            btnApply.Depth = 0;
            btnApply.HighEmphasis = true;
            btnApply.Icon = null;
            btnApply.Location = new Point(4, 6);
            btnApply.Margin = new Padding(4, 6, 4, 6);
            btnApply.MouseState = MaterialSkin.MouseState.HOVER;
            btnApply.Name = "btnApply";
            btnApply.NoAccentTextColor = Color.Empty;
            btnApply.Size = new Size(120, 36);
            btnApply.TabIndex = 4;
            btnApply.Text = "적용";
            btnApply.Type = MaterialButton.MaterialButtonType.Contained;
            btnApply.UseAccentColor = false;
            btnApply.UseVisualStyleBackColor = true;
            // 
            // btnAllOutputs
            // 
            btnAllOutputs.AutoSize = false;
            btnAllOutputs.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnAllOutputs.Density = MaterialButton.MaterialButtonDensity.Default;
            btnAllOutputs.Depth = 0;
            btnAllOutputs.HighEmphasis = true;
            btnAllOutputs.Icon = null;
            btnAllOutputs.Location = new Point(132, 6);
            btnAllOutputs.Margin = new Padding(4, 6, 4, 6);
            btnAllOutputs.MouseState = MaterialSkin.MouseState.HOVER;
            btnAllOutputs.Name = "btnAllOutputs";
            btnAllOutputs.NoAccentTextColor = Color.Empty;
            btnAllOutputs.Size = new Size(120, 36);
            btnAllOutputs.TabIndex = 5;
            btnAllOutputs.Text = "전체 출력";
            btnAllOutputs.Type = MaterialButton.MaterialButtonType.Contained;
            btnAllOutputs.UseAccentColor = false;
            btnAllOutputs.UseVisualStyleBackColor = true;
            // 
            // btnSettings
            // 
            btnSettings.AutoSize = false;
            btnSettings.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnSettings.Density = MaterialButton.MaterialButtonDensity.Default;
            btnSettings.Depth = 0;
            btnSettings.HighEmphasis = true;
            btnSettings.Icon = null;
            btnSettings.Location = new Point(644, 6);
            btnSettings.Margin = new Padding(4, 6, 4, 6);
            btnSettings.MouseState = MaterialSkin.MouseState.HOVER;
            btnSettings.Name = "btnSettings";
            btnSettings.NoAccentTextColor = Color.Empty;
            btnSettings.Size = new Size(100, 36);
            btnSettings.TabIndex = 6;
            btnSettings.Text = "⚙️ 설정";
            btnSettings.Type = MaterialButton.MaterialButtonType.Outlined;
            btnSettings.UseAccentColor = false;
            btnSettings.UseVisualStyleBackColor = true;
            // 
            // btnGetStatus
            // 
            btnGetStatus.AutoSize = false;
            btnGetStatus.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnGetStatus.Density = MaterialButton.MaterialButtonDensity.Default;
            btnGetStatus.Depth = 0;
            btnGetStatus.HighEmphasis = true;
            btnGetStatus.Icon = null;
            btnGetStatus.Location = new Point(260, 6);
            btnGetStatus.Margin = new Padding(4, 6, 4, 6);
            btnGetStatus.MouseState = MaterialSkin.MouseState.HOVER;
            btnGetStatus.Name = "btnGetStatus";
            btnGetStatus.NoAccentTextColor = Color.Empty;
            btnGetStatus.Size = new Size(120, 36);
            btnGetStatus.TabIndex = 7;
            btnGetStatus.Text = "상태 조회";
            btnGetStatus.Type = MaterialButton.MaterialButtonType.Outlined;
            btnGetStatus.UseAccentColor = false;
            btnGetStatus.UseVisualStyleBackColor = true;
            // 
            // btnGetLcdStatus
            // 
            btnGetLcdStatus.AutoSize = false;
            btnGetLcdStatus.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnGetLcdStatus.Density = MaterialButton.MaterialButtonDensity.Default;
            btnGetLcdStatus.Depth = 0;
            btnGetLcdStatus.HighEmphasis = true;
            btnGetLcdStatus.Icon = null;
            btnGetLcdStatus.Location = new Point(388, 6);
            btnGetLcdStatus.Margin = new Padding(4, 6, 4, 6);
            btnGetLcdStatus.MouseState = MaterialSkin.MouseState.HOVER;
            btnGetLcdStatus.Name = "btnGetLcdStatus";
            btnGetLcdStatus.NoAccentTextColor = Color.Empty;
            btnGetLcdStatus.Size = new Size(120, 36);
            btnGetLcdStatus.TabIndex = 8;
            btnGetLcdStatus.Text = "LCD 조회";
            btnGetLcdStatus.Type = MaterialButton.MaterialButtonType.Outlined;
            btnGetLcdStatus.UseAccentColor = false;
            btnGetLcdStatus.UseVisualStyleBackColor = true;
            // 
            // btnSearchDevice
            // 
            btnSearchDevice.AutoSize = false;
            btnSearchDevice.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnSearchDevice.Density = MaterialButton.MaterialButtonDensity.Default;
            btnSearchDevice.Depth = 0;
            btnSearchDevice.HighEmphasis = true;
            btnSearchDevice.Icon = null;
            btnSearchDevice.Location = new Point(516, 6);
            btnSearchDevice.Margin = new Padding(4, 6, 4, 6);
            btnSearchDevice.MouseState = MaterialSkin.MouseState.HOVER;
            btnSearchDevice.Name = "btnSearchDevice";
            btnSearchDevice.NoAccentTextColor = Color.Empty;
            btnSearchDevice.Size = new Size(120, 36);
            btnSearchDevice.TabIndex = 9;
            btnSearchDevice.Text = "장치 검색";
            btnSearchDevice.Type = MaterialButton.MaterialButtonType.Outlined;
            btnSearchDevice.UseAccentColor = false;
            btnSearchDevice.UseVisualStyleBackColor = true;
            // 
            // panelPresets
            // 
            panelPresets.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelPresets.AutoScroll = true;
            panelPresets.Location = new Point(20, 380);
            panelPresets.Name = "panelPresets";
            panelPresets.Size = new Size(760, 100);
            panelPresets.TabIndex = 10;
            // 
            // lblPresets
            // 
            lblPresets.AutoSize = true;
            lblPresets.Depth = 0;
            lblPresets.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblPresets.Location = new Point(20, 355);
            lblPresets.MouseState = MaterialSkin.MouseState.HOVER;
            lblPresets.Name = "lblPresets";
            lblPresets.Size = new Size(37, 19);
            lblPresets.TabIndex = 11;
            lblPresets.Text = "프리셋";
            // 
            // btnSavePreset
            // 
            btnSavePreset.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSavePreset.AutoSize = false;
            btnSavePreset.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnSavePreset.Density = MaterialButton.MaterialButtonDensity.Default;
            btnSavePreset.Depth = 0;
            btnSavePreset.HighEmphasis = true;
            btnSavePreset.Icon = null;
            btnSavePreset.Location = new Point(640, 347);
            btnSavePreset.Margin = new Padding(4, 6, 4, 6);
            btnSavePreset.MouseState = MaterialSkin.MouseState.HOVER;
            btnSavePreset.Name = "btnSavePreset";
            btnSavePreset.NoAccentTextColor = Color.Empty;
            btnSavePreset.Size = new Size(140, 32);
            btnSavePreset.TabIndex = 12;
            btnSavePreset.Text = "프리셋 저장";
            btnSavePreset.Type = MaterialButton.MaterialButtonType.Text;
            btnSavePreset.UseAccentColor = true;
            btnSavePreset.UseVisualStyleBackColor = true;
            // 
            // panelDeviceInfo
            // 
            panelDeviceInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelDeviceInfo.Location = new Point(20, 510);
            panelDeviceInfo.Name = "panelDeviceInfo";
            panelDeviceInfo.Size = new Size(760, 200);
            panelDeviceInfo.TabIndex = 13;
            // 
            // txtDeviceInfo
            // 
            txtDeviceInfo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtDeviceInfo.BackColor = Color.White;
            txtDeviceInfo.BorderStyle = BorderStyle.FixedSingle;
            txtDeviceInfo.Font = new Font("Consolas", 9F);
            txtDeviceInfo.Location = new Point(20, 535);
            txtDeviceInfo.Name = "txtDeviceInfo";
            txtDeviceInfo.ReadOnly = true;
            txtDeviceInfo.Size = new Size(760, 175);
            txtDeviceInfo.TabIndex = 14;
            txtDeviceInfo.Text = "";
            // 
            // lblDeviceInfo
            // 
            lblDeviceInfo.AutoSize = true;
            lblDeviceInfo.Depth = 0;
            lblDeviceInfo.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblDeviceInfo.Location = new Point(20, 490);
            lblDeviceInfo.MouseState = MaterialSkin.MouseState.HOVER;
            lblDeviceInfo.Name = "lblDeviceInfo";
            lblDeviceInfo.Size = new Size(53, 19);
            lblDeviceInfo.TabIndex = 15;
            lblDeviceInfo.Text = "장치 정보";
            lblDeviceInfo.Visible = false;
            // 
            // panelToast
            // 
            panelToast.Anchor = AnchorStyles.Top;
            panelToast.BackColor = Color.FromArgb(76, 175, 80);
            panelToast.Location = new Point(490, 38);
            panelToast.Name = "panelToast";
            panelToast.Size = new Size(300, 50);
            panelToast.TabIndex = 16;
            panelToast.Visible = false;
            // 
            // lblToast
            // 
            lblToast.Anchor = AnchorStyles.Top;
            lblToast.BackColor = Color.Transparent;
            lblToast.Depth = 0;
            lblToast.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblToast.ForeColor = Color.White;
            lblToast.Location = new Point(500, 48);
            lblToast.MouseState = MaterialSkin.MouseState.HOVER;
            lblToast.Name = "lblToast";
            lblToast.Size = new Size(280, 30);
            lblToast.TabIndex = 17;
            lblToast.TextAlign = ContentAlignment.MiddleCenter;
            lblToast.Visible = false;
            // 
            // panelButtons
            // 
            panelButtons.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelButtons.AutoSize = true;
            panelButtons.Controls.Add(btnApply);
            panelButtons.Controls.Add(btnAllOutputs);
            panelButtons.Controls.Add(btnGetStatus);
            panelButtons.Controls.Add(btnGetLcdStatus);
            panelButtons.Controls.Add(btnSearchDevice);
            panelButtons.Controls.Add(btnSettings);
            panelButtons.Location = new Point(20, 295);
            panelButtons.Name = "panelButtons";
            panelButtons.Padding = new Padding(0, 0, 0, 10);
            panelButtons.Size = new Size(760, 58);
            panelButtons.TabIndex = 18;
            // 
            // btnClearLog
            // 
            btnClearLog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearLog.AutoSize = false;
            btnClearLog.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnClearLog.Density = MaterialButton.MaterialButtonDensity.Default;
            btnClearLog.Depth = 0;
            btnClearLog.HighEmphasis = true;
            btnClearLog.Icon = null;
            btnClearLog.Location = new Point(680, 485);
            btnClearLog.Margin = new Padding(4, 6, 4, 6);
            btnClearLog.MouseState = MaterialSkin.MouseState.HOVER;
            btnClearLog.Name = "btnClearLog";
            btnClearLog.NoAccentTextColor = Color.Empty;
            btnClearLog.Size = new Size(100, 32);
            btnClearLog.TabIndex = 19;
            btnClearLog.Text = "로그 지우기";
            btnClearLog.Type = MaterialButton.MaterialButtonType.Text;
            btnClearLog.UseAccentColor = false;
            btnClearLog.UseVisualStyleBackColor = true;
            btnClearLog.Visible = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 730);
            Controls.Add(lblToast);
            Controls.Add(panelToast);
            Controls.Add(lblDeviceInfo);
            Controls.Add(btnClearLog);
            Controls.Add(txtDeviceInfo);
            Controls.Add(panelDeviceInfo);
            Controls.Add(btnSavePreset);
            Controls.Add(lblPresets);
            Controls.Add(panelPresets);
            Controls.Add(panelButtons);
            Controls.Add(lblOutputs);
            Controls.Add(panelOutputs);
            Controls.Add(lblInputs);
            Controls.Add(panelInputs);
            MinimumSize = new Size(800, 730);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AV Matrix Controller";
            panelButtons.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel panelInputs;
        private MaterialLabel lblInputs;
        private System.Windows.Forms.Panel panelOutputs;
        private MaterialLabel lblOutputs;
        private MaterialButton btnApply;
        private MaterialButton btnAllOutputs;
        private MaterialButton btnSettings;
        private MaterialButton btnGetStatus;
        private MaterialButton btnGetLcdStatus;
        private MaterialButton btnSearchDevice;
        private System.Windows.Forms.Panel panelPresets;
        private MaterialLabel lblPresets;
        private MaterialButton btnSavePreset;
        private System.Windows.Forms.Panel panelDeviceInfo;
        private System.Windows.Forms.RichTextBox txtDeviceInfo;
        private MaterialLabel lblDeviceInfo;
        private System.Windows.Forms.Panel panelToast;
        private MaterialLabel lblToast;
        private System.Windows.Forms.FlowLayoutPanel panelButtons;
        private MaterialButton btnClearLog;
    }
}
