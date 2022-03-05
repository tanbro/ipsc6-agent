
namespace SipClientWinFormsDemo
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
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_SIpUser = new System.Windows.Forms.TextBox();
            this.textBox_SipPassword = new System.Windows.Forms.TextBox();
            this.textBox_SipRgistrar = new System.Windows.Forms.TextBox();
            this.button_Register = new System.Windows.Forms.Button();
            this.textBox_Log = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "SIP 注册用户";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(160, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "SIP Registrar 地址";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(315, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 24);
            this.label3.TabIndex = 0;
            this.label3.Text = "SIP 注册口令";
            // 
            // textBox_SIpUser
            // 
            this.textBox_SIpUser.Location = new System.Drawing.Point(131, 9);
            this.textBox_SIpUser.Name = "textBox_SIpUser";
            this.textBox_SIpUser.Size = new System.Drawing.Size(150, 30);
            this.textBox_SIpUser.TabIndex = 2;
            // 
            // textBox_SipPassword
            // 
            this.textBox_SipPassword.Location = new System.Drawing.Point(434, 9);
            this.textBox_SipPassword.Name = "textBox_SipPassword";
            this.textBox_SipPassword.Size = new System.Drawing.Size(150, 30);
            this.textBox_SipPassword.TabIndex = 2;
            // 
            // textBox_SipRgistrar
            // 
            this.textBox_SipRgistrar.Location = new System.Drawing.Point(178, 51);
            this.textBox_SipRgistrar.Name = "textBox_SipRgistrar";
            this.textBox_SipRgistrar.Size = new System.Drawing.Size(406, 30);
            this.textBox_SipRgistrar.TabIndex = 2;
            // 
            // button_Register
            // 
            this.button_Register.Location = new System.Drawing.Point(606, 12);
            this.button_Register.Name = "button_Register";
            this.button_Register.Size = new System.Drawing.Size(143, 69);
            this.button_Register.TabIndex = 3;
            this.button_Register.Text = "增加这个注册账户";
            this.button_Register.UseVisualStyleBackColor = true;
            this.button_Register.Click += new System.EventHandler(this.button_Register_Click);
            // 
            // textBox_Log
            // 
            this.textBox_Log.Location = new System.Drawing.Point(12, 145);
            this.textBox_Log.Multiline = true;
            this.textBox_Log.Name = "textBox_Log";
            this.textBox_Log.Size = new System.Drawing.Size(776, 293);
            this.textBox_Log.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBox_Log);
            this.Controls.Add(this.button_Register);
            this.Controls.Add(this.textBox_SipPassword);
            this.Controls.Add(this.textBox_SipRgistrar);
            this.Controls.Add(this.textBox_SIpUser);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_SIpUser;
        private System.Windows.Forms.TextBox textBox_SipPassword;
        private System.Windows.Forms.TextBox textBox_SipRgistrar;
        private System.Windows.Forms.Button button_Register;
        private System.Windows.Forms.TextBox textBox_Log;
    }
}

