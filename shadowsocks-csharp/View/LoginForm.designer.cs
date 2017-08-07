using Shadowsocks.Controller;

namespace Shadowsocks.View
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.LoginButton = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.usernameTB = new System.Windows.Forms.TextBox();
            this.passwordTB = new System.Windows.Forms.TextBox();
            this.brandLabel = new System.Windows.Forms.Label();
            this.notifyLabel = new System.Windows.Forms.Label();
            this.SignupLabel = new System.Windows.Forms.LinkLabel();
            this.FindPasswordBackLabel = new System.Windows.Forms.LinkLabel();
            this.btn_close = new System.Windows.Forms.Button();
            this.btn_min = new System.Windows.Forms.Button();
            this.account_Pic = new System.Windows.Forms.PictureBox();
            this.password_Pic = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.account_Pic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.password_Pic)).BeginInit();
            this.SuspendLayout();
            // 
            // LoginButton
            // 
            this.LoginButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.LoginButton.BackColor = System.Drawing.Color.Transparent;
            this.LoginButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.LoginButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LoginButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LoginButton.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoginButton.ForeColor = System.Drawing.Color.Transparent;
            this.LoginButton.Location = new System.Drawing.Point(128, 210);
            this.LoginButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(145, 28);
            this.LoginButton.TabIndex = 0;
            this.LoginButton.Text = "登录 LOGIN";
            this.LoginButton.UseCompatibleTextRendering = true;
            this.LoginButton.UseVisualStyleBackColor = false;
            this.LoginButton.Click += new System.EventHandler(this.login);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // usernameTB
            // 
            this.usernameTB.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.usernameTB.BackColor = System.Drawing.Color.White;
            this.usernameTB.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernameTB.ForeColor = System.Drawing.Color.Black;
            this.usernameTB.Location = new System.Drawing.Point(80, 90);
            this.usernameTB.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.usernameTB.Name = "usernameTB";
            this.usernameTB.Size = new System.Drawing.Size(224, 26);
            this.usernameTB.TabIndex = 1;
            this.usernameTB.Enter += new System.EventHandler(this.usernameTB_Enter);
            this.usernameTB.Leave += new System.EventHandler(this.usernameTB_Leave);
            // 
            // passwordTB
            // 
            this.passwordTB.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.passwordTB.BackColor = System.Drawing.Color.White;
            this.passwordTB.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passwordTB.ForeColor = System.Drawing.Color.Black;
            this.passwordTB.Location = new System.Drawing.Point(80, 154);
            this.passwordTB.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.passwordTB.Name = "passwordTB";
            this.passwordTB.PasswordChar = '*';
            this.passwordTB.Size = new System.Drawing.Size(224, 26);
            this.passwordTB.TabIndex = 4;
            this.passwordTB.Enter += new System.EventHandler(this.passwordTB_Enter);
            this.passwordTB.Leave += new System.EventHandler(this.passwordTB_Leave);
            // 
            // brandLabel
            // 
            this.brandLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.brandLabel.BackColor = System.Drawing.Color.Transparent;
            this.brandLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.brandLabel.Font = new System.Drawing.Font("Arial", 16.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.brandLabel.ForeColor = System.Drawing.Color.White;
            this.brandLabel.Location = new System.Drawing.Point(109, 21);
            this.brandLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.brandLabel.Name = "brandLabel";
            this.brandLabel.Size = new System.Drawing.Size(164, 35);
            this.brandLabel.TabIndex = 7;
            this.brandLabel.Text = "Speedfast365";
            this.brandLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.brandLabel.Click += new System.EventHandler(this.brandLabel_Click);
            // 
            // notifyLabel
            // 
            this.notifyLabel.BackColor = System.Drawing.Color.Transparent;
            this.notifyLabel.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.notifyLabel.ForeColor = System.Drawing.Color.White;
            this.notifyLabel.Location = new System.Drawing.Point(56, 259);
            this.notifyLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.notifyLabel.Name = "notifyLabel";
            this.notifyLabel.Size = new System.Drawing.Size(291, 25);
            this.notifyLabel.TabIndex = 9;
            this.notifyLabel.Text = "您已登出";
            this.notifyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SignupLabel
            // 
            this.SignupLabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(202)))), ((int)(((byte)(164)))));
            this.SignupLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.SignupLabel.BackColor = System.Drawing.Color.Transparent;
            this.SignupLabel.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SignupLabel.LinkColor = System.Drawing.Color.White;
            this.SignupLabel.Location = new System.Drawing.Point(308, 92);
            this.SignupLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SignupLabel.Name = "SignupLabel";
            this.SignupLabel.Size = new System.Drawing.Size(39, 27);
            this.SignupLabel.TabIndex = 10;
            this.SignupLabel.TabStop = true;
            this.SignupLabel.Text = "注册";
            this.SignupLabel.UseCompatibleTextRendering = true;
            this.SignupLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SignupLabel_LinkClicked);
            // 
            // FindPasswordBackLabel
            // 
            this.FindPasswordBackLabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(202)))), ((int)(((byte)(164)))));
            this.FindPasswordBackLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.FindPasswordBackLabel.BackColor = System.Drawing.Color.Transparent;
            this.FindPasswordBackLabel.Font = new System.Drawing.Font("Microsoft YaHei", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FindPasswordBackLabel.LinkColor = System.Drawing.Color.White;
            this.FindPasswordBackLabel.Location = new System.Drawing.Point(308, 157);
            this.FindPasswordBackLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.FindPasswordBackLabel.Name = "FindPasswordBackLabel";
            this.FindPasswordBackLabel.Size = new System.Drawing.Size(72, 27);
            this.FindPasswordBackLabel.TabIndex = 12;
            this.FindPasswordBackLabel.TabStop = true;
            this.FindPasswordBackLabel.Text = "找回密码";
            this.FindPasswordBackLabel.UseCompatibleTextRendering = true;
            this.FindPasswordBackLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.FindPasswordBackLabel_LinkClicked);
            // 
            // btn_close
            // 
            this.btn_close.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_close.BackColor = System.Drawing.Color.Transparent;
            this.btn_close.FlatAppearance.BorderSize = 0;
            this.btn_close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_close.ForeColor = System.Drawing.Color.Transparent;
            this.btn_close.Image = global::Shadowsocks.Properties.Resources.btn_close_18;
            this.btn_close.Location = new System.Drawing.Point(374, 2);
            this.btn_close.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(24, 26);
            this.btn_close.TabIndex = 13;
            this.btn_close.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btn_close.UseVisualStyleBackColor = false;
            this.btn_close.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_min
            // 
            this.btn_min.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_min.BackColor = System.Drawing.Color.Transparent;
            this.btn_min.FlatAppearance.BorderSize = 0;
            this.btn_min.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_min.ForeColor = System.Drawing.Color.Transparent;
            this.btn_min.Image = global::Shadowsocks.Properties.Resources.btn_min_18;
            this.btn_min.Location = new System.Drawing.Point(345, 2);
            this.btn_min.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_min.Name = "btn_min";
            this.btn_min.Size = new System.Drawing.Size(24, 26);
            this.btn_min.TabIndex = 14;
            this.btn_min.UseVisualStyleBackColor = false;
            this.btn_min.Click += new System.EventHandler(this.btn_min_Click);
            // 
            // account_Pic
            // 
            this.account_Pic.BackgroundImage = global::Shadowsocks.Properties.Resources.user_96;
            this.account_Pic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.account_Pic.ErrorImage = null;
            this.account_Pic.InitialImage = null;
            this.account_Pic.Location = new System.Drawing.Point(52, 88);
            this.account_Pic.Name = "account_Pic";
            this.account_Pic.Size = new System.Drawing.Size(24, 24);
            this.account_Pic.TabIndex = 15;
            this.account_Pic.TabStop = false;
            // 
            // password_Pic
            // 
            this.password_Pic.BackgroundImage = global::Shadowsocks.Properties.Resources.pwd_96;
            this.password_Pic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.password_Pic.ErrorImage = null;
            this.password_Pic.InitialImage = null;
            this.password_Pic.Location = new System.Drawing.Point(52, 152);
            this.password_Pic.Name = "password_Pic";
            this.password_Pic.Size = new System.Drawing.Size(24, 24);
            this.password_Pic.TabIndex = 16;
            this.password_Pic.TabStop = false;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(50)))), ((int)(((byte)(65)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(400, 298);
            this.Controls.Add(this.password_Pic);
            this.Controls.Add(this.account_Pic);
            this.Controls.Add(this.btn_min);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.FindPasswordBackLabel);
            this.Controls.Add(this.SignupLabel);
            this.Controls.Add(this.notifyLabel);
            this.Controls.Add(this.brandLabel);
            this.Controls.Add(this.passwordTB);
            this.Controls.Add(this.usernameTB);
            this.Controls.Add(this.LoginButton);
            this.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "邮箱/手机号";
            this.Text = "Speedfast365加速";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoginForm_FormClosing);
            this.Load += new System.EventHandler(this.LoginForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.account_Pic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.password_Pic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.TextBox usernameTB;
        private System.Windows.Forms.TextBox passwordTB;
        private System.Windows.Forms.Label brandLabel;
        private System.Windows.Forms.Label notifyLabel;
        private System.Windows.Forms.LinkLabel SignupLabel;
        private System.Windows.Forms.LinkLabel FindPasswordBackLabel;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Button btn_min;
        private System.Windows.Forms.PictureBox account_Pic;
        private System.Windows.Forms.PictureBox password_Pic;
    }
}