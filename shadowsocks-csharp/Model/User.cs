using Shadowsocks.Controller;
using Shadowsocks.View;
using System;
using Newtonsoft.Json.Linq;
using Shadowsocks.Extensions;
using System.Windows.Forms;
using System.Diagnostics;
using Shadowsocks.Util;
using Shadowsocks.Exceptions;
using System.Threading.Tasks;

namespace Shadowsocks.Model
{
    public class User
    {
        private Properties.Settings settings = Properties.Settings.Default;
        private ShadowsocksController controller = ShadowsocksController.one();
        public String username
        {
            get
            {
                return settings.username;
            }
            set
            {
                settings.username = value;
                settings.Save();
            }
        }
        public String sessionId
        {
            get
            {
                return settings.sessionId;
            }
            set
            {
                settings.sessionId = value;
                settings.Save();
            }
        }
        public String password
        {
            get
            {
                return settings.password;
            }
            set
            {
                settings.password = value;
                settings.Save();
            }
        }
        public bool loggedIn
        {
            get
            {
                return !string.IsNullOrEmpty(sessionId);
            }
        }
        private static User _user = null;
        public static User one()
        {
            if (_user == null)
            {
                _user = new User();
            }
            return _user;
        }
        public void Logout()
        {
            var result = Request.one().Post("logout");
            ShadowsocksController controller = ShadowsocksController.one();
            MenuViewController view = MenuViewController.one();
            controller.ToggleEnable(false);
            controller.EmptyServers();
            sessionId = null;
            view.ShowLoginForm();
            view.SetLoginItemText();
            LoginForm.one().ShowTitle(I18N.GetString("You have logged out"));
        }
        private string preSystemNotificationStr = "";
        private void ParseSystemNotification (JToken systemNotification)
        {
            if (systemNotification != null)
            {
                if (systemNotification.ToString() == preSystemNotificationStr) return;
                preSystemNotificationStr = systemNotification.ToString();
                Util.Utils.isNewVersionAvailable(systemNotification);
                if ((Boolean?)systemNotification["show"] ?? false)
                {
                    var result = MessageBox.Show((String)systemNotification["message"], "通知", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (Util.Utils.UrlRegex.Match(systemNotification["link"].ToString()).Success)
                    {
                        Process.Start((String)systemNotification["link"]);
                    }
                }
            }
        }
        public JToken FetchRalletsNotification(bool onStartup = false)
        {
            Debug.WriteLine("Speedfast365 FetchNotification");
            // 第一次启动的时候, 清空代理设置，但是不会修改到配置
            if (onStartup)
            {
                SystemProxy.Disable();
            }
            //var enabled = controller.GetFreshConfiguration().enabled;
            try
            {
                //JObject notification = Request.one().Post("rallets_notification", encrypt: true);
                JObject notification = Request.one().Post("index_notification", encrypt: false);
                if (notification.ok())
                {
                    controller.AssignAndReloadConfigsIfDiff(notification["configs"]);
                    MenuViewController.one().UpdateView(notification);
                    if (onStartup)
                    {
                        controller.UpdatePACFromGFWList();
                    }
                }
                else
                {
                    if (onStartup || loggedIn)
                    {
                        Logout();
                        MenuViewController.one().loginForm.showWarningTitle((String)notification.SelectToken("message"));
                    }
                }
                ParseSystemNotification(notification["systemNotification"]);
                return notification;
            }
            catch (RalletsException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return null;
        }
    }
}
