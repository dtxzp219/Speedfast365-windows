using Newtonsoft.Json.Linq;
using Shadowsocks.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThoughtWorks.QRCode.Codec;

namespace Shadowsocks.View
{
    public partial class PaymentForm : Form
    {
        public static PaymentForm _one = null;
        public static PaymentForm one()
        {
            if (_one == null)
            {
                _one = new PaymentForm();
            }
            return _one;
        }

        public PaymentForm()
        {
            InitializeComponent();
        }

        private void PaymentForm_Load(object sender, EventArgs e)
        {
            LoadPaymentList();
        }

        private void PayButton_Click(object sender, EventArgs e)
        {
        }

        private void LoadPaymentList()
        {
        }

        private void GoodCheckedList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
        }

        private void tableLayoutPanel1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void WeChat_Click(object sender, EventArgs e)
        {

        }

        private void AliPay_Click(object sender, EventArgs e)
        {

        }
    }

    public partial class Form2 : Form
    {
    }
}
