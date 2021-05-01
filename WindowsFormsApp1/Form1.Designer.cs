
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
            this.textBox_ServerAddressList = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_Init = new System.Windows.Forms.Button();
            this.textBox_workerNum = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_password = new System.Windows.Forms.TextBox();
            this.button_open = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_ServerAddressList
            // 
            this.textBox_ServerAddressList.Location = new System.Drawing.Point(142, 11);
            this.textBox_ServerAddressList.Name = "textBox_ServerAddressList";
            this.textBox_ServerAddressList.Size = new System.Drawing.Size(635, 21);
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
            // button_Init
            // 
            this.button_Init.Location = new System.Drawing.Point(323, 38);
            this.button_Init.Name = "button_Init";
            this.button_Init.Size = new System.Drawing.Size(75, 23);
            this.button_Init.TabIndex = 2;
            this.button_Init.Text = "初始化";
            this.button_Init.UseVisualStyleBackColor = true;
            this.button_Init.Click += new System.EventHandler(this.button_Init_Click);
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
            this.button_open.Location = new System.Drawing.Point(415, 38);
            this.button_open.Name = "button_open";
            this.button_open.Size = new System.Drawing.Size(75, 23);
            this.button_open.TabIndex = 6;
            this.button_open.Text = "打开连接";
            this.button_open.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button_open);
            this.Controls.Add(this.textBox_password);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_workerNum);
            this.Controls.Add(this.button_Init);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_ServerAddressList);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_ServerAddressList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_Init;
        private System.Windows.Forms.TextBox textBox_workerNum;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_password;
        private System.Windows.Forms.Button button_open;
    }
}

