using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using ZXing;
using ZXing.Common;
using ZXing.QrCode;

using Shadowsocks.Controller;
using Shadowsocks.Model;
using Shadowsocks.Properties;
using Shadowsocks.Util;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.IO;

namespace Shadowsocks.View
{
    public class MenuViewController
    {
        // yes this is just a menu view controller
        // when config form is closed, it moves away from RAM
        // and it should just do anything related to the config form

        private ShadowsocksController controller = ShadowsocksController.one();
        private UpdateChecker updateChecker;

        private NotifyIcon _notifyIcon;
        private Bitmap icon_baseBitmap;
        private Icon icon_base, icon_in, icon_out, icon_both, targetIcon;
        private ContextMenu contextMenu1;

        private bool _isFirstRun;
        private bool _isStartupChecking;
        private MenuItem enableItem;
        private MenuItem modeItem;
        private MenuItem AutoStartupItem;
        private MenuItem ShareOverLANItem;
        private MenuItem SeperatorItem;
        //private MenuItem ConfigItem;
        private MenuItem ServersItem;
        private MenuItem StrategyModeItem;
        private MenuItem globalModeItem;
        private MenuItem PACModeItem;
        private MenuItem localPACItem;
        private MenuItem onlinePACItem;
        private MenuItem editLocalPACItem;
        private MenuItem updateFromGFWListItem;
        private MenuItem editGFWUserRuleItem;
        private MenuItem editOnlinePACItem;
        private MenuItem autoCheckUpdatesToggleItem;
        private MenuItem proxyItem;
        private MenuItem hotKeyItem;
        private MenuItem VerboseLoggingToggleItem;
        private ConfigForm configForm;
        private ProxyForm proxyForm;
        private LogForm logForm;
        private HotkeySettingsForm hotkeySettingsForm;
        private string _urlToOpen;
        private MenuItem LoginItem;
        private MenuItem TrafficItem;
        public String LeftTraffic ;
        public LoginForm loginForm = LoginForm.one();
        public static MenuViewController _one = null;
        public static MenuViewController one()
        {
            if (_one == null)
            {
                _one = new MenuViewController();
            }
            return _one;
        }
        public MenuViewController()
        {
            LoadMenu();

            controller.EnableStatusChanged += controller_EnableStatusChanged;
            controller.ConfigChanged += controller_ConfigChanged;
            controller.PACFileReadyToOpen += controller_FileReadyToOpen;
            controller.UserRuleFileReadyToOpen += controller_FileReadyToOpen;
            controller.ShareOverLANStatusChanged += controller_ShareOverLANStatusChanged;
            controller.VerboseLoggingStatusChanged += controller_VerboseLoggingStatusChanged;
            controller.EnableGlobalChanged += controller_EnableGlobalChanged;
            controller.Errored += controller_Errored;
            controller.UpdatePACFromGFWListCompleted += controller_UpdatePACFromGFWListCompleted;
            controller.UpdatePACFromGFWListError += controller_UpdatePACFromGFWListError;

            _notifyIcon = new NotifyIcon();
            UpdateTrayIcon();
            _notifyIcon.Visible = true;
            _notifyIcon.ContextMenu = contextMenu1;
            _notifyIcon.BalloonTipClicked += notifyIcon1_BalloonTipClicked;
            _notifyIcon.MouseClick += notifyIcon1_Click;
            _notifyIcon.MouseDoubleClick += notifyIcon1_DoubleClick;
            _notifyIcon.BalloonTipClosed += _notifyIcon_BalloonTipClosed;
            controller.TrafficChanged += controller_TrafficChanged;

            this.updateChecker = new UpdateChecker();
            updateChecker.CheckUpdateCompleted += updateChecker_CheckUpdateCompleted;

            LoadCurrentConfiguration();

            Configuration config = controller.GetFreshConfiguration();

            if (config.isDefault)
            {
                _isFirstRun = true;
                ShowConfigForm();
            }
            else if(config.autoCheckUpdate)
            {
                _isStartupChecking = true;
                updateChecker.CheckUpdate(config, 3000);
            }
        }
        public void BlinkTrayIcon()
        {
            var times = 0;
            var timer = new System.Windows.Forms.Timer();
            bool isTurned = false;
            if (!controller.GetFreshConfiguration().enabled)
                return;
            timer.Tick += new EventHandler((o, e) =>
            {
                if (times++ < 6)
                {
                    _notifyIcon.Icon = !isTurned ? Resources.icon_32_g : Resources.icon_32_b;
                    isTurned = !isTurned;
                }
                else
                {
                    timer.Stop();
                }
            });
            timer.Interval = 80;
            timer.Start();
        }

