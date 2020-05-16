using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NesixChatServerV1
{
    public partial class ServerFrm : Form
    {
        public ServerFrm()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            NesixChatServerV1.ChatServerTypes.UpdateTextSafe(textBox1, "Starting Server \r\n");

            new ServerListenerHandler(this).Startup(6366);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            StartButton_Click(this, e);
            ((Timer)sender).Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ChatServerTypes.UiForm = this;
        }

        private void MonitorWindowBtn_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripButton tsb = (ToolStripButton)sender;
            if (NesixChatServerV1.ChatServerTypes.MWuiForm == null) ChatServerTypes.MWuiForm = new MonitorWindow();
            if (tsb.Checked) ChatServerTypes.MWuiForm.Show();
            else { ChatServerTypes.MWuiForm.Close(); ChatServerTypes.MWuiForm = null; }
        }
    }
}
