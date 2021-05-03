
namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBox_ServerAddressList = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_workerNum = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_password = new System.Windows.Forms.TextBox();
            this.button_open = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label_agentState = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label_agentName = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label_agentId = new System.Windows.Forms.Label();
            this.listView_Groups = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.numericUpDown_mainServerIndex = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.listView_sipAccounts = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.textBox_ringInfo = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.numericUpDown_ReqNum2 = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox_ReqContent2 = new System.Windows.Forms.TextBox();
            this.numericUpDown_ReqType2 = new System.Windows.Forms.NumericUpDown();
            this.button9 = new System.Windows.Forms.Button();
            this.textBox_reqRes = new System.Windows.Forms.TextBox();
            this.checkBox_forceClose = new System.Windows.Forms.CheckBox();
            this.listView_connections = new System.Windows.Forms.ListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_mainServerIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReqNum2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReqType2)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox_ServerAddressList
            // 
            this.textBox_ServerAddressList.Location = new System.Drawing.Point(142, 11);
            this.textBox_ServerAddressList.Name = "textBox_ServerAddressList";
            this.textBox_ServerAddressList.Size = new System.Drawing.Size(541, 21);
            this.textBox_ServerAddressList.TabIndex = 0;
            this.textBox_ServerAddressList.Text = "192.168.2.107,192.168.2.108";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "服务器列表(\",\"分隔)";
            // 
            // textBox_workerNum
            // 
            this.textBox_workerNum.Location = new System.Drawing.Point(52, 40);
            this.textBox_workerNum.Name = "textBox_workerNum";
            this.textBox_workerNum.Size = new System.Drawing.Size(100, 21);
            this.textBox_workerNum.TabIndex = 3;
            this.textBox_workerNum.Text = "1000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "工号";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(171, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "密码";
            // 
            // textBox_password
            // 
            this.textBox_password.Location = new System.Drawing.Point(206, 40);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.Size = new System.Drawing.Size(100, 21);
            this.textBox_password.TabIndex = 5;
            this.textBox_password.Text = "1000";
            // 
            // button_open
            // 
            this.button_open.Location = new System.Drawing.Point(312, 38);
            this.button_open.Name = "button_open";
            this.button_open.Size = new System.Drawing.Size(75, 23);
            this.button_open.TabIndex = 6;
            this.button_open.Text = "上线";
            this.button_open.UseVisualStyleBackColor = true;
            this.button_open.Click += new System.EventHandler(this.button_open_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 261);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "技能组";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(393, 38);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "下线";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(577, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "AgentState:";
            // 
            // label_agentState
            // 
            this.label_agentState.AutoSize = true;
            this.label_agentState.Location = new System.Drawing.Point(654, 76);
            this.label_agentState.Name = "label_agentState";
            this.label_agentState.Size = new System.Drawing.Size(29, 12);
            this.label_agentState.TabIndex = 11;
            this.label_agentState.Text = "....";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(577, 38);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "AgentID:";
            // 
            // label_agentName
            // 
            this.label_agentName.AutoSize = true;
            this.label_agentName.Location = new System.Drawing.Point(648, 59);
            this.label_agentName.Name = "label_agentName";
            this.label_agentName.Size = new System.Drawing.Size(29, 12);
            this.label_agentName.TabIndex = 13;
            this.label_agentName.Text = "....";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(7, 109);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(101, 23);
            this.button2.TabIndex = 18;
            this.button2.Text = "Sign in - All";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(114, 109);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(99, 23);
            this.button3.TabIndex = 19;
            this.button3.Text = "Sign out - All";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(577, 59);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 20;
            this.label9.Text = "AgentName:";
            // 
            // label_agentId
            // 
            this.label_agentId.AutoSize = true;
            this.label_agentId.Location = new System.Drawing.Point(648, 35);
            this.label_agentId.Name = "label_agentId";
            this.label_agentId.Size = new System.Drawing.Size(29, 12);
            this.label_agentId.TabIndex = 21;
            this.label_agentId.Text = "....";
            // 
            // listView_Groups
            // 
            this.listView_Groups.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView_Groups.ContextMenuStrip = this.contextMenuStrip1;
            this.listView_Groups.FullRowSelect = true;
            this.listView_Groups.HideSelection = false;
            this.listView_Groups.Location = new System.Drawing.Point(7, 276);
            this.listView_Groups.Name = "listView_Groups";
            this.listView_Groups.Size = new System.Drawing.Size(256, 110);
            this.listView_Groups.TabIndex = 22;
            this.listView_Groups.UseCompatibleStateImageBehavior = false;
            this.listView_Groups.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 89;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 88;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(127, 48);
            this.contextMenuStrip1.Text = "Sign in or out of Agent Group";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(126, 22);
            this.toolStripMenuItem1.Text = "Sign In";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(126, 22);
            this.toolStripMenuItem2.Text = "Sign Out";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(231, 109);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 23;
            this.button4.Text = "示闲";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(312, 109);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 24;
            this.button5.Text = "示忙";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // numericUpDown_mainServerIndex
            // 
            this.numericUpDown_mainServerIndex.Location = new System.Drawing.Point(122, 67);
            this.numericUpDown_mainServerIndex.Name = "numericUpDown_mainServerIndex";
            this.numericUpDown_mainServerIndex.Size = new System.Drawing.Size(57, 21);
            this.numericUpDown_mainServerIndex.TabIndex = 25;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(21, 73);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 12);
            this.label10.TabIndex = 26;
            this.label10.Text = "MainServerIndex";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(185, 64);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(42, 23);
            this.button6.TabIndex = 27;
            this.button6.Text = "Set";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // listView_sipAccounts
            // 
            this.listView_sipAccounts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5});
            this.listView_sipAccounts.HideSelection = false;
            this.listView_sipAccounts.Location = new System.Drawing.Point(269, 276);
            this.listView_sipAccounts.Name = "listView_sipAccounts";
            this.listView_sipAccounts.Size = new System.Drawing.Size(726, 110);
            this.listView_sipAccounts.TabIndex = 28;
            this.listView_sipAccounts.UseCompatibleStateImageBehavior = false;
            this.listView_sipAccounts.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Width = 217;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Width = 466;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(393, 109);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 23);
            this.button7.TabIndex = 29;
            this.button7.Text = "接听";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(474, 109);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(75, 23);
            this.button8.TabIndex = 30;
            this.button8.Text = "挂断";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // textBox_ringInfo
            // 
            this.textBox_ringInfo.Location = new System.Drawing.Point(308, 67);
            this.textBox_ringInfo.Name = "textBox_ringInfo";
            this.textBox_ringInfo.ReadOnly = true;
            this.textBox_ringInfo.Size = new System.Drawing.Size(263, 21);
            this.textBox_ringInfo.TabIndex = 31;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(253, 69);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 32;
            this.label11.Text = "振铃信息";
            // 
            // numericUpDown_ReqNum2
            // 
            this.numericUpDown_ReqNum2.Location = new System.Drawing.Point(64, 167);
            this.numericUpDown_ReqNum2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numericUpDown_ReqNum2.Maximum = new decimal(new int[] {
            0,
            1,
            0,
            0});
            this.numericUpDown_ReqNum2.Name = "numericUpDown_ReqNum2";
            this.numericUpDown_ReqNum2.Size = new System.Drawing.Size(52, 21);
            this.numericUpDown_ReqNum2.TabIndex = 38;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(64, 153);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(41, 12);
            this.label12.TabIndex = 36;
            this.label12.Text = "MsgNum";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(121, 153);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(65, 12);
            this.label13.TabIndex = 37;
            this.label13.Text = "MsgContent";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(7, 154);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 12);
            this.label14.TabIndex = 35;
            this.label14.Text = "MsgType";
            // 
            // textBox_ReqContent2
            // 
            this.textBox_ReqContent2.Location = new System.Drawing.Point(121, 167);
            this.textBox_ReqContent2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_ReqContent2.Name = "textBox_ReqContent2";
            this.textBox_ReqContent2.Size = new System.Drawing.Size(133, 21);
            this.textBox_ReqContent2.TabIndex = 34;
            // 
            // numericUpDown_ReqType2
            // 
            this.numericUpDown_ReqType2.Location = new System.Drawing.Point(8, 168);
            this.numericUpDown_ReqType2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numericUpDown_ReqType2.Maximum = new decimal(new int[] {
            0,
            1,
            0,
            0});
            this.numericUpDown_ReqType2.Name = "numericUpDown_ReqType2";
            this.numericUpDown_ReqType2.Size = new System.Drawing.Size(52, 21);
            this.numericUpDown_ReqType2.TabIndex = 33;
            this.numericUpDown_ReqType2.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(260, 165);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(117, 23);
            this.button9.TabIndex = 39;
            this.button9.Text = "Send Request";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // textBox_reqRes
            // 
            this.textBox_reqRes.Location = new System.Drawing.Point(7, 193);
            this.textBox_reqRes.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_reqRes.Name = "textBox_reqRes";
            this.textBox_reqRes.ReadOnly = true;
            this.textBox_reqRes.Size = new System.Drawing.Size(370, 21);
            this.textBox_reqRes.TabIndex = 40;
            // 
            // checkBox_forceClose
            // 
            this.checkBox_forceClose.AutoSize = true;
            this.checkBox_forceClose.Location = new System.Drawing.Point(471, 40);
            this.checkBox_forceClose.Name = "checkBox_forceClose";
            this.checkBox_forceClose.Size = new System.Drawing.Size(90, 16);
            this.checkBox_forceClose.TabIndex = 41;
            this.checkBox_forceClose.Text = "force close";
            this.checkBox_forceClose.UseVisualStyleBackColor = true;
            // 
            // listView_connections
            // 
            this.listView_connections.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
            this.listView_connections.HideSelection = false;
            this.listView_connections.Location = new System.Drawing.Point(727, 11);
            this.listView_connections.Name = "listView_connections";
            this.listView_connections.Size = new System.Drawing.Size(286, 97);
            this.listView_connections.TabIndex = 42;
            this.listView_connections.UseCompatibleStateImageBehavior = false;
            this.listView_connections.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Width = 107;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Width = 76;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1054, 450);
            this.Controls.Add(this.listView_connections);
            this.Controls.Add(this.checkBox_forceClose);
            this.Controls.Add(this.textBox_reqRes);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.numericUpDown_ReqNum2);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.textBox_ReqContent2);
            this.Controls.Add(this.numericUpDown_ReqType2);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.textBox_ringInfo);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.listView_sipAccounts);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.numericUpDown_mainServerIndex);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.listView_Groups);
            this.Controls.Add(this.label_agentId);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label_agentName);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label_agentState);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button_open);
            this.Controls.Add(this.textBox_password);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_workerNum);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_ServerAddressList);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_mainServerIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReqNum2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReqType2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_ServerAddressList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_workerNum;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_password;
        private System.Windows.Forms.Button button_open;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label_agentState;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label_agentName;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label_agentId;
        private System.Windows.Forms.ListView listView_Groups;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.NumericUpDown numericUpDown_mainServerIndex;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.ListView listView_sipAccounts;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.TextBox textBox_ringInfo;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown numericUpDown_ReqNum2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBox_ReqContent2;
        private System.Windows.Forms.NumericUpDown numericUpDown_ReqType2;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.TextBox textBox_reqRes;
        private System.Windows.Forms.CheckBox checkBox_forceClose;
        private System.Windows.Forms.ListView listView_connections;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
    }
}

