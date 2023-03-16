using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using PTMB_Signature.Model.DAO;
using PTMB_Signature.Model.Data_Set;
using PTMB_Signature.Model;
using PTMB_Signature.Model.Plugin;
using PTMB_Signature_API.Data_Set;
using System.Threading;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using PTMB_Signature.Implement_Form.InsertDataForm;

namespace PTMB_Signature.Implement_Form.Dev
{
    public partial class EmployeeConfiglModify : MaterialForm
    {
        public EmployeeConfiglModify()
        {
            InitializeComponent();
            InitialMaterialColor();
        }
        private void InitialMaterialColor()
        {
            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                (Primary)YC_MaterialColorPlugin.YCMaterialColorPlugin.Primary.LINE_,
                (Primary)YC_MaterialColorPlugin.YCMaterialColorPlugin.Primary.LINE_,
                (Primary)YC_MaterialColorPlugin.YCMaterialColorPlugin.Primary.LINE_,
                (Accent)YC_MaterialColorPlugin.YCMaterialColorPlugin.Accent.RED,
                TextShade.WHITE
                );
        }

        private void Function_TabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            string TabControllPage = Function_TabControl.SelectedTab.Text.ToString();
            Thread_TabControlLoad(TabControllPage);
        }








        #region 委派
        delegate void RefreshCompoment();
        delegate void UpdateCompoment(object sender, UpdateCompomentOption option);
        delegate void UpdateCompomentItems(object sender, string Data);
        delegate void UpdateProgressCompoment(object sender, int Data, UpdateProgressBarOption option);
        delegate string GetCompomentText(object Sender);

        #region TabPage_Controller(TabContro)
        private void Thread_TabControlLoad(string tabPage)
        {
            switch (tabPage)
            {
                case "逐級層級":
                    Thread Thread_LoadSignEmployeeLevelData = new Thread(LoadSignEmployeeLevelData);
                    Thread_LoadSignEmployeeLevelData.Start();
                    break;

                default:
                    break;
            }
        }
        #endregion





        #region 逐級層級_


        public List<Data_Set_Employee_Account_LOANIT_ForBackStage> ModifyRecord_List = new List<Data_Set_Employee_Account_LOANIT_ForBackStage>();
        public List<Data_Set_Employee_Account_LOANIT_ForBackStage> MissionEmployeeDataList { get; set; }

        #region TabePage_Event        
        private void LoadSignEmployeeLevelData()
        {
            MissionEmployeeDataList = new List<Data_Set_Employee_Account_LOANIT_ForBackStage>();

            LOANIT_SQL_Plugin sql_plugin = new LOANIT_SQL_Plugin();
            LOANIT_DAO_Plugin dao_plugin = LOANIT_DAO_Plugin.getInstance();
            Data_Set_DAO_Data get_mission_employee_level_result = sql_plugin.get_mission_employee_data_loanit_ForBackStage(Sql_Action_Category_Option.GET, Model.Sql_Action_Option.GET_MISSION_EMPLOYEE_LEVEL);
            MissionEmployeeDataList = dao_plugin.set_mission_employee_level_loanit_ForBackStage(get_mission_employee_level_result);
            Initial_ModifyData();


            Thread Thread_UpdateCombobox = new Thread(() => Function_UpdateCombobox(UpdateCompomentOption.LOAD_MISSIONTYPE_DATA_COMBOBOX, MissionType_Combobox));
            Thread_UpdateCombobox.Start();
        }
        private void MissionType_Combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitiProgressBar();
            Function_UpdateCombobox(UpdateCompomentOption.LOAD_EMPLOYEELEVEL_DATA_COMBOBOX, sender);
        }
        private void MissionLevel_Combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitiProgressBar();
            Function_UpdateCombobox(UpdateCompomentOption.LOAD_PERSON_DATA_COMBOBOX, sender);

        }
        private void Person_Combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitiProgressBar();
            Function_UpdateCombobox(UpdateCompomentOption.LOAD_NEW_EMPLOYEELEVEL_COMBOBOX, sender);
        }
        #endregion
        private void InitiProgressBar()
        {
            UI_ProgressBar1.Value = 0;
        }


        #region UIEvents
        private void UpdateComboboxCompomentData(object sender, UpdateCompomentOption option)
        {
            string MissionType_Combobox_Select_Item = "";
            string EmployeeLevel_Combobox_Select_Item = "";

            switch (option)
            {
                case UpdateCompomentOption.LOAD_MISSIONTYPE_DATA_COMBOBOX:

                    this.BeginInvoke(new Action<object>(ClearCombobox), sender);
                    foreach (Data_Set_Employee_Account_LOANIT_ForBackStage item in MissionEmployeeDataList)
                    {
                        if (MissionType_Combobox.Items.IndexOf(item.mission_type) == -1)
                        {
                            this.Invoke(new UpdateCompomentItems(UpdateComboboxItem), this.MissionType_Combobox, item.mission_type);
                        }
                        this.BeginInvoke(new UpdateProgressCompoment(UpdateProgressBar), UI_ProgressBar1, 1, UpdateProgressBarOption.UPDATE_VALUE);
                    }

                    break;

                case UpdateCompomentOption.LOAD_EMPLOYEELEVEL_DATA_COMBOBOX:
                    this.BeginInvoke(new Action<object>(ClearCombobox), EmployeeLevel_Combobox);


                    MissionType_Combobox_Select_Item = this.Invoke(new GetCompomentText(GetComboboxCompomentText), MissionType_Combobox).ToString();

                    foreach (Data_Set_Employee_Account_LOANIT_ForBackStage item in MissionEmployeeDataList)
                    {
                        if (MissionType_Combobox_Select_Item.Equals(item.mission_type))
                        {
                            this.BeginInvoke(new UpdateCompomentItems(UpdateComboboxItem), this.EmployeeLevel_Combobox, item.employee_level);
                        }
                        this.BeginInvoke(new UpdateProgressCompoment(UpdateProgressBar), UI_ProgressBar1, 1, UpdateProgressBarOption.UPDATE_VALUE);
                    }

                    break;

                case UpdateCompomentOption.LOAD_PERSON_DATA_COMBOBOX:

                    this.BeginInvoke(new Action<object>(ClearCombobox), Person_Combobox);
                    MissionType_Combobox_Select_Item = this.Invoke(new GetCompomentText(GetComboboxCompomentText), MissionType_Combobox).ToString();
                    EmployeeLevel_Combobox_Select_Item = this.Invoke(new GetCompomentText(GetComboboxCompomentText), EmployeeLevel_Combobox).ToString();

                    foreach (Data_Set_Employee_Account_LOANIT_ForBackStage item in MissionEmployeeDataList)
                    {
                        if (MissionType_Combobox_Select_Item.Equals(item.mission_type) && EmployeeLevel_Combobox_Select_Item.Equals(item.employee_level))
                        {
                            foreach (MainSysAccount Account_List in item.AccountList)
                            {
                                this.BeginInvoke(new UpdateCompomentItems(UpdateComboboxItem), this.Person_Combobox, Account_List.account);
                            }
                        }
                        this.BeginInvoke(new UpdateProgressCompoment(UpdateProgressBar), UI_ProgressBar1, 1, UpdateProgressBarOption.UPDATE_VALUE);
                    }
                    break;
                case UpdateCompomentOption.LOAD_NEW_EMPLOYEELEVEL_COMBOBOX:

                    this.BeginInvoke(new Action<object>(ClearCombobox), NewEmployeeLevel_Combobox);
                    MissionType_Combobox_Select_Item = this.Invoke(new GetCompomentText(GetComboboxCompomentText), MissionType_Combobox).ToString();

                    foreach (Data_Set_Employee_Account_LOANIT_ForBackStage item in MissionEmployeeDataList)
                    {
                        if (MissionType_Combobox_Select_Item.Equals(item.mission_type))
                        {
                            if (NewEmployeeLevel_Combobox.Items.IndexOf(item.employee_level) == -1)
                            {
                                if (!item.employee_level.Contains(".5")) /// 財務長特殊邏輯實作才會使用
                                {
                                    this.BeginInvoke(new UpdateCompomentItems(UpdateComboboxItem), this.NewEmployeeLevel_Combobox, item.employee_level);
                                }
                            }
                        }
                        this.BeginInvoke(new UpdateProgressCompoment(UpdateProgressBar), UI_ProgressBar1, 1, UpdateProgressBarOption.UPDATE_VALUE);
                    }
                    this.BeginInvoke(new UpdateCompomentItems(UpdateComboboxItem), this.NewEmployeeLevel_Combobox, "無權限");
                    break;
                default:
                    break;
            }
        }
        private void UpdateProgressBar(object sender, int Value, UpdateProgressBarOption option)
        {
            System.Windows.Forms.ProgressBar progressBar = sender as System.Windows.Forms.ProgressBar;
            switch (option)
            {
                case UpdateProgressBarOption.SET_MAXIMUM:
                    this.BeginInvoke(new Action<object, string>(UpdateProgressBarMax), progressBar, MissionEmployeeDataList.Count.ToString());
                    break;
                case UpdateProgressBarOption.UPDATE_VALUE:
                    this.BeginInvoke(new Action<object, string>(UpdateProgressBarValue), progressBar, Value.ToString());
                    break;
                default:
                    break;
            }
        }

        private void UpdateComboboxItem(object sender, string Data)
        {
            MaterialSkin.Controls.MaterialComboBox combobox = sender as MaterialSkin.Controls.MaterialComboBox;
            combobox.Items.Add(Data);
        }
        private void UpdateProgressBarValue(object sender, string Value)
        {
            System.Windows.Forms.ProgressBar progressbar = sender as System.Windows.Forms.ProgressBar;
            try
            {
                progressbar.Value += int.Parse(Value);
                progressbar.Text = Math.Round(Convert.ToDecimal(UI_ProgressBar1.Value) / Convert.ToDecimal(UI_ProgressBar1.Maximum) * 100, 2).ToString() + "%";
            }
            catch (Exception)
            {
                progressbar.Value = UI_ProgressBar1.Maximum;
            }
            progressbar.Update();
        }
        private void UpdateProgressBarMax(object sender, string Value)
        {
            UI_ProgressBar1.Maximum = int.Parse(Value);
            UI_ProgressBar1.Value = 0;
            UI_ProgressBar1.Update();

        }
        private void ClearCombobox(object sender)
        {
            MaterialSkin.Controls.MaterialComboBox combobox = sender as MaterialSkin.Controls.MaterialComboBox;
            combobox.Items.Clear();
        }
        private string GetComboboxCompomentText(object sender)
        {
            MaterialSkin.Controls.MaterialComboBox combobox = sender as MaterialSkin.Controls.MaterialComboBox;
            return combobox.Text;
        }
        private void LockComboboxCompomentFunction(object sender, UpdateCompomentOption option)
        {
            MaterialSkin.Controls.MaterialComboBox combobox = sender as MaterialSkin.Controls.MaterialComboBox;
            switch (option)
            {
                case UpdateCompomentOption.LOCK_COMPOMENT:
                    combobox.Enabled = false;
                    break;
                case UpdateCompomentOption.UNLOCK_COMPOMENT:
                    combobox.Enabled = true;
                    break;
                default:
                    break;
            }
        }

        #endregion


        #region UI_Thread
        Thread Thread_UpdateCombobox;

        private void Function_UpdateCombobox(UpdateCompomentOption option, object sender)
        {
            this.Invoke(new UpdateCompoment(LockComboboxCompomentFunction), sender, UpdateCompomentOption.LOCK_COMPOMENT);
            switch (option)
            {
                case UpdateCompomentOption.LOAD_MISSIONTYPE_DATA_COMBOBOX:
                    this.BeginInvoke(new UpdateProgressCompoment(UpdateProgressBar), UI_ProgressBar1, MissionEmployeeDataList.Count, UpdateProgressBarOption.SET_MAXIMUM);
                    Thread_UpdateCombobox = new Thread(() => UpdateComboboxCompomentData(this.MissionType_Combobox, UpdateCompomentOption.LOAD_MISSIONTYPE_DATA_COMBOBOX));
                    break;
                case UpdateCompomentOption.LOAD_EMPLOYEELEVEL_DATA_COMBOBOX:
                    this.BeginInvoke(new UpdateProgressCompoment(UpdateProgressBar), UI_ProgressBar1, MissionEmployeeDataList.Count, UpdateProgressBarOption.SET_MAXIMUM);
                    Thread_UpdateCombobox = new Thread(() => UpdateComboboxCompomentData(this.NewEmployeeLevel_Combobox, UpdateCompomentOption.LOAD_EMPLOYEELEVEL_DATA_COMBOBOX));
                    break;
                case UpdateCompomentOption.LOAD_PERSON_DATA_COMBOBOX:
                    this.BeginInvoke(new UpdateProgressCompoment(UpdateProgressBar), UI_ProgressBar1, MissionEmployeeDataList.Count, UpdateProgressBarOption.SET_MAXIMUM);
                    Thread_UpdateCombobox = new Thread(() => UpdateComboboxCompomentData(this.Person_Combobox, UpdateCompomentOption.LOAD_PERSON_DATA_COMBOBOX));
                    break;
                case UpdateCompomentOption.LOAD_NEW_EMPLOYEELEVEL_COMBOBOX:
                    this.BeginInvoke(new UpdateProgressCompoment(UpdateProgressBar), UI_ProgressBar1, MissionEmployeeDataList.Count, UpdateProgressBarOption.SET_MAXIMUM);
                    Thread_UpdateCombobox = new Thread(() => UpdateComboboxCompomentData(this.NewEmployeeLevel_Combobox, UpdateCompomentOption.LOAD_NEW_EMPLOYEELEVEL_COMBOBOX));
                    break;
                default:
                    break;
            }
            Thread_UpdateCombobox.Start();
            this.Invoke(new UpdateCompoment(LockComboboxCompomentFunction), sender, UpdateCompomentOption.UNLOCK_COMPOMENT);

        }


        #endregion


        private void AddUser_Button_Click(object sender, EventArgs e)
        {
            NewUser_Form newUser = new NewUser_Form();
            var result = newUser.ShowDialog();
            if (result == DialogResult.OK)
            {
                Person_Combobox.Items.Add(newUser.ResponeData);

                foreach (Data_Set_Employee_Account_LOANIT_ForBackStage item in MissionEmployeeDataList)
                {
                    if (item.mission_type.Equals(MissionType_Combobox.Text))
                    {
                        if (item.employee_level.Equals(EmployeeLevel_Combobox.Text))
                        {
                            MainSysAccount NewAccount = new MainSysAccount();
                            NewAccount.account = newUser.ResponeData;
                            item.AccountList.Add(NewAccount);
                            Add_MissionEmployee_ModifyRecordToList(item, NewAccount, "新增");
                        }
                    }
                }
            }

        }
        private void DeleteUser_Button_Click(object sender, EventArgs e)
        {


            foreach (Data_Set_Employee_Account_LOANIT_ForBackStage item in MissionEmployeeDataList)
            {
                if (item.mission_type.Equals(MissionType_Combobox.Text))
                {
                    if (item.employee_level.Equals(EmployeeLevel_Combobox.Text))
                    {
                        MainSysAccount RemoveAccount = new MainSysAccount();
                        RemoveAccount.account = Person_Combobox.Text;
                        //item.AccountList.Remove(RemoveAccount);
                        for (int i = 0; i <= item.AccountList.Count -1; i++)
                        {
                            if (item.AccountList[i].account.Equals(RemoveAccount.account))
                            {
                                item.AccountList.RemoveAt(i);
                                Add_MissionEmployee_ModifyRecordToList(item, RemoveAccount, "移除");
                            }
                        }


                        Person_Combobox.Items.Remove(Person_Combobox.Text);
                    }
                }
            }

        }
        private void materialButton1_Click(object sender, EventArgs e)
        {
            string Record = "請確認是否做以下修改 \r\n";
            foreach (Data_Set_Employee_Account_LOANIT_ForBackStage ModifyRecord in ModifyRecord_List)
            {
                if(ModifyRecord.AccountList.Count > 0)
                {
                    Record += string.Format("任務類別：{0} 層級：{1} \r\n", ModifyRecord.mission_type, ModifyRecord.employee_level);
                    foreach (MainSysAccount Account in ModifyRecord.AccountList)
                    {
                        Record += string.Format("    {0}\r\n", Account.account.Replace("|"," "));
                    }
                
                }
            }


            var result =  MaterialMessageBox.Show(this,  Record, "通知", MessageBoxButtons.YesNo, true,FlexibleMaterialForm.ButtonsPosition.Fill);

            if (result == DialogResult.Yes)
            {
                MessageBox.Show("Yes");
                LOANIT_BACKSTAGE_CONTROLLER BackStage_Controller = LOANIT_BACKSTAGE_CONTROLLER.getInstnace();
                MaterialMessageBox.Show(BackStage_Controller.UpdateEmployeeAccountData(MissionEmployeeDataList).feedback_result,"結果通知");
            }
            else
            {
                MessageBox.Show("No");
            }

        
        }

        private void Add_MissionEmployee_ModifyRecordToList(Data_Set_Employee_Account_LOANIT_ForBackStage RecordData, MainSysAccount RecordAccount, string ModifyStatus)
        {
            MainSysAccount Temp_RecordAccount = JsonConvert.DeserializeObject<MainSysAccount>(JsonConvert.SerializeObject(RecordAccount));
            foreach (Data_Set_Employee_Account_LOANIT_ForBackStage item in ModifyRecord_List)
            {
                if (item.mission_type.Equals(RecordData.mission_type))
                {
                    if (item.employee_level.Equals(RecordData.employee_level))
                    {
                        Temp_RecordAccount.account += string.Format("  狀態為  ========>  {0} |||||||||||||||||||", ModifyStatus);
                        item.AccountList.Add(RecordAccount);
                    }
                }
            }
        }
        private void Initial_ModifyData()
        {
            ModifyRecord_List = new List<Data_Set_Employee_Account_LOANIT_ForBackStage>();
            List<Data_Set_Employee_Account_LOANIT_ForBackStage> Temp_List_Mission_Employee = new List<Data_Set_Employee_Account_LOANIT_ForBackStage>();
            Temp_List_Mission_Employee = JsonConvert.DeserializeObject<List<Data_Set_Employee_Account_LOANIT_ForBackStage>>(JsonConvert.SerializeObject(MissionEmployeeDataList.ToList()));
            foreach (Data_Set_Employee_Account_LOANIT_ForBackStage item in Temp_List_Mission_Employee)
            {
                item.AccountList = new List<MainSysAccount>();
                ModifyRecord_List.Add(item);
            }
        }
        #endregion

        #endregion


    }




    public enum UpdateCompomentOption
    {
        LOAD_MISSIONTYPE_DATA_COMBOBOX,
        LOAD_EMPLOYEELEVEL_DATA_COMBOBOX,
        LOAD_PERSON_DATA_COMBOBOX,
        LOAD_NEW_EMPLOYEELEVEL_COMBOBOX,
        LOCK_COMPOMENT,
        UNLOCK_COMPOMENT
    }

    public enum UpdateProgressBarOption
    {
        SET_MAXIMUM,
        UPDATE_VALUE
    }
}
