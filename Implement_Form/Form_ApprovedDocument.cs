using Microsoft.Office.Interop.Word;
using New_Customer_Submit.API.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static New_Customer_Submit.API.Controller.Approval_Notice;
using Word = Microsoft.Office.Interop.Word;
using YC_ExportPdfAndWord_API.Model.Implement;
using Finance;
using PTMB_Signature.Model.Plugin;
using PTMB_Signature_API.Data_Set;
using Finance.ProductCalculatePreview.API;
using static Finance.ProductCalculatePreview.API.ProductPreviewer;
using System.Configuration;
using System.Text.RegularExpressions;
using ScorecardAPI.Repositories.Tables;
using ScorecardAPI;

namespace PTMB_Signature.Implement_Form
{
    public partial class Form_ApprovedDocument : Form
    {
        public Approval_Notice approvval_notice_data { get; set; }
        public static Form_ApprovedDocument instance = new Form_ApprovedDocument();
        public string additions_Conditions_data_collateral = "";
        public string additions_Conditions_data_mainbuilds = "";
        public string c_key = "";
        public static Form_ApprovedDocument getInstance()
        {
            if (instance is null)
            {
                return new Form_ApprovedDocument();
            }
            else
            {
                return instance;
            }

        }
        private Form_ApprovedDocument()
        {
            InitializeComponent();
        }

        public void Set_Data(Approval_Notice receive_approvval_notice, string receive_c_key, string m_id)
        {
            c_key = receive_c_key;
            approvval_notice_data = receive_approvval_notice;
            
            InitAmountLabel(m_id);

            Load_Data();
        }
        private void InitAmountLabel(string m_id)
        {
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            Data_Set_Mission_Details mission_data = sign_plugin.get_mission_information(m_id);

            string Total_Amount = get_TotalAmount_Format(mission_data);

            First_Requirment_Condition_Label.Text = string.Format("{1}萬 ({0}%/月)",
                //Convert(sign_plugin.get_mission_curent_amount(mission_data)),
                sign_plugin.get_mission_curent_rate(mission_data).ToString(),
                Total_Amount);

            string Process_Fee_Amount = (Math.Round(int.Parse(sign_plugin.get_mission_curent_amount(mission_data)) * 0.03)).ToString();
            string Process_Fee_Source = string.Format("{0}元 ({1}萬,{2}%)", changeToMoneyType(decimal.Parse(Process_Fee_Amount)), (decimal.Parse(sign_plugin.get_mission_curent_amount(mission_data).ToString()) / 10000).ToString(), "3");
            string Process_Fee = string.Format("{0}", Process_Fee_Source);
            Charge_Label.Text = Process_Fee;
            Repayment_Security_Deposit_Label.Text = Process_Fee;
            Monthly_Amount_Label.Text = changeToMoneyType(get_MonthlyAmount(mission_data)) + "元/月";
            Prepay_Month_Label.Text = string.Format("預繳月付金 {0} 期：共 {1} 元", sign_plugin.get_mission_curent_PrepayMonth(mission_data), changeToMoneyType((decimal.Parse(sign_plugin.get_mission_curent_PrepayMonth(mission_data))) * get_MonthlyAmount(mission_data)).ToString());
        }
        public void Load_Data()
        {
            approvval_notice_data.additional_Conditions = new List<string>();
            if (approvval_notice_data != null)
            {
                set_Land_Build_Info();

                Notice_Date_Label.Text = Regex.Split(DateTime.Now.ToString(), " ")[0];
                Customer_Name_Label.Text = get_FormatName(approvval_notice_data.customer_name);

                //Charge_Label.Text = ;
                Repayment_Method_Label.Text = get_Repayment_Method(approvval_notice_data.repayment_way1);
                //Repayment_Method_Label.Text = string.Format("{0},{1}", Get_MonthlyPayment(), approvval_notice_data.repayment_way3);
                Vioable_Amount_Label.Text = String.Format("{0}", "1年內為放款金額的16%，2年內為放款金額10%，2年以上不收。");
                Other_Remark_Label.Text = String.Format("{0}\r\n{1}", "1.該額度時效為通知日起14天為限。", "2.若有代償銀行欠款需由客戶本人匯款或備註客戶訊息以利判斷。");
                Additional_Collateral_Label.Text = string.Format("{0}", additions_Conditions_data_collateral);

            }
        }
        private void Generated_ApporvedDocument_Button_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> dictSource = new Dictionary<string, string>();
            dictSource.Add("[Notice_Date]", Notice_Date_Label.Text);
            dictSource.Add("[Customer_Name]", Customer_Name_Label.Text);

