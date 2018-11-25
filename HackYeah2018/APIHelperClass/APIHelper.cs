using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HackYeah2018.APIHelperClass
{
    public static class APIHelper
    {
        public static async Task<string> GetTokenAsync(APISettingsConfig apiSettingsConfig, HttpClient client, string code,string bankId)
        {
            var uriBuilder = new UriBuilder($"{apiSettingsConfig.SERVER_PATH}/{bankId}/{apiSettingsConfig.TOKEN_PATH}");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["clientId"] = apiSettingsConfig.API_KEY;
            query["code"] = code;
            uriBuilder.Query = query.ToString();

            var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.ToString());
            request.Headers.Add("Accept-Encoding", "gzip");
            request.Headers.Add("Accept-Charset", "utf-8");

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (payload["error"] != null)
            {
                throw new InvalidOperationException("An error occurred while retrieving an access token.");
            }

            return (string)payload["access_token"];
        }
        public static async Task<List<Account>> GetAccountsAsync(APISettingsConfig apiSettingsConfig, HttpClient client, string token, string code, string bankId)
        {
            var uriBuilder = new UriBuilder($"{apiSettingsConfig.SERVER_PATH}/{bankId}/{apiSettingsConfig.ACCOUNTS_PATH}");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["bankId"] = bankId;
            uriBuilder.Query = query.ToString();

            var request = GetCommonRequestMessage(apiSettingsConfig, uriBuilder.ToString(), token);
            
            var requestHeaderInfo = new RequestHeaderInfo
            {
                requestHeader = new RequestHeader
                {
                    requestId = Guid.NewGuid().ToString(),
                    token = token,
                    tppId = apiSettingsConfig.API_KEY,
                    sendDate = DateTime.Now
                }
            };

            var json = JsonConvert.SerializeObject(requestHeaderInfo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Content = content;

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (payload["error"] != null)
            {
                throw new InvalidOperationException("An error occurred while retrieving an access token.");
            }

            var ret = new List<Account>();
            var accounts = payload["accounts"];
           
            var count = accounts.Count();

            if (count>0)
            {
                var accountsArray = accounts.ToObject<Account[]>();
                ret = accountsArray.ToList();
            }

            return ret;
        }

        public static async Task<Account> GetAccountAsync(APISettingsConfig apiSettingsConfig, HttpClient client, string token, string code, string bankId, string accountNumber)
        {
            var uriBuilder = new UriBuilder($"{apiSettingsConfig.SERVER_PATH}/{bankId}/{apiSettingsConfig.ACCOUNT_PATH}");
            var request = GetCommonRequestMessage(apiSettingsConfig, uriBuilder.ToString(), token);

            var requestHeaderInfo = new RequestHeaderInfo
            {
                accountNumber = accountNumber,

                requestHeader = new RequestHeader
                {
                    requestId = Guid.NewGuid().ToString(),
                    token = token,
                    tppId = apiSettingsConfig.API_KEY,
                    sendDate = DateTime.Now
                }
            };

            var json = JsonConvert.SerializeObject(requestHeaderInfo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Content = content;

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (payload["error"] != null)
            {
                throw new InvalidOperationException("An error occurred while retrieving an access token.");
            }

            var ret = new Account();
            var account = payload["account"];

            var count = account.Count();

            if (count > 0)
                ret = account.ToObject<Account>();

            return ret;
        }

        public static async Task<List<Transaction>> GetTrasactionsAsync(APISettingsConfig apiSettingsConfig, HttpClient client, string token, string code, string bankId, string accountNumber)
        {
            var uriBuilder = new UriBuilder($"{apiSettingsConfig.SERVER_PATH}/{bankId}/{apiSettingsConfig.TRANSACTIONS_PATH}");
            var request = GetCommonRequestMessage(apiSettingsConfig, uriBuilder.ToString(), token);

            var requestHeaderInfo = new RequestHeaderInfo
            {
                accountNumber = accountNumber,
                requestHeader = new RequestHeader
                {
                    requestId = Guid.NewGuid().ToString(),
                    token = token,
                    tppId = apiSettingsConfig.API_KEY,
                    sendDate = DateTime.Now
                }
            };

            var json = JsonConvert.SerializeObject(requestHeaderInfo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Content = content;

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (payload["error"] != null)
            {
                throw new InvalidOperationException("An error occurred while retrieving an access token.");
            }

            var ret = new List<Transaction>();
            var transactions = payload["transactions"];

            var count = transactions.Count();

            if (count > 0)
            {
                var transactionsArray = transactions.ToObject<Transaction[]>();
                ret = transactionsArray.ToList();
            }

            return ret;
        }

        private static HttpRequestMessage GetCommonRequestMessage(APISettingsConfig apiSettingsConfig, string url, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", apiSettingsConfig.AUTH_BARERE_PREFIX + token);
            request.Headers.Add("Accept-Language", "en-US");
            request.Headers.Add("Accept-Encoding", "gzip");
            request.Headers.Add("Accept-Charset", "utf-8");
            request.Headers.Add("X-JWS-SIGNATURE", apiSettingsConfig.X_JWS_SIGNATURE);

            return request;
        }
    }

    public class RequestHeaderInfo
    {
        public string accountNumber { get; set; }
        public RequestHeader requestHeader { get; set; }
    }

    public class RequestHeader
    {
        public string requestId { get; set; }
        public DateTime sendDate { get; set; }
        public string token { get; set; }

        public string tppId { get; set; }
    }
}
