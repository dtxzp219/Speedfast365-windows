using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shadowsocks.Controller;
using Shadowsocks.Model;
using Shadowsocks.View;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Shadowsocks.Extensions;
using System.Windows;
using Shadowsocks.Exceptions;
using System.Security.Cryptography;
using Shadowsocks.Encryption;
using System.Net.Http;
using System.Threading.Tasks;
using Shadowsocks.Data;

namespace Shadowsocks.Util
{
    public class Request
    {
        private const string UserAgent = "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.3319.102 Safari/537.36";
        private static int REQUEST_TIMES = 3;
        private static string[] CONF_URLS = {
            "http://git.oschina.net/rallets/rallets/raw/master/EncryptedConfig",
            "https://coding.net/u/rallets/p/rallets/git/raw/master/EncryptedConfig",
            "http://raw.githubusercontent.com/ralletstellar/rallets/master/EncryptedConfig"
        };
        private User user = User.one();
        private string _serverRoot = null;
        private static Request _request = null;
        public static Request one()
        {
            if (_request == null)
            {
                _request = new Request();
            }
            return _request;
        }
        private string ServerRoot
        {
            set
            {
            }
            get
            {

                if (_serverRoot == null)
                {
                    if (_get(Constants.DEFAULT_SEVER_ROOT + "ping") == "pong")
                    {
                        _serverRoot = Constants.DEFAULT_SEVER_ROOT;
                    } else
                    {
                        string domain = _get("http://45.79.96.118/domain_windows");
                        _serverRoot = domain;
                        //for (int i = 0; i < CONF_URLS.Length; i++)
                        //{
                        //    try
                        //    {
                        //       string base64ServerRoot = _get(CONF_URLS[i]);
                        //        if (base64ServerRoot != null)
                        //       {
                        //            _serverRoot = Encoding.UTF8.GetString(Convert.FromBase64String(base64ServerRoot)).TrimEnd();
                        //           break;
                        //        }
                        //    }
                        //    catch (Exception e)
                        //    {
                        //        Debug.WriteLine(e);
                        //    }
                        //}

                    }
                }
                if (_serverRoot == null) _serverRoot = Constants.DEFAULT_SEVER_ROOT;
                return _serverRoot;
            }
        }
        private string _get(string url)
        {
            for (int i = 0; i < REQUEST_TIMES; i++)
            {
                try
                {
                    HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                    request.Method = "GET";
                    request.Timeout = 6000;
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                        return reader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
            return null;
        }

        private JObject PreparePostData(JObject data, bool encrypt)
        {
            if (data == null)
            {
                data = new JObject();
            }
            data["DEVICE_TYPE"] = "WINDOWS";
            data["VERSION"] = Util.Utils.getCurrentVersion();
            data["session_id"] = user.sessionId;
            data["encrypt"] = encrypt;
            return data;
        }
        public JObject Post(string url, JObject data = null, bool encrypt = false)
        {
            data = PreparePostData(data, encrypt);
            for (int i = 0; i < REQUEST_TIMES; i++)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var response = client.PostAsync(ServerRoot + url, new StringContent(data.ToString(), Encoding.UTF8, "application/json")).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var resultStr = response.Content.ReadAsStringAsync().Result;
                            dynamic result = JsonConvert.DeserializeObject(resultStr);
                            var jobj = JObject.Parse(resultStr);
                            if(jobj.Property("cipherText") != null)
                            {
                                if (encrypt && result.cipherText != null)
                                {
                                    var plainText = EncryptionHelper.Decrypt(result.cipherText.ToString(), Util.Utils.getCurrentVersion() + User.one().sessionId);
                                    return JObject.Parse(plainText);
                                }
                            }
                            return result;
                        }
                    }
                }
                catch { }
            }
            throw new RalletsException("http aysnc post with url: " + url + " failed.");
        }
        public string Get(string relativeUrl)
        {
            return _get(ServerRoot + relativeUrl);
        }
        public WebClient CreateWebClient()
        {
            WebClient http = new WebClient();
            http.Encoding = Encoding.UTF8;
            http.Headers.Add("User-Agent", UserAgent);
            // http.Proxy = new WebProxy(IPAddress.Loopback.ToString(), config.localPort);
            return http;
        }
        public void GetAsync(string relativeUrl, Action<object, DownloadStringCompletedEventArgs> done)
        {
            WebClient http = CreateWebClient();
            http.DownloadStringAsync(new Uri(ServerRoot + relativeUrl));
            http.DownloadStringCompleted += (object o, DownloadStringCompletedEventArgs e) => done(o, e);
        }
        public void DownloadAsync(string url, string localFileName, Action<object, System.ComponentModel.AsyncCompletedEventArgs> done)
        {
            WebClient http = CreateWebClient();
            http.DownloadFileAsync(new Uri(url), localFileName);
            http.DownloadFileCompleted += (object o, System.ComponentModel.AsyncCompletedEventArgs e) => done(o, e);
        }
    }
}
