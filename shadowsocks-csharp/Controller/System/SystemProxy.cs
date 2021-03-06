﻿using System;
using Shadowsocks.Model;
using Shadowsocks.Util.SystemProxy;

namespace Shadowsocks.Controller
{
    public static class SystemProxy
    {
        private static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssfff");
        }
        public static void Disable()
        {
            WinINet.SetIEProxy(false, false, "", "");
        }
        public static void Update(Configuration config, bool forceDisable)
        {
            bool global = config.global;
            bool enabled = config.enabled;

            if (forceDisable)
            {
                enabled = false;
            }

            try
            {
                if (enabled)
                {
                    if (global)
                    {
                        WinINet.SetIEProxy(true, true, "127.0.0.1:" + config.localPort.ToString(), "");
                    }
                    else
                    {
                        string pacUrl;
                        if (config.useOnlinePac && !config.pacUrl.IsNullOrEmpty())
                            pacUrl = config.pacUrl;
                        else
                            pacUrl = $"http://127.0.0.1:{config.localPort}/pac?t={GetTimestamp(DateTime.Now)}";
                        WinINet.SetIEProxy(true, false, "", pacUrl);
                    }
                }
                else
                {
                    Disable();
                }
            }
            catch (ProxyException ex)
            {
                Logging.LogUsefulException(ex);
            }
        }
    }
}