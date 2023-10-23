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
    /// FNumberWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FNumberWindow : MyMacClass
    {
        private string fbarcodeLength;
        private string fbarcodeFront;
        public FNumberWindow(string fbarcodeLength, string fbarcodeFront)
        {
            InitializeComponent();
            this.fbarcodeLength = fbarcodeLength;
            this.fbarcodeFront = fbarcodeFront;
        }

        private void fnumberTxb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int a = 0;
                if (int.TryParse(fbarcodeLength, out a) == false) //判断是否可以转换为整型
                {
                    a = 0;
                }
                if (!string.IsNullOrEmpty(fnumberTxb.Text)  && fnumberTxb.Text.Length==a   )
                {
                    if(fbarcodeFront!="")
                    {
                        if (fnumberTxb.Text.IndexOf(fbarcodeFront)==0)
                        {
                            Thread.Sleep(750);
                            DialogResult = true;
                        }
                        else
                        {
                            MessageBox.Show("扫描前缀错误！！！");
                        }
                    }
                    else
                    {
                        DialogResult = true;
                    }

                }
                else
                {
                    MessageBox.Show("扫描长度错误！！！");
                }

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fnumberTxb.Focus();

        }



    }
}
