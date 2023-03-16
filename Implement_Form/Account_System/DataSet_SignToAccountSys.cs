using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTMB_Signature.Implement_Form.Account_System
{
    public class DataSet_SignToAccountSys
    {
        public string M_id { get; set; }
        public string C_Key { get; set; }
        public string Repayment_TotalPeriods { get; set; }
        public string Monthly_Interest_Rate { get; set; } 
        public int PrepaidPeriod { get; set; }
        public string Loan_Amount { get; set; }
        public string Monthly_Service_Rate { get; set; }
        public string First_Repayment_Date { get; set; }
        public string Product_Category { get; set; }
    }
}
