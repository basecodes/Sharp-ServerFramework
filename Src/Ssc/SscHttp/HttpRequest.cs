#if NETCOREAPP2_1

using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ssc.SscHttp {
    public class HttpRequest {
        public static async Task GetHttpRequest(string url, Action<HttpStatusCode, string> callback) {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
            using (var httpClient = new HttpClient()) {
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode) {
                    var result = response.Content.ReadAsStringAsync().Result;
                    callback?.Invoke(response.StatusCode, result);
                }

                callback?.Invoke(response.StatusCode, "");
            }
        }

        public static async Task PostHttpRequest(
            string url, string content, Action<HttpStatusCode, string> callback) {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrEmpty(content)) throw new ArgumentNullException(nameof(content));
            using (var httpClient = new HttpClient()) {
                var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, httpContent);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode) {
                    var result = response.Content.ReadAsStringAsync().Result;
                    callback?.Invoke(response.StatusCode, result);
                }

                callback?.Invoke(response.StatusCode, "");
            }
        }
    }
}

#endif