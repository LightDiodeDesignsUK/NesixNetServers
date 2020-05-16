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
    public partial class MonitorWindow : Form
    {
        public MonitorWindow()
        {
            InitializeComponent();
        }

        private void MonitorWindow_Load(object sender, EventArgs e)
        {
        }

   
        public delegate void ClientistBoxAddDelegate(string[] s, bool clearFirst);
        void AddToListBox(string[] s, bool CF)
        {
            if (CF) this.listView1.Items.Clear();
            ListViewItem lvi = new ListViewItem(s);
            this.listView1.Items.Add(lvi);
            lvi = null;
        }
        public void AddToClientListSafe(string[] s, bool clearFirst)
        {
            listView1.Invoke(new ClientistBoxAddDelegate(AddToListBox), new object[] { s, clearFirst });
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            ChatServerTypes.hTTPCW.ReportClients();
            string sTime = InboundMessageParser.GetSetDivString("*", "ActiveUsersDiv", DateTime.Now.ToLongTimeString());
            ChatServerTypes.hTTPCW.SendToAll(sTime);
        }
    }
}
