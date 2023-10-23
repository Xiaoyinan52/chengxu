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
    /// FmodelLcok.xaml 的交互逻辑
    /// </summary>
    public partial class FmodelLcok : MyMacClass
    {
        public FmodelLcok()
         {
            InitializeComponent();
        }

        private void gridMolds_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex()+1;

                
        }
        private DataTable suoOfTazi = null;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //测试台锁的信息
            DBaccessHelp db3 = new DBaccessHelp();
            suoOfTazi = db3.ExecuteQuery("select MKName,SuoName, SuoType , StartPoint, EndPoint,FStatus,DesignUser ,designDate   from  SuoSource order by SuoType ,MKName,SuoName ");
            db3.closeOleDbConnection();
            gridMolds.ItemsSource = suoOfTazi.DefaultView;
            //显示有哪些图号
            DBaccessHelp2 db = new DBaccessHelp2();
            DataTable dt = db.ExecuteQuery("select fmodel   from  SuoSource group by Fmodel ");
            db.closeOleDbConnection();
            this.lstProducts.ItemsSource = null;
            this.lstProducts.ItemsSource= dt.DefaultView;
        }
        //单个型号查询
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DBaccessHelp2 db3 = new DBaccessHelp2();
            DataTable dt3 = db3.ExecuteQuery("select Fmodel,MKName,SuoName, SuoType , StartPoint, EndPoint,FStatus  from  SuoSource  where   fmodel  ='"+fmodel_txt.Text+"' order by SuoType ,MKName,SuoName ");
            db3.closeOleDbConnection();
            gridMolds2.ItemsSource = dt3.DefaultView;
        }
        //图号锁全部删除
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            DBaccessHelp2 db = new DBaccessHelp2();
            int s = db.insertEx("delete  from   SuoSource  where   fmodel  ='" + fmodel_txt.Text + "' ");
            db.closeOleDbConnection();

            DBaccessHelp2 db3 = new DBaccessHelp2();
            DataTable dt3 = db3.ExecuteQuery("select Fmodel,MKName,SuoName, SuoType , StartPoint, EndPoint,FStatus  from  SuoSource  where   fmodel  ='" + fmodel_txt.Text + "' order by SuoType ,MKName,SuoName ");
            db3.closeOleDbConnection();
            gridMolds2.ItemsSource = dt3.DefaultView;

            DBaccessHelp2 db4 = new DBaccessHelp2();
            DataTable dt4 = db4.ExecuteQuery("select fmodel   from  SuoSource group by Fmodel ");
            db4.closeOleDbConnection();
            this.lstProducts.ItemsSource = null;
            this.lstProducts.ItemsSource = dt4.DefaultView;

        }
        //图号单个删除
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DBaccessHelp2 db = new DBaccessHelp2();
            int s = db.insertEx("delete from  SuoSource where  StartPoint= '" + fmdoelStart.Text + "' and  EndPoint='" + fmdoelEnd.Text + "' and fmodel='"+fmodel_txt.Text+"'  ");
            db.closeOleDbConnection();

            DBaccessHelp2 db3 = new DBaccessHelp2();
            DataTable dt = db3.ExecuteQuery("select  Fmodel,MKName,SuoName, SuoType , StartPoint, EndPoint,FStatus   from  SuoSource where fmodel='" + fmodel_txt.Text + "' order by SuoType ,MKName,SuoName ");
            db3.closeOleDbConnection();
            gridMolds2.ItemsSource = dt.DefaultView;
        }
        //图号单个更新保存
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (fmodel_txt.Text != "")
            {
                DBaccessHelp db3 = new DBaccessHelp();
                DataTable dt = db3.ExecuteQuery("select MKName,SuoName, SuoType , StartPoint, EndPoint,FStatus   from  SuoSource  where  StartPoint= '" + taiziStart.Text + "' and  EndPoint='" + taiziEnd.Text + "'");
                db3.closeOleDbConnection();
                if (dt.Rows.Count > 0)
                {
                    DBaccessHelp2 db0 = new DBaccessHelp2();
                    int s0 = db0.insertEx("delete from  SuoSource where  StartPoint= '" + taiziStart.Text + "' and  EndPoint='" + taiziEnd.Text + "' and  fmodel='" + fmodel_txt.Text + "' ");
                    db0.closeOleDbConnection();
                    DBaccessHelp2 db = new DBaccessHelp2();
                    int s = db.insertEx("insert into SuoSource (Fmodel,MKName,SuoName, SuoType , StartPoint, EndPoint,FStatus)  values  ( '" + fmodel_txt.Text + "','" + Convert.ToString(dt.Rows[0][0]) + "','" + Convert.ToString(dt.Rows[0][1]) + "','" + Convert.ToString(dt.Rows[0][2]) + "','" + Convert.ToString(dt.Rows[0][3]) + "','" + Convert.ToString(dt.Rows[0][4]) + "','" + Convert.ToString(dt.Rows[0][5]) + "' )  ");
                    db.closeOleDbConnection();
                }
                else
                {
                    MessageBox.Show("测试台不存在当前锁信息！");
                }

                DBaccessHelp2 db2 = new DBaccessHelp2();
                DataTable dt2 = db2.ExecuteQuery("select  Fmodel,MKName,SuoName, SuoType , StartPoint, EndPoint,FStatus   from  SuoSource where   fmodel  ='" + fmodel_txt.Text + "' order by SuoType ,MKName,SuoName  ");
                db2.closeOleDbConnection();
                gridMolds2.ItemsSource = dt2.DefaultView;

                DBaccessHelp2 db4 = new DBaccessHelp2();
                DataTable dt4 = db4.ExecuteQuery("select fmodel   from  SuoSource group by Fmodel ");
                db4.closeOleDbConnection();
                this.lstProducts.ItemsSource = null;
                this.lstProducts.ItemsSource = dt4.DefaultView;
            }


        }
        //图号批量更新
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            DBaccessHelp2 db = new DBaccessHelp2();
            int s = db.insertEx("delete  from   SuoSource  where   fmodel  ='" + fmodel_txt.Text + "' ");
            db.closeOleDbConnection();

            if (suoOfTazi.Rows.Count > 0 &&  fmodel_txt.Text!="")
            {
                
                for (int i = 0; i < suoOfTazi.Rows.Count; i++)
                {
                    DBaccessHelp2 db5 = new DBaccessHelp2();
                    int s5 = db5.insertEx("insert into SuoSource (Fmodel,MKName,SuoName, SuoType , StartPoint, EndPoint,FStatus)  values  ( '" + fmodel_txt.Text + "','" + Convert.ToString(suoOfTazi.Rows[i][0]) + "','" + Convert.ToString(suoOfTazi.Rows[i][1]) + "','" + Convert.ToString(suoOfTazi.Rows[i][2]) + "','" + Convert.ToString(suoOfTazi.Rows[i][3]) + "','" + Convert.ToString(suoOfTazi.Rows[i][4]) + "','" + Convert.ToString(suoOfTazi.Rows[i][5]) + "' )  ");
                    db5.closeOleDbConnection();
                }

                
            }
            else
            {
                MessageBox.Show("请确认测试台锁和要更新的型号！！！");
            }
            DBaccessHelp2 db3 = new DBaccessHelp2();
            DataTable dt = db3.ExecuteQuery("select  Fmodel,MKName,SuoName, SuoType , StartPoint, EndPoint,FStatus   from  SuoSource where fmodel='" + fmodel_txt.Text + "' order by SuoType ,MKName,SuoName ");
            db3.closeOleDbConnection();
            gridMolds2.ItemsSource = null;
            gridMolds2.ItemsSource = dt.DefaultView;

            DBaccessHelp2 db4 = new DBaccessHelp2();
            DataTable dt4 = db4.ExecuteQuery("select fmodel   from  SuoSource group by Fmodel ");
            db4.closeOleDbConnection();
            this.lstProducts.ItemsSource = null;
            this.lstProducts.ItemsSource = dt4.DefaultView;

        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
          if( string.IsNullOrEmpty (faddmodel_txt.Text))
            {
                MessageBox.Show("新增型号名称不能为空");
                return;
            }
            if (string.IsNullOrEmpty(fmodel_txt.Text))
            {
                MessageBox.Show("复制源型号名称不能为空");
                return;
            }
            DBaccessHelp2 db = new DBaccessHelp2();
            int s = db.insertEx(" insert into SuoSource (Fmodel,MKName,SuoName, SuoType , StartPoint, EndPoint,FStatus)   select '" + faddmodel_txt.Text + "',MKName,SuoName, SuoType , StartPoint, EndPoint,FStatus  from  SuoSource  where   fmodel  ='" + fmodel_txt.Text + "'  ");
            db.closeOleDbConnection();
            if (s > 0)
            {
                //显示有哪些图号
                DBaccessHelp2 db2 = new DBaccessHelp2();
                DataTable dt2 = db2.ExecuteQuery("select fmodel   from  SuoSource group by Fmodel ");
                db2.closeOleDbConnection();
                this.lstProducts.ItemsSource = null;
                this.lstProducts.ItemsSource = dt2.DefaultView;
                MessageBox.Show("复制成功");
            }
            else
            {
                MessageBox.Show("复制失败");

            }
            
            
           
           



        }





    }
}
