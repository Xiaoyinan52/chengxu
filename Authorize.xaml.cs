using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using WireTestProgram.Register;

namespace WireTestProgram
{
    /// <summary>
    /// Authorize.xaml 的交互逻辑
    /// </summary>
    public partial class Authorize : MyMacClass
    {     
        private string md5;
        public Authorize(string md5)//userNameSecutity的值是login界面传值过来的
        {
            InitializeComponent();
            this.md5 = md5;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //System.Environment.Exit(0);
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            Txt_Xuliehao.Text = md5;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            ComputerInfo cc = new ComputerInfo();//初始化实例，用于获取电脑硬件信息
            EncryptionHelper ee = new EncryptionHelper();//初始化实例，生成
            string encryptComputer = ee.Encrypt(cc.GetComputerInfo(), "20070901");//获取电脑硬件信息，并与后面的字符串一起加密，获取加密字符串
            string md5 = ee.GetMD5String(encryptComputer);//获取加密字符串
            if (Txt_Jihuoma.Text == md5)
            {               
                cc.WriteFile(md5, string.Format(@"{0}\license.txt", System.Windows.Forms.Application.StartupPath));
                MessageBox.Show("已注册完成！");

            }
            else
            {
                MessageBox.Show("输入的注册码不正确，请重新输入！");
            }

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)//退出
        {
            this.Close();
        }
    }
}
