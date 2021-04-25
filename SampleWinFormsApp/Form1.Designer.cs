
namespace SampleWinFormsApp
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
            this.button_LogOut = new System.Windows.Forms.Button();
            this.textBox_Psw1 = new System.Windows.Forms.TextBox();
            this.textBox_User1 = new System.Windows.Forms.TextBox();
            this.button_LogIn1 = new System.Windows.Forms.Button();
            this.label_ConnectStatus1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label_ConnectStatus2 = new System.Windows.Forms.Label();
            this.button_Connect2 = new System.Windows.Forms.Button();
            this.textBox_Log1 = new System.Windows.Forms.TextBox();
            this.numericUpDown_ReqType1 = new System.Windows.Forms.NumericUpDown();
            this.textBox_ReqContent1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button_Req1 = new System.Windows.Forms.Button();
            this.numericUpDown_ReqNum1 = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReqType1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReqNum1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Server";
            // 
            // textBox_Server1
            // 
            this.textBox_Server1.Location = new System.Drawing.Point(77, 41);
            this.textBox_Server1.Name = "textBox_Server1";
            this.textBox_Server1.Size = new System.Drawing.Size(176, 23);
            this.textBox_Server1.TabIndex = 2;
            this.textBox_Server1.Text = "192.168.2.108";
            // 
            // textBox_Server2
            // 
            this.textBox_Server2.Location = new System.Drawing.Point(81, 35);
            this.textBox_Server2.Name = "textBox_Server2";
            this.textBox_Server2.Size = new System.Drawing.Size(176, 23);
            this.textBox_Server2.TabIndex = 3;
            this.textBox_Server2.Text = "192.168.2.109";
            // 
            // button_Connect1
            // 
            this.button_Connect1.Location = new System.Drawing.Point(77, 70);
            this.button_Connect1.Name = "button_Connect1";
            this.button_Connect1.Size = new System.Drawing.Size(75, 23);
            this.button_Connect1.TabIndex = 4;
            this.button_Connect1.Text = "Connect";
            this.button_Connect1.UseVisualStyleBackColor = true;
            this.button_Connect1.Click += new System.EventHandler(this.button_Connect1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericUpDown_ReqNum1);
            this.groupBox1.Controls.Add(this.button_Req1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBox_ReqContent1);
            this.groupBox1.Controls.Add(this.numericUpDown_ReqType1);
            this.groupBox1.Controls.Add(this.button_LogOut);
            this.groupBox1.Controls.Add(this.textBox_Psw1);
            this.groupBox1.Controls.Add(this.textBox_User1);
            this.groupBox1.Controls.Add(this.button_LogIn1);
            this.groupBox1.Controls.Add(this.label_ConnectStatus1);
            this.groupBox1.Controls.Add(this.textBox_Server1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button_Connect1);
            this.groupBox1.Location = new System.Drawing.Point(27, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(414, 287);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // button_LogOut
            // 
            this.button_LogOut.Location = new System.Drawing.Point(319, 134);
            this.button_LogOut.Name = "button_LogOut";
            this.button_LogOut.Size = new System.Drawing.Size(75, 23);
            this.button_LogOut.TabIndex = 9;
            this.button_LogOut.Text = "Log Out";
            this.button_LogOut.UseVisualStyleBackColor = true;
            this.button_LogOut.Click += new System.EventHandler(this.button_LogOut_Click);
            // 
            // textBox_Psw1
            // 
            this.textBox_Psw1.Location = new System.Drawing.Point(131, 134);
            this.textBox_Psw1.Name = "textBox_Psw1";
            this.textBox_Psw1.Size = new System.Drawing.Size(100, 23);
            this.textBox_Psw1.TabIndex = 8;
            this.textBox_Psw1.Text = "1001";
            // 
            // textBox_User1
            // 
            this.textBox_User1.Location = new System.Drawing.Point(25, 134);
            this.textBox_User1.Name = "textBox_User1";
            this.textBox_User1.Size = new System.Drawing.Size(100, 23);
            this.textBox_User1.TabIndex = 7;
            this.textBox_User1.Text = "1001";
            // 
            // button_LogIn1
            // 
            this.button_LogIn1.Location = new System.Drawing.Point(238, 134);
            this.button_LogIn1.Name = "button_LogIn1";
            this.button_LogIn1.Size = new System.Drawing.Size(75, 23);
            this.button_LogIn1.TabIndex = 6;
            this.button_LogIn1.Text = "Log In";
            this.button_LogIn1.UseVisualStyleBackColor = true;
            this.button_LogIn1.Click += new System.EventHandler(this.button_LogIn1_Click);
            // 
            // label_ConnectStatus1
            // 
            this.label_ConnectStatus1.AutoSize = true;
            this.label_ConnectStatus1.Location = new System.Drawing.Point(260, 44);
            this.label_ConnectStatus1.Name = "label_ConnectStatus1";
            this.label_ConnectStatus1.Size = new System.Drawing.Size(43, 17);
            this.label_ConnectStatus1.TabIndex = 5;
            this.label_ConnectStatus1.Text = "label3";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label_ConnectStatus2);
            this.groupBox2.Controls.Add(this.button_Connect2);
            this.groupBox2.Controls.Add(this.textBox_Server2);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(460, 44);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(414, 284);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // label_ConnectStatus2
            // 
            this.label_ConnectStatus2.AutoSize = true;
            this.label_ConnectStatus2.Location = new System.Drawing.Point(263, 38);
            this.label_ConnectStatus2.Name = "label_ConnectStatus2";
            this.label_ConnectStatus2.Size = new System.Drawing.Size(43, 17);
            this.label_ConnectStatus2.TabIndex = 6;
            this.label_ConnectStatus2.Text = "label3";
            // 
            // button_Connect2
            // 
            this.button_Connect2.Location = new System.Drawing.Point(81, 67);
            this.button_Connect2.Name = "button_Connect2";
            this.button_Connect2.Size = new System.Drawing.Size(75, 23);
            this.button_Connect2.TabIndex = 5;
            this.button_Connect2.Text = "Connect";
            this.button_Connect2.UseVisualStyleBackColor = true;
            this.button_Connect2.Click += new System.EventHandler(this.button_Connect2_Click);
            // 
            // textBox_Log1
            // 
            this.textBox_Log1.Location = new System.Drawing.Point(27, 334);
            this.textBox_Log1.Multiline = true;
            this.textBox_Log1.Name = "textBox_Log1";
            this.textBox_Log1.ReadOnly = true;
            this.textBox_Log1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Log1.Size = new System.Drawing.Size(414, 335);
            this.textBox_Log1.TabIndex = 7;
            this.textBox_Log1.WordWrap = false;
            // 
            // numericUpDown_ReqType1
            // 
            this.numericUpDown_ReqType1.Location = new System.Drawing.Point(27, 234);
            this.numericUpDown_ReqType1.Name = "numericUpDown_ReqType1";
            this.numericUpDown_ReqType1.Size = new System.Drawing.Size(60, 23);
            this.numericUpDown_ReqType1.TabIndex = 10;
            // 
            // textBox_ReqContent1
            // 
            this.textBox_ReqContent1.Location = new System.Drawing.Point(158, 233);
            this.textBox_ReqContent1.Name = "textBox_ReqContent1";
            this.textBox_ReqContent1.Size = new System.Drawing.Size(155, 23);
            this.textBox_ReqContent1.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 214);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "MsgType";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(158, 213);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 17);
            this.label4.TabIndex = 13;
            this.label4.Text = "MsgContent";
            // 
            // button_Req1
            // 
            this.button_Req1.Location = new System.Drawing.Point(319, 234);
            this.button_Req1.Name = "button_Req1";
            this.button_Req1.Size = new System.Drawing.Size(75, 23);
            this.button_Req1.TabIndex = 14;
            this.button_Req1.Text = "Send Request";
            this.button_Req1.UseVisualStyleBackColor = true;
            this.button_Req1.Click += new System.EventHandler(this.button_Req1_Click);
            // 
            // numericUpDown_ReqNum1
            // 
            this.numericUpDown_ReqNum1.Location = new System.Drawing.Point(92, 233);
            this.numericUpDown_ReqNum1.Name = "numericUpDown_ReqNum1";
            this.numericUpDown_ReqNum1.Size = new System.Drawing.Size(60, 23);
            this.numericUpDown_ReqNum1.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(92, 213);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 17);
            this.label5.TabIndex = 13;
            this.label5.Text = "MsgNum";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(918, 681);
            this.Controls.Add(this.textBox_Log1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReqType1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ReqNum1)).EndInit();
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
        private System.Windows.Forms.Button button_LogIn1;
        private System.Windows.Forms.TextBox textBox_Psw1;
        private System.Windows.Forms.TextBox textBox_User1;
        private System.Windows.Forms.Button button_LogOut;
        private System.Windows.Forms.NumericUpDown numericUpDown_ReqType1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_ReqContent1;
        private System.Windows.Forms.Button button_Req1;
        private System.Windows.Forms.NumericUpDown numericUpDown_ReqNum1;
        private System.Windows.Forms.Label label5;
    }
}

