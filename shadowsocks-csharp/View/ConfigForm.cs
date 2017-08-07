using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using Shadowsocks.Controller;
using Shadowsocks.Model;
using Shadowsocks.Properties;
using System.Runtime.InteropServices;
using System.Threading;
using Newtonsoft.Json.Linq;
using Shadowsocks.Util;
using Shadowsocks.Extensions;


namespace Shadowsocks.View
{
    public partial class ConfigForm : Form
    {
        // this is a copy of configuration that we are working on
        private ShadowsocksController controller = ShadowsocksController.one();
        private Configuration config;
        private int _lastSelectedIndex = -1;
        public Thread _trfThread;
        delegate void SetTextCallback(string text);
        public ConfigForm()
        {
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;
            InitializeComponent();

            this._trfThread = new Thread(new ThreadStart(this.UpdateTrafficDynamic));
            this._trfThread.IsBackground = true;
            this._trfThread.Start();
            // a dirty hack
            this.ServersListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PerformLayout();

            UpdateTexts();
            this.Icon = Resources.icon_128_b;
            ShadowsocksController.one().ConfigChanged += controller_ConfigChanged;
            LoadCurrentConfiguration();

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        ///定义无边框窗体Form
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;

        //禁用最大化
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")] //导入API函数
        public static extern System.IntPtr GetSystemMenu(System.IntPtr hWnd, System.IntPtr bRevert);
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        extern static int RemoveMenu(IntPtr hMenu, int nPos, int flags);
        private const int MF_BYPOSITION = 0x400;
        private const int MF_REMOVE = 0x1000;
        private const int SC_MAXISIZE = 0xf030;//最大化
        private const int WM_NCLBUTTONDBLCLK = 0xA3;//鼠标双击标题栏消息

        private void UpdateTexts()
        {
            this.Text = I18N.GetString("Speedfast365");
        }

        private void controller_ConfigChanged(object sender, EventArgs e)
        {
            LoadCurrentConfiguration();
        }

        private void ShowWindow()
        {
            this.Opacity = 1;
            this.Show();
        }

        private void LoadSelectedServer()
        {
            if (ServersListBox.SelectedIndex >= 0 && ServersListBox.SelectedIndex < config.configs.Count)
            {
                Server server = config.configs[ServersListBox.SelectedIndex];
            }
        }

        private void LoadConfiguration(Configuration configuration)
        {
            ServersListBox.Items.Clear();
            foreach (Server server in config.configs)
            {
                ServersListBox.Items.Add(server.FriendlyName());
            }
        }

        private void LoadCurrentConfiguration()
        {
            config = controller.GetCurrentConfiguration();
            LoadConfiguration(config);
            _lastSelectedIndex = config.index;
            if (0 <= _lastSelectedIndex && _lastSelectedIndex < ServersListBox.Items.Count)
            {
                ServersListBox.SelectedIndex = _lastSelectedIndex;
            }
            ModeSelectButton.Text = I18N.GetString(config.global ?  "Smart Mode" : "Global Mode");
            EnableButton.Text = I18N.GetString((config.enabled ?  "Disable" : "Enable") + " System Proxy");
            StateLabel.Text = I18N.GetString(config.enabled ? "Enable" :  "Disable") + " / " + I18N.GetString(config.global ?  "Global Mode" : "Smart Mode");
            TrafficLabel.Text = MenuViewController.one().LeftTraffic;
            LoadSelectedServer();
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            TopPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(ControlMouseDown);//添加拖动窗口事件
            TopLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(ControlMouseDown);
            //picture_icon.MouseDown += new System.Windows.Forms.MouseEventHandler(ControlMouseDown);
            BottomPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(ControlMouseDown);
            StateLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(ControlMouseDown);
            TrafficLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(ControlMouseDown);

            ServersListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;//重绘listbox
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    if ((int)m.Result == HTCLIENT) m.Result = (IntPtr)HTCAPTION; return;
                case WM_NCLBUTTONDBLCLK:
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        /// 控件拖动窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlMouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        private void ConfigForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Sometimes the users may hit enter key by mistake, and the form will close without saving entries.

            if (e.KeyCode == Keys.Enter)
            {
                Server server = controller.GetCurrentServer();
                if (config.configs.Count == 0)
                {
                    MessageBox.Show(I18N.GetString("Please add at least one server"));
                    return;
                }
                controller.SaveServers(config.configs, config.localPort);
                controller.SelectServerIndex(config.configs.IndexOf(server));
            }

        }

        private void ServersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ServersListBox.CanSelect)
            {
                return;
            }

