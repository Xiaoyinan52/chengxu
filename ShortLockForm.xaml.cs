using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WireTestProgram.HelperClass;

namespace WireTestProgram
{
    /// <summary>
    /// ShortLockForm.xaml 的交互逻辑
    /// </summary>
    public partial class ShortLockForm : MyMacClass
    {
      

        public ShortLockForm()
        {
            InitializeComponent();
          
        }

        private void MyMacClass_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (getUserName())
            {

            }
            else
            {
                e.Cancel = true;
            }
            
        }


        //判断用户名
        private bool getUserName()
        {
            bool a = false;
            DBaccessHelp ex = new DBaccessHelp();
            DataTable dt = ex.ExecuteQuery("select userName,password,authority from userList where userName='" + UserTextBox.Text + "' and authority='设计工程师' order by userName");
            if (dt.Rows.Count > 0)
            {
                if (UserTextBox.Text == dt.Rows[0][0].ToString() && PwdTextBox.Password == dt.Rows[0][1].ToString())
                {
                    a = true;
                }
            }
            else
            {
                if(UserTextBox.Text=="admin" && PwdTextBox.Password == "123okOK!")
                {
                    a = true;
                }
            }
            return a;
        }

        private void UserTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PwdTextBox.Password = "";
                PwdTextBox.Focus();
            }
        }
    }
}
