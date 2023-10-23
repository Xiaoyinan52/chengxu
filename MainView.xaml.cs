using System;
using System.Collections.Generic;
using System.Configuration;
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
using WireTestProgram.selfCheck;

namespace WireTestProgram
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView :MyMacClass
    {
        private string userNameSecutity;
        public MainView(string userNameSecutity)//userNameSecutity的值是login界面传值过来的
        {
            InitializeComponent();
            this.userNameSecutity =userNameSecutity;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            usertxt.Text = userNameSecutity.Split(',')[0];
        }

        private void userManage_Click(object sender, RoutedEventArgs e)//用户管理按钮
        {
            if (userNameSecutity.Split(',')[1] == "设计工程师" || userNameSecutity.Split(',')[0] == "admin")
            {
                UserRegisterWindow user = new UserRegisterWindow();
                user.ShowDialog();
            }
        }

        

        private void modelBTN_Click(object sender, RoutedEventArgs e)//设计模块
        {
            if (userNameSecutity.Split(',')[1] == "设计工程师" || userNameSecutity.Split(',')[0] == "admin" )
            {
                Window1 w1 = new Window1(userNameSecutity, Convert.ToInt32(bankaqtyTxt.Text));
                w1.Show();
            }
            else
            {
                MessageBox.Show("在相应权限下请先进行自检！！！");
            }
           
        }

        private void testBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(bankaqtyTxt.Text) > 0)
            {
                TestWindow1 t1 = new TestWindow1(userNameSecutity, Convert.ToInt32(bankaqtyTxt.Text));
                t1.ShowDialog();
            }
            else
            {
                MessageBox.Show("请先进行自检！！！");
            }
            //TestWindow1 t1 = new TestWindow1(userNameSecutity, Convert.ToInt32(bankaqtyTxt.Text));
            //t1.ShowDialog();

        }

        private void selfCheckBTN_Click(object sender, RoutedEventArgs e)
        {
            SelfCheck self = new SelfCheck();

            if (self.ShowDialog() == true)
            {
                string bankaQty = self.HGQtyTxb.Text;
                bankaqtyTxt.Text = bankaQty;
            }
            self.Close();
        }

        private void sysSetBTN_Click(object sender, RoutedEventArgs e)
        {
           
            if (userNameSecutity.Split(',')[1] == "设计工程师" || userNameSecutity.Split(',')[0] == "admin")
            {
                SetWindow set = new SetWindow();
                set.ShowDialog();
            }
        }

        private void exitBTN_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
        }




    }
}
