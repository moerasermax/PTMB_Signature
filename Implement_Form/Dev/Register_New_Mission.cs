using API_SendMail.DataSet;
using Finance;
using Newtonsoft.Json;
using PTMB_Signature.Implement_Risk;
using PTMB_Signature.Model.Data_Set;
using PTMB_Signature.Model.Plugin;
using PTMB_Signature_API.Data_Set;
using PTMB_Signature_API.Model.Abstract;
using PTMB_Signature_API.Model.Implement;
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
    public partial class Register_New_Mission : Form
    {
        LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();


        public static Register_New_Mission instance = new Register_New_Mission();
        public static Register_New_Mission getInstance()
        {
            if(instance == null)
            {
                return instance = new Register_New_Mission();
            }
            return instance;
        }

        private Register_New_Mission()
        {
            InitializeComponent();
        }

        private void Mission_Register_Button_Click(object sender, EventArgs e)
        {
            Controller_Sign sign = Controller_Sign.getInstance();
            Data_Set_Excutre_Result result = sign_plugin.CompareTokenvalid();
            if (result.excute_result.isSuccesed)
            {
                try
                {
                    string[] risk_value = sign_plugin.Compare_New_Risk(Register_New_Mission_Amount_Textbox.Text).Split(','); /// 計算風險值。/// 資料格式：[{執行結果},{風險值}]

                    LOANIT_RISK loanit = LOANIT_RISK.getInstance();
                    loanit.initail_mission_object();
                    loanit.set_mission_object_data(Set_Mission_Data_Option.NAME, Mission_Name_Textbox.Text);
                    //loanit.set_mission_object_data(Set_Mission_Data_Option.TYPE, Set_Mission_MissionTypes.CH001.ToString());
                    loanit.set_mission_object_data(Set_Mission_Data_Option.TYPE, SubSysNo.CH001.ToString());
                    loanit.set_mission_object_data(Set_Mission_Data_Option.COMPANY, Company.LOANIT.ToString());
                    loanit.set_mission_object_data(Set_Mission_Data_Option.BINDING_PROJECT_ID, Mission_Binding_ProjectID_Textbox.Text);
                    loanit.set_mission_object_data(Set_Mission_Data_Option.RISK_VALUE, risk_value[1]);
                    loanit.set_mission_object_data(Set_Mission_Data_Option.require_SIGNATURE, loanit.get_mission_require_signature(1, SubSysNo.CH001)); /// 預設 1 開始
                    //loanit.set_mission_object_data(Set_Mission_Data_Option.require_SIGNATURE, loanit.get_mission_require_signature(int.Parse(risk_value[1]),SubSysNo.CH001)); ///基於評分卡出來的結果決定層級
                    //loanit.set_mission_object_data(Set_Mission_Data_Option.require_SIGNATURE, loanit.get_mission_require_signature(int.Parse(comboBox1.SelectedItem.ToString()))); //填入風險係數
                    //loanit.set_mission_object_data(Set_Mission_Data_Option.require_SIGNATURE, loanit.get_mission_require_signature(5)); //填入風險係數
                    Data_Set_Excutre_Result excute_result = JsonConvert.DeserializeObject<Data_Set_Excutre_Result>(sign_plugin.register_new_mission(loanit));

                    if (excute_result.excute_result.isSuccesed)
                    {
                        ConsoleLog_Textbox.Text += (String.Format("任務 {0} 已新增完成 \r\n", loanit.data_set_mission.name));
                    }
                    else
                    {
                        ConsoleLog_Textbox.Text += string.Format("任務 {0} 新增失敗 \r\n 失敗原因：{1}", loanit.data_set_mission.binding_project_id, excute_result.excute_result.feedb_back_message);
                    }
                }
                catch (Exception ex)
                {
                    ConsoleLog_Textbox.Text += (string.Format("錯誤\r\n原因為：{0}", ex.Message));
                }
            }
            else
            {
                ConsoleLog_Textbox.Text += String.Format("{0}\r\n", result.excute_result.feedb_back_message);
            }
        }

        private void Register_New_Mission_FormClosing(object sender, FormClosingEventArgs e)
        {
            instance = null;
            GC.Collect();
        }

        private void Register_New_Mission_Load(object sender, EventArgs e)
        {
            
        }
    }
}
