namespace PTMB_Signature.Implement_Form.InsertDataForm
{
    partial class NewUser_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainSysAccount_TextBox = new MaterialSkin.Controls.MaterialMaskedTextBox();
            this.AddCustomer_Button = new MaterialSkin.Controls.MaterialButton();
            this.SuspendLayout();
            // 
            // MainSysAccount_TextBox
            // 
            this.MainSysAccount_TextBox.AllowPromptAsInput = true;
            this.MainSysAccount_TextBox.AnimateReadOnly = false;
            this.MainSysAccount_TextBox.AsciiOnly = false;
            this.MainSysAccount_TextBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.MainSysAccount_TextBox.BeepOnError = false;
            this.MainSysAccount_TextBox.CutCopyMaskFormat = System.Windows.Forms.MaskFormat.IncludeLiterals;
            this.MainSysAccount_TextBox.Depth = 0;
            this.MainSysAccount_TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.MainSysAccount_TextBox.HidePromptOnLeave = false;
            this.MainSysAccount_TextBox.HideSelection = true;
            this.MainSysAccount_TextBox.Hint = "MainSysAccount";
            this.MainSysAccount_TextBox.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Default;
            this.MainSysAccount_TextBox.LeadingIcon = null;
            this.MainSysAccount_TextBox.Location = new System.Drawing.Point(81, 80);
            this.MainSysAccount_TextBox.Mask = "";
            this.MainSysAccount_TextBox.MaxLength = 32767;
            this.MainSysAccount_TextBox.MouseState = MaterialSkin.MouseState.OUT;
            this.MainSysAccount_TextBox.Name = "MainSysAccount_TextBox";
            this.MainSysAccount_TextBox.PasswordChar = '\0';
            this.MainSysAccount_TextBox.PrefixSuffixText = null;
            this.MainSysAccount_TextBox.PromptChar = '_';
            this.MainSysAccount_TextBox.ReadOnly = false;
            this.MainSysAccount_TextBox.RejectInputOnFirstFailure = false;
            this.MainSysAccount_TextBox.ResetOnPrompt = true;
            this.MainSysAccount_TextBox.ResetOnSpace = true;
            this.MainSysAccount_TextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.MainSysAccount_TextBox.SelectedText = "";
            this.MainSysAccount_TextBox.SelectionLength = 0;
            this.MainSysAccount_TextBox.SelectionStart = 0;
            this.MainSysAccount_TextBox.ShortcutsEnabled = true;
            this.MainSysAccount_TextBox.Size = new System.Drawing.Size(250, 48);
            this.MainSysAccount_TextBox.SkipLiterals = true;
            this.MainSysAccount_TextBox.TabIndex = 0;
            this.MainSysAccount_TextBox.TabStop = false;
            this.MainSysAccount_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.MainSysAccount_TextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.IncludeLiterals;
            this.MainSysAccount_TextBox.TrailingIcon = null;
            this.MainSysAccount_TextBox.UseAccent = false;
            this.MainSysAccount_TextBox.UseSystemPasswordChar = false;
            this.MainSysAccount_TextBox.ValidatingType = null;
            // 
            // AddCustomer_Button
            // 
            this.AddCustomer_Button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AddCustomer_Button.BackColor = System.Drawing.Color.Black;
            this.AddCustomer_Button.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.AddCustomer_Button.Depth = 0;
            this.AddCustomer_Button.HighEmphasis = true;
            this.AddCustomer_Button.Icon = null;
            this.AddCustomer_Button.Location = new System.Drawing.Point(168, 149);
            this.AddCustomer_Button.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.AddCustomer_Button.MouseState = MaterialSkin.MouseState.HOVER;
            this.AddCustomer_Button.Name = "AddCustomer_Button";
            this.AddCustomer_Button.NoAccentTextColor = System.Drawing.Color.Empty;
            this.AddCustomer_Button.Size = new System.Drawing.Size(64, 36);
            this.AddCustomer_Button.TabIndex = 1;
            this.AddCustomer_Button.Text = "Add";
            this.AddCustomer_Button.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.AddCustomer_Button.UseAccentColor = false;
            this.AddCustomer_Button.UseVisualStyleBackColor = false;
            this.AddCustomer_Button.Click += new System.EventHandler(this.AddCustomer_Button_Click);
            // 
            // NewUser_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 194);
            this.Controls.Add(this.AddCustomer_Button);
            this.Controls.Add(this.MainSysAccount_TextBox);
            this.Name = "NewUser_Form";
            this.Text = "NewUser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialMaskedTextBox MainSysAccount_TextBox;
        private MaterialSkin.Controls.MaterialButton AddCustomer_Button;
    }
}