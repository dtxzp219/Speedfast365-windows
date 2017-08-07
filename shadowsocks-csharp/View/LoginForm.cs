using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Shadowsocks.Controller;
using Shadowsocks.Model;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Shadowsocks.Util;
using Shadowsocks.Extensions;
using Shadowsocks.Exceptions;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Shadowsocks.View
{
    public partial class LoginForm : Form
    {
        public static Color ralletsGreen = Color.FromArgb(9, 170, 224);
        private ShadowsocksController controller = ShadowsocksController.one();
        private NotificationRunner notificationRunner = NotificationRunner.one();
        private bool EnableLogin = true;
        private bool usernameTBUsingPlaceholder = false;

        public static LoginForm _one = null;
        public static LoginForm one()
        {
            if (_one == null)
            {
                _one = new LoginForm();
            }
            return _one;
        }
        public LoginForm()
        {
            InitializeComponent();
            
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        ///定义无边框窗体Form
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;

        //拖拽窗体
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        //禁用最大化
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")] //导入API函数
        public static extern System.IntPtr GetSystemMenu(System.IntPtr hWnd, System.IntPtr bRevert);
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        extern static int RemoveMenu(IntPtr hMenu, int nPos, int flags);
        private const int MF_BYPOSITION = 0x400;
        private const int MF_REMOVE = 0x1000;
        private const int SC_MAXISIZE = 0xf030;//最大化
        private const int WM_NCLBUTTONDBLCLK = 0xA3;//鼠标双击标题栏消息


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

        protected override CreateParams CreateParams
        {
            get
            {
                var parms = base.CreateParams;
                parms.Style &= ~0x02000000; // Turn off WS_CLIPCHILDREN 
                return parms; //防闪烁
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
         //   RemoveMenu(GetSystemMenu(Handle, IntPtr.Zero), 0, MF_BYPOSITION | MF_REMOVE);
        }

        public void showWarningTitle(string text)
        {
            notifyLabel.Text = text;
            notifyLabel.ForeColor = ralletsGreen;
        }

        public void ShowTitle(string text)
        {
            notifyLabel.ForeColor = Color.White;
            notifyLabel.Text = text;
        }

        public void EnableLoginButton(bool enabled)
        {
            EnableLogin = enabled;
        }

        private void login(object sender, EventArgs e)
        {
            if (!EnableLogin)
            {
                return;
            }
            EnableLoginButton(false);
            var data = new JObject();
            var username = usernameTB.Text;
            var password = passwordTB.Text;
            var user = User.one();
            user.username = username;
            user.password = password;
            data["username_or_email"] = username;
            data["login_password"] = password;
            notifyLabel.Text = I18N.GetString("Connecting server, please wait...");
            var form = this;
            try
            {
                JObject res = Request.one().Post("login", data);
                if (res.ok())
                {
                    user.sessionId = (String)res["session_id"];
                    ShowTitle(I18N.GetString("Welcome to Speedfast365"));
                    Hide();
                    User.one().FetchRalletsNotification();
                }
                else
                {
                    showWarningTitle((string)res["message"]);
                }
            }
            catch (RalletsException)
            {
                showWarningTitle(I18N.GetString("Server is busy, please try login later"));
            }
            catch (Exception)
            {
                showWarningTitle(string.Format(I18N.GetString("Server is not available, please report this issue to {0}"), "rallets@126.com"));
            }
            finally
            {
                EnableLoginButton(true);
            }
        }


        private void LoginForm_Load(object sender, EventArgs eventArgs)
        {
            brandLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(ControlMouseDown);//添加拖动窗口事件
            //pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(ControlMouseDown);
            account_Pic.MouseDown += new System.Windows.Forms.MouseEventHandler(ControlMouseDown);
            password_Pic.MouseDown += new System.Windows.Forms.MouseEventHandler(ControlMouseDown);
            notifyLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(ControlMouseDown);
    
            var user = User.one();
            if (string.IsNullOrEmpty(user.username))
            {
                usernameTB.SetWatermark(I18N.GetString("Email or Mobile Number"));
            } else
            {
                usernameTB.ClearWatermark();
                usernameTB.Text = user.username;
            }
            
           
            if (string.IsNullOrEmpty(user.password))
            {
                passwordTB.SetWatermark(I18N.GetString("Password"));
            }
            else
            {
                passwordTB.ClearWatermark();
                passwordTB.Text = user.password;
            }
        
            passwordTB.KeyDown += new KeyEventHandler((s, e) =>
            {
                if (e.KeyValue == 13)
                {
                    this.login(sender, e);
                }
            });

        }

        private void usernameTB_Leave(object sender, EventArgs e)
        {
            if( String.IsNullOrEmpty(usernameTB.Text))
                usernameTB.SetWatermark(I18N.GetString("Email or Mobile Number"));
        }

        private void usernameTB_Enter(object sender, EventArgs e)
        {
            usernameTB.ClearWatermark();
        }

        private void passwordTB_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(passwordTB.Text))
                passwordTB.SetWatermark(I18N.GetString("Password"));
        }

        private void passwordTB_Enter(object sender, EventArgs e)
        {
            passwordTB.ClearWatermark();
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void SignupLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Url.SIGNUP_URL);
        }
        private void FindPasswordBackLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Url.FIND_PASSWORD_URL);
        }

        private void btn_min_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void brandLabel_Click(object sender, EventArgs e)
        {

        }
    }

    public static class TextBoxToolV2
    {
        private const uint ECM_FIRST = 0x1500;
        private const uint EM_SETCUEBANNER = ECM_FIRST + 1;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        /// <summary>
        /// 为TextBox设置水印文字
        /// </summary>
        /// <param name="textBox">TextBox</param>
        /// <param name="watermark">水印文字</param>
        public static void SetWatermark(this TextBox textBox, string watermark)
        {
            SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, watermark);
        }
        /// <summary>
        /// 清除水印文字
        /// </summary>
        /// <param name="textBox">TextBox</param>
        public static void ClearWatermark(this TextBox textBox)
        {
            SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, string.Empty);
        }
    }
}
