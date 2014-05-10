using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Windows.Storage;

namespace lab3
{
    class API
    {
        public async static Task<string> sendCommand(List<KeyValuePair<string, string>> parameters)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            string username = (string) localSettings.Values["username"];
            string password = (string) localSettings.Values["password"];

            parameters.Add(new KeyValuePair<string, string>("email", username));
            parameters.Add(new KeyValuePair<string, string>("password", password));
            
            var httpClient = new HttpClient(new HttpClientHandler());
            String serverUrl = "http://www.cs.rit.edu/~jsb/2135/ProgSkills/Labs/Messenger/api.php";
            FormUrlEncodedContent encodedParameters = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response = await httpClient.PostAsync(serverUrl, encodedParameters);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }
    }
}
