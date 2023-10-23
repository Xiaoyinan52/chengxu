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
    /// SetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetWindow :  MyMacClass
    {
        public SetWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DBaccessHelp db = new DBaccessHelp();
           
            DataTable dt = db.ExecuteQuery("select scanModel,memoryModel,shortBreak,comport,fmodelLength,fbarcodeLength,fbarcodeFront,switchcomport,fontsize  from  setTable where setFlag='set'");
            scanModel.Text = dt.Rows[0][0].ToString();
            memoryModel.Text = dt.Rows[0][1].ToString();
            duanLuZhongduan.Text = dt.Rows[0][2].ToString();
            comport.Text = dt.Rows[0][3].ToString();
            fmodelLength.Text= dt.Rows[0][4].ToString();
            fbarcodeLength.Text = dt.Rows[0][5].ToString();
            fbarcodeFront.Text= dt.Rows[0][6].ToString();
            switchcomport.Text = dt.Rows[0][7].ToString();
            ffontsize.Text = dt.Rows[0][8].ToString();

            db.closeOleDbConnection();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DBaccessHelp db = new DBaccessHelp();
            int s = db.insertEx("update setTable set scanModel='" + scanModel.Text + "',memoryModel='" + memoryModel.Text + "',shortBreak='" + duanLuZhongduan.Text + "',comport='" + comport.Text + "',fmodelLength='"+fmodelLength.Text+"',fbarcodeLength='"+fbarcodeLength.Text+ "',fbarcodeFront='" + fbarcodeFront.Text+ "',switchcomport='" + switchcomport.Text + "',fontsize='"+ffontsize.Text+"'        where setFlag='set' ");
            db.closeOleDbConnection();
            if (s > 0)
            {
                MessageBox.Show("成功保存到数据库!");
            }
            db.closeOleDbConnection();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void scanModel_SelectionChanged()
        {

        }
    }
}
