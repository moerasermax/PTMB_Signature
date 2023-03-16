using PTMB_Signature.Implement_Form;
using PTMB_Signature_API.Data_Set;
using PTMB_Signature_API.Model.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PTMB_Signature.Model.Data_Set;
using PTMB_Signature_API.Informatio_Set;
using PTMB_Signature_API.Model.Implement;
using System.Text.RegularExpressions;
using PTMB_Signature_API.Data_Set.Login;
using PTMB_Signature.Model.DAO;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Security.Cryptography.X509Certificates;
using New_Customer_Submit.API.Controller;
using System.Reflection;
using PTMB_Signature.Implement_Risk;
using New_Customer_Submit.API.Model.Data_Set;
using Microsoft.FSharp.Core;

namespace PTMB_Signature.Model.Plugin
{
    /// <summary>
    /// 任何判斷 "temp_done" or "done" 都是因為【兄弟姊妹單】；正常狀況的一般單只有判別【pass】
    /// </summary>
    public class LOANIT_CONTROLLER_Plugin : Controller_Sign_Test_Abstract
    {
        Controller_Sign sign = Controller_Sign.getInstance();

        /// 增加效能演算法
        public List<Information_Mission> Temp_RequireSign_Mission { get; set; }
        public List<Information_Mission> Temp_RequireDone_Mission { get; set; }
        public List<Information_Mission> Temp_ALL_Mission { get; set; }
        public List<Information_Mission> Temp_Fail_Mission { get; set; }
        public List<CKeyAndName> Temp_CkeyAndNameData { get; set; }

        public bool get_ga_back_status(Data_Set_Mission_Details mission_data, Data_Set_Employee employee_data)
        {
            bool ga_record_exsting = false;
            employee_data.employee_level = get_employee_level(employee_data.e_id, mission_data.mission_type, mission_data.risk_value, mission_data);
            /// 如果判定 true 就代表，輪到財務長退回
            foreach (Data_Set_Mission_Collect_Signed collect_signature in mission_data.collect_signed)
            {

                if ((collect_signature.department == "GA") && (collect_signature.department == employee_data.department) && (employee_data.company.Equals("PTMB")) && (collect_signature.employee_level.Equals(employee_data.employee_level)))
                {
                    ga_record_exsting = true;
                    if (collect_signature.sign_status.Equals("pass") || collect_signature.sign_status.Equals("back"))
                    {
                        return false;
                    }
                }
            }

            /// 若財務長還沒簽過，不能將呈核給隱藏。
            if (!ga_record_exsting)
            {
                return false;
            }
            return true;
        }
        public string get_employee_level(string account, string mission_type, string risk_value, Data_Set_Mission_Details mission_data)
        {
            string result_level = "";
            LOANIT_CONTROLLER_Plugin sign_plus = LOANIT_CONTROLLER_Plugin.getInstance();
            Form_Sign form_sign = Form_Sign.getInstance();
            foreach (Data_Set_Mission_Employee_Level_LOANIT mission_employee_level_data in form_sign.data_set_mission_employee_level)
            {
                if (mission_employee_level_data.mission_type.Equals(mission_type))
                {
                    foreach (Data_Set_Employee_Account_LOANIT employee_account in mission_employee_level_data.account_list)
                    {
                        if (employee_account.account.Equals(account))
                        {
                            if (employee_account.risk != null)
                            {
                                if (employee_account.risk.Equals(risk_value))
                                {
                                    return mission_employee_level_data.employee_level;
                                }
                            }
                            else
                            {
                                result_level += mission_employee_level_data.employee_level + ",";
                            }
                        }
                    }
                }
            }
            if (!result_level.Equals(""))
            {
                /// 去計算已蒐集簽裡面，有幾個是同一個人簽的並且是pass，是的話就 -1 把最小的給去掉，因為是由小簽到大，即可找到目前他是第幾層級的簽
                string[] result_level_arr = Regex.Split(result_level, ",");
                if (result_level_arr.Length - 1 > 1)
                {
                    int count_sign = 0;
                    foreach (Data_Set_Mission_Collect_Signed collect_sign in mission_data.collect_signed)
                    {
                        //Console.WriteLine("Mission_ID------->" + mission_data.m_id);
                        //Console.WriteLine("A1------>" + collect_sign.public_key);
                        //Console.WriteLine("A2------>" + sign_plus.get_employee_information(account).publickey);
                        if (collect_sign.public_key.Equals(sign_plus.get_employee_information(account).publickey) && collect_sign.sign_status.Equals("pass"))
                        {
                            count_sign++;
                        }
                    }


                    if (!mission_data.status_id.Equals("3"))
                    {
                        if (result_level_arr.Length - 1 - 1 - count_sign < 0)
                        {
                            return result_level_arr[0];

                        }
                        else
                        {
                            return result_level_arr[result_level_arr.Length - 1 - 1 - count_sign]; /// 第1個-1是要把 "" 給去掉 第二個是算位置
                        }
                    }
                    else
                    {
                        return result_level_arr[result_level_arr.Length - 1 - 1]; /// 第1個-1是要把 "" 給去掉 第二個是算位置
                    }


                }
                else
                {
                    return result_level.Replace(",", "");
                }
            }
            else
            {
                return "此帳號在此案件中沒有對應的簽核層級，請聯絡【研發中心-郁宸】進行調整。";
            }
        }
        public List<Information_Mission> get_employee_miossion_information_requirment_loanit(string e_id)
        {
            Controller_Sign sign = Controller_Sign.getInstance();
            Data_Set_Excutre_Result result = CompareTokenvalid();
            if (Temp_RequireSign_Mission is null)
            {
                if (result.excute_result.isSuccesed)
                {

                    Data_Set_Employee employee_data = get_employee_information(e_id);
                    List<Data_Set_Mission_Details> list_mission_data = get_mission_information_all();

                    List<Information_Mission> list_information_mission = new List<Information_Mission>();
                    bool compare_company = false, compare_department = false, compare_emplyee_Level = false, compare_sign_status = false;

                    /// 過濾已達成的簽核
                    filter_Mission_Done_RequireAmount(list_mission_data);


                    foreach (Data_Set_Mission_Details mission_data in list_mission_data)
                    {
                        if (!sign.Existing_Fail_Signed(mission_data))
                        {
                            mission_data.require_sign.Sort((x, y) => -x.employee_level.CompareTo(y.employee_level));

                            foreach (Data_Set_Mission_require mission_require_signature in mission_data.require_sign.ToArray())
                            {
                                //if (employee_data.employee_level is null || employee_data.employee_level.Contains("【研發中心-郁宸】"))
                                { employee_data.employee_level = get_employee_level(employee_data.e_id, mission_data.mission_type, mission_data.risk_value, mission_data); }

                                if (employee_data.company.Equals(mission_require_signature.company))
                                {
                                    compare_company = true;
                                }
                                if (employee_data.department.Equals(mission_require_signature.department) || employee_data.department.ToUpper().Equals("ALL"))
                                {
                                    compare_department = true;
                                }
                                if (mission_require_signature.employee_level.Equals(employee_data.employee_level))
                                {
                                    compare_emplyee_Level = true;
                                }
                                if ((compare_company == true) && (compare_department == true) && (compare_emplyee_Level == true) && (!mission_data.status_id.Equals("99")))
                                {
                                    //Console.WriteLine("Mission_ID " + mission_data.m_id);

                                    /// 將簽核資訊與需求簽核資訊比對
                                    if (get_compare_level_sequence(employee_data, mission_data.collect_signed, mission_data.require_sign))  ///判斷是否輪到此職員
                                    {
                                        if (mission_data.require_sign.Count != 1)
                                        {
                                            Information_Mission prepare_add_mission = new Information_Mission();
                                            prepare_add_mission.mission_id = mission_data.m_id;
                                            prepare_add_mission.mission_status = mission_data.status_id;
                                            prepare_add_mission.binding_project_id = mission_data.binding_project_id;
                                            list_information_mission.Add(prepare_add_mission);
                                        }
                                    }
                                }
                                compare_company = false;
                                compare_department = false;
                                compare_emplyee_Level = false;
                            }
                        }
                    }
                    Temp_RequireSign_Mission = list_information_mission;
                    return Temp_RequireSign_Mission;
                }
                else
                {
                    throw new ArgumentException("請先登入");
                }
            }
            else
            {
                return Temp_RequireSign_Mission;
            }

        }
        public List<Information_Mission> get_employee_mission_information_requirement_done(string e_id)
        {
            Data_Set_Excutre_Result result = CompareTokenvalid();

            if (Temp_RequireDone_Mission is null)
            {
                if (result.excute_result.isSuccesed)
                {

                    Data_Set_Employee employee_data = get_employee_information(e_id);
                    List<Data_Set_Mission_Details> list_mission_data = get_mission_information_all();

                    List<Information_Mission> list_information_mission = new List<Information_Mission>();
                    bool compare_company = false, compare_department = false, compare_emplyee_Level = false, compare_sign_status = false;

                    /// 過濾已達成的簽核
                    filter_Mission_Done_RequireAmount(list_mission_data); // 過濾還需要多少簽核


                    foreach (Data_Set_Mission_Details mission_data in list_mission_data)
                    {
                        if (!Existing_Fail_Signed(mission_data))
                        {
                            mission_data.require_sign.Sort((x, y) => -x.employee_level.CompareTo(y.employee_level));

                            foreach (Data_Set_Mission_require mission_require_signature in mission_data.require_sign.ToArray())
                            {
                                //if (employee_data.employee_level is null) 
                                //{ 
                                //    employee_data.employee_level = get_employee_level(employee_data.e_id, mission_data.mission_type, mission_data.risk_value, mission_data); 
                                //}
                                employee_data.employee_level = get_employee_level(employee_data.e_id, mission_data.mission_type, mission_data.risk_value, mission_data);
                                if (employee_data.company.Equals(mission_require_signature.company))
                                {
                                    compare_company = true;
                                }
                                if (employee_data.department.Equals(mission_require_signature.department) || employee_data.department.ToUpper().Equals("ALL"))
                                {
                                    compare_department = true;
                                }
                                if (employee_data.employee_level.Equals(mission_require_signature.employee_level))
                                {
                                    compare_emplyee_Level = true;
                                }
                                if ((compare_company == true) && (compare_department == true) && (compare_emplyee_Level == true))
                                {
                                    /// 將簽核資訊與需求簽核資訊比對
                                    if (get_compare_level_sequence(employee_data, mission_data.collect_signed, mission_data.require_sign))  ///判斷是否輪到此職員
                                    {
                                        if (mission_data.require_sign.Count == 1)
                                        {
                                            Information_Mission prepare_add_mission = new Information_Mission();
                                            prepare_add_mission.mission_id = mission_data.m_id;
                                            prepare_add_mission.mission_status = mission_data.status_id;
                                            prepare_add_mission.binding_project_id = mission_data.binding_project_id;
                                            list_information_mission.Add(prepare_add_mission);
                                        }
                                    }
                                }
                                compare_company = false;
                                compare_department = false;
                                compare_emplyee_Level = false;
                            }
                        }
                    }
                    Temp_RequireDone_Mission = list_information_mission;
                    return Temp_RequireDone_Mission;
                }
                else
                {
                    throw new ArgumentException("請先登入");
                }
            }
            else
            {
                return Temp_RequireDone_Mission;
            }

        }

