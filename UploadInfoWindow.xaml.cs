using System;
using System.Collections.Generic;
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
    /// UploadInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UploadInfoWindow :MyMacClass
    {
        private string str;
        public UploadInfoWindow(string str)
        {
            InitializeComponent();
            this.str = str;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(str.Split(',')[2]))
            {
                SaveData sd = new SaveData();
               int s= sd.insertEx("insert into testRecord (BarCode,Fmodel,Remark,UserName,Infomation,Infomation2,CheckTime) values ('" + str.Split(',')[2] + "','" + str.Split(',')[3] + "','0','" + str.Split(',')[0] + "','"+ infoTypeCBX.Text+ "','" + infoDetail.Text+" "+responseCBX.Text + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')");
                sd.closeConn();
                sd.Dispose();
                this.Close();
            }
          
        }
    }
}
