using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PTMB_Signature_API.Data_Set;

using PTMB_Signature.Model.Form_UI;
using System.Linq.Expressions;
using PTMB_Signature_API.Informatio_Set;
using PTMB_Signature_API.Model.Abstract;
using PTMB_Signature.Implement_Risk;
using PTMB_Signature_API.Model.Implement;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using PTMB_Signature_API.Data_Set.Login;
using Button = System.Windows.Forms.Button;
using TextBox = System.Windows.Forms.TextBox;
using PTMB_Signature_API.Model.Implement.DAO;
using PTMB_Signature.Model;
using PTMB_Signature.Model.DAO;
using Sql_Action_Option = PTMB_Signature.Model.Sql_Action_Option;
using PTMB_Signature.Model.Data_Set;
using PTMB_Signature.Model.Plugin;
using System.Diagnostics;
using Finance;
using Microsoft.SqlServer.Server;
using New_Customer_Submit.API.Controller;
using System.IO;
using VsPipe;
using PTMB_Signature.Implement_Form.Account_System;
using System.Security.Cryptography;
using ScorecardAPI.APIs;
using API_SendMail;
using ScorecardAPI;
using ScorecardAPI.Repositories.Tables;
using System.Collections;
using System.Threading;

namespace PTMB_Signature.Implement_Form
{
    /// <summary>
    /// 因設計邏輯上，資料使用：最大簽層用數字遞減式表示，未來較好新增更大多簽核；舉例：若今天需在增加一位 層級5 的審核，只需要新增一個 level5 的層級即可；反之若是 遞增式 則需一直調動所有人
    /// 實務上畫面上顯示：4層級 為 第1關、3層級 為 第2關、2層級 為 第3關、1層級 為 第4關；
    /// </summary>
    public partial class Form_Sign : Form
    {

        public string[] args = null;
        public string Current_Select_Mission_No; /// 目前選取的MissionID
        public Data_Set_Employee employee_data = null;
        public List<Data_Set_Mission_Employee_Level_LOANIT> data_set_mission_employee_level = new List<Data_Set_Mission_Employee_Level_LOANIT>();
        LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
        bool excute_done_compare = false; bool special_note = false;


