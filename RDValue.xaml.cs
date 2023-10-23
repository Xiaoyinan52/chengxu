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

namespace WireTestProgram.RDValueStudy
{
    /// <summary>
    /// RDValue.xaml 的交互逻辑
    /// </summary>
    public partial class RDValue :Window
    {
        private DesignerCanvas dc = null;
        public RDValue(DesignerCanvas dc)
        {
            InitializeComponent();
            this.dc = dc;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true; // this cancels the close event.
        }

        private void Lselect_btn_Click(object sender, RoutedEventArgs e)
        {
            List<Connection> selectedConnections =
                             dc.SelectionService.CurrentSelection.OfType<Connection>().ToList();
            if (selectedConnections.Count == 1)
            {
                List<string> a = new List<string>();
                a.Add(selectedConnections[0].StartPoint);
                LineLeftPoints_txt.Text = Newtonsoft.Json.JsonConvert.SerializeObject(a).ToString();
                List<string> b = new List<string>();
                b.Add(selectedConnections[0].EndPoint);
                LineRightPoints_txt.Text = Newtonsoft.Json.JsonConvert.SerializeObject(b).ToString(); 
            }
           
        }

        private void MLselect_btn_Click(object sender, RoutedEventArgs e)
        {
            List<Connection> selectedConnections =
                            dc.SelectionService.CurrentSelection.OfType<Connection>().ToList();
            var d01Start = selectedConnections.Select(t => t.StartPoint).Distinct();
            var d01End = selectedConnections.Select(t => t.EndPoint).Distinct();


            
            List<string> listA = new List<string>();
            foreach (var list in d01Start)
            {
                listA.Add(list);
            }

            
            List<string> listB = new List<string>();
            foreach (var list in d01End)
            {
                listB.Add(list);
            }
            List<string> c = listA.Union(listB).Distinct().OrderBy(r => System.Int32.Parse(r, System.Globalization.NumberStyles.HexNumber)).ToList<string>();
            //给c排序 保证第一个最小
           
            MLineLeftPoints_txt.Text = Newtonsoft.Json.JsonConvert.SerializeObject(c).ToString(); 


        }





        private string returnType()
        {
            string str ="L";
            if (this.dianzu_cmb.IsChecked == true)
            {
                str = "R";
            }
            else if (this.erjiguan_cmb.IsChecked == true)
            {
                str = "D";
            }
            else if (this.dianzuHeerjiguan_cmb.IsChecked == true)
            {
                str = "RD";
            }

            return str;
        }

        
        private void MLright_btn_Click(object sender, RoutedEventArgs e)
        {
            List<Connection> selectedConnections =
                            dc.SelectionService.CurrentSelection.OfType<Connection>().ToList();
            var d01Start = selectedConnections.Select(t => t.StartPoint).Distinct();
            var d01End = selectedConnections.Select(t => t.EndPoint).Distinct();



            List<string> listA = new List<string>();
            foreach (var list in d01Start)
            {
                listA.Add(list);
            }


            List<string> listB = new List<string>();
            foreach (var list in d01End)
            {
                listB.Add(list);
            }
            List<string> c = listA.Union(listB).Distinct().OrderBy(r => System.Int32.Parse(r, System.Globalization.NumberStyles.HexNumber)).ToList<string>();
            MLineRightPoints_txt.Text = Newtonsoft.Json.JsonConvert.SerializeObject(c).ToString(); ;
        }

        private void Lsave_btn_Click(object sender, RoutedEventArgs e)
        {
            List<string> a = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(LineLeftPoints_txt.Text);
            List<string> b = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(LineRightPoints_txt.Text);
            if (a.Count == 1 && b.Count == 1)
            {
                List<Connection> c = dc.Children.OfType<Connection>().Where(s => (s.StartPoint == a[0] && s.EndPoint == b[0]) || (s.EndPoint == a[0] && s.StartPoint == b[0])).ToList();
                if (c.Count == 1)
                {
                    if (returnType() != "L" && a.Count > 0 && b.Count > 0)
                    {
                        c[0].RDType = returnType();
                        c[0].RDValue = RDValue_txt.Text;
                        c[0].RDDirection = RDDirection_cmb.Text;
                        c[0].MutilLineCount = "1";
                        c[0].LeftPoints = LineLeftPoints_txt.Text;
                        c[0].RightPoints = LineRightPoints_txt.Text;
                        MessageBox.Show("保存成功！！！");

                    }
                    

                }


            }
          
            
        }

        private void MLsave_btn_Click(object sender, RoutedEventArgs e)
        {
            List<string> a = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(MLineLeftPoints_txt.Text);
            List<string> b = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(MLineRightPoints_txt.Text);
            if (a.Count >0 && b.Count > 0)
            {
                //多线 大点数量
                bool aa = false;

                if (System.Int32.Parse(a[0], System.Globalization.NumberStyles.HexNumber) > System.Int32.Parse(b[0], System.Globalization.NumberStyles.HexNumber))
                {
                    //RDMutilQty_txt.Text = a.Count.ToString();
                    aa = true;
                }
                else
                {
                    //RDMutilQty_txt.Text = b.Count.ToString();
                    aa = false;
                }

                //存在线  去掉
                List<Connection> c = dc.Children.OfType<Connection>().Where(s => (s.StartPoint == a[0] && s.EndPoint == b[0]) || (s.EndPoint == a[0] && s.StartPoint == b[0])).ToList();
                if (c.Count >0 )
                {
                    dc.Children.Remove(c[0]);
                    dc.UpdateZIndex(); 
                }
                //更改导线的属性
                if (returnType() != "L" && a.Count > 0 && b.Count > 0)
                    {
                    //画线
                     Connection lc=   dc.getConnectorBaseOnZuoBiaoName(a[0],b[0]);
                     lc.RDType = returnType();
                     lc.RDValue = RDValue_txt.Text;
                     lc.RDDirection = RDDirection_cmb.Text;
                     lc.MutilLineCount = "2020";
                     if (aa == true)
                     {
                         lc.LeftPoints = MLineRightPoints_txt.Text;
                         lc.RightPoints = MLineLeftPoints_txt.Text;
                     }
                     else
                     {
                         lc.LeftPoints = MLineLeftPoints_txt.Text;
                         lc.RightPoints = MLineRightPoints_txt.Text;


                     }
                   
                     MessageBox.Show("保存成功！！！");

                    }


               
            }


        }

  


    }



}