            dictSource.Add("[Additional_Collateral]", additions_Conditions_data_collateral.Replace("\r \r", "(ERROR !! 房屋擔保品: 有必填欄位未填)"));
            //dictSource.Add("[Additional_CollateralBuildNumber]", );
            dictSource.Add("[First_Requirment_Condition]", First_Requirment_Condition_Label.Text);
            dictSource.Add("[Additional_Condition]", Additional_Conditio_Textbox.Text);
            dictSource.Add("[Charge]", Charge_Label.Text);
            if (Repayment_Security_Deposit_Switch_Checkbox.Checked)
            {
                dictSource.Add("[Repayment_Security_Deposit]", Repayment_Security_Deposit_Label.Text.Trim()); 
            }
            else
            {
                dictSource.Add("^p還款保證金：[Repayment_Security_Deposit]", "");
            }
            dictSource.Add("[Monthly_Amount]", Monthly_Amount_Label.Text);
            dictSource.Add("[PrePay_Month]", Prepay_Month_Label.Text);
            dictSource.Add("[Vioable_Amount]", Vioable_Amount_Label.Text);
            dictSource.Add("[Repayment_Method]", Repayment_Method_Label.Text);
            dictSource.Add("[Other_Remark]", Other_Remark_Label.Text.Replace("\r\n", "\r"));


            string Releast_Path;

            if (ConfigurationManager.AppSettings["Mode"].Contains("Development"))
            {
                Releast_Path = "";
            }
            else
            {
                Releast_Path = "\\..\\SubSys_Zoo\\SubSys_SignSystem\\";
            }

            object templateFile = System.Environment.CurrentDirectory + Releast_Path + "\\Templates\\核定通知書_模板.docx";
            string saveFile_DOC = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + string.Format("\\測試核定通知書.docx", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string saveFile_PDF = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + string.Format("\\測試核定通知書{0}.pdf", DateTime.Now.ToString("yyyyMMddHHmmss"));
            ExportPdfAndWordAPI exportPdfAndWordAPI = new ExportPdfAndWordAPI();
            exportPdfAndWordAPI.ExportPdfAndWord(YC_ExportPdfAndWord_API.DataSet.ExportModeEnum_Option.PDF, dictSource, templateFile.ToString(), saveFile_PDF, false);

            MessageBox.Show("輸出成功");
        }
        private void Form_ApprovedDocument_Load(object sender, EventArgs e)
        {
            panel1.AutoScroll = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }
        private void Form_ApprovedDocument_FormClosing(object sender, FormClosingEventArgs e)
        {
            instance = null;
        }




