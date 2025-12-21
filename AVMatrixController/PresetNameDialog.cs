using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AVMatrixController
{
    public class PresetNameDialog : MaterialForm
    {
        private MaterialLabel lblName;
        private MaterialTextBox txtName;
        private MaterialButton btnSave;
        private MaterialButton btnCancel;

        public string PresetName { get; private set; } = "";

        public PresetNameDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.lblName = new MaterialLabel();
            this.txtName = new MaterialTextBox();
            this.btnSave = new MaterialButton();
            this.btnCancel = new MaterialButton();
            this.SuspendLayout();

            // lblName
            this.lblName.AutoSize = true;
            this.lblName.Depth = 0;
            this.lblName.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            this.lblName.Location = new Point(20, 80);
            this.lblName.MouseState = MaterialSkin.MouseState.HOVER;
            this.lblName.Name = "lblName";
            this.lblName.Size = new Size(105, 19);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "프리셋 이름:";

            // txtName
            this.txtName.AnimateReadOnly = false;
            this.txtName.BorderStyle = BorderStyle.None;
            this.txtName.Depth = 0;
            this.txtName.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            this.txtName.Hint = "프리셋 이름을 입력하세요";
            this.txtName.LeadingIcon = null;
            this.txtName.Location = new Point(20, 110);
            this.txtName.MaxLength = 50;
            this.txtName.MouseState = MaterialSkin.MouseState.OUT;
            this.txtName.Name = "txtName";
            this.txtName.Size = new Size(320, 50);
            this.txtName.TabIndex = 1;
            this.txtName.Text = "";
            this.txtName.TrailingIcon = null;

            // btnSave
            this.btnSave.AutoSize = false;
            this.btnSave.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.btnSave.Density = MaterialButton.MaterialButtonDensity.Default;
            this.btnSave.Depth = 0;
            this.btnSave.HighEmphasis = true;
            this.btnSave.Icon = null;
            this.btnSave.Location = new Point(140, 190);
            this.btnSave.Margin = new Padding(4, 6, 4, 6);
            this.btnSave.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSave.Name = "btnSave";
            this.btnSave.NoAccentTextColor = Color.Empty;
            this.btnSave.Size = new Size(100, 36);
            this.btnSave.TabIndex = 2;
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
            this.btnCancel.Location = new Point(250, 190);
            this.btnCancel.Margin = new Padding(4, 6, 4, 6);
            this.btnCancel.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.NoAccentTextColor = Color.Empty;
            this.btnCancel.Size = new Size(100, 36);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "취소";
            this.btnCancel.Type = MaterialButton.MaterialButtonType.Text;
            this.btnCancel.UseAccentColor = false;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += BtnCancel_Click;

            // PresetNameDialog
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(360, 250);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PresetNameDialog";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "프리셋 저장";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("프리셋 이름을 입력해주세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PresetName = txtName.Text.Trim();
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
