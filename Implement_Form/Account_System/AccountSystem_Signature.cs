using PTMB_Signature.Model.Plugin;
using PTMB_Signature_API.Data_Set;
using PTMB_Signature_API.Informatio_Set;
using PTMB_Signature_API.Model.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VsPipe;
using PTMB_Signature.Implement_Form.Account_System;
using Newtonsoft.Json;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections;

namespace PTMB_Signature.Implement_Form
{
    public partial class AccountSystem_Signature : Form
    {
        VsPipeServer server = new VsPipeServer();
        VsPipeClient client = new VsPipeClient();
        PipeController pipecontroller = PipeController.createInstance();

        PipeData GobalPipeData = new PipeData();
        bool InitProgressbar = true;

        public AccountSystem_Signature()
        {
            try
            {
                InitializeComponent();

            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("[簽核系統]發生未知問題，問題描述：{0}    請聯絡【研發中心-郁宸】\r\n", ex.Message));
                groupBox1.Enabled = false;
            }

        }

        public void InitPipe()
        {

            pipecontroller.startPipe(PipeType.Tail, "sign", responseAction, transactionEndAction);


            PipeData pipeData = new PipeData();

            pipeData.type = "c_keys$CH"; /// Send 1. 撈取所有c_keys
            pipeData.result_feedback = new List<string>();
            pipeData.result_feedback.Add("Send Message To David");
            pipeData.isSuccess = true;

            pipecontroller.sendMessage(pipeData);

        }

