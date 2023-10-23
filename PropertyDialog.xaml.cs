using HslCommunication.LogNet;
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
    /// PropertyDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PropertyDialog : MyMacClass
    {
        private ILogNet logNet = new LogNetSingle(System.Windows.Forms.Application.StartupPath + "\\Logs\\DesignLog.txt");
        DesignerCanvas dc;
        DesignerItem d;
        public PropertyDialog(DesignerCanvas dc, DesignerItem d)
        {
            InitializeComponent();
            this.dc = dc;
            this.d = d;

            gridProductDetails.DataContext = d;
            Xzuobiao_txt.Text = Convert.ToString(Convert.ToInt32(Canvas.GetLeft(d)));
            Yzuobiao_txt.Text = Convert.ToString(Convert.ToInt32(Canvas.GetTop(d)));

            if (d.ZuoBiaoName != null )
            {
                Clipboard.SetDataObject(d.ZuoBiaoName);
             
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


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DesignerItem dd = findHutao(d);
            if (dd != null)
            {
                itemGroup_txb.Text = dd.MuKuaiName;
            }
            else
            {
                itemGroup_txb.Text = "";
            }

            if (!string.IsNullOrEmpty(zuobiao_txb.Text))
            {
                try
                {
                    int s0 = System.Int32.Parse(zuobiao_txb.Text, System.Globalization.NumberStyles.HexNumber);
                    int chu = 0;
                    //int chu = s0 / 64 ;
                    int yu = s0 % 64;
                    if (yu == 0)
                    {
                        chu = s0 / 64;
                        yu = 64;
                    }
                    else
                    {
                         chu = s0 / 64   +1;

                    }
                    address_txb.Text = chu.ToString() + "-" + yu.ToString(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                    logNet.WriteDebug("转换点坐标 error", ex.Message);
                    logNet.WriteDebug("----------");
                }

//转换成10进制
               


            }
            address1_txt.Text = "";
            address1_txt.Focus();
        }

        private void Address1_txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrEmpty(address1_txt.Text))
                {
                    try
                    {
                        int s0 = System.Int32.Parse(address1_txt.Text, System.Globalization.NumberStyles.HexNumber);
                        int chu = 0;
                        //int chu = s0 / 64 ;
                        int yu = s0 % 64;
                        if (yu == 0)
                        {
                            chu = s0 / 64;
                            yu = 64;
                        }
                        else
                        {
                            chu = s0 / 64 + 1;

                        }
                        address2_txt.Text = chu.ToString() + "-" + yu.ToString();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                        logNet.WriteDebug("转换点坐标 error", ex.Message);
                        logNet.WriteDebug("----------");
                    }

                    //转换成10进制



                }


            }
         }




    }
}
