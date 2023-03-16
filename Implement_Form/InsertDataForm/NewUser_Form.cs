using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PTMB_Signature.Implement_Form.InsertDataForm
{
    public partial class NewUser_Form : MaterialForm
    {
        public string ResponeData { get; set; }
        public bool Count_Confirm { get; set; }
        public  NewUser_Form()
        {
            InitializeComponent();
        }

        private void AddCustomer_Button_Click(object sender, EventArgs e)
        {
            if (Count_Confirm)
            {
                ResponeData = MainSysAccount_TextBox.Text;
                this.DialogResult= DialogResult.OK;
            }
            else
            {
                Count_Confirm= true;
                AddCustomer_Button.Text = "Confirm";
            }
        }


    }
}
