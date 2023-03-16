namespace PTMB_Signature.Implement_Form
{
    partial class Login_Account
    {
        /// <summary>
        /// required designer variable.
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
        /// required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Account_Label = new System.Windows.Forms.Label();
            this.Login = new System.Windows.Forms.Button();
            this.Login_Groupbox = new System.Windows.Forms.GroupBox();
            this.Password_Textbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.Console_groupBox = new System.Windows.Forms.GroupBox();
            this.Console_Textbox = new System.Windows.Forms.TextBox();
            this.Login_Groupbox.SuspendLayout();
            this.Console_groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // Account_Label
            // 
            this.Account_Label.AutoSize = true;
            this.Account_Label.Location = new System.Drawing.Point(19, 27);
            this.Account_Label.Name = "Account_Label";
            this.Account_Label.Size = new System.Drawing.Size(56, 12);
            this.Account_Label.TabIndex = 0;
            this.Account_Label.Text = "Account：";
            // 
            // Login
            // 
            this.Login.Location = new System.Drawing.Point(56, 115);
            this.Login.Name = "Login";
            this.Login.Size = new System.Drawing.Size(75, 23);
            this.Login.TabIndex = 2;
            this.Login.Text = "登入";
            this.Login.UseVisualStyleBackColor = true;
            this.Login.Click += new System.EventHandler(this.Login_Click);
            // 
            // Login_Groupbox
            // 
            this.Login_Groupbox.Controls.Add(this.Password_Textbox);
            this.Login_Groupbox.Controls.Add(this.label1);
            this.Login_Groupbox.Controls.Add(this.comboBox1);
            this.Login_Groupbox.Controls.Add(this.Login);
            this.Login_Groupbox.Controls.Add(this.Account_Label);
            this.Login_Groupbox.Location = new System.Drawing.Point(12, 12);
            this.Login_Groupbox.Name = "Login_Groupbox";
            this.Login_Groupbox.Size = new System.Drawing.Size(202, 159);
            this.Login_Groupbox.TabIndex = 3;
            this.Login_Groupbox.TabStop = false;
            this.Login_Groupbox.Text = "展示登入介面";
            // 
            // Password_Textbox
            // 
            this.Password_Textbox.Location = new System.Drawing.Point(72, 69);
            this.Password_Textbox.Name = "Password_Textbox";
            this.Password_Textbox.Size = new System.Drawing.Size(110, 22);
            this.Password_Textbox.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "Password：";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "minghan",
            "T123992133",
            "winks880046",
            "leokuo",
            "test",
            "tammy",
            "TKFLYC0509",
            "Error_Account",
            "P0006",
            "Angela"});
            this.comboBox1.Location = new System.Drawing.Point(72, 21);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(110, 20);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // Console_groupBox
            // 
            this.Console_groupBox.Controls.Add(this.Console_Textbox);
            this.Console_groupBox.Location = new System.Drawing.Point(14, 177);
            this.Console_groupBox.Name = "Console_groupBox";
            this.Console_groupBox.Size = new System.Drawing.Size(200, 117);
            this.Console_groupBox.TabIndex = 4;
            this.Console_groupBox.TabStop = false;
            this.Console_groupBox.Text = "運算結果";
            // 
            // Console_Textbox
            // 
            this.Console_Textbox.Location = new System.Drawing.Point(6, 21);
            this.Console_Textbox.Multiline = true;
            this.Console_Textbox.Name = "Console_Textbox";
            this.Console_Textbox.ReadOnly = true;
            this.Console_Textbox.Size = new System.Drawing.Size(188, 90);
            this.Console_Textbox.TabIndex = 0;
            // 
            // Login_Account
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(225, 306);
            this.Controls.Add(this.Console_groupBox);
            this.Controls.Add(this.Login_Groupbox);
            this.Name = "Login_Account";
            this.Text = "Login_Account";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Login_Account_KeyPress);
            this.Login_Groupbox.ResumeLayout(false);
            this.Login_Groupbox.PerformLayout();
            this.Console_groupBox.ResumeLayout(false);
            this.Console_groupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Account_Label;
        private System.Windows.Forms.Button Login;
        private System.Windows.Forms.GroupBox Login_Groupbox;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox Console_groupBox;
        private System.Windows.Forms.TextBox Console_Textbox;
        private System.Windows.Forms.TextBox Password_Textbox;
        private System.Windows.Forms.Label label1;
    }
}