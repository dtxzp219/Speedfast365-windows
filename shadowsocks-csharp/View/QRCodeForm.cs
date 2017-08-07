﻿using ZXing.QrCode.Internal;
using Shadowsocks.Controller;
using Shadowsocks.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shadowsocks.Model;

namespace Shadowsocks.View
{
    public partial class QRCodeForm : Form
    {
        private string code;

        public QRCodeForm(string code)
        {
            this.code = code;
            InitializeComponent();
            this.Icon = Icon.FromHandle(Resources.logo_128.GetHicon());
            this.Text = I18N.GetString("QRCode");
        }

        private void GenQR(string ssconfig)
        {
            string qrText = ssconfig;
            QRCode code = ZXing.QrCode.Internal.Encoder.encode(qrText, ErrorCorrectionLevel.M);
            ByteMatrix m = code.Matrix;
            int blockSize = Math.Max(pictureBox1.Height/m.Height, 1);
            Bitmap drawArea = new Bitmap((m.Width*blockSize), (m.Height*blockSize));
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.Clear(Color.White);
                using (Brush b = new SolidBrush(Color.Black))
                {
                    for (int row = 0; row < m.Width; row++)
                    {
                        for (int col = 0; col < m.Height; col++)
                        {
                            if (m[row, col] != 0)
                            {
                                g.FillRectangle(b, blockSize*row, blockSize*col, blockSize, blockSize);
                            }
                        }
                    }
                }
            }
            pictureBox1.Image = drawArea;
        }

        private void QRCodeForm_Load(object sender, EventArgs e)
        {
            var servers = Configuration.LoadFromProperties();
            var serverDatas = servers.configs.Select(
                server =>
                    new KeyValuePair<string, string>(ShadowsocksController.GetQRCode(server), server.FriendlyName())
                ).ToList();
            listBox1.DataSource = serverDatas;

            var selectIndex = serverDatas.FindIndex(serverData => serverData.Key.StartsWith(code));
            if (selectIndex >= 0) listBox1.SetSelected(selectIndex, true);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GenQR((sender as ListBox)?.SelectedValue.ToString());
        }
    }
}