using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GONOrderingSystems.Common.Common
{
    public class Util
    {
        public static async Task PostApi(string uri, string data)
        {
            using (var client = new HttpClient())
            {
                await client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
            }
                
        }
    }
}
