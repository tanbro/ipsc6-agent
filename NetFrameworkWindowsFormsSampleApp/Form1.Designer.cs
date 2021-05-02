
namespace NetFrameworkWindowsFormsSampleApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Server1 = new System.Windows.Forms.TextBox();
            this.textBox_Server2 = new System.Windows.Forms.TextBox();
            this.button_Connect1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_Disconnect1 = new System.Windows.Forms.Button();
            this.numericUpDown_ReqNum1 = new System.Windows.Forms.NumericUpDown();
            this.button_Req1 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_ReqContent1 = new System.Windows.Forms.TextBox();
            this.numericUpDown_ReqType1 = new System.Windows.Forms.NumericUpDown();
            this.textBox_Psw1 = new System.Windows.Forms.TextBox();
            this.textBox_User1 = new System.Windows.Forms.TextBox();
            this.label_ConnectStatus1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericUpDown_ReqNum2 = new System.Windows.Forms.NumericUpDown();
            this.textBox_Psw2 = new System.Windows.Forms.TextBox();
            this.button_Req2 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_User2 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.button_Disconnect2 = new System.Windows.Forms.Button();
            this.textBox_ReqContent2 = new System.Windows.Forms.TextBox();
            this.label_ConnectStatus2 = new System.Windows.Forms.Label();
            this.numericUpDown_ReqType2 = new System.Windows.Forms.NumericUpDown();
            this.button_Connect2 = new System.Windows.Forms.Button();
            this.textBox_Log1 = new System.Windows.Forms.TextBox();
            this.textBox_Log2 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label_SipReg1 = new System.Windows.Forms.Label();
            this.label_SipReg2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.checkedListBox_AudCapture = new System.Windows.Forms.CheckedListBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.checkedListBox_AudPlayback = new System.Windows.Forms.CheckedListBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.SIP2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReqNum1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReqType1)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReqNum2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReqType2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Server";
            // 
            // textBox_Server1
            // 
            this.textBox_Server1.Location = new System.Drawing.Point(66, 29);
            this.textBox_Server1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_Server1.Name = "textBox_Server1";
            this.textBox_Server1.Size = new System.Drawing.Size(151, 21);
            this.textBox_Server1.TabIndex = 2;
            this.textBox_Server1.Text = "192.168.2.108";
            // 
            // textBox_Server2
            // 
            this.textBox_Server2.Location = new System.Drawing.Point(69, 25);
            this.textBox_Server2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_Server2.Name = "textBox_Server2";
            this.textBox_Server2.Size = new System.Drawing.Size(151, 21);
            this.textBox_Server2.TabIndex = 3;
            this.textBox_Server2.Text = "192.168.2.107";
            // 
            // button_Connect1
            // 
            this.button_Connect1.Location = new System.Drawing.Point(204, 61);
            this.button_Connect1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_Connect1.Name = "button_Connect1";
            this.button_Connect1.Size = new System.Drawing.Size(64, 24);
            this.button_Connect1.TabIndex = 4;
            this.button_Connect1.Text = "Connect";
            this.button_Connect1.UseVisualStyleBackColor = true;
            this.button_Connect1.Click += new System.EventHandler(this.button_Connect1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_Disconnect1);
            this.groupBox1.Controls.Add(this.numericUpDown_ReqNum1);
            this.groupBox1.Controls.Add(this.button_Req1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBox_ReqContent1);
            this.groupBox1.Controls.Add(this.numericUpDown_ReqType1);
            this.groupBox1.Controls.Add(this.textBox_Psw1);
            this.groupBox1.Controls.Add(this.textBox_User1);
            this.groupBox1.Controls.Add(this.label_ConnectStatus1);
            this.groupBox1.Controls.Add(this.textBox_Server1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button_Connect1);
            this.groupBox1.Location = new System.Drawing.Point(23, 29);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(355, 203);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // button_Disconnect1
            // 
            this.button_Disconnect1.Location = new System.Drawing.Point(273, 61);
            this.button_Disconnect1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_Disconnect1.Name = "button_Disconnect1";
            this.button_Disconnect1.Size = new System.Drawing.Size(64, 24);
            this.button_Disconnect1.TabIndex = 16;
            this.button_Disconnect1.Text = "Close";
            this.button_Disconnect1.UseVisualStyleBackColor = true;
            this.button_Disconnect1.Click += new System.EventHandler(this.button_Disconnect1_Click);
            // 
            // numericUpDown_ReqNum1
            // 
            this.numericUpDown_ReqNum1.Location = new System.Drawing.Point(98, 164);
            this.numericUpDown_ReqNum1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numericUpDown_ReqNum1.Maximum = new decimal(new int[] {
            0,
            1,
            0,
            0});
            this.numericUpDown_ReqNum1.Name = "numericUpDown_ReqNum1";
            this.numericUpDown_ReqNum1.Size = new System.Drawing.Size(70, 21);
            this.numericUpDown_ReqNum1.TabIndex = 15;
            // 
            // button_Req1
            // 
            this.button_Req1.Location = new System.Drawing.Point(299, 164);
            this.button_Req1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_Req1.Name = "button_Req1";
            this.button_Req1.Size = new System.Drawing.Size(49, 22);
            this.button_Req1.TabIndex = 14;
            this.button_Req1.Text = "Send Request";
            this.button_Req1.UseVisualStyleBackColor = true;
            this.button_Req1.Click += new System.EventHandler(this.button_Req1_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(96, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "MsgNum";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(187, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 13;
            this.label4.Text = "MsgContent";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 151);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "MsgType";
            // 
            // textBox_ReqContent1
            // 
            this.textBox_ReqContent1.Location = new System.Drawing.Point(174, 164);
            this.textBox_ReqContent1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_ReqContent1.Name = "textBox_ReqContent1";
            this.textBox_ReqContent1.Size = new System.Drawing.Size(119, 21);
            this.textBox_ReqContent1.TabIndex = 11;
            // 
            // numericUpDown_ReqType1
            // 
            this.numericUpDown_ReqType1.Location = new System.Drawing.Point(23, 165);
            this.numericUpDown_ReqType1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numericUpDown_ReqType1.Maximum = new decimal(new int[] {
            0,
            1,
            0,
            0});
            this.numericUpDown_ReqType1.Name = "numericUpDown_ReqType1";
            this.numericUpDown_ReqType1.Size = new System.Drawing.Size(69, 21);
            this.numericUpDown_ReqType1.TabIndex = 10;
            this.numericUpDown_ReqType1.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // textBox_Psw1
            // 
            this.textBox_Psw1.Location = new System.Drawing.Point(115, 64);
            this.textBox_Psw1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_Psw1.Name = "textBox_Psw1";
            this.textBox_Psw1.Size = new System.Drawing.Size(86, 21);
            this.textBox_Psw1.TabIndex = 8;
            this.textBox_Psw1.Text = "1001";
            // 
            // textBox_User1
            // 
            this.textBox_User1.Location = new System.Drawing.Point(23, 64);
            this.textBox_User1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_User1.Name = "textBox_User1";
            this.textBox_User1.Size = new System.Drawing.Size(86, 21);
            this.textBox_User1.TabIndex = 7;
            this.textBox_User1.Text = "1001";
            // 
            // label_ConnectStatus1
            // 
            this.label_ConnectStatus1.AutoSize = true;
            this.label_ConnectStatus1.Location = new System.Drawing.Point(223, 31);
            this.label_ConnectStatus1.Name = "label_ConnectStatus1";
            this.label_ConnectStatus1.Size = new System.Drawing.Size(41, 12);
            this.label_ConnectStatus1.TabIndex = 5;
            this.label_ConnectStatus1.Text = "label3";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numericUpDown_ReqNum2);
            this.groupBox2.Controls.Add(this.textBox_Psw2);
            this.groupBox2.Controls.Add(this.button_Req2);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.textBox_User2);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.button_Disconnect2);
            this.groupBox2.Controls.Add(this.textBox_ReqContent2);
            this.groupBox2.Controls.Add(this.label_ConnectStatus2);
            this.groupBox2.Controls.Add(this.numericUpDown_ReqType2);
            this.groupBox2.Controls.Add(this.button_Connect2);
            this.groupBox2.Controls.Add(this.textBox_Server2);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(394, 31);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Size = new System.Drawing.Size(355, 200);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // numericUpDown_ReqNum2
            // 
            this.numericUpDown_ReqNum2.Location = new System.Drawing.Point(77, 162);
            this.numericUpDown_ReqNum2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numericUpDown_ReqNum2.Maximum = new decimal(new int[] {
            0,
            1,
            0,
            0});
            this.numericUpDown_ReqNum2.Name = "numericUpDown_ReqNum2";
            this.numericUpDown_ReqNum2.Size = new System.Drawing.Size(52, 21);
            this.numericUpDown_ReqNum2.TabIndex = 15;
            // 
            // textBox_Psw2
            // 
            this.textBox_Psw2.Location = new System.Drawing.Point(110, 59);
            this.textBox_Psw2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_Psw2.Name = "textBox_Psw2";
            this.textBox_Psw2.Size = new System.Drawing.Size(86, 21);
            this.textBox_Psw2.TabIndex = 12;
            this.textBox_Psw2.Text = "1001";
            // 
            // button_Req2
            // 
            this.button_Req2.Location = new System.Drawing.Point(272, 163);
            this.button_Req2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_Req2.Name = "button_Req2";
            this.button_Req2.Size = new System.Drawing.Size(64, 16);
            this.button_Req2.TabIndex = 14;
            this.button_Req2.Text = "Send Request";
            this.button_Req2.UseVisualStyleBackColor = true;
            this.button_Req2.Click += new System.EventHandler(this.button_Req2_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(77, 148);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 12);
            this.label8.TabIndex = 13;
            this.label8.Text = "MsgNum";
            // 
            // textBox_User2
            // 
            this.textBox_User2.Location = new System.Drawing.Point(18, 59);
            this.textBox_User2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_User2.Name = "textBox_User2";
            this.textBox_User2.Size = new System.Drawing.Size(86, 21);
            this.textBox_User2.TabIndex = 11;
            this.textBox_User2.Text = "1001";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(134, 148);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 13;
            this.label7.Text = "MsgContent";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 149);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "MsgType";
            // 
            // button_Disconnect2
            // 
            this.button_Disconnect2.Location = new System.Drawing.Point(272, 56);
            this.button_Disconnect2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_Disconnect2.Name = "button_Disconnect2";
            this.button_Disconnect2.Size = new System.Drawing.Size(64, 24);
            this.button_Disconnect2.TabIndex = 7;
            this.button_Disconnect2.Text = "Close";
            this.button_Disconnect2.UseVisualStyleBackColor = true;
            this.button_Disconnect2.Click += new System.EventHandler(this.button_Disconnect2_Click);
            // 
            // textBox_ReqContent2
            // 
            this.textBox_ReqContent2.Location = new System.Drawing.Point(134, 162);
            this.textBox_ReqContent2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_ReqContent2.Name = "textBox_ReqContent2";
            this.textBox_ReqContent2.Size = new System.Drawing.Size(133, 21);
            this.textBox_ReqContent2.TabIndex = 11;
            // 
            // label_ConnectStatus2
            // 
            this.label_ConnectStatus2.AutoSize = true;
            this.label_ConnectStatus2.Location = new System.Drawing.Point(225, 27);
            this.label_ConnectStatus2.Name = "label_ConnectStatus2";
            this.label_ConnectStatus2.Size = new System.Drawing.Size(41, 12);
            this.label_ConnectStatus2.TabIndex = 6;
            this.label_ConnectStatus2.Text = "label3";
            // 
            // numericUpDown_ReqType2
            // 
            this.numericUpDown_ReqType2.Location = new System.Drawing.Point(21, 163);
            this.numericUpDown_ReqType2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numericUpDown_ReqType2.Maximum = new decimal(new int[] {
            0,
            1,
            0,
            0});
            this.numericUpDown_ReqType2.Name = "numericUpDown_ReqType2";
            this.numericUpDown_ReqType2.Size = new System.Drawing.Size(52, 21);
            this.numericUpDown_ReqType2.TabIndex = 10;
            this.numericUpDown_ReqType2.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // button_Connect2
            // 
            this.button_Connect2.Location = new System.Drawing.Point(202, 56);
            this.button_Connect2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_Connect2.Name = "button_Connect2";
            this.button_Connect2.Size = new System.Drawing.Size(64, 24);
            this.button_Connect2.TabIndex = 5;
            this.button_Connect2.Text = "Connect";
            this.button_Connect2.UseVisualStyleBackColor = true;
            this.button_Connect2.Click += new System.EventHandler(this.button_Connect2_Click);
            // 
            // textBox_Log1
            // 
            this.textBox_Log1.Location = new System.Drawing.Point(23, 337);
            this.textBox_Log1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_Log1.Multiline = true;
            this.textBox_Log1.Name = "textBox_Log1";
            this.textBox_Log1.ReadOnly = true;
            this.textBox_Log1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Log1.Size = new System.Drawing.Size(355, 238);
            this.textBox_Log1.TabIndex = 7;
            this.textBox_Log1.WordWrap = false;
            // 
            // textBox_Log2
            // 
            this.textBox_Log2.Location = new System.Drawing.Point(394, 337);
            this.textBox_Log2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox_Log2.Multiline = true;
            this.textBox_Log2.Name = "textBox_Log2";
            this.textBox_Log2.ReadOnly = true;
            this.textBox_Log2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Log2.Size = new System.Drawing.Size(355, 238);
            this.textBox_Log2.TabIndex = 8;
            this.textBox_Log2.WordWrap = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(23, 8);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 16);
            this.button1.TabIndex = 9;
            this.button1.Text = "clear";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label_SipReg1
            // 
            this.label_SipReg1.AutoSize = true;
            this.label_SipReg1.Location = new System.Drawing.Point(839, 254);
            this.label_SipReg1.Name = "label_SipReg1";
            this.label_SipReg1.Size = new System.Drawing.Size(53, 12);
            this.label_SipReg1.TabIndex = 10;
            this.label_SipReg1.Text = "Register";
            // 
            // label_SipReg2
            // 
            this.label_SipReg2.AutoSize = true;
            this.label_SipReg2.Location = new System.Drawing.Point(839, 276);
            this.label_SipReg2.Name = "label_SipReg2";
            this.label_SipReg2.Size = new System.Drawing.Size(53, 12);
            this.label_SipReg2.TabIndex = 11;
            this.label_SipReg2.Text = "Register";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(792, 320);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(107, 23);
            this.button2.TabIndex = 12;
            this.button2.Text = "Accept SIP Call";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // checkedListBox_AudCapture
            // 
            this.checkedListBox_AudCapture.FormattingEnabled = true;
            this.checkedListBox_AudCapture.HorizontalScrollbar = true;
            this.checkedListBox_AudCapture.Location = new System.Drawing.Point(804, 44);
            this.checkedListBox_AudCapture.Name = "checkedListBox_AudCapture";
            this.checkedListBox_AudCapture.Size = new System.Drawing.Size(129, 180);
            this.checkedListBox_AudCapture.TabIndex = 13;
            this.checkedListBox_AudCapture.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox_AudCapture_ItemCheck);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(802, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(131, 12);
            this.label9.TabIndex = 14;
            this.label9.Text = "Audio Capture Devices";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(937, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(137, 12);
            this.label10.TabIndex = 14;
            this.label10.Text = "Audio Playback Devices";
            // 
            // checkedListBox_AudPlayback
            // 
            this.checkedListBox_AudPlayback.FormattingEnabled = true;
            this.checkedListBox_AudPlayback.HorizontalScrollbar = true;
            this.checkedListBox_AudPlayback.Location = new System.Drawing.Point(939, 44);
            this.checkedListBox_AudPlayback.Name = "checkedListBox_AudPlayback";
            this.checkedListBox_AudPlayback.Size = new System.Drawing.Size(135, 180);
            this.checkedListBox_AudPlayback.TabIndex = 15;
            this.checkedListBox_AudPlayback.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox_AudPlayback_ItemCheck);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(905, 320);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(107, 23);
            this.button3.TabIndex = 16;
            this.button3.Text = "Dismiss SIP Call";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(804, 254);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 12);
            this.label11.TabIndex = 17;
            this.label11.Text = "SIP1";
            // 
            // SIP2
            // 
            this.SIP2.AutoSize = true;
            this.SIP2.Location = new System.Drawing.Point(806, 276);
            this.SIP2.Name = "SIP2";
            this.SIP2.Size = new System.Drawing.Size(29, 12);
            this.SIP2.TabIndex = 18;
            this.SIP2.Text = "SIP2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1144, 586);
            this.Controls.Add(this.SIP2);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.checkedListBox_AudPlayback);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.checkedListBox_AudCapture);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label_SipReg2);
            this.Controls.Add(this.label_SipReg1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox_Log2);
            this.Controls.Add(this.textBox_Log1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReqNum1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReqType1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReqNum2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReqType2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_Server1;
        private System.Windows.Forms.TextBox textBox_Server2;
        private System.Windows.Forms.Button button_Connect1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_Connect2;
        private System.Windows.Forms.Label label_ConnectStatus1;
        private System.Windows.Forms.Label label_ConnectStatus2;
        private System.Windows.Forms.TextBox textBox_Log1;
        private System.Windows.Forms.TextBox textBox_Psw1;
        private System.Windows.Forms.TextBox textBox_User1;
        private System.Windows.Forms.NumericUpDown numericUpDown_ReqType1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_ReqContent1;
        private System.Windows.Forms.Button button_Req1;
        private System.Windows.Forms.NumericUpDown numericUpDown_ReqNum1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_Disconnect1;
        private System.Windows.Forms.Button button_Disconnect2;
        private System.Windows.Forms.TextBox textBox_Psw2;
        private System.Windows.Forms.TextBox textBox_User2;
        private System.Windows.Forms.NumericUpDown numericUpDown_ReqNum2;
        private System.Windows.Forms.Button button_Req2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox_ReqContent2;
        private System.Windows.Forms.NumericUpDown numericUpDown_ReqType2;
        private System.Windows.Forms.TextBox textBox_Log2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label_SipReg1;
        private System.Windows.Forms.Label label_SipReg2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckedListBox checkedListBox_AudCapture;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckedListBox checkedListBox_AudPlayback;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label SIP2;
    }
}

