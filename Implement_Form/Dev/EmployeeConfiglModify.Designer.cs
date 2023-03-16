namespace PTMB_Signature.Implement_Form.Dev
{
    partial class EmployeeConfiglModify
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmployeeConfiglModify));
            this.Function_TabControl = new MaterialSkin.Controls.MaterialTabControl();
            this.Setting = new System.Windows.Forms.TabPage();
            this.SignEmployeeLevel = new System.Windows.Forms.TabPage();
            this.DeleteUser_Button = new MaterialSkin.Controls.MaterialButton();
            this.AddUser_Button = new MaterialSkin.Controls.MaterialButton();
            this.materialButton1 = new MaterialSkin.Controls.MaterialButton();
            this.NewEmployeeLevel_Combobox = new MaterialSkin.Controls.MaterialComboBox();
            this.Person_Combobox = new MaterialSkin.Controls.MaterialComboBox();
            this.EmployeeLevel_Combobox = new MaterialSkin.Controls.MaterialComboBox();
            this.MissionType_Combobox = new MaterialSkin.Controls.MaterialComboBox();
            this.SignEmployeeData = new System.Windows.Forms.TabPage();
            this.MailContractData = new System.Windows.Forms.TabPage();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.materialMultiLineTextBox1 = new MaterialSkin.Controls.MaterialMultiLineTextBox();
            this.UI_ProgressBar1 = new System.Windows.Forms.ProgressBar();
            this.Function_TabControl.SuspendLayout();
            this.SignEmployeeLevel.SuspendLayout();
            this.SuspendLayout();
            // 
            // Function_TabControl
            // 
            this.Function_TabControl.Controls.Add(this.Setting);
            this.Function_TabControl.Controls.Add(this.SignEmployeeLevel);
            this.Function_TabControl.Controls.Add(this.SignEmployeeData);
            this.Function_TabControl.Controls.Add(this.MailContractData);
            this.Function_TabControl.Depth = 0;
            this.Function_TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Function_TabControl.ImageList = this.imageList1;
            this.Function_TabControl.Location = new System.Drawing.Point(3, 64);
            this.Function_TabControl.MouseState = MaterialSkin.MouseState.HOVER;
            this.Function_TabControl.Multiline = true;
            this.Function_TabControl.Name = "Function_TabControl";
            this.Function_TabControl.SelectedIndex = 0;
            this.Function_TabControl.Size = new System.Drawing.Size(708, 401);
            this.Function_TabControl.TabIndex = 0;
            this.Function_TabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.Function_TabControl_Selecting);
            // 
            // Setting
            // 
            this.Setting.ImageKey = "Setting.png";
            this.Setting.Location = new System.Drawing.Point(4, 39);
            this.Setting.Name = "Setting";
            this.Setting.Padding = new System.Windows.Forms.Padding(3);
            this.Setting.Size = new System.Drawing.Size(700, 358);
            this.Setting.TabIndex = 0;
            this.Setting.Text = "設置";
            this.Setting.UseVisualStyleBackColor = true;
            // 
            // SignEmployeeLevel
            // 
            this.SignEmployeeLevel.BackColor = System.Drawing.Color.White;
            this.SignEmployeeLevel.Controls.Add(this.DeleteUser_Button);
            this.SignEmployeeLevel.Controls.Add(this.AddUser_Button);
            this.SignEmployeeLevel.Controls.Add(this.materialButton1);
            this.SignEmployeeLevel.Controls.Add(this.NewEmployeeLevel_Combobox);
            this.SignEmployeeLevel.Controls.Add(this.Person_Combobox);
            this.SignEmployeeLevel.Controls.Add(this.EmployeeLevel_Combobox);
            this.SignEmployeeLevel.Controls.Add(this.MissionType_Combobox);
            this.SignEmployeeLevel.ImageKey = "contract.png";
            this.SignEmployeeLevel.Location = new System.Drawing.Point(4, 39);
            this.SignEmployeeLevel.Name = "SignEmployeeLevel";
            this.SignEmployeeLevel.Padding = new System.Windows.Forms.Padding(3);
            this.SignEmployeeLevel.Size = new System.Drawing.Size(700, 358);
            this.SignEmployeeLevel.TabIndex = 1;
            this.SignEmployeeLevel.Text = "逐級層級";
            // 
            // DeleteUser_Button
            // 
            this.DeleteUser_Button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DeleteUser_Button.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.DeleteUser_Button.Depth = 0;
            this.DeleteUser_Button.HighEmphasis = true;
            this.DeleteUser_Button.Icon = null;
            this.DeleteUser_Button.Location = new System.Drawing.Point(161, 313);
            this.DeleteUser_Button.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.DeleteUser_Button.MouseState = MaterialSkin.MouseState.HOVER;
            this.DeleteUser_Button.Name = "DeleteUser_Button";
            this.DeleteUser_Button.NoAccentTextColor = System.Drawing.Color.Empty;
            this.DeleteUser_Button.Size = new System.Drawing.Size(73, 36);
            this.DeleteUser_Button.TabIndex = 2;
            this.DeleteUser_Button.Text = "Delete";
            this.DeleteUser_Button.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.DeleteUser_Button.UseAccentColor = true;
            this.DeleteUser_Button.UseVisualStyleBackColor = true;
            this.DeleteUser_Button.Click += new System.EventHandler(this.DeleteUser_Button_Click);
            // 
            // AddUser_Button
            // 
            this.AddUser_Button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AddUser_Button.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.AddUser_Button.Depth = 0;
            this.AddUser_Button.HighEmphasis = true;
            this.AddUser_Button.Icon = null;
            this.AddUser_Button.Location = new System.Drawing.Point(262, 259);
            this.AddUser_Button.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.AddUser_Button.MouseState = MaterialSkin.MouseState.HOVER;
            this.AddUser_Button.Name = "AddUser_Button";
            this.AddUser_Button.NoAccentTextColor = System.Drawing.Color.Empty;
            this.AddUser_Button.Size = new System.Drawing.Size(64, 36);
            this.AddUser_Button.TabIndex = 2;
            this.AddUser_Button.Text = "Add";
            this.AddUser_Button.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.AddUser_Button.UseAccentColor = false;
            this.AddUser_Button.UseVisualStyleBackColor = true;
            this.AddUser_Button.Click += new System.EventHandler(this.AddUser_Button_Click);
            // 
            // materialButton1
            // 
            this.materialButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.materialButton1.Depth = 0;
            this.materialButton1.HighEmphasis = true;
            this.materialButton1.Icon = null;
            this.materialButton1.Location = new System.Drawing.Point(85, 259);
            this.materialButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialButton1.Name = "materialButton1";
            this.materialButton1.NoAccentTextColor = System.Drawing.Color.Empty;
            this.materialButton1.Size = new System.Drawing.Size(64, 36);
            this.materialButton1.TabIndex = 2;
            this.materialButton1.Text = "Save";
            this.materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.materialButton1.UseAccentColor = false;
            this.materialButton1.UseVisualStyleBackColor = true;
            this.materialButton1.Click += new System.EventHandler(this.materialButton1_Click);
            // 
            // NewEmployeeLevel_Combobox
            // 
            this.NewEmployeeLevel_Combobox.AutoResize = false;
            this.NewEmployeeLevel_Combobox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.NewEmployeeLevel_Combobox.Depth = 0;
            this.NewEmployeeLevel_Combobox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.NewEmployeeLevel_Combobox.DropDownHeight = 174;
            this.NewEmployeeLevel_Combobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NewEmployeeLevel_Combobox.DropDownWidth = 121;
            this.NewEmployeeLevel_Combobox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.NewEmployeeLevel_Combobox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.NewEmployeeLevel_Combobox.FormattingEnabled = true;
            this.NewEmployeeLevel_Combobox.Hint = "NewEmployeeLevel";
            this.NewEmployeeLevel_Combobox.IntegralHeight = false;
            this.NewEmployeeLevel_Combobox.ItemHeight = 43;
            this.NewEmployeeLevel_Combobox.Location = new System.Drawing.Point(36, 172);
            this.NewEmployeeLevel_Combobox.MaxDropDownItems = 4;
            this.NewEmployeeLevel_Combobox.MouseState = MaterialSkin.MouseState.OUT;
            this.NewEmployeeLevel_Combobox.Name = "NewEmployeeLevel_Combobox";
            this.NewEmployeeLevel_Combobox.Size = new System.Drawing.Size(341, 49);
            this.NewEmployeeLevel_Combobox.StartIndex = 0;
            this.NewEmployeeLevel_Combobox.TabIndex = 1;
            this.NewEmployeeLevel_Combobox.UseAccent = false;
            // 
            // Person_Combobox
            // 
            this.Person_Combobox.AutoResize = false;
            this.Person_Combobox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.Person_Combobox.Depth = 0;
            this.Person_Combobox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.Person_Combobox.DropDownHeight = 174;
            this.Person_Combobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Person_Combobox.DropDownWidth = 121;
            this.Person_Combobox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.Person_Combobox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Person_Combobox.FormattingEnabled = true;
            this.Person_Combobox.Hint = "Person";
            this.Person_Combobox.IntegralHeight = false;
            this.Person_Combobox.ItemHeight = 43;
            this.Person_Combobox.Location = new System.Drawing.Point(36, 96);
            this.Person_Combobox.MaxDropDownItems = 4;
            this.Person_Combobox.MouseState = MaterialSkin.MouseState.OUT;
            this.Person_Combobox.Name = "Person_Combobox";
            this.Person_Combobox.Size = new System.Drawing.Size(341, 49);
            this.Person_Combobox.StartIndex = 0;
            this.Person_Combobox.TabIndex = 1;
            this.Person_Combobox.UseAccent = false;
            this.Person_Combobox.SelectedIndexChanged += new System.EventHandler(this.Person_Combobox_SelectedIndexChanged);
            // 
            // EmployeeLevel_Combobox
            // 
            this.EmployeeLevel_Combobox.AutoResize = false;
            this.EmployeeLevel_Combobox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.EmployeeLevel_Combobox.Depth = 0;
            this.EmployeeLevel_Combobox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.EmployeeLevel_Combobox.DropDownHeight = 174;
            this.EmployeeLevel_Combobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EmployeeLevel_Combobox.DropDownWidth = 121;
            this.EmployeeLevel_Combobox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.EmployeeLevel_Combobox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.EmployeeLevel_Combobox.FormattingEnabled = true;
            this.EmployeeLevel_Combobox.Hint = "EmployeeLevel";
            this.EmployeeLevel_Combobox.IntegralHeight = false;
            this.EmployeeLevel_Combobox.ItemHeight = 43;
            this.EmployeeLevel_Combobox.Location = new System.Drawing.Point(222, 17);
            this.EmployeeLevel_Combobox.MaxDropDownItems = 4;
            this.EmployeeLevel_Combobox.MouseState = MaterialSkin.MouseState.OUT;
            this.EmployeeLevel_Combobox.Name = "EmployeeLevel_Combobox";
            this.EmployeeLevel_Combobox.Size = new System.Drawing.Size(155, 49);
            this.EmployeeLevel_Combobox.StartIndex = 0;
            this.EmployeeLevel_Combobox.TabIndex = 1;
            this.EmployeeLevel_Combobox.UseAccent = false;
            this.EmployeeLevel_Combobox.SelectedIndexChanged += new System.EventHandler(this.MissionLevel_Combobox_SelectedIndexChanged);
            // 
            // MissionType_Combobox
            // 
            this.MissionType_Combobox.AutoResize = false;
            this.MissionType_Combobox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.MissionType_Combobox.Depth = 0;
            this.MissionType_Combobox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.MissionType_Combobox.DropDownHeight = 174;
            this.MissionType_Combobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MissionType_Combobox.DropDownWidth = 121;
            this.MissionType_Combobox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.MissionType_Combobox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.MissionType_Combobox.FormattingEnabled = true;
            this.MissionType_Combobox.Hint = "MissionType";
            this.MissionType_Combobox.IntegralHeight = false;
            this.MissionType_Combobox.ItemHeight = 43;
            this.MissionType_Combobox.Location = new System.Drawing.Point(36, 17);
            this.MissionType_Combobox.MaxDropDownItems = 4;
            this.MissionType_Combobox.MouseState = MaterialSkin.MouseState.OUT;
            this.MissionType_Combobox.Name = "MissionType_Combobox";
            this.MissionType_Combobox.Size = new System.Drawing.Size(148, 49);
            this.MissionType_Combobox.StartIndex = 0;
            this.MissionType_Combobox.TabIndex = 1;
            this.MissionType_Combobox.UseAccent = false;
            this.MissionType_Combobox.SelectedIndexChanged += new System.EventHandler(this.MissionType_Combobox_SelectedIndexChanged);
            // 
            // SignEmployeeData
            // 
            this.SignEmployeeData.ImageKey = "group.png";
            this.SignEmployeeData.Location = new System.Drawing.Point(4, 39);
            this.SignEmployeeData.Name = "SignEmployeeData";
            this.SignEmployeeData.Size = new System.Drawing.Size(700, 358);
            this.SignEmployeeData.TabIndex = 2;
            this.SignEmployeeData.Text = "逐級人員";
            this.SignEmployeeData.UseVisualStyleBackColor = true;
            // 
            // MailContractData
            // 
            this.MailContractData.ImageKey = "phone-book.png";
            this.MailContractData.Location = new System.Drawing.Point(4, 39);
            this.MailContractData.Name = "MailContractData";
            this.MailContractData.Size = new System.Drawing.Size(700, 358);
            this.MailContractData.TabIndex = 3;
            this.MailContractData.Text = "集團員工資料";
            this.MailContractData.UseVisualStyleBackColor = true;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Setting.png");
            this.imageList1.Images.SetKeyName(1, "contract.png");
            this.imageList1.Images.SetKeyName(2, "group.png");
            this.imageList1.Images.SetKeyName(3, "phone-book.png");
            // 
            // materialMultiLineTextBox1
            // 
            this.materialMultiLineTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.materialMultiLineTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.materialMultiLineTextBox1.Depth = 0;
            this.materialMultiLineTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.materialMultiLineTextBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialMultiLineTextBox1.Location = new System.Drawing.Point(17, 17);
            this.materialMultiLineTextBox1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialMultiLineTextBox1.Name = "materialMultiLineTextBox1";
            this.materialMultiLineTextBox1.Size = new System.Drawing.Size(210, 263);
            this.materialMultiLineTextBox1.TabIndex = 0;
            this.materialMultiLineTextBox1.Text = "";
            // 
            // UI_ProgressBar1
            // 
            this.UI_ProgressBar1.Location = new System.Drawing.Point(0, 54);
            this.UI_ProgressBar1.Name = "UI_ProgressBar1";
            this.UI_ProgressBar1.Size = new System.Drawing.Size(715, 10);
            this.UI_ProgressBar1.TabIndex = 3;
            // 
            // EmployeeConfiglModify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 468);
            this.Controls.Add(this.UI_ProgressBar1);
            this.Controls.Add(this.Function_TabControl);
            this.DrawerShowIconsWhenHidden = true;
            this.DrawerTabControl = this.Function_TabControl;
            this.Name = "EmployeeConfiglModify";
            this.Text = "YC_DashBorad";
            this.Function_TabControl.ResumeLayout(false);
            this.SignEmployeeLevel.ResumeLayout(false);
            this.SignEmployeeLevel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MaterialSkin.Controls.MaterialTabControl Function_TabControl;
        private System.Windows.Forms.TabPage Setting;
        private System.Windows.Forms.TabPage SignEmployeeLevel;
        private System.Windows.Forms.TabPage SignEmployeeData;
        private System.Windows.Forms.TabPage MailContractData;
        private System.Windows.Forms.ImageList imageList1;
        private MaterialSkin.Controls.MaterialComboBox EmployeeLevel_Combobox;
        private MaterialSkin.Controls.MaterialComboBox MissionType_Combobox;
        private MaterialSkin.Controls.MaterialComboBox NewEmployeeLevel_Combobox;
        private MaterialSkin.Controls.MaterialComboBox Person_Combobox;
        private MaterialSkin.Controls.MaterialButton materialButton1;
        private MaterialSkin.Controls.MaterialMultiLineTextBox materialMultiLineTextBox1;
        private MaterialSkin.Controls.MaterialButton AddUser_Button;
        private System.Windows.Forms.ProgressBar UI_ProgressBar1;
        private MaterialSkin.Controls.MaterialButton DeleteUser_Button;
    }
}