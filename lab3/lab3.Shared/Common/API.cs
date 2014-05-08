using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace lab3
{
    class API
    {
        public async static Task<string> sendCommand(List<KeyValuePair<string, string>> parameters)
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            String serverUrl = "http://www.cs.rit.edu/~jsb/2135/ProgSkills/Labs/Messenger/api.php";
            FormUrlEncodedContent encodedParameters = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response = await httpClient.PostAsync(serverUrl, encodedParameters);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
