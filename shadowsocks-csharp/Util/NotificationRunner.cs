using Shadowsocks.Controller;
using System;
using System.Timers;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Specialized;
using Shadowsocks.View;
using Newtonsoft.Json.Linq;
using Shadowsocks.Util;
using Shadowsocks.Extensions;
using System.Threading.Tasks;
using System.Threading;
using Shadowsocks.Data;

namespace Shadowsocks.Model
{
    public class NotificationRunner
    {
        private ShadowsocksController controller = ShadowsocksController.one();
        public static NotificationRunner _one = null;
        public static NotificationRunner one()
        {
            if (_one == null)
            {
                _one = new NotificationRunner();
            }
            return _one;
        }

        private System.Windows.Forms.Timer _timer = null;
        private System.Windows.Forms.Timer timer {
            get
            {
                if (_timer == null)
                {
                    _timer = new System.Windows.Forms.Timer();
                    _timer.Tick += new EventHandler((o, e) => {
                        User.one().FetchRalletsNotification();
                    });
                    _timer.Interval = 1000 * Constants.NOTIFICATION_INTERVAL;
                }
                return _timer;
            }
        }

        public void Start()
        {
            User.one().FetchRalletsNotification(onStartup: true);
            timer.Start();
        }
    }
}