        public void UpdateView(JObject notification = null)
        {
            if (notification == null)
            {
                return;
            }
            SetLoginItemText();
            JToken traffic = notification.SelectToken("self.traffic") ?? new JObject();
            double GB = Math.Pow(1024, 3);
            LeftTraffic = this.TrafficItem.Text = I18N.GetString("Left Traffic") + ": " +
                    Util.Utils.Truncate(((double)(traffic["premium"] ?? 0) / GB), 2) + "G" + "\n";
            UpdateTrayIcon();
        }
        private void controller_TrafficChanged(object sender, EventArgs e)
        {
            if (icon_base == null)
                return;
            if (!controller.GetFreshConfiguration().enabled)
                return;

            Icon newIcon;

            bool hasInbound = controller.traffic.Last.inboundIncreasement > 0;
            bool hasOutbound = controller.traffic.Last.outboundIncreasement > 0;

            if (hasInbound && hasOutbound)
                newIcon = icon_both;
            else if (hasInbound)
                newIcon = icon_in;
            else if (hasOutbound)
                newIcon = icon_out;
            else
                newIcon = icon_base;

            if (newIcon != this.targetIcon)
            {
                this.targetIcon = newIcon;
                _notifyIcon.Icon = newIcon;
            }
        }

        void controller_Errored(object sender, System.IO.ErrorEventArgs e)
        {
            MessageBox.Show(e.GetException().ToString(), String.Format(I18N.GetString("Speedfast365 Error: {0}"), e.GetException().Message));
        }

        #region Tray Icon

        // Bug: System.Runtime.IneropServices.ExternalExeption
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        private void UpdateTrayIcon()
        {
            int dpi,icon_baseSize;
            Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
            dpi = (int)graphics.DpiX;
            graphics.Dispose();
            //icon_baseBitmap = null;
            
            Configuration config = controller.GetFreshConfiguration();
            bool enabled = config.enabled;
            bool global = config.global;
            //icon_baseBitmap = getTrayIconByState(icon_base.ToBitmap(), enabled, global);

            if (!enabled)
                icon_base = Resources.icon_32_g;
            else
            {
                icon_base = Resources.icon_32_b;
                icon_in = Resources.icon_32_d;
                icon_out = Resources.icon_32_u;
                icon_both = Resources.icon_32_ud;
            }

            _notifyIcon.Icon = icon_base;
            // Bug: System.Runtime.IneropServices.ExternalExeption
           
            string serverInfo = null;
            if (controller.GetCurrentStrategy() != null)
            {
                serverInfo = controller.GetCurrentStrategy().Name;
            }
            else
            {
                serverInfo = config.GetCurrentServer().FriendlyName();
            }
            // we want to show more details but notify icon title is limited to 63 characters
            string text = I18N.GetString("Speedfast365") + " " + Util.Utils.getCurrentVersion() + "\n"
                + this.TrafficItem.Text
                    + (enabled ?
                              I18N.GetString("System Proxy On: ") + (global ? I18N.GetString("Global Mode") : I18N.GetString("Smart Mode")) :
                              String.Format(I18N.GetString("Running: Port {0}"), config.localPort))  // this feedback is very important because they need to know Shadowsocks is running
                          + "\n" + serverInfo;
            _notifyIcon.Text = text.Substring(0, Math.Min(63, text.Length));
        }

