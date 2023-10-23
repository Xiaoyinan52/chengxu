using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WireTestProgram.Controls;
using System.Data;
using WireTestProgram.pinDian;
using WireTestProgram.gasLock;
using WireTestProgram.HelperClass;
using System.Threading;
using WireTestProgram.RDValueStudy;
using HslCommunication.LogNet;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WireTestProgram
{
    public partial class DesignerCanvas
    {
        private ILogNet logNet = new LogNetSingle(System.Windows.Forms.Application.StartupPath + "\\Logs\\DesignLog.txt");
        public static RoutedCommand Group = new RoutedCommand();
        public static RoutedCommand Ungroup = new RoutedCommand();
        public static RoutedCommand BringForward = new RoutedCommand();
        public static RoutedCommand BringToFront = new RoutedCommand();
        public static RoutedCommand SendBackward = new RoutedCommand();
        public static RoutedCommand SendToBack = new RoutedCommand();
        public static RoutedCommand AlignTop = new RoutedCommand();
        public static RoutedCommand AlignVerticalCenters = new RoutedCommand();
        public static RoutedCommand AlignBottom = new RoutedCommand();
        public static RoutedCommand AlignLeft = new RoutedCommand();
        public static RoutedCommand AlignHorizontalCenters = new RoutedCommand();
        public static RoutedCommand AlignRight = new RoutedCommand();
        public static RoutedCommand DistributeHorizontal = new RoutedCommand();
        public static RoutedCommand DistributeVertical = new RoutedCommand();
        public static RoutedCommand SelectAll = new RoutedCommand();
        //自定义
        public static RoutedCommand ShuipingKuoda = new RoutedCommand();
        public static RoutedCommand ShuipingSuoxiao = new RoutedCommand();
        public static RoutedCommand ShuzhiKuoda = new RoutedCommand();
        public static RoutedCommand ShuzhiSuoxiao = new RoutedCommand();

        public static RoutedCommand DeleteOwn = new RoutedCommand();
        public static RoutedCommand PasteOwn = new RoutedCommand();
        public static RoutedCommand Chexiao = new RoutedCommand();
        public static RoutedCommand MokuaiBeijing = new RoutedCommand();
        public static RoutedCommand LeftBeijing = new RoutedCommand();
        public static RoutedCommand MuKuaiName = new RoutedCommand();
        //public static RoutedCommand ZuoBiaoName = new RoutedCommand();
        //public static RoutedCommand ConnectionRelation = new RoutedCommand();
        public static RoutedCommand PropertyDialogCMD = new RoutedCommand();
        public static RoutedCommand ReadDataFromHardware = new RoutedCommand();
       // public static RoutedCommand GasLockLearn = new RoutedCommand();
        public static RoutedCommand HowManyConnection = new RoutedCommand();
        //public static RoutedCommand DioLearn = new RoutedCommand();
        public static RoutedCommand ResistanceLearn = new RoutedCommand();
        public static RoutedCommand ConnectionProperty = new RoutedCommand();
        //之测量框子
        private RDValue rdValue;
        public DesignerCanvas()
        {
           

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.New, New_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Open_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, Print_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, Cut_Executed, Cut_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, Copy_Executed, Copy_Enabled));


            // this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Paste_Executed, Paste_Enabled));
            // this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, Delete_Executed, Delete_Enabled));

            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.ShuipingKuoda, ShuipingKuoda_Executed, ShuipingKuoda_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.ShuipingSuoxiao, ShuipingSuoxiao_Executed, ShuipingSuoxiao_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.ShuzhiKuoda, ShuzhiKuoda_Executed, ShuzhiKuoda_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.ShuzhiSuoxiao, ShuzhiSuoxiao_Executed, ShuzhiSuoxiao_Enabled));


            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.DeleteOwn, Delete_Executed, Delete_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.PasteOwn, Paste_Executed, Paste_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.Chexiao, Chexiao_Executed, Chexiao_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.Group, Group_Executed, Group_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.Ungroup, Ungroup_Executed, Ungroup_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.BringForward, BringForward_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.BringToFront, BringToFront_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SendBackward, SendBackward_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SendToBack, SendToBack_Executed, Order_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignTop, AlignTop_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignVerticalCenters, AlignVerticalCenters_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignBottom, AlignBottom_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignLeft, AlignLeft_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignHorizontalCenters, AlignHorizontalCenters_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.AlignRight, AlignRight_Executed, Align_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.DistributeHorizontal, DistributeHorizontal_Executed, Distribute_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.DistributeVertical, DistributeVertical_Executed, Distribute_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SelectAll, SelectAll_Executed));
            //
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.MokuaiBeijing, MokuaiBeijing_Executed));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.LeftBeijing, LeftBeijing_Executed));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.MuKuaiName, MukuaiName_Executed));
            
           // this.CommandBindings.Add(new CommandBinding(DesignerCanvas.ConnectionRelation, ConnectionRelation_Executed));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.PropertyDialogCMD, PropertyDialog_Executed));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.ReadDataFromHardware, ReadDataFromHardware_Executed));
            //this.CommandBindings.Add(new CommandBinding(DesignerCanvas.GasLockLearn, GasLockLearn_Executed));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.HowManyConnection, HowManyConnection_Executed));
            //this.CommandBindings.Add(new CommandBinding(DesignerCanvas.DioLearn, DioLearn_Executed));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.ResistanceLearn, ResistanceLearn_Executed));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.ConnectionProperty, ConnectionProperty_Executed));
            //

            SelectAll.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Control));

            this.AllowDrop = true;
            Clipboard.Clear();

            rdValue = new RDValue(this);

            
    }
        //自定义
       
        //public List<Connection> connectionSum { get; set; }
        //public List<DesignerItem> designerItemSum { get; set; }

        public string fmodelInsert { get; set; }
        private void MokuaiBeijing_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            DesignerCanvas dc = (DesignerCanvas)sender;
            if (dc.Tag.ToString() == "design")
            {
                DesignerItem selectedDesignerItem =
                            this.SelectionService.CurrentSelection.OfType<DesignerItem>().Where(s => s.Content.ToString().Contains("System.Windows.Shapes.Path")).ToList().FirstOrDefault();


                System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
               
                openFileDialog.InitialDirectory = string.Format(@"{0}\images", System.Windows.Forms.Application.StartupPath);     //打开对话框后的初始目录
                openFileDialog.Filter = "图片|*.jpg;*.png;*.gif;*.bmp;*.jpeg";
                openFileDialog.RestoreDirectory = false;    //若为false，则打开对话框后为上次的目录。若为true，则为初始目录
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    if (selectedDesignerItem != null)
                    {

                        string openFileName = openFileDialog.FileName;

                        if (openFileName.Contains(string.Format(@"{0}\images", System.Windows.Forms.Application.StartupPath)))
                        {
                            //自定义属性字段
                            selectedDesignerItem.BKImgSource = openFileName.Replace(string.Format(@"{0}\images", System.Windows.Forms.Application.StartupPath), "");
                            /*
                            ImageBrush imagebrush = new ImageBrush();
                            //BitmapImage bitmapImage=  new BitmapImage(new Uri(openFileName, UriKind.Absolute));
                            BitmapImage bitmapImage = new BitmapImage();


                            bitmapImage.BeginInit();  //给BitmapImage对象赋予数据的时候，需要用BeginInit()开始，用EndInit()结束
                            bitmapImage.UriSource = new Uri(openFileName, UriKind.Absolute);
                            //bitmapImage.DecodePixelWidth = 320;   //对大图片，可以节省内存。尽可能不要同时设置DecodePixelWidth和DecodePixelHeight，否则宽高比可能改变
                            bitmapImage.EndInit();

                            //MessageBox.Show(bitmapImage.PixelHeight.ToString());
                            //MessageBox.Show(bitmapImage.PixelWidth.ToString());
                            imagebrush.ImageSource = bitmapImage;
                            selectedDesignerItem.Background = imagebrush;
                            */
                            System.IO.FileStream fs =new System.IO.FileStream(openFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                            byte[] buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, buffer.Length);
                            fs.Close();
                            fs.Dispose();
                            System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.StreamSource = ms;
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.EndInit();
                            ms.Dispose();
                            ImageBrush imagebrush = new ImageBrush();
                            imagebrush.ImageSource = bitmapImage;
                            selectedDesignerItem.Background = imagebrush;
                           

                        }
                        else
                        {
                            MessageBox.Show("请把模块背景放入测试程序下的images文件夹下！！！");
                        }

                    }

                }
                    
                   
            }
            else
            {
                MessageBox.Show("没有相关权限！");
            }

        }
        private void LeftBeijing_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DesignerCanvas dc = (DesignerCanvas)sender;
            if (dc.Tag.ToString() == "design")
            {

                DesignerItem selectedDesignerItem =
                            this.SelectionService.CurrentSelection.OfType<DesignerItem>().Where(s => s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList().FirstOrDefault(); 
               

               
                System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //System.Drawing.Color c1 = colorDialog.Color;
                    System.Windows.Forms.ColorDialog colorDialog2 = new System.Windows.Forms.ColorDialog();
                    if (colorDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Ellipse nodeCircle = new Ellipse();
                        nodeCircle.Stroke = new SolidColorBrush(Colors.Black);
                        LinearGradientBrush brush = new LinearGradientBrush();

                       
                        brush.StartPoint = new Point(0.5, 0);
                        brush.EndPoint = new Point(0.5, 1);
                        System.Windows.Media.Color color = new System.Windows.Media.Color();
                        color.A = colorDialog.Color.A;
                        color.B = colorDialog.Color.B;
                        color.G = colorDialog.Color.G;
                        color.R = colorDialog.Color.R;
                        GradientStop gs1 = new GradientStop();
                        gs1.Offset = 0;
                        gs1.Color = color;
                        brush.GradientStops.Add(gs1);

                        GradientStop gs2 = new GradientStop();
                        gs2.Offset = 0.7;
                        gs2.Color = color;
                        brush.GradientStops.Add(gs2);

                        GradientStop gs3 = new GradientStop();
                        gs3.Offset = 0.71;
                        gs3.Color = Colors.White;
                        brush.GradientStops.Add(gs3);

                        System.Windows.Media.Color color2 = new System.Windows.Media.Color();
                        color2.A = colorDialog2.Color.A;
                        color2.B = colorDialog2.Color.B;
                        color2.G = colorDialog2.Color.G;
                        color2.R = colorDialog2.Color.R;
                        GradientStop gs4 = new GradientStop();
                        gs4.Offset = 0.71;
                        gs4.Color = color2;
                        //因为GradientStops属性返回一个GradientStopCollection对象，而且GradientStopCollection类实现了IList接口
                        brush.GradientStops.Add(gs4);//所以这句代码的本质是IList list = brush.GradientStops; list.Add(gs3);



                        GradientStop gs5 = new GradientStop();
                        gs5.Offset = 1;
                        gs5.Color = color2;
                        //因为GradientStops属性返回一个GradientStopCollection对象，而且GradientStopCollection类实现了IList接口
                        brush.GradientStops.Add(gs5);//所以这句代码的本质是IList list = brush.GradientStops; list.Add(gs3);



                        nodeCircle.Fill = brush;

                        selectedDesignerItem.Content = nodeCircle;
                    }

                 



                }
            }
            else
            {
                MessageBox.Show("没有相关权限！");
            }

        }
        public string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        private void MukuaiName_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            //var mi = e.Source as MenuItem;
            //var cm = mi.Parent as ContextMenu;
            //var des = cm.PlacementTarget as DesignerItem;
            //var descan = des.Parent as DesignerCanvas;
            //MessageBox.Show(descan.Tag.ToString());

            DesignerCanvas dc = (DesignerCanvas)sender;
            if (dc.Tag.ToString() == "design")
            {  int ffontsize = 0;
               DBaccessHelp db = new DBaccessHelp();
                DataTable dt = db.ExecuteQuery("select scanModel,memoryModel,shortBreak,comport,fmodelLength,fbarcodeLength,fbarcodeFront,switchcomport,fontsize  from  setTable where setFlag='set'");

                ffontsize = Convert.ToInt32(dt.Rows[0][8]);

                db.closeOleDbConnection();
            



        DesignerItem selectedDesignerItem =
                            this.SelectionService.CurrentSelection.OfType<DesignerItem>().Where(s => !s.Content.ToString().Contains("System.Windows.Controls.TextBlock")).ToList().FirstOrDefault();

                String PM = Microsoft.VisualBasic.Interaction.InputBox("请以#号开头输入模块名，尽量不要重复", "命名提示", "", -1, -1);
                if (selectedDesignerItem != null)
                {

                    //方块命名
                    if (selectedDesignerItem.Content.ToString().Contains("System.Windows.Shapes.Path") || selectedDesignerItem.Content.ToString().Contains("System.Windows.Controls.Canvas"))
                    {
                        if (PM != string.Empty)
                        {


                            if (PM.Substring(0, 1) == "#")
                            {
                                if (PM.Length > 1)
                                {
                                    DesignerItem newItem = new DesignerItem();
                                    SetConnectorDecoratorTemplate(newItem);

                                    newItem.MuKuaiName = "$#%" + GetTimeStamp();
                                    newItem.ConnectorVisble = true;
                                  
                                    TextBlock tx = new TextBlock();
                                    //Color ccc = (Color)ColorConverter.ConvertFromString("#007ACC");
                                    
                                    

                                    tx.IsHitTestVisible = false;
                                    


                                    tx.Text = PM;
                                    tx.TextWrapping = TextWrapping.Wrap;
                                    tx.VerticalAlignment = VerticalAlignment.Stretch;
                                    tx.HorizontalAlignment = HorizontalAlignment.Stretch;
                                    tx.FontSize = ffontsize;
                                    //tx.FontWeight = FontWeights.Bold;
                                    tx.Foreground = Brushes.White;
                                    tx.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)); //252,3,18 222, 237, 249

                                    tx.Width = 216;
                                    tx.Height = 120;
                                    newItem.Content = tx;

                                    newItem.ItemContent .Append (XamlWriter.Save(newItem.Content));
                                    //MessageBox.Show(XamlWriter.Save(newItem.Content));


                                    newItem.Width = 38;
                                    newItem.Height = 18;


                                    DesignerCanvas.SetLeft(newItem, Canvas.GetLeft(selectedDesignerItem));
                                    DesignerCanvas.SetTop(newItem, Canvas.GetTop(selectedDesignerItem));
                                    Canvas.SetZIndex(newItem, this.Children.Count);
                                    this.Children.Add(newItem);
                                    //update selection
                                    this.SelectionService.SelectItem(newItem);
                                    newItem.Focus();
                                    selectedDesignerItem.MuKuaiName = PM;
                                }
                                else
                                {
                                    selectedDesignerItem.MuKuaiName = "";
                                }
                            }
                            else
                            {
                                MessageBox.Show("警告！！！模块命名请以#号开始！！！");
                            }


                        }

                    }
                    /*
                    //圆圈命名
                    else if (selectedDesignerItem.Content.ToString().Contains("System.Windows.Shapes.Ellipse"))
                   {
                       if (PM != string.Empty)
                       {

                           if (PM.Substring(0, 1) == "#" && PM.Length == 1)
                           {

                               selectedDesignerItem.MuKuaiName ="";
                               return;

                           }

                               List<DesignerItem> designItems = this.Children.OfType<DesignerItem>().Where(s => s.Content.ToString().Contains("System.Windows.Shapes.Ellipse") && !string.IsNullOrEmpty(s.MuKuaiName) && s.MuKuaiName == PM).ToList();


                               if (designItems.Count > 1)
                               {
                                   MessageBox.Show("警告！！！坐标点命名重名！");
                                   this.SelectionService.ClearSelection();
                                   foreach (var item in designItems)
                                   {


                                       this.SelectionService.AddToSelection(item);

                                   }
                                   return;
                               }
                               selectedDesignerItem.MuKuaiName = PM;
                          


                }
            }
                    else if (selectedDesignerItem.Content.ToString().Contains("System.Windows.Controls.TextBlock"))
                    {
                        MessageBox.Show("警告！！！非法操作！！！");
                    }
            */



                }
            }
            else
            {
                MessageBox.Show("没有相关权限！");
            }

        }
        //
      

        private void PropertyDialog_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DesignerItem selectedConnections =
                       this.SelectionService.CurrentSelection.OfType<DesignerItem>().ToList().FirstOrDefault();
            if (selectedConnections != null)
            {
                PropertyDialog d = new PropertyDialog(this,selectedConnections);
                d.ShowDialog();
            }
         
        }
      
       


        //根据坐标命名  来连线 
        
        public Connection getConnectorBaseOnZuoBiaoName(string zuobBiaoName1, string zuobBiaoName2)
        {

            
            List<DesignerItem> designItem = this.Children.OfType<DesignerItem>().Where(s => (s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")) && s.ZuoBiaoName == zuobBiaoName1 || s.ZuoBiaoName == zuobBiaoName2).ToList();
                
               
            if (designItem.Count == 2)
            {
                //坐标点比较大小
                int s0 = 0;
                int s1 = 0;
                int P0 = System.Int32.Parse(designItem[0].ZuoBiaoName, System.Globalization.NumberStyles.HexNumber);
                int P1 = System.Int32.Parse(designItem[1].ZuoBiaoName, System.Globalization.NumberStyles.HexNumber);
                if (P0 > P1)
                {
                    s0 = P1;
                    s1 = P0;
                }
                else
                {
                    s0 = P0;
                    s1 = P1;
                }
                var connectionsMou = this.Children.OfType<Connection>().Where(item => item.StartPoint== s0.ToString("X4")  &&  item.EndPoint== s1.ToString("X4")).ToList();
                if(connectionsMou!=null  && connectionsMou.Count > 0)
                {
                    MessageBox.Show("系统检测到已经存在连线关系共" + connectionsMou.Count.ToString() + " 当前要连接的第一个点:" + zuobBiaoName1 + "当前要连接的第二个点:" + zuobBiaoName2);
                    return null;
                }


                Connector sourceConnector = GetConnector(designItem[0].ID, "Center");
                Connector sinkConnector = GetConnector(designItem[1].ID, "Center");
               
                Connection connection = new Connection(sourceConnector, sinkConnector);
              


                connection.StartPoint = s0.ToString("X4");
                connection.EndPoint = s1.ToString("X4");

                connection.PathGeometry = new PathGeometry();
                PathFigure figure = new PathFigure();
                figure.StartPoint = sourceConnector.Position;
                figure.Segments.Add(new LineSegment(sinkConnector.Position, true));
                connection.PathGeometry.Figures.Add(figure);

                this.Children.Add(connection);
                Canvas.SetZIndex(connection, this.Children.Count);
               


                return connection;
            }
            else
            {
                MessageBox.Show("系统检测到共"+ designItem.Count.ToString() + "个元素。当前要连接的第一个点:"+ zuobBiaoName1+ "当前要连接的第二个点:" + zuobBiaoName2);
                return null;
            }

        }

        //ReadDataFromHardware_Executed
        private void ReadDataFromHardware_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DesignerCanvas dc = (DesignerCanvas)sender;
            if (dc.Tag.ToString() == "design")
            {

                DesignerItem d =
                            this.SelectionService.CurrentSelection.OfType<DesignerItem>().Where(s => s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList().FirstOrDefault();
                if (d != null)
                {


                    ReadDataFromHardWare read = new ReadDataFromHardWare(this.bankaQty);
                    if (read.ShowDialog() == true)
                    {
                        //MessageBox.Show(read.mtextshow.SerialData);
                        //1取得数据 read.mtextshow.SerialData
                        //2给当前元素定义坐标名

                        List<DesignerItem> designItem = this.Children.OfType<DesignerItem>().Where(s => s.ZuoBiaoName == read.mtextshow.SerialData && s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();



                        if (designItem.Count == 1)
                        {

                            if (MessageBox.Show("是否覆盖当前点？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                            {
                                selectionService.ClearSelection();
                                designItem[0].ZuoBiaoName = "";
                                d.ZuoBiaoName = read.mtextshow.SerialData;

                                this.SelectionService.SelectItem(d);
                                d.Focus();
                            }
                            else
                            {
                                selectionService.ClearSelection();
                                this.SelectionService.SelectItem(designItem[0]);
                                designItem[0].Focus();
                            }
                        }
                        else if (designItem.Count == 0)
                        {
                            d.ZuoBiaoName = read.mtextshow.SerialData;

                            this.SelectionService.SelectItem(d);
                            d.Focus();
                        }
                        else if (designItem.Count > 1)
                        {
                            MessageBox.Show("警告！！！存在不只一个相同的物理坐标点！");
                        }

                    }
                    read.Close();
                }

            }
            else
            {
                MessageBox.Show("没有相关权限！");
            }
        }
        /*
        private void GasLockLearn_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DesignerCanvas dc = (DesignerCanvas)sender;
            if (dc.Tag.ToString() == "design")
            {
                List<DesignerItem> selectedConnections =
                              this.SelectionService.CurrentSelection.OfType<DesignerItem>().ToList();
                if (selectedConnections.Count == 2)
                {
                    GasLockPinDian gasLockPinDian = new GasLockPinDian();

                    if (gasLockPinDian.ShowDialog() == true)
                    {
                        if (!string.IsNullOrEmpty(gasLockPinDian.startzuobiaoName.Text) && !string.IsNullOrEmpty(gasLockPinDian.endzuobiaoName.Text))
                        {
                            selectedConnections[0].ZuoBiaoName = gasLockPinDian.startzuobiaoName.Text;
                            selectedConnections[1].ZuoBiaoName = gasLockPinDian.endzuobiaoName.Text;
                            //画线

                            Connector sourceConnector = GetConnector(selectedConnections[0].ID, "Center");
                            Connector sinkConnector = GetConnector(selectedConnections[1].ID, "Center");

                            Connection newConnection = new Connection(sourceConnector, sinkConnector);
                            newConnection.StartPoint = gasLockPinDian.startzuobiaoName.Text;
                            newConnection.EndPoint = gasLockPinDian.endzuobiaoName.Text;
                            newConnection.PathGeometry = new PathGeometry();
                            PathFigure figure = new PathFigure();
                            figure.StartPoint = sourceConnector.Position;
                            figure.Segments.Add(new LineSegment(sinkConnector.Position, true));
                            newConnection.PathGeometry.Figures.Add(figure);

                            Canvas.SetZIndex(newConnection, this.Children.Count);
                            this.Children.Add(newConnection);
                        }
                    }
                    gasLockPinDian.Close();
                }
            }
            else
            {
                MessageBox.Show("没有相关权限！");
            }
        }
         * */
        private void HowManyConnection_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DesignerCanvas dc = (DesignerCanvas)sender;
            if (dc.Tag.ToString() == "design")
            {
                List<DesignerItem> selectedDesignerItem =
                            this.SelectionService.CurrentSelection.OfType<DesignerItem>().Where(s => s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();
                List<Connection> connections = (from item in this.Children.OfType<Connection>()
                                                select item).ToList();
                
                 if (selectedDesignerItem.Count == 1)
                 {
                     Connector connector = this.GetConnector(selectedDesignerItem[0].ID, "Center");
                     List<Connection> list = connector.Connections;
                    searchConnections.Clear();
                   
                     if (list.Count > 0)
                     {
                         searchConnection(list[0]);
                         var exp = connections.Where(a => !searchConnections.Exists(t => (a.ID.ToString()).Contains(t.ID.ToString()))    ).ToList();
                         // Console.WriteLine("--查找connections集合中存在，而list不存在的数据--");
                         foreach (var item in exp)
                         {
                             item.Visibility = Visibility.Hidden;
                         }
                        foreach (var item in searchConnections)
                        {
                            item.Visibility = Visibility.Visible;
                        }
                    }
                 }
 
              


            }
            else
            {
                MessageBox.Show("没有相关权限！");
            }

        }
        //获取连线有多少连线
        public List<Connection> searchConnections = new List<Connection>();

        public void searchConnection(Connection conn)
        {
           

           
            searchConnections.Add(conn);
            //var connections = this.Children.OfType<Connection>().Where(item => (item.StartPoint == conn.StartPoint || item.EndPoint == conn.EndPoint || item.StartPoint == conn.EndPoint || item.EndPoint == conn.StartPoint) && !searchConnections.Exists(t => (t.ID.ToString()).Contains(item.ID.ToString()))).ToList();
            var connections = this.Children.OfType<Connection>().Where(item => (item.StartPoint == conn.StartPoint || item.EndPoint == conn.EndPoint || item.StartPoint == conn.EndPoint || item.EndPoint == conn.StartPoint) ).ToList();
            var exp = connections.Where(a => !searchConnections.Exists(t => (a.ID.ToString()).Contains(t.ID.ToString()))).ToList();



            if (exp.Count > 0)
            {
                for (int i = 0; i < exp.Count; i++)
                {
                    searchConnections.Add(exp[i]);
                    searchConnection(exp[i]);
                }

                    


            }
            else
            {
                return;

            }
        

           
        }

        
        private void ResistanceLearn_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.Tag.ToString() == "design")
            {

                rdValue.ShowDialog();
            }
            else
            {
                MessageBox.Show("没有相关权限！");
            }


        }
        
        private void ConnectionProperty_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DesignerCanvas dc = (DesignerCanvas)sender;
            //  if (dc.Tag.ToString() == "design")
            //{
            List<Connection> selectedConnections =
                     this.SelectionService.CurrentSelection.OfType<Connection>().ToList();
            if (selectedConnections.Count == 1)
            {//当前连线的 终点坐标 有没有其它连线
                string endPoint = selectedConnections[0].EndPoint;
                List<Connection> ConnnConnections = (from item in this.Children.OfType<Connection>()
                                        where item.StartPoint == endPoint || item.EndPoint == endPoint
                                        select item).ToList();


                ConnectionPropertyWindow connectionProperty = new ConnectionPropertyWindow(selectedConnections[0], ConnnConnections.Count);
                connectionProperty.ShowDialog();
            }
            //}
            //  else
            //  {
            //      MessageBox.Show("没有相关权限！");
            //  }

        }

        //zidingyi
        private List<ImageInfoClass> imageInfoList;
        #region New Command

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            System.Windows.Forms.MessageBoxButtons messButton = System.Windows.Forms.MessageBoxButtons.OKCancel;
            System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show("确定要新建吗" , "新建", messButton);

            if (dr == System.Windows.Forms.DialogResult.OK)//如果点击“确定”按钮

            {
              
            double x = SystemParameters.WorkArea.Width;//得到屏幕工作区域宽度
            double y = SystemParameters.WorkArea.Height;//得到屏幕工作区域高度
            double x1 = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
            double y1 = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度

            //MessageBox.Show(y.ToString()+"      "+y1.ToString());
           


            this.Children.Clear();
            this.SelectionService.ClearSelection();

            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog1.Description = "请选择文件夹";

            folderBrowserDialog1.ShowNewFolderButton = true;
            //folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop;
            folderBrowserDialog1.SelectedPath = string.Format(@"{0}\images", System.Windows.Forms.Application.StartupPath);
            System.Windows.Forms.DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)

            {
                if (!folderBrowserDialog1.SelectedPath.Contains(string.Format(@"{0}\images", System.Windows.Forms.Application.StartupPath)))
                {
                    MessageBox.Show("请选择测试目录images文件下png图片文件");
                    return;
                }
                DirectoryInfo dir = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                FileInfo[] files = dir.GetFiles("*.png");
                DirectoryInfo[] dii = dir.GetDirectories();
                int imageID = 0;
                imageInfoList = new List<ImageInfoClass>();
                foreach (var file in files)
                {

                    System.IO.FileStream fs = new System.IO.FileStream(file.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    fs.Close();
                    fs.Dispose();
                    System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    ms.Dispose();


                    
                    //System.Diagnostics.Debug.WriteLine("名:"+ file.FullName + "宽："+ bitmapImage.PixelHeight+"高："+ bitmapImage.PixelWidth);
                    ImageInfoClass imageInfoClass = new ImageInfoClass();
                    imageInfoClass.ImageID = imageID;
                    imageInfoClass.ImageName = file.FullName.Replace(string.Format(@"{0}\images", System.Windows.Forms.Application.StartupPath), "");
                    imageInfoClass.ImageShortName = file.Name.Replace(".png", "");

                    int ImagePlaceWidth = bitmapImage.PixelWidth;
                    int ImagePlaceHeight = bitmapImage.PixelHeight;
                    /*
                    if (ImagePlaceHeight > 300)
                    {
                        ImagePlaceWidth = Convert.ToInt32(0.6 * ImagePlaceWidth);
                        ImagePlaceHeight = Convert.ToInt32(0.6 * ImagePlaceHeight);
                    }
                    */
                    imageInfoClass.ImageWidth = ImagePlaceWidth;
                    imageInfoClass.ImageHeight = ImagePlaceHeight;

                    imageInfoClass.ImageLeft = 0;
                    imageInfoClass.ImageTop = 0;
                    imageInfoClass.ImageUse = false;
                    imageInfoClass.ImageUseColumn = 0;
                    imageInfoList.Add(imageInfoClass);
                    imageID += 1;

                   

                    
                }
                //System.Diagnostics.Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(imageInfoList));
                int placecolumn =1;//放置在哪列
                int placeleft = 0;//放置水平
                int placetop = 1;//放置垂直
                while (true)
                {
                    var listTmp = imageInfoList.Where(t => t.ImageUse == false).OrderByDescending(o => o.ImageWidth).ToList();//降序
                    
                    //System.Diagnostics.Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(listTmp));
                    if (listTmp == null || listTmp.Count==0)
                    {
                        break;
                    }
                    //放置图形 判断 当前宽度最大的一个
                    ImageInfoClass listTmpImage = listTmp[0];

                    var currentColumnList = imageInfoList.Where(t => t.ImageUseColumn == placecolumn).OrderByDescending(o => o.ImageWidth).ToList();//降序
                    if (currentColumnList.Count > 0)  //当前列 没有的话 就是0  有的话 就把所有元素的高度相加
                    {
                        int columnHeight = 0;
                        foreach (var currentColumnListone in currentColumnList)
                        {
                             columnHeight = columnHeight + currentColumnListone.ImageHeight+4;
                        }
                        placetop = columnHeight;
                    }
                    if (placetop > 720)
                    {
                        placeleft += currentColumnList[0].ImageWidth + 10;
                        placetop = 1;
                        placecolumn += 1;

                    }

                    //放置图形
                    Object content = XamlReader.Load(XmlReader.Create(string.Format(@"{0}\path.xml", System.Windows.Forms.Application.StartupPath)));
                    DesignerItem newItem = new DesignerItem();
                    newItem.Content = content;
                    newItem.ItemContent.Append(XamlWriter.Save(newItem.Content));


                    newItem.Width = listTmpImage.ImageWidth;
                    newItem.Height = listTmpImage.ImageHeight;
                        //Thread.Sleep(1);
                        newItem.MuKuaiName = "#" + listTmpImage.ImageShortName;
                        newItem.BKImgSource = listTmpImage.ImageName;
                        newItem.ConnectorVisble = true;

                       string imageFileName = string.Format(@"{0}\images{1}", System.Windows.Forms.Application.StartupPath, newItem.BKImgSource);
                        System.IO.FileStream fs = new System.IO.FileStream(imageFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        fs.Close();
                        fs.Dispose();
                        System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = ms;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        ms.Dispose();
                        ImageBrush imagebrush = new ImageBrush();
                        imagebrush.ImageSource = bitmapImage;
                        newItem.Background = imagebrush;

                    DesignerCanvas.SetLeft(newItem, placeleft);
                    DesignerCanvas.SetTop(newItem, placetop);
                    this.Children.Add(newItem);
                    SetConnectorDecoratorTemplate(newItem);
                    this.SelectionService.SelectItem(newItem);
                    newItem.Focus();

                    //将原来集合的数据更新 已经放置在界面上了
                    ImageInfoClass imageInfoClass =imageInfoList.Where(t=> t.ImageID== listTmpImage.ImageID).FirstOrDefault();//降序
                    if (imageInfoClass != null)
                    {
                        imageInfoClass.ImageUse = true;
                        imageInfoClass.ImageUseColumn = placecolumn;
                        imageInfoClass.ImageLeft = placeleft;
                        imageInfoClass.ImageTop = placetop;
                    }

                }

                foreach (var item in imageInfoList)
                {
                    //放置名称
                    DesignerItem newItemText = new DesignerItem();
                    Thread.Sleep(2);
                    newItemText.MuKuaiName = "$#%" + GetTimeStamp();
                    newItemText.ConnectorVisble = true;
                    TextBlock tx = new TextBlock();

                    tx.IsHitTestVisible = false;



                    tx.Text = "#" + item.ImageShortName;
                    tx.TextWrapping = TextWrapping.Wrap;
                    tx.VerticalAlignment = VerticalAlignment.Stretch;
                    tx.HorizontalAlignment = HorizontalAlignment.Stretch;
                    tx.FontSize = 16;
                        //tx.FontWeight = tx.FontWeight = FontWeights.Bold;
                        tx.Foreground = Brushes.White;
                        tx.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)); //252,3,18 222, 237, 249

                        tx.Width = 216;
                        tx.Height = 120;
                      
                    newItemText.Content = tx;
                        newItemText.ItemContent.Append(XamlWriter.Save(newItemText.Content));
                        newItemText.Width = 38;
                    newItemText.Height = 18;


                    DesignerCanvas.SetLeft(newItemText, item.ImageLeft);
                    DesignerCanvas.SetTop(newItemText, item.ImageTop);
                    Canvas.SetZIndex(newItemText, this.Children.Count);
                    this.Children.Add(newItemText);

                    SetConnectorDecoratorTemplate(newItemText);

                    newItemText.ApplyTemplate();
                    //update selection
                    this.SelectionService.SelectItem(newItemText);
                    newItemText.Focus();

                }
            }

            }
            else//如果点击“取消”按钮
            {



            }


        }



        #endregion

        #region Open Command

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
             try 
            {
                
                if (this.Tag.ToString() == "design")
                {
                    Window1 w1 = GetWindow1(this);
                    w1.showWaitIco();
                }
                if (this.Tag.ToString() == "test")
                {
                    //TestWindow1  t1 = GetTestWindow1(this);
                    //t1.showWaitIco();
                }

                XElement root = LoadSerializedDataFromFile();

            if (root == null)
                return;

            this.SelectionService.ClearSelection();
            this.Children.Clear();
           

            IEnumerable<XElement> itemsXML = root.Elements("DesignerItems").Elements("DesignerItem");
            foreach (XElement itemXML in itemsXML)
            {
                Guid id = new Guid(itemXML.Element("ID").Value);
                DesignerItem item = DeserializeDesignerItem(itemXML, id, 0, 0);

                this.Children.Add(item);

                SetConnectorDecoratorTemplate(item);
            }

            this.InvalidateVisual();

            IEnumerable<XElement> connectionsXML = root.Elements("Connections").Elements("Connection");
            foreach (XElement connectionXML in connectionsXML)
            {
                Guid sourceID = new Guid(connectionXML.Element("SourceID").Value);
                Guid sinkID = new Guid(connectionXML.Element("SinkID").Value);

                String sourceConnectorName = connectionXML.Element("SourceConnectorName").Value;
                String sinkConnectorName = connectionXML.Element("SinkConnectorName").Value;

                Connector sourceConnector = GetConnector(sourceID, sourceConnectorName);
                Connector sinkConnector = GetConnector(sinkID, sinkConnectorName);

                Connection connection = new Connection(sourceConnector, sinkConnector);
                connection.LineName = connectionXML.Element("LineName").Value;
                connection.RDType = connectionXML.Element("RDType").Value;
                connection.MutilLineCount = connectionXML.Element("MutilLineCount").Value;
                connection.RDValue = connectionXML.Element("RDValue").Value;
                connection.RDDirection = connectionXML.Element("RDDirection").Value;
                connection.LeftPoints = connectionXML.Element("LeftPoints").Value;
                connection.RightPoints = connectionXML.Element("RightPoints").Value;
                connection.StartPoint = connectionXML.Element("start").Value;
                connection.EndPoint = connectionXML.Element("end").Value;
               
              
              
                this.Children.Add(connection);
                Canvas.SetZIndex(connection, Int32.Parse(connectionXML.Element("zIndex").Value));
                //this.RegisterName("L" + connection.Name, connection);

            }

                if (this.Tag.ToString() == "design")
                {
                    Window1 w1 = GetWindow1(this);
                    w1.hideWaitIco(fmodelInsert);
                }
                if (this.Tag.ToString() == "test")
                {
                    //TestWindow1 t1 = GetTestWindow1(this);
                   // t1.hideWaitIco();
                }



            }
             catch (Exception ex)
             {
                 MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                 logNet.WriteDebug("打开文件 error", ex.Message);
                 logNet.WriteDebug("----------");
             }
           
        }
        //打开文件
       public void OpenFile()
        {
            try 
            {
                
                if (this.Tag.ToString() == "design")
                {
                    Window1 w1 = GetWindow1(this);
                    w1.showWaitIco();
                }
                if (this.Tag.ToString() == "test")
                {
                    //TestWindow1 t1 = GetTestWindow1(this);
                    //t1.showWaitIco();
                }
               
                XElement root = LoadSerializedDataFromFile();

            if (root == null)
                return;

            this.SelectionService.ClearSelection();
            this.Children.Clear();
           

            IEnumerable<XElement> itemsXML = root.Elements("DesignerItems").Elements("DesignerItem");
            foreach (XElement itemXML in itemsXML)
            {
                Guid id = new Guid(itemXML.Element("ID").Value);
                DesignerItem item = DeserializeDesignerItem(itemXML, id, 0, 0);

                this.Children.Add(item);

                SetConnectorDecoratorTemplate(item);
            }

            this.InvalidateVisual();

            IEnumerable<XElement> connectionsXML = root.Elements("Connections").Elements("Connection");
            foreach (XElement connectionXML in connectionsXML)
            {
                Guid sourceID = new Guid(connectionXML.Element("SourceID").Value);
                Guid sinkID = new Guid(connectionXML.Element("SinkID").Value);

                String sourceConnectorName = connectionXML.Element("SourceConnectorName").Value;
                String sinkConnectorName = connectionXML.Element("SinkConnectorName").Value;

                Connector sourceConnector = GetConnector(sourceID, sourceConnectorName);
                Connector sinkConnector = GetConnector(sinkID, sinkConnectorName);

                Connection connection = new Connection(sourceConnector, sinkConnector);
                connection.LineName = connectionXML.Element("LineName").Value;
                connection.RDType = connectionXML.Element("RDType").Value;
                connection.MutilLineCount = connectionXML.Element("MutilLineCount").Value;
                connection.RDValue = connectionXML.Element("RDValue").Value;

                connection.RDDirection = connectionXML.Element("RDDirection").Value;
                connection.LeftPoints = connectionXML.Element("LeftPoints").Value;
                connection.RightPoints = connectionXML.Element("RightPoints").Value;
      
                connection.StartPoint = connectionXML.Element("start").Value;
                connection.EndPoint = connectionXML.Element("end").Value;
                
               
                this.Children.Add(connection);
                Canvas.SetZIndex(connection, Int32.Parse(connectionXML.Element("zIndex").Value));
                
               
               
                
               
            }

                if (this.Tag.ToString() == "design")
                {
                    Window1 w1 = GetWindow1(this);
                    w1.hideWaitIco(fmodelInsert);
                }
                if (this.Tag.ToString() == "test")
                {
                    //TestWindow1 t1 = GetTestWindow1(this);
                    //t1.hideWaitIco();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                logNet.WriteDebug("打开文件 error", ex.Message);
                logNet.WriteDebug("----------");
            }
           
        }
       public void OpenFile2(string fileName)
       {
           try
           {
                if (this.Tag.ToString() == "design")
                {
                    Window1 w1 = GetWindow1(this);
                    w1.showWaitIco();
                }
                if (this.Tag.ToString() == "test")
                {
                    //TestWindow1 t1 = GetTestWindow1(this);
                    //t1.showWaitIco();
                }
                XElement root = XElement.Load(fileName);

               if (root == null)
                   return;

               this.SelectionService.ClearSelection();
               this.Children.Clear();


               IEnumerable<XElement> itemsXML = root.Elements("DesignerItems").Elements("DesignerItem");
               foreach (XElement itemXML in itemsXML)
               {
                   Guid id = new Guid(itemXML.Element("ID").Value);
                   DesignerItem item = DeserializeDesignerItem(itemXML, id, 0, 0);

                   this.Children.Add(item);

                   SetConnectorDecoratorTemplate(item);
               }

               this.InvalidateVisual();

               IEnumerable<XElement> connectionsXML = root.Elements("Connections").Elements("Connection");
               foreach (XElement connectionXML in connectionsXML)
               {
                   Guid sourceID = new Guid(connectionXML.Element("SourceID").Value);
                   Guid sinkID = new Guid(connectionXML.Element("SinkID").Value);

                   String sourceConnectorName = connectionXML.Element("SourceConnectorName").Value;
                   String sinkConnectorName = connectionXML.Element("SinkConnectorName").Value;

                   Connector sourceConnector = GetConnector(sourceID, sourceConnectorName);
                   Connector sinkConnector = GetConnector(sinkID, sinkConnectorName);

                   Connection connection = new Connection(sourceConnector, sinkConnector);
                   connection.LineName = connectionXML.Element("LineName").Value;
                   connection.RDType = connectionXML.Element("RDType").Value;
                   connection.MutilLineCount = connectionXML.Element("MutilLineCount").Value;
                   connection.RDValue = connectionXML.Element("RDValue").Value;

                   connection.RDDirection = connectionXML.Element("RDDirection").Value;
                   connection.LeftPoints = connectionXML.Element("LeftPoints").Value;
                   connection.RightPoints = connectionXML.Element("RightPoints").Value;

                   connection.StartPoint = connectionXML.Element("start").Value;
                   connection.EndPoint = connectionXML.Element("end").Value;


                   this.Children.Add(connection);
                   Canvas.SetZIndex(connection, Int32.Parse(connectionXML.Element("zIndex").Value));





               }
                if (this.Tag.ToString() == "design")
                {
                    Window1 w1 = GetWindow1(this);
                    w1.hideWaitIco(fmodelInsert);
                }
                if (this.Tag.ToString() == "test")
                {
                    //TestWindow1 t1 = GetTestWindow1(this);
                   // t1.hideWaitIco();
                }
            }
           catch (Exception ex)
           {
               MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
               logNet.WriteDebug("打开文件 error", ex.Message);
               logNet.WriteDebug("----------");
           }

       }
        #endregion
        /// <summary>  
        /// 获得指定元素的所有子元素  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
        //public static List<T> GetChildObjects<T>(DependencyObject obj) where T : FrameworkElement
        //{
        //    DependencyObject child = null;
        //    List<T> childList = new List<T>();

        //    for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
        //    {
        //        child = VisualTreeHelper.GetChild(obj, i);

        //        if (child is T)
        //        {
        //            childList.Add((T)child);
        //        }
        //        childList.AddRange(GetChildObjects<T>(child));
        //    }
        //    return childList;
        //}
        #region Save Command

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.Tag.ToString() == "design")
            {
                Window1 w1 = GetWindow1(this);
                w1.showWaitIco();
            }
            if (this.Tag.ToString() == "test")
            {
                //TestWindow1 t1 = GetTestWindow1(this);
               // t1.showWaitIco();
            }

            IEnumerable<DesignerItem> designerItems = this.Children.OfType<DesignerItem>();
            IEnumerable<Connection> connections = this.Children.OfType<Connection>();

            XElement designerItemsXML = SerializeDesignerItems(designerItems);
            XElement connectionsXML = SerializeConnections(connections);

            XElement root = new XElement("Root");
            root.Add(designerItemsXML);
            root.Add(connectionsXML);

            SaveFile(root);

            if (this.Tag.ToString() == "design")
            {
                Window1 w1 = GetWindow1(this);
                w1.hideWaitIco(fmodelInsert);
            }
            if (this.Tag.ToString() == "test")
            {
                //TestWindow1 t1 = GetTestWindow1(this);
                //t1.hideWaitIco();
            }

        }

        #endregion

        #region Print Command

        private void Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectionService.ClearSelection();

            PrintDialog printDialog = new PrintDialog();

            if (true == printDialog.ShowDialog())
            {
                printDialog.PrintVisual(this, "Diagram");
            }


            //List<System.Windows.Shapes.Path> mis = GetChildObjects<System.Windows.Shapes.Path>(selectedConnections[0]);
            //ImageBrush imagebrush = new ImageBrush();
            //imagebrush.ImageSource = new BitmapImage(new Uri(@"D:\vsproject\图形界面开发\新建文件夹\FD.png", UriKind.Absolute));
            //mis[0].Fill = imagebrush;

        }

        #endregion

        private void Chexiao_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Chexiao_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //撤销  加载 进来  delCount
            try
            {
                string s = string.Format(@"{0}\backup\{1}.xml", System.Windows.Forms.Application.StartupPath, delCount.ToString());
                XElement root = XElement.Load(s);

                if (root == null)
                    return;

                IEnumerable<XElement> itemsXML = root.Elements("DesignerItems").Elements("DesignerItem");
                foreach (XElement itemXML in itemsXML)
                {
                    Guid id = new Guid(itemXML.Element("ID").Value);
                   
                    //
                    DesignerItem item = new DesignerItem(id);
                    item.Width = Double.Parse(itemXML.Element("Width").Value, CultureInfo.InvariantCulture);
                    item.Height = Double.Parse(itemXML.Element("Height").Value, CultureInfo.InvariantCulture);
                    item.ParentID = new Guid(itemXML.Element("ParentID").Value);
                    item.IsGroup = Boolean.Parse(itemXML.Element("IsGroup").Value);
                    Canvas.SetLeft(item, Double.Parse(itemXML.Element("Left").Value, CultureInfo.InvariantCulture));
                    Canvas.SetTop(item, Double.Parse(itemXML.Element("Top").Value, CultureInfo.InvariantCulture) );
                    Canvas.SetZIndex(item, Int32.Parse(itemXML.Element("zIndex").Value));
                    Object content = XamlReader.Load(XmlReader.Create(new StringReader(itemXML.Element("Content").Value)));
                    item.Content = content;
                    item.ItemContent.Append(itemXML.Element("Content").Value);
                    item.BKImgSource = Convert.ToString(itemXML.Element("BK").Value);
                    string imageFileName = string.Format(@"{0}\images{1}", System.Windows.Forms.Application.StartupPath, item.BKImgSource);
                    if (item.BKImgSource != string.Empty && File.Exists(imageFileName))
                    {




                        System.IO.FileStream fs = new System.IO.FileStream(imageFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        fs.Close();
                        fs.Dispose();
                        System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = ms;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        ms.Dispose();
                        ImageBrush imagebrush = new ImageBrush();
                        imagebrush.ImageSource = bitmapImage;
                        item.Background = imagebrush;




                    }
                    item.MuKuaiName = Convert.ToString(itemXML.Element("MKN").Value);


                    item.ZuoBiaoName = Convert.ToString(itemXML.Element("ZBN").Value);
                     var  itemsa = this.Children.OfType<DesignerItem>().Where(ss =>   ss.ZuoBiaoName== item.ZuoBiaoName).ToList();
                    if(itemsa!=null   && itemsa.Count() > 0)
                    {
                        item.ZuoBiaoName = "";
                    }


                    item.ConnectorVisble = Convert.ToBoolean(itemXML.Element("ConnectorVisble").Value);
                    //
                   

                    this.Children.Add(item);

                    SetConnectorDecoratorTemplate(item);
                }

                this.InvalidateVisual();

              if  ( delCount > 0){ delCount = delCount - 1; }
            }
            catch (Exception ex)
            {
                MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                logNet.WriteDebug("打开文件 error", ex.Message);
                logNet.WriteDebug("----------");
            }





        }

            #region Copy Command

            private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CopyCurrentSelection();
        }

        private void Copy_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.CurrentSelection.Count() > 0;
        }

        #endregion

        #region Paste Command

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
            XElement root = LoadSerializedDataFromClipBoard();

            if (root == null)
                return;

            // create DesignerItems
            Dictionary<Guid, Guid> mappingOldToNewIDs = new Dictionary<Guid, Guid>();
            List<ISelectable> newItems = new List<ISelectable>();
            IEnumerable<XElement> itemsXML = root.Elements("DesignerItems").Elements("DesignerItem");

            double offsetX = Double.Parse(root.Attribute("OffsetX").Value, CultureInfo.InvariantCulture);
            double offsetY = Double.Parse(root.Attribute("OffsetY").Value, CultureInfo.InvariantCulture);

            foreach (XElement itemXML in itemsXML)
            {
                Guid oldID = new Guid(itemXML.Element("ID").Value);
                Guid newID = Guid.NewGuid();
                mappingOldToNewIDs.Add(oldID, newID);
                DesignerItem item = DeserializeDesignerItem(itemXML, newID, offsetX, offsetY);
                item.ZuoBiaoName = "";
                item.MuKuaiName = "";
                if (item.Content.ToString().Contains("System.Windows.Shapes.Ellipse"))
                {
                    item.ConnectorVisble = false;
                }
                else
                {
                    item.ConnectorVisble = true;
                }
                
               
                this.Children.Add(item);
                SetConnectorDecoratorTemplate(item);
                newItems.Add(item);
            }

            // update group hierarchy
            SelectionService.ClearSelection();
            foreach (DesignerItem el in newItems)
            {
                if (el.ParentID != Guid.Empty)
                    el.ParentID = mappingOldToNewIDs[el.ParentID];
            }


            foreach (DesignerItem item in newItems)
            {
                if (item.ParentID == Guid.Empty)
                {
                    SelectionService.AddToSelection(item);
                }
            }
