using Shadowsocks.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shadowsocks.Extensions;
using System.Diagnostics;

namespace Shadowsocks.Util
{
    class Url
    {
        public const string ACCOUNT_BASE = "http://45.79.96.118/";
        public const string PAYMENT_URL = ACCOUNT_BASE + "";
        public const string PROFILE_URL = ACCOUNT_BASE + "";
        public const string SIGNUP_URL = ACCOUNT_BASE + "";
        public const string MANUAL_URL = ACCOUNT_BASE+"support/help";
        public const string FIND_PASSWORD_URL = ACCOUNT_BASE + "member/login";
        public static string loginSuffix()
        {
            return $"?username_or_email={User.one().username.ToUrlEncodeBase64()}&login_password={User.one().password.ToUrlEncodeBase64()}";
        }
        public static void GotoPayment()
        {
            Process.Start(PAYMENT_URL + loginSuffix());
        }
        public static void GotoMyProfile()
        {
            Process.Start(PROFILE_URL + loginSuffix());
        }
        public static void Goto(string url)
        {
            Process.Start(url);
        }
    }
}
