using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using hesong.plum.client.Utils;

namespace hesong.plum.client
{
    public partial class frmSoftTel : Form
    {
  

        public frmSoftTel()
        {
            InitializeComponent();
            ucCallOut.OnClick += UcCallOutOnClick;

            // 直接记日志方法调用示例
            Logger.Log("info", $"[frmSoftTel] Initialize Completed.");
        }

        /// <summary>
        /// “外呼”输入卡的小按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UcCallOutOnClick(object sender, EventArgs e)
        {

            using (var frm = new frmDial(ucCallOut))
            {
                var res = frm.ShowDialog(this);
                if (res == DialogResult.OK)
                {

                }
            }
        }

        private void frmSoftTel_Load(object sender, EventArgs e)
        {
            // 设置窗体出现的初始位置
            this.Top = 200;
            this.Left = 20;

            // 初始化内存数据库（整个程序开始时初始化一次就行）
            // 如果不希望SQLiteMem记日志， 传递一个null也行
            SQLiteMem.InstanceSqLiteMem = new SQLiteMem(Logger.LogSaver);

            // 示例调用内存数据库
            var sqlite = SQLiteMem.InstanceSqLiteMem;
            // 执行建表SQL演示（例如建立一张叫做user的表，有id、name、age三个字段，其中id是自增长主键字段）
            sqlite.ExeSql("CREATE TABLE user(id INTEGER PRIMARY KEY, name VARCHAR(50), age INT);");
            // 执行纯SQL插入操作演示
            sqlite.ExeSql("INSERT INTO user(name, age)values('张三', 12);");
            // 执行附加参数SQL插入数据
            var args = new Hashtable { { "name", "李四" }, { "age", 34 } };
            sqlite.ExeSql("INSERT INTO user(name, age)values($name, $age);", args);
            // 带参数更新
            args["name"] = "李四改名";
            sqlite.ExeSql("UPDATE user SET name=$name WHERE id=2;", args);
            // 查询
            var ds = sqlite.Query("select * from user;");
            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var dr = ds.Tables[0].Rows[i];
                MessageBox.Show($"id={dr["id"]}, name ={ dr["name"]}, age ={dr["age"]}");
            }

            ResetControls();
        }

        private void ResetControls()
        {
            ucQueueAndLost.QueueCount = 0;
            ucQueueAndLost.LostCount = 0;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void butLogon_Click(object sender, EventArgs e)
        {
            var frm = new frmLogon();
            frm.ShowDialog(this);
            frm.Dispose();
        }
    }
}
