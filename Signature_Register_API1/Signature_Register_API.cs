using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTMB_Signature_API.Model.Abstract;
using PTMB_Signature.Model.Data_Set;
using PTMB_Signature.Model.DAO;
using PTMB_Signature.Model;
using PTMB_Signature_API.Data_Set;
using PTMB_Signature_API.Model.Implement;
using PTMB_Signature.Model.Plugin;
using Newtonsoft.Json;
using API_SendMail;
using PTMB_Signature.Implement_Risk;

namespace MissionRegister_API
{
    public class Signature_Register_API : Controller_Sign_Test_Abstract
    {
        #region 目前沒用到
        public override string Compare_New_Risk(string amount)
        {

            int compare_amount = int.Parse(amount);
            List<Data_Set_Risk_Data_Reference> list_risk_reference_data = get_risk_reference_data();

            foreach (Data_Set_Risk_Data_Reference risk_reference_data in list_risk_reference_data)
            {
                if (risk_reference_data.risk_type.Equals("Loanit_Amount"))
                {
                    if (risk_reference_data.min_data <= compare_amount && compare_amount <= risk_reference_data.max_data)
                    {
                        return ("true," + risk_reference_data.risk_value);
                    }
                }
            }

            return ("fasle" + 0.ToString());
        }
        public List<Data_Set_Risk_Data_Reference> get_risk_reference_data()
        {
            LOANIT_SQL_Plugin sql_plugin = new LOANIT_SQL_Plugin();
            LOANIT_DAO_Plugin dao_plugin = LOANIT_DAO_Plugin.getInstance();

            return dao_plugin.set_risk_reference_data_loanit(sql_plugin.get_risk_reference_data(Sql_Action_Category_Option.GET, PTMB_Signature.Model.Sql_Action_Option.GET_RISK_REFERENCE_DATA));
        }
        #endregion



        public void InitialMissionData()
        {
            LOANIT_RISK loanit = LOANIT_RISK.getInstance();
            loanit.initail_mission_object();
        }
        public void SetMissionObjectData(Set_Mission_Data_Option Category, string value)
        {
            LOANIT_RISK loanit = LOANIT_RISK.getInstance();
            loanit.set_mission_object_data(Category, value);

        }
        public int GetMissionRiskValue(string Amount)
        {
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            string[] risk_value = sign_plugin.Compare_New_Risk(Amount).Split(','); /// 計算風險值。/// 資料格式：[{執行結果},{風險值}]
            return int.Parse(risk_value[1]);
        }

        public Data_Set_Excutre_Result RegisterMission()
        {
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();

            LOANIT_RISK loanit = LOANIT_RISK.getInstance();
            return JsonConvert.DeserializeObject<Data_Set_Excutre_Result>(sign_plugin.register_new_mission(loanit));/// 註冊任務
        }
        
        public string GetMissionRequireSign(int RiskValue, SubSysNo SubsysNo)
        {
            return LOANIT_RISK.getInstance().get_mission_require_signature(RiskValue, SubsysNo);
        }

        public void SendRegisterMail(int RiskValue)
        {
            LOANIT_RISK loanit = LOANIT_RISK.getInstance();
            DemoPluginFunction abstract_PlgunFunction = new DemoPluginFunction();
            abstract_PlgunFunction.SendRegisterMissionMail(loanit, RiskValue);
        }

        public static Signature_Register_API Instance = new Signature_Register_API();
        public static Signature_Register_API getInstance()
        {
            return Instance;
        }
        private Signature_Register_API()
        {

        }

    }
}
