using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ZaloDotNetSDK;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using ChatboxVido.Models;
using System.Windows.Shapes;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using System.Data.Entity;
using System.Text.RegularExpressions;
using static ChatboxVido.Models.ProfileZalo;

namespace ChatboxVido.ViewModel
{
    public class CallZaloAPI
    {
        private string _AccessToken = "";
        public ApplicationConfig GetApplicationConfig()
        {
            var applicationConfig = DataProvider.Instance.DB.ApplicationConfigs.FirstOrDefault(a => a.Id >= 1);
            DateTime dateTime = DateTime.Now;
            dateTime = (DateTime)applicationConfig.LastUpdatedDateTime;
            if (dateTime.AddMinutes(-50) > DateTime.Now)
            {
                return null;
            }
            return applicationConfig;
        }

        public string SendMessage(string userId, string messages)
        {
            var appConfig = GetApplicationConfig();
            if (appConfig != null)
            {
                ZaloClient client = new ZaloClient(appConfig.AccessToken);

                JObject result = client.sendTextMessageToUserId(userId, messages);
                return result.ToString();
            }

            return null;
        }


        public JObject getMessageHistory(long userId)
        {
            var appConfig = GetApplicationConfig();
            if (appConfig != null)
            {
                ZaloClient client = new ZaloClient(appConfig.AccessToken);
                JObject result = client.getListConversationWithUser(userId, 0, 10);
                return result;
            }
            return null;
        }
        public ZaloGetProfileResult getProfile(string userId)
        {
            ZaloClient client = new ZaloClient(_AccessToken);
            ZaloGetProfileResult result = JsonConvert.DeserializeObject<ZaloGetProfileResult>(client.getProfileOfFollower(userId).ToString());
            if(result.error == -216)
            {
                RefeshToken();
                return getProfile(userId);
            }
            else
            {
                return result;
            }
        }
        public string GetAnswerByQuestion(string question)
        {
            Console.WriteLine("GetAnswerByQuestion: {0}", question);
            question = convertToUnSign(question);
            Console.WriteLine("GetAnswerByQuestion:1{0}", question);
            var flight = DataProvider.Instance.DB.FQADetails.Where(s => s.Question == question).Include(s => s.FQA).FirstOrDefault();

            if (flight == null)
            {
                return null;
            }
            //This mapping won't work as I have not done the Profiles section Duh!!!
            return flight.FQA.Answers;
        }

        public string getMessageType(string userId)
        {
            var message = getMessageHistory(Int64.Parse(userId));
            Console.WriteLine("message: " + message);
            string result = "";
            if (message["data"].Count() == 3)
            {
                if (message["data"][0]["from_display_name"].ToString() == "Cao đẳng Viễn Đông TPHCM")
                {
                    if (message["data"][2]["message"].ToString() == "Email:\n- Ví Dụ: nguyenvana@vido.com")
                    {
                        result = "email";
                    }
                    else if (message["data"][2]["message"].ToString() == "Họ và Tên:\n- Ví dụ: Nguyễn Văn A")
                    {
                        result = "ten";
                    }
                    else if (message["data"][2]["message"].ToString() == "Địa chỉ:\n- Ví Dụ: Cao Đẳng Viễn Đông")
                    {
                        result = "dc";
                    }
                    else if (message["data"][2]["message"].ToString() == "Số Điện Thoại:\n- Ví Dụ: 0931231311")
                    {
                        result = "sdt";
                    }
                    else if (message["data"][2]["message"].ToString() == "Ngày Sinh: \n- Ví Dụ: 01/01/2000")
                    {
                        result = "ns";
                    }
                }
                else if (message["data"][0]["from_display_name"].ToString() != "Cao đẳng Viễn Đông TPHCM")
                {
                    if (message["data"][1]["message"].ToString() == "Email:\n- Ví Dụ: nguyenvana@vido.com")
                    {
                        result = "email";
                    }
                    else if (message["data"][1]["message"].ToString() == "Họ và Tên:\n- Ví dụ: Nguyễn Văn A")
                    {
                        result = "ten";
                    }
                    else if (message["data"][1]["message"].ToString() == "Địa chỉ:\n- Ví Dụ: Cao Đẳng Viễn Đông")
                    {
                        result = "dc";
                    }
                    else if (message["data"][1]["message"].ToString() == "Số Điện Thoại:\n- Ví Dụ: 0931231311")
                    {
                        result = "sdt";
                    }
                    else if (message["data"][1]["message"].ToString() == "Ngày Sinh: \n- Ví Dụ: 01/01/2000")
                    {
                        result = "ns";
                    }
                }
            }
            Console.WriteLine("result: " + result);
            return result;
        }
        public static string convertToUnSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
        public void RefeshToken()
        {
            var applicationConfig = DataProvider.Instance.DB.ApplicationConfigs.FirstOrDefault(a => a.Id >= 1);
            DateTime dateTime = DateTime.Now;
            dateTime = (DateTime)applicationConfig.LastUpdatedDateTime;
            _AccessToken = applicationConfig.AccessToken;
            //if (dateTime.AddMinutes(-50) > DateTime.Now)
            //{
            //    return null;
            //}
            //return ;
        }
    }
}
