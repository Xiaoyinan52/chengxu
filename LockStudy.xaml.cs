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

namespace WireTestProgram.gasLock
{
    /// <summary>
    /// LockStudy.xaml 的交互逻辑
    /// </summary>
    public partial class LockStudy :MyMacClass
    {
        private List<DesignerItem> designerItemSum;
        private List<DesignerItem> designerItemAllPoints;
        private string userName;
        private int bankaQty = 0;
        public LockStudy(List<DesignerItem> designerItemSum, string userName, int bankaQty , List<DesignerItem> designerItemAllPoints)
        {
            InitializeComponent();
            this.designerItemSum = designerItemSum;
            this.designerItemAllPoints = designerItemAllPoints;
            this.userName = userName;
            this.bankaQty = bankaQty;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
           
                GasLockPinDian pingD = new GasLockPinDian(this,designerItemSum, userName,bankaQty, designerItemAllPoints);
                pingD.ShowDialog();
                /*
                 if (pingD.ShowDialog()==true)
                 {
                     if (!string.IsNullOrEmpty(pingD.startzuobiaoName.Text) && !string.IsNullOrEmpty(pingD.endzuobiaoName.Text) && !string.IsNullOrEmpty(pingD.mokuaiName.Text) && !string.IsNullOrEmpty(pingD.suoName.Text))
                     {
                        DBaccessHelp db0 = new DBaccessHelp();
                        DataTable dt0 = db0.ExecuteQuery("select *   from  SuoSource where  SuoName='"+ pingD.suoName.Text + "' or StartPoint='"+ pingD.startzuobiaoName.Text + "' or  EndPoint='"+ pingD.startzuobiaoName.Text + "' or StartPoint='" + pingD.endzuobiaoName.Text + "' or  EndPoint='" + pingD.endzuobiaoName.Text + "' ");
                        db0.closeOleDbConnection();
                        if (dt0.Rows.Count > 0)
                        {
                            MessageBox.Show("警告，锁已经存在，请检查锁名称、起始坐标、终止坐标！！！");
                            return;
                        }

                        DBaccessHelp db = new DBaccessHelp();
                         int s = db.insertEx("delete from  SuoSource where   StartPoint= '" + pingD.startzuobiaoName.Text + "' and  EndPoint='" + pingD.endzuobiaoName.Text + "'  ");
                         db.closeOleDbConnection();
                         
                         DBaccessHelp db2 = new DBaccessHelp();
                         int s2 = db2.insertEx("insert into SuoSource (MKName,SuoName, SuoType , StartPoint, EndPoint,DesignUser )    values  ( '" + pingD.mokuaiName.Text + "','" + pingD.suoName.Text + "','" + pingD.suoType + "' ,'" + pingD.startzuobiaoName.Text + "','" + pingD.endzuobiaoName.Text + "' ,'"+userName+"') ");
                         db2.closeOleDbConnection();

                         DBaccessHelp db3 = new DBaccessHelp();
                         DataTable dt = db3.ExecuteQuery("select MKName,SuoName, SuoType , StartPoint, EndPoint,FStatus,DesignUser ,designDate   from  SuoSource order by SuoType ,MKName,SuoName ");
                         db3.closeOleDbConnection();
                         gridMolds.ItemsSource = dt.DefaultView;
                     }
                     
                 }
                pingD.Close();
                */
            
          
        }


//提取锁名称 
        public DataTable getSuoList()
        {
            DBaccessHelp db = new DBaccessHelp();
            DataTable dt = db.ExecuteQuery("select MKName,SuoName, SuoType , StartPoint, EndPoint,FStatus,DesignUser ,designDate   from  SuoSource order by SuoType ,MKName,SuoName ");
            db.closeOleDbConnection();
            return dt;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          DataTable dt=  getSuoList();
          gridMolds.ItemsSource = dt.DefaultView;
        }

        private void gridMolds_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
        
//
       

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DBaccessHelp db = new DBaccessHelp();
            int s = db.insertEx("delete from  SuoSource where  StartPoint= '" + startPoint_txt.Text + "' and  EndPoint='" + endPoint_txt.Text + "'  ");
            db.closeOleDbConnection();

            DBaccessHelp db3 = new DBaccessHelp();
            DataTable dt = db3.ExecuteQuery("select MKName,SuoName, SuoType , StartPoint, EndPoint,FStatus,DesignUser ,designDate   from  SuoSource order by SuoType ,MKName,SuoName ");
            db3.closeOleDbConnection();
            gridMolds.ItemsSource = dt.DefaultView;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DBaccessHelp db = new DBaccessHelp();
            int s = db.insertEx("update   SuoSource set FStatus='禁用'  where  StartPoint= '" + startPoint_txt.Text + "' and  EndPoint='" + endPoint_txt.Text + "' and  FStatus='启用' ");
            db.closeOleDbConnection();

            DBaccessHelp db3 = new DBaccessHelp();
            DataTable dt = db3.ExecuteQuery("select MKName,SuoName, SuoType , StartPoint, EndPoint,FStatus,DesignUser ,designDate   from  SuoSource order by SuoType ,MKName,SuoName ");
            db3.closeOleDbConnection();
            gridMolds.ItemsSource = dt.DefaultView;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            DBaccessHelp db = new DBaccessHelp();
            int s = db.insertEx("update   SuoSource set FStatus='启用'  where  StartPoint= '" + startPoint_txt.Text + "' and  EndPoint='" + endPoint_txt.Text + "' and FStatus='禁用' ");
            db.closeOleDbConnection();

            DBaccessHelp db3 = new DBaccessHelp();
            DataTable dt = db3.ExecuteQuery("select MKName,SuoName, SuoType , StartPoint, EndPoint,FStatus,DesignUser ,designDate   from  SuoSource order by SuoType ,MKName,SuoName ");
            db3.closeOleDbConnection();
            gridMolds.ItemsSource = dt.DefaultView;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            FmodelLcok fmodelLock = new FmodelLcok();
            fmodelLock.ShowDialog();
        }
        //锁分析 查询锁有没有直线
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {

        }
    }
}