/*
            // create Connections
            IEnumerable<XElement> connectionsXML = root.Elements("Connections").Elements("Connection");
            foreach (XElement connectionXML in connectionsXML)
            {
                Guid oldSourceID = new Guid(connectionXML.Element("SourceID").Value);
                Guid oldSinkID = new Guid(connectionXML.Element("SinkID").Value);

                if (mappingOldToNewIDs.ContainsKey(oldSourceID) && mappingOldToNewIDs.ContainsKey(oldSinkID))
                {
                    Guid newSourceID = mappingOldToNewIDs[oldSourceID];
                    Guid newSinkID = mappingOldToNewIDs[oldSinkID];

                    String sourceConnectorName = connectionXML.Element("SourceConnectorName").Value;
                    String sinkConnectorName = connectionXML.Element("SinkConnectorName").Value;

                    Connector sourceConnector = GetConnector(newSourceID, sourceConnectorName);
                    Connector sinkConnector = GetConnector(newSinkID, sinkConnectorName);

                    Connection connection = new Connection(sourceConnector, sinkConnector);
                    connection.StartPoint = connectionXML.Element("start").Value;
                    connection.EndPoint = connectionXML.Element("end").Value;
                    Canvas.SetZIndex(connection, Int32.Parse(connectionXML.Element("zIndex").Value));
                    this.Children.Add(connection);

                    SelectionService.AddToSelection(connection);
                }
            }
*/
            DesignerCanvas.BringToFront.Execute(null, this);

            // update paste offset
            root.Attribute("OffsetX").Value = (offsetX + 10).ToString();
            root.Attribute("OffsetY").Value = (offsetY + 10).ToString();
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
        }

        private void Paste_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsData(DataFormats.Xaml);
        }

        #endregion

        #region Delete Command

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
          
            System.Windows.Forms.MessageBoxButtons messButton = System.Windows.Forms.MessageBoxButtons.OKCancel;
            System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show("确定要删除", "删除", messButton);

            if (dr == System.Windows.Forms.DialogResult.OK)//如果点击“确定”按钮

            {
                DeleteCurrentSelection();

            }
            else//如果点击“取消”按钮
            {



            }
        }

        private void Delete_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SelectionService.CurrentSelection.Count() > 0;
        }

        #endregion

        #region Cut Command

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CopyCurrentSelection();
            DeleteCurrentSelection();
        }

        private void Cut_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SelectionService.CurrentSelection.Count() > 0;
        }

        #endregion

        #region Group Command

        private void Group_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var items = from item in this.SelectionService.CurrentSelection.OfType<DesignerItem>()
                        where item.ParentID == Guid.Empty && !item.Content.ToString().Contains("System.Windows.Controls.TextBlock")
                        select item;

            Rect rect = GetBoundingRectangle(items);

            DesignerItem groupItem = new DesignerItem();
            groupItem.IsGroup = true;
            groupItem.Width = rect.Width;
            groupItem.Height = rect.Height;
            groupItem.MuKuaiName = "$#%" + GetTimeStamp();
            groupItem.ConnectorVisble = true;
            Canvas.SetLeft(groupItem, rect.Left);
            Canvas.SetTop(groupItem, rect.Top);
            Canvas groupCanvas = new Canvas();
            groupItem.Content = groupCanvas;
            groupItem.ItemContent.Append(XamlWriter.Save(groupItem.Content));
            Canvas.SetZIndex(groupItem, this.Children.Count);
            this.Children.Add(groupItem);

            foreach (DesignerItem item in items)
                item.ParentID = groupItem.ID;

            this.SelectionService.SelectItem(groupItem);
        }

        private void Group_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            int count = (from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                         where item.ParentID == Guid.Empty
                         select item).Count();

            e.CanExecute = count > 1;
        }

        #endregion

        #region Ungroup Command

        private void Ungroup_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var groups = (from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                          where item.IsGroup && item.ParentID == Guid.Empty
                          select item).ToArray();

            foreach (DesignerItem groupRoot in groups)
            {
                var children = from child in SelectionService.CurrentSelection.OfType<DesignerItem>()
                               where child.ParentID == groupRoot.ID
                               select child;

                foreach (DesignerItem child in children)
                    child.ParentID = Guid.Empty;

                this.SelectionService.RemoveFromSelection(groupRoot);
                this.Children.Remove(groupRoot);
                UpdateZIndex();
            }
        }

        private void Ungroup_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            var groupedItem = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                              where item.ParentID != Guid.Empty
                              select item;


            e.CanExecute = groupedItem.Count() > 0;
        }

        #endregion

        #region BringForward Command

        private void BringForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> ordered = (from item in SelectionService.CurrentSelection
                                       orderby Canvas.GetZIndex(item as UIElement) descending
                                       select item as UIElement).ToList();

            int count = this.Children.Count;

            for (int i = 0; i < ordered.Count; i++)
            {
                int currentIndex = Canvas.GetZIndex(ordered[i]);
                int newIndex = Math.Min(count - 1 - i, currentIndex + 1);
                if (currentIndex != newIndex)
                {
                    Canvas.SetZIndex(ordered[i], newIndex);
                    IEnumerable<UIElement> it = this.Children.OfType<UIElement>().Where(item => Canvas.GetZIndex(item) == newIndex);

                    foreach (UIElement elm in it)
                    {
                        if (elm != ordered[i])
                        {
                            Canvas.SetZIndex(elm, currentIndex);
                            break;
                        }
                    }
                }
            }
        }

        private void Order_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = SelectionService.CurrentSelection.Count() > 0;
            e.CanExecute = true;
        }

        #endregion

        #region BringToFront Command

        private void BringToFront_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> selectionSorted = (from item in SelectionService.CurrentSelection
                                               orderby Canvas.GetZIndex(item as UIElement) ascending
                                               select item as UIElement).ToList();

            List<UIElement> childrenSorted = (from UIElement item in this.Children
                                              orderby Canvas.GetZIndex(item as UIElement) ascending
                                              select item as UIElement).ToList();

            int i = 0;
            int j = 0;
            foreach (UIElement item in childrenSorted)
            {
                if (selectionSorted.Contains(item))
                {
                    int idx = Canvas.GetZIndex(item);
                    Canvas.SetZIndex(item, childrenSorted.Count - selectionSorted.Count + j++);
                }
                else
                {
                    Canvas.SetZIndex(item, i++);
                }
            }
        }

        #endregion

        #region SendBackward Command

        private void SendBackward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> ordered = (from item in SelectionService.CurrentSelection
                                       orderby Canvas.GetZIndex(item as UIElement) ascending
                                       select item as UIElement).ToList();

            int count = this.Children.Count;

            for (int i = 0; i < ordered.Count; i++)
            {
                int currentIndex = Canvas.GetZIndex(ordered[i]);
                int newIndex = Math.Max(i, currentIndex - 1);
                if (currentIndex != newIndex)
                {
                    Canvas.SetZIndex(ordered[i], newIndex);
                    IEnumerable<UIElement> it = this.Children.OfType<UIElement>().Where(item => Canvas.GetZIndex(item) == newIndex);

                    foreach (UIElement elm in it)
                    {
                        if (elm != ordered[i])
                        {
                            Canvas.SetZIndex(elm, currentIndex);
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region SendToBack Command

        private void SendToBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<UIElement> selectionSorted = (from item in SelectionService.CurrentSelection
                                               orderby Canvas.GetZIndex(item as UIElement) ascending
                                               select item as UIElement).ToList();

            List<UIElement> childrenSorted = (from UIElement item in this.Children
                                              orderby Canvas.GetZIndex(item as UIElement) ascending
                                              select item as UIElement).ToList();
            int i = 0;
            int j = 0;
            foreach (UIElement item in childrenSorted)
            {
                if (selectionSorted.Contains(item))
                {
                    int idx = Canvas.GetZIndex(item);
                    Canvas.SetZIndex(item, j++);

                }
                else
                {
                    Canvas.SetZIndex(item, selectionSorted.Count + i++);
                }
            }
        }

        #endregion

        #region AlignTop Command

        private void AlignTop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double top = Canvas.GetTop(selectedItems.First());

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = top - Canvas.GetTop(item);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                    }
                }
            }
        }

        private void Align_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //var groupedItem = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
            //                  where item.ParentID == Guid.Empty
            //                  select item;


            //e.CanExecute = groupedItem.Count() > 1;
            e.CanExecute = true;
        }

        #endregion
        //水平扩大
        private void ShuipingKuoda_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
           
            e.CanExecute = true;
        }
        private void ShuipingKuoda_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItemEclips = SelectionService.CurrentSelection.OfType<DesignerItem>().Where(s => s.ParentID == Guid.Empty && s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")   ).ToList();
            var selectedItems = selectedItemEclips.OrderBy(o => Canvas.GetLeft(o)).ToList();//降序

            if (selectedItems.Count() > 1)
            {
                //double bottom = Canvas.GetTop(selectedItems.First()) + selectedItems.First().Height / 2;

                foreach (DesignerItem item in selectedItems)
                {
                   
                }
                for(int i=1;i< selectedItems.Count; i++)
                {
                    //Canvas.SetLeft(selectedItems[i], Canvas.GetLeft(selectedItems[i]) + 1*i);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(selectedItems[i]))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + 1*i);
                    }
                }



            }
        }
        //水平缩小
        private void ShuipingSuoxiao_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = true;
        }
        private void ShuipingSuoxiao_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItemEclips = SelectionService.CurrentSelection.OfType<DesignerItem>().Where(s => s.ParentID == Guid.Empty && s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();
            var selectedItems = selectedItemEclips.OrderBy(o => Canvas.GetLeft(o)).ToList();//降序

            if (selectedItems.Count() > 1)
            {
                //double bottom = Canvas.GetTop(selectedItems.First()) + selectedItems.First().Height / 2;

                foreach (DesignerItem item in selectedItems)
                {

                }
                for (int i = 1; i < selectedItems.Count; i++)
                {
                    //Canvas.SetLeft(selectedItems[i], Canvas.GetLeft(selectedItems[i]) + 1*i);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(selectedItems[i]))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) - 1 * i);
                    }
                }



            }
        }

        //竖直
        //水平扩大
        private void ShuzhiKuoda_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = true;
        }
        private void ShuzhiKuoda_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItemEclips = SelectionService.CurrentSelection.OfType<DesignerItem>().Where(s => s.ParentID == Guid.Empty && s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();
            var selectedItems = selectedItemEclips.OrderBy(o => Canvas.GetLeft(o)).ToList();//降序

            if (selectedItems.Count() > 1)
            {
                //double bottom = Canvas.GetTop(selectedItems.First()) + selectedItems.First().Height / 2;

                foreach (DesignerItem item in selectedItems)
                {

                }
                for (int i = 1; i < selectedItems.Count; i++)
                {
                    //Canvas.SetLeft(selectedItems[i], Canvas.GetLeft(selectedItems[i]) + 1*i);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(selectedItems[i]))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + 1 * i);
                    }
                }



            }
        }
        //水平缩小
        private void ShuzhiSuoxiao_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {

            e.CanExecute = true;
        }
        private void ShuzhiSuoxiao_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItemEclips = SelectionService.CurrentSelection.OfType<DesignerItem>().Where(s => s.ParentID == Guid.Empty && s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();
            var selectedItems = selectedItemEclips.OrderBy(o => Canvas.GetLeft(o)).ToList();//降序

            if (selectedItems.Count() > 1)
            {
                //double bottom = Canvas.GetTop(selectedItems.First()) + selectedItems.First().Height / 2;

                foreach (DesignerItem item in selectedItems)
                {

                }
                for (int i = 1; i < selectedItems.Count; i++)
                {
                    //Canvas.SetLeft(selectedItems[i], Canvas.GetLeft(selectedItems[i]) + 1*i);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(selectedItems[i]))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) - 1 * i);
                    }
                }



            }
        }


        #region AlignVerticalCenters Command
        //以中心数值对齐
        private void AlignVerticalCenters_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double bottom = Canvas.GetTop(selectedItems.First()) + selectedItems.First().Height / 2;

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = bottom - (Canvas.GetTop(item) + item.Height / 2);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region AlignBottom Command

        private void AlignBottom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double bottom = Canvas.GetTop(selectedItems.First()) + selectedItems.First().Height;

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = bottom - (Canvas.GetTop(item) + item.Height);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region AlignLeft Command

        private void AlignLeft_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double left = Canvas.GetLeft(selectedItems.First());

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = left - Canvas.GetLeft(item);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
                    }
                }
            }
        }

        #endregion


        #region AlignHorizontalCenters Command  
        //以水平中心对齐
        private void AlignHorizontalCenters_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double center = Canvas.GetLeft(selectedItems.First()) + selectedItems.First().Width / 2;

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = center - (Canvas.GetLeft(item) + item.Width / 2);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region AlignRight Command

        private void AlignRight_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                select item;

            if (selectedItems.Count() > 1)
            {
                double right = Canvas.GetLeft(selectedItems.First()) + selectedItems.First().Width;

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = right - (Canvas.GetLeft(item) + item.Width);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
                    }
                }
            }
        }

        #endregion

        #region DistributeHorizontal Command

        private void DistributeHorizontal_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                let itemLeft = Canvas.GetLeft(item)
                                orderby itemLeft
                                select item;

            if (selectedItems.Count() > 1)
            {
                double left = Double.MaxValue;
                double right = Double.MinValue;
                double sumWidth = 0;
                foreach (DesignerItem item in selectedItems)
                {
                    left = Math.Min(left, Canvas.GetLeft(item));
                    right = Math.Max(right, Canvas.GetLeft(item) + item.Width);
                    sumWidth += item.Width;
                }

                double distance = Math.Max(0, (right - left - sumWidth) / (selectedItems.Count() - 1));
                double offset = Canvas.GetLeft(selectedItems.First());

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = offset - Canvas.GetLeft(item);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetLeft(di, Canvas.GetLeft(di) + delta);
                    }
                    offset = offset + item.Width + distance;
                }
            }
        }

        private void Distribute_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            //var groupedItem = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
            //                  where item.ParentID == Guid.Empty
            //                  select item;


            //e.CanExecute = groupedItem.Count() > 1;
            e.CanExecute = true;
        }

        #endregion

        #region DistributeVertical Command

        private void DistributeVertical_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedItems = from item in SelectionService.CurrentSelection.OfType<DesignerItem>()
                                where item.ParentID == Guid.Empty
                                let itemTop = Canvas.GetTop(item)
                                orderby itemTop
                                select item;

            if (selectedItems.Count() > 1)
            {
                double top = Double.MaxValue;
                double bottom = Double.MinValue;
                double sumHeight = 0;
                foreach (DesignerItem item in selectedItems)
                {
                    top = Math.Min(top, Canvas.GetTop(item));
                    bottom = Math.Max(bottom, Canvas.GetTop(item) + item.Height);
                    sumHeight += item.Height;
                }

                double distance = Math.Max(0, (bottom - top - sumHeight) / (selectedItems.Count() - 1));
                double offset = Canvas.GetTop(selectedItems.First());

                foreach (DesignerItem item in selectedItems)
                {
                    double delta = offset - Canvas.GetTop(item);
                    foreach (DesignerItem di in SelectionService.GetGroupMembers(item))
                    {
                        Canvas.SetTop(di, Canvas.GetTop(di) + delta);
                    }
                    offset = offset + item.Height + distance;
                }
            }
        }

        #endregion

        #region SelectAll Command

        private void SelectAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectionService.SelectAll();
        }

        #endregion

        #region Helper Methods

        private XElement LoadSerializedDataFromFile()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();

            openFileDialog.InitialDirectory = string.Format(@"{0}", System.Windows.Forms.Application.StartupPath);     //打开对话框后的初始目录
            openFileDialog.Filter = "Designer Files (*.xml)|*.xml|All Files (*.*)|*.*";
            openFileDialog.RestoreDirectory = false;    //若为false，则打开对话框后为上次的目录。若为true，则为初始目录
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                try
                {
                    string str = openFileDialog.FileName.ToString().Split('\\')[openFileDialog.FileName.ToString().Split('\\').Length - 1];
                    this.fmodelInsert = str.Substring(0, str.IndexOf(".xml"));
                    //MessageBox.Show(this.fmodelInsert);
                    return XElement.Load(openFileDialog.FileName);

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


           

            return null;
        }

        void SaveFile(XElement xElement)
        {
           System.Windows.Forms. SaveFileDialog saveFile = new System.Windows.Forms.SaveFileDialog();
            saveFile.InitialDirectory = string.Format(@"{0}", System.Windows.Forms.Application.StartupPath);     //打开对话框后的初始目录
            saveFile.Filter = "Designer Files (*.xml)|*.xml|All Files (*.*)|*.*";
            saveFile.RestoreDirectory = false;    //若为false，则打开对话框后为上次的目录。若为true，则为初始目录
            if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    xElement.Save(saveFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            
        }

        private XElement LoadSerializedDataFromClipBoard()
        {
            if (Clipboard.ContainsData(DataFormats.Xaml))
            {
                String clipboardData = Clipboard.GetData(DataFormats.Xaml) as String;

                if (String.IsNullOrEmpty(clipboardData))
                    return null;
                try
                {
                    return XElement.Load(new StringReader(clipboardData));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return null;
        }

        private XElement SerializeDesignerItems(IEnumerable<DesignerItem> designerItems)
        {
            XElement serializedItems = new XElement("DesignerItems",
                                       from item in designerItems
                                           let contentXaml = XamlWriter.Save(((DesignerItem)item).Content)
                                      
                                       select new XElement("DesignerItem",
                                                  new XElement("Left", Canvas.GetLeft(item)),
                                                  new XElement("Top", Canvas.GetTop(item)),
                                                  new XElement("Width", item.Width),
                                                  new XElement("Height", item.Height),
                                                  new XElement("ID", item.ID),
                                                  new XElement("zIndex", Canvas.GetZIndex(item)),
                                                  new XElement("IsGroup", item.IsGroup),
                                                  new XElement("ParentID", item.ParentID),
                                                  new XElement("Content", contentXaml),
                                                  new XElement("BK", item.BKImgSource),
                                                  new XElement("MKN", item.MuKuaiName),
                                                  new XElement("ZBN", item.ZuoBiaoName),
                                                  new XElement("ConnectorVisble", item.ConnectorVisble)

                                              )
                                   );

            return serializedItems;
        }

        private XElement SerializeConnections(IEnumerable<Connection> connections)
        {
            var serializedConnections = new XElement("Connections",
                           from connection in connections
                           select new XElement("Connection",
                                      new XElement("LineName", connection.LineName),
                                      new XElement("RDType", connection.RDType),
                                      new XElement("MutilLineCount", connection.MutilLineCount),
                                      new XElement("RDValue", connection.RDValue),

                                       new XElement("RDDirection", connection.RDDirection),
                                      new XElement("LeftPoints", connection.LeftPoints),
                                      new XElement("RightPoints", connection.RightPoints),

                                      new XElement("SourceID", connection.Source.ParentDesignerItem.ID),
                                      new XElement("SinkID", connection.Sink.ParentDesignerItem.ID),
                                      new XElement("SourceConnectorName", connection.Source.Name),
                                      new XElement("SinkConnectorName", connection.Sink.Name),
                                      new XElement("SourceArrowSymbol", connection.SourceArrowSymbol),
                                      new XElement("SinkArrowSymbol", connection.SinkArrowSymbol),
                                      new XElement("zIndex", Canvas.GetZIndex(connection)),
                                      new XElement("start", connection.StartPoint),
                                      new XElement("end", connection.EndPoint)
                              
                                     

                                     )
                                  );

            return serializedConnections;
        }

        private static DesignerItem DeserializeDesignerItem(XElement itemXML, Guid id, double OffsetX, double OffsetY)
        {
            DesignerItem item = new DesignerItem(id);
            item.Width = Double.Parse(itemXML.Element("Width").Value, CultureInfo.InvariantCulture);
            item.Height = Double.Parse(itemXML.Element("Height").Value, CultureInfo.InvariantCulture);
            item.ParentID = new Guid(itemXML.Element("ParentID").Value);
            item.IsGroup = Boolean.Parse(itemXML.Element("IsGroup").Value);
            Canvas.SetLeft(item, Double.Parse(itemXML.Element("Left").Value, CultureInfo.InvariantCulture) + OffsetX);
            Canvas.SetTop(item, Double.Parse(itemXML.Element("Top").Value, CultureInfo.InvariantCulture) + OffsetY);
            Canvas.SetZIndex(item, Int32.Parse(itemXML.Element("zIndex").Value));
            Object content = XamlReader.Load(XmlReader.Create(new StringReader(itemXML.Element("Content").Value)));
            item.Content = content;
            item.ItemContent.Append( itemXML.Element("Content").Value);
            item.BKImgSource = Convert.ToString(itemXML.Element("BK").Value);
            string imageFileName= string.Format(@"{0}\images{1}", System.Windows.Forms.Application.StartupPath, item.BKImgSource);
            if (item.BKImgSource != string.Empty   && File.Exists(imageFileName)   )
            {
             

               

                System.IO.FileStream fs = new System.IO.FileStream(imageFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
                fs.Dispose();
                System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                ms.Dispose();
                ImageBrush imagebrush = new ImageBrush();
                imagebrush.ImageSource = bitmapImage;
                item.Background = imagebrush;




            }
            item.MuKuaiName = Convert.ToString(itemXML.Element("MKN").Value);
            item.ZuoBiaoName = Convert.ToString(itemXML.Element("ZBN").Value);
            item.ConnectorVisble = Convert.ToBoolean(itemXML.Element("ConnectorVisble").Value);
            return item;
        }

        private void CopyCurrentSelection()
        {
            IEnumerable<DesignerItem> selectedDesignerItems =
                this.SelectionService.CurrentSelection.OfType<DesignerItem>().Where(s => !s.Content.ToString().Contains("System.Windows.Controls.TextBlock")).ToList();
            /*
            List<Connection> selectedConnections =
                this.SelectionService.CurrentSelection.OfType<Connection>().ToList();

            foreach (Connection connection in this.Children.OfType<Connection>())
            {
                if (!selectedConnections.Contains(connection))
                {
                    //根据连线 找图形
                    DesignerItem sourceItem = (from item in selectedDesignerItems
                                               where item.ID == connection.Source.ParentDesignerItem.ID
                                               select item).FirstOrDefault();

                    DesignerItem sinkItem = (from item in selectedDesignerItems
                                             where item.ID == connection.Sink.ParentDesignerItem.ID
                                             select item).FirstOrDefault();

                    if (sourceItem != null &&
                        sinkItem != null &&
                        BelongToSameGroup(sourceItem, sinkItem))
                    {
                        selectedConnections.Add(connection);
                    }
                }
            }
             * */

            XElement designerItemsXML = SerializeDesignerItems(selectedDesignerItems);
            //XElement connectionsXML = SerializeConnections(selectedConnections);

            XElement root = new XElement("Root");
            root.Add(designerItemsXML);
           // root.Add(connectionsXML);

            root.Add(new XAttribute("OffsetX", 10));
            root.Add(new XAttribute("OffsetY", 10));

            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
        }
        private Window1 GetWindow1(DependencyObject element)
        {
            while (element != null && !(element is Window1))
                element = VisualTreeHelper.GetParent(element);

            return element as Window1;
        }
        private TestWindow1 GetTestWindow1(DependencyObject element)
        {
            while (element != null && !(element is TestWindow1))
                element = VisualTreeHelper.GetParent(element);

            return element as TestWindow1;
        }
        private int delCount = 0;
        public void DeleteCurrentSelection()
        {
            delCount += 1;
            IEnumerable<DesignerItem> designerItems = SelectionService.CurrentSelection.OfType<DesignerItem>();
           XElement designerItemsXML = SerializeDesignerItems(designerItems);
            Task.Run(() =>
            {

                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)delegate ()
                {
                    
                    XElement root = new XElement("Root");
                    root.Add(designerItemsXML);
                    string s = string.Format(@"{0}\backup\{1}.xml", System.Windows.Forms.Application.StartupPath, delCount.ToString());// DateTime.Now.ToString("yyyyMMddHHmmssffff")
                    root.Save(s);
           });
            });

            
                 foreach (Connection connection in SelectionService.CurrentSelection.OfType<Connection>())
                 {
                     //connection.

                     this.Children.Remove(connection);
                 }

                 foreach (DesignerItem item in SelectionService.CurrentSelection.OfType<DesignerItem>())
                 {
                     Control cd = item.Template.FindName("PART_ConnectorDecorator", item) as Control;

                     List<Connector> connectors = new List<Connector>();
                     GetConnectors(cd, connectors);

                     foreach (Connector connector in connectors)
                     {
                         foreach (Connection con in connector.Connections)
                         {
                             this.Children.Remove(con);
                         }
                     }
                     this.Children.Remove(item);
                 }

                 SelectionService.ClearSelection();
                 UpdateZIndex();
                
        }

        public void UpdateZIndex()
        {
            List<UIElement> ordered = (from UIElement item in this.Children
                                       orderby Canvas.GetZIndex(item as UIElement)
                                       select item as UIElement).ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                Canvas.SetZIndex(ordered[i], i);
            }
        }

        private static Rect GetBoundingRectangle(IEnumerable<DesignerItem> items)
        {
            double x1 = Double.MaxValue;
            double y1 = Double.MaxValue;
            double x2 = Double.MinValue;
            double y2 = Double.MinValue;

            foreach (DesignerItem item in items)
            {
                x1 = Math.Min(Canvas.GetLeft(item), x1);
                y1 = Math.Min(Canvas.GetTop(item), y1);

                x2 = Math.Max(Canvas.GetLeft(item) + item.Width, x2);
                y2 = Math.Max(Canvas.GetTop(item) + item.Height, y2);
            }

            return new Rect(new Point(x1, y1), new Point(x2, y2));
        }

        private void GetConnectors(DependencyObject parent, List<Connector> connectors)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is Connector)
                {
                    connectors.Add(child as Connector);
                }
                else
                    GetConnectors(child, connectors);
            }
        }

        public Connector GetConnector(Guid itemID, String connectorName)
        {
            DesignerItem designerItem = (from item in this.Children.OfType<DesignerItem>()
                                         where item.ID == itemID
                                         select item).FirstOrDefault();

            Control connectorDecorator = designerItem.Template.FindName("PART_ConnectorDecorator", designerItem) as Control;
            connectorDecorator.ApplyTemplate();

            return connectorDecorator.Template.FindName(connectorName, connectorDecorator) as Connector;
        }

        private bool BelongToSameGroup(IGroupable item1, IGroupable item2)
        {
            IGroupable root1 = SelectionService.GetGroupRoot(item1);
            IGroupable root2 = SelectionService.GetGroupRoot(item2);

            return (root1.ID == root2.ID);
        }

        #endregion

     
    }
}
