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
    /// UserRegisterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UserRegisterWindow :MyMacClass
    {
        public UserRegisterWindow()
        {
            InitializeComponent();
        }

        private void btnlogin_Click(object sender, RoutedEventArgs e)
        {
            string str = string.Empty;
            if (designCkb.IsChecked == true)
            {
                str = "设计工程师";

            }
            else
            {
                str = "测试工程师";
            }
            if (UserTextBox.Text != string.Empty && PwdTextBox.Text != string.Empty && str!="")
            {
                DBaccessHelp ex = new DBaccessHelp();
                int s0 = ex.insertEx("delete from  userList  where [userName]='" + UserTextBox.Text + "'  ");
                ex.closeOleDbConnection();


                DBaccessHelp ex2 = new DBaccessHelp();
                int s = ex2.insertEx("insert into userList ([userName],[password],[authority]) values ('" + UserTextBox.Text + "','" + PwdTextBox.Text + "','" + str + "')");
                ex2.closeOleDbConnection();

                DBaccessHelp ex3 = new DBaccessHelp();
                DataTable dt = ex3.ExecuteQuery("select userName,password,authority from userList order by userName");
                ex3.closeOleDbConnection();
              
                gridProducts.ItemsSource = dt.DefaultView;

            }
        }

        private void btnexit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DBaccessHelp ex2 = new DBaccessHelp();
            DataTable dt = ex2.ExecuteQuery("select userName,password,authority from userList order by userName");
            gridProducts.ItemsSource = dt.DefaultView;
        }

        private void gridProducts_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void MenuItem2_Click(object sender, RoutedEventArgs e)
        {
            DBaccessHelp ex = new DBaccessHelp();
            int s0 = ex.insertEx("delete from  userList  where [userName]='" + userName_nouse.Text + "'  ");
            ex.closeOleDbConnection();


           
            DBaccessHelp ex3 = new DBaccessHelp();
            DataTable dt = ex3.ExecuteQuery("select userName,password,authority from userList order by userName");
            ex3.closeOleDbConnection();
           
            gridProducts.ItemsSource = dt.DefaultView;
        }
    }
}
