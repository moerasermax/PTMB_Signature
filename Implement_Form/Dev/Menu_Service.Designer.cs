namespace PTMB_Signature.Implement_Form
{
    partial class Menu_Service
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
            this.run_signature_system = new System.Windows.Forms.Button();
            this.run_register_new_mission = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.EmployeeLevelModify_Button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // run_signature_system
            // 
            this.run_signature_system.Location = new System.Drawing.Point(36, 128);
            this.run_signature_system.Name = "run_signature_system";
            this.run_signature_system.Size = new System.Drawing.Size(193, 35);
            this.run_signature_system.TabIndex = 0;
            this.run_signature_system.Text = "審批系統";
            this.run_signature_system.UseVisualStyleBackColor = true;
            this.run_signature_system.Click += new System.EventHandler(this.run_signature_system_Click);
            this.run_signature_system.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Menu_Service_KeyPress);
            // 
            // run_register_new_mission
            // 
            this.run_register_new_mission.Location = new System.Drawing.Point(36, 42);
            this.run_register_new_mission.Name = "run_register_new_mission";
            this.run_register_new_mission.Size = new System.Drawing.Size(193, 35);
            this.run_register_new_mission.TabIndex = 0;
            this.run_register_new_mission.Text = "建立審批任務";
            this.run_register_new_mission.UseVisualStyleBackColor = true;
            this.run_register_new_mission.Click += new System.EventHandler(this.run_register_new_mission_Click);
            this.run_register_new_mission.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Menu_Service_KeyPress);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(36, 207);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(193, 35);
            this.button1.TabIndex = 0;
            this.button1.Text = "帳本系統_嵌入簽核";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Menu_Service_KeyPress);
            // 
            // EmployeeLevelModify_Button
            // 
            this.EmployeeLevelModify_Button.Location = new System.Drawing.Point(335, 128);
            this.EmployeeLevelModify_Button.Name = "EmployeeLevelModify_Button";
            this.EmployeeLevelModify_Button.Size = new System.Drawing.Size(193, 35);
            this.EmployeeLevelModify_Button.TabIndex = 0;
            this.EmployeeLevelModify_Button.Text = "層級調整";
            this.EmployeeLevelModify_Button.UseVisualStyleBackColor = true;
            this.EmployeeLevelModify_Button.Visible = false;
            this.EmployeeLevelModify_Button.Click += new System.EventHandler(this.EmployeeLevelModify_Button_Click);
            // 
            // Menu_Service
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 272);
            this.Controls.Add(this.run_register_new_mission);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.EmployeeLevelModify_Button);
            this.Controls.Add(this.run_signature_system);
            this.Name = "Menu_Service";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Menu_Service_FormClosing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Menu_Service_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button run_signature_system;
        private System.Windows.Forms.Button run_register_new_mission;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button EmployeeLevelModify_Button;
    }
}