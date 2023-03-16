using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PTMB_Signature_API.Data_Set;
using PTMB_Signature_API.Model.Implement;
using PTMB_Signature.Model.DAO;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using PTMB_Signature.Model.Data_Set;

namespace PTMB_Signature.Model
{
    public class LOANIT_SQL_Plugin : SQL
    {
        public Data_Set_DAO_Data get_GA_Level(Sql_Action_Category_Option category_option, Sql_Action_Option action_option,string value)
        {
            //// 應該可以拿掉不需要這個
            /// Value -> x1,,,,,x2,,,,,
            /// x1 -> risk":"4"
            /// x2 -> P0006

            LOANIT_DAO_Plugin loanit_dao_sql = LOANIT_DAO_Plugin.getInstance();

            using (SqlConnection conn = get_Sql_Load_Connection(Formal_MainSystem))
            {
                string cmd_str = string.Format("select *   FROM [Signature].[dbo].[employee_level] where account_list like '%\"risk\":\"{0}\"%' and account_list like '%\"{1}\"%'", "@risk", "@account");
                SqlCommand cmd = get_Sql_Command(conn, cmd_str);
                set_Sql_Parametert(action_option, cmd, value);
                return loanit_dao_sql.trans_employee_GA_level(excute_Sql_cmd(category_option, conn, cmd));
            }
        }
        public Data_Set_DAO_Data get_mission_employee_data_loanit(Sql_Action_Category_Option category_option, Sql_Action_Option action_option)
        {
            LOANIT_DAO_Plugin loanit_dao_sql = LOANIT_DAO_Plugin.getInstance();
            Data_Set_Excutre_Result ss = new Data_Set_Excutre_Result();
            using (SqlConnection conn = get_Sql_Load_Connection(Formal_MainSystem))
            {
                string cmd_str = string.Format("SELECT * FROM [Signature].[dbo].[employee_level]");
                SqlCommand cmd = get_Sql_Command(conn, cmd_str);
                return loanit_dao_sql.trans_mission_employee_information(excute_Sql_cmd(category_option, conn, cmd));
            }
        }
        public Data_Set_DAO_Data get_mission_employee_data_loanit_ForBackStage(Sql_Action_Category_Option category_option, Sql_Action_Option action_option)
        {
            LOANIT_DAO_Plugin loanit_dao_sql = LOANIT_DAO_Plugin.getInstance();
            Data_Set_Excutre_Result ss = new Data_Set_Excutre_Result();
            using (SqlConnection conn = get_Sql_Load_Connection(Formal_MainSystem))
            {
                string cmd_str = string.Format("SELECT * FROM [Signature].[dbo].[employee_level]");
                SqlCommand cmd = get_Sql_Command(conn, cmd_str);
                return loanit_dao_sql.trans_mission_employee_information_ForBackStage(excute_Sql_cmd(category_option, conn, cmd));
            }
        }

        public Data_Set_DAO_Data set_register_new_mission_type(Sql_Action_Category_Option category_option, Sql_Action_Option action_option, string value)
        {
            using (SqlConnection conn = get_Sql_Load_Connection(Formal_MainSystem))
            {
                string cmd_str = string.Format("Insert INTO [Signature].[dbo].[mission_type_information] (company,department,mission_type_id,summary) VALUES ({0},{1},{2},{3})", "@company", "@department", "@mission_type_id", "@summary");
                SqlCommand cmd = get_Sql_Command(conn, cmd_str);
                set_Sql_Parametert(action_option, cmd, value);
                return dao_sql.trans_register_mission_type(excute_Sql_cmd(category_option, conn, cmd));
            }

        }
        public Data_Set_DAO_Data get_risk_reference_data(Sql_Action_Category_Option category_optiopn, Sql_Action_Option Sql_Action_Option) 
        {
            LOANIT_DAO_Plugin dao_plugin = LOANIT_DAO_Plugin.getInstance();
            using(SqlConnection conn = get_Sql_Load_Connection(Formal_MainSystem))
            {
                string cmd_str = string.Format("Select * FROM [Signature].[dbo].[loanit_risk_reference]");
                SqlCommand cmd = get_Sql_Command(conn,cmd_str);
                return dao_plugin.trans_risk_reference_information(excute_Sql_cmd(category_optiopn, conn, cmd));
            }
        }
        public Data_Set_DAO_Data update_AccountToSignSys_mission_statusid(Sql_Action_Category_Option category_option, Sql_Action_Option action_option,string value)
        {
            LOANIT_DAO_Plugin dao_plugin = LOANIT_DAO_Plugin.getInstance();
            using(SqlConnection conn = get_Sql_Load_Connection(Formal_MainSystem))
            {
                string cmd_str = string.Format("UPDATE [Signature].[dbo].[mission_information] SET status_id = @status_id where company = @company AND mission_type LIKE @mission_type AND binding_project_id = @binding_project_id ");
                SqlCommand cmd = get_Sql_Command(conn, cmd_str);
                set_Sql_Parametert(action_option, cmd, value);
                return dao_sql.trans_excute_result(excute_Sql_cmd(category_option, conn, cmd));
            }

        }
        public Data_Set_DAO_Data updateCollectSign_SiblingRule(Sql_Action_Category_Option category_option, Sql_Action_Option action_option, string value)
        {
            using(SqlConnection conn = get_Sql_Load_Connection(Formal_MainSystem))
            {
                string cmd_str = string.Format("Update [Signature].[dbo].[mission_information] SET collect_signed = @collect_signed Where m_id=@m_id");
                SqlCommand cmd = get_Sql_Command(conn,cmd_str);
                set_Sql_Parametert(action_option, cmd , value);
                return dao_sql.trans_excute_result(excute_Sql_cmd(category_option, conn, cmd));
            }
        }

