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
    /// ConnectionPropertyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectionPropertyWindow : Window
    {
        private Connection selectConnection;
        private int ConnConnectionsCount;
        public ConnectionPropertyWindow(Connection selectConnection,int ConnConnectionsCount)
        {
            InitializeComponent();
            this.selectConnection = selectConnection;
            this.ConnConnectionsCount=ConnConnectionsCount;
            gridProductDetails.DataContext = selectConnection;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
            
            
           
        }

    

        

       
      

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            selectConnection.RDType = "L";
            selectConnection.MutilLineCount = "1";
            selectConnection.RDValue = "0";
            selectConnection.RDDirection = "1";
            selectConnection.LeftPoints = "";
            selectConnection.RightPoints= "";
        }



    }
}
