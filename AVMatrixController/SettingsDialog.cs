using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AVMatrixController
{
    public class SettingsDialog : MaterialForm
    {
        private MaterialLabel lblIp;
        private MaterialTextBox txtIp;
        private MaterialLabel lblPort;
        private MaterialTextBox txtPort;
        private MaterialButton btnSave;
        private MaterialButton btnCancel;

        public string DeviceIp { get; private set; }
        public int DevicePort { get; private set; }

        public SettingsDialog(string currentIp, int currentPort)
        {
            DeviceIp = currentIp;
            DevicePort = currentPort;
            InitializeComponent();
            txtIp.Text = currentIp;
            txtPort.Text = currentPort.ToString();
        }

        private void InitializeComponent()
        {
            this.lblIp = new MaterialLabel();
            this.txtIp = new MaterialTextBox();
            this.lblPort = new MaterialLabel();
            this.txtPort = new MaterialTextBox();
            this.btnSave = new MaterialButton();
            this.btnCancel = new MaterialButton();
            this.SuspendLayout();

            // lblIp
            this.lblIp.AutoSize = true;
            this.lblIp.Depth = 0;
            this.lblIp.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            this.lblIp.Location = new Point(20, 80);
            this.lblIp.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblIp.Name = "lblIp";
            this.lblIp.Size = new Size(68, 19);
            this.lblIp.TabIndex = 0;
            this.lblIp.Text = "IP 주소:";

            // txtIp
            this.txtIp.AnimateReadOnly = false;
            this.txtIp.BorderStyle = BorderStyle.None;
            this.txtIp.Depth = 0;
            this.txtIp.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            this.txtIp.Hint = "192.168.1.200";
            this.txtIp.LeadingIcon = null;
            this.txtIp.Location = new Point(20, 110);
            this.txtIp.MaxLength = 50;
            this.txtIp.MouseState = MaterialSkin.MouseState.OUT;
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new Size(320, 50);
            this.txtIp.TabIndex = 1;
            this.txtIp.Text = "";
            this.txtIp.TrailingIcon = null;

            // lblPort
            this.lblPort.AutoSize = true;
            this.lblPort.Depth = 0;
            this.lblPort.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            this.lblPort.Location = new Point(20, 180);
            this.lblPort.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new Size(80, 19);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "포트 번호:";

            // txtPort
            this.txtPort.AnimateReadOnly = false;
            this.txtPort.BorderStyle = BorderStyle.None;
            this.txtPort.Depth = 0;
            this.txtPort.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            this.txtPort.Hint = "7000";
            this.txtPort.LeadingIcon = null;
            this.txtPort.Location = new Point(20, 210);
            this.txtPort.MaxLength = 5;
            this.txtPort.MouseState = MaterialSkin.MouseState.OUT;
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new Size(320, 50);
            this.txtPort.TabIndex = 3;
            this.txtPort.Text = "";
            this.txtPort.TrailingIcon = null;

            // btnSave
            this.btnSave.AutoSize = false;
            this.btnSave.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.btnSave.Density = MaterialButton.MaterialButtonDensity.Default;
            this.btnSave.Depth = 0;
            this.btnSave.HighEmphasis = true;
            this.btnSave.Icon = null;
            this.btnSave.Location = new Point(140, 290);
            this.btnSave.Margin = new Padding(4, 6, 4, 6);
            this.btnSave.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSave.Name = "btnSave";
            this.btnSave.NoAccentTextColor = Color.Empty;
            this.btnSave.Size = new Size(100, 36);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "저장";
            this.btnSave.Type = MaterialButton.MaterialButtonType.Contained;
            this.btnSave.UseAccentColor = false;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += BtnSave_Click;

            // btnCancel
            this.btnCancel.AutoSize = false;
            this.btnCancel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.btnCancel.Density = MaterialButton.MaterialButtonDensity.Default;
            this.btnCancel.Depth = 0;
            this.btnCancel.HighEmphasis = true;
            this.btnCancel.Icon = null;
            this.btnCancel.Location = new Point(250, 290);
            this.btnCancel.Margin = new Padding(4, 6, 4, 6);
            this.btnCancel.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NoAccentTextColor = Color.Empty;
            this.btnCancel.Size = new Size(100, 36);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "취소";
            this.btnCancel.Type = MaterialButton.MaterialButtonType.Text;
            this.btnCancel.UseAccentColor = false;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += BtnCancel_Click;

            // SettingsDialog
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(360, 350);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.txtIp);
            this.Controls.Add(this.lblIp);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "장치 설정";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIp.Text))
            {
                MessageBox.Show("IP 주소를 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtPort.Text, out int port) || port < 1 || port > 65535)
            {
                MessageBox.Show("올바른 포트 번호를 입력해주세요. (1-65535)", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DeviceIp = txtIp.Text.Trim();
            DevicePort = port;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