        private Bitmap AddBitmapOverlay(Bitmap original, params Bitmap[] overlays)
        {
            Bitmap bitmap = new Bitmap(original.Width, original.Height, PixelFormat.Format64bppArgb);
            Graphics canvas = Graphics.FromImage(bitmap);
            canvas.DrawImage(original, new Point(0, 0));
            foreach (Bitmap overlay in overlays)
            {
                canvas.DrawImage(new Bitmap(overlay, original.Size), new Point(0, 0));
            }
            canvas.Save();
            return bitmap;
        }

        #endregion

        #region MenuItems and MenuGroups

        private MenuItem CreateMenuItem(string text, EventHandler click)
        {
            return new MenuItem(I18N.GetString(text), click);
        }

        private MenuItem CreateMenuGroup(string text, MenuItem[] items)
        {
            return new MenuItem(I18N.GetString(text), items);
        }
        private void LoginItem_Click(object sender, EventArgs e)
        {
            if (User.one().loggedIn)
            {
                if (configForm != null)
                {
                    configForm.Dispose();
                    configForm = null;
                    Utils.ReleaseMemory(true);
                }
                User.one().Logout();
            }
            else
            {
                ShowLoginForm();
            }
        }
        private void LoadMenu()
        {
            this.StrategyModeItem = CreateMenuGroup("Server Selection Mode", new MenuItem[] { });
            var strategyItems = new MenuItem[] {};
            foreach (var strategy in controller.GetStrategies())
            {
                MenuItem item = new MenuItem(strategy.Name);
                item.Tag = strategy.ID;
                item.Click += AStrategyItem_Click;
                this.StrategyModeItem.MenuItems.Add(item);
            }
            this.contextMenu1 = new ContextMenu(new MenuItem[] {
                this.enableItem = CreateMenuItem("Enable System Proxy", new EventHandler(this.EnableItem_Click)),
                this.modeItem = CreateMenuGroup("Mode", new MenuItem[] {
                    this.PACModeItem = CreateMenuItem("Smart Mode", new EventHandler(this.PACModeItem_Click)),
                    this.globalModeItem = CreateMenuItem("Global Mode", new EventHandler(this.GlobalModeItem_Click))
                }),
                this.ServersItem = CreateMenuGroup("Servers", new MenuItem[] {
                    //this.ConfigItem = CreateMenuItem("Edit Servers...", new EventHandler(this.Config_Click)),
                    //CreateMenuItem("Show QRCode...", new EventHandler(this.QRCodeItem_Click)),
                    //CreateMenuItem("Scan QRCode from Screen...", new EventHandler(this.ScanQRCodeItem_Click))
                }),
                CreateMenuGroup("More Settings And Info", new MenuItem[] {
                    this.AutoStartupItem = CreateMenuItem("Start on Boot", new EventHandler(this.AutoStartupItem_Click)),
                    this.ShareOverLANItem = CreateMenuItem("Allow Clients from LAN", new EventHandler(this.ShareOverLANItem_Click)),
                    this.StrategyModeItem,
                    CreateMenuItem("Statistics Config...", StatisticsConfigItem_Click),
                    this.hotKeyItem = CreateMenuItem("Edit Hotkeys...", new EventHandler(this.hotKeyItem_Click)),
                    CreateMenuGroup("PAC ", new MenuItem[] {
                        this.localPACItem = CreateMenuItem("Local PAC", new EventHandler(this.LocalPACItem_Click)),
                        this.onlinePACItem = CreateMenuItem("Online PAC", new EventHandler(this.OnlinePACItem_Click)),
                        new MenuItem("-"),
                        this.editLocalPACItem = CreateMenuItem("Edit Local PAC File...", new EventHandler(this.EditPACFileItem_Click)),
                        this.updateFromGFWListItem = CreateMenuItem("Update Local PAC from Speedfast365", new EventHandler(this.UpdatePACFromGFWListItem_Click)),
                        this.editGFWUserRuleItem = CreateMenuItem("Edit User Rule for Speedfast365...", new EventHandler(this.EditUserRuleFileForGFWListItem_Click)),
                        this.editOnlinePACItem = CreateMenuItem("Edit Online PAC URL...", new EventHandler(this.UpdateOnlinePACURLItem_Click)),
                    }),
                    this.proxyItem = CreateMenuItem("Forward Proxy...", new EventHandler(this.proxyItem_Click)),
                    CreateMenuGroup("Logs", new MenuItem[] {
                        CreateMenuItem("Show Logs...", new EventHandler(this.ShowLogItem_Click)),
                        this.VerboseLoggingToggleItem = CreateMenuItem( "Verbose Logging", new EventHandler(this.VerboseLoggingToggleItem_Click) )
                    }),
                    CreateMenuGroup("Updates Speedfast365", new MenuItem[] {
                        CreateMenuItem("Check for Updates...", new EventHandler(this.checkUpdatesItem_Click)),
                        new MenuItem("-"),
                        this.autoCheckUpdatesToggleItem = CreateMenuItem("Check for Updates at Startup", new EventHandler(this.autoCheckUpdatesToggleItem_Click)),
                    }),
                    new MenuItem(I18N.GetString("About Speedfast365") + " " + Util.Utils.getCurrentVersion(), this.AboutItem_Click)
                }),
                new MenuItem("-"),
                CreateMenuGroup("Account", new MenuItem[] {
                    CreateMenuItem("Buy Service", (e, t) => Url.GotoPayment()),
                    //CreateMenuItem("Buy Service", new EventHandler(this.PaymentItem_Click)),
                    CreateMenuItem("My Profile", (e, t) => Url.GotoMyProfile()),
                    this.TrafficItem = CreateMenuItem("Traffic Info Loading...", null),
                    this.LoginItem = CreateMenuItem("Login", new EventHandler(this.LoginItem_Click)),
                }),
                new MenuItem("-"),
                CreateMenuItem("Quit", new EventHandler(this.Quit_Click))
            });
        }

