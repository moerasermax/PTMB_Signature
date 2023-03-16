using PTMB_Signature.Implement_Form.Dev;
using PTMB_Signature_API.Data_Set;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PTMB_Signature.Implement_Form
{
    public partial class Menu_Service : Form
    {
        public Data_Set_Employee employee_data = null;


        public static Menu_Service instance = new Menu_Service();
        public static Menu_Service getInstance()
        {
            if(instance == null)
            {
                return instance = new Menu_Service();
            }
            return instance;
        }

        public void Init_Form(Data_Set_Employee receive_employee_data)
        {
            employee_data = receive_employee_data;

        }

        private Menu_Service()
        {
            InitializeComponent();
        }

        private void run_signature_system_Click(object sender, EventArgs e)
        {
            Form_Sign sign_form = Form_Sign.getInstance();
            sign_form.Initial_Form(employee_data);
            sign_form.employee_data = employee_data;
            sign_form.Show();
        }


        private void run_register_new_mission_Click(object sender, EventArgs e)
        {
            Register_New_Mission register_form = Register_New_Mission.getInstance();
            register_form.Show();

        }




        private void Menu_Service_FormClosing(object sender, FormClosingEventArgs e)
        {
            instance = null;
            GC.Collect();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AccountSystem_Signature es = new AccountSystem_Signature();
            es.Show();
        }



        public string Password { get; set; }
        private void Menu_Service_KeyPress(object sender, KeyPressEventArgs e)
        {
            Password += e.KeyChar;

            if (Password.Contains("8"))
            {
                this.Width = 582;
                EmployeeLevelModify_Button.Visible = true;
            }
        }

        private void EmployeeLevelModify_Button_Click(object sender, EventArgs e)
        {
            EmployeeConfiglModify employeeconfiglmodify_form = new EmployeeConfiglModify();
            employeeconfiglmodify_form.Show();
        }
    }
}