        private PipeData responseAction(PipeData input)
        {
            PipeData pipeData = new PipeData();

            try
            {
                if (pipeData.type != null)
                {
                    pipeData.type = input.type; /// Func 
                    pipeData.result_feedback = new List<string>();
                    pipeData.result_feedback = input.result_feedback;
                    pipeData.isSuccess = true;
                }

                /// type ->  c_keys 撈取所有帳務的c_keys
                /// m    ->  傳送 mission 資料
                /// mc   ->  完成建帳後收 c_key


                /// 實作從 Client端接到指令後的動作。
                if (input.type.Equals("mc$CH"))
                {
                    LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
                    LOANIT_AccountSignSys_Controller AccountSignSys_Controller = LOANIT_AccountSignSys_Controller.getinstance();

                    DataSet_SignToAccountSys SignToAccountSys = new DataSet_SignToAccountSys();
                    SignToAccountSys.C_Key = input.result_feedback[0];

                    this.BeginInvoke(new UpdateUI(_UpdateUI_ConsoleTextbox), new object[] { String.Format("[簽核系統] 接收到的Ckey：{0}", SignToAccountSys.C_Key) });
                    AccountSignSys_Controller.update_AccountToSignSys_mission_statusid("LOANIT", "CH%", SignToAccountSys.C_Key);


                    sign_plugin.ClearTempData();
                    LoadCanCreateAccount_Mission();

                    this.BeginInvoke(new UpdateUI(_UpdateUI_ConsoleTextbox), new object[] { String.Format("[簽核系統] 專案 {0} 已成功完成建帳。", SignToAccountSys.C_Key) });
                }
                else if (input.type.Equals("c_keys$CH"))
                {
                    GobalPipeData = input;
                    LoadCreatedAccount();

                    this.BeginInvoke(new UpdateUI(_UpdateUI_ConsoleTextbox), new object[] { String.Format("[簽核系統] 已更新尚未同步簽核資料庫列表。") });
                }
                else
                {
                    //this.BeginInvoke(new UpdateUI(_UpdateUI_ConsoleTextbox), new object[] { String.Format("[Pipe通道] 收到未知指令，請聯絡【研發中心-郁宸】。") });
                }
            }
            catch (Exception ex)
            {
                pipeData.type = input.type; /// Func 
                pipeData.result_feedback = new List<string>();
                pipeData.result_feedback.Add(ex.Message.ToString());
                pipeData.isSuccess = true;

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
                    if (input.type.Equals("mc$CH"))
                    {
                        LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
                        LOANIT_AccountSignSys_Controller AccountSignSys_Controller = LOANIT_AccountSignSys_Controller.getinstance();

                        DataSet_SignToAccountSys SignToAccountSys = new DataSet_SignToAccountSys();
                        SignToAccountSys.C_Key = input.result_feedback[0];

                        AccountSignSys_Controller.update_AccountToSignSys_mission_statusid("LOANIT", "CH%", SignToAccountSys.C_Key);

                        LoadCanCreateAccount_Mission();

                        this.BeginInvoke(new UpdateUI(_UpdateUI_ConsoleTextbox), new object[] { String.Format("[簽核系統] 專案 {0} 已成功完成建帳。", SignToAccountSys.C_Key) });

                        //輸出還沒同步到簽核系統的 c_key
                        //foreach (string item in input.result_feedback)
                        //{
                        //    this.BeginInvoke(new UpdateUI(_UpdateUI_ConsoleTextbox), new object[] { item.ToString() });
                        //}
                    }
                    else if (input.type.Equals("c_keys$CH"))
                    { // Receive 1. 

                        GobalPipeData = input;
                        LoadCreatedAccount();

                        this.BeginInvoke(new UpdateUI(_UpdateUI_ConsoleTextbox), new object[] { String.Format("[簽核系統] 已更新尚未同步簽核資料庫列表。") });
                    }
                    else if (input.type.Equals("m$CH"))
                    {
                        //this.BeginInvoke(new UpdateUI(_UpdateUI_ConsoleTextbox), new object[] { String.Format("[Pipe通道] 已完成資料傳送。") });
                    }
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




        #region 執行續

        public void LoadThread(string Mode)
        {
            /// 定義副程式
            Thread LoadCreatedAccount = new Thread(LoadCreatedAccount_Mission);
            Thread LoadSigningMission = new Thread(LoadSigningMission_Mission);
            Thread LoadCanCreateAccount = new Thread(LoadCanCreateAccount_Mission);
            _Set_ProgressBar_Max(Mode);



            switch (Mode)
            {
                case "已建帳":
                    CreatedAccount_Listbox.Items.Clear();
                    LoadCreatedAccount.Start();
                    break;
                case "簽核中":
                    Signing_Mission_Listbox.Items.Clear();
                    LoadSigningMission.Start();
                    break;
                case "可建帳":
                    CanCreateAccount_Listbox.Items.Clear();
                    LoadCanCreateAccount.Start();
                    break;
                case "ALL":
                    /// [未完成]這邊要想一下卡控TabControl的結構
                    LoadSigningMission.Start();
                    LoadCreatedAccount.Start();
                    LoadCanCreateAccount.Start();

                    break;
                default:
                    break;
            }
        }

        #endregion


        #region 委派
        delegate void UpdateUI(string data);
        delegate void Thread_Refresh_Listbox();

        private void _UpdateUI_ConsoleTextbox(string data)
        {
            ConsoleLog_Texbox.Text += string.Format("{0}{1}", data, "\r\n");
        }
        private void _Clear_CanCreateListbox(string data)
        {
            CanCreateAccount_Listbox.Items.Clear();
            CreatedAccount_Listbox.Items.Clear();
            Signing_Mission_Listbox.Items.Clear();
        }
        private void _UpdateUI_CanCreateListbox(string data)
        {
            CanCreateAccount_Listbox.Items.Add(data);
        }
        private void _UpdateUI_CreatedListbox(string data)
        {
            CreatedAccount_Listbox.Items.Add(data);
        }
        private void _UpdateUI_SigningListbox(string data)
        {
            Signing_Mission_Listbox.Items.Add(data);
        }
        private void _UpdateUI_ProgressBar(string add_value)
        {
            try
            {
                progressBar1.Value += int.Parse(add_value);
                progress_molecular_label.Text = progressBar1.Value.ToString();

            }
            catch (Exception ex)
            {
                progressBar1.Value = progressBar1.Maximum;
                progress_molecular_label.Text = progressBar1.Value.ToString();
            }
        }
        private void _UpdateUI_TabControlPage(string mode)
        {
            if (mode.Equals("on"))
            {
                Signature_Function_TabControl.Enabled = true;
            }
            else
            {
                Signature_Function_TabControl.Enabled = false;
            }
        }
        private void _Set_ProgressBar_Max(string mode)
        {
            // Example
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            List<Data_Set_Mission_Details> list_information_mission = sign_plugin.get_mission_information_all();
            int count_progressbar_max = 0;

            if (mode.Equals("可建帳"))
            {
                // Example
                foreach (Data_Set_Mission_Details information_mission in list_information_mission)
                {
                    if (information_mission.status_id.Equals("3"))
                    {
                        if(information_mission.company.Equals("LOANIT") && information_mission.mission_type.Contains("CH"))
                        {
                            count_progressbar_max += 1;
                        }
                    }
                }
            }

            if (mode.Equals("已建帳"))
            {
                // Example
                foreach (Data_Set_Mission_Details information_mission in list_information_mission)
                {
                    if (information_mission.status_id.Equals("5") || information_mission.status_id.Equals("51"))
                    {
                        if (information_mission.company.Equals("LOANIT") && information_mission.mission_type.Contains("CH"))
                        {
                            count_progressbar_max += 1;
                        }
                    }
                }
            }

            if (mode.Equals("簽核中"))
            {
                // Example
                foreach (Data_Set_Mission_Details information_mission in list_information_mission)
                {
                    if (!information_mission.status_id.Equals("5") && !information_mission.status_id.Equals("3") && !information_mission.status_id.Equals("99") && !information_mission.status_id.Equals("51"))
                    {
                        if (information_mission.company.Equals("LOANIT") && information_mission.mission_type.Contains("CH"))
                        {
                            count_progressbar_max += 1;
                        }
                    }
                }
            }

            if (mode.Equals("ALL"))
            {

                foreach (Data_Set_Mission_Details information_mission in list_information_mission)
                {
                    /// 0--->尚未簽過
                    /// 2--->簽核中(用數量判斷【需核定、需簽核】)
                    ///3--->已核定
                    ///4--->退回重簽中
                    ///5--->已建帳
                    ///51--->帳務系統既有的已建帳資料

                    /// 可建帳
                    if (information_mission.status_id.Equals("3"))
                    {
                        if (information_mission.company.Equals("LOANIT") && information_mission.mission_type.Contains("CH"))
                        {
                            count_progressbar_max += 1;
                        }
                    }
                    /// 已建帳
                    if (information_mission.status_id.Equals("5") || information_mission.status_id.Equals("51"))
                    {
                        if (information_mission.company.Equals("LOANIT") && information_mission.mission_type.Contains("CH"))
                        {
                            count_progressbar_max += 1;
                        }
                    }
                    /// 簽核中
                    if (!information_mission.status_id.Equals("5") && !information_mission.status_id.Equals("3") && !information_mission.status_id.Equals("99") && !information_mission.status_id.Equals("51"))
                    {
                        if (information_mission.company.Equals("LOANIT") && information_mission.mission_type.Contains("CH"))
                        {
                            count_progressbar_max += 1;
                        }
                    }

                }

            }

            progressBar1.Maximum = count_progressbar_max;
            progress_denominator_label.Text = count_progressbar_max.ToString();
            progressBar1.Value = 0;
        }

        #endregion

        #region UI_Update
        public void LoadCanCreateAccount_Mission()
        {
            if (InitProgressbar == false) { this.BeginInvoke(new Action<string>(_UpdateUI_ProgressBar), string.Format("{0}", 0.ToString())); }

            this.BeginInvoke(new UpdateUI(_Clear_CanCreateListbox), new object[] { string.Format("Refresh") });
            // Example
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            List<Data_Set_Mission_Details> list_information_mission = sign_plugin.get_mission_information_all();
            this.BeginInvoke(new Action<string>(_UpdateUI_TabControlPage), string.Format("{0}", "off"));
            // Show Result
            foreach (Data_Set_Mission_Details information_mission in list_information_mission)
            {
                if (information_mission.company.Equals("LOANIT") && information_mission.mission_type.Contains("CH")) /// 過濾掉非LOANIT的CH任務
                {
                    if (information_mission.status_id.Equals("3") && (information_mission.mission_type.Contains("CH")))
                    {
                        int total_requiresign = sign.get_mission_information(information_mission.m_id).require_sign.Count;
                        int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission.m_id))); /// 過濾剩下需要多少簽
                        string current_requiresign = (total_requiresign - remains_requiresign).ToString();
                        //CanCreateAccount_Listbox.Items.Add(string.Format("{0}./ 案件代號：{1}  ({2}/{3})   {4}", information_mission.m_id, information_mission.binding_project_id, current_requiresign, total_requiresign, "可建帳"));
                        this.BeginInvoke(new UpdateUI(_UpdateUI_CanCreateListbox), new object[] { string.Format("{0}./ 案件代號：{1}  ({2}/{3})   {4}", information_mission.m_id, information_mission.binding_project_id, current_requiresign, total_requiresign, "可建帳") });
                        if (InitProgressbar == false) { this.BeginInvoke(new Action<string>(_UpdateUI_ProgressBar), string.Format("{0}", 1.ToString())); }
                    }

                }
            }
            this.BeginInvoke(new Action<string>(_UpdateUI_TabControlPage), string.Format("{0}", "on"));
            InitProgressbar = false;
            this.BeginInvoke(new Thread_Refresh_Listbox(Fillter_TestData_Listbox));
            //ConsoleLog_Texbox.Text += String.Format("{0}\r\n", "已完成【可建帳之任務】資料載入");
        }
        public void LoadCreatedAccount_Mission()
        {
            // Example
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            List<Data_Set_Mission_Details> list_information_mission = sign_plugin.get_mission_information_all();

            this.BeginInvoke(new Action<string>(_UpdateUI_TabControlPage), string.Format("{0}", "off"));
            // Show Result
            foreach (Data_Set_Mission_Details information_mission in list_information_mission)
            {
                if (information_mission.company.Equals("LOANIT") && information_mission.mission_type.Contains("CH")) /// 過濾掉非LOANIT的CH任務
                {
                    if ((information_mission.status_id.Equals("5") || information_mission.status_id.Equals("51")) && (information_mission.mission_type.Contains("CH")))
                    {
                        int total_requiresign = sign.get_mission_information(information_mission.m_id).require_sign.Count;
                        int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission.m_id))); /// 過濾剩下需要多少簽
                        string current_requiresign = (total_requiresign - remains_requiresign).ToString();
                        this.BeginInvoke(new UpdateUI(_UpdateUI_CreatedListbox), new object[] { string.Format("{0}./ 案件代號：{1}  ({2}/{3})   {4}", information_mission.m_id, information_mission.binding_project_id, current_requiresign, total_requiresign, "已建帳") });
                        this.BeginInvoke(new Action<string>(_UpdateUI_ProgressBar), string.Format("{0}", 1.ToString()));
                    }
                }
            }
            this.BeginInvoke(new Action<string>(_UpdateUI_TabControlPage), string.Format("{0}", "on"));
            //ConsoleLog_Texbox.Text += String.Format("{0}\r\n", "已完成【已建帳之任務】資料載入");
            this.BeginInvoke(new Thread_Refresh_Listbox(Fillter_TestData_Listbox));
        }
        public void LoadSigningMission_Mission()
        {
            this.BeginInvoke(new UpdateUI(_Clear_CanCreateListbox), new object[] { string.Format("Refresh") });
            // Example
            Controller_Sign sign = Controller_Sign.getInstance();
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            List<Data_Set_Mission_Details> list_information_mission = sign_plugin.get_mission_information_all();
            this.BeginInvoke(new Action<string>(_UpdateUI_TabControlPage), string.Format("{0}", "off"));
            // Show Result


            foreach (Data_Set_Mission_Details information_mission in list_information_mission)
            {
                if (information_mission.company.Equals("LOANIT") && information_mission.mission_type.Contains("CH")) /// 過濾掉非LOANIT的CH任務
                {
                    if ((!information_mission.status_id.Equals("5") && !information_mission.status_id.Equals("3") && !information_mission.status_id.Equals("99") && !information_mission.status_id.Equals("51")) && (information_mission.mission_type.Contains("CH")))
                    {
                        int total_requiresign = sign.get_mission_information(information_mission.m_id).require_sign.Count;
                        int remains_requiresign = int.Parse(sign_plugin.filter_Mission_Done_RequireAmount(sign_plugin.get_mission_information(information_mission.m_id))); /// 過濾剩下需要多少簽
                        string current_requiresign = (total_requiresign - remains_requiresign).ToString();
                        this.BeginInvoke(new UpdateUI(_UpdateUI_SigningListbox), new object[] { string.Format("{0}./ 案件代號：{1}  ({2}/{3})   {4}", information_mission.m_id, information_mission.binding_project_id, current_requiresign, total_requiresign, "簽核中") });
                        this.BeginInvoke(new Action<string>(_UpdateUI_ProgressBar), string.Format("{0}", 1.ToString()));
                    }
                }
            }
            this.BeginInvoke(new Action<string>(_UpdateUI_TabControlPage), string.Format("{0}", "on"));
            //ConsoleLog_Texbox.Text += String.Format("{0}\r\n", "已完成【簽核中之任務】資料載入");
            this.BeginInvoke(new Thread_Refresh_Listbox(Fillter_TestData_Listbox));
        }
        public void LoadMissionSummary(Data_Set_Mission_Details mission_data)
        {
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();

            Mission_BindingProjectID_Label.Text = mission_data.binding_project_id;
            Mission_CreateTime_Label.Text = mission_data.create_time;
            Mission_Status_Label.Text = sign_plugin.get_missionStatusDescrption(mission_data.status_id);
            Mission_CurrentAmount_Label.Text = changeToMoneyType(decimal.Parse(sign_plugin.get_mission_curent_amount(mission_data)));
            Mission_CurrentProcessRatio_Label.Text = sign_plugin.get_mission_curent_processratio(mission_data);
            Mission_CurrentRate_Label.Text = sign_plugin.get_mission_curent_rate(mission_data);
            Mission_PrePayPeriod_Label.Text = sign_plugin.get_mission_PrepaidPeriod(mission_data).ToString();

        }
        /// <summary>
        /// 暫存用(一次性，同步簽核後即可)，還尚未把篩選規則實作。
        /// </summary>
        public void LoadCreatedAccount()
        {
            Exising_Account_Listbox.Items.Clear();
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            List<Data_Set_Mission_Details> all_mission = sign_plugin.get_mission_information_all();
            List<string> NotYet_AsnycSignDB_Account = new List<string>();
            /// 篩選掉已建帳的
            foreach (string created_account_c_keys in GobalPipeData.result_feedback)
            {
                bool isCreated = false;
                foreach (Data_Set_Mission_Details mission_details in all_mission)
                {
                    if (created_account_c_keys.Equals(mission_details.binding_project_id) && mission_details.status_id.Equals("51"))
                    {
                        isCreated = true;
                    }
                }
                if (!isCreated)
                {
                    NotYet_AsnycSignDB_Account.Add(created_account_c_keys);
                }
            }


            try
            {
                foreach (string item in NotYet_AsnycSignDB_Account)
                {
                    Exising_Account_Listbox.Items.Add(item);
                }
            }
            catch (Exception ex)
            {

                ConsoleLog_Texbox.Text += String.Format("[簽核系統] 發生錯誤。詳細錯誤資訊：{0}，請聯繫【研發中心-郁宸】", ex.Message);
            }
        }
        private void AccountSystem_Signature_Load(object sender, EventArgs e)
        {
            InitPipe();
            LoadCanCreateAccount_Mission();
            this.ControlBox = false;
            ConsoleLog_Texbox.Text += String.Format("[簽核系統]已完成資料載入。\r\n");

            /// 先隱藏掉，確認這樣沒問題一段時間後邊可刪掉這個物件
            this.ALL_Mission_TabPage.Parent = null;
        }
        public void UpdateMissionStatusVision(string pass_level, List<Data_Set_Mission_require> list_require_sign)
        {
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

                        Status_1_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                        Status_GA_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                        Status_2_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                        Status_3_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                        Status_4_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        break;
                    case "2":
                        Position_Level1.Text = "X"; Position_Level1.ForeColor = Color.Black;
                        Position_GA.Text = "X"; Position_GA.ForeColor = Color.Black;
                        Position_Level2.Text = "X"; Position_Level2.ForeColor = Color.Black;
                        Position_Level3.Text = "●"; Position_Level3.ForeColor = Color.Lime;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;

                        Status_1_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                        Status_GA_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                        Status_2_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                        Status_3_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        Status_4_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        break;
                    case "3":
                        Position_Level1.Text = "X"; Position_Level1.ForeColor = Color.Black;
                        Position_GA.Text = "X"; Position_GA.ForeColor = Color.Black;
                        Position_Level2.Text = "●"; Position_Level2.ForeColor = Color.Lime;
                        Position_Level3.Text = "●"; Position_Level3.ForeColor = Color.Lime;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;

                        Status_1_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                        Status_GA_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                        Status_2_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        Status_3_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        Status_4_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        break;
                    case "4":
                        Position_Level1.Text = "●"; Position_Level1.ForeColor = Color.Lime;
                        Position_Level2.Text = "●"; Position_Level2.ForeColor = Color.Lime;
                        Position_Level3.Text = "●"; Position_Level3.ForeColor = Color.Lime;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;

                        bool GA_Rule_Switch = false;
                        if (GA_Rule_Switch)
                        {
                            Position_Level1.Text = "X"; Position_Level1.ForeColor = Color.Black;
                            Position_GA.Text = "●"; Position_GA.ForeColor = Color.Lime;
                            Status_1_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                            Status_GA_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        }

                        Status_1_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        Status_2_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        Status_3_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        Status_4_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        break;
                    case "5":
                        Position_Level1.Text = "●"; Position_Level1.ForeColor = Color.Lime;
                        Position_GA.Text = "●"; Position_GA.ForeColor = Color.Lime;
                        Position_Level2.Text = "●"; Position_Level2.ForeColor = Color.Lime;
                        Position_Level3.Text = "●"; Position_Level3.ForeColor = Color.Lime;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;

                        Status_1_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        Status_GA_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        Status_2_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        Status_3_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        Status_4_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
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

                        Status_1_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/needsign.png";
                        Status_GA_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        Status_2_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        Status_3_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        Status_4_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        break;
                    case "2":
                        Position_Level1.Text = "○"; Position_Level1.ForeColor = Color.Black;
                        Position_GA.Text = "○"; Position_GA.ForeColor = Color.Black;
                        Position_Level2.Text = "●"; Position_Level2.ForeColor = Color.DarkOrange;
                        Position_Level3.Text = "●"; Position_Level3.ForeColor = Color.Lime;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;

                        Status_1_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/notyet_pass.png";
                        Status_GA_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/notyet_pass.png";
                        Status_2_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/needsign.png";
                        Status_3_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        Status_4_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        break;
                    case "3":
                        Position_Level1.Text = "○"; Position_Level1.ForeColor = Color.Black;
                        Position_GA.Text = "○"; Position_GA.ForeColor = Color.Black;
                        Position_Level2.Text = "○"; Position_Level2.ForeColor = Color.Black;
                        Position_Level3.Text = "●"; Position_Level3.ForeColor = Color.DarkOrange;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;

                        Status_1_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/notyet_pass.png";
                        Status_GA_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/notyet_pass.png";
                        Status_2_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/notyet_pass.png";
                        Status_3_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/needsign.png";
                        Status_4_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        break;
                    case "4":
                        Position_Level1.Text = "○"; Position_Level1.ForeColor = Color.Black;
                        Position_GA.Text = "○"; Position_GA.ForeColor = Color.Black;
                        Position_Level2.Text = "○"; Position_Level2.ForeColor = Color.Black;
                        Position_Level3.Text = "○"; Position_Level3.ForeColor = Color.Black;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.DarkOrange;

                        Status_1_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/notyet_pass.png";
                        Status_GA_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/notyet_pass.png";
                        Status_2_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/notyet_pass.png";
                        Status_3_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/notyet_pass.png";
                        Status_4_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/needsign.png";
                        break;
                    case "1.5":
                        Position_Level1.Text = "○"; Position_Level1.ForeColor = Color.Black;
                        Position_GA.Text = "●"; Position_GA.ForeColor = Color.DarkOrange;
                        Position_Level2.Text = "●"; Position_Level2.ForeColor = Color.Lime;
                        Position_Level3.Text = "●"; Position_Level3.ForeColor = Color.Lime;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;

                        Status_1_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/notyet_pass.png";
                        Status_GA_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/needsign.png";
                        Status_2_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        Status_3_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        Status_4_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        break;
                    case "2.5":
                        Position_Level1.Text = "X"; Position_Level1.ForeColor = Color.Black;
                        Position_GA.Text = "●"; Position_GA.ForeColor = Color.DarkOrange;
                        Position_Level2.Text = "○"; Position_Level2.ForeColor = Color.Black;
                        Position_Level3.Text = "●"; Position_Level3.ForeColor = Color.Lime;
                        Position_Level4.Text = "●"; Position_Level4.ForeColor = Color.Lime;



                        Status_1_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                        Status_GA_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/needsign.png";
                        Status_2_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/notyet_pass.png";
                        Status_3_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
                        Status_4_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/pass.png";
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

            switch (list_require_sign.Count.ToString()) //這裡指數量
            {
                case "5":
                    break;
                case "4":
                    bool GA_Rule_Switch = false;
                    if (GA_Rule_Switch)
                    {
                        Position_Level1.Text = "🚫"; Position_Level1.ForeColor = Color.Black;
                        Status_1_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                    }

                    break;
                case "3":
                    Position_Level1.Text = "🚫"; Position_Level1.ForeColor = Color.Black;
                    Position_GA.Text = "🚫"; Position_Level1.ForeColor = Color.Black;

                    Status_1_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                    Status_GA_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                    break;
                case "2":
                    Position_Level1.Text = "🚫"; Position_Level1.ForeColor = Color.Black;
                    Position_GA.Text = "🚫"; Position_Level1.ForeColor = Color.Black;
                    Position_Level2.Text = "🚫"; Position_Level1.ForeColor = Color.Black;

                    Status_1_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                    Status_GA_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                    Status_2_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                    break;
                case "1":
                    Position_Level1.Text = "🚫"; Position_Level1.ForeColor = Color.Black;
                    Position_GA.Text = "🚫"; Position_Level1.ForeColor = Color.Black;
                    Position_Level2.Text = "🚫"; Position_Level1.ForeColor = Color.Black;
                    Position_Level3.Text = "🚫"; Position_Level1.ForeColor = Color.Black;

                    Status_1_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                    Status_GA_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                    Status_2_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                    Status_3_PictureBox.ImageLocation = "../SubSys_Zoo/SubSys_AccountSignSystem/img/ban.png";
                    break;
                default:
                    break;
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
        #endregion

        #region UI_EVENT
        private void Signature_Function_TabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            string TabControllPage = Signature_Function_TabControl.SelectedTab.Text.ToString();
            button1.Visible = false;
            try
            {
                switch (TabControllPage)
                {
                    case "可建帳":
                        LoadThread(TabControllPage);
                        //LoadCanCreateAccount_Mission();
                        break;
                    case "已建帳":
                        LoadThread(TabControllPage);
                        //LoadCreatedAccount_Mission();
                        break;
                    case "簽核中":
                        LoadThread(TabControllPage);
                        //LoadSigningMission_Mission();
                        break;
                    case "既有帳務資料(需同步簽核)":
                        LoadCreatedAccount();
                        button1.Visible = true;
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
        private void Signature_Function_TabControl_Selecting()
        {
            string TabControllPage = Signature_Function_TabControl.SelectedTab.Text.ToString();
            button1.Visible = false;
            try
            {
                switch (TabControllPage)
                {
                    case "可建帳":
                        LoadThread(TabControllPage);
                        //LoadCanCreateAccount_Mission();
                        break;
                    case "已建帳":
                        LoadThread(TabControllPage);
                        //LoadCreatedAccount_Mission();
                        break;
                    case "簽核中":
                        LoadThread(TabControllPage);
                        //LoadSigningMission_Mission();
                        break;
                    case "既有帳務資料(需同步簽核)":
                        LoadCreatedAccount();
                        button1.Visible = true;
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
        private void CanCreateAccount_Listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();

            if (CanCreateAccount_Listbox.SelectedItem != null)
            {
                string[] Mission_Info_Arr = CanCreateAccount_Listbox.SelectedItem.ToString().Split('.');
                Data_Set_Mission_Details mission_data = sign_plugin.get_mission_information(Mission_Info_Arr[0]);

                /// 更新簽核簡介
                LoadMissionSummary(mission_data);

                /// 更新圖示化部分
                string HigestSigned_Level = sign_plugin.get_mission_signing_status_forVision(mission_data);
                mission_data = sign_plugin.get_mission_information(mission_data.m_id);
                UpdateMissionStatusVision(HigestSigned_Level, mission_data.require_sign);

                {

                    /// Send Pipe_Gateway
                    DataSet_SignToAccountSys SignToAccountSys = new DataSet_SignToAccountSys();
                    SignToAccountSys.M_id = mission_data.m_id;
                    SignToAccountSys.Loan_Amount = sign_plugin.get_mission_curent_amount(mission_data);
                    SignToAccountSys.C_Key = mission_data.binding_project_id;
                    SignToAccountSys.Monthly_Interest_Rate = sign_plugin.get_mission_curent_rate(mission_data);
                    SignToAccountSys.PrepaidPeriod = sign_plugin.get_mission_PrepaidPeriod(mission_data);
                    /// PipeData
                    PipeData send_data = new PipeData();
                    send_data.isSuccess = true;
                    send_data.type = "m$CH"; // Send 2.
                    send_data.result_feedback = new List<string>();
                    send_data.result_feedback.Add(JsonConvert.SerializeObject(SignToAccountSys));

                    pipecontroller.sendMessage(send_data);
                }

                //ConsoleLog_Texbox.Text += String.Format("[簽核系統] 已選擇 {0} 專案\r\n", mission_data.binding_project_id);

            }
        }
        private void CreatedAccount_Listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            if (CreatedAccount_Listbox.SelectedItem != null)
            {
                string[] Mission_Info_Arr = CreatedAccount_Listbox.SelectedItem.ToString().Split('.');
                Data_Set_Mission_Details mission_data = sign_plugin.get_mission_information(Mission_Info_Arr[0]);

                /// 更新簽核簡介
                LoadMissionSummary(mission_data);

                /// 更新圖示化部分
                string HigestSigned_Level = sign_plugin.get_mission_signing_status_forVision(mission_data);
                mission_data = sign_plugin.get_mission_information(mission_data.m_id); /// 取level的部分，會執行過濾，因此需要重新載入資料。
                UpdateMissionStatusVision(HigestSigned_Level, mission_data.require_sign);

                //ConsoleLog_Texbox.Text += String.Format("[簽核系統] 已選擇 {0} 專案\r\n", mission_data.binding_project_id);
            }
        }
        private void Signing_Mission_Listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();

            if (Signing_Mission_Listbox.SelectedItem != null)
            {
                string[] Mission_Info_Arr = Signing_Mission_Listbox.SelectedItem.ToString().Split('.');
                Data_Set_Mission_Details mission_data = sign_plugin.get_mission_information(Mission_Info_Arr[0]);

                /// 更新簽核簡介
                LoadMissionSummary(mission_data);

                /// 更新圖示化部分
                string HigestSigned_Level = sign_plugin.get_mission_signing_status_forVision(mission_data);
                mission_data = sign_plugin.get_mission_information(mission_data.m_id); /// 取level的部分，會執行過濾，因此需要重新載入資料。
                UpdateMissionStatusVision(HigestSigned_Level, mission_data.require_sign);

                //ConsoleLog_Texbox.Text += String.Format("[簽核系統] 已選擇 {0} 專案\r\n", mission_data.binding_project_id);
            }
        }
        private void All_Mission_Listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();

            if (All_Mission_Listbox.SelectedItem != null)
            {
                string[] Mission_Info_Arr = All_Mission_Listbox.SelectedItem.ToString().Split('.');
                Data_Set_Mission_Details mission_data = sign_plugin.get_mission_information(Mission_Info_Arr[0]);


                /// 更新簽核簡介
                LoadMissionSummary(mission_data);

                /// 更新圖示化部分
                string HigestSigned_Level = sign_plugin.get_mission_signing_status_forVision(mission_data);
                mission_data = sign_plugin.get_mission_information(mission_data.m_id); /// 取level的部分，會執行過濾，因此需要重新載入資料。
                UpdateMissionStatusVision(HigestSigned_Level, mission_data.require_sign);

                //ConsoleLog_Texbox.Text += String.Format("[簽核系統] 已選擇 {0} 專案\r\n", mission_data.binding_project_id);
            }
        }
        private void UpdateListbox_Button_Click(object sender, EventArgs e)
        {

            Signature_Function_TabControl_Selecting();
            ConsoleLog_Texbox.Text = String.Format("[簽核系統]已更新所有清單列表\r\n");
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


        #endregion





        private void button1_Click(object sender, EventArgs e)
        {
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            foreach (string item in Exising_Account_Listbox.Items)
            {
                sign_plugin.insert_createdaccount(item);
            }
            Signature_Function_TabControl_Selecting();

        }

        private void AccountSystem_Signature_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

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


    }
}