        #endregion

        private void controller_ConfigChanged(object sender, EventArgs e)
        {
            LoadCurrentConfiguration();
            UpdateTrayIcon();
        }

        private void controller_EnableStatusChanged(object sender, EventArgs e)
        {
            enableItem.Checked = controller.GetFreshConfiguration().enabled;
            modeItem.Enabled = enableItem.Checked;
        }

        void controller_ShareOverLANStatusChanged(object sender, EventArgs e)
        {
            ShareOverLANItem.Checked = controller.GetFreshConfiguration().shareOverLan;
        }

        void controller_VerboseLoggingStatusChanged(object sender, EventArgs e) {
            VerboseLoggingToggleItem.Checked = controller.GetFreshConfiguration().isVerboseLogging;
        }

        void controller_EnableGlobalChanged(object sender, EventArgs e)
        {
            globalModeItem.Checked = controller.GetFreshConfiguration().global;
            PACModeItem.Checked = !globalModeItem.Checked;
        }

        void controller_FileReadyToOpen(object sender, ShadowsocksController.PathEventArgs e)
        {
            string argument = @"/select, " + e.Path;

            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        public void ShowBalloonTip(string title, string content, ToolTipIcon icon, int timeout)
        {
            _notifyIcon.BalloonTipTitle = title;
            _notifyIcon.BalloonTipText = content;
            _notifyIcon.BalloonTipIcon = icon;
            _notifyIcon.ShowBalloonTip(timeout);
        }

        void controller_UpdatePACFromGFWListError(object sender, System.IO.ErrorEventArgs e)
        {
            ShowBalloonTip(I18N.GetString("Failed to update PAC file"), e.GetException().Message, ToolTipIcon.Error, 5000);
            Logging.LogUsefulException(e.GetException());
        }

        void controller_UpdatePACFromGFWListCompleted(object sender, GFWListUpdater.ResultEventArgs e)
        {
            string result = e.Success
                ? I18N.GetString("PAC updated")
                : I18N.GetString("No updates found. Please report to Speedfast365 if you have problems with it.");
            ShowBalloonTip("", result, ToolTipIcon.Info, 1000);
        }

        void updateChecker_CheckUpdateCompleted(object sender, EventArgs e)
        {
            if (updateChecker.NewVersionFound)
            {
                ShowBalloonTip(String.Format(I18N.GetString("Speedfast365 {0} Update Found"), updateChecker.LatestVersionNumber), I18N.GetString("Click here to update"), ToolTipIcon.Info, 5000);
            }
            else if (!_isStartupChecking)
            {
                ShowBalloonTip("", I18N.GetString("No update is available"), ToolTipIcon.Info, 5000);
            }
            _isStartupChecking = false;
        }

        void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            if (updateChecker.NewVersionFound)
            {
                updateChecker.NewVersionFound = false; /* Reset the flag */
                if (System.IO.File.Exists(updateChecker.LatestVersionLocalName))
                {
                    string argument = "/select, \"" + updateChecker.LatestVersionLocalName + "\"";
                    System.Diagnostics.Process.Start("explorer.exe", argument);
                }
            }
        }

