using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// TongjiForm.xaml 的交互逻辑
    /// </summary>
    public partial class TongjiForm : Window
    {
        private DesignerCanvas dc;
        public TongjiForm(DesignerCanvas dc)
        {
            InitializeComponent();
            this.dc = dc;
        }

        private ObservableCollection<TongjiClass> tongjis = new ObservableCollection<TongjiClass>();

        private void MyMacClass_Loaded(object sender, RoutedEventArgs e)
        {


        }

        private void gridMolds_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
        //多线
        private List<List<string>> ms = new List<List<string>>();
        //执行函数
        public void taskexcute()
        {
            ms.Clear();
            tongjis.Clear();
            info_richbox.Document.Blocks.Clear();
            List<DesignerItem> ds = dc.Children.OfType<DesignerItem>().Where(s => s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();
            if (ds != null && ds.Count > 0)
            {
                for (int i = 0; i < ds.Count; i++)
                {
                    //多线的个数
                    Connector ctr = dc.GetConnector(ds[i].ID, "Center");
                    List<Connection> connectionMGS = ctr.Connections.Where(t=> !t.MutilLineCount.Contains("2020")).ToList();
                    if (connectionMGS.Count > 1)
                    {
                        List<string> ls = new List<string>();
                        foreach (Connection c in connectionMGS)
                        {
                           
                            ls.Add(c.StartPoint);
                            ls.Add(c.EndPoint);
                        }
                        ms.Add(ls);

                    }
                    /*
                    Connector selectConnector = dc.GetConnector(ds[i].ID, "Center");
                    if (selectConnector.Connections.Count > 0)
                    {

                        continue;
                    }
                    */
                    TongjiClass tongji = new TongjiClass();
                    tongji.DesinItemID = ds[i].ID.ToString();
                    tongji.MuKuaiName = ds[i].MuKuaiName;
                    DesignerItem dd = findHutao(ds[i]);
                    if (dd != null)
                    {
                        tongji.GroupName =  dd.MuKuaiName;
                    }
                    else
                    {
                        tongji.GroupName =  "";
                    }
                    
                    tongji.Xzuobiao = Convert.ToString(Convert.ToInt32(Canvas.GetLeft(ds[i])));
                    tongji.Yzuobiao = Convert.ToString(Convert.ToInt32(Canvas.GetTop(ds[i])));
                    tongji.ZuoBiaoName = ds[i].ZuoBiaoName;
                    tongjis.Add(tongji);
                   

                }
               //MessageBox.Show(Newtonsoft.Json.JsonConvert.SerializeObject(ms));

            }
            //System.Diagnostics.Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(tongjis));
            gridMolds.ItemsSource = tongjis;
            //信息统计
            //if (content.ToString() == "System.Windows.Shapes.Path")//文字 System.Windows.Controls.TextBlock//组合System.Windows.Controls.Canvas//圆圈System.Windows.Shapes.Ellipse
            List<DesignerItem> designerItemsAll = dc.Children.OfType<DesignerItem>().Where(s => (s.Content.ToString().Contains("System.Windows.Shapes.Path") && s.ParentID.ToString() == "00000000-0000-0000-0000-000000000000" ) || (s.Content.ToString().Contains("System.Windows.Controls.Canvas") && s.ParentID.ToString() == "00000000-0000-0000-0000-000000000000" )).ToList();
            List<DesignerItem> designerItems = dc.Children.OfType<DesignerItem>().Where(s => (s.Content.ToString().Contains("System.Windows.Shapes.Path")&& s.ParentID.ToString()== "00000000-0000-0000-0000-000000000000"&& s.MuKuaiName.Length>0 && !s.MuKuaiName.Contains("$#%")  ) || (s.Content.ToString().Contains("System.Windows.Controls.Canvas") && s.ParentID.ToString() == "00000000-0000-0000-0000-000000000000" && s.MuKuaiName.Length> 0 && !s.MuKuaiName.Contains("$#%") )).ToList();
            List<DesignerItem> designerItemzuobiaos = dc.Children.OfType<DesignerItem>().Where(s => s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();
            List<Connection> connectionsAll = dc.Children.OfType<Connection>().ToList();
            //多线的统计
          
           
            Dictionary<int, int> pList = new Dictionary<int, int>();
            int lineCount = 0;
            //直线
            for (int i = 0; i < connectionsAll.Count; i++)
            {
                List<Connection> connectionsStart = connectionsAll.Where(s=>   s.StartPoint== connectionsAll[i].StartPoint).ToList();
                List<Connection> connectionsEnd = connectionsAll.Where(s => s.EndPoint == connectionsAll[i].EndPoint).ToList();
                if(connectionsStart.Count ==1   && connectionsEnd.Count ==1)
                {
                    lineCount += 1;
                }

            

                
            }

            //统计 connectionsMutilLine  里面的 list<string> 每个的数量

            for (int i = 0; i < ms.Count; i++)
            {
                int countRow = ms[i].Count/2; ;
                if (pList.ContainsKey(countRow) == false)
                {
                    pList.Add(countRow, 1);
                }
                else
                {
                    pList[countRow] = pList[countRow] + 1;
                }


            }
            Dictionary<int, int> dic1_SortedByKey = pList.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);

            TextRange rangeOfText1 = new TextRange(info_richbox.Document.ContentEnd, info_richbox.Document.ContentEnd);
            rangeOfText1.Text =  "1.模块总共数量：" + Convert.ToString(designerItemsAll.Count); 
            rangeOfText1.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

            TextRange rangeOfText2 = new TextRange(info_richbox.Document.ContentEnd, info_richbox.Document.ContentEnd);
            rangeOfText2.Text = Environment.NewLine;   
            rangeOfText2.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

            TextRange rangeOfText3 = new TextRange(info_richbox.Document.ContentEnd, info_richbox.Document.ContentEnd);
            rangeOfText3.Text = "2.模块使用数量：" + Convert.ToString(designerItems.Count) ;
            rangeOfText3.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

            TextRange rangeOfText4 = new TextRange(info_richbox.Document.ContentEnd, info_richbox.Document.ContentEnd);
            rangeOfText4.Text = Environment.NewLine;
            rangeOfText4.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

            TextRange rangeOfText5 = new TextRange(info_richbox.Document.ContentEnd, info_richbox.Document.ContentEnd);
            rangeOfText5.Text = "3.坐标点总共数量：" + Convert.ToString(designerItemzuobiaos.Count);
            rangeOfText5.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

            TextRange rangeOfText6 = new TextRange(info_richbox.Document.ContentEnd, info_richbox.Document.ContentEnd);
            rangeOfText6.Text = Environment.NewLine;
            rangeOfText6.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

            TextRange rangeOfText7 = new TextRange(info_richbox.Document.ContentEnd, info_richbox.Document.ContentEnd);
            rangeOfText7.Text = "4.坐标点使用数量：" + Convert.ToString(designerItemzuobiaos.Count-tongjis.Count);
            rangeOfText7.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

            TextRange rangeOfText8 = new TextRange(info_richbox.Document.ContentEnd, info_richbox.Document.ContentEnd);
            rangeOfText8.Text = Environment.NewLine;
            rangeOfText8.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

            TextRange rangeOfText9 = new TextRange(info_richbox.Document.ContentEnd, info_richbox.Document.ContentEnd);
            rangeOfText9.Text = "5.连接线总共数量：" + Convert.ToString(connectionsAll.Count);
            rangeOfText9.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

            TextRange rangeOfText10 = new TextRange(info_richbox.Document.ContentEnd, info_richbox.Document.ContentEnd);
            rangeOfText10.Text = Environment.NewLine;
            rangeOfText10.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

            TextRange rangeOfText11 = new TextRange(info_richbox.Document.ContentEnd, info_richbox.Document.ContentEnd);
            rangeOfText11.Text = "6.直线数量分布：" + Convert.ToString(lineCount);
            rangeOfText11.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

            TextRange rangeOfText12 = new TextRange(info_richbox.Document.ContentEnd, info_richbox.Document.ContentEnd);
            rangeOfText12.Text = Environment.NewLine;
            rangeOfText12.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

            TextRange rangeOfText13 = new TextRange(info_richbox.Document.ContentEnd, info_richbox.Document.ContentEnd);
            rangeOfText13.Text = "7.多线数量分布" ;
            rangeOfText13.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

            //遍历Key和Value
            foreach (var dic in dic1_SortedByKey)
            {
                TextRange rangeOfText14 = new TextRange(info_richbox.Document.ContentEnd, info_richbox.Document.ContentEnd);
                rangeOfText14.Text = Environment.NewLine;
                rangeOfText14.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

                TextRange rangeOfText15 = new TextRange(info_richbox.Document.ContentEnd, info_richbox.Document.ContentEnd);
                rangeOfText15.Text = " "+dic.Key+"个点多线数量：" + dic.Value;
                rangeOfText15.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
               // Console.WriteLine("Output Key : {0}, Value : {1} ", dic.Key, dic.Value);
            }

        }

        //输入一个点  找到 在那个护套里
        private DesignerItem findHutao(DesignerItem d)
        {

            List<DesignerItem> designerItems = dc.Children.OfType<DesignerItem>().Where(s => (s.Content.ToString().Contains("System.Windows.Shapes.Path") && s.ParentID.ToString() == "00000000-0000-0000-0000-000000000000" && s.MuKuaiName.Length > 0 && !s.MuKuaiName.Contains("$#%")) || (s.Content.ToString().Contains("System.Windows.Controls.Canvas") && s.ParentID.ToString() == "00000000-0000-0000-0000-000000000000" && s.MuKuaiName.Length > 0 && !s.MuKuaiName.Contains("$#%"))).ToList();
            if (designerItems.Count > 0)
            {
                DesignerItem r = null;
                foreach (DesignerItem item in designerItems)
                {
                    double x1 = Canvas.GetLeft(item);
                    double y1 = Canvas.GetTop(item);

                    double x2 = Canvas.GetLeft(item) + item.Width;
                    double y2 = Canvas.GetTop(item) + item.Height;

                    if (Canvas.GetLeft(d) >= x1 && Canvas.GetLeft(d) <= x2 && Canvas.GetTop(d) >= y1 && Canvas.GetTop(d) <= y2)
                    {
                        r = item;
                        break;
                    }

                }

                return r;
            }
            return null;


        }


        //判断字符串在不在 某个new List<List<string>>()
        private bool judgeInListstring(string p, List<List<string>> ls)
        {
            bool a = false;
            if (ls != null && ls.Count > 0)
            {
                for (int i = 0; i < ls.Count; i++)
                {
                    if (ls[i].Contains(p))
                    {
                        a = true;
                        break;
                    }
                }
            }
            return a;


        }
        private void MyMacClass_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void GridMolds_MouseUp(object sender, MouseButtonEventArgs e)
        {

            //d.SelectionService.AddToSelection(item);
            dc.SelectionService.ClearSelection();
            var items = gridMolds.SelectedItems;
            List<DesignerItem> designerItemSumsss = new List<DesignerItem>();
            foreach (TongjiClass item in items)
            {
                var querrys = dc.Children.OfType<DesignerItem>().Where(s => s.ID.ToString() == item.DesinItemID && s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();
                if (querrys != null && querrys.Count > 0)
                {
                    designerItemSumsss.Add(querrys[0]);
                }
            }


            foreach (DesignerItem ds in designerItemSumsss)
            {
                dc.SelectionService.AddToSelection(ds);
            }








        }
        //确定转为kong点
        private void Button_Click(object sender, RoutedEventArgs e)
        {
          

            System.Windows.Forms.MessageBoxButtons messButton = System.Windows.Forms.MessageBoxButtons.OKCancel;
            System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show("确定要置为空吗","确定", messButton);

            if (dr == System.Windows.Forms.DialogResult.OK)//如果点击“确定”按钮

            {
                var items = gridMolds.SelectedItems;
                foreach (TongjiClass item in items)
                {
                    item.MuKuaiName = "K";
                }
                //有连线的不能清空
                List<DesignerItem> designerItemSumsss = new List<DesignerItem>();
                foreach (TongjiClass item in items)
                {
                    var querrys = dc.Children.OfType<DesignerItem>().Where(s => s.ID.ToString() == item.DesinItemID && s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();
                    if (querrys != null && querrys.Count > 0)
                    {
                        designerItemSumsss.Add(querrys[0]);
                    }
                }


                foreach (DesignerItem ds in designerItemSumsss)
                {
                    ds.MuKuaiName = "K";
                }


            }
            else//如果点击“取消”按钮
            {



            }



        }




    }
}