        public void Record_BackNewRequirSign(Sql_Action_Category_Option category_option, Sql_Action_Option action_option, string value)
        {
            using(SqlConnection conn = get_Sql_Load_Connection(Formal_MainSystem))
            {
                string cmd_str = string.Format("Update [Signature].[dbo].[mission_information] SET require_sign = @require_sign WHERE m_id = @m_id");
                SqlCommand cmd = get_Sql_Command(conn, cmd_str);
                set_Sql_Parametert(action_option, cmd, value);
                excute_Sql_cmd(category_option, conn, cmd);
            }

        }

        


        public Data_Set_DAO_Data Update_EmployeeAccountLevel_Data(Sql_Action_Category_Option category_option, Sql_Action_Option action_option, string value)
        {
            using(SqlConnection conn = get_Sql_Load_Connection(Formal_MainSystem))
            {
                string cmd_str = "Update [Signature].[dbo].[employee_level] SET account_list = @account_list where mission_type = @mission_type AND employee_level = @employee_level";
                SqlCommand cmd = get_Sql_Command(conn, cmd_str);
                set_Sql_Parametert(action_option, cmd, value);
                return dao_sql.trans_excute_result(excute_Sql_cmd(category_option,conn,cmd));
            }
        }

        [Obsolete]
        public SqlCommand set_Sql_Parametert(Sql_Action_Option action_option, SqlCommand cmd, string value)
        {
            switch (action_option)
            {
                case Sql_Action_Option.GET_EMPLOYEE_LEVEL_GA:
                    string[] employee_ga_data = Regex.Split(value, ",,,,,");
                    cmd.Parameters.AddWithValue("@account", employee_ga_data[0]);
                    cmd.Parameters.AddWithValue("@risk", employee_ga_data[1]);
                    return cmd;
                case Sql_Action_Option.UPDATE_ACCOUNTTOSIGN_MISSION_STATUSID:
                    string[] Update_AccountToSignSys_MissionStatusID_Arr = Regex.Split(value, ",,,,,");
                    cmd.Parameters.AddWithValue("@binding_project_id", Update_AccountToSignSys_MissionStatusID_Arr[0]);
                    cmd.Parameters.AddWithValue("@status_id", Update_AccountToSignSys_MissionStatusID_Arr[1]);
                    cmd.Parameters.AddWithValue("@company", Update_AccountToSignSys_MissionStatusID_Arr[2]);
                    cmd.Parameters.AddWithValue("@mission_type", Update_AccountToSignSys_MissionStatusID_Arr[3]);
                    return cmd;
                case Sql_Action_Option.UPDATE_COLLECTSIGN_SIBLINGRULE:
                    string[] Update_CollectSign_SiblingRule = Regex.Split(value, ",,,,,");
                    cmd.Parameters.AddWithValue("@m_id", Update_CollectSign_SiblingRule[0]);
                    cmd.Parameters.AddWithValue("@collect_signed", Update_CollectSign_SiblingRule[1]);
                    return cmd;
                case Sql_Action_Option.UPDATE_BACK_REQUIRESIGN:
                    string[] Update_RequireSign_DataArr = Regex.Split(value, ",,,,,");
                    cmd.Parameters.Add("@m_id", Update_RequireSign_DataArr[0]);
                    cmd.Parameters.Add("@require_sign", Update_RequireSign_DataArr[1]);
                    return cmd;
                case Sql_Action_Option.UPDATE_BACKSTAGE_EMPLOYEEACCOUNTLEVEL:
                    Data_Set_Employee_Account_LOANIT_ForBackStage ModifyeEmployeeAccountLevelData = JsonConvert.DeserializeObject<Data_Set_Employee_Account_LOANIT_ForBackStage>(value);
                    cmd.Parameters.Add("@account_list",JsonConvert.SerializeObject(ModifyeEmployeeAccountLevelData.AccountList));
                    cmd.Parameters.Add("@mission_type", ModifyeEmployeeAccountLevelData.mission_type);
                    cmd.Parameters.Add("@employee_level",ModifyeEmployeeAccountLevelData.employee_level);
                    return cmd;
                default:
                    return cmd;
            }
        }
    }
    public enum Sql_Action_Option
    {
        GET_EMPLOYEE_LEVEL_GA,
        GET_MISSION_EMPLOYEE_LEVEL,
        GET_RISK_REFERENCE_DATA,
        UPDATE_ACCOUNTTOSIGN_MISSION_STATUSID,
        UPDATE_COLLECTSIGN_SIBLINGRULE,
        UPDATE_BACK_REQUIRESIGN,



        UPDATE_BACKSTAGE_EMPLOYEEACCOUNTLEVEL
    }

}
