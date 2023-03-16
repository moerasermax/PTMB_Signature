using Newtonsoft.Json;
using PTMB_Signature.Model.Data_Set;
using PTMB_Signature_API.Data_Set;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YC_ExportPdfAndWord_API.DataSet;

namespace PTMB_Signature.Model.Plugin
{
    public class LOANIT_BACKSTAGE_CONTROLLER
    {

        public DataSet_Result UpdateEmployeeAccountData(List<Data_Set_Employee_Account_LOANIT_ForBackStage> ModifiedData_List)
        {
            DataSet_Result result = new DataSet_Result();
            LOANIT_SQL_Plugin sql_plugin = new LOANIT_SQL_Plugin();
            try
            {
                foreach (Data_Set_Employee_Account_LOANIT_ForBackStage ModifiedData in ModifiedData_List)
                {
                    try
                    {
                        sql_plugin.Update_EmployeeAccountLevel_Data(Sql_Action_Category_Option.UPDATE, Sql_Action_Option.UPDATE_BACKSTAGE_EMPLOYEEACCOUNTLEVEL, JsonConvert.SerializeObject(ModifiedData));
                        result.feedback_result += string.Format("任務類別：{0} 層級：{1} 更新完畢\r\n", ModifiedData.mission_type, ModifiedData.employee_level);
                    }
                    catch (Exception ex)
                    {
                        result.feedback_result += string.Format("任務類別：{0} 層級：{1} 更新失敗 \r\n 錯誤原因：{2} \r\n", ModifiedData.mission_type, ModifiedData.employee_level, ex.Message);
                    }

                }
                result.isSucess = true;
            }
            catch (Exception ex)
            {
                result.isSucess = false;
                result.feedback_result = ex.Message;
            }
            return result;
        }




        #region 封裝
        private LOANIT_BACKSTAGE_CONTROLLER() { }
        public static LOANIT_BACKSTAGE_CONTROLLER Instance = new LOANIT_BACKSTAGE_CONTROLLER();
        public static LOANIT_BACKSTAGE_CONTROLLER getInstnace() { return Instance; }
        #endregion


    }
}
