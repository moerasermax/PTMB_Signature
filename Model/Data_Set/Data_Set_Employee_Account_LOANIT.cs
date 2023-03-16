using PTMB_Signature_API.Data_Set;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTMB_Signature.Model.Data_Set
{


    public class Data_Set_Mission_Employee_Level_LOANIT : Data_Set_Mission_Employee_Level
    {
        public List<Data_Set_Employee_Account_LOANIT> account_list { get; set; }

    }

    public class Data_Set_Employee_Account_LOANIT
    {
        public string mission_type { get; set; }
        public string employee_level { get; set; }
        public string account { get; set; }
        public string risk { get; set; }

    }


    public class Data_Set_Employee_Account_LOANIT_ForBackStage
    {
        public string mission_type { get; set; }
        public string employee_level { get; set; }
        public List<MainSysAccount> AccountList { get; set; }
        public Data_Set_Result excute_result { get; set; }
    }

    public class MainSysAccount
    {
        public string account { get; set; }
        public string risk { get; set; }
    }
}