        #region 更新UI
        public void LoadMissionSummary(Data_Set_Mission_Details mission_data)
        {
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            if (mission_data.binding_project_id.Contains("_X"))
            {
                string HighestEmployeeLevel = sign_plugin.get_HighestEmployeeRequireLevel(mission_data);
                Sibling_Mission_CurrentAmount_Label.Text = changeToMoneyType(sign_plugin.computing_CurrentAmount_SiblingRule(mission_data, int.Parse(sign_plugin.get_CurrentAmount_SiblingRule(mission_data, HighestEmployeeLevel)), employee_data, HighestEmployeeLevel));
                Sibling_Mission_CurrentAmount_Tiitle_Label.Visible = true;
                Sibling_Mission_CurrentAmount_Label.Visible = true;

            }
            else
            {
                Mission_CurrentAmount_Label.Text = changeToMoneyType(decimal.Parse(sign_plugin.get_mission_curent_amount(mission_data)));
                Sibling_Mission_CurrentAmount_Tiitle_Label.Visible = false;
                Sibling_Mission_CurrentAmount_Label.Visible = false;
            }
            Mission_BindingProjectID_Label.Text = mission_data.binding_project_id;
            Mission_CreateTime_Label.Text = mission_data.create_time;
            Mission_Status_Label.Text = sign_plugin.get_missionStatusDescrption(mission_data.status_id);
            Mission_CurrentProcessRatio_Label.Text = sign_plugin.get_mission_curent_processratio(mission_data);
            Mission_CurrentRate_Label.Text = sign_plugin.get_mission_curent_rate(mission_data);

        }
        public void UpdateMissionStatusVision(string pass_level, List<Data_Set_Mission_require> list_require_sign)
        {
            string app_path = Directory.GetParent(System.IO.Path.GetDirectoryName(Application.ExecutablePath)).FullName + @"\SubSys_SignSystem\";

            if (pass_level.Equals("0"))///這裡指數量
            {
                switch (list_require_sign.Count.ToString())
                {
                    case "1":
                        Position_Level1.Text = "X"; Position_Level1.ForeColor = Color.Black;
                        Position_GA.Text = "X"; Position_GA.ForeColor = Color.Black;
                        Position_Level2.Text = "X"; Position_Level2.ForeColor = Color.Black;
                        Position_Level3.Text = "X"; Position_Level3.ForeColor = Color.Black;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;

                        Status_1_PictureBox.ImageLocation = app_path + @"img\ban.png";
                        Status_GA_PictureBox.ImageLocation = app_path + @"img\ban.png";
                        Status_2_PictureBox.ImageLocation = app_path + @"img\ban.png";
                        Status_3_PictureBox.ImageLocation = app_path + @"img\ban.png";
                        Status_4_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        break;
                    case "2":
                        Position_Level1.Text = "X"; Position_Level1.ForeColor = Color.Black;
                        Position_GA.Text = "X"; Position_GA.ForeColor = Color.Black;
                        Position_Level2.Text = "X"; Position_Level2.ForeColor = Color.Black;
                        Position_Level3.Text = "●"; Position_Level3.ForeColor = Color.Lime;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;

                        Status_1_PictureBox.ImageLocation = app_path + @"img\ban.png";
                        Status_GA_PictureBox.ImageLocation = app_path + @"img\ban.png";
                        Status_2_PictureBox.ImageLocation = app_path + @"img\ban.png";
                        Status_3_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        Status_4_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        break;
                    case "3":
                        Position_Level1.Text = "X"; Position_Level1.ForeColor = Color.Black;
                        Position_GA.Text = "X"; Position_GA.ForeColor = Color.Black;
                        Position_Level2.Text = "●"; Position_Level2.ForeColor = Color.Lime;
                        Position_Level3.Text = "●"; Position_Level3.ForeColor = Color.Lime;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;

                        Status_1_PictureBox.ImageLocation = app_path + @"img\ban.png";
                        Status_GA_PictureBox.ImageLocation = app_path + @"img\ban.png";
                        Status_2_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        Status_3_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        Status_4_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        break;
                    case "4":
                        Position_Level1.Text = "●"; Position_Level1.ForeColor = Color.Lime;
                        Status_1_PictureBox.ImageLocation = app_path + @"img\pass.png";

                        bool GA_Rule_Switch = false;
                        if (GA_Rule_Switch)
                        {
                            Position_GA.Text = "●"; Position_GA.ForeColor = Color.Lime;
                            Position_Level1.Text = "X"; Position_Level1.ForeColor = Color.Black;
                            Status_1_PictureBox.ImageLocation = app_path + @"img\ban.png";
                        }

                        Position_Level2.Text = "●"; Position_Level2.ForeColor = Color.Lime;
                        Position_Level3.Text = "●"; Position_Level3.ForeColor = Color.Lime;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;

                        Status_GA_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        Status_2_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        Status_3_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        Status_4_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        break;
                    case "5":
                        Position_Level1.Text = "●"; Position_Level1.ForeColor = Color.Lime;
                        Position_GA.Text = "●"; Position_GA.ForeColor = Color.Lime;
                        Position_Level2.Text = "●"; Position_Level2.ForeColor = Color.Lime;
                        Position_Level3.Text = "●"; Position_Level3.ForeColor = Color.Lime;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;

                        Status_1_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        Status_GA_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        Status_2_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        Status_3_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        Status_4_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        break;
                    default:
                        break;
                }

            }
            else
            {
                switch (pass_level) //這裡指層級
                {
                    case "1":
                        Position_Level1.Text = "●"; Position_Level1.ForeColor = Color.DarkOrange;
                        Position_GA.Text = "●"; Position_GA.ForeColor = Color.Lime;
                        Position_Level2.Text = "●"; Position_Level2.ForeColor = Color.Lime;
                        Position_Level3.Text = "●"; Position_Level3.ForeColor = Color.Lime;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;

                        Status_1_PictureBox.ImageLocation = app_path + @"img\needsign.png";
                        Status_GA_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        Status_2_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        Status_3_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        Status_4_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        break;
                    case "2":
                        Position_Level1.Text = "○"; Position_Level1.ForeColor = Color.Black;
                        Position_GA.Text = "○"; Position_GA.ForeColor = Color.Black;
                        Position_Level2.Text = "●"; Position_Level2.ForeColor = Color.DarkOrange;
                        Position_Level3.Text = "●"; Position_Level3.ForeColor = Color.Lime;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;

                        Status_1_PictureBox.ImageLocation = app_path + @"img\notyet_pass.png";
                        Status_GA_PictureBox.ImageLocation = app_path + @"img\notyet_pass.png";
                        Status_2_PictureBox.ImageLocation = app_path + @"img\needsign.png";
                        Status_3_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        Status_4_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        break;
                    case "3":
                        Position_Level1.Text = "○"; Position_Level1.ForeColor = Color.Black;
                        Position_GA.Text = "○"; Position_GA.ForeColor = Color.Black;
                        Position_Level2.Text = "○"; Position_Level2.ForeColor = Color.Black;
                        Position_Level3.Text = "●"; Position_Level3.ForeColor = Color.DarkOrange;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;

                        Status_1_PictureBox.ImageLocation = app_path + @"img\notyet_pass.png";
                        Status_GA_PictureBox.ImageLocation = app_path + @"img\notyet_pass.png";
                        Status_2_PictureBox.ImageLocation = app_path + @"img\notyet_pass.png";
                        Status_3_PictureBox.ImageLocation = app_path + @"img\needsign.png";
                        Status_4_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        break;
                    case "4":
                        Position_Level1.Text = "○"; Position_Level1.ForeColor = Color.Black;
                        Position_GA.Text = "○"; Position_GA.ForeColor = Color.Black;
                        Position_Level2.Text = "○"; Position_Level2.ForeColor = Color.Black;
                        Position_Level3.Text = "○"; Position_Level3.ForeColor = Color.Black;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.DarkOrange;

                        Status_1_PictureBox.ImageLocation = app_path + @"img\notyet_pass.png";
                        Status_GA_PictureBox.ImageLocation = app_path + @"img\notyet_pass.png";
                        Status_2_PictureBox.ImageLocation = app_path + @"img\notyet_pass.png";
                        Status_3_PictureBox.ImageLocation = app_path + @"img\notyet_pass.png";
                        Status_4_PictureBox.ImageLocation = app_path + @"img\needsign.png";
                        break;
                    case "1.5":
                        Position_Level1.Text = "○"; Position_Level1.ForeColor = Color.Black;
                        Position_GA.Text = "●"; Position_GA.ForeColor = Color.DarkOrange;
                        Position_Level2.Text = "●"; Position_Level2.ForeColor = Color.Lime;
                        Position_Level3.Text = "●"; Position_Level3.ForeColor = Color.Lime;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;

                        Status_1_PictureBox.ImageLocation = app_path + @"img\notyet_pass.png";
                        Status_GA_PictureBox.ImageLocation = app_path + @"img\needsign.png";
                        Status_2_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        Status_3_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        Status_4_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        break;
                    case "2.5":
                        Position_Level1.Text = "X"; Position_Level1.ForeColor = Color.Black;
                        Position_GA.Text = "●"; Position_GA.ForeColor = Color.DarkOrange;
                        Position_Level2.Text = "○"; Position_Level2.ForeColor = Color.Black;
                        Position_Level3.Text = "●"; Position_Level3.ForeColor = Color.Lime;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;



                        Status_1_PictureBox.ImageLocation = app_path + @"img\ban.png";
                        Status_GA_PictureBox.ImageLocation = app_path + @"img\needsign.png";
                        Status_2_PictureBox.ImageLocation = app_path + @"img\notyet_pass.png";
                        Status_3_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        Status_4_PictureBox.ImageLocation = app_path + @"img\pass.png";
                        break;
                    default:
                        break;
                }

            }
            UpdateMissionStatusVisionProhibit(list_require_sign);

        }
        public void UpdateMissionStatusVisionProhibit(List<Data_Set_Mission_require> list_require_sign)
        {
            Status_1_PictureBox.Visible = true;
            Status_2_PictureBox.Visible = true;
            Status_3_PictureBox.Visible = true;
            Status_GA_PictureBox.Visible = true;
            Status_4_PictureBox.Visible = true;
            string app_path = Directory.GetParent(System.IO.Path.GetDirectoryName(Application.ExecutablePath)).FullName + @"\SubSys_SignSystem\";

            switch (list_require_sign.Count.ToString()) //這裡指數量
            {
                case "5":
                    break;
                case "4":
                    bool GA_Rule_Switch = false;
                    if (GA_Rule_Switch)
                    {
                        Position_Level1.Text = "🚫"; Position_Level1.ForeColor = Color.Black;
                        Status_1_PictureBox.ImageLocation = app_path + @"img\ban.png";
                    }
                    break;
                case "3":
                    Position_Level1.Text = "🚫"; Position_Level1.ForeColor = Color.Black;
                    Position_GA.Text = "🚫"; Position_Level1.ForeColor = Color.Black;

                    Status_1_PictureBox.ImageLocation = app_path + @"img\ban.png";
                    Status_GA_PictureBox.ImageLocation = app_path + @"img\ban.png";
                    break;
                case "2":
                    Position_Level1.Text = "🚫"; Position_Level1.ForeColor = Color.Black;
                    Position_GA.Text = "🚫"; Position_Level1.ForeColor = Color.Black;
                    Position_Level2.Text = "🚫"; Position_Level1.ForeColor = Color.Black;

                    Status_1_PictureBox.ImageLocation = app_path + @"img\ban.png";
                    Status_GA_PictureBox.ImageLocation = app_path + @"img\ban.png";
                    Status_2_PictureBox.ImageLocation = app_path + @"img\ban.png";
                    break;
                case "1":
                    Position_Level1.Text = "🚫"; Position_Level1.ForeColor = Color.Black;
                    Position_GA.Text = "🚫"; Position_Level1.ForeColor = Color.Black;
                    Position_Level2.Text = "🚫"; Position_Level1.ForeColor = Color.Black;
                    Position_Level3.Text = "🚫"; Position_Level1.ForeColor = Color.Black;

                    Status_1_PictureBox.ImageLocation = app_path + @"img\ban.png";
                    Status_GA_PictureBox.ImageLocation = app_path + @"img\ban.png";
                    Status_2_PictureBox.ImageLocation = app_path + @"img\ban.png";
                    Status_3_PictureBox.ImageLocation = app_path + @"img\ban.png";
                    break;
                default:
                    break;
            }
        }
        public void LoadRegisterNewMissionType()
        {
            // Example
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            Data_Set_Excutre_Result result = sign_plugin.CompareTokenvalid();

            // Show Result
            if (result.excute_result.isSuccesed)
            {
                Mission_Type_ID_Textbox.Enabled = true;
                Mission_Type_Summary_Textbox.Enabled = true;
                Register_New_Mission_Type_Button.Enabled = true;
            }
            else
            {
                MessageBox.Show(string.Format("錯誤\r\n錯誤代號：{0}\r\n詳細訊息：{1}", 0, "請先登入"));
            }

        }
        public void LoadAllMission()
        {
            // Example
            All_Mission_Listbox.Items.Clear();
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            List<Information_Mission> list_information_mission = sign_plugin.get_employee_mission_information_all_loanit(employee_data.e_id);

            // Show Result
            foreach (Information_Mission information_mission in list_information_mission)
            {

                if (information_mission.mission_status.Equals("4"))
                {
                    string customer_name = sign_plugin.getCustomerName(information_mission.binding_project_id);


                    int total_requiresign = sign_plugin.get_mission_information(information_mission.mission_id).require_sign.Count;
                    int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission.mission_id))); /// 過濾剩下需要多少簽
                    string current_requiresign = (total_requiresign - remains_requiresign).ToString();
                    All_Mission_Listbox.Items.Add(string.Format("{0}./ 案件代號：{1}{5}    ({2}/{3})   {4}"
                        , information_mission.mission_id, information_mission.binding_project_id
                        , current_requiresign, total_requiresign, "退回重簽中", customer_name));
                }
                else
                {
                    int total_requiresign = sign_plugin.get_mission_information(information_mission.mission_id).require_sign.Count;
                    int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission.mission_id))); /// 過濾剩下需要多少簽
                    string current_requiresign = (total_requiresign - remains_requiresign).ToString();

                    string customer_name = sign_plugin.getCustomerName(information_mission.binding_project_id);


                    if (information_mission.mission_status.Equals("99"))
                    {
                        All_Mission_Listbox.Items.Add(string.Format("{0}./ 案件代號：{1}{5}    ({2}/{3})   {4}", information_mission.mission_id, information_mission.binding_project_id, current_requiresign, total_requiresign, "已拒絕", customer_name));
                    }
                    else if (remains_requiresign == 0)
                    {
                        All_Mission_Listbox.Items.Add(string.Format("{0}./ 案件代號：{1}{5}    ({2}/{3})   {4}", information_mission.mission_id, information_mission.binding_project_id, current_requiresign, total_requiresign, "已核定", customer_name));
                    }
                    else if (remains_requiresign == 1)
                    {
                        All_Mission_Listbox.Items.Add(string.Format("{0}./ 案件代號：{1}{5}    ({2}/{3})   {4}", information_mission.mission_id, information_mission.binding_project_id, current_requiresign, total_requiresign, "待核定", customer_name));
                    }
                    else
                    {
                        All_Mission_Listbox.Items.Add(string.Format("{0}./ 案件代號：{1}{5}    ({2}/{3})   {4}", information_mission.mission_id, information_mission.binding_project_id, current_requiresign, total_requiresign, "需簽核", customer_name));
                    }
                }

            }
            Fillter_TestData_Listbox();
            //ConsoleLog_Texbox.Text += String.Format("{0}\r\n", "已完成【所有任務】資料載入");
        }
        public void LoadRequirMission()
        {
            LOANIT_CONTROLLER_Plugin controller_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            // Example
            Require_Mission_Listbox.Items.Clear();
            List<Information_Mission> list_information_mission_requirment_signature = controller_plugin.get_employee_miossion_information_requirment_loanit(employee_data.e_id);
            List<Information_Mission> list_information_mission_requirment_done = sign_plugin.get_employee_mission_information_requirement_done(employee_data.e_id);

            // Show Result
            foreach (Information_Mission information_mission_requirment_signature in list_information_mission_requirment_signature)
            {
                if (!information_mission_requirment_signature.mission_status.Equals("99"))
                {

                    string customer_name = sign_plugin.getCustomerName(information_mission_requirment_signature.binding_project_id);


                    int total_requiresign = sign_plugin.get_mission_information(information_mission_requirment_signature.mission_id).require_sign.Count;
                    int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission_requirment_signature.mission_id))); /// 過濾剩下需要多少簽
                    string current_requiresign = (total_requiresign - remains_requiresign).ToString();


                    Require_Mission_Listbox.Items.Add(string.Format("{0}./ 案件代號：{1}{5}  ({2}/{3})  {4}"
                        , information_mission_requirment_signature.mission_id, information_mission_requirment_signature.binding_project_id
                        , current_requiresign, total_requiresign, "需簽核", customer_name));


                }
            }
            // Show Result-需核定
            foreach (Information_Mission information_mission_requirment_signature in list_information_mission_requirment_done)
            {
                if (!information_mission_requirment_signature.mission_status.Equals("99"))
                {
                    string customer_name = sign_plugin.getCustomerName(information_mission_requirment_signature.binding_project_id);

                    int total_requiresign = sign_plugin.get_mission_information(information_mission_requirment_signature.mission_id).require_sign.Count;
                    int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission_requirment_signature.mission_id))); /// 過濾剩下需要多少簽
                    string current_requiresign = (total_requiresign - remains_requiresign).ToString();

                    Require_Mission_Listbox.Items.Add(string.Format("{0}./ 案件代號：{1}{5}  ({3}/{4})  {2}",
                        information_mission_requirment_signature.mission_id, information_mission_requirment_signature.binding_project_id, "需核定",
                        current_requiresign, total_requiresign, customer_name));


                }
            }
            Fillter_TestData_Listbox();

            //ConsoleLog_Texbox.Text += String.Format("{0}\r\n", "已完成【需簽核任務】資料載入");
        }
        public void LoadSignedMission()
        {
            // Example
            Signed_Mission_Listbox.Items.Clear();
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();


            // Show Result
            foreach (Data_Set_Sign_History signed_history in sign_plugin.get_employee_signed_information(employee_data.publickey))
            {
                Data_Set_Mission_Details information_mission = sign_plugin.get_mission_information(signed_history.mission_id);
                if (!information_mission.m_id.Contains("不存在"))
                {
                    /// 撈取客戶姓名
                    string customer_name = sign_plugin.getCustomerName(information_mission.binding_project_id);



                    int total_requiresign = sign_plugin.get_mission_information(information_mission.m_id).require_sign.Count;
                    int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission.m_id))); /// 過濾剩下需要多少簽
                    string current_requiresign = (total_requiresign - remains_requiresign).ToString();
                    Signed_Mission_Listbox.Items.Add(string.Format("{0}./ 案件代號：{1}{5}  ({3}/{4})  {2}"
                        , information_mission.m_id, information_mission.binding_project_id, "已簽核", current_requiresign, total_requiresign, customer_name));
                }
                else
                {
                    Signed_Mission_Listbox.Items.Add(information_mission.m_id);
                }
            }
            Fillter_TestData_Listbox();

            //ConsoleLog_Texbox.Text += String.Format("{0}\r\n", "已完成【已簽核任務】資料載入");
        }
        public void LoadDoneMission()
        {
            // Example
            Done_Mission_Listbox.Items.Clear();
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            List<Information_Mission> list_information_mission = sign_plugin.get_employee_mission_information_all_loanit(employee_data.e_id);


            // Show Result
            foreach (Information_Mission information_mission in list_information_mission)
            {
                if (information_mission.mission_status.Equals("3"))
                {
                    string customer_name = sign_plugin.getCustomerName(information_mission.binding_project_id);


                    int total_requiresign = sign.get_mission_information(information_mission.mission_id).require_sign.Count;
                    int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission.mission_id))); /// 過濾剩下需要多少簽
                    string current_requiresign = (total_requiresign - remains_requiresign).ToString();
                    Done_Mission_Listbox.Items.Add(string.Format("{0}./ 案件代號：{1}{5}  ({2}/{3})   {4}"
                        , information_mission.mission_id, information_mission.binding_project_id
                        , current_requiresign, total_requiresign, "已核定", customer_name));
                }
            }
            //ConsoleLog_Texbox.Text += String.Format("{0}\r\n", "已完成【已核定的任務】資料載入");
            Export_ApprovedDocument_Button.Visible = true;
            Fillter_TestData_Listbox();

        }
        public void LoadFailMission()
        {

            // Example
            Fail_Mission_Listbox.Items.Clear();
            Controller_Sign sign = Controller_Sign.getInstance();

            foreach (Data_Set_Mission_Details fail_mission in sign_plugin.get_employee_mission_information_fail(employee_data))
            {
                string customer_name = sign_plugin.getCustomerName(fail_mission.binding_project_id);

                // Show Result
                Fail_Mission_Listbox.Items.Add(string.Format("{0}./ 案件代號：{1}{3} {2}", fail_mission.m_id, fail_mission.binding_project_id, "已拒絕", customer_name));

            }

            if (Fail_Mission_Listbox.Items.Count == 0)
            {
                Fail_Mission_Listbox.Items.Add("不存在已拒絕的簽核任務");
            }
            Fillter_TestData_Listbox();

            //ConsoleLog_Texbox.Text += String.Format("{0}\r\n", "已完成【簽核失敗任務】資料載入");
        }
        public void LoadSigningMission()
        {
            // Example
            Signing_Mission_Listbox.Items.Clear();
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            List<Information_Mission> list_information_mission = sign_plugin.get_employee_mission_information_all_loanit(employee_data.e_id);

            // Show Result
            foreach (Information_Mission information_mission in list_information_mission)
            {
                if (information_mission.mission_status.Equals("2") || information_mission.mission_status.Equals("0"))
                {
                    string customer_name = sign_plugin.getCustomerName(information_mission.binding_project_id);


                    int total_requiresign = sign_plugin.get_mission_information(information_mission.mission_id).require_sign.Count;
                    int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission.mission_id))); /// 過濾剩下需要多少簽
                    string current_requiresign = (total_requiresign - remains_requiresign).ToString();

                    if (remains_requiresign == 1)
                    {
                        Signing_Mission_Listbox.Items.Add(string.Format("{0}./ 案件代號：{1}{5}    ({2}/{3})   {4}"
                            , information_mission.mission_id, information_mission.binding_project_id, current_requiresign
                            , total_requiresign, "待核定", customer_name));
                    }
                    else
                    {
                        Signing_Mission_Listbox.Items.Add(string.Format("{0}./ 案件代號：{1}{5}    ({2}/{3})   {4}"
                            , information_mission.mission_id, information_mission.binding_project_id
                            , current_requiresign, total_requiresign, "需簽核", customer_name));
                    }

                }
                else if (information_mission.mission_status.Equals("4"))
                {
                    int total_requiresign = sign_plugin.get_mission_information(information_mission.mission_id).require_sign.Count;
                    int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission.mission_id))); /// 過濾剩下需要多少簽
                    string current_requiresign = (total_requiresign - remains_requiresign).ToString();

                    ApprovalLevel_Controller approvalLevel_Controller = ApprovalLevel_Controller.getInstance();
                    Approval_Notice customer_data = approvalLevel_Controller.getApprovalNotice(information_mission.binding_project_id)[0];

                    Signing_Mission_Listbox.Items.Add(string.Format("{0}./ 案件代號：{1}{5}    ({2}/{3})   {4}"
                        , information_mission.mission_id, information_mission.binding_project_id
                        , current_requiresign, total_requiresign, "退回重簽中", customer_data.customer_name));

                }
            }
            Fillter_TestData_Listbox();

            //ConsoleLog_Texbox.Text += String.Format("{0}\r\n", "已完成【已核定的任務】資料載入");
        }
        public void Update_Sign_ButtonANDTextbox_UI(bool option)
        {
            if (option)
            {
                Sign_Mission_Pass_Button.Enabled = true;
                Sign_Mission_Fail_Button.Enabled = true;
                history_Textbox.Enabled = true;
            }
            else
            {
                Sign_Mission_Pass_Button.Enabled = false;
                Sign_Mission_Fail_Button.Enabled = false;
                history_Textbox.Enabled = false;
            }
            this.BeginInvoke(new Thread_UpdatUI_Refresh(Thread_Unlock_TabPageControl));
        }
        public void Initial_Form(Data_Set_Employee get_employee_data)
        {
            this.WindowState = FormWindowState.Maximized;
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();

            // Example
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_RISK loan_risk = LOANIT_RISK.getInstance();
            loan_risk.mission_type_data_all = sign_plugin.get_mission_type_information(get_employee_data);

            // Show Result
            employee_data = get_employee_data;
            E_ID_Label.Text = employee_data.e_id;
            E_Name_Label.Text = employee_data.name;
            E_Department_Label.Text = employee_data.department;
            E_Company_Label.Text = employee_data.company;

            /// 將新增任務類別的部分隱藏起來
            this.Register_New_Mission_Type_TabPage.Parent = null;
        }
        private void LoadSignedInformation_Sign(Data_Set_Mission_Details mission_data, string mode, bool excute_done_compare)
        {
            Init_Level_Groupbobx_All_Compment();
            Controller_Sign sign = Controller_Sign.getInstance();
            mission_data.collect_signed.Sort((x, y) => -x.sign_time.CompareTo(y.sign_time));
            int CurrentLevel = 0;
            foreach (Data_Set_Mission_Collect_Signed collect_signed in mission_data.collect_signed)
            {
                if(CurrentLevel < int.Parse(collect_signed.employee_level) || CurrentLevel == 0 )
                {
                    if (collect_signed.sign_status.Equals("pass") || collect_signed.sign_status.ToLower().Equals("false") )
                {
                    CurrentLevel = int.Parse(collect_signed.employee_level);
                    string PrePayMonthText = GetPrePayMonthText(collect_signed.extend_data.prepay_month);
                    if (collect_signed.employee_level.Equals("4"))
                    {
                        Signed_Level4_Name_Label.Text = sign_plugin.get_employee_information(collect_signed.e_id).name;
                        Signed_Level4_SingedTime_Label.Text = collect_signed.sign_time;
                        Signed_Level4_ProcessRatio_Textbox.Text = collect_signed.extend_data.loan_process_ratio;
                        Signed_Level4_Rate_Textbox.Text = collect_signed.extend_data.loan_rate;
                        Signed_Level4_Amount_Textbox.Text = collect_signed.extend_data.loan_amount;
                        Signed_Level4_Advise_Textbox.Text = collect_signed.extend_data.advice;
                        Signed_Level4_Suggestion_Textbox.Text = collect_signed.extend_data.suggestion;
                        Signed_Level4_PrePayMonth_Combobox.Text = PrePayMonthText;
                        if (collect_signed.extend_data.special_note != null && collect_signed.extend_data.special_note.Equals("true")) { special_note = true; } else { special_note = false; }
                    }
                    else if (collect_signed.employee_level.Equals("3"))
                    {
                        Signed_Level3_Name_Label.Text = sign_plugin.get_employee_information(collect_signed.e_id).name;
                        Signed_Level3_SingedTime_Label.Text = collect_signed.sign_time;
                        Signed_Level3_ProcessRatio_Textbox.Text = collect_signed.extend_data.loan_process_ratio;
                        Signed_Level3_Rate_Textbox.Text = collect_signed.extend_data.loan_rate;
                        Signed_Level3_Amount_Textbox.Text = collect_signed.extend_data.loan_amount;
                        Signed_Level3_Advise_Textbox.Text = collect_signed.extend_data.advice;
                        Signed_Level3_Suggestion_Textbox.Text = collect_signed.extend_data.suggestion;
                        Signed_Level3_PrePayMonth_Combobox.Text = PrePayMonthText;
                        if (collect_signed.extend_data.special_note != null && collect_signed.extend_data.special_note.Equals("true")) { special_note = true; } else { special_note = false; }
                    }
                    else if (collect_signed.employee_level.Equals("2"))
                    {
                        Signed_Level2_Name_Label.Text = sign_plugin.get_employee_information(collect_signed.e_id).name;
                        Signed_Level2_SingedTime_Label.Text = collect_signed.sign_time;
                        Signed_Level2_ProcessRatio_Textbox.Text = collect_signed.extend_data.loan_process_ratio;
                        Signed_Level2_Rate_Textbox.Text = collect_signed.extend_data.loan_rate;
                        Signed_Level2_Amount_Textbox.Text = collect_signed.extend_data.loan_amount;
                        Signed_Level2_Advise_Textbox.Text = collect_signed.extend_data.advice;
                        Signed_Level2_Suggestion_Textbox.Text = collect_signed.extend_data.suggestion;
                        Signed_Level2_PrePayMonth_Combobox.Text = PrePayMonthText;
                        if (collect_signed.extend_data.special_note != null && collect_signed.extend_data.special_note.Equals("true")) { special_note = true; } else { special_note = false; }
                    }
                    else if (collect_signed.employee_level.Equals("2.5") || collect_signed.employee_level.Equals("1.5"))
                    {
                        Signed_GA_Name_Label.Text = sign_plugin.get_employee_information(collect_signed.e_id).name;
                        Signed_GA_SingedTime_Label.Text = collect_signed.sign_time;
                        Signed_GA_Advise_Textbox.Text = collect_signed.extend_data.advice;
                    }
                    else if (collect_signed.employee_level.Equals("1"))
                    {
                        Signed_Level1_Name_Label.Text = sign_plugin.get_employee_information(collect_signed.e_id).name;
                        Signed_Level1_SingedTime_Label.Text = collect_signed.sign_time;
                        Signed_Level1_ProcessRatio_Textbox.Text = collect_signed.extend_data.loan_process_ratio;
                        Signed_Level1_Rate_Textbox.Text = collect_signed.extend_data.loan_rate;
                        Signed_Level1_Amount_Textbox.Text = collect_signed.extend_data.loan_amount;
                        Signed_Level1_Advise_Textbox.Text = collect_signed.extend_data.advice;
                        Signed_Level1_Suggestion_Textbox.Text = collect_signed.extend_data.suggestion;
                        Signed_Level1_PrePayMonth_Combobox.Text = PrePayMonthText;
                        if (collect_signed.extend_data.special_note != null && collect_signed.extend_data.special_note.Equals("true")) { special_note = true; } else { special_note = false; }
                    }

                }
                }
            }

            UpdateSpecialNote_Checkbox();


            switch (mode)
            {
                case "Sign":
                    Init_Group_Color();
                    // Update_Groupbox(sign_plugin.get_employee_level(employee_data.e_id, mission_data.mission_type,mission_data.risk_value));
                    Update_Groupbox_ReadOnly_Off();
                    Update_Groupbox(sign_plugin.get_employee_level(employee_data.e_id, mission_data.mission_type, mission_data.risk_value, mission_data), mission_data, excute_done_compare);
                    break;
                case "Search":
                    Init_Group_Color();
                    break;
                case "ReadOnly":
                    Init_Group_Color();
                    Update_Groupbox_ReadOnly_On();
                    break;
                default:
                    break;
            }


        }
        public void Update_Sign_Panel_ReadOnly()
        {
            groupBox8.Enabled = true;
            Signature_Info_Panel.Enabled = true;
        }
        private void compare_SpecialNote(string amount, string process_ratio)
        {
            amount = amount.Replace(",", "");
            if (process_ratio == "") { process_ratio = "0"; }
            if (decimal.Parse(amount) >= 3500000 || decimal.Parse(process_ratio) >= 95)
            {
                Signed_Level4_specialnote_Checkbox.Checked = true;
                Signed_Level3_specialnote_Checkbox.Checked = true;
                Signed_Level2_specialnote_Checkbox.Checked = true;
                Signed_Level1_specialnote_Checkbox.Checked = true;
            }
            else
            {
                Signed_Level4_specialnote_Checkbox.Checked = false;
                Signed_Level3_specialnote_Checkbox.Checked = false;
                Signed_Level2_specialnote_Checkbox.Checked = false;
                Signed_Level1_specialnote_Checkbox.Checked = false;
            }
        }

        #region 渲染輸入格區的顏色.狀態
        private void Update_Groupbox(string employee_level, Data_Set_Mission_Details mission_data, bool excute_done_compare)
        {
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            Signed_Level1_Groupbox.Enabled = false;
            Signed_GA_Groupbox.Enabled = false;
            Signed_Level2_Groupbox.Enabled = false;
            Signed_Level3_Groupbox.Enabled = false;
            Signed_Level4_Groupbox.Enabled = false;

            /// 需去做目前是多少層級
            /// 演算法：依照蒐集到的簽，的publickey 又是 pass 去做遞減

            //string[] employee_level_arr = employee_level.Split(',');
            //for (int i = 0; i <= employee_level_arr.Length - 1; i++)
            //{
            //}

            Update_Signed_Groupbox(employee_level, mission_data);






        }
        private void Update_Groupbox_SignedRefresh()
        {
            Init_Level_Groupbobx_All_Compment();
            Update_Groupbox_ReadOnly_On();
            Update_Groupbox_ReadOnly_Off();
        }
        private void Update_Groupbox_ReadOnly_On()
        {
            Signed_Level1_Groupbox.Enabled = true;
            Signed_Level2_Groupbox.Enabled = true;
            Signed_Level3_Groupbox.Enabled = true;
            Signed_Level4_Groupbox.Enabled = true;

            Signed_Level4_Advise_Textbox.ReadOnly = true;
            Signed_Level4_Amount_Textbox.ReadOnly = true;
            Signed_Level4_Rate_Textbox.ReadOnly = true;
            Signed_Level4_Suggestion_Textbox.ReadOnly = true;
            Signed_Level4_ProcessRatio_Textbox.ReadOnly = true;
            Signed_Level4_PrePayMonth_Combobox.Enabled = false;
            ScoreCardCalculator_Level4_Button.Enabled = false;

            Signed_Level3_Advise_Textbox.ReadOnly = true;
            Signed_Level3_Amount_Textbox.ReadOnly = true;
            Signed_Level3_Rate_Textbox.ReadOnly = true;
            Signed_Level3_Suggestion_Textbox.ReadOnly = true;
            Signed_Level3_ProcessRatio_Textbox.ReadOnly = true;
            Signed_Level3_PrePayMonth_Combobox.Enabled = false;
            ScoreCardCalculator_Level3_Button.Enabled = false;

            Signed_Level2_Advise_Textbox.ReadOnly = true;
            Signed_Level2_Amount_Textbox.ReadOnly = true;
            Signed_Level2_Rate_Textbox.ReadOnly = true;
            Signed_Level2_Suggestion_Textbox.ReadOnly = true;
            Signed_Level2_ProcessRatio_Textbox.ReadOnly = true;
            Signed_Level2_PrePayMonth_Combobox.Enabled = false;
            ScoreCardCalculator_Level2_Button.Enabled = false;

            Signed_Level1_Advise_Textbox.ReadOnly = true;
            Signed_Level1_Amount_Textbox.ReadOnly = true;
            Signed_Level1_Rate_Textbox.ReadOnly = true;
            Signed_Level1_Suggestion_Textbox.ReadOnly = true;
            Signed_Level1_ProcessRatio_Textbox.ReadOnly = true;
            Signed_Level1_PrePayMonth_Combobox.Enabled = false;
            ScoreCardCalculator_Level1_Button.Enabled = false;


            Signed_Level1_ProcessRatio_Textbox.BackColor = Color.Gainsboro;
            Signed_Level1_Rate_Textbox.BackColor = Color.Gainsboro;
            Signed_Level1_Amount_Textbox.BackColor = Color.Gainsboro;
            Signed_Level1_Advise_Textbox.BackColor = Color.Gainsboro;
            Signed_Level1_Suggestion_Textbox.BackColor = Color.Gainsboro;
            Signed_Level1_PrePayMonth_Combobox.BackColor = Color.Gainsboro;

            Signed_GA_Advise_Textbox.BackColor = Color.Gainsboro;

            Signed_Level2_ProcessRatio_Textbox.BackColor = Color.Gainsboro;
            Signed_Level2_Rate_Textbox.BackColor = Color.Gainsboro;
            Signed_Level2_Amount_Textbox.BackColor = Color.Gainsboro;
            Signed_Level2_Advise_Textbox.BackColor = Color.Gainsboro;
            Signed_Level2_Suggestion_Textbox.BackColor = Color.Gainsboro;
            Signed_Level2_PrePayMonth_Combobox.BackColor = Color.Gainsboro;

            Signed_Level3_ProcessRatio_Textbox.BackColor = Color.Gainsboro;
            Signed_Level3_Rate_Textbox.BackColor = Color.Gainsboro;
            Signed_Level3_Amount_Textbox.BackColor = Color.Gainsboro;
            Signed_Level3_Advise_Textbox.BackColor = Color.Gainsboro;
            Signed_Level3_Suggestion_Textbox.BackColor = Color.Gainsboro;
            Signed_Level3_PrePayMonth_Combobox.BackColor = Color.Gainsboro;

            Signed_Level4_ProcessRatio_Textbox.BackColor = Color.Gainsboro;
            Signed_Level4_Rate_Textbox.BackColor = Color.Gainsboro;
            Signed_Level4_Amount_Textbox.BackColor = Color.Gainsboro;
            Signed_Level4_Advise_Textbox.BackColor = Color.Gainsboro;
            Signed_Level4_Suggestion_Textbox.BackColor = Color.Gainsboro;
            Signed_Level4_PrePayMonth_Combobox.BackColor = Color.Gainsboro;


            Visible_Button();

        }
        private void Update_Groupbox_ReadOnly_Off()
        {
            Signed_Level1_Groupbox.Enabled = true;
            Signed_Level2_Groupbox.Enabled = true;
            Signed_Level3_Groupbox.Enabled = true;
            Signed_Level4_Groupbox.Enabled = true;

            Signed_Level4_Advise_Textbox.ReadOnly = false;
            Signed_Level4_Amount_Textbox.ReadOnly = false;
            Signed_Level4_Rate_Textbox.ReadOnly = false;
            Signed_Level4_Suggestion_Textbox.ReadOnly = false;
            Signed_Level4_ProcessRatio_Textbox.ReadOnly = false;
            Signed_Level4_PrePayMonth_Combobox.Enabled = true;
            ScoreCardCalculator_Level4_Button.Enabled = true;

            Signed_Level3_Advise_Textbox.ReadOnly = false;
            Signed_Level3_Amount_Textbox.ReadOnly = false;
            Signed_Level3_Rate_Textbox.ReadOnly = false;
            Signed_Level3_Suggestion_Textbox.ReadOnly = false;
            Signed_Level3_ProcessRatio_Textbox.ReadOnly = false;
            Signed_Level3_PrePayMonth_Combobox.Enabled = true;
            ScoreCardCalculator_Level3_Button.Enabled = true;

            Signed_Level2_Advise_Textbox.ReadOnly = false;
            Signed_Level2_Amount_Textbox.ReadOnly = false;
            Signed_Level2_Rate_Textbox.ReadOnly = false;
            Signed_Level2_Suggestion_Textbox.ReadOnly = false;
            Signed_Level2_ProcessRatio_Textbox.ReadOnly = false;
            Signed_Level2_PrePayMonth_Combobox.Enabled = true;
            ScoreCardCalculator_Level2_Button.Enabled = true;

            Signed_Level1_Advise_Textbox.ReadOnly = false;
            Signed_Level1_Amount_Textbox.ReadOnly = false;
            Signed_Level1_Rate_Textbox.ReadOnly = false;
            Signed_Level1_Suggestion_Textbox.ReadOnly = false;
            Signed_Level1_ProcessRatio_Textbox.ReadOnly = false;
            Signed_Level1_PrePayMonth_Combobox.Enabled = true;
            ScoreCardCalculator_Level1_Button.Enabled = true;
        }
        private void UpdateSpecialNote_Checkbox()
        {
            if (special_note == true)
            {
                Signed_Level4_specialnote_Checkbox.Checked = true;
                Signed_Level3_specialnote_Checkbox.Checked = true;
                Signed_Level2_specialnote_Checkbox.Checked = true;
                Signed_Level1_specialnote_Checkbox.Checked = true;

            }
            else
            {
                Signed_Level4_specialnote_Checkbox.Checked = false;
                Signed_Level3_specialnote_Checkbox.Checked = false;
                Signed_Level2_specialnote_Checkbox.Checked = false;
                Signed_Level1_specialnote_Checkbox.Checked = false;
            }
        }
        public void Load_Mission_History(string history_data)
        {
            string[] history_arr = Regex.Split(history_data, "\r\n");
            Console_Log_RichTextbox.Clear();
            Console_Log_RichTextbox.BackColor = Color.WhiteSmoke;
            foreach (string history in history_arr)
            {
                if (history.Contains("否決"))
                {
                    Console_Log_RichTextbox.SelectionColor = Color.Red;
                    Console_Log_RichTextbox.SelectionFont = new Font("", 9, FontStyle.Bold);
                    //Console_Log_RichTextbox.SelectionFont = new Font("Tahoma", Font.Size(14), style);
                    Console_Log_RichTextbox.AppendText(String.Format("{0}\r\n", history));
                }
                else if (history.Contains("通過"))
                {
                    Console_Log_RichTextbox.SelectionColor = Color.Green;
                    Console_Log_RichTextbox.SelectionFont = new Font("", 9, FontStyle.Bold);
                    //Console_Log_RichTextbox.SelectionFont = new Font("Tahoma", Font.Size(14), style);
                    Console_Log_RichTextbox.AppendText(String.Format("{0}\r\n", history));
                }
            }
        }
        private void Init_Group_Color()
        {
            Signed_Level1_Groupbox.Enabled = false;
            Signed_GA_Groupbox.Enabled = false;
            Signed_Level2_Groupbox.Enabled = false;
            Signed_Level3_Groupbox.Enabled = false;
            Signed_Level4_Groupbox.Enabled = false;

            Signed_Level1_ProcessRatio_Textbox.BackColor = Color.White;
            Signed_Level1_Rate_Textbox.BackColor = Color.White;
            Signed_Level1_Amount_Textbox.BackColor = Color.White;
            Signed_Level1_Advise_Textbox.BackColor = Color.White;
            Signed_Level1_Suggestion_Textbox.BackColor = Color.White;
            Signed_Level1_PrePayMonth_Combobox.BackColor = Color.White;


            Signed_GA_Advise_Textbox.BackColor = Color.White;

            Signed_Level2_ProcessRatio_Textbox.BackColor = Color.White;
            Signed_Level2_Rate_Textbox.BackColor = Color.White;
            Signed_Level2_Amount_Textbox.BackColor = Color.White;
            Signed_Level2_Advise_Textbox.BackColor = Color.White;
            Signed_Level2_Suggestion_Textbox.BackColor = Color.White;
            Signed_Level2_PrePayMonth_Combobox.BackColor = Color.White;


            Signed_Level3_ProcessRatio_Textbox.BackColor = Color.White;
            Signed_Level3_Rate_Textbox.BackColor = Color.White;
            Signed_Level3_Amount_Textbox.BackColor = Color.White;
            Signed_Level3_Advise_Textbox.BackColor = Color.White;
            Signed_Level3_Suggestion_Textbox.BackColor = Color.White;
            Signed_Level3_PrePayMonth_Combobox.BackColor = Color.White;



            Signed_Level4_ProcessRatio_Textbox.BackColor = Color.White;
            Signed_Level4_Rate_Textbox.BackColor = Color.White;
            Signed_Level4_Amount_Textbox.BackColor = Color.White;
            Signed_Level4_Advise_Textbox.BackColor = Color.White;
            Signed_Level4_Suggestion_Textbox.BackColor = Color.White;
            Signed_Level4_PrePayMonth_Combobox.BackColor = Color.White;


        }
        private void Init_Button()
        {
            Signed_Level4_Pass_Button.Visible = true;
            Signed_Level4_Done_Button.Visible = false;
            Signed_Level4_Back_Button.Visible = true;

            Signed_Level3_Pass_Button.Visible = true;
            Signed_Level3_Done_Button.Visible = false;
            Signed_Level3_Back_Button.Visible = true;

            Signed_Level2_Pass_Button.Visible = true;
            Signed_Level2_Done_Button.Visible = false;
            Signed_Level2_Back_Button.Visible = true;

            Signed_GA_Pass_Button.Visible = true;
            Signed_GA_Done_Button.Visible = false;
            Signed_GA_Back_Button.Visible = true;

            Signed_Level1_Pass_Button.Visible = true;
            Signed_Level1_Done_Button.Visible = false;
            Signed_Level1_Back_Button.Visible = true;

        }
        private void Visible_Button()
        {
            Signed_Level4_Back_Button.Visible = false;
            Signed_Level4_Pass_Button.Visible = false;
            Signed_Level4_Done_Button.Visible = false;

            Signed_Level3_Back_Button.Visible = false;
            Signed_Level3_Pass_Button.Visible = false;
            Signed_Level3_Done_Button.Visible = false;

            Signed_Level2_Back_Button.Visible = false;
            Signed_Level2_Pass_Button.Visible = false;
            Signed_Level2_Done_Button.Visible = false;

            Signed_GA_Pass_Button.Visible = false;
            Signed_GA_Done_Button.Visible = false;
            Signed_GA_Back_Button.Visible = false;

            Signed_Level1_Pass_Button.Visible = false;
            Signed_Level1_Done_Button.Visible = false;
            Signed_Level1_Back_Button.Visible = false;

        }
        private void StartOtherSubsysButton()
        {
            /// Enable 子系統按鈕
            ReviewForm_Button.Enabled = true;
            Amortization_Trial_Balance_Button.Enabled = true;
            button1.Enabled = true;
        }
        private void Update_Signed_Groupbox(string employee_level, Data_Set_Mission_Details mission_data)
        {
            switch (employee_level)
            {
                case "1":
                    Signed_Level1_Groupbox.Enabled = true;
                    Signed_Level1_ProcessRatio_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level1_Rate_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level1_Amount_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level1_Advise_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level1_Suggestion_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level1_PrePayMonth_Combobox.BackColor = Color.FromArgb(250, 254, 193);
                    if (excute_done_compare) { Signed_Level1_Pass_Button.Visible = false; }

                    break;
                case "1.5":
                    Signed_GA_Groupbox.Enabled = true;
                    Signed_GA_Done_Button.Visible = false;
                    Signed_GA_Advise_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    if (sign_plugin.get_ga_back_status(mission_data, employee_data))
                    {
                        Signed_GA_Pass_Button.Enabled = false;
                        Signed_GA_Back_Button.Enabled = true;
                    }
                    else
                    {
                        Signed_GA_Pass_Button.Enabled = true;
                        Signed_GA_Back_Button.Enabled = false;
                    }

                    break;
                case "2.5":
                    Signed_GA_Groupbox.Enabled = true;
                    Signed_GA_Done_Button.Visible = false;
                    Signed_GA_Advise_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    if (sign_plugin.get_ga_back_status(mission_data, employee_data))
                    {
                        Signed_GA_Pass_Button.Enabled = false;
                        Signed_GA_Back_Button.Enabled = true;
                    }
                    else
                    {
                        Signed_GA_Pass_Button.Enabled = true;
                        Signed_GA_Back_Button.Enabled = false;
                    }

                    break;
                case "2":
                    Signed_Level2_Groupbox.Enabled = true;
                    Signed_Level2_ProcessRatio_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level2_Rate_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level2_Amount_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level2_Advise_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level2_Suggestion_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level2_PrePayMonth_Combobox.BackColor = Color.FromArgb(250, 254, 193);
                    if (excute_done_compare) { Signed_Level2_Pass_Button.Visible = false; }

                    break;
                case "3":
                    Signed_Level3_Groupbox.Enabled = true;
                    Signed_Level3_ProcessRatio_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level3_Rate_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level3_Amount_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level3_Advise_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level3_Suggestion_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level3_PrePayMonth_Combobox.BackColor = Color.FromArgb(250, 254, 193);
                    if (excute_done_compare) { Signed_Level3_Pass_Button.Visible = false; }

                    break;
                case "4":
                    Signed_Level4_Groupbox.Enabled = true;
                    Signed_Level4_ProcessRatio_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level4_Rate_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level4_Amount_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level4_Advise_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level4_Suggestion_Textbox.BackColor = Color.FromArgb(250, 254, 193);
                    Signed_Level4_PrePayMonth_Combobox.BackColor = Color.FromArgb(250, 254, 193);
                    if (excute_done_compare) { Signed_Level4_Pass_Button.Visible = false; }

                    break;
                default:
                    break;
            }
        }
        #endregion

        #endregion

        #region 初始化
        public static Form_Sign instance = new Form_Sign();
        public static Form_Sign getInstance()
        {
            if (instance == null)
            {
                instance = new Form_Sign();
            }
            return instance;
        }
        public Form_Sign()
        {
            InitializeComponent();
        }
        private void Sign_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            instance = null;
            showNotification("審批通知", "即將為您關閉審批系統，請稍後。", 2);
            //pipecontroller.stopPipe(); pipe功能關閉 如果之後又需要再把註解拿掉
            System.Environment.Exit(0);



        }
        private void Form_Sign_Load(object sender, EventArgs e)
        {
            MailController mailController = MailController.getInstance();

            Init();
            Load_Data();
            timer1.Start();
            showNotification("審批系統通知", "已初始化完成，請開始您的審批之旅。", 3);
        }
        public void Load_User()
        {
        }
        private void Init()
        {
            /// Example 初始化 
            Controller_Sign sign = Controller_Sign.getInstance();

            /// 記憶登入帳號區
            /// employee_data = new Data_Set_Employee();
            /// employee_data.e_id = Properties.Settings.Default.Account;
            /// Account_Textbox.Text = Properties.Settings.Default.Account;
            /// Password_Textbox.Text = Properties.Settings.Default.Password;
            /// 


            //InitPipe();   ///將pipe 關掉 未來如果需要時再開啟；目前只有 評分卡 有用到
            sign_plugin.Current_Token = JsonConvert.DeserializeObject<Date_Set_Login>(Properties.Settings.Default.Token);
            if (sign_plugin.Current_Token == null) { sign_plugin.Current_Token = new Date_Set_Login(); }
            data_set_mission_employee_level = Load_Mission_Employee_Loanit();
            LoadRequirMission(); //更新 需簽核的版面
            LoadPrePayMonthlyComboData();
            history_Textbox.Enabled = true;

        }
        private void Init_Level_Groupbobx_All_Compment()
        {
            Signed_Level4_Name_Label.Text = "";
            Signed_Level4_SingedTime_Label.Text = "";
            Signed_Level4_Advise_Textbox.Text = "";
            Signed_Level4_Amount_Textbox.Text = "";
            Signed_Level4_Rate_Textbox.Text = "";
            Signed_Level4_ProcessRatio_Textbox.Text = "";
            Signed_Level4_Suggestion_Textbox.Text = "";
            Signed_Level4_Groupbox.Enabled = false;

            Signed_Level3_Name_Label.Text = "";
            Signed_Level3_SingedTime_Label.Text = "";
            Signed_Level3_Advise_Textbox.Text = "";
            Signed_Level3_Amount_Textbox.Text = "";
            Signed_Level3_Rate_Textbox.Text = "";
            Signed_Level3_ProcessRatio_Textbox.Text = "";
            Signed_Level3_Suggestion_Textbox.Text = "";
            Signed_Level3_Groupbox.Enabled = false;

            Signed_Level2_Name_Label.Text = "";
            Signed_Level2_SingedTime_Label.Text = "";
            Signed_Level2_Advise_Textbox.Text = "";
            Signed_Level2_Amount_Textbox.Text = "";
            Signed_Level2_Rate_Textbox.Text = "";
            Signed_Level2_ProcessRatio_Textbox.Text = "";
            Signed_Level2_Suggestion_Textbox.Text = "";
            Signed_Level2_Groupbox.Enabled = false;

            Signed_GA_Name_Label.Text = "";
            Signed_GA_SingedTime_Label.Text = "";
            Signed_GA_Advise_Textbox.Text = "";
            Signed_GA_Groupbox.Enabled = false;

            Signed_Level1_Name_Label.Text = "";
            Signed_Level1_SingedTime_Label.Text = "";
            Signed_Level1_Advise_Textbox.Text = "";
            Signed_Level1_Amount_Textbox.Text = "";
            Signed_Level1_Rate_Textbox.Text = "";
            Signed_Level1_ProcessRatio_Textbox.Text = "";
            Signed_Level1_Suggestion_Textbox.Text = "";
            Signed_Level1_Suggestion_Textbox.Text = "";
            Signed_Level1_Groupbox.Enabled = false;

        }

        private List<Data_Set_Mission_Employee_Level_LOANIT> Load_Mission_Employee_Loanit()
        {

            List<Data_Set_Mission_Employee_Level_LOANIT> MissionEmployeeDataList = new List<Data_Set_Mission_Employee_Level_LOANIT>();

            LOANIT_SQL_Plugin sql_plugin = new LOANIT_SQL_Plugin();
            LOANIT_DAO_Plugin dao_plugin = LOANIT_DAO_Plugin.getInstance();
            Data_Set_DAO_Data get_mission_employee_level_result = sql_plugin.get_mission_employee_data_loanit(Sql_Action_Category_Option.GET, Sql_Action_Option.GET_MISSION_EMPLOYEE_LEVEL);
            MissionEmployeeDataList = dao_plugin.set_mission_employee_level_loanit(get_mission_employee_level_result);
            return MissionEmployeeDataList;
        }

        private void LoadPrePayMonthlyComboData()
        {
            using (ScorecardEF db = new ScorecardEF())
            {
                REPO_DISTRIBUTORS_PREPAID Repo_Distributors = new REPO_DISTRIBUTORS_PREPAID(db);
                List<DISTRIBUTORS_PREPAID> DISTRIBUTORS_PREPAID_List = (List<DISTRIBUTORS_PREPAID>)Repo_Distributors.GetByCondition(x => x.ACT_TYP.ToUpper().Equals("A")).ToList();
                Signed_Level4_PrePayMonth_Combobox.DataSource = DISTRIBUTORS_PREPAID_List;
                Signed_Level4_PrePayMonth_Combobox.DisplayMember = "DTR_PPY_TXT";
                Signed_Level4_PrePayMonth_Combobox.ValueMember = "DTR_PPY_SN";
            }

            using (ScorecardEF db = new ScorecardEF())
            {
                REPO_DISTRIBUTORS_PREPAID Repo_Distributors = new REPO_DISTRIBUTORS_PREPAID(db);
                List<DISTRIBUTORS_PREPAID> DISTRIBUTORS_PREPAID_List = (List<DISTRIBUTORS_PREPAID>)Repo_Distributors.GetByCondition(x => x.ACT_TYP.ToUpper().Equals("A")).ToList();
                Signed_Level3_PrePayMonth_Combobox.DataSource = DISTRIBUTORS_PREPAID_List;
                Signed_Level3_PrePayMonth_Combobox.DisplayMember = "DTR_PPY_TXT";
                Signed_Level3_PrePayMonth_Combobox.ValueMember = "DTR_PPY_SN";
            }

            using (ScorecardEF db = new ScorecardEF())
            {
                REPO_DISTRIBUTORS_PREPAID Repo_Distributors = new REPO_DISTRIBUTORS_PREPAID(db);
                List<DISTRIBUTORS_PREPAID> DISTRIBUTORS_PREPAID_List = (List<DISTRIBUTORS_PREPAID>)Repo_Distributors.GetByCondition(x => x.ACT_TYP.ToUpper().Equals("A")).ToList();
                Signed_Level2_PrePayMonth_Combobox.DataSource = DISTRIBUTORS_PREPAID_List;
                Signed_Level2_PrePayMonth_Combobox.DisplayMember = "DTR_PPY_TXT";
                Signed_Level2_PrePayMonth_Combobox.ValueMember = "DTR_PPY_SN";
            }

            using (ScorecardEF db = new ScorecardEF())
            {
                REPO_DISTRIBUTORS_PREPAID Repo_Distributors = new REPO_DISTRIBUTORS_PREPAID(db);
                List<DISTRIBUTORS_PREPAID> DISTRIBUTORS_PREPAID_List = (List<DISTRIBUTORS_PREPAID>)Repo_Distributors.GetByCondition(x => x.ACT_TYP.ToUpper().Equals("A")).ToList();
                Signed_Level1_PrePayMonth_Combobox.DataSource = DISTRIBUTORS_PREPAID_List;
                Signed_Level1_PrePayMonth_Combobox.DisplayMember = "DTR_PPY_TXT";
                Signed_Level1_PrePayMonth_Combobox.ValueMember = "DTR_PPY_SN";
            }

        }



        #endregion

        #region UI_EVENT




        private void Signature_Function_TabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            int i = Signature_Function_TabControl.SelectedIndex;
            string TabControllPage = Signature_Function_TabControl.SelectedTab.Text.ToString();
            Export_ApprovedDocument_Button.Visible = false;
            try
            {
                ThreadLoad(TabControllPage);
                //TabPage_UIController(TabControllPage);

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("錯誤\r\n錯誤代號：{0}\r\n錯誤標籤頁：{2}\r\n詳細訊息：{1}", i.ToString(), ex.Message, TabControllPage));
            }

        }
        private void TabPage_UIController(string TabControllPage)
        {
            switch (TabControllPage)
            {
                case "新增簽核類別": // 新增任務類別
                    Update_Sign_ButtonANDTextbox_UI(false);
                    LoadRegisterNewMissionType();
                    break;
                case "需簽核": // 需簽核
                    Update_Sign_ButtonANDTextbox_UI(true);
                    LoadRequirMission();
                    break;

                case "全部任務": // 全部任務
                    Update_Sign_ButtonANDTextbox_UI(false);
                    LoadAllMission();
                    break;

                case "已簽核": // 已簽核完畢
                    Update_Sign_ButtonANDTextbox_UI(false);
                    LoadSignedMission();
                    //Sign_Mission_Fail_Button.Visible = true;
                    //Sign_Mission_Pass_Button.Visible = true;
                    //groupBox4.Visible = true;
                    break;
                case "已拒絕": // 已拒絕之簽核清單
                    Update_Sign_ButtonANDTextbox_UI(false);
                    LoadFailMission();
                    break;
                case "歷史紀錄": // 簽核歷史紀錄
                    Update_Sign_ButtonANDTextbox_UI(false);
                    break;
                case "已完成": //已完成
                    Update_Sign_ButtonANDTextbox_UI(false);
                    LoadDoneMission();
                    Update_Sign_Panel_ReadOnly();
                    break;
                case "進行中": //進行中
                    Update_Sign_ButtonANDTextbox_UI(false);
                    LoadSigningMission();
                    break;
                default:
                    break;
            }
        }
        private void Signature_Function_TabControl_Selecting()
        {
            int i = Signature_Function_TabControl.SelectedIndex;
            string TabControllPage = Signature_Function_TabControl.SelectedTab.Text.ToString();
            Export_ApprovedDocument_Button.Visible = false;
            try
            {
                switch (TabControllPage)
                {
                    case "新增簽核類別": // 新增任務類別
                        Update_Sign_ButtonANDTextbox_UI(false);
                        LoadRegisterNewMissionType();
                        break;
                    case "需簽核": // 需簽核
                        Update_Sign_ButtonANDTextbox_UI(true);
                        LoadRequirMission();
                        break;

                    case "全部任務": // 全部任務
                        Update_Sign_ButtonANDTextbox_UI(false);
                        LoadAllMission();
                        break;

                    case "已簽核": // 已簽核完畢
                        Update_Sign_ButtonANDTextbox_UI(false);
                        LoadSignedMission();
                        //Sign_Mission_Fail_Button.Visible = true;
                        //Sign_Mission_Pass_Button.Visible = true;
                        //groupBox4.Visible = true;
                        break;
                    case "已拒絕": // 已拒絕之簽核清單
                        Update_Sign_ButtonANDTextbox_UI(false);
                        LoadFailMission();
                        break;
                    case "歷史紀錄": // 簽核歷史紀錄
                        Update_Sign_ButtonANDTextbox_UI(false);
                        break;
                    case "已完成": //已完成
                        Update_Sign_ButtonANDTextbox_UI(false);
                        LoadDoneMission();
                        Update_Sign_Panel_ReadOnly();
                        break;
                    case "進行中": //進行中
                        Update_Sign_ButtonANDTextbox_UI(false);
                        LoadSigningMission();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("錯誤\r\n錯誤代號：{0}\r\n錯誤標籤頁：{2}\r\n詳細訊息：{1}", i.ToString(), ex.Message, TabControllPage));
            }

        }

        private void All_Mission_Listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Eaxmple
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_RISK loanit_risk = LOANIT_RISK.getInstance();

            if (All_Mission_Listbox.SelectedItem != null)
            {
                string[] Mission_Info_Arr = All_Mission_Listbox.SelectedItem.ToString().Split('.');
                Current_Select_Mission_No = Mission_Info_Arr[0];
                Data_Set_Mission_Details mission_data = sign_plugin.get_mission_information(Mission_Info_Arr[0]);
                LoadSignedInformation_Sign(mission_data, "Search", false);


                /// 更新圖示化部分
                string HigestSigned_Level = sign_plugin.get_mission_signing_status_forVision(mission_data);
                mission_data = sign_plugin.get_mission_information(mission_data.m_id);
                UpdateMissionStatusVision(HigestSigned_Level, mission_data.require_sign);

                /// 更新簽核簡介的部分
                LoadMissionSummary(mission_data);

                // Show Result
                foreach (Data_Set_Mission_Type mission_type_data in loanit_risk.mission_type_data_all)
                {
                    if (mission_type_data.mission_type_id.Equals(mission_data.mission_type))
                    {
                        Mission_Type_Descrption_Textbox.Text = mission_type_data.summary;
                        Mission_history_Textbox.Text = mission_data.history;
                        Load_Mission_History(mission_data.history);
                    }
                }

                StartOtherSubsysButton();
            }
        }
        private void Requirl_Mission_Listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Example
            Init_Button();
            excute_done_compare = false;
            groupBox8.Enabled = true;
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_RISK loanit_risk = LOANIT_RISK.getInstance();
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();

            if (Require_Mission_Listbox.SelectedItem != null)
            {
                string[] Mission_Info_Arr = Require_Mission_Listbox.SelectedItem.ToString().Split('.');
                Current_Select_Mission_No = Mission_Info_Arr[0];
                if (Require_Mission_Listbox.SelectedItem.ToString().Contains("需核定")) { excute_done_compare = true; }
                Data_Set_Mission_Details mission_data = sign_plugin.get_mission_information(Mission_Info_Arr[0]);

                /// 更新圖示化部分
                string HigestSigned_Level = sign_plugin.get_mission_signing_status_forVision(mission_data);
                mission_data = sign_plugin.get_mission_information(mission_data.m_id);
                UpdateMissionStatusVision(HigestSigned_Level, mission_data.require_sign);

                /// 更新簽核簡介的部分
                LoadMissionSummary(mission_data);

                LoadSignedInformation_Sign(mission_data, "Sign", excute_done_compare);
                // Show Result
                foreach (Data_Set_Mission_Type mission_type_data in loanit_risk.mission_type_data_all)
                {
                    if (mission_type_data.mission_type_id.Equals(mission_data.mission_type))
                    {
                        Mission_Type_Descrption_Textbox.Text = mission_type_data.summary;
                        Mission_history_Textbox.Text = mission_data.history;
                        Load_Mission_History(mission_data.history);
                    }
                }

                if (excute_done_compare)
                {
                    switch (sign_plugin.get_employee_level(employee_data.e_id, mission_data.mission_type, mission_data.risk_value, mission_data))
                    {
                        case "1":
                            Signed_Level1_Done_Button.Visible = true;
                            break;
                        case "2":
                            Signed_Level2_Done_Button.Visible = true;
                            break;
                        case "3":
                            Signed_Level3_Done_Button.Visible = true;
                            break;
                        case "4":
                            Signed_Level4_Done_Button.Visible = true;
                            break;
                        case "1.5":
                            Signed_GA_Done_Button.Visible = true;
                            break;
                        case "2.5":
                            Signed_GA_Done_Button.Visible = true;
                            break;
                        default:
                            break;
                    }
                }


                StartOtherSubsysButton(); /// 啟動主系統按鈕

                /// 更新 ScrollBar 的初始位置
                int Scroll_Position;
                try
                {
                    Scroll_Position = (4 - int.Parse(sign_plugin.get_employee_level(employee_data.e_id, mission_data.mission_type, mission_data.risk_value, mission_data))) * 480;
                }
                catch (Exception)
                {

                    Scroll_Position = (4 - 1) * 480;
                }
                Signature_Info_Panel.VerticalScroll.Value = Scroll_Position;
            }
        }
        private void Signed_Mission_Listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((Signed_Mission_Listbox.SelectedItem != null) && (!Signed_Mission_Listbox.SelectedItem.ToString().Contains("不存在")))
            {

                // Example
                Controller_Sign sign = Controller_Sign.getInstance();
                LOANIT_RISK loanit_risk = LOANIT_RISK.getInstance();
                LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
                if (Signed_Mission_Listbox.SelectedItem != null)
                {
                    string[] Mission_Info_Arr = Signed_Mission_Listbox.SelectedItem.ToString().Split('.');
                    Current_Select_Mission_No = Mission_Info_Arr[0];
                    Data_Set_Mission_Details mission_data = sign_plugin.get_mission_information(Mission_Info_Arr[0]);

                    /// 更新圖示化部分
                    string HigestSigned_Level = sign_plugin.get_mission_signing_status_forVision(mission_data);
                    mission_data = sign_plugin.get_mission_information(mission_data.m_id);
                    UpdateMissionStatusVision(HigestSigned_Level, mission_data.require_sign);

                    /// 更新簽核簡介的部分
                    LoadMissionSummary(mission_data);

                    LoadSignedInformation_Sign(mission_data, "Search", false);
                    // Show Result
                    foreach (Data_Set_Mission_Type mission_type_data in loanit_risk.mission_type_data_all)
                    {
                        if (mission_type_data.mission_type_id.Equals(mission_data.mission_type))
                        {
                            Mission_Type_Descrption_Textbox.Text = mission_type_data.summary;
                            Mission_history_Textbox.Text = mission_data.history;
                            Load_Mission_History(mission_data.history);
                        }
                    }
                }
            }
            else
            {
                Init_Level_Groupbobx_All_Compment();
                Init_Group_Color();
                Mission_Type_Descrption_Textbox.Text = "";
            }

        }
        private void Done_Mission_Listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Eaxmple
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_RISK loanit_risk = LOANIT_RISK.getInstance();

            if (Done_Mission_Listbox.SelectedItem != null)
            {
                string[] Mission_Info_Arr = Done_Mission_Listbox.SelectedItem.ToString().Split('.');
                Current_Select_Mission_No = Mission_Info_Arr[0];
                Data_Set_Mission_Details mission_data = sign_plugin.get_mission_information(Mission_Info_Arr[0]);
                LoadSignedInformation_Sign(mission_data, "ReadOnly", false);



                /// 更新圖示化部分
                string HigestSigned_Level = sign_plugin.get_mission_signing_status_forVision(mission_data);
                mission_data = sign_plugin.get_mission_information(mission_data.m_id);
                UpdateMissionStatusVision(HigestSigned_Level, mission_data.require_sign);

                /// 更新簽核簡介的部分
                LoadMissionSummary(mission_data);

                // Show Result
                foreach (Data_Set_Mission_Type mission_type_data in loanit_risk.mission_type_data_all)
                {
                    if (mission_type_data.mission_type_id.Equals(mission_data.mission_type))
                    {
                        Mission_Type_Descrption_Textbox.Text = mission_type_data.summary;
                        Mission_history_Textbox.Text = mission_data.history;
                        Load_Mission_History(mission_data.history);
                    }
                }
            }
        }
        private void Fail_Mission_Listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((Fail_Mission_Listbox.SelectedItem != null) && (!Fail_Mission_Listbox.SelectedItem.ToString().Contains("不存在")))
            {

                // Example
                Controller_Sign sign = Controller_Sign.getInstance();
                LOANIT_RISK loanit_risk = LOANIT_RISK.getInstance();
                LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
                if (Fail_Mission_Listbox.SelectedItem != null)
                {
                    string[] Mission_Info_Arr = Fail_Mission_Listbox.SelectedItem.ToString().Split('.');
                    Current_Select_Mission_No = Mission_Info_Arr[0];
                    Data_Set_Mission_Details mission_data = sign_plugin.get_mission_information(Mission_Info_Arr[0]);

                    /// 更新圖示化部分
                    string HigestSigned_Level = sign_plugin.get_mission_signing_status_forVision(mission_data);
                    mission_data = sign_plugin.get_mission_information(mission_data.m_id);
                    UpdateMissionStatusVision(HigestSigned_Level, mission_data.require_sign);

                    /// 更新簽核簡介的部分
                    LoadMissionSummary(mission_data);


                    LoadSignedInformation_Sign(mission_data, "Search", false);
                    // Show Result-備註
                    Mission_history_Textbox.Text = mission_data.history;
                    Load_Mission_History(mission_data.history);

                    // Show Result-任務描述
                    foreach (Data_Set_Mission_Type mission_type_data in loanit_risk.mission_type_data_all)
                    {
                        if (mission_type_data.mission_type_id.Equals(mission_data.mission_type))
                        {
                            Mission_Type_Descrption_Textbox.Text = mission_type_data.summary;
                        }
                    }
                }
            }
            else
            {
                Init_Level_Groupbobx_All_Compment();
                Init_Group_Color();
                Mission_Type_Descrption_Textbox.Text = "";
            }
        }
        private void Signing_Mission_Listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Eaxmple
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            LOANIT_RISK loanit_risk = LOANIT_RISK.getInstance();

            if (Signing_Mission_Listbox.SelectedItem != null)
            {
                string[] Mission_Info_Arr = Signing_Mission_Listbox.SelectedItem.ToString().Split('.');
                Current_Select_Mission_No = Mission_Info_Arr[0];
                Data_Set_Mission_Details mission_data = sign_plugin.get_mission_information(Mission_Info_Arr[0]);

                /// 更新圖示化部分
                string HigestSigned_Level = sign_plugin.get_mission_signing_status_forVision(mission_data);
                mission_data = sign_plugin.get_mission_information(mission_data.m_id);
                UpdateMissionStatusVision(HigestSigned_Level, mission_data.require_sign);



                /// 更新簽核簡介的部分
                LoadMissionSummary(mission_data);

                LoadSignedInformation_Sign(mission_data, "Search", false);
                // Show Result
                foreach (Data_Set_Mission_Type mission_type_data in loanit_risk.mission_type_data_all)
                {
                    if (mission_type_data.mission_type_id.Equals(mission_data.mission_type))
                    {
                        Mission_Type_Descrption_Textbox.Text = mission_type_data.summary;
                        Mission_history_Textbox.Text = mission_data.history;
                        Load_Mission_History(mission_data.history);
                    }
                }
            }
        }

        private void Sign_Mission_Pass_Button_Click(object sender, EventArgs e)
        {
            //// 簽完需初始化物件 textbox enable 要false
            if (!string.IsNullOrEmpty(employee_data.publickey))
            {
                string mission_summary_data = Require_Mission_Listbox.SelectedItem.ToString();
                string[] mission_data_arr = mission_summary_data.Split('.');
                Data_Set_Mission_Details mission_data = sign_plugin.get_mission_information(mission_data_arr[0]);
                string record_data = ""; ///此次簽核要記錄的資料
                Button btn = sender as Button;

                // Example
                Controller_Sign sign = Controller_Sign.getInstance();
                Data_Set_Employee data_set_employee = sign_plugin.get_employee_information(employee_data.e_id);
                Data_Set_Excutre_Result sign_result = null;
                switch (btn.Name)
                {
                    case ("Signed_Level4_Pass_Button"):
                        if (Signed_Level4_Amount_Textbox.Text.Equals("") || Signed_Level4_ProcessRatio_Textbox.Text.Equals(""))
                        {
                            MessageBox.Show("金額、總債比不得為空");
                        }
                        else
                        {
                            if (!PassAndDoneButton_Switch.getInstance().Level4_PassDoneButton_Switch)
                            {
                                record_data = string.Format("{0},,,,,{1},,,,,{2},,,,,{3},,,,,{4},,,,,{5},,,,,{6},,,,,{7}",
                                    history_Textbox.Text, Signed_Level4_Amount_Textbox.Text, Signed_Level4_Rate_Textbox.Text, Signed_Level4_ProcessRatio_Textbox.Text, Signed_Level4_Advise_Textbox.Text
                                    , Signed_Level4_Suggestion_Textbox.Text, Signed_Level4_specialnote_Checkbox.Checked.ToString(), Signed_Level4_PrePayMonth_Combobox.SelectedValue.ToString());
                            }
                            else
                            {
                                MessageBox.Show("請先按下【計算】按鈕,通知");
                            }
                        }
                        break;
                    case ("Signed_Level3_Pass_Button"):
                        if (Signed_Level3_Amount_Textbox.Text.Equals("") || Signed_Level3_ProcessRatio_Textbox.Text.Equals(""))
                        {
                            MessageBox.Show("金額、總債比不得為空");
                        }
                        else
                        {
                            if (!PassAndDoneButton_Switch.getInstance().Level3_PassDoneButton_Switch)
                            {
                                record_data = string.Format("{0},,,,,{1},,,,,{2},,,,,{3},,,,,{4},,,,,{5},,,,,{6},,,,,{7}",
                                history_Textbox.Text, Signed_Level3_Amount_Textbox.Text, Signed_Level3_Rate_Textbox.Text, Signed_Level3_ProcessRatio_Textbox.Text, Signed_Level3_Advise_Textbox.Text
                                , Signed_Level3_Suggestion_Textbox.Text, Signed_Level3_specialnote_Checkbox.Checked.ToString(), Signed_Level3_PrePayMonth_Combobox.SelectedValue.ToString());
                            }
                            else
                            {
                                MessageBox.Show("請先按下【計算】按鈕,通知");
                            }
                        }
                        break;
                    case ("Signed_Level2_Pass_Button"):
                        if (Signed_Level2_Amount_Textbox.Text.Equals("") || Signed_Level2_ProcessRatio_Textbox.Text.Equals(""))
                        {
                            MessageBox.Show("金額、總債比不得為空");
                        }
                        else
                        {
                            if (!PassAndDoneButton_Switch.getInstance().Level2_PassDoneButton_Switch)
                            {
                                record_data = string.Format("{0},,,,,{1},,,,,{2},,,,,{3},,,,,{4},,,,,{5},,,,,{6},,,,,{7}",
                                history_Textbox.Text, Signed_Level2_Amount_Textbox.Text, Signed_Level2_Rate_Textbox.Text, Signed_Level2_ProcessRatio_Textbox.Text, Signed_Level2_Advise_Textbox.Text
                                , Signed_Level2_Suggestion_Textbox.Text, Signed_Level2_specialnote_Checkbox.Checked.ToString(), Signed_Level2_PrePayMonth_Combobox.SelectedValue.ToString());

                            }
                            else
                            {
                                MessageBox.Show("請先按下【計算】按鈕,通知");
                            }
                        }
                        break;
                    case ("Signed_GA_Pass_Button"):
                        record_data = string.Format("{0},,,,,{1},,,,,{2},,,,,{3},,,,,{4},,,,,{5},,,,,{6},,,,,{7}",
                        history_Textbox.Text, "", "", "", Signed_GA_Advise_Textbox.Text, "", "", "");
                        break;
                    case ("Signed_Level1_Pass_Button"):
                        if (Signed_Level1_Amount_Textbox.Text.Equals("") || Signed_Level1_ProcessRatio_Textbox.Text.Equals(""))
                        {
                            MessageBox.Show("金額、總債比不得為空");
                        }
                        else
                        {
                            if (!PassAndDoneButton_Switch.getInstance().Level1_PassDoneButton_Switch)
                            {
                                record_data = string.Format("{0},,,,,{1},,,,,{2},,,,,{3},,,,,{4},,,,,{5},,,,,{6},,,,,{7}",
                                history_Textbox.Text, Signed_Level1_Amount_Textbox.Text, Signed_Level1_Rate_Textbox.Text, Signed_Level1_ProcessRatio_Textbox.Text, Signed_Level1_Advise_Textbox.Text
                                , Signed_Level1_Suggestion_Textbox.Text, Signed_Level1_specialnote_Checkbox.Checked.ToString(), Signed_Level1_PrePayMonth_Combobox.SelectedValue.ToString());
                            }
                            else
                            {
                                MessageBox.Show("請先按下【計算】按鈕,通知");
                            }
                        }
                        break;
                    default:
                        break;
                }

                if (!record_data.Equals(""))
                {
                    if (mission_data.binding_project_id.Contains("_X"))
                    {
                        sign_result = sign_plugin.excute_sign_pass_sibling_loanit(data_set_employee, mission_data_arr[0], record_data);
                    }
                    else
                    {
                        sign_result = sign_plugin.excute_sign_pass_loanit(data_set_employee, mission_data_arr[0], record_data); /// 一般單簽核執行
                        showNotification("審批結果通知", "已完成您的 呈核 請求。", 3);
                        //DemoPluginFunction demoPluginFunction = new DemoPluginFunction();
                        //demoPluginFunction.SendNextSignMissionMail(sign_plugin.get_mission_information(mission_data_arr[0]));
                        string ScoreCardMaxAmount_RiskValue = sign_plugin.Compare_New_Risk((GetScoreCardHistoryHeigh(mission_data.binding_project_id) * 10000).ToString());
                        string[] ScoreCardMaxAmount_RiskValue_Arr = ScoreCardMaxAmount_RiskValue.Split(',');
                        Thread SendMail = new Thread(() => Thread_SendMail(mission_data_arr[0], int.Parse(ScoreCardMaxAmount_RiskValue_Arr[1])));
                        SendMail.Start();


                        groupBox8.Enabled = false;
                        // Show Result

                        // 簽核結果提示
                        if (sign_result != null)
                        {
                            MessageBox.Show(sign_result.excute_result.feedb_back_message);
                        }

                        // Refresh 原先的簽核清單、Groupbox物件
                        List<Information_Mission> list_information_mission_requirment_signature = sign_plugin.get_employee_mission_information_requirment(employee_data.e_id);
                        Require_Mission_Listbox.Items.Clear();
                        sign_plugin.ClearTempData();
                        LoadRequirMission();
                        Update_Groupbox_SignedRefresh();
                    }
                }
            }
            else
            {
                MessageBox.Show("您的印鑑已遺失，因此無簽章");
            }
        }
        private void Sign_Mission_Fail_Button_Click(object sender, EventArgs e)
        {
            if (history_Textbox.Text.Equals(""))
            {
                MessageBox.Show(String.Format("{0}", "請填寫否決原因"));
            }
            else
            {
                string mission_summary_data = Require_Mission_Listbox.SelectedItem.ToString();
                string[] mission_data_arr = mission_summary_data.Split('.');

                // Example
                Controller_Sign sign = Controller_Sign.getInstance();
                LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
                Data_Set_Employee data_set_employee = sign_plugin.get_employee_information(employee_data.e_id);
                Data_Set_Excutre_Result sign_result = new Data_Set_Excutre_Result();

                string record_data = "";
                Button btn = sender as Button;
                switch (btn.Name)
                {
                    case ("Signed_Level4_Back_Button"):
                        record_data = string.Format("{0},,,,,{1},,,,,{2},,,,,{3},,,,,{4},,,,,{5},,,,,{6},,,,,{7}",
                            history_Textbox.Text, Signed_Level4_Amount_Textbox.Text, Signed_Level4_Rate_Textbox.Text, Signed_Level4_ProcessRatio_Textbox.Text, Signed_Level4_Advise_Textbox.Text
                            , Signed_Level4_Suggestion_Textbox.Text, Signed_Level4_specialnote_Checkbox.Checked.ToString(), Signed_Level4_PrePayMonth_Combobox.SelectedValue.ToString());
                        break;
                    case ("Signed_Level3_Back_Button"):
                        record_data = string.Format("{0},,,,,{1},,,,,{2},,,,,{3},,,,,{4},,,,,{5},,,,,{6},,,,,{7}",
                        history_Textbox.Text, Signed_Level3_Amount_Textbox.Text, Signed_Level3_Rate_Textbox.Text, Signed_Level3_ProcessRatio_Textbox.Text, Signed_Level3_Advise_Textbox.Text
                        , Signed_Level3_Suggestion_Textbox.Text, Signed_Level3_specialnote_Checkbox.Checked.ToString(), Signed_Level3_PrePayMonth_Combobox.SelectedValue.ToString());

                        break;
                    case ("Signed_Level2_Back_Button"):
                        record_data = string.Format("{0},,,,,{1},,,,,{2},,,,,{3},,,,,{4},,,,,{5},,,,,{6},,,,,{7}",
                        history_Textbox.Text, Signed_Level2_Amount_Textbox.Text, Signed_Level2_Rate_Textbox.Text, Signed_Level2_ProcessRatio_Textbox.Text, Signed_Level2_Advise_Textbox.Text
                        , Signed_Level2_Suggestion_Textbox.Text, Signed_Level2_specialnote_Checkbox.Checked.ToString(), Signed_Level2_PrePayMonth_Combobox.SelectedValue.ToString());
                        break;
                    case ("Signed_GA_Back_Button"):
                        record_data = string.Format("{0},,,,,{1},,,,,{2},,,,,{3},,,,,{4},,,,,{5},,,,,{6},,,,,{7}",
                        history_Textbox.Text, "", "", "", Signed_GA_Advise_Textbox.Text, "", "", "");
                        break;
                    case ("Signed_Level1_Back_Button"):
                        record_data = string.Format("{0},,,,,{1},,,,,{2},,,,,{3},,,,,{4},,,,,{5},,,,,{6},,,,,{7}",
                        history_Textbox.Text, Signed_Level1_Amount_Textbox.Text, Signed_Level1_Rate_Textbox.Text, Signed_Level1_ProcessRatio_Textbox.Text, Signed_Level1_Advise_Textbox.Text
                        , Signed_Level1_Suggestion_Textbox.Text, Signed_Level1_specialnote_Checkbox.Checked.ToString(), Signed_Level1_PrePayMonth_Combobox.SelectedValue.ToString());
                        break;
                    default:
                        break;
                }

                if (record_data != "")
                {
                    if (mission_summary_data.Contains("_X"))
                    {
                        sign_result = sign_plugin.excute_sign_back_sibling_loanit(data_set_employee, mission_data_arr[0], history_Textbox.Text);
                    }
                    else
                    {

                        sign_result = sign_plugin.excute_sign_back_loanit(data_set_employee, mission_data_arr[0], history_Textbox.Text, record_data); /// 簽核執行
                        showNotification("審批結果通知", "已完成您的 退回 請求。", 3);
                        //DemoPluginFunction demoPluginFunction = new DemoPluginFunction();
                        //demoPluginFunction.SendNextSignMissionMail(sign_plugin.get_mission_information(mission_data_arr[0]));

                        string ScoreCardMaxAmount_RiskValue = sign_plugin.Compare_New_Risk((GetScoreCardHistoryHeigh(sign_plugin.get_mission_information(mission_data_arr[0]).binding_project_id) * 10000).ToString());
                        string[] ScoreCardMaxAmount_RiskValue_Arr = ScoreCardMaxAmount_RiskValue.Split(',');
                        Thread SendMail = new Thread(() => Thread_SendMail(mission_data_arr[0], int.Parse(ScoreCardMaxAmount_RiskValue_Arr[1])));
                        SendMail.Start();

                        // ShowResult

                        // 簽核結果提示
                        MessageBox.Show(sign_result.excute_result.feedb_back_message);

                        // Refresh 原先的簽核清單
                        groupBox8.Enabled = false;
                        List<Information_Mission> list_information_mission_requirment_signature = sign_plugin.get_employee_mission_information_requirment(employee_data.e_id);
                        Require_Mission_Listbox.Items.Clear();
                        history_Textbox.Text = "";
                        foreach (Information_Mission information_mission_requirment_signature in list_information_mission_requirment_signature)
                        {
                            if (!information_mission_requirment_signature.mission_status.Equals("99"))
                            {
                                int total_requiresign = sign_plugin.get_mission_information(information_mission_requirment_signature.mission_id).require_sign.Count;
                                int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission_requirment_signature.mission_id))); /// 過濾剩下需要多少簽
                                string current_requiresign = (total_requiresign - remains_requiresign).ToString();
                                Require_Mission_Listbox.Items.Add(string.Format("{0}./ 案件代號：{1}  ({3}/{4})  {2}", information_mission_requirment_signature.mission_id, information_mission_requirment_signature.binding_project_id, "需核定", current_requiresign, total_requiresign));
                            }
                        }
                        sign_plugin.ClearTempData();
                        LoadRequirMission();
                        Update_Groupbox_SignedRefresh();

                    }
                }
                
            }

        }
        private void Sign_Mission_Done_Buttone_Click(object sender, EventArgs e)
        {


            if (!string.IsNullOrEmpty(employee_data.publickey))
            {
                try
                {
                    LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();

                    string mission_summary_data = Require_Mission_Listbox.SelectedItem.ToString();
                    string[] mission_data_arr = mission_summary_data.Split('.');
                    Data_Set_Mission_Details mission_data = sign_plugin.get_mission_information(mission_data_arr[0]);
                    Button btn = sender as Button;
                    string record_data = "";
                    // Example
                    Controller_Sign sign = Controller_Sign.getInstance();
                    Data_Set_Employee data_set_employee = sign_plugin.get_employee_information(employee_data.e_id);
                    Data_Set_Excutre_Result sign_result = null;


                    switch (btn.Name)
                    {
                        case ("Signed_Level4_Done_Button"):

                            if (Signed_Level4_Amount_Textbox.Text.Equals("") || Signed_Level4_ProcessRatio_Textbox.Text.Equals(""))
                            {
                                MessageBox.Show("金額、總債比不得為空");
                            }
                            else
                            {
                                if (!PassAndDoneButton_Switch.getInstance().Level4_PassDoneButton_Switch)
                                {
                                    record_data = string.Format("{0},,,,,{1},,,,,{2},,,,,{3},,,,,{4},,,,,{5},,,,,{6},,,,,{7}", history_Textbox.Text, Signed_Level4_Amount_Textbox.Text,
                                    Signed_Level4_Rate_Textbox.Text, Signed_Level4_ProcessRatio_Textbox.Text, Signed_Level4_Advise_Textbox.Text,
                                    Signed_Level4_Suggestion_Textbox.Text, Signed_Level4_specialnote_Checkbox.Checked.ToString(), Signed_Level4_PrePayMonth_Combobox.SelectedValue.ToString());
                                }
                                else
                                {
                                    MessageBox.Show("請先按下【計算】按鈕,通知");
                                }
                            }
                            break;

                        case ("Signed_Level3_Done_Button"):

                            if (Signed_Level3_Amount_Textbox.Text.Equals("") || Signed_Level3_ProcessRatio_Textbox.Text.Equals(""))
                            {
                                MessageBox.Show("金額、總債比不得為空");
                            }
                            else
                            {

                                if (!PassAndDoneButton_Switch.getInstance().Level3_PassDoneButton_Switch)
                                {
                                    record_data = string.Format("{0},,,,,{1},,,,,{2},,,,,{3},,,,,{4},,,,,{5},,,,,{6},,,,,{7}", history_Textbox.Text, Signed_Level3_Amount_Textbox.Text,
                                    Signed_Level3_Rate_Textbox.Text, Signed_Level3_ProcessRatio_Textbox.Text, Signed_Level3_Advise_Textbox.Text,
                                    Signed_Level3_Suggestion_Textbox.Text, Signed_Level3_specialnote_Checkbox.Checked.ToString(), Signed_Level3_PrePayMonth_Combobox.SelectedValue.ToString());

                                }
                                else
                                {
                                    MessageBox.Show("請先按下【計算】按鈕,通知");
                                }
                            }

                            break;
                        case ("Signed_Level2_Done_Button"):

                            if (Signed_Level2_Amount_Textbox.Text.Equals("") || Signed_Level2_ProcessRatio_Textbox.Text.Equals(""))
                            {
                                MessageBox.Show("金額、總債比不得為空");
                            }
                            else
                            {
                                if (!PassAndDoneButton_Switch.getInstance().Level2_PassDoneButton_Switch)
                                {
                                    record_data = string.Format("{0},,,,,{1},,,,,{2},,,,,{3},,,,,{4},,,,,{5},,,,,{6},,,,,{7}", history_Textbox.Text, Signed_Level2_Amount_Textbox.Text,
                                    Signed_Level2_Rate_Textbox.Text, Signed_Level2_ProcessRatio_Textbox.Text, Signed_Level2_Advise_Textbox.Text,
                                    Signed_Level2_Suggestion_Textbox.Text, Signed_Level2_specialnote_Checkbox.Checked.ToString(), Signed_Level2_PrePayMonth_Combobox.SelectedValue.ToString());
                                }
                                else
                                {
                                    MessageBox.Show("請先按下【計算】按鈕,通知");
                                }
                            }
                            break;

                        case ("Signed_GA_Done_Button"):
                            record_data = string.Format("{0},,,,,{1},,,,,{2},,,,,{3},,,,,{4},,,,,{5},,,,,{6},,,,,{7}", history_Textbox.Text, "", "", ""
                                    , Signed_GA_Advise_Textbox.Text, "", "", "");
                            break;
                        case ("Signed_Level1_Done_Button"):

                            if (Signed_Level1_Amount_Textbox.Text.Equals("") || Signed_Level1_ProcessRatio_Textbox.Text.Equals(""))
                            {
                                MessageBox.Show("金額、總債比不得為空");
                            }
                            else
                            {
                                if (!PassAndDoneButton_Switch.getInstance().Level1_PassDoneButton_Switch)
                                {
                                    record_data = string.Format("{0},,,,,{1},,,,,{2},,,,,{3},,,,,{4},,,,,{5},,,,,{6},,,,,{7}", history_Textbox.Text, Signed_Level1_Amount_Textbox.Text,
                                        Signed_Level1_Rate_Textbox.Text, Signed_Level1_ProcessRatio_Textbox.Text, Signed_Level1_Advise_Textbox.Text,
                                        Signed_Level1_Suggestion_Textbox.Text, Signed_Level1_specialnote_Checkbox.Checked.ToString(), Signed_Level1_PrePayMonth_Combobox.SelectedValue.ToString());
                                }
                                else
                                {
                                    MessageBox.Show("請先按下【計算】按鈕,通知");
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    if (!record_data.Equals(""))
                    {
                        if (mission_data.binding_project_id.Contains("_X"))
                        {

                            sign_result = sign_plugin.excute_sign_done_sibling_loanit(data_set_employee, mission_data_arr[0], record_data); /// 兄弟單簽核執行
                        }
                        else
                        {
                            sign_result = sign_plugin.excute_sign_done_loanit(data_set_employee, mission_data_arr[0], record_data); /// 簽核執行
                            showNotification("審批結果通知", "已完成您的 核定 請求。", 3);
                            //DemoPluginFunction demoPluginFunction = new DemoPluginFunction();
                            //demoPluginFunction.SendNextSignMissionMail(sign_plugin.get_mission_information(mission_data_arr[0]));

                            string ScoreCardMaxAmount_RiskValue = sign_plugin.Compare_New_Risk((GetScoreCardHistoryHeigh(mission_data.binding_project_id) * 10000).ToString());
                            string[] ScoreCardMaxAmount_RiskValue_Arr = ScoreCardMaxAmount_RiskValue.Split(',');
                            Thread SendMail = new Thread(() => Thread_SendMail(mission_data_arr[0], int.Parse(ScoreCardMaxAmount_RiskValue_Arr[1])));
                            SendMail.Start();
                            // Show Result

                            // 簽核結果提示
                            MessageBox.Show(sign_result.excute_result.feedb_back_message);


                            groupBox8.Enabled = false;
                            // Refresh 原先的簽核清單
                            Require_Mission_Listbox.Items.Clear();
                            sign_plugin.ClearTempData();
                            LoadRequirMission();
                            Update_Groupbox_SignedRefresh();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

            }
            else
            {
                MessageBox.Show("您的印鑑已遺失，因此無簽章");
            }
        }

        private void Register_New_Mission_Type_Button_Click(object sender, EventArgs e)
        {
            // Example
            LOANIT loanit = LOANIT.getinstance();
            Controller_Sign sign = Controller_Sign.getInstance();

            if ((!Mission_Type_ID_Textbox.Text.Equals("")) && (!Mission_Type_Summary_Textbox.Text.Equals("")))
            {
                loanit.initial_mission_type();
                loanit.set_mission_type_data(Set_Mission_Type_Option.COMPANY, employee_data.company);
                loanit.set_mission_type_data(Set_Mission_Type_Option.DEPARTMENT, employee_data.department);
                loanit.set_mission_type_data(Set_Mission_Type_Option.MISSION_TYPE_ID, Mission_Type_ID_Textbox.Text);
                loanit.set_mission_type_data(Set_Mission_Type_Option.SUMMARY, Mission_Type_Summary_Textbox.Text);

                // show Result
                Data_Set_Excutre_Result result_show = sign_plugin.register_new_mission_type(loanit.data_set_mission_type);
                MessageBox.Show(result_show.excute_result.isSuccesed + " " + result_show.excute_result.feedb_back_message);
            }
            else
            {
                MessageBox.Show("請把欄位填寫完整。");
            }
        }
        private void Login_Button_Click(object sender, EventArgs e)
        {

            if (Account_Textbox.Text.Equals(employee_data.e_id))
            {
                // Example
                Controller_Sign sign = Controller_Sign.getInstance();
                Data_Set_Excutre_Result result = sign_plugin.login(Account_Textbox.Text, Password_Textbox.Text);
                ConsoleLog_Texbox.Text += string.Format("{0}\r\n", result.excute_result.feedb_back_message);

                // Show Result
                if (result.excute_result.isSuccesed)
                {
                    Initial_Form(sign_plugin.get_employee_information(Account_Textbox.Text));
                    UpdateUIData();

                    {
                        Logout_button.Enabled = true;
                    }
                }
            }
            else
            {
                MessageBox.Show(String.Format("{0}", "請根據您主系統的帳號進行登入。"));
            }
        }
        private void Logout_button_Click(object sender, EventArgs e)
        {
            Controller_Sign sign = Controller_Sign.getInstance();
            MessageBox.Show(sign_plugin.logout().excute_result.feedb_back_message);
            sign_plugin.CompareTokenvalid();
            UpdateUIData();
            this.Close();
        }
        private void Display_TestData_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            Stack PreRemoveIndex = new Stack();
            ListBox CurrentListBox = new ListBox();

            if (!Display_TestData_Checkbox.Checked)
            {
                TabPage Current_Page = Signature_Function_TabControl.SelectedTab; /// 獲取當前的頁面
                foreach (Control item in Current_Page.Controls)
                {
                    if (item is ListBox) /// 獲取此頁面底下的Listbox
                    {
                        CurrentListBox = item as ListBox;

                        for (int i = 0; i <= CurrentListBox.Items.Count - 1; i++)
                        {
                            if (CurrentListBox.Items[i].ToString().Contains("A123456789"))
                            {
                                PreRemoveIndex.Push(i); /// 將符合【測試資料】的 Index 存下來
                            }

                        }
                    }
                }

                /// 執行刪除
                foreach (int item in PreRemoveIndex)
                {
                    CurrentListBox.Items.RemoveAt(item);
                    CurrentListBox.Refresh();
                }

            }
            else
            {
                Signature_Function_TabControl_Selecting();
            }
        }
        private void Fillter_TestData_Listbox()
        {
            Stack PreRemoveIndex = new Stack();
            ListBox CurrentListBox = new ListBox();

            if (!Display_TestData_Checkbox.Checked)
            {
                TabPage Current_Page = Signature_Function_TabControl.SelectedTab; /// 獲取當前的頁面
                foreach (Control item in Current_Page.Controls)
                {
                    if (item is ListBox) /// 獲取此頁面底下的Listbox
                    {
                        CurrentListBox = item as ListBox;

                        for (int i = 0; i <= CurrentListBox.Items.Count - 1; i++)
                        {
                            if (CurrentListBox.Items[i].ToString().Contains("A123456789"))
                            {
                                PreRemoveIndex.Push(i); /// 將符合【測試資料】的 Index 存下來
                            }

                        }
                    }
                }

                /// 執行刪除
                foreach (int item in PreRemoveIndex)
                {
                    CurrentListBox.Items.RemoveAt(item);
                    CurrentListBox.Refresh();
                }

            }
        }

        #region 檢測核定/呈核呈現功能區

        public void compare_NormalAmountRule(Data_Set_Employee data_set_employee, string[] mission_data_arr, Employee_Sign_Level employee_sign_level)
        {
            switch (employee_sign_level)
            {
                case Employee_Sign_Level.LEVEL1:
                    break;
                case Employee_Sign_Level.LEVEL2:
                    compare_SpecialNote(Signed_Level2_Amount_Textbox.Text, Signed_Level2_ProcessRatio_Textbox.Text);
                    if (sign_plugin.compare_current_amount_rule(data_set_employee, mission_data_arr[0], Signed_Level2_Amount_Textbox.Text, Signed_Level2_ProcessRatio_Textbox.Text))
                    {
                        Signed_Level2_Done_Button.Visible = false;
                        Signed_Level2_Pass_Button.Visible = true;
                    }
                    else if (excute_done_compare == true)
                    {
                        Signed_Level2_Done_Button.Visible = true;
                        Signed_Level2_Pass_Button.Visible = false;
                    }
                    break;
                case Employee_Sign_Level.LEVEL3:
                    compare_SpecialNote(Signed_Level3_Amount_Textbox.Text, Signed_Level3_ProcessRatio_Textbox.Text);
                    if (sign_plugin.compare_current_amount_rule(data_set_employee, mission_data_arr[0], Signed_Level3_Amount_Textbox.Text, Signed_Level3_ProcessRatio_Textbox.Text))
                    {
                        Signed_Level3_Done_Button.Visible = false;
                        Signed_Level3_Pass_Button.Visible = true;
                    }
                    else if (excute_done_compare == true)
                    {
                        Signed_Level3_Done_Button.Visible = true;
                        Signed_Level3_Pass_Button.Visible = false;
                    }
                    break;
                case Employee_Sign_Level.LEVEL4:
                    compare_SpecialNote(Signed_Level4_Amount_Textbox.Text, Signed_Level4_ProcessRatio_Textbox.Text);
                    if (sign_plugin.compare_current_amount_rule(data_set_employee, mission_data_arr[0], Signed_Level4_Amount_Textbox.Text, Signed_Level4_ProcessRatio_Textbox.Text))
                    {
                        Signed_Level4_Done_Button.Visible = false;
                        Signed_Level4_Pass_Button.Visible = true;
                    }
                    else if (excute_done_compare == true)
                    {
                        Signed_Level4_Done_Button.Visible = true;
                        Signed_Level4_Pass_Button.Visible = false;
                    }
                    break;
                default:
                    break;
            }
        }
        public void compare_SiblingAmountRule(Data_Set_Employee data_set_employee, string[] mission_data_arr, Employee_Sign_Level employee_sign_level)
        {
            Data_Set_Mission_Details mission_data = sign_plugin.get_mission_information(mission_data_arr[0]);
            data_set_employee.employee_level = sign_plugin.get_employee_level(data_set_employee.e_id, mission_data.mission_type, mission_data.risk_value, mission_data);


            switch (employee_sign_level)
            {
                case Employee_Sign_Level.LEVEL1:
                    break;
                case Employee_Sign_Level.LEVEL2:

                    if (sign_plugin.compare_current_amount_SiblingRule(data_set_employee, mission_data_arr[0], Signed_Level2_Amount_Textbox.Text, Signed_Level2_ProcessRatio_Textbox.Text))
                    {
                        Signed_Level2_Done_Button.Visible = false;
                        Signed_Level2_Pass_Button.Visible = true;
                    }
                    else if (excute_done_compare == true)
                    {
                        Signed_Level2_Done_Button.Visible = true;
                        Signed_Level2_Pass_Button.Visible = false;
                    }
                    break;
                case Employee_Sign_Level.LEVEL3:
                    if (sign_plugin.compare_current_amount_SiblingRule(data_set_employee, mission_data_arr[0], Signed_Level3_Amount_Textbox.Text, Signed_Level3_ProcessRatio_Textbox.Text))
                    {
                        Signed_Level3_Done_Button.Visible = false;
                        Signed_Level3_Pass_Button.Visible = true;
                    }
                    else if (excute_done_compare == true)
                    {
                        Signed_Level3_Done_Button.Visible = true;
                        Signed_Level3_Pass_Button.Visible = false;
                    }
                    break;
                case Employee_Sign_Level.LEVEL4:
                    if (sign_plugin.compare_current_amount_SiblingRule(data_set_employee, mission_data_arr[0], Signed_Level4_Amount_Textbox.Text, Signed_Level4_ProcessRatio_Textbox.Text))
                    {
                        Signed_Level4_Done_Button.Visible = false;
                        Signed_Level4_Pass_Button.Visible = true;
                    }
                    else if (excute_done_compare == true)
                    {
                        Signed_Level4_Done_Button.Visible = true;
                        Signed_Level4_Pass_Button.Visible = false;
                    }
                    break;
                default:
                    break;
            }
        }
        private void Signed_Level4_AmountProcessRation_Textbox_TextChanged(object sender, EventArgs e)
        {
            if (Signed_Level4_Amount_Textbox.Enabled is true)
            {
                if (Signed_Level4_Amount_Textbox.Text != "" && Require_Mission_Listbox.SelectedItem != null)
                {
                    Data_Set_Employee data_set_employee = sign_plugin.get_employee_information(employee_data.e_id);
                    string mission_summary_data = Require_Mission_Listbox.SelectedItem.ToString();
                    string[] mission_data_arr = mission_summary_data.Split('.');

                    if (mission_summary_data.Contains("_X"))
                    {
                        compare_SiblingAmountRule(data_set_employee, mission_data_arr, Employee_Sign_Level.LEVEL4);
                    }
                    else
                    {
                        /// 一般單流程
                        compare_NormalAmountRule(data_set_employee, mission_data_arr, Employee_Sign_Level.LEVEL4);
                    }

                }
            }

            if (Signed_Level4_Amount_Textbox.Text.Equals("")) { Signed_Level4_Amount_Textbox.Text = "0"; } // 若為空字串則自動補0
            Signed_Level4_Amount_Textbox.Text = changeToMoneyType(decimal.Parse(Signed_Level4_Amount_Textbox.Text.Replace(",", ""))); //轉換千分位
            Signed_Level4_Amount_Textbox.Select(Signed_Level4_Amount_Textbox.Text.Length, 0);//調整到數字的尾末
            PassAndDoneButton_Switch.getInstance().Level4_PassDoneButton_Switch = true;

        }
        private void Signed_Level3_AmountProcessRation_Textbox_TextChanged(object sender, EventArgs e)
        {
            if (Signed_Level3_Amount_Textbox.Enabled is true)
            {
                if (Signed_Level3_Amount_Textbox.Text != "" && Require_Mission_Listbox.SelectedItem != null)
                {
                    Data_Set_Employee data_set_employee = sign_plugin.get_employee_information(employee_data.e_id);
                    string mission_summary_data = Require_Mission_Listbox.SelectedItem.ToString();
                    string[] mission_data_arr = mission_summary_data.Split('.');
                    if (mission_summary_data.Contains("_X"))
                    {
                        compare_SiblingAmountRule(data_set_employee, mission_data_arr, Employee_Sign_Level.LEVEL3);
                    }
                    else
                    {
                        /// 一般單流程
                        compare_NormalAmountRule(data_set_employee, mission_data_arr, Employee_Sign_Level.LEVEL3);
                    }

                }
            }
            if (Signed_Level3_Amount_Textbox.Text.Equals("")) { Signed_Level3_Amount_Textbox.Text = "0"; } // 若為空字串則自動補0
            Signed_Level3_Amount_Textbox.Text = changeToMoneyType(decimal.Parse(Signed_Level3_Amount_Textbox.Text));
            Signed_Level3_Amount_Textbox.Select(Signed_Level3_Amount_Textbox.Text.Length, 0);//調整到數字的尾末
            PassAndDoneButton_Switch.getInstance().Level3_PassDoneButton_Switch = true;

        }
        private void Signed_Level2_AmountProcessRation_Textbox_TextChanged(object sender, EventArgs e)
        {
            if (Signed_Level2_Amount_Textbox.Enabled is true)
            {
                if (Signed_Level2_Amount_Textbox.Text != "" && Require_Mission_Listbox.SelectedItem != null)
                {
                    Data_Set_Employee data_set_employee = sign_plugin.get_employee_information(employee_data.e_id);
                    string mission_summary_data = Require_Mission_Listbox.SelectedItem.ToString();
                    string[] mission_data_arr = mission_summary_data.Split('.');
                    if (mission_summary_data.Contains("_X"))
                    {
                        compare_SiblingAmountRule(data_set_employee, mission_data_arr, Employee_Sign_Level.LEVEL2);
                    }
                    else
                    {
                        /// 一般單流程
                        compare_NormalAmountRule(data_set_employee, mission_data_arr, Employee_Sign_Level.LEVEL2);
                    }
                }
            }
            if (Signed_Level2_Amount_Textbox.Text.Equals("")) { Signed_Level2_Amount_Textbox.Text = "0"; } // 
            Signed_Level2_Amount_Textbox.Text = changeToMoneyType(decimal.Parse(Signed_Level2_Amount_Textbox.Text));
            Signed_Level2_Amount_Textbox.Select(Signed_Level2_Amount_Textbox.Text.Length, 0);//調整到數字的尾末
            PassAndDoneButton_Switch.getInstance().Level2_PassDoneButton_Switch = true;


        }
        private void Signed_Level1_Amount_Textbox_TextChanged(object sender, EventArgs e)
        {
            if (Require_Mission_Listbox.SelectedItem != null)
            {
                if (Signed_Level1_Amount_Textbox.Text.Equals("")) { Signed_Level1_Amount_Textbox.Text = "0"; } // 若為空字串則自動補0
                Signed_Level1_Amount_Textbox.Text = changeToMoneyType(decimal.Parse(Signed_Level1_Amount_Textbox.Text.Replace(",", ""))); //轉換千分位
                Signed_Level1_Amount_Textbox.Select(Signed_Level1_Amount_Textbox.Text.Length, 0);//調整到數字的尾末
            }
            PassAndDoneButton_Switch.getInstance().Level1_PassDoneButton_Switch = true;

        }
        private void Export_ApprovedDocument_Button_Click(object sender, EventArgs e)
        {
            if (Done_Mission_Listbox.SelectedItem != null)
            {
                string mission_summary_data = Done_Mission_Listbox.SelectedItem.ToString();
                string[] mission_data_arr = mission_summary_data.Split('.');

                Data_Set_Mission_Details mission_data = sign_plugin.get_mission_information(mission_data_arr[0]);


                ApprovalLevel_Controller approvalLevel_Controller = ApprovalLevel_Controller.getInstance();
                //Approval_Notice result = approvalLevel_Controller.getApprovalNotice("A123456789_F0001")[0];
                Approval_Notice result = approvalLevel_Controller.getApprovalNotice(mission_data.binding_project_id)[0];

                DAO.setNewDB_IpAndAcctAndPassword("192.168.1.25,55888", "ptmbrd_yc", "1dayisgood");

                Form_ApprovedDocument form_approvedocument = Form_ApprovedDocument.getInstance();

                //form_approvedocument.Set_Data(result, "A123456789_F0001", mission_data_arr[0]);
                form_approvedocument.Set_Data(result, mission_data.binding_project_id, mission_data_arr[0]);
                form_approvedocument.Show();
            }
            else
            {
                MessageBox.Show(string.Format("請選擇一個已完成的案件。"));
            }




        }
        #endregion

        #region 檢測輸入的值是否是數字
        private void Signed_Level4_Amount_Textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Compare_KeyDataIsNumber_Amount(e, sender);
        }
        private void Signed_Level3_Amount_Textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Compare_KeyDataIsNumber_Amount(e, sender);

        }
        private void Signed_Level2_Amount_Textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Compare_KeyDataIsNumber_Amount(e, sender);
        }
        private void Signed_Level1_Amount_Textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Compare_KeyDataIsNumber_Amount(e, sender);
        }

        private void Compare_KeyDataIsNumber_Amount(KeyPressEventArgs e, object sender)
        {
            TextBox senders = sender as TextBox;
            //判斷輸入的類型
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
            {
                e.Handled = true;
            }
            else
            {
                if (((senders.TextLength <= 10) || ((int)e.KeyChar == 8)) && (int)e.KeyChar != 46)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }

            //小数点的处理。
            //if ((int)e.KeyChar == 46)//小數點
            //{
            //    if (Signed_Level3_Amount_Textbox.Text.Length <= 0)
            //        e.Handled = true;   //不能在第一位
            //    else
            //    {
            //        float f;
            //        float oldf;
            //        bool b1 = false, b2 = false;
            //        b1 = float.TryParse(Signed_Level3_Amount_Textbox.Text, out oldf);
            //        b2 = float.TryParse(Signed_Level3_Amount_Textbox.Text + e.KeyChar.ToString(), out f);
            //        if (b2 == false)
            //        {
            //            if (b1 == true)
            //                e.Handled = true;
            //            else
            //                e.Handled = false;
            //        }
            //    }
            //}
        }

        private void Compare_KeyDataIsRaitioFormat(object sender, KeyPressEventArgs e)
        {
            TextBox senders = sender as TextBox;
            //判斷輸入的類型
            if (((int)e.KeyChar < 48 || (int)e.KeyChar > 57) && (int)e.KeyChar != 8 && (int)e.KeyChar != 46)
            {
                e.Handled = true;
            }
            else
            {
                /// 48 -> 0

                if (senders.Text.Length == 0)
                {
                    if ((int)e.KeyChar == 46)
                    {
                        e.Handled = true;
                        PassAndDoneButton_Switch.getInstance().OpenAllSwitch();

                    }
                    else
                    {
                        e.Handled = false;
                    }
                }
                else if (senders.Text.Length > 0)
                {
                    if ((!senders.Text.Contains('.')))
                    {
                        if (senders.Text.Length > 2)
                        {
                            double Pre_CurretRatio = 0.0;
                            if ((int)e.KeyChar != 8)
                            {
                                Pre_CurretRatio = double.Parse(string.Format("{0}{1}", senders.Text, e.KeyChar));
                                CompareRatioRule(Pre_CurretRatio, e, senders);
                                PassAndDoneButton_Switch.getInstance().OpenAllSwitch();

                            }
                            else
                            {
                                e.Handled = false;
                            }
                        }
                        else
                        {
                            if ((int)e.KeyChar != 8)
                            {
                                double Pre_CurretRatio = double.Parse(string.Format("{0}{1}", senders.Text, e.KeyChar));
                                if (Pre_CurretRatio > 1.5)
                                {
                                    MessageBox.Show("輸入範圍：【 0.8 <= x <= 1.5】", "利率規則");
                                    senders.Text = "";
                                    e.Handled = true;
                                }
                                else
                                {
                                    e.Handled = false;
                                }
                            }
                            else
                            {
                                e.Handled |= false;
                            }

                        }

                    }
                    else
                    {
                        if ((int)e.KeyChar != 46)
                        {

                            double Pre_CurretRatio = 0.0;
                            if ((int)e.KeyChar != 8)
                            {
                                Pre_CurretRatio = double.Parse(string.Format("{0}{1}", senders.Text, e.KeyChar));
                                CompareRatioRule(Pre_CurretRatio, e, senders);
                                PassAndDoneButton_Switch.getInstance().OpenAllSwitch();

                            }
                            else
                            {
                                e.Handled = false;
                            }

                        }
                        else
                        {
                            e.Handled = true;
                        }
                    }
                }

            }
        }
        private void CompareRatioRule(double Pre_CurretRatio, KeyPressEventArgs e, TextBox senders)
        {
            if (Pre_CurretRatio >= 0.8 && Pre_CurretRatio <= 1.5)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
                MessageBox.Show("輸入範圍：【 0.8 <= x <= 1.5】", "利率規則");
                senders.Text = "";
            }
        }
        #region 打開其他子系統
        private void Amortization_Trial_Balance_Button_Click(object sender, EventArgs e)
        {
            try
            {
                string app_path = Directory.GetParent(System.IO.Path.GetDirectoryName(Application.ExecutablePath)).FullName + @"\SubSys_ProductCalculatePreview\ProductCalculatePreview.exe";
                Process.Start(app_path, args[0]);
            }
            catch (Exception EX)
            {

                MessageBox.Show(string.Format("未使用主系統開啟，請再次確認啟動模式。"));
            }
        }
        private void ScoreCardCalculator_Button_Click(object sender, EventArgs e)
        {

            //Button send_btn = sender as Button;
            PipeData_ScoreCardCalculator pipedata_scorecard = new PipeData_ScoreCardCalculator();
            string mission_summary_data = Require_Mission_Listbox.SelectedItem.ToString();
            string[] mission_data_arr = mission_summary_data.Split('.');

            pipedata_scorecard.level = "";
            pipedata_scorecard.amount = "0";
            pipedata_scorecard.rate = "0";

            //if (send_btn.Name.Contains("Level1"))
            //{
            //    pipedata_scorecard.level = "Level1";
            //    pipedata_scorecard.amount = Signed_Level1_Amount_Textbox.Text.Replace(",", "");
            //    pipedata_scorecard.rate = Signed_Level1_Rate_Textbox.Text.Replace(",", "");
            //    ScoreCardCalculator_Level1_Button.Enabled = false;
            //}
            //else if (send_btn.Name.Contains("Level2"))
            //{
            //    pipedata_scorecard.level = "Level2";
            //    pipedata_scorecard.amount = Signed_Level2_Amount_Textbox.Text.Replace(",", "");
            //    pipedata_scorecard.rate = Signed_Level2_Rate_Textbox.Text.Replace(",", "");
            //    ScoreCardCalculator_Level2_Button.Enabled = false;

            //}
            //else if (send_btn.Name.Contains("Level3"))
            //{
            //    pipedata_scorecard.level = "Level3";
            //    pipedata_scorecard.amount = Signed_Level3_Amount_Textbox.Text.Replace(",", "");
            //    pipedata_scorecard.rate = Signed_Level3_Rate_Textbox.Text.Replace(",", "");
            //    ScoreCardCalculator_Level3_Button.Enabled = false;

            //}
            //else if (send_btn.Name.Contains("Level4"))
            //{
            //    pipedata_scorecard.level = "Level4";
            //    pipedata_scorecard.amount = Signed_Level4_Amount_Textbox.Text.Replace(",", "");
            //    pipedata_scorecard.rate = Signed_Level4_Rate_Textbox.Text.Replace(",", "");
            //    ScoreCardCalculator_Level4_Button.Enabled = false;
            //}


            ///// 將選單卡住，已防止點到其他的專案
            //Require_Mission_Listbox.Enabled = false;
            //Signature_Function_TabControl.Enabled = false;

            pipedata_scorecard.x_key = "X0000"; /// 一般單就是 X0000 等之後 居政 將兄弟單那邊的技術債還清 就可以設 ""
            pipedata_scorecard.c_key = sign_plugin.get_mission_information(mission_data_arr[0]).binding_project_id;
            string Send_Data_Json = JsonConvert.SerializeObject(pipedata_scorecard);
            Send_Data_Json = Send_Data_Json.Replace("\"", "'");

            try
            {
                string test_token = "{'cardAuth':false,'applyFormAuth':false,'assetAuth':false,'financeAuth':true,'advertisementAuth':false,'cityCardAuth':false,'acctbook':true,'isAccountOfficer':false,'isAccountCreater':false,'user_name':'林郁宸','isManager':false,'isAdmin':false,'isAssetController':false,'isOtherAcctbook_Loanit':false,'isOtherAcctbook_PTMB':false,'account':'TKFLYC0509','public_key':'','timeStmp':'20221226171709','token':'7842EC243001D543522D86F3DD5C5F13F8B790F4E0F999898C0827B0DD7952D7'}";
                string prorcess_info = string.Format("{0}※{1}※{2}", test_token, Send_Data_Json, "A");
                //ConsoleLog_Texbox.Text = prorcess_info;

                //args[0] = args[0].Replace("\"", "'");
                //string prorcess_info = string.Format("{0}※{1}※{2}", args[0], Send_Data_Json, "A");
                ConsoleLog_Texbox.Text = (prorcess_info);

                string app_path = Directory.GetParent(System.IO.Path.GetDirectoryName(Application.ExecutablePath)).FullName + @"\SubSys_ScoreCardCalculator\ScorecardCalculate.exe";
                Process.Start(app_path, prorcess_info);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        private void ProcessRatio_Computing_Click(object sender, EventArgs e)
        {
            Button send_btn = sender as Button;

            if (Require_Mission_Listbox.SelectedItem != null)
            {
                string mission_summary_data = Require_Mission_Listbox.SelectedItem.ToString();
                string[] mission_data_arr = mission_summary_data.Split('.');
                string c_key = sign_plugin.get_mission_information(mission_data_arr[0]).binding_project_id;
                string[] c_key_arr = c_key.Split('_');
                List<double> ProcessRatio_List = new List<double>();

                AuthCalculate authCalculate = new AuthCalculate(c_key_arr[0], c_key_arr[1]);

                if (send_btn.Name.Contains("Level4"))
                {
                    ProcessRatio_List = authCalculate.CalculateByAmountAndRate(decimal.Parse(Signed_Level4_Amount_Textbox.Text), double.Parse(Signed_Level4_Rate_Textbox.Text), int.Parse(Signed_Level4_PrePayMonth_Combobox.SelectedValue.ToString()));
                    Signed_Level4_ProcessRatio_Textbox.Text = ProcessRatio_List[0].ToString();
                    Signed_Level4_AmountProcessRation_Textbox_TextChanged(sender, new EventArgs());
                    PassAndDoneButton_Switch.getInstance().Level4_PassDoneButton_Switch = false;
                }
                else if (send_btn.Name.Contains("Level3"))
                {
                    ProcessRatio_List = authCalculate.CalculateByAmountAndRate(decimal.Parse(Signed_Level3_Amount_Textbox.Text), double.Parse(Signed_Level3_Rate_Textbox.Text), int.Parse(Signed_Level3_PrePayMonth_Combobox.SelectedValue.ToString()));
                    Signed_Level3_ProcessRatio_Textbox.Text = ProcessRatio_List[0].ToString();
                    Signed_Level3_AmountProcessRation_Textbox_TextChanged(sender, new EventArgs());
                    PassAndDoneButton_Switch.getInstance().Level3_PassDoneButton_Switch = false;
                }
                else if (send_btn.Name.Contains("Level2"))
                {
                    ProcessRatio_List = authCalculate.CalculateByAmountAndRate(decimal.Parse(Signed_Level2_Amount_Textbox.Text), double.Parse(Signed_Level2_Rate_Textbox.Text), int.Parse(Signed_Level2_PrePayMonth_Combobox.SelectedValue.ToString()));
                    Signed_Level2_ProcessRatio_Textbox.Text = ProcessRatio_List[0].ToString();
                    Signed_Level2_AmountProcessRation_Textbox_TextChanged(sender, new EventArgs());
                    PassAndDoneButton_Switch.getInstance().Level2_PassDoneButton_Switch = false;
                }
                else if (send_btn.Name.Contains("Level1"))
                {
                    compare_SpecialNote(Signed_Level1_Amount_Textbox.Text, Signed_Level1_ProcessRatio_Textbox.Text);
                    ProcessRatio_List = authCalculate.CalculateByAmountAndRate(decimal.Parse(Signed_Level1_Amount_Textbox.Text), double.Parse(Signed_Level1_Rate_Textbox.Text), int.Parse(Signed_Level1_PrePayMonth_Combobox.SelectedValue.ToString()));
                    Signed_Level1_ProcessRatio_Textbox.Text = ProcessRatio_List[0].ToString();
                    PassAndDoneButton_Switch.getInstance().Level1_PassDoneButton_Switch = false;
                }

            }
        }
        private void ReviewForm_Button_Click(object sender, EventArgs e)
        {
            Data_Set_Mission_Details mission_data = sign_plugin.get_mission_information(Current_Select_Mission_No);
            ApprovalLevel_Controller approvalLevel_Controller = ApprovalLevel_Controller.getInstance();
            approvalLevel_Controller.getPage6_Form_ReadOnly(mission_data.binding_project_id);
        }
        private void Telephone_investigation_Button_Click(object sender, EventArgs e)
        {
            ApprovalLevel_Controller approvalLevel_Controller = ApprovalLevel_Controller.getInstance();
            Data_Set_Mission_Details mission_data = sign_plugin.get_mission_information(Current_Select_Mission_No);
            approvalLevel_Controller.getCellPhone_Form_ReadOnly(mission_data.binding_project_id);
        }
        #endregion



        #endregion


        #endregion

        #region 記住資料區
        private void UpdateUIData()
        {
            // Example 開啟自動載入帳號、密碼、Token
            Controller_Sign sign = Controller_Sign.getInstance();

            Properties.Settings.Default.Account = Account_Textbox.Text;
            Properties.Settings.Default.Password = Password_Textbox.Text;
            Properties.Settings.Default.Token = JsonConvert.SerializeObject(sign_plugin.Current_Token);
            Properties.Settings.Default.Save();
            Account_Textbox.Enabled = false;
            Password_Textbox.Enabled = false;
            Login_Button.Enabled = false;
        }
        private void Load_Data()
        {
            // Example 
            Controller_Sign sign = Controller_Sign.getInstance();
            Data_Set_Excutre_Result result = sign_plugin.CompareTokenvalid();

            // Show Result
            if (result.excute_result.isSuccesed)
            {
                employee_data = sign_plugin.get_employee_information(employee_data.e_id);
                //Initial_Form(sign_plugin.get_employee_information(employee_data.e_id));
                UpdateUIData();
            }

            if (sign_plugin.CompareTokenvalid().excute_result.isSuccesed)
            {
                Logout_button.Enabled = true;
            }
        }
        public void Set_args(string[] receive_args)
        {
            args = receive_args;
        }

        #endregion

        #region 顯示小輔助
        public static string changeToMoneyType(decimal inputMoney)
        {
            if (inputMoney == 0)
            {
                return "0";
            }
            else
            {
                return inputMoney.ToString("###,###");
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            rotate90Picture(pictureBox2);
        }
        public static void rotate90Picture(PictureBox pb)
        {
            Image img1 = pb.Image;
            img1.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pb.Image = img1;
        }
        private void showNotification(string title, string content, int keepTime)
        {
            notifyIcon1.BalloonTipTitle = title;
            notifyIcon1.BalloonTipText = content;
            notifyIcon1.ShowBalloonTip(keepTime);
        }
        #endregion

        #region Pipe資料通道
        PipeController pipecontroller = PipeController.createInstance();

        public void InitPipe()
        {
            pipecontroller.startPipe(PipeType.Head, "sign_score", responseAction, transactionEndAction);

            PipeData pipeData = new PipeData();

            pipeData.type = "Receive";
            pipeData.result_feedback = new List<string>();
            pipeData.result_feedback.Add("Send need computing by ScoreCard Data to Kai");
            pipeData.isSuccess = true;

            pipecontroller.sendMessage(pipeData);

        }

        private PipeData responseAction(PipeData input)
        {
            PipeData pipeData = new PipeData();
            try
            {
                if (input.type.Equals("ScoreCard_Result"))
                {
                    string Recv_Json = JsonConvert.SerializeObject(input);
                    string Receive_JsonResult = input.result_feedback[0];
                    //this.BeginInvoke(new UpdateUI(_UpdateUI_ConsoleTextbox), new object[] { String.Format("[Pipe通道]接收到結果：{0}", Receive_JsonResult) });
                    //this.BeginInvoke(new UpdateUI(_UpdateUI_ConsoleTextbox), new object[] { String.Format("Done_ScoreComputing") });
                    this.BeginInvoke(new UpdateProcessUI(_UpdateProcessRatio), input);
                }
                else if (input.type.Equals("ScoreCard_Closing"))
                {
                    this.BeginInvoke(new UpdateProcessUI(_UpdateUI_EnableRequireListbox), input);
                }


            }
            catch (Exception ex)
            {
                pipeData.type = input.type; /// Func 
                pipeData.result_feedback = new List<string>();
                pipeData.result_feedback.Add(ex.Message.ToString());
                pipeData.isSuccess = true;

                MessageBox.Show(string.Format("發生未預期之錯誤\r\n錯誤訊息{0}\r\n請聯絡【研發中心-郁宸】", ex.Message.ToString()));
                //this.BeginInvoke(new UpdateUI(_UpdateUI_ConsoleTextbox), new object[] { String.Format("[Pipe通道]發生未知錯誤，問題描述：{0}    請聯絡【研發中心-郁宸】", ex.Message) });
            }

            return pipeData;
        }
        private void transactionEndAction(PipeData input)
        {

            if ((input.type != null) && !input.type.Equals("stop_this_success"))
            {
                //GobalPipeData = input;
                try
                {

                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new UpdateUI(_UpdateUI_ConsoleTextbox), new object[] { ex.Message.ToString() });

                }
            }
            else
            {
                //this.BeginInvoke(new UpdateUI(_UpdateUI_ConsoleTextbox), new object[] { String.Format("[Pipe通道]無接收到訊息") });
                Console.WriteLine("");
            }
        }

        /// <summary>
        /// 委派更新UI
        /// </summary>
        /// <param name="data"></param>
        delegate void UpdateUI(string data);
        delegate void UpdateProcessUI(PipeData data);

        private void _UpdateUI_ConsoleTextbox(string data)
        {
            ConsoleLog_Texbox.Text += string.Format("{0}{1}", data, "\r\n");
            if (data.Equals("Done_ScoreComputing"))
            {
                ScoreCardCalculator_Level1_Button.Enabled = false;
                ScoreCardCalculator_Level2_Button.Enabled = false;
                ScoreCardCalculator_Level3_Button.Enabled = false;
                ScoreCardCalculator_Level4_Button.Enabled = false;
            }
        }
        public void _UpdateUI_EnableRequireListbox(PipeData data)
        {
            if (data.type.Equals("ScoreCard_Closing"))
            {
                Require_Mission_Listbox.Enabled = true;
                Signature_Function_TabControl.Enabled = true;
            }
        }
        private void _UpdateProcessRatio(PipeData data)
        {
            try
            {
                PipeData_ScoreCardCalculator pipeData = JsonConvert.DeserializeObject<PipeData_ScoreCardCalculator>(data.result_feedback[0]);
                Mission_CurrentAmount_Max_Label.Text = pipeData.suggestion_Amount.ToString();
                if (pipeData.level.Contains("Level1"))
                {
                    Signed_Level1_ProcessRatio_Textbox.Text = pipeData.proceess_ratio.ToString();
                    Signed_Level1_Amount_Textbox.Text = pipeData.amount;
                    Signed_Level1_Rate_Textbox.Text = pipeData.rate;
                    ScoreCardCalculator_Level1_Button.Enabled = true;

                }
                else if (pipeData.level.Contains("Level2"))
                {
                    Signed_Level2_ProcessRatio_Textbox.Text = pipeData.proceess_ratio.ToString();
                    Signed_Level2_Amount_Textbox.Text = pipeData.amount;
                    Signed_Level2_Rate_Textbox.Text = pipeData.rate;
                    ScoreCardCalculator_Level2_Button.Enabled = true;

                }
                else if (pipeData.level.Contains("Level3"))
                {
                    Signed_Level3_ProcessRatio_Textbox.Text = pipeData.proceess_ratio.ToString();
                    Signed_Level3_Amount_Textbox.Text = pipeData.amount;
                    Signed_Level3_Rate_Textbox.Text = pipeData.rate;
                    ScoreCardCalculator_Level3_Button.Enabled = true;

                }
                else if (pipeData.level.Contains("Level4"))
                {
                    Signed_Level4_ProcessRatio_Textbox.Text = pipeData.proceess_ratio.ToString();
                    Signed_Level4_Amount_Textbox.Text = pipeData.amount.ToString();
                    Signed_Level4_Rate_Textbox.Text = pipeData.rate.ToString();
                    ScoreCardCalculator_Level4_Button.Enabled = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("[Pipe通道]產生資料解析錯誤，請聯絡【研發中心-郁宸】"));
            }



        }










        #endregion

        #region UI_EVENT_委派區
        delegate void Thread_UpdateUI_ListboxAdd(ListBox listbox, string value);
        delegate void Thread_UpdateUI_ProgressBarbarAdd(System.Windows.Forms.ProgressBar progressBar);
        delegate void Thread_UpdateUI_ProgressBarbarSetMax(System.Windows.Forms.ProgressBar progressBar, int value);
        delegate void Thread_UpdatUI_Refresh();
        delegate void Thread_Refresh_Listbox();
        public void ThreadLoad(string Mode)
        {
            try
            {
                switch (Mode)
                {
                    case "新增簽核類別": // 新增任務類別
                        Update_Sign_ButtonANDTextbox_UI(false);
                        LoadRegisterNewMissionType();
                        break;
                    case "需簽核": // 需簽核
                        Update_Sign_ButtonANDTextbox_UI(true);
                        Thread LoadRequirMission_Thread = new Thread(Thread_LoadRequirMission);
                        LoadRequirMission_Thread.Start();
                        break;

                    case "全部任務": // 全部任務
                        Update_Sign_ButtonANDTextbox_UI(false);
                        Thread LoadAllMission_Thread = new Thread(Thread_LoadAllMission);
                        LoadAllMission_Thread.Start();
                        break;

                    case "已簽核": // 已簽核完畢
                        Update_Sign_ButtonANDTextbox_UI(false);
                        Thread LoadSignedMission_Thread = new Thread(Thread_LoadSignedMission);
                        LoadSignedMission_Thread.Start();
                        break;
                    case "已拒絕": // 已拒絕之簽核清單
                        Update_Sign_ButtonANDTextbox_UI(false);
                        Thread LoadFailMission_Thread = new Thread(Thread_LoadFailMission);
                        LoadFailMission_Thread.Start();
                        break;
                    case "歷史紀錄": // 簽核歷史紀錄
                        Update_Sign_ButtonANDTextbox_UI(false);
                        break;
                    case "已完成": //已完成
                        Update_Sign_ButtonANDTextbox_UI(false);
                        Thread LoadDoneMission_Thread = new Thread(Thread_LoadDoneMission);
                        this.BeginInvoke(new Thread_UpdatUI_Refresh(Update_Sign_Panel_ReadOnly));///需修改
                        LoadDoneMission_Thread.Start();
                        break;
                    case "進行中": //進行中
                        Update_Sign_ButtonANDTextbox_UI(false);
                        Thread LoadSigningMission_Thread = new Thread(Thread_LoadSigningMission);
                        LoadSigningMission_Thread.Start();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }


        }
        public void Thread_LoadAllMission()
        {
            this.BeginInvoke(new Thread_UpdatUI_Refresh(Thread_Lock_TabPageControl));

            // Example
            this.BeginInvoke(new Thread_UpdatUI_Refresh(RefreshAllUIObject));
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            List<Information_Mission> list_information_mission = sign_plugin.get_employee_mission_information_all_loanit(employee_data.e_id);

            this.BeginInvoke(new Thread_UpdateUI_ProgressBarbarSetMax(Thread_ProgressBar_SetMax), progressBar1, list_information_mission.Count);

            // Show Result
            foreach (Information_Mission information_mission in list_information_mission)
            {

                if (information_mission.mission_status.Equals("4"))
                {
                    string customer_name = sign_plugin.getCustomerName(information_mission.binding_project_id);


                    int total_requiresign = sign_plugin.get_mission_information(information_mission.mission_id).require_sign.Count;
                    int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission.mission_id))); /// 過濾剩下需要多少簽
                    string current_requiresign = (total_requiresign - remains_requiresign).ToString();

                    this.BeginInvoke(new Thread_UpdateUI_ListboxAdd(ThreadUIEvenet_ListboxAdd), All_Mission_Listbox, string.Format("{0}./ 案件代號：{1}{5}    ({2}/{3})   {4}"
                        , information_mission.mission_id, information_mission.binding_project_id
                        , current_requiresign, total_requiresign, "退回重簽中", customer_name));
                }
                else
                {
                    int total_requiresign = sign_plugin.get_mission_information(information_mission.mission_id).require_sign.Count;
                    int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission.mission_id))); /// 過濾剩下需要多少簽
                    string current_requiresign = (total_requiresign - remains_requiresign).ToString();

                    string customer_name = sign_plugin.getCustomerName(information_mission.binding_project_id);


                    if (information_mission.mission_status.Equals("99"))
                    {
                        this.BeginInvoke(new Thread_UpdateUI_ListboxAdd(ThreadUIEvenet_ListboxAdd), All_Mission_Listbox, string.Format("{0}./ 案件代號：{1}{5}    ({2}/{3})   {4}"
                            , information_mission.mission_id, information_mission.binding_project_id, current_requiresign, total_requiresign, "已拒絕", customer_name));
                    }
                    else if (remains_requiresign == 0)
                    {
                        this.BeginInvoke(new Thread_UpdateUI_ListboxAdd(ThreadUIEvenet_ListboxAdd), All_Mission_Listbox, string.Format("{0}./ 案件代號：{1}{5}    ({2}/{3})   {4}"
                            , information_mission.mission_id, information_mission.binding_project_id, current_requiresign, total_requiresign, "已核定", customer_name));
                    }
                    else if (remains_requiresign == 1)
                    {
                        this.BeginInvoke(new Thread_UpdateUI_ListboxAdd(ThreadUIEvenet_ListboxAdd), All_Mission_Listbox, string.Format("{0}./ 案件代號：{1}{5}    ({2}/{3})   {4}"
                            , information_mission.mission_id, information_mission.binding_project_id, current_requiresign, total_requiresign, "待核定", customer_name));
                    }
                    else
                    {
                        this.BeginInvoke(new Thread_UpdateUI_ListboxAdd(ThreadUIEvenet_ListboxAdd), All_Mission_Listbox, string.Format("{0}./ 案件代號：{1}{5}    ({2}/{3})   {4}"
                            , information_mission.mission_id, information_mission.binding_project_id, current_requiresign, total_requiresign, "需簽核", customer_name));
                    }
                }
                this.BeginInvoke(new Thread_UpdateUI_ProgressBarbarAdd(Thread_ProgressBar_Increase), progressBar1);
            }
            //ConsoleLog_Texbox.Text += String.Format("{0}\r\n", "已完成【所有任務】資料載入");
            this.BeginInvoke(new Thread_UpdatUI_Refresh(Thread_Unlock_TabPageControl));
            this.BeginInvoke(new Thread_Refresh_Listbox(Fillter_TestData_Listbox));
        }
        public void Thread_LoadRequirMission()
        {
            this.BeginInvoke(new Thread_UpdatUI_Refresh(Thread_Lock_TabPageControl));

            LOANIT_CONTROLLER_Plugin controller_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            // Example
            this.BeginInvoke(new Thread_UpdatUI_Refresh(RefreshAllUIObject));
            List<Information_Mission> list_information_mission_requirment_signature = controller_plugin.get_employee_miossion_information_requirment_loanit(employee_data.e_id);
            List<Information_Mission> list_information_mission_requirment_done = sign_plugin.get_employee_mission_information_requirement_done(employee_data.e_id);

            this.BeginInvoke(new Thread_UpdateUI_ProgressBarbarSetMax(Thread_ProgressBar_SetMax), progressBar1, (list_information_mission_requirment_done.Count + list_information_mission_requirment_signature.Count) != 0 ? ((list_information_mission_requirment_done.Count + list_information_mission_requirment_signature.Count)) : 1);

            // Show Result
            foreach (Information_Mission information_mission_requirment_signature in list_information_mission_requirment_signature)
            {
                if (!information_mission_requirment_signature.mission_status.Equals("99"))
                {

                    string customer_name = sign_plugin.getCustomerName(information_mission_requirment_signature.binding_project_id);


                    int total_requiresign = sign_plugin.get_mission_information(information_mission_requirment_signature.mission_id).require_sign.Count;
                    int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission_requirment_signature.mission_id))); /// 過濾剩下需要多少簽
                    string current_requiresign = (total_requiresign - remains_requiresign).ToString();

                    this.BeginInvoke(new Thread_UpdateUI_ListboxAdd(ThreadUIEvenet_ListboxAdd), Require_Mission_Listbox, string.Format("{0}./ 案件代號：{1}{5}  ({2}/{3})  {4}"
                        , information_mission_requirment_signature.mission_id, information_mission_requirment_signature.binding_project_id
                        , current_requiresign, total_requiresign, "需簽核", customer_name));

                }
                this.BeginInvoke(new Thread_UpdateUI_ProgressBarbarAdd(Thread_ProgressBar_Increase), progressBar1);
            }
            // Show Result-需核定
            foreach (Information_Mission information_mission_requirment_signature in list_information_mission_requirment_done)
            {
                if (!information_mission_requirment_signature.mission_status.Equals("99"))
                {
                    string customer_name = sign_plugin.getCustomerName(information_mission_requirment_signature.binding_project_id);

                    int total_requiresign = sign_plugin.get_mission_information(information_mission_requirment_signature.mission_id).require_sign.Count;
                    int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission_requirment_signature.mission_id))); /// 過濾剩下需要多少簽
                    string current_requiresign = (total_requiresign - remains_requiresign).ToString();

                    this.BeginInvoke(new Thread_UpdateUI_ListboxAdd(ThreadUIEvenet_ListboxAdd), Require_Mission_Listbox, string.Format("{0}./ 案件代號：{1}{5}  ({3}/{4})  {2}",
                        information_mission_requirment_signature.mission_id, information_mission_requirment_signature.binding_project_id, "需核定",
                        current_requiresign, total_requiresign, customer_name));
                }
                this.BeginInvoke(new Thread_UpdateUI_ProgressBarbarAdd(Thread_ProgressBar_Increase), progressBar1);
            }
            this.BeginInvoke(new Thread_UpdatUI_Refresh(Thread_Unlock_TabPageControl));
            this.BeginInvoke(new Thread_Refresh_Listbox(Fillter_TestData_Listbox));

            //ConsoleLog_Texbox.Text += String.Format("{0}\r\n", "已完成【需簽核任務】資料載入");
        }
        public void Thread_LoadSignedMission()
        {
            this.BeginInvoke(new Thread_UpdatUI_Refresh(Thread_Lock_TabPageControl));

            // Example
            this.BeginInvoke(new Thread_UpdatUI_Refresh(RefreshAllUIObject));
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();

            List<Data_Set_Sign_History> sign_history_list = sign_plugin.get_employee_signed_information(employee_data.publickey);

            this.BeginInvoke(new Thread_UpdateUI_ProgressBarbarSetMax(Thread_ProgressBar_SetMax), progressBar1, sign_history_list.Count != 0 ? sign_history_list.Count : 1);
            // Show Result
            foreach (Data_Set_Sign_History signed_history in sign_history_list)
            {
                Data_Set_Mission_Details information_mission = sign_plugin.get_mission_information(signed_history.mission_id);
                if (!information_mission.m_id.Contains("不存在"))
                {
                    /// 撈取客戶姓名
                    string customer_name = sign_plugin.getCustomerName(information_mission.binding_project_id);

                    int total_requiresign = sign_plugin.get_mission_information(information_mission.m_id).require_sign.Count;
                    int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission.m_id))); /// 過濾剩下需要多少簽
                    string current_requiresign = (total_requiresign - remains_requiresign).ToString();

                    this.BeginInvoke(new Thread_UpdateUI_ListboxAdd(ThreadUIEvenet_ListboxAdd), Signed_Mission_Listbox, string.Format("{0}./ 案件代號：{1}{5}  ({3}/{4})  {2}"
                        , information_mission.m_id, information_mission.binding_project_id, "已簽核", current_requiresign, total_requiresign, customer_name));
                }
                else
                {
                    this.BeginInvoke(new Thread_UpdateUI_ListboxAdd(ThreadUIEvenet_ListboxAdd), Signed_Mission_Listbox, string.Format("{0}", information_mission.m_id));
                }
                this.BeginInvoke(new Thread_UpdateUI_ProgressBarbarAdd(Thread_ProgressBar_Increase), progressBar1);
            }
            this.BeginInvoke(new Thread_UpdatUI_Refresh(Thread_Unlock_TabPageControl));
            this.BeginInvoke(new Thread_Refresh_Listbox(Fillter_TestData_Listbox));

            //ConsoleLog_Texbox.Text += String.Format("{0}\r\n", "已完成【已簽核任務】資料載入");
        }
        public void Thread_LoadDoneMission()
        {
            this.BeginInvoke(new Thread_UpdatUI_Refresh(Thread_Lock_TabPageControl));

            // Example
            this.BeginInvoke(new Thread_UpdatUI_Refresh(RefreshAllUIObject));
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            List<Information_Mission> list_information_mission = sign_plugin.get_employee_mission_information_all_loanit(employee_data.e_id);

            this.BeginInvoke(new Thread_UpdateUI_ProgressBarbarSetMax(Thread_ProgressBar_SetMax), progressBar1, list_information_mission.Count != 0 ? list_information_mission.Count : 1);
            // Show Result
            foreach (Information_Mission information_mission in list_information_mission)
            {
                if (information_mission.mission_status.Equals("3"))
                {
                    string customer_name = sign_plugin.getCustomerName(information_mission.binding_project_id);


                    int total_requiresign = sign.get_mission_information(information_mission.mission_id).require_sign.Count;
                    int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission.mission_id))); /// 過濾剩下需要多少簽
                    string current_requiresign = (total_requiresign - remains_requiresign).ToString();

                    this.BeginInvoke(new Thread_UpdateUI_ListboxAdd(ThreadUIEvenet_ListboxAdd), Done_Mission_Listbox, string.Format("{0}./ 案件代號：{1}{5}  ({2}/{3})   {4}"
                        , information_mission.mission_id, information_mission.binding_project_id
                        , current_requiresign, total_requiresign, "已核定", customer_name));
                }
                this.BeginInvoke(new Thread_UpdateUI_ProgressBarbarAdd(Thread_ProgressBar_Increase), progressBar1);
            }
            this.BeginInvoke(new Thread_UpdatUI_Refresh(Thread_Unlock_TabPageControl));

            //ConsoleLog_Texbox.Text += String.Format("{0}\r\n", "已完成【已核定的任務】資料載入");
            this.BeginInvoke(new Thread_UpdatUI_Refresh(Thread_Enable_ExportApproveDoucmentButton));
            this.BeginInvoke(new Thread_Refresh_Listbox(Fillter_TestData_Listbox));

        }
        public void Thread_LoadFailMission()
        {
            this.BeginInvoke(new Thread_UpdatUI_Refresh(Thread_Lock_TabPageControl));

            // Example
            this.BeginInvoke(new Thread_UpdatUI_Refresh(RefreshAllUIObject));
            Controller_Sign sign = Controller_Sign.getInstance();
            List<Data_Set_Mission_Details> fail_mission_list = sign_plugin.get_employee_mission_information_fail(employee_data);
            this.BeginInvoke(new Thread_UpdateUI_ProgressBarbarSetMax(Thread_ProgressBar_SetMax), progressBar1, fail_mission_list.Count != 0 ? fail_mission_list.Count : 1);

            foreach (Data_Set_Mission_Details fail_mission in fail_mission_list)
            {
                string customer_name = sign_plugin.getCustomerName(fail_mission.binding_project_id);

                // Show Result
                this.BeginInvoke(new Thread_UpdateUI_ListboxAdd(ThreadUIEvenet_ListboxAdd), Fail_Mission_Listbox, string.Format("{0}./ 案件代號：{1}{3} {2}"
                    , fail_mission.m_id, fail_mission.binding_project_id, "已拒絕", customer_name));
            }

            if (Fail_Mission_Listbox.Items.Count == 0)
            {
                this.BeginInvoke(new Thread_UpdateUI_ListboxAdd(ThreadUIEvenet_ListboxAdd), Fail_Mission_Listbox, string.Format("不存在已拒絕的簽核任務"));
            }
            this.BeginInvoke(new Thread_UpdateUI_ProgressBarbarAdd(Thread_ProgressBar_Increase), progressBar1);
            //ConsoleLog_Texbox.Text += String.Format("{0}\r\n", "已完成【簽核失敗任務】資料載入");
            this.BeginInvoke(new Thread_UpdatUI_Refresh(Thread_Unlock_TabPageControl));
            this.BeginInvoke(new Thread_Refresh_Listbox(Fillter_TestData_Listbox));

        }
        public void Thread_LoadSigningMission()
        {
            this.BeginInvoke(new Thread_UpdatUI_Refresh(Thread_Lock_TabPageControl));

            // Example
            this.BeginInvoke(new Thread_UpdatUI_Refresh(RefreshAllUIObject));
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            List<Information_Mission> list_information_mission = sign_plugin.get_employee_mission_information_all_loanit(employee_data.e_id);

            this.BeginInvoke(new Thread_UpdateUI_ProgressBarbarSetMax(Thread_ProgressBar_SetMax), progressBar1, list_information_mission.Count);
            // Show Result
            foreach (Information_Mission information_mission in list_information_mission)
            {
                if (information_mission.mission_status.Equals("2") || information_mission.mission_status.Equals("0"))
                {
                    string customer_name = sign_plugin.getCustomerName(information_mission.binding_project_id);


                    int total_requiresign = sign_plugin.get_mission_information(information_mission.mission_id).require_sign.Count;
                    int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission.mission_id))); /// 過濾剩下需要多少簽
                    string current_requiresign = (total_requiresign - remains_requiresign).ToString();

                    if (remains_requiresign == 1)
                    {
                        this.BeginInvoke(new Thread_UpdateUI_ListboxAdd(ThreadUIEvenet_ListboxAdd), Signing_Mission_Listbox, string.Format("{0}./ 案件代號：{1}{5}    ({2}/{3})   {4}"
                            , information_mission.mission_id, information_mission.binding_project_id, current_requiresign
                            , total_requiresign, "待核定", customer_name));
                    }
                    else
                    {
                        this.BeginInvoke(new Thread_UpdateUI_ListboxAdd(ThreadUIEvenet_ListboxAdd), Signing_Mission_Listbox, string.Format("{0}./ 案件代號：{1}{5}    ({2}/{3})   {4}"
                            , information_mission.mission_id, information_mission.binding_project_id
                            , current_requiresign, total_requiresign, "需簽核", customer_name));
                    }

                }
                else if (information_mission.mission_status.Equals("4"))
                {
                    int total_requiresign = sign_plugin.get_mission_information(information_mission.mission_id).require_sign.Count;
                    int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission.mission_id))); /// 過濾剩下需要多少簽
                    string current_requiresign = (total_requiresign - remains_requiresign).ToString();

                    ApprovalLevel_Controller approvalLevel_Controller = ApprovalLevel_Controller.getInstance();
                    Approval_Notice customer_data = approvalLevel_Controller.getApprovalNotice(information_mission.binding_project_id)[0];

                    this.BeginInvoke(new Thread_UpdateUI_ListboxAdd(ThreadUIEvenet_ListboxAdd), Signing_Mission_Listbox, string.Format("{0}./ 案件代號：{1}{5}    ({2}/{3})   {4}"
                        , information_mission.mission_id, information_mission.binding_project_id
                        , current_requiresign, total_requiresign, "退回重簽中", customer_data.customer_name));

                }
                this.BeginInvoke(new Thread_UpdateUI_ProgressBarbarAdd(Thread_ProgressBar_Increase), progressBar1);
            }
            this.BeginInvoke(new Thread_UpdatUI_Refresh(Thread_Unlock_TabPageControl));
            this.BeginInvoke(new Thread_Refresh_Listbox(Fillter_TestData_Listbox));

            //ConsoleLog_Texbox.Text += String.Format("{0}\r\n", "已完成【已核定的任務】資料載入");
        }
        public void ThreadUIEvenet_ListboxAdd(ListBox listbox, string Value)
        {
            listbox.Items.Add(Value);
        }
        private void RefreshAllUIObject()
        {
            Fail_Mission_Listbox.Items.Clear();
            Require_Mission_Listbox.Items.Clear();
            Done_Mission_Listbox.Items.Clear();
            Signing_Mission_Listbox.Items.Clear();
            All_Mission_Listbox.Items.Clear();
            Signed_Mission_Listbox.Items.Clear();
            progressBar1.Value = 0;
        }
        private void Thread_Enable_ExportApproveDoucmentButton()
        {
            Export_ApprovedDocument_Button.Visible = true;
        }
        private void Thread_ProgressBar_Increase(System.Windows.Forms.ProgressBar progressbar)
        {
            progressbar.Value++;
        }
        private void Thread_Lock_TabPageControl()
        {
            this.Signature_Function_TabControl.Enabled = false;
        }
        private void Thread_Unlock_TabPageControl()
        {
            this.Signature_Function_TabControl.Enabled = true;
        }
        private void Thread_ProgressBar_SetMax(System.Windows.Forms.ProgressBar progressbar, int Max)
        {
            progressbar.Maximum = Max;
        }
        private void Thread_SendMail(string m_id, int risk_value)
        {
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            DemoPluginFunction demoPluginFunction = new DemoPluginFunction();
            demoPluginFunction.SendNextSignMissionMail(sign_plugin.get_mission_information(m_id), risk_value);
        }
        #endregion

        private void ReloadData_Click(object sender, EventArgs e)
        {
            sign_plugin.ClearTempData();
        }

        private int GetScoreCardHistoryHeigh(string c_key)
        {
            string[] Customer_Data = c_key.Split('_');
            AuthCalculate GetMaximumLogReqAmt = new AuthCalculate(Customer_Data[0], Customer_Data[1]);
            return Convert.ToInt32(GetMaximumLogReqAmt.GetMaximumLogReqAmt());
        }

        private string GetPrePayMonthText(string mounth)
        {
            using (ScorecardEF db = new ScorecardEF())
            {
                REPO_DISTRIBUTORS_PREPAID Repo_Distributors = new REPO_DISTRIBUTORS_PREPAID(db);
                List<DISTRIBUTORS_PREPAID> DISTRIBUTORS_PREPAID_List = Repo_Distributors.GetByCondition(x => x.ACT_TYP.ToUpper().Equals("A")).ToList();
                foreach (DISTRIBUTORS_PREPAID item in DISTRIBUTORS_PREPAID_List)
                {
                    if (item.DTR_PPY_SN.ToString().Equals(mounth))
                    {
                        return item.DTR_PPY_TXT;
                    }
                }
            }
            return mounth;
        }
    }




    public class PassAndDoneButton_Switch
    {
        public bool Level4_PassDoneButton_Switch { get; set; }
        public bool Level3_PassDoneButton_Switch { get; set; }
        public bool Level2_PassDoneButton_Switch { get; set; }
        public bool Level1_PassDoneButton_Switch { get; set; }


        public void OpenAllSwitch()
        {
            Level4_PassDoneButton_Switch = true;
            Level3_PassDoneButton_Switch = true;
            Level2_PassDoneButton_Switch = true;
            Level1_PassDoneButton_Switch = true;
        }


        public static PassAndDoneButton_Switch Instance = new PassAndDoneButton_Switch();
        public static PassAndDoneButton_Switch getInstance()
        {
            return Instance;
        }
        private PassAndDoneButton_Switch()
        {

        }

    }
}
