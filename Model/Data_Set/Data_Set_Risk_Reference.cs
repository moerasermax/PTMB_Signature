using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTMB_Signature.Model.Data_Set
{
    public class Data_Set_Risk_Amount_Reference
    {
        public decimal min_amount { get; set; }
        public decimal max_amount { get; set; }
        public string risk_value { get; set; }
    }
    public class Data_Set_Risk_ProcessRratio_Reference
    {
        public decimal min_ratio { get; set; }
        public decimal max_ratio { get; set; }
        public string risk_value { get; set; }
    }
    public class Data_Set_Risk_Data_Reference
    {
        public decimal min_data { get; set; }
        public decimal max_data { get; set; }
        public string risk_value { get; set; }
        public string risk_type { get; set; }
    }
}
