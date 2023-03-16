using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PTMB_Signature_API.Data_Set;
using PTMB_Signature_API.Model.Abstract;
using PTMB_Signature_API.Model.Implement;
namespace PTMB_Signature.Implement_Risk
{
    public class LOANIT_RISK : Mission
    {
        bool GA_Rule_Switch = false; /// 財務長若要使用開啟 (true)

        public override string get_mission_require_signature(int risk_value, SubSysNo subSysNo)
        {
            List<Data_Set_Mission_require> data_set_mission_require = new List<Data_Set_Mission_require>();

            if (subSysNo == SubSysNo.CH001)
            {
                /// 根據不同邏輯去做對應的輸入，這邊是範例
                if (risk_value == 1)
                {

                    /// 4等職員 
                    Data_Set_Mission_require employee_level4 = new Data_Set_Mission_require();
                    employee_level4.company = "LOANIT";
                    employee_level4.department = "RC";
                    employee_level4.amount = 1.ToString();
                    employee_level4.employee_level = 4.ToString();

                    if (!employee_level4.amount.Equals("0")) { data_set_mission_require.Add(employee_level4); }

                }
                else if (risk_value == 2)
                {

                    /// 4等職員 
                    Data_Set_Mission_require employee_level4 = new Data_Set_Mission_require();
                    employee_level4.company = "LOANIT";
                    employee_level4.department = "RC";
                    employee_level4.amount = 1.ToString();
                    employee_level4.employee_level = 4.ToString();

                    /// 3等職員 
                    Data_Set_Mission_require employee_level3 = new Data_Set_Mission_require();
                    employee_level3.company = "LOANIT";
                    employee_level3.department = "RC";
                    employee_level3.amount = 1.ToString();
                    employee_level3.employee_level = 3.ToString();

                    if (!employee_level4.amount.Equals("0")) { data_set_mission_require.Add(employee_level4); }
                    if (!employee_level3.amount.Equals("0")) { data_set_mission_require.Add(employee_level3); }
                }
                else if (risk_value == 3)
                {

                    /// 4等職員 
                    Data_Set_Mission_require employee_level4 = new Data_Set_Mission_require();
                    employee_level4.company = "LOANIT";
                    employee_level4.department = "RC";
                    employee_level4.amount = 1.ToString();
                    employee_level4.employee_level = 4.ToString();

                    /// 3等職員 
                    Data_Set_Mission_require employee_level3 = new Data_Set_Mission_require();
                    employee_level3.company = "LOANIT";
                    employee_level3.department = "RC";
                    employee_level3.amount = 1.ToString();
                    employee_level3.employee_level = 3.ToString();

                    /// 2等職員 
                    Data_Set_Mission_require employee_level2 = new Data_Set_Mission_require();
                    employee_level2.company = "LOANIT";
                    employee_level2.department = "RC";
                    employee_level2.amount = 1.ToString();
                    employee_level2.employee_level = 2.ToString();



                    if (!employee_level4.amount.Equals("0")) { data_set_mission_require.Add(employee_level4); }
                    if (!employee_level3.amount.Equals("0")) { data_set_mission_require.Add(employee_level3); }
                    if (!employee_level2.amount.Equals("0")) { data_set_mission_require.Add(employee_level2); }
                }
                else if (risk_value == 4)
                {

                    /// 4等職員 
                    Data_Set_Mission_require employee_level4 = new Data_Set_Mission_require();
                    employee_level4.company = "LOANIT";
                    employee_level4.department = "RC";
                    employee_level4.amount = 1.ToString();
                    employee_level4.employee_level = 4.ToString();

                    /// 3等職員 
                    Data_Set_Mission_require employee_level3 = new Data_Set_Mission_require();
                    employee_level3.company = "LOANIT";
                    employee_level3.department = "RC";
                    employee_level3.amount = 1.ToString();
                    employee_level3.employee_level = 3.ToString();


                    if (GA_Rule_Switch)
                    {
                        /// 集團財務長
                        Data_Set_Mission_require employee_GA = new Data_Set_Mission_require();
                        employee_GA.company = "PTMB";
                        employee_GA.department = "GA";
                        employee_GA.amount = 1.ToString();
                        employee_GA.employee_level = 2.5.ToString();

                        if (!employee_GA.amount.Equals("0")) { data_set_mission_require.Add(employee_GA); }
                    }


                    /// 2等職員 
                    Data_Set_Mission_require employee_level2 = new Data_Set_Mission_require();
                    employee_level2.company = "LOANIT";
                    employee_level2.department = "RC";
                    employee_level2.amount = 1.ToString();
                    employee_level2.employee_level = 2.ToString();

                    if (risk_value == 4 && GA_Rule_Switch)
                    {
                        /// 1等職員 
                        Data_Set_Mission_require employee_level1 = new Data_Set_Mission_require();
                        employee_level1.company = "LOANIT";
                        employee_level1.department = "RC";
                        employee_level1.amount = 1.ToString();
                        employee_level1.employee_level = 1.ToString();
                        if (!employee_level1.amount.Equals("0")) { data_set_mission_require.Add(employee_level1); }
                    }///因財務長不需要簽核因此需將風險值往上提。


                    if (!employee_level4.amount.Equals("0")) { data_set_mission_require.Add(employee_level4); }
                    if (!employee_level3.amount.Equals("0")) { data_set_mission_require.Add(employee_level3); }
                    if (!employee_level2.amount.Equals("0")) { data_set_mission_require.Add(employee_level2); }

                }
                else if (risk_value == 5)
                {

                    /// 4等職員 
                    Data_Set_Mission_require employee_level4 = new Data_Set_Mission_require();
                    employee_level4.company = "LOANIT";
                    employee_level4.department = "RC";
                    employee_level4.amount = 1.ToString();
                    employee_level4.employee_level = 4.ToString();

                    /// 3等職員 
                    Data_Set_Mission_require employee_level3 = new Data_Set_Mission_require();
                    employee_level3.company = "LOANIT";
                    employee_level3.department = "RC";
                    employee_level3.amount = 1.ToString();
                    employee_level3.employee_level = 3.ToString();

                    /// 2等職員 
                    Data_Set_Mission_require employee_level2 = new Data_Set_Mission_require();
                    employee_level2.company = "LOANIT";
                    employee_level2.department = "RC";
                    employee_level2.amount = 1.ToString();
                    employee_level2.employee_level = 2.ToString();

                    if (GA_Rule_Switch)
                    {
                        /// 集團財務長
                        Data_Set_Mission_require employee_GA = new Data_Set_Mission_require();
                        employee_GA.company = "PTMB";
                        employee_GA.department = "GA";
                        employee_GA.amount = 1.ToString();
                        employee_GA.employee_level = 1.5.ToString();

                        if (!employee_GA.amount.Equals("0")) { data_set_mission_require.Add(employee_GA); }
                    }

                    /// 1等職員 
                    Data_Set_Mission_require employee_level1 = new Data_Set_Mission_require();
                    employee_level1.company = "LOANIT";
                    employee_level1.department = "RC";
                    employee_level1.amount = 1.ToString();
                    employee_level1.employee_level = 1.ToString();

                    if (!employee_level4.amount.Equals("0")) { data_set_mission_require.Add(employee_level4); }
                    if (!employee_level3.amount.Equals("0")) { data_set_mission_require.Add(employee_level3); }
                    if (!employee_level2.amount.Equals("0")) { data_set_mission_require.Add(employee_level2); }
                    if (!employee_level1.amount.Equals("0")) { data_set_mission_require.Add(employee_level1); }

                }

            }
            else if (subSysNo == SubSysNo.LS002)
            {
                Data_Set_Mission_require employee_level1 = new Data_Set_Mission_require();
                employee_level1.company = "LOANIT";
                employee_level1.department = "ALL";
                employee_level1.amount = 1.ToString();
                employee_level1.employee_level = 1.ToString();
                if (!employee_level1.amount.Equals("0")) { data_set_mission_require.Add(employee_level1); }
            }
            else if(subSysNo == SubSysNo.AT001)
            {
                Data_Set_Mission_require employee_level2 = new Data_Set_Mission_require();
                employee_level2.company = "PTMB";
                employee_level2.department = "RD";
                employee_level2.amount = 1.ToString();
                employee_level2.employee_level = 2.ToString();
                if (!employee_level2.amount.Equals("0")) { data_set_mission_require.Add(employee_level2); }

                Data_Set_Mission_require employee_level1 = new Data_Set_Mission_require();
                employee_level1.company = "PTMB";
                employee_level1.department = "RD";
                employee_level1.amount = 1.ToString();
                employee_level1.employee_level = 1.ToString();
                if (!employee_level1.amount.Equals("0")) { data_set_mission_require.Add(employee_level1); }
            }




            return JsonConvert.SerializeObject(data_set_mission_require);
        }



        public static LOANIT_RISK instance = new LOANIT_RISK();
        public static LOANIT_RISK getInstance()
        {
            return instance;
        }
        private LOANIT_RISK()
        {

        }

    }
}
