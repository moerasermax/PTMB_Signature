using Newtonsoft.Json;
using PTMB_Signature.Model.Data_Set;
using PTMB_Signature_API.Data_Set;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTMB_Signature.Model.DAO
{
    public class LOANIT_DAO_Plugin
    {
        
        #region 設置資料區
        public List<Data_Set_Mission_Employee_Level_LOANIT> set_mission_employee_level_loanit(Data_Set_DAO_Data dao_data)
        {
            List<Data_Set_Mission_Employee_Level_LOANIT> data_set_mission_employee_result = JsonConvert.DeserializeObject<List<Data_Set_Mission_Employee_Level_LOANIT>>(dao_data.excute_result_json);
            return data_set_mission_employee_result;
        }
        public List<Data_Set_Employee_Account_LOANIT_ForBackStage> set_mission_employee_level_loanit_ForBackStage(Data_Set_DAO_Data dao_data)
        {
            List<Data_Set_Employee_Account_LOANIT_ForBackStage> data_set_mission_employee_result = JsonConvert.DeserializeObject<List<Data_Set_Employee_Account_LOANIT_ForBackStage>>(dao_data.excute_result_json);
            return data_set_mission_employee_result;
        }
        public List<Data_Set_Risk_Data_Reference> set_risk_reference_data_loanit(Data_Set_DAO_Data dao_data)
        {
            List<Data_Set_Risk_Data_Reference> data_set_risk_reference_result = JsonConvert.DeserializeObject<List<Data_Set_Risk_Data_Reference>>(dao_data.excute_result_json);
            return data_set_risk_reference_result;
        }
        public Data_Set_Result trans_excute_result(Data_Set_DAO_Data dao_data)
        {
            Data_Set_Result excute_result = JsonConvert.DeserializeObject<Data_Set_Result>(dao_data.excute_result_json);
            return excute_result;
        }

        #endregion


        #region 轉換區
        public Data_Set_DAO_Data trans_employee_GA_level(DataTable dt)
        {
            Data_Set_DAO_Data dao_data = new Data_Set_DAO_Data();
            Data_Set_Mission_Employee_Level employee_data = new Data_Set_Mission_Employee_Level();

            if (!dt.Rows[0]["excute_result"].ToString().Contains("Fail"))
            {

                    Data_Set_Mission_Employee_Level single_mission_employee = new Data_Set_Mission_Employee_Level();
                    single_mission_employee.mission_type = dt.Rows[0]["mission_type"].ToString();
                    single_mission_employee.employee_level = dt.Rows[0]["employee_level"].ToString();
                    single_mission_employee.account_list = JsonConvert.DeserializeObject<List<Data_Set_Employee_Account>>(dt.Rows[0]["account_list"].ToString());

                    single_mission_employee.excute_result = new Data_Set_Result();
                    single_mission_employee.excute_result.feedb_back_message = dt.Rows[0]["excute_result"].ToString();
                    single_mission_employee.excute_result.fail_reason = "";
                    single_mission_employee.excute_result.result = "查詢成功";
                    single_mission_employee.excute_result.isSuccesed = true;
                    single_mission_employee.excute_result.isError = false;

                    employee_data = single_mission_employee;
                
            }
            else
            {
                Data_Set_Mission_Employee_Level single_mission_employee = new Data_Set_Mission_Employee_Level();

                single_mission_employee.excute_result = new Data_Set_Result();
                single_mission_employee.excute_result.feedb_back_message = "發生問題，請聯繫【研發中心-郁宸】";
                single_mission_employee.excute_result.fail_reason = dt.Rows[0]["excute_result"].ToString();
                single_mission_employee.excute_result.result = "查詢成功";
                single_mission_employee.excute_result.isSuccesed = true;
                single_mission_employee.excute_result.isError = false;

            }
            dao_data.excute_result_json = JsonConvert.SerializeObject(employee_data);
            return dao_data;
        }
        public Data_Set_DAO_Data trans_mission_employee_information(DataTable dt)
        {
            Data_Set_DAO_Data dao_data = new Data_Set_DAO_Data();
            List<Data_Set_Mission_Employee_Level_LOANIT> MissionEmployeeDataList = new List<Data_Set_Mission_Employee_Level_LOANIT>();
            if (!dt.Rows[0]["excute_result"].ToString().Contains("Fail") && dt.Rows.Count > 1)
            {
                for (int i = 1; i <= dt.Rows.Count - 1; i++)
                {
                    Data_Set_Mission_Employee_Level_LOANIT single_mission_employee = new Data_Set_Mission_Employee_Level_LOANIT();
                    single_mission_employee.mission_type = dt.Rows[i]["mission_type"].ToString();
                    single_mission_employee.employee_level = dt.Rows[i]["employee_level"].ToString();
                    single_mission_employee.account_list = JsonConvert.DeserializeObject<List<Data_Set_Employee_Account_LOANIT>>(dt.Rows[i]["account_list"].ToString());
                    foreach (Data_Set_Employee_Account_LOANIT item in single_mission_employee.account_list)
                    {
                        item.mission_type = dt.Rows[i]["mission_type"].ToString();
                        item.employee_level = dt.Rows[i]["employee_level"].ToString();
                    }

                    single_mission_employee.excute_result = new Data_Set_Result();
                    single_mission_employee.excute_result.feedb_back_message = dt.Rows[0]["excute_result"].ToString();
                    single_mission_employee.excute_result.fail_reason = "";
                    single_mission_employee.excute_result.result = "查詢成功";
                    single_mission_employee.excute_result.isSuccesed = true;
                    single_mission_employee.excute_result.isError = false;

                    MissionEmployeeDataList.Add(single_mission_employee);
                }
            }
            else
            {
                Data_Set_Mission_Employee_Level single_mission_employee = new Data_Set_Mission_Employee_Level();

                single_mission_employee.excute_result = new Data_Set_Result();
                single_mission_employee.excute_result.feedb_back_message = "查詢時發生問題，請聯絡【研發中心-郁宸】";
                single_mission_employee.excute_result.fail_reason = dt.Rows[0]["excute_result"].ToString();
                single_mission_employee.excute_result.result = "查詢失敗";
                single_mission_employee.excute_result.isSuccesed = false;
                single_mission_employee.excute_result.isError = true;
            }

            dao_data.excute_result_json = JsonConvert.SerializeObject(MissionEmployeeDataList);
            return dao_data;
        }
        public Data_Set_DAO_Data trans_mission_employee_information_ForBackStage(DataTable dt)
        {
            Data_Set_DAO_Data dao_data = new Data_Set_DAO_Data();
            List<Data_Set_Employee_Account_LOANIT_ForBackStage> MissionEmployeeDataList = new List<Data_Set_Employee_Account_LOANIT_ForBackStage>();
            if (!dt.Rows[0]["excute_result"].ToString().Contains("Fail") && dt.Rows.Count > 1)
            {
                for (int i = 1; i <= dt.Rows.Count - 1; i++)
                {
                    Data_Set_Employee_Account_LOANIT_ForBackStage single_mission_employee = new Data_Set_Employee_Account_LOANIT_ForBackStage();
                    single_mission_employee.mission_type = dt.Rows[i]["mission_type"].ToString();
                    single_mission_employee.employee_level = dt.Rows[i]["employee_level"].ToString();
                    single_mission_employee.AccountList = JsonConvert.DeserializeObject<List<MainSysAccount>>(dt.Rows[i]["account_list"].ToString());

                    single_mission_employee.excute_result = new Data_Set_Result();
                    single_mission_employee.excute_result.feedb_back_message = dt.Rows[0]["excute_result"].ToString();
                    single_mission_employee.excute_result.fail_reason = "";
                    single_mission_employee.excute_result.result = "查詢成功";
                    single_mission_employee.excute_result.isSuccesed = true;
                    single_mission_employee.excute_result.isError = false;

                    MissionEmployeeDataList.Add(single_mission_employee);
                }
            }
            else
            {
                Data_Set_Mission_Employee_Level single_mission_employee = new Data_Set_Mission_Employee_Level();

                single_mission_employee.excute_result = new Data_Set_Result();
                single_mission_employee.excute_result.feedb_back_message = "查詢時發生問題，請聯絡【研發中心-郁宸】";
                single_mission_employee.excute_result.fail_reason = dt.Rows[0]["excute_result"].ToString();
                single_mission_employee.excute_result.result = "查詢失敗";
                single_mission_employee.excute_result.isSuccesed = false;
                single_mission_employee.excute_result.isError = true;
            }

            dao_data.excute_result_json = JsonConvert.SerializeObject(MissionEmployeeDataList);
            return dao_data;
        }

        public Data_Set_DAO_Data trans_risk_reference_information(DataTable dt)
        {
            Data_Set_DAO_Data dao_data = new Data_Set_DAO_Data();
            List<Data_Set_Risk_Data_Reference> list_risk_reference_data = new List<Data_Set_Risk_Data_Reference>();
            if (!dt.Rows[0]["excute_result"].ToString().Contains("Fail") && dt.Rows.Count >1)
            {
                for(int i = 1; i < dt.Rows.Count; i++)
                {
                    Data_Set_Risk_Data_Reference single_risk_reference = new Data_Set_Risk_Data_Reference();
                    single_risk_reference.max_data = decimal.Parse(dt.Rows[i]["max_data"].ToString());
                    single_risk_reference.min_data = decimal.Parse(dt.Rows[i]["min_data"].ToString());
                    single_risk_reference.risk_value = dt.Rows[i]["risk_value"].ToString();
                    single_risk_reference.risk_type = dt.Rows[i]["risk_type"].ToString();

                    list_risk_reference_data.Add(single_risk_reference);
                }
            }
            else
            {
                Data_Set_Mission_Employee_Level single_mission_employee = new Data_Set_Mission_Employee_Level();

                single_mission_employee.excute_result = new Data_Set_Result();
                single_mission_employee.excute_result.feedb_back_message = "查詢時發生問題，請聯絡【研發中心-郁宸】";
                single_mission_employee.excute_result.fail_reason = dt.Rows[0]["excute_result"].ToString();
                single_mission_employee.excute_result.result = "查詢失敗";
                single_mission_employee.excute_result.isSuccesed = false;
                single_mission_employee.excute_result.isError = true;
            }

            dao_data.excute_result_json = JsonConvert.SerializeObject(list_risk_reference_data);
            return dao_data;
        }
        #endregion





        private LOANIT_DAO_Plugin()
        {

        }

        public static LOANIT_DAO_Plugin instance = new LOANIT_DAO_Plugin();
        public static LOANIT_DAO_Plugin getInstance()
        {
            return instance;
        }


    }
}