        private void _notifyIcon_BalloonTipClosed(object sender, EventArgs e)
        {
            if (updateChecker.NewVersionFound)
            {
                updateChecker.NewVersionFound = false; /* Reset the flag */
            }
        }

        private void LoadCurrentConfiguration()
        {
            Configuration config = controller.GetFreshConfiguration();
            UpdateServersMenu();
            UpdateStrategyModeMenu();
            enableItem.Checked = config.enabled;
            modeItem.Enabled = config.enabled;
            globalModeItem.Checked = config.global;
            PACModeItem.Checked = !config.global;
            ShareOverLANItem.Checked = config.shareOverLan;
            VerboseLoggingToggleItem.Checked = config.isVerboseLogging;
            AutoStartupItem.Checked = AutoStartup.Check();
            onlinePACItem.Checked = onlinePACItem.Enabled && config.useOnlinePac;
            localPACItem.Checked = !onlinePACItem.Checked;
            UpdatePACItemsEnabledStatus();
            UpdateUpdateMenu();
        }

        private void UpdateServersMenu()
        {
            // Rallets
            var items = ServersItem.MenuItems;
            items.Clear();
            int i = 0;
            Configuration configuration = controller.GetFreshConfiguration();
            foreach (var server in configuration.configs)
            {
                MenuItem item = new MenuItem(server.FriendlyName());
                item.Tag = i;
                item.Click += AServerItem_Click;
                items.Add(i, item);
                i++;
            }
            foreach (MenuItem item in items)
            {
                if (item.Tag != null && item.Tag.ToString() == configuration.index.ToString())
                {
                    item.Checked = true;
                }
            }
        }
        private void UpdateStrategyModeMenu()
        {
            var items = StrategyModeItem.MenuItems;
            Configuration configuration = controller.GetFreshConfiguration();
            foreach (MenuItem item in items)
            {
                item.Checked = item.Tag != null && item.Tag.ToString() == configuration.strategy;
            }
        }

        private void ShowConfigForm()
        {
            if (configForm != null)
            {
                configForm.Activate();
            }
            else
            {
                configForm = new ConfigForm();
                configForm.Show();
                configForm.Activate();
                configForm.FormClosed += configForm_FormClosed;
            }
        }

        private void ShowProxyForm()
        {
            if (proxyForm != null)
            {
                proxyForm.Activate();
            }
            else
            {
                proxyForm = new ProxyForm(controller);
                proxyForm.Show();
                proxyForm.Activate();
                proxyForm.FormClosed += proxyForm_FormClosed;
            }
        }

