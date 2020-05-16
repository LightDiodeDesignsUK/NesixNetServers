namespace NesixChatServerV1
{
    partial class MonitorWindow
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.IPCH = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.uch1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.IPCH,
            this.uch1,
            this.ch2,
            this.ch3,
            this.columnHeader1});
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(6, 15);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(412, 364);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // IPCH
            // 
            this.IPCH.Text = "IP";
            this.IPCH.Width = 54;
            // 
            // uch1
            // 
            this.uch1.Text = "Username";
            // 
            // ch2
            // 
            this.ch2.Text = "LastMessageText";
            this.ch2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ch2.Width = 160;
            // 
            // ch3
            // 
            this.ch3.Text = "Room";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "LastTime";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 501;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // MonitorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 400);
            this.Controls.Add(this.listView1);
            this.Name = "MonitorWindow";
            this.Text = "MonitorWindow";
            this.Load += new System.EventHandler(this.MonitorWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader IPCH;
        private System.Windows.Forms.ColumnHeader uch1;
        private System.Windows.Forms.ColumnHeader ch2;
        private System.Windows.Forms.ColumnHeader ch3;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        public System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.Timer timer2;
    }
}