        public void set_Land_Build_Info()
        {
            foreach (House_Info additional_Conditions in approvval_notice_data.house_Info)
            {
                additions_Conditions_data_collateral += string.Format("{0} \r", additional_Conditions.collateral);

                foreach (Main_Build main_builds in additional_Conditions.main_Builds)
                {
                    if (main_builds.area != null) { additions_Conditions_data_mainbuilds += string.Format("{0}", main_builds.area); }
                    if (main_builds.area != null) { additions_Conditions_data_mainbuilds += string.Format("{0}", main_builds.section); }
                    if (main_builds.area != null) { additions_Conditions_data_mainbuilds += string.Format(" 建號:{0}", main_builds.build_number); }

                    string additions_Conditions_data_mainbuilds_landnumber = "";
                    foreach (Public_Build public_build in main_builds.public_Builds)
                    {
                        additions_Conditions_data_mainbuilds_landnumber += string.Format("，{0}{1} 建號：{2} 地號：{3}", public_build.area, public_build.section, main_builds.build_number, public_build.land_number);
                    }
                    //additions_Conditions_data_mainbuilds_landnumber = additions_Conditions_data_mainbuilds_landnumber.Substring(0, additions_Conditions_data_mainbuilds_landnumber.Length - 1);
                    //if (main_builds.area != null) { additions_Conditions_data_mainbuilds += string.Format(" 地號:{0}", additions_Conditions_data_mainbuilds_landnumber); }
                    additions_Conditions_data_collateral += additions_Conditions_data_mainbuilds_landnumber + "。\r\n";
                }
                //additions_Conditions_data_collateral += additions_Conditions_data_mainbuilds_landnumber; 
                additions_Conditions_data_mainbuilds = "";

                //additions_Conditions_data_mainbuilds += String.Format("1.{0} 2.{1} 3.{2} 4.{3}",additional_Conditions.main_Builds.)
            }
        }
        private decimal get_MonthlyAmount(Data_Set_Mission_Details mission_data)
        {
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();


            if (approvval_notice_data.repayment_way1.Contains("本利攤"))
            {
                approvval_notice_data.repayment_way1 = "本利攤";
            }
            else if (approvval_notice_data.repayment_way1.Contains("先息後本"))
            {
                approvval_notice_data.repayment_way1 = "先息後本";
            }

            Block block = ProductPreviewer.getPreviewBlock(
                    decimal.Parse(sign_plugin.get_mission_curent_amount(mission_data)), ///核貸金額
                    int.Parse(approvval_notice_data.repayment_way3),  /// 利率
                    0.00m, /// 服務費 3%
                    (decimal.Parse(sign_plugin.get_mission_curent_rate(mission_data)) / 100), ///月利率
                    DateTime.Now.Date, DateTime.Now.AddMonths(1).Date,
                    (ProductType)Enum.Parse(typeof(ProductType), approvval_notice_data.repayment_way1) /// 還款方式
                );

            List<LoanDataPerPeriod> data = block.getProduct().getLastestGroup().getAcctbookUnit().getThisLoanUnit().mainLoanDatas;
            decimal MonthlyAmount = data[0].totalAmountNeedReceived;
            return MonthlyAmount;
        }
        private string get_Repayment_Method(string Repayment_Way1)
        {
            string result = "";
            if (Repayment_Way1.Contains("本利攤"))
            {
                result = string.Format("{0}還(本金與利息分期攤還)，{1}期", approvval_notice_data.repayment_way1, approvval_notice_data.repayment_way3);
            }
            else if (Repayment_Way1.Contains("先息後本"))
            {
                result = string.Format("{0}(本金到期後一併償還)，{1}期", approvval_notice_data.repayment_way1, approvval_notice_data.repayment_way3);
            }
            return result;
        }
        private string get_TotalAmount_Format(Data_Set_Mission_Details mission_data)
        {
            LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
            string Amount = (decimal.Parse(sign_plugin.get_mission_curent_amount(mission_data).ToString()) / 10000).ToString();
            string[] Amount_arr = Amount.Split('.');
            Console.WriteLine(Amount_arr[0]);
            return Amount_arr[0];

        }
        private string get_FormatName(string name)
        {
            char[] name_arr = name.ToCharArray();
            name_arr[1] = '*';
            string result = new string(name_arr);
            return result;
        }

        /// <summary>
        /// 獲取每個月月付金總額
        /// </summary>
        /// <returns></returns>



        /// <summary>
        /// 數字轉換大寫方法
        /// </summary>
        /// <param name="number">數字</param>
        /// <returns>大寫字符串</returns>
        public static string Convert(string strNumber)
        {
            strNumber = strNumber.Replace(",", "");
            string[] Unit = { "", "拾", "佰", "仟", "萬", "拾", "佰", "仟", "億", "拾", "佰", "仟", "萬" };
            string[] Case = { "零", "壹", "貳", "參", "肆", "伍", "陸", "柒", "捌", "玖", "拾" };
            //返回字符串
            string strValue = "";
            int value = 0;
            for (int i = 0; i < strNumber.Length; i++)
            {
                value = int.Parse(strNumber[strNumber.Length - 1 - i].ToString());
                if (value != 0)
                {
                    strValue = strValue.Insert(0, string.Concat(Case[value], Unit[i]));
                }
                else
                {
                    if (i % 4 == 0)//萬、億、萬億
                    {
                        strValue = strValue.Insert(0, Unit[i]);
                    }
                    else
                    {
                        if (strValue.Length > 0)
                        {
                            if (strValue.Substring(0, 1) != Case[0] && !Array.Exists(Unit, o => o == strValue.Substring(0, 1)))
                            {
                                strValue = strValue.Insert(0, Case[0]);
                            }
                        }
                    }
                }

            }
            return strValue;
        }
        public static string changeToMoneyType(decimal inputMoney)
        {
            if (inputMoney == 0)
            {
                return "0";
            }
            else
            {
                return inputMoney.ToString("###,###");
            }
        }



    }
}