        private void ShowHotKeySettingsForm()
        {
            if (hotkeySettingsForm != null)
            {
                hotkeySettingsForm.Activate();
            }
            else
            {
                hotkeySettingsForm = new HotkeySettingsForm(controller);
                hotkeySettingsForm.Show();
                hotkeySettingsForm.Activate();
                hotkeySettingsForm.FormClosed += hotkeySettingsForm_FormClosed;
            }
        }
        public void SetLoginItemText()
        {
            if (this.LoginItem != null)
            {
                this.LoginItem.Text = User.one().loggedIn ? I18N.GetString("Logout") + " " + User.one().username : I18N.GetString("Login");
            }
        }
        public void ShowLoginForm()
        {
            loginForm.Show();
            loginForm.BringToFront();
        }
        private void ShowLogForm()
        {
            if (logForm != null)
            {
                logForm.Activate();
            }
            else
            {
                logForm = new LogForm(controller, Logging.LogFilePath);
                logForm.Show();
                logForm.Activate();
                logForm.FormClosed += logForm_FormClosed;
            }
        }

        void logForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            logForm.Dispose();
            logForm = null;
            Utils.ReleaseMemory(true);
        }

        void configForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            configForm.Dispose();
            configForm = null;
            Utils.ReleaseMemory(true);
            if (_isFirstRun)
            {
                CheckUpdateForFirstRun();
                ShowFirstTimeBalloon();
                _isFirstRun = false;
            }
        }

        void proxyForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            proxyForm.Dispose();
            proxyForm = null;
            Utils.ReleaseMemory(true);
        }

        void hotkeySettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            hotkeySettingsForm.Dispose();
            hotkeySettingsForm = null;
            Utils.ReleaseMemory(true);
        }

        private void Config_Click(object sender, EventArgs e)
        {
            ShowConfigForm();
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            controller.Stop();
            loginForm.Close();
            loginForm.Dispose();
            _notifyIcon.Visible = false;
            Application.Exit();
        }

        private void CheckUpdateForFirstRun()
        {
            Configuration config = controller.GetFreshConfiguration();
            if (config.isDefault) return;
            _isStartupChecking = true;
            updateChecker.CheckUpdate(config, 3000);
        }

        private void ShowFirstTimeBalloon()
        {
            _notifyIcon.BalloonTipTitle = I18N.GetString("Speedfast365 is here");
            _notifyIcon.BalloonTipText = I18N.GetString("You can turn on/off Speedfast365 in the context menu");
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            _notifyIcon.ShowBalloonTip(0);
        }

        private void AboutItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://45.79.96.118/");
        }

        private void notifyIcon1_Click(object sender, MouseEventArgs e)
        {
            if ( e.Button == MouseButtons.Middle )
            {
                ShowLogForm();
            }
        }

        private void notifyIcon1_DoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowConfigForm();
            }
        }

        private void EnableItem_Click(object sender, EventArgs e)
        {
            controller.ToggleEnable(!enableItem.Checked);
        }

        private void GlobalModeItem_Click(object sender, EventArgs e)
        {
            controller.ToggleGlobal(true);
        }

        private void PACModeItem_Click(object sender, EventArgs e)
        {
            controller.ToggleGlobal(false);
        }

        private void ShareOverLANItem_Click(object sender, EventArgs e)
        {
            ShareOverLANItem.Checked = !ShareOverLANItem.Checked;
            controller.ToggleShareOverLAN(ShareOverLANItem.Checked);
        }

        private void EditPACFileItem_Click(object sender, EventArgs e)
        {
            controller.TouchPACFile();
        }

        private void UpdatePACFromGFWListItem_Click(object sender, EventArgs e)
        {
            controller.UpdatePACFromGFWList();
        }

        private void EditUserRuleFileForGFWListItem_Click(object sender, EventArgs e)
        {
            controller.TouchUserRuleFile();
        }

        private void AServerItem_Click(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            controller.SelectServerIndex((int)item.Tag);
        }

        private void AStrategyItem_Click(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            controller.SelectStrategy((string)item.Tag);
        }

        private void VerboseLoggingToggleItem_Click( object sender, EventArgs e ) {
            VerboseLoggingToggleItem.Checked = ! VerboseLoggingToggleItem.Checked;
            controller.ToggleVerboseLogging( VerboseLoggingToggleItem.Checked );
        }

        private void StatisticsConfigItem_Click(object sender, EventArgs e)
        {
            StatisticsStrategyConfigurationForm form = new StatisticsStrategyConfigurationForm(controller);
            form.Show();
        }

        private void PaymentItem_Click(object sender, EventArgs e)
        {
            PaymentForm form = new PaymentForm();
            form.Show();
        }

        private void QRCodeItem_Click(object sender, EventArgs e)
        {
            QRCodeForm qrCodeForm = new QRCodeForm(controller.GetQRCodeForCurrentServer());
            //qrCodeForm.Icon = this.Icon;
            // TODO
            qrCodeForm.Show();
        }

        private void ScanQRCodeItem_Click(object sender, EventArgs e)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                using (Bitmap fullImage = new Bitmap(screen.Bounds.Width,
                                                screen.Bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(fullImage))
                    {
                        g.CopyFromScreen(screen.Bounds.X,
                                         screen.Bounds.Y,
                                         0, 0,
                                         fullImage.Size,
                                         CopyPixelOperation.SourceCopy);
                    }
                    int maxTry = 10;
                    for (int i = 0; i < maxTry; i++)
                    {
                        int marginLeft = (int)((double)fullImage.Width * i / 2.5 / maxTry);
                        int marginTop = (int)((double)fullImage.Height * i / 2.5 / maxTry);
                        Rectangle cropRect = new Rectangle(marginLeft, marginTop, fullImage.Width - marginLeft * 2, fullImage.Height - marginTop * 2);
                        Bitmap target = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);

                        double imageScale = (double)screen.Bounds.Width / (double)cropRect.Width;
                        using (Graphics g = Graphics.FromImage(target))
                        {
                            g.DrawImage(fullImage, new Rectangle(0, 0, target.Width, target.Height),
                                            cropRect,
                                            GraphicsUnit.Pixel);
                        }
                        var source = new BitmapLuminanceSource(target);
                        var bitmap = new BinaryBitmap(new HybridBinarizer(source));
                        QRCodeReader reader = new QRCodeReader();
                        var result = reader.decode(bitmap);
                        if (result != null)
                        {
                            var success = controller.AddServerBySSURL(result.Text);
                            QRCodeSplashForm splash = new QRCodeSplashForm();
                            if (success)
                            {
                                splash.FormClosed += splash_FormClosed;
                            }
                            else if (result.Text.StartsWith("http://") || result.Text.StartsWith("https://"))
                            {
                                _urlToOpen = result.Text;
                                splash.FormClosed += openURLFromQRCode;
                            }
                            else
                            {
                                MessageBox.Show(I18N.GetString("Failed to decode QRCode"));
                                return;
                            }
                            double minX = Int32.MaxValue, minY = Int32.MaxValue, maxX = 0, maxY = 0;
                            foreach (ResultPoint point in result.ResultPoints)
                            {
                                minX = Math.Min(minX, point.X);
                                minY = Math.Min(minY, point.Y);
                                maxX = Math.Max(maxX, point.X);
                                maxY = Math.Max(maxY, point.Y);
                            }
                            minX /= imageScale;
                            minY /= imageScale;
                            maxX /= imageScale;
                            maxY /= imageScale;
                            // make it 20% larger
                            double margin = (maxX - minX) * 0.20f;
                            minX += -margin + marginLeft;
                            maxX += margin + marginLeft;
                            minY += -margin + marginTop;
                            maxY += margin + marginTop;
                            splash.Location = new Point(screen.Bounds.X, screen.Bounds.Y);
                            // we need a panel because a window has a minimal size
                            // TODO: test on high DPI
                            splash.TargetRect = new Rectangle((int)minX + screen.Bounds.X, (int)minY + screen.Bounds.Y, (int)maxX - (int)minX, (int)maxY - (int)minY);
                            splash.Size = new Size(fullImage.Width, fullImage.Height);
                            splash.Show();
                            return;
                        }
                    }
                }
            }
            MessageBox.Show(I18N.GetString("No QRCode found. Try to zoom in or move it to the center of the screen."));
        }

        void splash_FormClosed(object sender, FormClosedEventArgs e)
        {
            ShowConfigForm();
        }

        void openURLFromQRCode(object sender, FormClosedEventArgs e)
        {
            Process.Start(_urlToOpen);
        }

        private void AutoStartupItem_Click(object sender, EventArgs e)
        {
            AutoStartupItem.Checked = !AutoStartupItem.Checked;
            if (!AutoStartup.Set(AutoStartupItem.Checked))
            {
                MessageBox.Show(I18N.GetString("Failed to update registry"));
            }
        }

        private void LocalPACItem_Click(object sender, EventArgs e)
        {
            if (!localPACItem.Checked)
            {
                localPACItem.Checked = true;
                onlinePACItem.Checked = false;
                controller.UseOnlinePAC(false);
                UpdatePACItemsEnabledStatus();
            }
        }

        private void OnlinePACItem_Click(object sender, EventArgs e)
        {
            if (!onlinePACItem.Checked)
            {
                if (controller.GetFreshConfiguration().pacUrl.IsNullOrEmpty())
                {
                    UpdateOnlinePACURLItem_Click(sender, e);
                }
                if (!controller.GetFreshConfiguration().pacUrl.IsNullOrEmpty())
                {
                    localPACItem.Checked = false;
                    onlinePACItem.Checked = true;
                    controller.UseOnlinePAC(true);
                }
                UpdatePACItemsEnabledStatus();
            }
        }

        private void UpdateOnlinePACURLItem_Click(object sender, EventArgs e)
        {
            string origPacUrl = controller.GetFreshConfiguration().pacUrl;
            string pacUrl = Microsoft.VisualBasic.Interaction.InputBox(
                I18N.GetString("Please input PAC Url"),
                I18N.GetString("Edit Online PAC URL"),
                origPacUrl, -1, -1);
            if (!pacUrl.IsNullOrEmpty() && pacUrl != origPacUrl)
            {
                controller.SavePACUrl(pacUrl);
            }
        }

        private void UpdatePACItemsEnabledStatus()
        {
            if (this.localPACItem.Checked)
            {
                this.editLocalPACItem.Enabled = true;
                this.updateFromGFWListItem.Enabled = true;
                this.editGFWUserRuleItem.Enabled = true;
                this.editOnlinePACItem.Enabled = false;
            }
            else
            {
                this.editLocalPACItem.Enabled = false;
                this.updateFromGFWListItem.Enabled = false;
                this.editGFWUserRuleItem.Enabled = false;
                this.editOnlinePACItem.Enabled = true;
            }
        }

        private void UpdateUpdateMenu()
        {
            Configuration configuration = controller.GetFreshConfiguration();
            autoCheckUpdatesToggleItem.Checked = configuration.autoCheckUpdate;
        }

        private void autoCheckUpdatesToggleItem_Click(object sender, EventArgs e)
        {
            Configuration configuration = controller.GetFreshConfiguration();
            controller.ToggleCheckingUpdate(!configuration.autoCheckUpdate);
            UpdateUpdateMenu();
        }

        private void checkUpdatesItem_Click(object sender, EventArgs e)
        {
            updateChecker.CheckUpdate(controller.GetFreshConfiguration());
        }

        private void proxyItem_Click(object sender, EventArgs e)
        {
            ShowProxyForm();
        }

        private void hotKeyItem_Click(object sender, EventArgs e)
        {
            ShowHotKeySettingsForm();
        }

        private void ShowLogItem_Click(object sender, EventArgs e)
        {
            ShowLogForm();
        }

        public void ShowLogForm_HotKey()
        {
            ShowLogForm();
        }
    }
}
