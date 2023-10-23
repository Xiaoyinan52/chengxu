using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WireTestProgram
{
    /// <summary>
    /// FModelWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FModelWindow : MyMacClass
    {
        private string fmodel;
        private string fmodelLength;
        public FModelWindow(string fmodel, string fmodelLength)
        {
            InitializeComponent();
            this.fmodel = fmodel;
            this.fmodelLength = fmodelLength;
        }

        private void fmodelTxb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int a = 0;
                if (int.TryParse(fmodelLength, out a) == false) //判断是否可以转换为整型
                {
                    a = 0;
                }
                if (!string.IsNullOrEmpty(fmodelTxb.Text) && fmodelTxb.Text==fmodel && fmodelTxb.Text.Length==a)
                {
                    Thread.Sleep(750);
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("测试程序不符、扫描长度错误！！！");
                }
                
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fmodelTxb.Focus();
        }

       



    }
}
