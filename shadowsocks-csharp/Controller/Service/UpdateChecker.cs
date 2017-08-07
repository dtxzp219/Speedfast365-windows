using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Shadowsocks.Extensions;

using Newtonsoft.Json.Linq;

using Shadowsocks.Model;
using Shadowsocks.Util;
using System.Diagnostics;

namespace Shadowsocks.Controller
{
    public class UpdateChecker
    {
        private Configuration config;
        public bool NewVersionFound;
        public string LatestVersionNumber;
        public string LatestVersionName;
        public string LatestVersionURL;
        public string LatestVersionLocalName;
        public event EventHandler CheckUpdateCompleted;

        private class CheckUpdateTimer : System.Timers.Timer
        {
            public Configuration config;

            public CheckUpdateTimer(int p) : base(p)
            {
            }
        }

        public void CheckUpdate(Configuration config, int delay)
        {
            CheckUpdateTimer timer = new CheckUpdateTimer(delay);
            timer.AutoReset = false;
            timer.Elapsed += Timer_Elapsed;
            timer.config = config;
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckUpdateTimer timer = (CheckUpdateTimer)sender;
            Configuration config = timer.config;
            timer.Elapsed -= Timer_Elapsed;
            timer.Enabled = false;
            timer.Dispose();
            CheckUpdate(config);
        }

        public void CheckUpdate(Configuration config)
        {
            this.config = config;
            try
            {
                Request.one().GetAsync("system_data", http_DownloadStringCompleted);
            }
            catch (Exception ex)
            {
                Logging.LogUsefulException(ex);
            }
        }

        private void http_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string response = e.Result;
                JObject result = JObject.Parse(response);
                if (result.ok())
                {
                    JToken clients = result["settings"]["clients"];
                    string version = (string)clients["windows_version"];
                    if (Utils.isVersionNewerThanSystem(version))
                    {
                        NewVersionFound = true;
                        string url = (string)clients["windows_download_link"];
                        LatestVersionURL = url;
                        LatestVersionNumber = version;
                        string[] segs = url.Split('/');
                        LatestVersionName = segs[segs.Length - 1];
                        startDownload();
                        return;
                    }
                }
                Logging.Debug("No update is available");
                if (CheckUpdateCompleted != null)
                {
                    CheckUpdateCompleted(this, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Logging.LogUsefulException(ex);
            }
        }

        private void startDownload()
        {
            try
            {
                LatestVersionLocalName = Utils.GetTempPath(LatestVersionName);
                Debug.WriteLine(LatestVersionLocalName);
                Request.one().DownloadAsync(LatestVersionURL, LatestVersionLocalName, Http_DownloadFileCompleted);
            }
            catch (Exception ex)
            {
                Logging.LogUsefulException(ex);
            }
        }

        private void Http_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    Logging.LogUsefulException(e.Error);
                    return;
                }
                Logging.Debug($"New version {LatestVersionNumber} found: {LatestVersionLocalName}");
                if (CheckUpdateCompleted != null)
                {
                    CheckUpdateCompleted(this, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                Logging.LogUsefulException(ex);
            }
        }

        private void SortByVersions(List<Asset> asserts)
        {
            asserts.Sort(new VersionComparer());
        }

        public class Asset
        {
            public bool prerelease;
            public string name;
            public string version;
            public string browser_download_url;

            public bool IsNewVersion(string currentVersion)
            {
                if (prerelease)
                {
                    return false;
                }
                if (version == null)
                {
                    return false;
                }
                return Utils.CompareVersion(version, currentVersion) > 0;
            }

            public void Parse(JObject asset)
            {
                name = (string)asset["name"];
                browser_download_url = (string)asset["browser_download_url"];
                version = ParseVersionFromURL(browser_download_url);
                prerelease = browser_download_url.IndexOf("prerelease", StringComparison.Ordinal) >= 0;
            }

            private static string ParseVersionFromURL(string url)
            {
                Match match = Regex.Match(url, @".*Shadowsocks-win.*?-([\d\.]+)\.\w+", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    if (match.Groups.Count == 2)
                    {
                        return match.Groups[1].Value;
                    }
                }
                return null;
            }

        }

        class VersionComparer : IComparer<Asset>
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed. 
            public int Compare(Asset x, Asset y)
            {
                return Utils.CompareVersion(x.version, y.version);
            }
        }
    }
}