        public List<Information_Mission> get_employee_mission_information_all_loanit(string e_id)
        {
            Controller_Sign sign = Controller_Sign.getInstance();
            Data_Set_Excutre_Result result = CompareTokenvalid();

            if (Temp_ALL_Mission is null)
            {
                if (result.excute_result.isSuccesed)
                {

                    Data_Set_Employee employee_data = get_employee_information(e_id);
                    List<Data_Set_Mission_Details> list_mission_data = get_mission_information_all();

                    List<Information_Mission> list_information_mission = new List<Information_Mission>();
                    bool compare_company = false, compare_department = false, compare_emplyee_Level = false;

                    foreach (Data_Set_Mission_Details mission_data in list_mission_data)
                    {
                        foreach (Data_Set_Mission_require mission_require_signature in mission_data.require_sign)
                        {
                            { employee_data.employee_level = get_employee_level(employee_data.e_id, mission_data.mission_type, mission_data.risk_value, mission_data); }
                            if (employee_data.company.Equals(mission_require_signature.company))
                            {
                                compare_company = true;
                            }
                            if (employee_data.department.Equals(mission_require_signature.department) || employee_data.department.ToUpper().Equals("ALL"))
                            {
                                compare_department = true;
                            }
                            if (employee_data.employee_level.Equals(mission_require_signature.employee_level))
                            {
                                compare_emplyee_Level = true;
                            }
                            if ((compare_company == true) && (compare_department == true) && (compare_emplyee_Level == true))
                            {
                                Information_Mission prepare_add_mission = new Information_Mission();
                                prepare_add_mission.mission_id = mission_data.m_id;
                                prepare_add_mission.mission_status = mission_data.status_id;
                                prepare_add_mission.history = mission_data.history;
                                prepare_add_mission.binding_project_id = mission_data.binding_project_id;
                                list_information_mission.Add(prepare_add_mission);
                            }
                            compare_company = false;
                            compare_department = false;
                            compare_emplyee_Level = false;
                        }
                    }
                    Temp_ALL_Mission = list_information_mission;
                    return Temp_ALL_Mission;
                }
                else
                {
                    throw new ArgumentException("請先登入");
                }
            }
            else
            {
                return Temp_ALL_Mission;
            }


        }