            if (_lastSelectedIndex == ServersListBox.SelectedIndex)
            {
                // we are moving back to oldSelectedIndex or doing a force move
                return;
            }
            if (_lastSelectedIndex >= 0)
            {
                ServersListBox.Items[_lastSelectedIndex] = config.configs[_lastSelectedIndex].FriendlyName();
            }
            _lastSelectedIndex = ServersListBox.SelectedIndex;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ConfigForm_Shown(object sender, EventArgs e)
        {
        }

        private void ConfigForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            controller.ConfigChanged -= controller_ConfigChanged;
        }

        private void ModeSelectButton_Click(object sender, EventArgs e)
        {
            controller.ToggleGlobal(!config.global);
        }

        private void ServersListBox_DoubleClick(object sender, EventArgs e)
        {
            if (ServersListBox.SelectedIndex >= 0 && ServersListBox.SelectedIndex < config.configs.Count)
            {
                controller.SelectServerIndex(ServersListBox.SelectedIndex);
                //Server server = config.configs[ServersListBox.SelectedIndex];
            }
        }

        private void EnableButton_Click(object sender, EventArgs e)
        {
            controller.ToggleEnable(!config.enabled);
        }

        private void btn_min_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
            Utils.ReleaseMemory(true);
        }

        private void UpdateTrafficDynamic()
        {
            int recharge_notice_count = 0;

            while (true)
            {
                if (!this.IsDisposed)
                {
                    try
                    {
                        //JObject notification = Request.one().Post("rallets_notification", encrypt: true);
                        JObject notification = Request.one().Post("index_notification", encrypt: false);

                        controller.AssignAndReloadConfigsIfDiff(notification["configs"]);//add

                        JToken traffic = notification.SelectToken("self.traffic") ?? new JObject();
                        double GB = Math.Pow(1024, 3);
                        double left_traffic = (double)traffic["premium"];
                        if (left_traffic > 0)
                        {
                            String leftTraffic = I18N.GetString("Left Traffic") + ": " +
                                Util.Utils.Truncate(((double)(traffic["premium"] ?? 0) / GB), 2) + "G" + "\n";
                            SetText(leftTraffic);
                        }
                        else {
                            SetText("流量已经用完，请充值");
                        }

                        JToken expire_dateline = notification.SelectToken("self.expire_dateline") ?? new JObject();
                        String ExpireTime = I18N.GetString("Expire Time") + ": " + expire_dateline["premium"].ToString();
                        SetText_Expire(ExpireTime);
                        String need_recharge = notification.SelectToken("self.need_recharge").ToString();
                        if (need_recharge.Equals("yes")) {
                            //if (MessageBox.Show(I18N.GetString("Please Recharge"), MessageBoxButtons.OKCancel) == DialogResult.OK)
                            if (recharge_notice_count < 1)
                            {
                                recharge_notice_count += 1;
                                if (MessageBox.Show(I18N.GetString("Speedfast365 Need Recharge")) == DialogResult.OK)
                                {
                                    Process.Start("http://45.79.96.118/member/alipayrecharge");
                                }
                            }
                        }
                    }
                    catch { }
                }
                Thread.Sleep(20*1000);
            }

        }

        private void SetText(string text)
        {
            if (this.TrafficLabel.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.TrafficLabel.Text = text;
            }
        }

        private void SetText_Expire(string text)
        {
            if (this.ExpireLabel.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText_Expire);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.ExpireLabel.Text = text;
            }
            
        }

        private void ServersListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1) return;
            Graphics g = e.Graphics;
            string s = this.ServersListBox.Items[e.Index].ToString();
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                TextRenderer.DrawText(g, s, this.ServersListBox.Font, e.Bounds, Color.FromArgb(255, 12, 189),
                              TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                //e.Graphics.DrawString(s, this.Font, Brushes.DeepPink, e.Bounds);
            }
            else
            {
                TextRenderer.DrawText(g, s, this.ServersListBox.Font, e.Bounds, this.ServersListBox.ForeColor,
                              TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                //e.Graphics.DrawString(s, this.Font, Brushes.DeepSkyBlue, e.Bounds);
            }
                
        }

        private void ExpireLabel_Click(object sender, EventArgs e)
        {
            Process.Start("http://45.79.96.118/");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void TrafficLabel_Click(object sender, EventArgs e)
        {
            
        }
    }
}
