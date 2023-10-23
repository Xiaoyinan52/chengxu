using System;
using System.Collections.Generic;
using System.IO;
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

namespace WireTestProgram
{
    /// <summary>
    /// FModelWindow2.xaml 的交互逻辑
    /// </summary>
    public partial class FModelWindow2 :MyMacClass
    {
        private TestWindow1 t1 = null;
        private DesignerCanvas d1 = null;
        public FModelWindow2(TestWindow1 t1,DesignerCanvas d1)
        {
            InitializeComponent();
            this.t1 = t1;
            this.d1 = d1;

        }

      

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fmodelTxb2.Focus();
        }

        private void fmodelTxb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {



                string fmodel = fmodelTxb2.Text;
                string fileName = string.Format(@"{0}\testFile\{1}.xml", System.Windows.Forms.Application.StartupPath, fmodel);
                if (File.Exists(fileName))
                {
                    d1.fmodelInsert = fmodel;
                    d1.OpenFile2(fileName);
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("警告！！！"+fmodel + "型号对应测试文件不存在！！！");
                    
                }


            }
        }




    }
}
