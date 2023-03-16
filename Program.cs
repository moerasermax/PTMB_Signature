using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PTMB_Signature.Implement_Form;

using Finance;
using System.Configuration;
using System.Runtime.CompilerServices;
using PTMB_Signature_API.Data_Set;
using PTMB_Signature.Model.Plugin;
using static Finance.VerifyTools;
using Newtonsoft.Json;
using API_SendMail.DataSet;

namespace PTMB_Signature
{
    internal static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //string sss = "{\"cardAuth\":true,\"applyFormAuth\":true,\"assetAuth\":true,\"financeAuth\":true,\"advertisementAuth\":true,\"cityCardAuth\":true,\"acctbook\":true,\"isAccountOfficer\":true,\"isAccountCreater\":true,\"user_name\":\"林姍儀\",\"isManager\":true,\"isAdmin\":true,\"isAssetController\":true,\"isOtherAcctbook_Loanit\":true,\"isOtherAcctbook_PTMB\":true,\"account\":\"tammy\",\"public_key\":\"<RSAKeyValue><Modulus>q65l9Fm9LozFikbpglxkaaLupveTeByCfgkwVlniXDzHg3FTrmsoi164UVK1QC0ueQBgxXbh8K1z6K5zj/LOK22uqrdFO2OlRVOrf1peEYetw+/GLdZ4QYB0E0PGupeCh7kXdmfAY7HtHB1RzIjov8H0UBXPyHz4fzlHT7qc2WnvPHaXP7PK1Nx8/Tk3mmL4VJ0akahDGHF3LmU8C/SWzW3EbM9RfjT1vPuFLUuerfTAkUaURNqjvtyB3OR2QMY+ipxKR6oHTvXSroo58oEN48XTutlDtG79DMDJ0AqrdTBJ66mT2v52KoYpEQ+hrtVQ2O3XeEfVuErkdDu29C1t1Q==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>\",\"timeStmp\":\"20221011163434\",\"token\":\"458CA89B7BC7B701FCD1EEA10DAD81B56E094939279FFBD4A117E5C67516DBCF\"}";
            //AuthDatas authData = JsonConvert.DeserializeObject<AuthDatas>(sss);
            try
            {
                string mode = ConfigurationManager.AppSettings["Mode"];
                if (args.Length == 0)
                {
                    if (mode.Equals("Development_SignSystem"))
                    {
                        Login_Account login_account = new Login_Account();
                        Application.EnableVisualStyles();
                        Application.Run(login_account);
                    }
                    else if (mode.Equals("Development_Account"))
                    {
                        Application.EnableVisualStyles();
                        Application.Run(new AccountSystem_Signature());
                    }
                    else
                    {
                        MessageBox.Show("模式非開發模式");
                    }
                }
                else
                {
                    try
                    {
                        MainSystemData mainsystemdata = MainSystemData.getInstance();
                        mainsystemdata.setAuth(SubSysInfoController.getInstance().getInfo_GetFromMainSystem(args[0].Trim()));
                        //mainsystemdata.setAuth(SubSysInfoController.getInstance().getInfo_GetFromMainSystem(sss));

                        if (VerifyTools.getInstance().verify(mainsystemdata.authDatas.timeStmp, mainsystemdata.authDatas.token))
                        //if (VerifyTools.getInstance().verify(verifyTools.getTimeStamp(), verifyTools.getToken()))
                        {
                            if (mode.Equals("Account"))
                            {
                                Application.EnableVisualStyles();
                                Application.Run(new AccountSystem_Signature());
                                Console.WriteLine("[簽核系統]執行帳務系統");
                            }
                            else if (mode.Equals("SignSystem"))
                            {
                                Form_Sign sign_form = Form_Sign.getInstance();
                                sign_form.Set_args(args);
                                Application.EnableVisualStyles();
                                LOANIT_CONTROLLER_Plugin sign_plugin = LOANIT_CONTROLLER_Plugin.getInstance();
                                //MessageBox.Show(String.Format("驗證結果:{0}\r\n登入帳號:{1}", VerifyTools.getInstance().verify(mainsystemdata.authDatas.timeStmp, mainsystemdata.authDatas.token), mainsystemdata.authDatas.account));
                                //MessageBox.Show(String.Format("驗證結果:{0}\r\n登入帳號:{1}", VerifyTools.getInstance().verify(VerifyTools.getInstance().getDataForAPI().getTimeStamp(), VerifyTools.getInstance().getDataForAPI().getToken()),"1"));
                                Data_Set_Employee employee_data = sign_plugin.get_employee_information(mainsystemdata.authDatas.account);

                                sign_form.Initial_Form(employee_data);
                                Application.Run(sign_form);
                                Console.WriteLine("[簽核系統]執行簽核系統");
                            }
                            else
                            {
                                Application.EnableVisualStyles();
                                Application.Run(new Form_ModeError());
                                Console.WriteLine("[簽核系統]錯誤的輸出模式");
                            }
                            Console.WriteLine("[簽核系統]驗證成功");
                        }
                        else
                        {
                            MessageBox.Show("驗證失敗");
                        }


                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                        }



                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());

            }


        }

    }

}
