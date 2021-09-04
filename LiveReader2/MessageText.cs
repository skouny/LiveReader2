using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveReader2
{
    public partial class MessageText : Form
    {
        public MessageText(string format, params object[] args)
        {
            InitializeComponent();
            this.textBoxMain.Text = string.Format(format, args).Replace('\n'.ToString(), Environment.NewLine);
            this.Width = 480;
            this.Height = 360;
        }
        public static void Show(string format, params object[] args)
        {
            var msg = new MessageText(format, args);
            msg.Show();
        }
    }
}