        #region LOANIT專屬逐級-簽核功能
        public Data_Set_Excutre_Result excute_sign_back_loanit(Data_Set_Employee data_set_employee, string m_id, string remark ,string record_data)
        {
            bool compare_isclear = false;
            bool mission_fail = true;

            Controller_Sign sign = Controller_Sign.getInstance();
            /// 這邊邏輯是從現在簽的人 反推，所以跟pass的保存簽的人邏輯相反。
            Data_Set_Excutre_Result result = CompareTokenvalid();

            if (result.excute_result.isSuccesed)
            {
                float current_level;



                Data_Set_Mission_Details mission_data = get_mission_information(m_id);
                List<Data_Set_Mission_Collect_Signed> data_set_sign_collect = mission_data.collect_signed;
                data_set_sign_collect.Sort((x, y) => x.employee_level.CompareTo(y.employee_level)); /// 排序

                string[] reocrd_data_arr = null;
                if (!record_data.Equals("")) { reocrd_data_arr = Regex.Split(record_data, ",,,,,"); }


                List<Data_Set_Sign> list_data_set_sign = new List<Data_Set_Sign>();

                /// 儲存現在簽的人
                Data_Set_Sign now_sign_person_data = new Data_Set_Sign();
                {
                    now_sign_person_data.extend_data = new Data_Set_Extends_LoanIt();
                    now_sign_person_data.e_id = data_set_employee.e_id;
                    now_sign_person_data.public_key = data_set_employee.publickey;
                    now_sign_person_data.company = data_set_employee.company;
                    now_sign_person_data.department = data_set_employee.department;
                    now_sign_person_data.sign_time = DateTime.Now.ToString();
                    now_sign_person_data.sign_status = "back";
                    if (data_set_employee.employee_level is null) { current_level = float.Parse(get_employee_level(data_set_employee.e_id, mission_data.mission_type, mission_data.risk_value, mission_data)); } else { current_level = int.Parse(data_set_employee.employee_level); }
                    now_sign_person_data.employee_level = current_level.ToString();
                    if (reocrd_data_arr[1] != "") { now_sign_person_data.extend_data.loan_amount = reocrd_data_arr[1]; } else { now_sign_person_data.extend_data.loan_amount = ""; }
                    if (reocrd_data_arr[2] != "") { now_sign_person_data.extend_data.loan_rate = reocrd_data_arr[2]; } else { now_sign_person_data.extend_data.loan_rate = "0"; }
                    if (reocrd_data_arr[3] != "") { now_sign_person_data.extend_data.loan_process_ratio = reocrd_data_arr[3]; } else { now_sign_person_data.extend_data.loan_process_ratio = "0"; }
                    if (reocrd_data_arr[4] != "") { now_sign_person_data.extend_data.advice = reocrd_data_arr[4]; } else { now_sign_person_data.extend_data.advice = reocrd_data_arr[4]; }
                    if (reocrd_data_arr[5] != "") { now_sign_person_data.extend_data.suggestion = reocrd_data_arr[5]; } else { now_sign_person_data.extend_data.suggestion = reocrd_data_arr[5]; }
                    if (reocrd_data_arr[6] != "") { now_sign_person_data.extend_data.special_note = reocrd_data_arr[6].ToLower(); } else { now_sign_person_data.extend_data.special_note = reocrd_data_arr[6].ToLower(); }
                    if (reocrd_data_arr[7] != "") { now_sign_person_data.extend_data.prepay_month = reocrd_data_arr[7].ToLower(); } else { now_sign_person_data.extend_data.prepay_month = reocrd_data_arr[7].ToLower(); }

                    list_data_set_sign.Add(now_sign_person_data);
                }

                /// 保存已簽過的人
                {
                    foreach (Data_Set_Mission_Collect_Signed collect_signed_data in data_set_sign_collect)
                    {
                        if (!collect_signed_data.public_key.Equals(""))
                        {
                            Data_Set_Sign save_collec_data = new Data_Set_Sign();
                            save_collec_data.e_id = collect_signed_data.e_id;
                            save_collec_data.public_key = collect_signed_data.public_key;
                            save_collec_data.company = collect_signed_data.company;
                            save_collec_data.department = collect_signed_data.department;
                            save_collec_data.employee_level = collect_signed_data.employee_level;
                            save_collec_data.sign_time = collect_signed_data.sign_time;
                            save_collec_data.extend_data = collect_signed_data.extend_data;


                            if (collect_signed_data.sign_status.Equals("pass"))
                            {
                                mission_fail = false; /// 控制是否要fail任務
                            }
                            /// 特【定】邏輯 要將上一層的人簽核結果變成【無效】
                            if (((current_level + 0.5) == (float.Parse(save_collec_data.employee_level))))
                            {
                                save_collec_data.sign_status = "false";
                                compare_isclear = true;
                            }
                            else
                            {
                                if (((current_level + 1) == (float.Parse(save_collec_data.employee_level))) && compare_isclear == false)
                                {
                                    save_collec_data.sign_status = "false";
                                }
                                else
                                {
                                    save_collec_data.sign_status = collect_signed_data.sign_status;
                                }
                            }
                            list_data_set_sign.Add(save_collec_data);
                        }
                    }
                }

                /// 若裡面包含pass代表不是第一關
                foreach (Data_Set_Sign item in list_data_set_sign)
                {
                    if (item.sign_status.Equals("pass"))
                    {
                        mission_fail = false;
                    }
                }


                if (mission_fail) /// 如果只有一個剛簽的人代表還在第一關，直接駁回。
                {
                    return excute_sign_fail(data_set_employee, m_id, remark);
                }
                else
                {
                    /// 保存至DB
                    {
                        string NewHistoryRecord = string.Format("【{3}】{0} 於 {1} 註記：【{2}】", data_set_employee.name, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), remark, "否決");
                        string history_record = mission_data.history + "\r\n" + NewHistoryRecord; // 把舊紀錄與新紀錄串接
                        string BackEvent_NewRequireSign = string.Format("{0},,,,,{1}", mission_data.m_id, get_BackEvent_NewRequireSign(mission_data));// 獲得新的簽層邏輯，直接將其退一階段

                        Record_Signed_History(now_sign_person_data, m_id); // 紀錄目前所有簽核
                        Record_Sign_history_Reason(m_id, history_record); // 紀錄備註
                        Record_Sign_Ststus_ID(m_id, 4.ToString()); // 紀錄目前狀態
                        Record_BackNewRequirSign(BackEvent_NewRequireSign);  // 紀錄新的簽層邏輯
                        Record_Signed_New_RiskValue(m_id, get_BackEvent_NewRiskValue(mission_data)); // 記錄目前風險值


                        return update_mission_new_signature(m_id, list_data_set_sign); ;
                    }
                }




            }
            else
            {
                throw new ArgumentException("請先登入");
            }
        }
        public Data_Set_Excutre_Result excute_sign_fail(Data_Set_Employee data_set_employee, string m_id, string history)
        {
            Data_Set_Excutre_Result result = CompareTokenvalid();

            if (result.excute_result.isSuccesed)
            {
                float current_level;

                Data_Set_Mission_Details mission_data = get_mission_information(m_id);
                List<Data_Set_Mission_Collect_Signed> data_set_sign_collect = mission_data.collect_signed;
                /// 保存已簽過的人
                List<Data_Set_Sign> list_data_set_sign = new List<Data_Set_Sign>();
                {
                    foreach (Data_Set_Mission_Collect_Signed collect_signed_data in data_set_sign_collect)
                    {
                        if (!collect_signed_data.public_key.Equals(""))
                        {
                            Data_Set_Sign save_collec_data = new Data_Set_Sign();
                            save_collec_data.e_id = collect_signed_data.e_id;
                            save_collec_data.public_key = collect_signed_data.public_key;
                            save_collec_data.company = collect_signed_data.company;
                            save_collec_data.department = collect_signed_data.department;
                            save_collec_data.employee_level = collect_signed_data.employee_level;
                            save_collec_data.sign_time = collect_signed_data.sign_time;
                            save_collec_data.extend_data = collect_signed_data.extend_data;
                            save_collec_data.sign_status = collect_signed_data.sign_status;
                            list_data_set_sign.Add(save_collec_data);
                        }
                    }
                }
                /// 儲存現在簽的人
                Data_Set_Sign now_sign_person_data = new Data_Set_Sign();
                {
                    now_sign_person_data.extend_data = new Data_Set_Extends_LoanIt();
                    now_sign_person_data.e_id = data_set_employee.e_id;
                    now_sign_person_data.public_key = data_set_employee.publickey;
                    now_sign_person_data.company = data_set_employee.company;
                    now_sign_person_data.department = data_set_employee.department;
                    now_sign_person_data.sign_time = DateTime.Now.ToString();
                    now_sign_person_data.sign_status = "fail";
                    if (data_set_employee.employee_level is null) { current_level = float.Parse(get_employee_level(data_set_employee.e_id, mission_data.mission_type, mission_data.risk_value, mission_data)); } else { current_level = int.Parse(data_set_employee.employee_level); }
                    now_sign_person_data.employee_level = current_level.ToString();
                    now_sign_person_data.extend_data = new Data_Set_Extends_LoanIt();
                    list_data_set_sign.Add(now_sign_person_data);
                }

                /// 保存至DB
                {
                    string NewhistoryRecord = string.Format("{0} 於 {1} 註記：【{2}】 簽核結果：【{3}】", data_set_employee.name, DateTime.Now, history, "否決");
                    string history_record = mission_data.history + "\r\n" + NewhistoryRecord; // 把舊紀錄與新紀錄串接


                    Record_Signed_History(now_sign_person_data, m_id); // 紀錄目前所有簽核
                    Record_Sign_history_Reason(m_id, history_record); // 紀錄備註
                    Record_Sign_Ststus_ID(m_id, 99.ToString()); // 紀錄目前狀態
                    return update_mission_new_signature(m_id, list_data_set_sign); ;
                }
            }
            else
            {
                throw new ArgumentException("請先登入");
            }
        }
        public Data_Set_Excutre_Result excute_sign_pass_loanit(Data_Set_Employee data_set_employee, string m_id, string record_data)
        {
            Controller_Sign sign = Controller_Sign.getInstance();
            string current_amount = "0"; string current_rate = "0"; string current_ProcessRatio = "0";
            Data_Set_Excutre_Result result = CompareTokenvalid();


            if (result.excute_result.isSuccesed)
            {

                Data_Set_Mission_Details mission_data = get_mission_information(m_id);
                List<Data_Set_Mission_Collect_Signed> data_set_sign_collect = mission_data.collect_signed;
                mission_data.require_sign.Sort((x, y) => -x.employee_level.CompareTo(y.employee_level)); /// 排序

                /// 保存已簽過的人
                List<Data_Set_Sign> list_data_set_sign = new List<Data_Set_Sign>();
                {
                    foreach (Data_Set_Mission_Collect_Signed collect_signed_data in data_set_sign_collect)
                    {
                        if (!collect_signed_data.public_key.Equals(""))
                        {
                            Data_Set_Sign save_collec_data = new Data_Set_Sign();
                            save_collec_data.e_id = collect_signed_data.e_id;
                            save_collec_data.public_key = collect_signed_data.public_key;
                            save_collec_data.company = collect_signed_data.company;
                            save_collec_data.department = collect_signed_data.department;
                            save_collec_data.employee_level = collect_signed_data.employee_level;
                            save_collec_data.sign_time = collect_signed_data.sign_time;
                            save_collec_data.extend_data = collect_signed_data.extend_data;
                            save_collec_data.sign_status = collect_signed_data.sign_status;
                            if (data_set_employee.department.Equals("GA") && collect_signed_data.sign_status.Equals("back")) { save_collec_data.sign_status = "back_cc"; } else { save_collec_data.sign_status = collect_signed_data.sign_status; }
                            current_amount = save_collec_data.extend_data.loan_amount;
                            current_rate = save_collec_data.extend_data.loan_rate;
                            current_ProcessRatio = save_collec_data.extend_data.loan_process_ratio;


                            list_data_set_sign.Add(save_collec_data);
                        }
                    }
                }
                string[] reocrd_data_arr = null;
                if (!record_data.Equals("")) { reocrd_data_arr = Regex.Split(record_data, ",,,,,"); }
                /// 儲存現在簽的人
                Data_Set_Sign now_sign_person_data = new Data_Set_Sign();
                {
                    now_sign_person_data.extend_data = new Data_Set_Extends_LoanIt();
                    now_sign_person_data.e_id = data_set_employee.e_id;
                    now_sign_person_data.public_key = data_set_employee.publickey;
                    now_sign_person_data.company = data_set_employee.company;
                    now_sign_person_data.department = data_set_employee.department;
                    if (data_set_employee.employee_level is null) { now_sign_person_data.employee_level = get_employee_level(data_set_employee.e_id, mission_data.mission_type, mission_data.risk_value, mission_data); } else { now_sign_person_data.employee_level = data_set_employee.employee_level; }
                    now_sign_person_data.sign_time = DateTime.Now.ToString();
                    now_sign_person_data.sign_status = "pass";
                    if (reocrd_data_arr[1] != "") { now_sign_person_data.extend_data.loan_amount = reocrd_data_arr[1]; } else { now_sign_person_data.extend_data.loan_amount = current_amount; }
                    if (reocrd_data_arr[2] != "") { now_sign_person_data.extend_data.loan_rate = reocrd_data_arr[2]; } else { now_sign_person_data.extend_data.loan_rate = current_rate; }
                    if (reocrd_data_arr[3] != "") { now_sign_person_data.extend_data.loan_process_ratio = reocrd_data_arr[3]; } else { now_sign_person_data.extend_data.loan_process_ratio = current_ProcessRatio; }
                    if (reocrd_data_arr[4] != "") { now_sign_person_data.extend_data.advice = reocrd_data_arr[4]; } else { now_sign_person_data.extend_data.advice = reocrd_data_arr[4]; }
                    if (reocrd_data_arr[5] != "") { now_sign_person_data.extend_data.suggestion = reocrd_data_arr[5]; } else { now_sign_person_data.extend_data.suggestion = reocrd_data_arr[5]; }
                    if (reocrd_data_arr[6] != "") { now_sign_person_data.extend_data.special_note = reocrd_data_arr[6].ToLower(); } else { now_sign_person_data.extend_data.special_note = reocrd_data_arr[6].ToLower(); }
                    if (reocrd_data_arr[7] != "") { now_sign_person_data.extend_data.prepay_month = reocrd_data_arr[7].ToLower(); } else { now_sign_person_data.extend_data.prepay_month = reocrd_data_arr[7].ToLower(); }
                    list_data_set_sign.Add(now_sign_person_data);
                }




                /// 判斷風險值是否改變
                if (!reocrd_data_arr[1].Equals(""))
                {
                    reocrd_data_arr[1] = reocrd_data_arr[1].Replace(",", "");
                    string compare_risk_value_result = Compare_New_Risk(reocrd_data_arr[1], reocrd_data_arr[3]);
                    if (compare_risk_value_result.Split(',')[0].Equals("true"))
                    {
                        string[] Risk_Arr = compare_risk_value_result.Split(',');
                        //if(mission_data)
                        if ((int.Parse(Risk_Arr[1].Trim()) >= int.Parse(mission_data.risk_value.Trim())))
                        {
                            string new_require_sign = LOANIT_RISK.instance.get_mission_require_signature(int.Parse(Risk_Arr[1]), (SubSysNo)Enum.Parse(typeof(SubSysNo), mission_data.mission_type));

                            if (update_mission_new_require_sign_rule(m_id, new_require_sign).excute_result.isSuccesed)
                            {
                                /// 更新完新簽核條件後所執行的動作
                                Record_Signed_New_RiskValue(m_id, Risk_Arr[1]); // 記錄目前風險值
                            }
                        }
                    }
                    else
                    {
                        Data_Set_Excutre_Result compare_reisk_result = new Data_Set_Excutre_Result();
                        compare_reisk_result.excute_result = new Data_Set_Result();
                        compare_reisk_result.excute_result.isSuccesed = false;
                        compare_reisk_result.excute_result.isError = true;
                        compare_reisk_result.excute_result.result = "失敗";
                        compare_reisk_result.excute_result.feedb_back_message = "判斷風險值失敗，請聯絡【研發中心-郁宸】";
                        compare_reisk_result.excute_result.fail_reason = "無指定的條件";
                        return compare_reisk_result;
                    }
                }


                /// 保存至DB
                {
                    string NewHistoryRecord = string.Format("【{3}】{0} 於 {1} 註記：【{2}】", data_set_employee.name, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), reocrd_data_arr[0], "通過");
                    string history_record = mission_data.history + "\r\n" + NewHistoryRecord; // 把舊紀錄與新紀錄串接

                    Record_Signed_History(now_sign_person_data, m_id); // 紀錄目前所有簽核
                    Record_Sign_history_Reason(m_id, history_record); // 紀錄備註
                    Record_Sign_Ststus_ID(m_id, 2.ToString()); // 紀錄目前狀態
                    return update_mission_new_signature(m_id, list_data_set_sign); ;
                }
            }
            else
            {
                throw new ArgumentException("請先登入");
            }
        }
        public Data_Set_Excutre_Result excute_sign_done_loanit(Data_Set_Employee data_set_employee, string m_id, string record_data)
        {
            string current_amount = "0"; string current_rate = "0"; string current_ProcessRatio = "0";
            Data_Set_Excutre_Result result = CompareTokenvalid();

            if (result.excute_result.isSuccesed)
            {

                Data_Set_Mission_Details mission_data = get_mission_information(m_id);
                List<Data_Set_Mission_Collect_Signed> data_set_sign_collect = mission_data.collect_signed;
                /// 保存已簽過的人
                List<Data_Set_Sign> list_data_set_sign = new List<Data_Set_Sign>();
                {
                    foreach (Data_Set_Mission_Collect_Signed collect_signed_data in data_set_sign_collect)
                    {
                        if (!collect_signed_data.public_key.Equals(""))
                        {
                            Data_Set_Sign save_collec_data = new Data_Set_Sign();
                            save_collec_data.e_id = collect_signed_data.e_id;
                            save_collec_data.public_key = collect_signed_data.public_key;
                            save_collec_data.company = collect_signed_data.company;
                            save_collec_data.department = collect_signed_data.department;
                            save_collec_data.employee_level = collect_signed_data.employee_level;
                            save_collec_data.sign_time = collect_signed_data.sign_time;
                            save_collec_data.extend_data = collect_signed_data.extend_data;

                            if (data_set_employee.department.Equals("GA") && collect_signed_data.sign_status.Equals("back")) { save_collec_data.sign_status = "back_cc"; } else { save_collec_data.sign_status = collect_signed_data.sign_status; }
                            current_amount = save_collec_data.extend_data.loan_amount;
                            current_ProcessRatio = save_collec_data.extend_data.loan_process_ratio;
                            current_rate = save_collec_data.extend_data.loan_rate;

                            list_data_set_sign.Add(save_collec_data);
                        }
                    }
                }


                string[] reocrd_data_arr = null;
                if (!record_data.Equals("")) { reocrd_data_arr = Regex.Split(record_data, ",,,,,"); }
                /// 儲存現在簽的人
                Data_Set_Sign now_sign_person_data = new Data_Set_Sign();
                {
                    now_sign_person_data.extend_data = new Data_Set_Extends_LoanIt();
                    now_sign_person_data.e_id = data_set_employee.e_id;
                    now_sign_person_data.public_key = data_set_employee.publickey;
                    now_sign_person_data.company = data_set_employee.company;
                    now_sign_person_data.department = data_set_employee.department;
                    if (data_set_employee.employee_level is null) { now_sign_person_data.employee_level = get_employee_level(data_set_employee.e_id, mission_data.mission_type, mission_data.risk_value, mission_data); } else { now_sign_person_data.employee_level = data_set_employee.employee_level; }
                    now_sign_person_data.sign_time = DateTime.Now.ToString();
                    now_sign_person_data.sign_status = "pass";
                    if (reocrd_data_arr[1] != "") { now_sign_person_data.extend_data.loan_amount = reocrd_data_arr[1]; } else { now_sign_person_data.extend_data.loan_amount = current_amount; }
                    if (reocrd_data_arr[2] != "") { now_sign_person_data.extend_data.loan_rate = reocrd_data_arr[2]; } else { now_sign_person_data.extend_data.loan_rate = current_rate; }
                    if (reocrd_data_arr[3] != "") { now_sign_person_data.extend_data.loan_process_ratio = reocrd_data_arr[3]; } else { now_sign_person_data.extend_data.loan_process_ratio = current_ProcessRatio; }
                    if (reocrd_data_arr[4] != "") { now_sign_person_data.extend_data.advice = reocrd_data_arr[4]; } else { now_sign_person_data.extend_data.advice = reocrd_data_arr[4]; }
                    if (reocrd_data_arr[5] != "") { now_sign_person_data.extend_data.suggestion = reocrd_data_arr[5]; } else { now_sign_person_data.extend_data.suggestion = reocrd_data_arr[5]; }
                    if (reocrd_data_arr[6] != "") { now_sign_person_data.extend_data.special_note = reocrd_data_arr[6].ToLower(); } else { now_sign_person_data.extend_data.special_note = reocrd_data_arr[6].ToLower(); }
                    if (reocrd_data_arr[7] != "") { now_sign_person_data.extend_data.prepay_month = reocrd_data_arr[7]; } else { now_sign_person_data.extend_data.prepay_month = "0"; }

                    list_data_set_sign.Add(now_sign_person_data);
                }

                /// 保存至DB
                {

                    if (get_mission_status_id(list_data_set_sign, mission_data.require_sign))
                    {

                        string NewHistoryRecord = string.Format("【{3}】{0} 於 {1} 註記：【{2}】", data_set_employee.name, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), reocrd_data_arr[0], "核定通過");
                        string history_record = mission_data.history + "\r\n" + NewHistoryRecord; // 把舊紀錄與新紀錄串接

                        Record_Signed_History(now_sign_person_data, m_id); // 記錄簽核歷史
                        Record_Sign_history_Reason(m_id, history_record); // 紀錄備註
                        Record_Sign_Ststus_ID(m_id, 3.ToString()); // 更新簽核任務狀態
                        return update_mission_new_signature(m_id, list_data_set_sign); ;
                    }
                    else
                    {
                        Data_Set_Excutre_Result sign_done_result = new Data_Set_Excutre_Result();
                        sign_done_result.excute_result = new Data_Set_Result();
                        sign_done_result.excute_result.isSuccesed = false;
                        sign_done_result.excute_result.isError = true;
                        sign_done_result.excute_result.result = "失敗";
                        sign_done_result.excute_result.fail_reason = "尚有簽章未蒐集，請檢查；若無法排除請聯絡【研發中心-郁宸】。";
                        sign_done_result.excute_result.feedb_back_message = "尚未把簽章蒐集完成";
                        return sign_done_result;
                    }

                }
            }
            else
            {
                throw new ArgumentException("請先登入");
            }
        }
        public bool compare_current_amount_rule(Data_Set_Employee data_set_employee, string m_id, string current_amount, string current_processratio)
        {
            current_amount = current_amount.Replace(",", "");
            Data_Set_Mission_Details mission_data = get_mission_information(m_id);
            List<Data_Set_Mission_Collect_Signed> data_set_sign_collect = mission_data.collect_signed;
            mission_data.require_sign.Sort((x, y) => -x.employee_level.CompareTo(y.employee_level)); /// 排序

            /// 保存已簽過的人                                                                                   
            List<Data_Set_Sign> list_data_set_sign = new List<Data_Set_Sign>();
            {
                foreach (Data_Set_Mission_Collect_Signed collect_signed_data in data_set_sign_collect)
                {
                    if (!collect_signed_data.public_key.Equals(""))
                    {
                        Data_Set_Sign save_collec_data = new Data_Set_Sign();
                        save_collec_data.e_id = collect_signed_data.e_id;
                        save_collec_data.public_key = collect_signed_data.public_key;
                        save_collec_data.company = collect_signed_data.company;
                        save_collec_data.department = collect_signed_data.department;
                        save_collec_data.employee_level = collect_signed_data.employee_level;
                        save_collec_data.sign_time = collect_signed_data.sign_time;
                        save_collec_data.extend_data = collect_signed_data.extend_data;

                        if (data_set_employee.department.Equals("GA") && collect_signed_data.sign_status.Equals("back")) { save_collec_data.sign_status = "back_cc"; } else { save_collec_data.sign_status = collect_signed_data.sign_status; }
                        list_data_set_sign.Add(save_collec_data);
                    }
                }
            }

            string compare_risk_value_result = Compare_New_Risk(current_amount, current_processratio);
            if (compare_risk_value_result.Contains("4")) { compare_risk_value_result = "true,3"; } /// 因財務長拔掉因此需這樣做；原因：保留財務長功能
            string[] Risk_Arr = compare_risk_value_result.Split(',');
            //if(mission_data)
            if ((int.Parse(Risk_Arr[1].Trim()) <= int.Parse(mission_data.risk_value.Trim())))
            {
                return false;
            }
            else
            {
                return true;
            }

        }


        #endregion

        #region LOANIT專屬逐級-簽核功能-兄弟姊妹單
        public Data_Set_Excutre_Result excute_sign_pass_sibling_loanit(Data_Set_Employee data_set_employee, string m_id, string record_data)
        {
            Controller_Sign sign = Controller_Sign.getInstance();
            string current_amount = "0"; string current_rate = "0"; string current_ProcessRatio = "0";
            Data_Set_Excutre_Result result = CompareTokenvalid();


            if (result.excute_result.isSuccesed)
            {

                Data_Set_Mission_Details mission_data = get_mission_information(m_id);
                List<Data_Set_Mission_Collect_Signed> data_set_sign_collect = mission_data.collect_signed;
                mission_data.require_sign.Sort((x, y) => -x.employee_level.CompareTo(y.employee_level)); /// 排序

                /// 保存已簽過的人
                List<Data_Set_Sign> list_data_set_sign = new List<Data_Set_Sign>();
                {
                    foreach (Data_Set_Mission_Collect_Signed collect_signed_data in data_set_sign_collect)
                    {
                        if (!collect_signed_data.public_key.Equals(""))
                        {
                            Data_Set_Sign save_collec_data = new Data_Set_Sign();
                            save_collec_data.e_id = collect_signed_data.e_id;
                            save_collec_data.public_key = collect_signed_data.public_key;
                            save_collec_data.company = collect_signed_data.company;
                            save_collec_data.department = collect_signed_data.department;
                            save_collec_data.employee_level = collect_signed_data.employee_level;
                            save_collec_data.sign_time = collect_signed_data.sign_time;
                            save_collec_data.extend_data = collect_signed_data.extend_data;
                            save_collec_data.sign_status = collect_signed_data.sign_status;
                            if (data_set_employee.department.Equals("GA") && collect_signed_data.sign_status.Equals("back")) { save_collec_data.sign_status = "back_cc"; } else { save_collec_data.sign_status = collect_signed_data.sign_status; }
                            current_amount = save_collec_data.extend_data.loan_amount;
                            current_rate = save_collec_data.extend_data.loan_rate;
                            current_ProcessRatio = save_collec_data.extend_data.loan_process_ratio;
                            list_data_set_sign.Add(save_collec_data);
                        }
                    }
                }
                string[] reocrd_data_arr = null;
                if (!record_data.Equals("")) { reocrd_data_arr = Regex.Split(record_data, ",,,,,"); }
                /// 儲存現在簽的人
                Data_Set_Sign now_sign_person_data = new Data_Set_Sign();
                {
                    now_sign_person_data.extend_data = new Data_Set_Extends_LoanIt();
                    now_sign_person_data.e_id = data_set_employee.e_id;
                    now_sign_person_data.public_key = data_set_employee.publickey;
                    now_sign_person_data.company = data_set_employee.company;
                    now_sign_person_data.department = data_set_employee.department;
                    if (data_set_employee.employee_level is null) { now_sign_person_data.employee_level = get_employee_level(data_set_employee.e_id, mission_data.mission_type, mission_data.risk_value, mission_data); data_set_employee.employee_level = now_sign_person_data.employee_level; } else { now_sign_person_data.employee_level = data_set_employee.employee_level; }
                    now_sign_person_data.sign_time = DateTime.Now.ToString();
                    now_sign_person_data.sign_status = "pass";
                    if (reocrd_data_arr[1] != "") { now_sign_person_data.extend_data.loan_amount = reocrd_data_arr[1]; current_amount = now_sign_person_data.extend_data.loan_amount; } else { now_sign_person_data.extend_data.loan_amount = current_amount; }
                    if (reocrd_data_arr[2] != "") { now_sign_person_data.extend_data.loan_rate = reocrd_data_arr[2]; } else { now_sign_person_data.extend_data.loan_rate = current_rate; }
                    if (reocrd_data_arr[3] != "") { now_sign_person_data.extend_data.loan_process_ratio = reocrd_data_arr[3]; } else { now_sign_person_data.extend_data.loan_process_ratio = current_ProcessRatio; }
                    if (reocrd_data_arr[4] != "") { now_sign_person_data.extend_data.advice = reocrd_data_arr[4]; } else { now_sign_person_data.extend_data.advice = reocrd_data_arr[4]; }
                    if (reocrd_data_arr[5] != "") { now_sign_person_data.extend_data.suggestion = reocrd_data_arr[5]; } else { now_sign_person_data.extend_data.suggestion = reocrd_data_arr[5]; }
                    if (reocrd_data_arr[6] != "") { now_sign_person_data.extend_data.special_note = reocrd_data_arr[6].ToLower(); } else { now_sign_person_data.extend_data.special_note = reocrd_data_arr[6].ToLower(); }
                    list_data_set_sign.Add(now_sign_person_data);
                }

                /// 判斷風險值是否改變
                if (!reocrd_data_arr[1].Equals(""))
                {
                    current_amount = computing_CurrentAmount_SiblingRule(mission_data, int.Parse(current_amount.Replace(",", "")), data_set_employee, get_HighestEmployeeRequireLevel(mission_data)).ToString();
                    string compare_risk_value_result = Compare_New_Risk(current_amount, reocrd_data_arr[3]);///需修改成 總金額
                    if (compare_risk_value_result.Split(',')[0].Equals("true"))
                    {
                        string[] Risk_Arr = compare_risk_value_result.Split(',');
                        //if(mission_data)
                        if ((int.Parse(Risk_Arr[1].Trim()) > int.Parse(mission_data.risk_value.Trim())))
                        {
                            string new_require_sign = LOANIT.instance.get_mission_require_signature(int.Parse(Risk_Arr[1]), SubSysNo.CH001);

                            /// 更新【兄弟姊妹單】的狀態
                            if (update_CollectSignStatus_pass_sibling_rule(mission_data, data_set_employee, new_require_sign, Risk_Arr[1]).isSuccesed)
                            {
                                /// 更新自己的簽核需求
                                if (update_mission_new_require_sign_rule(m_id, new_require_sign).excute_result.isSuccesed)
                                {
                                    /// 更新完新簽核條件後所執行的動作
                                    Record_Signed_New_RiskValue(m_id, Risk_Arr[1]); // 記錄目前風險值
                                }
                                /// 目前沒有需執行動作
                                Console.WriteLine("【兄弟姊妹單】已更新完畢");
                            }
                            else
                            {
                                MessageBox.Show(string.Format("發生【兄弟姊妹單】更新錯誤，請聯絡【研發中心-郁宸】"));
                            }
                        }
                    }
                    else
                    {
                        Data_Set_Excutre_Result compare_reisk_result = new Data_Set_Excutre_Result();
                        compare_reisk_result.excute_result = new Data_Set_Result();
                        compare_reisk_result.excute_result.isSuccesed = false;
                        compare_reisk_result.excute_result.isError = true;
                        compare_reisk_result.excute_result.result = "失敗";
                        compare_reisk_result.excute_result.feedb_back_message = "判斷風險值失敗，請聯絡【研發中心-郁宸】";
                        compare_reisk_result.excute_result.fail_reason = "無指定的條件";
                        return compare_reisk_result;
                    }
                }


                /// 保存至DB
                {
                    string NewHistoryRecord = string.Format("【{3}】{0} 於 {1} 註記：【{2}】", data_set_employee.name, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), reocrd_data_arr[0], "通過");
                    string history_record = mission_data.history + "\r\n" + NewHistoryRecord; // 把舊紀錄與新紀錄串接

                    Record_Signed_History(now_sign_person_data, m_id); // 紀錄目前所有簽核
                    Record_Sign_history_Reason(m_id, history_record); // 紀錄備註
                    Record_Sign_Ststus_ID(m_id, 2.ToString()); // 紀錄目前狀態
                    return update_mission_new_signature(m_id, list_data_set_sign); ;
                }
            }
            else
            {
                throw new ArgumentException("請先登入");
            }


            throw new Exception("尚未測試 兄弟姊妹單 的呈核");
        }
        public Data_Set_Excutre_Result excute_sign_done_sibling_loanit(Data_Set_Employee data_set_employee, string m_id, string record_data)
        {
            string current_amount = "0"; string current_rate = "0"; string current_ProcessRatio = "0";
            Data_Set_Excutre_Result result = CompareTokenvalid();

            if (result.excute_result.isSuccesed)
            {

                Data_Set_Mission_Details mission_data = get_mission_information(m_id);
                List<Data_Set_Mission_Collect_Signed> data_set_sign_collect = mission_data.collect_signed;
                /// 保存已簽過的人
                List<Data_Set_Sign> list_data_set_sign = new List<Data_Set_Sign>();
                {
                    foreach (Data_Set_Mission_Collect_Signed collect_signed_data in data_set_sign_collect)
                    {
                        if (!collect_signed_data.public_key.Equals(""))
                        {
                            Data_Set_Sign save_collec_data = new Data_Set_Sign();
                            save_collec_data.e_id = collect_signed_data.e_id;
                            save_collec_data.public_key = collect_signed_data.public_key;
                            save_collec_data.company = collect_signed_data.company;
                            save_collec_data.department = collect_signed_data.department;
                            save_collec_data.employee_level = collect_signed_data.employee_level;
                            save_collec_data.sign_time = collect_signed_data.sign_time;
                            save_collec_data.extend_data = collect_signed_data.extend_data;

                            if (data_set_employee.department.Equals("GA") && collect_signed_data.sign_status.Equals("back")) { save_collec_data.sign_status = "back_cc"; } else { save_collec_data.sign_status = collect_signed_data.sign_status; }
                            current_amount = save_collec_data.extend_data.loan_amount;
                            current_ProcessRatio = save_collec_data.extend_data.loan_process_ratio;
                            current_rate = save_collec_data.extend_data.loan_rate;

                            list_data_set_sign.Add(save_collec_data);
                        }
                    }
                }


                string[] reocrd_data_arr = null;
                if (!record_data.Equals("")) { reocrd_data_arr = Regex.Split(record_data, ",,,,,"); }
                /// 儲存現在簽的人
                Data_Set_Sign now_sign_person_data = new Data_Set_Sign();
                {
                    now_sign_person_data.extend_data = new Data_Set_Extends_LoanIt();
                    now_sign_person_data.e_id = data_set_employee.e_id;
                    now_sign_person_data.public_key = data_set_employee.publickey;
                    now_sign_person_data.company = data_set_employee.company;
                    now_sign_person_data.department = data_set_employee.department;
                    if (data_set_employee.employee_level is null) { now_sign_person_data.employee_level = get_employee_level(data_set_employee.e_id, mission_data.mission_type, mission_data.risk_value, mission_data); } else { now_sign_person_data.employee_level = data_set_employee.employee_level; }
                    now_sign_person_data.sign_time = DateTime.Now.ToString();
                    now_sign_person_data.sign_status = CompareIsTempDoneOrPass(mission_data, data_set_employee).ToString().ToLower();
                    if (reocrd_data_arr[1] != "") { now_sign_person_data.extend_data.loan_amount = reocrd_data_arr[1]; } else { now_sign_person_data.extend_data.loan_amount = current_amount; }
                    if (reocrd_data_arr[2] != "") { now_sign_person_data.extend_data.loan_rate = reocrd_data_arr[2]; } else { now_sign_person_data.extend_data.loan_rate = current_rate; }
                    if (reocrd_data_arr[3] != "") { now_sign_person_data.extend_data.loan_process_ratio = reocrd_data_arr[3]; } else { now_sign_person_data.extend_data.loan_process_ratio = current_ProcessRatio; }
                    if (reocrd_data_arr[4] != "") { now_sign_person_data.extend_data.advice = reocrd_data_arr[4]; } else { now_sign_person_data.extend_data.advice = reocrd_data_arr[4]; }
                    if (reocrd_data_arr[5] != "") { now_sign_person_data.extend_data.suggestion = reocrd_data_arr[5]; } else { now_sign_person_data.extend_data.suggestion = reocrd_data_arr[5]; }
                    if (reocrd_data_arr[6] != "") { now_sign_person_data.extend_data.special_note = reocrd_data_arr[6].ToLower(); } else { now_sign_person_data.extend_data.special_note = reocrd_data_arr[6].ToLower(); }

                    list_data_set_sign.Add(now_sign_person_data);
                }

                /// 保存至DB
                {

                    if (get_mission_status_id(list_data_set_sign, mission_data.require_sign))
                    {
                        Sign_Status sign_status = CompareIsTempDoneOrPass(mission_data, data_set_employee);

                        update_CollectSignStatus_done_sibling_rule(mission_data, data_set_employee); /// 更新兄弟單的狀態

                        string NewHistoryRecord = string.Format("【{3}】{0} 於 {1} 註記：【{2}】", data_set_employee.name, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), reocrd_data_arr[0], "核定通過");
                        string history_record = mission_data.history + "\r\n" + NewHistoryRecord; // 把舊紀錄與新紀錄串接

                        Record_Signed_History(now_sign_person_data, m_id); // 記錄簽核歷史
                        Record_Sign_history_Reason(m_id, history_record); // 紀錄備註
                        if (sign_status.ToString().Equals("Done"))
                        {
                            Record_Sign_Ststus_ID(m_id, 3.ToString()); // 更新簽核任務狀態
                        }
                        else if (sign_status.ToString().Equals("Temp_Done"))
                        {
                            Record_Sign_Ststus_ID(m_id, 39.ToString()); // 更新簽核任務狀態
                        }
                        else
                        {
                            new Exception("錯誤流程，請聯絡【研發中心-郁宸】");
                        }
                        return update_mission_new_signature(m_id, list_data_set_sign); ;
                    }
                    else
                    {
                        Data_Set_Excutre_Result sign_done_result = new Data_Set_Excutre_Result();
                        sign_done_result.excute_result = new Data_Set_Result();
                        sign_done_result.excute_result.isSuccesed = false;
                        sign_done_result.excute_result.isError = true;
                        sign_done_result.excute_result.result = "失敗";
                        sign_done_result.excute_result.fail_reason = "尚有簽章未蒐集，請檢查；若無法排除請聯絡【研發中心-郁宸】。";
                        sign_done_result.excute_result.feedb_back_message = "尚未把簽章蒐集完成";
                        return sign_done_result;
                    }

                }
            }
            else
            {
                throw new ArgumentException("請先登入");
            }




            throw new Exception("尚未完成 兄弟姊妹單 的核定");
        }
        public Data_Set_Excutre_Result excute_sign_back_sibling_loanit(Data_Set_Employee data_set_employee, string m_id, string remark)
        {
            bool mission_fail = true;
            Controller_Sign sign = Controller_Sign.getInstance();
            string current_amount = "0"; string current_rate = "0"; string current_ProcessRatio = "0";
            Data_Set_Excutre_Result result = CompareTokenvalid();

            float current_level;

            if (result.excute_result.isSuccesed)
            {

                Data_Set_Mission_Details mission_data = get_mission_information(m_id);
                data_set_employee.employee_level = get_employee_level(data_set_employee.e_id, mission_data.mission_type, mission_data.risk_value, mission_data);
                List<Data_Set_Mission_Collect_Signed> data_set_sign_collect = mission_data.collect_signed;
                mission_data.require_sign.Sort((x, y) => -x.employee_level.CompareTo(y.employee_level)); /// 排序

                List<Data_Set_Sign> list_data_set_sign = new List<Data_Set_Sign>();

                string[] reocrd_data_arr = null;
                /// 儲存現在簽的人
                Data_Set_Sign now_sign_person_data = new Data_Set_Sign();
                {
                    now_sign_person_data.extend_data = new Data_Set_Extends_LoanIt();
                    now_sign_person_data.e_id = data_set_employee.e_id;
                    now_sign_person_data.public_key = data_set_employee.publickey;
                    now_sign_person_data.company = data_set_employee.company;
                    now_sign_person_data.department = data_set_employee.department;
                    now_sign_person_data.sign_time = DateTime.Now.ToString();
                    now_sign_person_data.sign_status = "back";
                    if (data_set_employee.employee_level is null) { current_level = float.Parse(get_employee_level(data_set_employee.e_id, mission_data.mission_type, mission_data.risk_value, mission_data)); } else { current_level = int.Parse(data_set_employee.employee_level); }
                    now_sign_person_data.employee_level = current_level.ToString();
                    now_sign_person_data.extend_data = new Data_Set_Extends_LoanIt();

                    list_data_set_sign.Add(now_sign_person_data);
                }



                /// 保存已簽過的人
                {
                    foreach (Data_Set_Mission_Collect_Signed collect_signed_data in data_set_sign_collect)
                    {
                        if (!collect_signed_data.public_key.Equals(""))
                        {
                            Data_Set_Sign save_collec_data = new Data_Set_Sign();
                            save_collec_data.e_id = collect_signed_data.e_id;
                            save_collec_data.public_key = collect_signed_data.public_key;
                            save_collec_data.company = collect_signed_data.company;
                            save_collec_data.department = collect_signed_data.department;
                            save_collec_data.employee_level = collect_signed_data.employee_level;
                            save_collec_data.sign_time = collect_signed_data.sign_time;
                            save_collec_data.extend_data = collect_signed_data.extend_data;

                            if (collect_signed_data.sign_status.Equals("pass"))
                            {
                                mission_fail = false; /// 控制是否要fail任務
                            }
                            if (((current_level + 1) == (float.Parse(save_collec_data.employee_level))))
                            {
                                save_collec_data.sign_status = "false";
                            }
                            else
                            {
                                save_collec_data.sign_status = collect_signed_data.sign_status;
                            }
                            if (data_set_employee.department.Equals("GA") && collect_signed_data.sign_status.Equals("back")) { save_collec_data.sign_status = "back_cc"; }
                            current_amount = save_collec_data.extend_data.loan_amount;
                            current_rate = save_collec_data.extend_data.loan_rate;
                            current_ProcessRatio = save_collec_data.extend_data.loan_process_ratio;
                            list_data_set_sign.Add(save_collec_data);
                        }
                    }
                }


                /// 若裡面包含pass代表不是第一關
                /// 
                foreach (Data_Set_Sign item in list_data_set_sign)
                {
                    if (item.sign_status.Equals("pass"))
                    {
                        mission_fail = false;
                    }
                }

                if (mission_fail) /// 如果只有一個剛簽的人代表還在第一關，直接駁回。
                {
                    update_CollectSignStatus_fail_sibling_rule(mission_data, data_set_employee); //處理兄弟單 駁回

                    return excute_sign_fail(data_set_employee, m_id, remark);//處理本單 駁回

                }
                else
                {
                    update_CollectSignStatus_back_sibling_rule(mission_data, current_level); /// 更新兄弟單的退回狀態
                                                                                             /// 保存至DB
                    {
                        string NewHistoryRecord = string.Format("【{3}】{0} 於 {1} 註記：【{2}】", data_set_employee.name, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), remark, "否決");
                        string history_record = mission_data.history + "\r\n" + NewHistoryRecord; // 把舊紀錄與新紀錄串接

                        Record_Signed_History(now_sign_person_data, m_id); // 紀錄目前所有簽核
                        Record_Sign_history_Reason(m_id, history_record); // 紀錄備註
                        Record_Sign_Ststus_ID(m_id, 4.ToString()); // 紀錄目前狀態
                        return update_mission_new_signature(m_id, list_data_set_sign); ;
                    }
                }

            }
            else
            {
                throw new ArgumentException("請先登入");
            }



            throw new Exception("尚未完成 兄弟姊妹單 的退回");
        }
        public bool compare_current_amount_SiblingRule(Data_Set_Employee data_set_employee, string m_id, string input_current_amount, string current_processratio)
        {
            /// 經由{compare_current_amount_rule}移植更新而成



            int current_amount = int.Parse(input_current_amount.Replace(",", ""));
            Data_Set_Mission_Details mission_data = get_mission_information(m_id);

            /// <remarks>
            /// 用 get_mission_information_all 去撈出所有資訊，將一樣的binding_project抓出來，在修改以下去撈出 目前總金額，算完再去算目前風險值。
            /// </remarks>
            current_amount = computing_CurrentAmount_SiblingRule(mission_data, current_amount, data_set_employee, get_HighestEmployeeRequireLevel(mission_data));

            ///// 儲存目前已簽過的人
            //{
            //    List<Data_Set_Mission_Collect_Signed> data_set_sign_collect = mission_data.collect_signed;
            //    mission_data.require_sign.Sort((x, y) => -x.employee_level.CompareTo(y.employee_level)); /// 排序

            //    /// 保存已簽過的人                                                                                                                                                                            
            //    List<Data_Set_Sign> list_data_set_sign = new List<Data_Set_Sign>();
            //    {
            //        foreach (Data_Set_Mission_Collect_Signed collect_signed_data in data_set_sign_collect)
            //        {
            //            if (!collect_signed_data.public_key.Equals(""))
            //            {
            //                Data_Set_Sign save_collec_data = new Data_Set_Sign();
            //                save_collec_data.e_id = collect_signed_data.e_id;
            //                save_collec_data.public_key = collect_signed_data.public_key;
            //                save_collec_data.company = collect_signed_data.company;
            //                save_collec_data.department = collect_signed_data.department;
            //                save_collec_data.employee_level = collect_signed_data.employee_level;
            //                save_collec_data.sign_time = collect_signed_data.sign_time;
            //                save_collec_data.extend_data = collect_signed_data.extend_data;

            //                if (data_set_employee.department.Equals("GA") && collect_signed_data.sign_status.Equals("back")) { save_collec_data.sign_status = "back_cc"; } else { save_collec_data.sign_status = collect_signed_data.sign_status; }
            //                list_data_set_sign.Add(save_collec_data);
            //            }
            //        }
            //    }
            //}


            string compare_risk_value_result = Compare_New_Risk(current_amount.ToString(), current_processratio);

            string[] Risk_Arr = compare_risk_value_result.Split(',');
            //if(mission_data)
            if ((int.Parse(Risk_Arr[1].Trim()) <= int.Parse(mission_data.risk_value.Trim())))
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public int computing_CurrentAmount_SiblingRule(Data_Set_Mission_Details mission_data, int current_amount, Data_Set_Employee data_set_employee, string HighestEmployeeLevel)
        {
            List<Data_Set_Mission_Details> mission_data_all = get_mission_information_all();

            /// 更新目前兄弟姊妹單的所有簽核為pass；因簽層的規則改變因此需要。
            foreach (Data_Set_Mission_Details item in mission_data_all)
            {
                string mission_data_ckeys = mission_data.binding_project_id.Substring(0, 16);
                string item_mission_data_ckeys = item.binding_project_id.Substring(0, 16);
                if ((mission_data_ckeys.Equals(item_mission_data_ckeys)) && !(mission_data.m_id.Equals(item.m_id)) && (item.status_id != "5"))
                {
                    foreach (Data_Set_Mission_Collect_Signed item_collect_sign in item.collect_signed)
                    {
                        if (item_collect_sign.employee_level.Equals(HighestEmployeeLevel))
                        {
                            if (item_collect_sign.sign_status.Equals("pass") || item_collect_sign.sign_status.Equals("done") || item_collect_sign.sign_status.Equals("temp_done"))
                            {
                                current_amount += int.Parse(item_collect_sign.extend_data.loan_amount.Replace(",", ""));
                            }
                        }
                    }

                }
            }
            return current_amount;
        }
        public Data_Set_Result update_CollectSignStatus_pass_sibling_rule(Data_Set_Mission_Details mission_data, Data_Set_Employee data_set_employee, string new_require_sign, string new_risk_value)
        {
            List<Data_Set_Mission_Details> mission_data_all = get_mission_information_all();

            Sign_Status sign_status = Sign_Status.Pass;

            /// 計算出目前兄弟姊妹單的總金額
            foreach (Data_Set_Mission_Details item in mission_data_all)
            {
                string mission_data_ckeys = mission_data.binding_project_id.Substring(0, 16);
                string item_mission_data_ckeys = item.binding_project_id.Substring(0, 16);

                /// 同 c_keys 但不同單 並且 單號狀態不可是已建帳
                if ((mission_data_ckeys.Equals(item_mission_data_ckeys)) && !(mission_data.m_id.Equals(item.m_id)) && (item.status_id != "5"))
                {
                    foreach (Data_Set_Mission_Collect_Signed item_collect_sign in item.collect_signed)
                    {
                        if (data_set_employee.employee_level.Equals(item_collect_sign.employee_level))
                        {
                            item_collect_sign.sign_status = sign_status.ToString().ToLower();
                        }
                    }


                    /// 更新兄弟姊妹單的動作 
                    updateCollecSign_SiblingRule(item.m_id, item); //更新兄弟姊妹單的已有簽核
                    update_mission_new_require_sign_rule(item.m_id, new_require_sign); // 更新兄弟姊妹單的新簽核需求
                    Record_Signed_New_RiskValue(item.m_id, new_risk_value); // 更新兄弟姊妹單的目前風險值

                    switch (sign_status)
                    {
                        case Sign_Status.Temp_Done:
                            Record_Sign_Ststus_ID(item.m_id, 39.ToString()); // 紀錄目前狀態
                            break;
                        case Sign_Status.Pass:
                            Record_Sign_Ststus_ID(item.m_id, 2.ToString()); // 紀錄目前狀態
                            break;
                        case Sign_Status.Done:
                            Record_Sign_Ststus_ID(item.m_id, 3.ToString()); // 紀錄目前狀態
                            break;

                    }
                }
            }

            Data_Set_Result result = new Data_Set_Result();
            result.isSuccesed = true;
            result.feedb_back_message = "兄弟姊妹單 更新完畢";

            return result;
        }
        public Data_Set_Result update_CollectSignStatus_done_sibling_rule(Data_Set_Mission_Details mission_data, Data_Set_Employee data_set_employee)
        {
            List<Data_Set_Mission_Details> mission_data_all = get_mission_information_all();

            Sign_Status sign_status = CompareIsTempDoneOrPass(mission_data, data_set_employee);

            /// 計算出目前兄弟姊妹單的總金額
            foreach (Data_Set_Mission_Details item in mission_data_all)
            {
                string mission_data_ckeys = mission_data.binding_project_id.Substring(0, 16);
                string item_mission_data_ckeys = item.binding_project_id.Substring(0, 16);



                /// 同 c_keys 但不同單 並且 單號狀態不可是已建帳
                if ((mission_data_ckeys.Equals(item_mission_data_ckeys)) && !(mission_data.m_id.Equals(item.m_id)) && (item.status_id != "5") && (item.status_id != "0") && (item.status_id != "2"))
                {
                    foreach (Data_Set_Mission_Collect_Signed item_collect_sign in item.collect_signed)
                    {
                        data_set_employee.employee_level = get_employee_level(data_set_employee.e_id, mission_data.mission_type, mission_data.risk_value, mission_data);
                        if (data_set_employee.employee_level.Equals(item_collect_sign.employee_level))
                        {
                            item_collect_sign.sign_status = sign_status.ToString().ToLower();
                        }
                    }


                    /// 更新兄弟姊妹單的動作 
                    updateCollecSign_SiblingRule(item.m_id, item); //更新兄弟姊妹單的已有簽核

                    switch (sign_status)
                    {
                        case Sign_Status.Temp_Done:
                            Record_Sign_Ststus_ID(item.m_id, 39.ToString()); // 紀錄目前狀態
                            break;
                        case Sign_Status.Pass:
                            Record_Sign_Ststus_ID(item.m_id, 2.ToString()); // 紀錄目前狀態
                            break;
                        case Sign_Status.Done:
                            Record_Sign_Ststus_ID(item.m_id, 3.ToString()); // 紀錄目前狀態
                            break;

                    }
                }
            }

            Data_Set_Result result = new Data_Set_Result();
            result.isSuccesed = true;
            result.feedb_back_message = "兄弟姊妹單 更新完畢";

            return result;
        }
        public Data_Set_Result update_CollectSignStatus_back_sibling_rule(Data_Set_Mission_Details mission_data, float current_level)
        {
            List<Data_Set_Mission_Details> mission_data_all = get_mission_information_all();

            /// 計算出目前兄弟姊妹單的總金額
            foreach (Data_Set_Mission_Details item in mission_data_all)
            {
                string mission_data_ckeys = mission_data.binding_project_id.Substring(0, 16);
                string item_mission_data_ckeys = item.binding_project_id.Substring(0, 16);

                /// 同 c_keys 但不同單 並且 單號狀態不可是已建帳
                if ((mission_data_ckeys.Equals(item_mission_data_ckeys)) && !(mission_data.m_id.Equals(item.m_id)) && (item.status_id != "5") && (item.status_id != "0"))
                {
                    foreach (Data_Set_Mission_Collect_Signed item_collect_sign in item.collect_signed)
                    {
                        if (((current_level + 1) == (float.Parse(item_collect_sign.employee_level))))
                        {
                            item_collect_sign.sign_status = "false".ToLower();
                        }
                        else if (current_level == (float.Parse(item_collect_sign.employee_level)))
                        {
                            item_collect_sign.sign_status = "back";
                        }
                    }


                    /// 更新兄弟姊妹單的動作 
                    updateCollecSign_SiblingRule(item.m_id, item); //更新兄弟姊妹單的已有簽核
                    Record_Sign_Ststus_ID(item.m_id, 4.ToString()); // 紀錄目前狀態
                }
            }

            Data_Set_Result result = new Data_Set_Result();
            result.isSuccesed = true;
            result.feedb_back_message = "兄弟姊妹單 更新完畢";

            return result;
        }
        public Data_Set_Result update_CollectSignStatus_fail_sibling_rule(Data_Set_Mission_Details mission_data, Data_Set_Employee data_set_employee)
        {
            List<Data_Set_Mission_Details> mission_data_all = get_mission_information_all();

            /// 計算出目前兄弟姊妹單的總金額
            foreach (Data_Set_Mission_Details item in mission_data_all)
            {
                string mission_data_ckeys = mission_data.binding_project_id.Substring(0, 16);
                string item_mission_data_ckeys = item.binding_project_id.Substring(0, 16);

                /// 同 c_keys 但不同單 並且 單號狀態不可是已建帳
                if ((mission_data_ckeys.Equals(item_mission_data_ckeys)) && !(mission_data.m_id.Equals(item.m_id)) && (item.status_id != "5"))
                {
                    string history_msg = string.Format("因 {0} 被否絕此筆單也被否決。", mission_data.binding_project_id);
                    excute_sign_fail(data_set_employee, item.m_id, history_msg);
                }
            }

            Data_Set_Result result = new Data_Set_Result();
            result.isSuccesed = true;
            result.feedb_back_message = "兄弟姊妹單 更新完畢";

            return result;
        }
        public Data_Set_Result updateCollecSign_SiblingRule(string m_id, Data_Set_Mission_Details mission_data)
        {
            try
            {
                LOANIT_SQL_Plugin sql_plugin = new LOANIT_SQL_Plugin();
                LOANIT_DAO_Plugin dao_plugin = LOANIT_DAO_Plugin.getInstance();
                LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();

                string update_data = string.Format("{0},,,,,{1}", m_id, JsonConvert.SerializeObject(mission_data.collect_signed));

                return dao_plugin.trans_excute_result(sql_plugin.updateCollectSign_SiblingRule(Sql_Action_Category_Option.UPDATE, Sql_Action_Option.UPDATE_COLLECTSIGN_SIBLINGRULE, update_data));
            }
            catch (Exception ex)
            {

                throw new Exception("發生錯誤" + ex.Message);
            }
        }
        public bool get_mission_status_id(List<Data_Set_Sign> list_mission_collect_sign, List<Data_Set_Mission_require> list_mission_require_sign)
        {
            /// 計算每一層級共簽核了幾個，剩下幾個還沒簽。
            foreach (Data_Set_Mission_require mission_require in list_mission_require_sign)
            {
                foreach (Data_Set_Sign mission_signed in list_mission_collect_sign)
                {
                    if (mission_require.company.Equals(mission_signed.company))
                    {
                        if (mission_require.department.Equals(mission_signed.department) || mission_signed.department.Equals("ALL"))
                        {
                            if ((mission_require.employee_level.Equals(mission_signed.employee_level))
                                && (mission_signed.sign_status.Equals("pass") || mission_signed.sign_status.Equals("temp_done") || mission_signed.sign_status.Equals("done")))
                            {
                                mission_require.amount = (int.Parse(mission_require.amount) - 1).ToString();
                            }
                        }
                    }
                }
            }
            foreach (Data_Set_Mission_require mission_require in list_mission_require_sign.ToArray())
            {
                if (int.Parse(mission_require.amount) == 0)
                {
                    list_mission_require_sign.Remove(mission_require);
                }
            }

            switch (list_mission_require_sign.Count)
            {
                case 0:
                    return true;
                default:
                    return false;
            }

        }
        public string get_HighestEmployeeRequireLevel(Data_Set_Mission_Details mission_data)
        {
            List<Data_Set_Mission_Details> mission_data_all = get_mission_information_all();
            int temp_HighestLevel = 4;
            foreach (Data_Set_Mission_Details item in mission_data_all)
            {
                string mission_data_ckeys = mission_data.binding_project_id.Substring(0, 16);
                string item_mission_data_ckeys = item.binding_project_id.Substring(0, 16);
                if ((mission_data_ckeys.Equals(item_mission_data_ckeys)) && !(mission_data.m_id.Equals(item.m_id)) && (item.status_id != "5"))
                {
                    foreach (Data_Set_Mission_require item_require_sign in item.require_sign)
                    {

                        if (int.Parse(item_require_sign.employee_level) < temp_HighestLevel)
                        {
                            temp_HighestLevel = int.Parse(item_require_sign.employee_level);
                        }
                    }

                }
            }
            return temp_HighestLevel.ToString();
        }

        public string get_CurrentAmount_SiblingRule(Data_Set_Mission_Details mission_data, string HighestEmployeeLevel)
        {
            foreach (Data_Set_Mission_Collect_Signed collect_sign in mission_data.collect_signed)
            {
                if (collect_sign.Equals("done") || collect_sign.Equals("temp_done") || collect_sign.Equals("pass"))
                {
                    if (collect_sign.employee_level.Equals(HighestEmployeeLevel))
                    {
                        return collect_sign.extend_data.loan_amount.Replace(",", "");
                    }
                }
            }
            return "0";
        }
        #endregion

        public string get_BackEvent_NewRequireSign(Data_Set_Mission_Details mission_data)
        {
            //List<Data_Set_Mission_require> NewRequireSign = new List<Data_Set_Mission_require>();

            //for (int i = 0; i < mission_data.require_sign.Count - 1; i++)
            //{
            //    NewRequireSign.Add(mission_data.require_sign[i]);
            //}
            //return JsonConvert.SerializeObject(NewRequireSign);
            int count = 0;
            LOANIT loanit = LOANIT.getinstance();
            foreach (Data_Set_Mission_Collect_Signed item in mission_data.collect_signed)
            {
                if (item.sign_status.ToLower().Equals("pass"))
                {
                    count++;
                }
            }
            if(count == 0) { count = 1; }
            return loanit.get_mission_require_signature(count, SubSysNo.CH001);

        }
        public string get_BackEvent_NewRiskValue(Data_Set_Mission_Details mission_data)
        {
            //List<Data_Set_Mission_require> NewRequireSign = new List<Data_Set_Mission_require>();

            //for (int i = 0; i < mission_data.require_sign.Count - 1; i++)
            //{
            //    NewRequireSign.Add(mission_data.require_sign[i]);
            //}
            //return JsonConvert.SerializeObject(NewRequireSign);
            int count = 0;
            LOANIT loanit = LOANIT.getinstance();
            foreach (Data_Set_Mission_Collect_Signed item in mission_data.collect_signed)
            {
                if (item.sign_status.ToLower().Equals("pass"))
                {
                    count++;
                }
            }
            if (count == 0) { count = 1; }
            return count.ToString();

        }

        public override string Compare_New_Risk(string amount)
        {
            amount = amount.Replace(",", "");
            int compare_amount = int.Parse(amount);
            List<Data_Set_Risk_Data_Reference> list_risk_reference_data = get_risk_amount_reference_data();
            List<Data_Set_Risk_Amount_Reference> list_risk_amount_reference_data = new List<Data_Set_Risk_Amount_Reference>();

            foreach (Data_Set_Risk_Data_Reference item in list_risk_reference_data)
            {
                if (item.risk_type.Equals("Loanit_Amount"))
                {
                    Data_Set_Risk_Amount_Reference risk_amount_reference_data = new Data_Set_Risk_Amount_Reference();
                    risk_amount_reference_data.max_amount = item.max_data;
                    risk_amount_reference_data.min_amount = item.min_data;
                    risk_amount_reference_data.risk_value = item.risk_value;
                    list_risk_amount_reference_data.Add(risk_amount_reference_data);
                }
            }

            foreach (Data_Set_Risk_Amount_Reference risk_amount_reference_data in list_risk_amount_reference_data)
            {
                if (risk_amount_reference_data.min_amount <= compare_amount && compare_amount <= risk_amount_reference_data.max_amount)
                {
                    return ("true," + risk_amount_reference_data.risk_value);
                }
            }

            return ("fasle" + 0.ToString());
        }
        public string Compare_New_Risk(string amount, string processratio)
        {
            amount = amount.Replace(",", "");
            decimal compare_processratio = ConvertToDecimal(processratio);


            int compare_amount = int.Parse(amount);
            int max_risk_value = 0; ///最大的風險值
            List<Data_Set_Risk_Data_Reference> list_risk_reference_data = get_risk_amount_reference_data();
            List<Data_Set_Risk_Amount_Reference> list_risk_amount_reference_data = new List<Data_Set_Risk_Amount_Reference>();
            List<Data_Set_Risk_ProcessRratio_Reference> list_risk_process_ratio = new List<Data_Set_Risk_ProcessRratio_Reference>();

            foreach (Data_Set_Risk_Data_Reference item in list_risk_reference_data)
            {
                if (item.risk_type.Equals("Loanit_Amount"))
                {
                    Data_Set_Risk_Amount_Reference risk_amount_reference_data = new Data_Set_Risk_Amount_Reference();
                    risk_amount_reference_data.max_amount = item.max_data;
                    risk_amount_reference_data.min_amount = item.min_data;
                    risk_amount_reference_data.risk_value = item.risk_value;
                    list_risk_amount_reference_data.Add(risk_amount_reference_data);
                }
                else if (item.risk_type.Equals("Loanit_ProcessRatio"))
                {
                    Data_Set_Risk_ProcessRratio_Reference risk_processratio_reference_data = new Data_Set_Risk_ProcessRratio_Reference();
                    risk_processratio_reference_data.max_ratio = item.max_data;
                    risk_processratio_reference_data.min_ratio = item.min_data;
                    risk_processratio_reference_data.risk_value = item.risk_value;
                    list_risk_process_ratio.Add(risk_processratio_reference_data);
                }
            }

            /// 貸款金額
            foreach (Data_Set_Risk_Amount_Reference risk_amount_reference_data in list_risk_amount_reference_data)
            {
                if (risk_amount_reference_data.min_amount <= compare_amount && compare_amount <= risk_amount_reference_data.max_amount)
                {
                    max_risk_value = int.Parse(risk_amount_reference_data.risk_value);
                }
            }

            /// 處置總債比
            foreach (Data_Set_Risk_ProcessRratio_Reference risk_processratio_reference_data in list_risk_process_ratio)
            {
                if (decimal.Parse(risk_processratio_reference_data.min_ratio.ToString()) <= compare_processratio && compare_processratio <= decimal.Parse(risk_processratio_reference_data.max_ratio.ToString()))
                {
                    if (max_risk_value < decimal.Parse(risk_processratio_reference_data.risk_value))
                    {
                        max_risk_value = int.Parse(risk_processratio_reference_data.risk_value);
                    }

                }
            }

            return ("true," + max_risk_value.ToString());
        }
        public string get_mission_signing_status_forVision(Data_Set_Mission_Details mission_data)
        {
            filter_Mission_Done_RequireAmount(mission_data);
            List<Data_Set_Mission_require> list_require_sign_data = mission_data.require_sign;

            mission_data.require_sign.Sort((x, y) => -x.employee_level.CompareTo(y.employee_level));
            foreach (Data_Set_Mission_require require_sign_data in mission_data.require_sign)
            {
                return require_sign_data.employee_level;
            }

            /// 如果都沒有需要簽核的，直接返回0
            return "0";

        }
        public string get_mission_curent_amount(Data_Set_Mission_Details mission_data)
        {
            string current_amount = "0";
            foreach (Data_Set_Mission_Collect_Signed collect_signed_data in mission_data.collect_signed)
            {
                if (!collect_signed_data.public_key.Equals(""))
                {
                    current_amount = collect_signed_data.extend_data.loan_amount;
                }
            }
            if (current_amount != null)
            {
                return current_amount.Replace(",", "");
            }
            else
            {
                return "0";
            }

        }
        public string get_mission_curent_PrepayMonth(Data_Set_Mission_Details mission_data)
        {
            string current_PrepayMonth = "0";
            foreach (Data_Set_Mission_Collect_Signed collect_signed_data in mission_data.collect_signed)
            {
                if (collect_signed_data != null && !collect_signed_data.public_key.Equals(""))
                {
                    current_PrepayMonth = collect_signed_data.extend_data.prepay_month;
                }
            }
            return current_PrepayMonth.Replace(",", "");
        }
        public string get_mission_curent_rate(Data_Set_Mission_Details mission_data)
        {
            string current_rate = "";
            foreach (Data_Set_Mission_Collect_Signed collect_signed_data in mission_data.collect_signed)
            {
                if (!collect_signed_data.public_key.Equals(""))
                {
                    current_rate = collect_signed_data.extend_data.loan_rate;
                }
            }
            return current_rate;
        }
        public int get_mission_PrepaidPeriod(Data_Set_Mission_Details mission_data)
        {
            string current_month = "0";
            foreach (Data_Set_Mission_Collect_Signed collect_signed_data in mission_data.collect_signed)
            {
                if (!collect_signed_data.public_key.Equals(""))
                {
                    current_month = collect_signed_data.extend_data.prepay_month;
                }
            }
            return int.Parse(current_month);
        }
        public string get_mission_curent_processratio(Data_Set_Mission_Details mission_data)
        {
            string current_processratio = "";
            foreach (Data_Set_Mission_Collect_Signed collect_signed_data in mission_data.collect_signed)
            {
                if (!collect_signed_data.public_key.Equals(""))
                {
                    current_processratio = collect_signed_data.extend_data.loan_process_ratio;
                }
            }
            return current_processratio;
        }
        public List<Data_Set_Risk_Data_Reference> get_risk_amount_reference_data()
        {
            LOANIT_SQL_Plugin sql_plugin = new LOANIT_SQL_Plugin();
            LOANIT_DAO_Plugin dao_plugin = LOANIT_DAO_Plugin.getInstance();

            return dao_plugin.set_risk_reference_data_loanit(sql_plugin.get_risk_reference_data(Sql_Action_Category_Option.GET, Sql_Action_Option.GET_RISK_REFERENCE_DATA));
        }
        public void Record_BackNewRequirSign(string value)
        {
            LOANIT_SQL_Plugin sql_plugin = new LOANIT_SQL_Plugin();
            sql_plugin.Record_BackNewRequirSign(Sql_Action_Category_Option.SET, Sql_Action_Option.UPDATE_BACK_REQUIRESIGN, value);
        }
        


        public string get_missionStatusDescrption(string status_id)
        {
            switch (status_id)
            {
                case "0":
                    return "尚未簽核";
                case "2":
                    return "簽核中";
                case "3":
                    return "已核定/可建帳";
                case "4":
                    return "退回重簽中";
                case "5":
                    return "已建帳";
                case "39":
                    return "類核定中";
                case "99":
                    return "已駁回";
                default:
                    return "找不到指定的代號";
            }
        }
        public void insert_createdaccount(string c_keys)
        {
            Controller_Sign sign = Controller_Sign.getInstance();
            Data_Set_Excutre_Result result = sign.CompareTokenvalid();
            if (result.excute_result.isSuccesed)
            {
                try
                {
                    LOANIT loanit = LOANIT.getinstance();

                    loanit.initail_mission_object();
                    loanit.data_set_mission.status_id = "51";
                    loanit.data_set_mission.require_sign_json = JsonConvert.SerializeObject(new List<Data_Set_Mission_require>());
                    loanit.data_set_mission.collect_signed_json = JsonConvert.SerializeObject(new List<Data_Set_Mission_Collect_Signed>());
                    loanit.set_mission_object_data(Set_Mission_Data_Option.NAME, "帳務系統已建帳之資料");
                    loanit.set_mission_object_data(Set_Mission_Data_Option.TYPE, "CH003");
                    loanit.set_mission_object_data(Set_Mission_Data_Option.COMPANY, "LOANIT");
                    loanit.set_mission_object_data(Set_Mission_Data_Option.BINDING_PROJECT_ID, c_keys);
                    loanit.set_mission_object_data(Set_Mission_Data_Option.RISK_VALUE, "");
                    //loanit.set_mission_object_data(Set_Mission_Data_Option.require_SIGNATURE, loanit.get_mission_require_signature(5)); //填入風險係數

                    Data_Set_Excutre_Result excute_result = JsonConvert.DeserializeObject<Data_Set_Excutre_Result>(sign.register_new_mission(loanit));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("錯誤\r\n原因為：{0}", ex.Message));
                }
            }
            else
            {
                MessageBox.Show(String.Format("{0}\r\n", result.excute_result.feedb_back_message));
            }
        }
        public decimal ConvertToDecimal(string processratio)
        {
            string processratio_str = processratio.Trim();
            try
            {
                if (processratio != string.Empty & processratio != "" && processratio != null)
                {
                    return decimal.Parse(processratio_str);
                }
                else
                {

                    return decimal.Parse(processratio_str);
                }
            }
            catch (Exception)
            {

                try
                {

                    return decimal.Parse(processratio);
                }
                catch (Exception)
                {

                    return decimal.Parse("0");
                }
            }



        }


        public Sign_Status CompareIsTempDoneOrPass(Data_Set_Mission_Details mission_data, Data_Set_Employee data_set_employee)
        {
            ApprovalLevel_Controller approvalLevel_Controller = ApprovalLevel_Controller.getInstance();
            int count = 0;

            List<Data_Set_Mission_Details> mission_data_all = get_mission_information_all();
            foreach (Data_Set_Mission_Details item in mission_data_all)
            {
                string mission_data_ckeys = mission_data.binding_project_id.Substring(0, 16);
                string item_mission_data_ckeys = item.binding_project_id.Substring(0, 16);

                /// 同 c_keys 但不同單 並且 單號狀態不可是已建帳
                if ((mission_data_ckeys.Equals(item_mission_data_ckeys)) && !(mission_data.m_id.Equals(item.m_id)) && (item.status_id != "5"))
                {
                    if (item.status_id.Equals("39"))
                    {
                        count += 1;
                    }
                }
            }

            if ((count + 1).ToString().Equals("3")) /// 這3之後必須改成 居正的兄弟單欄位
            //if ((count + 1).ToString().Equals(approvalLevel_Controller.getTotalCaseNum(mission_data.binding_project_id)))
            {
                data_set_employee.employee_level = get_employee_level(data_set_employee.e_id, mission_data.mission_type, mission_data.risk_value, mission_data);
                if (CompareHeighestLevel(mission_data, data_set_employee))
                {
                    return Sign_Status.Done;
                }
                else
                {
                    return Sign_Status.Pass;
                }
            }
            else
            {
                return Sign_Status.Temp_Done;
            }
        }
        public bool CompareHeighestLevel(Data_Set_Mission_Details mission_data, Data_Set_Employee data_set_employee)
        {
            if (mission_data.require_sign.Count == 0 || mission_data.require_sign[0].employee_level.Equals(data_set_employee.employee_level))
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        public List<Data_Set_Mission_Details> filter_Mission_Done_RequireAmount(List<Data_Set_Mission_Details> list_mission_data)
        {

            foreach (Data_Set_Mission_Details mission_data in list_mission_data)
            {
                /// 計算每一層級共簽核了幾個，剩下幾個還沒簽。
                foreach (Data_Set_Mission_require mission_require in mission_data.require_sign)
                {
                    foreach (Data_Set_Mission_Collect_Signed mission_signed in mission_data.collect_signed)
                    {
                        if (mission_require.company.Equals(mission_signed.company))
                        {
                            if (mission_require.department.Equals(mission_signed.department) || mission_signed.department.Equals("ALL"))
                            {
                                if (mission_require.employee_level.Equals(mission_signed.employee_level))
                                {
                                    if (mission_signed.sign_status.Equals("pass") || mission_signed.sign_status.Equals("temp_done") || mission_signed.sign_status.Equals("done"))
                                    {
                                        mission_require.amount = (int.Parse(mission_require.amount) - 1).ToString();
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (Data_Set_Mission_require mission_require in mission_data.require_sign.ToArray())
                {
                    if ((int.Parse(mission_require.amount) == 0))
                    {
                        mission_data.require_sign.Remove(mission_require);
                    }
                }
            }
            return list_mission_data;
        }
        public string filter_Mission_Done_RequireAmount(Data_Set_Mission_Details mission_data)
        {
            /// 計算每一層級共簽核了幾個，剩下幾個還沒簽。
            foreach (Data_Set_Mission_require mission_require in mission_data.require_sign)
            {
                foreach (Data_Set_Mission_Collect_Signed mission_signed in mission_data.collect_signed)
                {
                    if (mission_require.company.Equals(mission_signed.company))
                    {
                        if (mission_require.department.Equals(mission_signed.department) || mission_signed.department.Equals("ALL"))
                        {
                            if (mission_require.employee_level.Equals(mission_signed.employee_level))
                            {
                                if (mission_signed.sign_status.Equals("pass") || mission_signed.sign_status.Equals("temp_done") || mission_signed.sign_status.Equals("done"))
                                {
                                    mission_require.amount = (int.Parse(mission_require.amount) - 1).ToString();
                                }
                            }
                        }
                    }
                }
            }
            foreach (Data_Set_Mission_require mission_require in mission_data.require_sign.ToArray())
            {
                if ((int.Parse(mission_require.amount) == 0))
                {
                    mission_data.require_sign.Remove(mission_require);
                }
            }

            return mission_data.require_sign.Count.ToString();
        }

        public bool get_compare_level_sequence(Data_Set_Employee employee_data, List<Data_Set_Mission_Collect_Signed> list_mission_collect_signature, List<Data_Set_Mission_require> list_mission_require_signature)
        {
            ///// 判斷是否包含他的簽名
            //foreach (Data_Set_Mission_Collect_Signed collect_signed in list_mission_collect_signature)
            //{
            //    if ((collect_signed.public_key.Equals(employee_data.publickey) && (collect_signed.sign_status.Equals("pass")) || collect_signed.sign_status.Equals("temp_done")) )
            //    {

            //        return false;

            //    }
            //}

            /// 判別是否換此職員簽核
            foreach (Data_Set_Mission_require mission_require in list_mission_require_signature)
            {
                //Console.WriteLine("Require_Level " + mission_require.employee_level);
                //Console.WriteLine("Employee_Level " + employee_data.employee_level);

                if (float.Parse(mission_require.employee_level) == float.Parse(employee_data.employee_level))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public void ClearTempData()
        {
            Temp_ALL_Mission = null;
            Temp_Fail_Mission = null;
            Temp_RequireDone_Mission = null;
            Temp_RequireSign_Mission = null;
        }
        public string getCustomerName(string c_key)
        {
            List<CKeyAndName> temp = getCustomerNameData_Reference();
            foreach (CKeyAndName item in temp)
            {
                if (item.CKey.Equals(c_key))
                {
                    return item.Name;
                }
            }
            return "找不到此客戶姓名";

        }

        

        public List<Data_Set_Employee> get_EmployeeData_All()
        {
            throw new Exception("還沒寫");
        }
        public List<CKeyAndName> getCustomerNameData_Reference()
        {
            ApprovalLevel_Controller approvalLevel_Controller = ApprovalLevel_Controller.getInstance();

            if (Temp_CkeyAndNameData != null)
            {
                return Temp_CkeyAndNameData;
            }
            else
            {
                Temp_CkeyAndNameData = approvalLevel_Controller.getAllCustomerCKeyAndName();
                return Temp_CkeyAndNameData;

            }
        }



        #region 封裝化
        public static LOANIT_CONTROLLER_Plugin instance = new LOANIT_CONTROLLER_Plugin();
        public static LOANIT_CONTROLLER_Plugin getInstance()
        {
            return instance;
        }
        private LOANIT_CONTROLLER_Plugin()
        {
            Current_Token = new Date_Set_Login();
            Load_Employee_Level_Reference();
        }
        #endregion



    }




    public class LOANIT_AccountSignSys_Controller
    {
        public Data_Set_Result update_AccountToSignSys_mission_statusid(string Company, string missiontype_category, string binding_project_id)
        {
            LOANIT_SQL_Plugin sql_plugin = new LOANIT_SQL_Plugin();
            LOANIT_DAO_Plugin dao_plugin = LOANIT_DAO_Plugin.getInstance();
            string update_AccountToSignSys_Mission_Statusid = string.Format("{0},,,,,{1},,,,,{2},,,,,{3}", binding_project_id, 5.ToString(), Company, missiontype_category);
            return dao_plugin.trans_excute_result(sql_plugin.update_AccountToSignSys_mission_statusid(Sql_Action_Category_Option.UPDATE, Sql_Action_Option.UPDATE_ACCOUNTTOSIGN_MISSION_STATUSID, update_AccountToSignSys_Mission_Statusid));
        }


        #region 封裝


        private LOANIT_AccountSignSys_Controller()
        {

        }

        public static LOANIT_AccountSignSys_Controller instance = new LOANIT_AccountSignSys_Controller();
        public static LOANIT_AccountSignSys_Controller getinstance()
        {
            return instance;
        }


        #endregion

    }
}
