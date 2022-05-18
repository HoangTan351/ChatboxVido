using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using ChatboxVido.Models;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace ChatboxVido.ViewModel
{
    public class CallCrispApi
    {
        public async void SendMessageCrisp(string message,string websiteId, string sessionId,string identifer, string key){ 
            message = message.Replace("\n", "\\n");
            
            HttpClient client = new HttpClient();
            string url = "https://api.crisp.chat/v1/website/"+websiteId+"/conversation/"+sessionId+"/message";
            
            var authenticationString = $"{identifer}:{key}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("basic",base64EncodedAuthenticationString);
            client.DefaultRequestHeaders.Add("X-Crisp-Tier","plugin");

            var payload = "{\"type\": \"text\",\"from\": \"operator\",\"origin\": \"chat\",\"content\": \""+message+"\"}";
            Console.WriteLine(payload);
            Console.WriteLine(url);
            try
            {
                HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");

                HttpResponseMessage httpResponse = client.PostAsync(url, c).GetAwaiter().GetResult();
                httpResponse.EnsureSuccessStatusCode(); // throws if not 200-299
                string responseString = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine(responseString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
            
    }
}
