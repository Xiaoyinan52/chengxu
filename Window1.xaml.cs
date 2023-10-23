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
using System.Text;
using Newtonsoft.Json;
using HslCommunication.LogNet;//日志文件控件的引用
using WireTestProgram.NodeTreeClass;
using WireTestProgram.RDValueStudy;
using System.Threading;//WPF线程系统类型
using System.IO.Ports;
using System.Windows.Threading;//WPF线程系统类型
using System.Diagnostics;
using System.Threading.Tasks;
using System.Configuration;
using WireTestProgram.Register;

namespace WireTestProgram
{

    public partial class Window1 : Window
    {
        //文字 System.Windows.Controls.TextBlock//组合System.Windows.Controls.Canvas//圆圈System.Windows.Shapes.Ellipse
        private static object objlock = new object();
        //通常的做法是日志文件存储在exe程序目录下的Logs文件夹中，无论在服务器端还是客户端都是非常适用的
        private ILogNet logNet = new LogNetSingle(System.Windows.Forms.Application.StartupPath + "\\Logs\\DesignLog.txt");//先实例化单文件存储的机制
        private string userNameSecutity = null;

        public int bankaQty = 0;
        private static System.Windows.Threading.DispatcherTimer readDataTimer;//集成到按指定时间间隔和指定优先级处理的 Dispatcher 队列中的计时器。

        //串口
        public System.IO.Ports.SerialPort _serialPort4;//表示串行端口资源。
        //开始涂色的标记
        public string startColor = "end";

        //信息统计
        private TongjiForm tongji;

        public Window1(string userNameSecutity, int bankaQty)
        {
            InitializeComponent();
            this.userNameSecutity = userNameSecutity;
            this.bankaQty = bankaQty;
            tongji = new TongjiForm(MyDesigner);

        }

        public void timeCycle(object sender, EventArgs e)  //获取系统时间
        {
            currentTime.Content = "系统时间:" + System.DateTime.Now.ToString();
        }

        private int AppearTimes(List<string> L, string text)
        {
            int c = 0;
            foreach (string s in L)
                if (s == text)
                    c++;
            return c;
        }
        //根据命名生成连线
        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "Excel文件|*.xls;*.xlsx";

                bool? result = openFileDialog.ShowDialog();
                if (result == true)
                {

                    List<Connection> connections = (from item in MyDesigner.Children.OfType<Connection>()
                                                    select item).ToList();
                    // MyDesigner.Children.OfType<Connection>().ToList();



                    foreach (Connection connection in connections)
                    {

                        MyDesigner.Children.Remove(connection);

                    }




                    MyDesigner.UpdateZIndex();


                    //打开excel 提取第一个表
                    MyTableUtil myTableUtil = new MyTableUtil();
                    DataSet dsa = myTableUtil.ExcelToDataSet(openFileDialog.FileName, true);
                    if(dsa.Tables.Count  >1)
                    {
                        MessageBox.Show("当前EXCEL存在多个工作表");
                        return;
                    }
                    DataTable dtCopy = dsa.Tables[0];


                    DataTable dt = dtCopy.Copy();

                    DataView dv = dt.DefaultView;
                    dv.Sort = dt.Columns[6].ColumnName;
                    dt = dv.ToTable();

                    int createSum = 0;

                    DataColumn dc1 = new DataColumn("startPoint", typeof(string));
                    //dc1.DefaultValue = "0000";
                    dt.Columns.Add(dc1);
                    DataColumn dc2 = new DataColumn("endPoint", typeof(string));
                    //dc2.DefaultValue = "0000";
                    dt.Columns.Add(dc2);
                    /*
                    List<string> chongList = new List<string>();
                    if (dt.Rows.Count > 0 )
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            chongList.Add(Convert.ToString(dt.Rows[i][3]));
                        }
                    }
                    */
                    //坐标点
                    List<DesignerItem> designItems = MyDesigner.Children.OfType<DesignerItem>().Where(s => s.Content.ToString().Contains("System.Windows.Shapes.Ellipse") && !string.IsNullOrEmpty(s.ZuoBiaoName) && !string.IsNullOrEmpty(s.MuKuaiName)).ToList();

                    if (dt.Rows.Count > 0 && designItems != null && designItems.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            /*
                            int at = AppearTimes(chongList, Convert.ToString(dt.Rows[i][3]));
                           if ( AppearTimes(chongList, Convert.ToString(dt.Rows[i][3]))>1)
                            {
                                MessageBox.Show(Convert.ToString(dt.Rows[i][3])+"重复出现"+ at+"次");
                                return;
                            }
                            */
                            if (Convert.ToString(dt.Rows[i][3]) == "K" || Convert.ToString(dt.Rows[i][5]) == "K")
                            {
                                MessageBox.Show(Convert.ToString(dt.Rows[i][3]) + "-" + Convert.ToString(dt.Rows[i][5]) + " 检测到空点K命名");
                                return;
                            }

                            //直线
                            if (Convert.ToString(dt.Rows[i][7]) == "N")
                            {

                                DesignerItem StartDesignItem = designItems.Where(s => !string.IsNullOrEmpty(s.MuKuaiName) && s.MuKuaiName == Convert.ToString(dt.Rows[i][3])).FirstOrDefault();
                                if (StartDesignItem != null)
                                {
                                    dt.Rows[i]["startPoint"] = StartDesignItem.ZuoBiaoName;
                                }
                                else
                                {
                                    MessageBox.Show(Convert.ToString(dt.Rows[i][3]) + "没能找到对应起始物理坐标！！！");
                                    return;
                                }
                                DesignerItem EndDesignItem = designItems.Where(s => !string.IsNullOrEmpty(s.MuKuaiName) && s.MuKuaiName == Convert.ToString(dt.Rows[i][5]) && !s.ZuoBiaoName.Contains(Convert.ToString(dt.Rows[i]["startPoint"]))).FirstOrDefault();
                                if (EndDesignItem != null)
                                {
                                    dt.Rows[i]["endPoint"] = EndDesignItem.ZuoBiaoName;
                                }
                                else
                                {
                                    MessageBox.Show(Convert.ToString(dt.Rows[i][5]) + "没能找到对应结束物理坐标！！！");
                                    return;
                                }
                            } //多线
                            else if (Convert.ToString(dt.Rows[i][7]) == "Y")
                            {
                                DesignerItem StartDesignItem = designItems.Where(s => !string.IsNullOrEmpty(s.MuKuaiName) && s.MuKuaiName == Convert.ToString(dt.Rows[i][3])).FirstOrDefault();
                                if (StartDesignItem != null)
                                {
                                    dt.Rows[i]["startPoint"] = StartDesignItem.ZuoBiaoName;
                                }
                                else
                                {
                                    MessageBox.Show(Convert.ToString(dt.Rows[i][3]) + "没能找到对应起始物理坐标！！！");
                                    return;
                                }
                            }


                        }


                        //System.Diagnostics.Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(dt));


                        string forFlag = "zhangping!";


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            //先连接直线
                            if (Convert.ToString(dt.Rows[i][7]) == "N")
                            {
                                Connection s = MyDesigner.getConnectorBaseOnZuoBiaoName(Convert.ToString(dt.Rows[i][8]), Convert.ToString(dt.Rows[i][9]));
                                if (s == null)
                                {
                                    return;
                                }
                                createSum += 1;
                            }
                            else if (Convert.ToString(dt.Rows[i][7]) == "Y")
                            {

                                if (Convert.ToString(dt.Rows[i][6]) != forFlag)
                                {
                                    forFlag = Convert.ToString(dt.Rows[i][6]);

                                    DataRow[] dr = dt.Select(dt.Columns[6].ColumnName + " = '" + Convert.ToString(dt.Rows[i][6]) + "' ");
                                    //多线
                                    if (dr.Length > 1)
                                    {
                                        //List<int> temArray = new List<int>();

                                        int[] temArray = new int[dr.Length];

                                        for (int j = 0; j < dr.Length; j++)
                                        {
                                            temArray[j] = System.Int32.Parse(dr[j][8].ToString(), System.Globalization.NumberStyles.HexNumber);

                                        }
                                        int minValue = temArray.Min();
                                        string string16 = minValue.ToString("X4");


                                        foreach (DataRow drN in dr)
                                        {

                                            if (Convert.ToString(drN[8]) != string16)
                                            {
                                                Connection s = MyDesigner.getConnectorBaseOnZuoBiaoName(string16, Convert.ToString(drN[8]));
                                                if (s == null)
                                                {
                                                    return;
                                                }
                                                createSum += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show(Convert.ToString(dt.Rows[i][1]) + "多线只有一个数据！！！");
                                        return;
                                    }

                                    //多线



                                }
                                else
                                {
                                    continue;
                                }


                            }


                        } //for (int i = 0; i < dt.Rows.Count; i++)


                        MessageBox.Show("表单共" + dt.Rows.Count.ToString() + "行数据， " + "总共生成" + createSum.ToString() + "条连接关系！");
                        
                    }
                }// if (result == true)
            }
            catch (Exception ex)
            {
                MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                logNet.WriteDebug("生成连线 error", ex.Message);
                logNet.WriteDebug("----------");
            }

        }
        //根据坐标生成连线
        /*
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            try
            {

           
              Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Filter = "Excel文件|*.xls;*.xlsx";

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
               
                List<Connection> connections = (from item in MyDesigner.Children.OfType<Connection>()
                                                select item).ToList();
                //MessageBox.Show(connections.Count.ToString());
                foreach (Connection connection in connections)
                {
                    //connection.
                    //Button btn = canvas.FindName("newButton") as Button;//找到刚新添加的按钮   
                    //if (btn != null)//判断是否找到，以免在未添加前就误点了   
                    //{
                    //    canvas.Children.Remove(btn);//移除对应按钮控件   
                    //    canvas.UnregisterName("newButton");//还需要把对用的名字注销掉，否则再次点击Button_Add会报错   
                    //}  
                    MyDesigner.Children.Remove(connection);
                    //MyDesigner.UnregisterName("L" + connection.Name);
                }


                MyDesigner.UpdateZIndex();
               

                //打开excel 提取第一个表
                MyTableUtil myTableUtil = new MyTableUtil();

                DataTable dtCopy = myTableUtil.ExcelToDataSet(openFileDialog.FileName, true).Tables[0];
                DataTable dt = dtCopy.Copy();

                DataView dv = dt.DefaultView;
                dv.Sort = dt.Columns[6].ColumnName;
                dt = dv.ToTable();

                int createSum = 0;
                //MessageBox.Show(JsonConvert.SerializeObject(dt));
                
                //MessageBox.Show(dt.Rows.Count.ToString());
                //先连接直线
               // List<DesignerItem> designItems = MyDesigner.Children.OfType<DesignerItem>().Where(s => s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();
                      
                string forFlag = "zhangping!";
                if (dt.Rows.Count > 0)
                {
                   
                  
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        //先连接直线
                        if ( Convert.ToString( dt.Rows[i][7] )== "N")
                        {
                           Connection s= MyDesigner.getConnectorBaseOnZuoBiaoName(Convert.ToString(dt.Rows[i][8]), Convert.ToString(dt.Rows[i][9]));
                           if (s == null)
                           {
                               return;
                           }
                           createSum += 1;
                        }
                        else if (Convert.ToString(dt.Rows[i][7]) == "Y")
                        {

                            if ( Convert.ToString( dt.Rows[i][6] ) != forFlag)
                            {
                                forFlag =Convert.ToString( dt.Rows[i][6] );
                                
                                DataRow[] dr = dt.Select(dt.Columns[6].ColumnName +" = '" + Convert.ToString( dt.Rows[i][6]) + "' ");
                                //多线
                                if (dr.Length > 1)
                                {
                                    //List<int> temArray = new List<int>();

                                    int[] temArray = new int[dr.Length];

                                    for (int j = 0; j < dr.Length; j++)
                                    {
                                        temArray[j] = System.Int32.Parse(dr[j][8].ToString(), System.Globalization.NumberStyles.HexNumber);

                                    }
                                    int minValue = temArray.Min();
                                    string string16 = minValue.ToString("X4");
                                    
                                    
                                    foreach (DataRow drN in dr)
                                    {

                                        if (Convert.ToString(drN[8]) != string16)
                                        {
                                            Connection s = MyDesigner.getConnectorBaseOnZuoBiaoName(string16,Convert.ToString(drN[8]));
                                            if (s == null)
                                            {
                                                return;
                                            }
                                            createSum += 1;
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(Convert.ToString(dt.Rows[i][1]) + "多线只有一个数据！！！");
                                    return;
                                }

                                //多线



                            }
                            else
                            {
                                continue;
                            }


                        }

                     
                    } //for (int i = 0; i < dt.Rows.Count; i++)


                    MessageBox.Show("表单共" + dt.Rows.Count.ToString()+"行数据， "+"总共生成"+createSum.ToString()+"条连接关系！");



                }// if (dt.Rows.Count > 0)


                

            }// if (result == true)

            }
            catch (Exception ex)
            {
                MessageBox.Show( "系统出现可预知错误，请到日志查看相关记录！！！");
                logNet.WriteDebug("生成连线 error", ex.Message);
                logNet.WriteDebug("----------");
            }

        }
        */
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            MyDesigner.bankaQty = this.bankaQty;

            readDataTimer = new System.Windows.Threading.DispatcherTimer();
            readDataTimer.Tick += new EventHandler(timeCycle);
            readDataTimer.Interval = new TimeSpan(0, 0, 0, 1);
            readDataTimer.Start();

            userName_lab.Content = "当前登录用户：" + userNameSecutity.Split(',')[0];
            //串口信息
            DBaccessHelp db = new DBaccessHelp();
            DataTable dt = db.ExecuteQuery("select comport from setTable where setFlag='set' ");
            db.closeOleDbConnection();
            _serialPort4 = new System.IO.Ports.SerialPort();
            _serialPort4.PortName = dt.Rows[0][0].ToString();
            _serialPort4.BaudRate = 115200;
            _serialPort4.Parity = System.IO.Ports.Parity.None;
            _serialPort4.DataBits = 8;
            _serialPort4.StopBits = StopBits.One;
            _serialPort4.ReadTimeout = 500;
            _serialPort4.WriteTimeout = 500;

            this._serialPort4.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort4_DataReceived_Pin);


        }
        //连线全部显示
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {


            List<Connection> connections = (from item in MyDesigner.Children.OfType<Connection>()
                                            select item).ToList();


            MessageBox.Show(connections.Count.ToString());
            foreach (var item in connections)
                item.Visibility = Visibility.Visible;
            /*
              List<DesignerItem> selectedDesignItem=
                      MyDesigner.Children.OfType<DesignerItem>().ToList();
              for (int i = 0; i < selectedDesignItem.Count; i++)
              {

                  MessageBox.Show(selectedDesignItem[i].Content.ToString());
              }
             * */

        }
        /*
                private void MenuItem_Click_2(object sender, RoutedEventArgs e)
                {
                    try
                    {



                        List<DesignerItem> designerItemSum = MyDesigner.Children.OfType<DesignerItem>().Where(s => (s.Content.ToString().Contains("System.Windows.Shapes.Ellipse"))).ToList();


                        int countI = 0;

                        foreach (var item in designerItemSum)
                        {
                      Connector ctor= MyDesigner.GetConnector(item.ID, "Center");

                      if (ctor!=null && ctor.Connections.Count == 0)
                      {
                          MyDesigner.SelectionService.AddToSelection(item);
                          countI = countI + 1;
                      }

                        }
                        MessageBox.Show("未连线点共有"+countI.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                        logNet.WriteDebug("查看连线 error", ex.Message);
                        logNet.WriteDebug("----------");
                    }


                }
        */
        private void suoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<DesignerItem> designerItems = MyDesigner.Children.OfType<DesignerItem>().Where(s => (s.Content.ToString().Contains("System.Windows.Shapes.Path") || s.Content.ToString().Contains("System.Windows.Controls.Canvas")) && !string.IsNullOrEmpty(s.MuKuaiName) && !s.MuKuaiName.Contains("$#%")).ToList();

                List<DesignerItem> designerItemAllPoints = MyDesigner.Children.OfType<DesignerItem>().Where(s => s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")  &&  !string.IsNullOrEmpty(s.ZuoBiaoName) ).ToList();


                LockStudy ls = new LockStudy(designerItems, userNameSecutity.Split(',')[0], bankaQty, designerItemAllPoints);
                ls.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                logNet.WriteDebug("锁 打开 error", ex.Message);
                logNet.WriteDebug("----------");
            }
        }
        /*
                private void MenuItem_Click_3(object sender, RoutedEventArgs e)
                {
                    try
                    {



                        List<DesignerItem> designerItemSumsss = MyDesigner.Children.OfType<DesignerItem>().Where(s => (s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")) ).ToList();

                        int countI = 0;

                        foreach (var item in designerItemSumsss)
                        {
                            //Connector ctor = MyDesigner.GetConnector(item.ID, "Center");

                            //if (ctor != null && ctor.Connections.Count == 0)
                            //{
                            if (string.IsNullOrEmpty(item.ZuoBiaoName))
                            {
                                MyDesigner.SelectionService.AddToSelection(item);
                                countI = countI + 1;
                            }

                            //}

                        }
                        MessageBox.Show("未Ping点共有" + countI.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                        logNet.WriteDebug("查看未Ping点 error", ex.Message);
                        logNet.WriteDebug("----------");
                    }
                }
        */

        //地址查询
        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            String PM = Microsoft.VisualBasic.Interaction.InputBox("请输入元素物理地址", "地址查询", "", -1, -1);
            if (PM != string.Empty)
            {

                try
                {
                    List<DesignerItem> designerItemSumsss = MyDesigner.Children.OfType<DesignerItem>().Where(s => s.ZuoBiaoName == PM && s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();

                    // int countI = 0;

                    StringBuilder sb = new StringBuilder();
                    MyDesigner.SelectionService.ClearSelection();
                    foreach (var item in designerItemSumsss)
                    {


                        DesignerItem fd = this.findHutao(item);
                        string hutaoName = string.Empty;
                        if (fd != null) hutaoName = fd.MuKuaiName;
                        MyDesigner.SelectionService.AddToSelection(item);
                        //countI = countI + 1;
                        sb.Append("N:" + item.MuKuaiName + " Z:" + item.ZuoBiaoName + " X:" + Convert.ToString(Convert.ToInt32(Canvas.GetLeft(item))) + " Y:" + Convert.ToString(Convert.ToInt32(Canvas.GetTop(item))) + " L:" + hutaoName);
                        sb.Append(Environment.NewLine);


                    }
                    MessageBox.Show(sb.ToString(), "查询明细");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                    logNet.WriteDebug("元素地址查询 error", ex.Message);
                    logNet.WriteDebug("----------");
                }
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (_serialPort4.IsOpen)
            {
                Thread.Sleep(1000);
                _serialPort4.Close();
                currentStatus_lab.Content = "";
                currentPoint_lab.Content = "";
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {//串口打开
            try
            {
                if (!_serialPort4.IsOpen)
                {

                    _serialPort4.Open();
                    currentStatus_lab.Content = "正在进行Pin点坐标";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                logNet.WriteDebug("打开串口错误 error", ex.Message);
                logNet.WriteDebug("----------");
            }

        }
        private List<string> pindianBanka = new List<string>();
        //第二步：声明第一步方法的委托
        private delegate void ModifyButton_dg(string fresult,int s);
        private void ModifyButton(string fresult, int s)
        {
            //System.Diagnostics.Debug.WriteLine("fresult:" + fresult + "  s " + s.ToString());
            if (fresult == "ok")
            {

            lock (objlock)
            {

                currentPoint_lab.Content = s.ToString("X4");
                //要执行的代码逻辑
                //mtextshow.SerialData = s.ToString("X4");
                DesignerItem ds = this.MyDesigner.SelectionService.CurrentSelection.OfType<DesignerItem>().Where(item => item.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList().FirstOrDefault();
                if (ds != null)
                {

                    //1取得数据 read.mtextshow.SerialData
                    //2给当前元素定义坐标名

                    List<DesignerItem> designItems = this.MyDesigner.Children.OfType<DesignerItem>().Where(item2 => item2.ZuoBiaoName == s.ToString("X4") && item2.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();



                    if (designItems.Count == 1)
                    {

                        if (MessageBox.Show("是否覆盖当前点？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {

                            designItems[0].ZuoBiaoName = "";
                            designItems[0].ConnectorVisble = false;
                            ds.ZuoBiaoName = s.ToString("X4");
                            ds.ConnectorVisble = true;
                        }
                        else
                        {
                            this.MyDesigner.SelectionService.ClearSelection();
                            this.MyDesigner.SelectionService.SelectItem(designItems[0]);
                            designItems[0].Focus();
                        }
                    }
                    else if (designItems.Count == 0)
                    {
                        ds.ZuoBiaoName = s.ToString("X4");
                        ds.ConnectorVisble = true;
                    }
                    else if (designItems.Count > 1)
                    {
                        MessageBox.Show("警告！！！该物理坐标点已经存在！");
                    }

                }
            }
            }
            else
            {
                currentPoint_lab.Content = currentPoint_lab.Content+s.ToString("X4")+"短路";
            }



        }
    private void serialPort4_DataReceived_Pin(object sender, SerialDataReceivedEventArgs e)//串口接收数据函数
        {
            try
            {
                Thread.Sleep(50);
                string serialPort_read = _serialPort4.ReadExisting();

                //string serialPort_read = _serialPort4.ReadTo("*");
                //System.Diagnostics.Debug.WriteLine(serialPort_read);

                if (serialPort_read.Contains("Msg,35")) //返回有几个板卡有数据
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)delegate ()
                {
                    lock (objlock)
                    {
                        currentPoint_lab.Content = "";
                    }

                });
                    pindianBanka.Clear();
                    string[] a = serialPort_read.Split(',');

                    if (System.Int32.Parse(a[3], System.Globalization.NumberStyles.HexNumber) == 0)
                    { //板卡数=0


                        MessageBox.Show("无单元板应答");
                        return;

                    }
                    else if (System.Int32.Parse(a[3], System.Globalization.NumberStyles.HexNumber) == 1)
                    { //板卡数=1 哪些板卡装进集合
                        pindianBanka.Add(a[4]);
                        Task task = new Task(() =>
                        {
                            Thread.Sleep(100);
                            _serialPort4.Write("$Cmd,40," + pindianBanka[0] + ",98*"); //成功了要数
                        });
                        //启动任务
                        task.Start();
                       
                        return;

                    }
                    else if (System.Int32.Parse(a[3], System.Globalization.NumberStyles.HexNumber) > 1)
                    { //板卡数>1 哪些板卡装进集合
                        pindianBanka.Add(a[4]);
                        pindianBanka.Add(a[5]);
                        Task task = new Task(() =>
                        {
                            Thread.Sleep(100);
                            _serialPort4.Write("$Cmd,40," + pindianBanka[0] + ",98*"); //成功了要数
                        });
                        //启动任务
                        task.Start();
                       
                        return;
                    }


                }//serialread 字符串  关联  35 几块板卡


                if (pindianBanka.Count == 1) //serialread 字符串  关联  40  哪块办卡有数据
                {
                    if (serialPort_read.Contains("Pin,40," + pindianBanka[0])) //返回有几个板卡有数据
                    {

                        string[] d1 = serialPort_read.Split(',');
                        if (System.Int32.Parse(d1[3], System.Globalization.NumberStyles.HexNumber) == 0){ //一块板子  没有数据

                            MessageBox.Show(pindianBanka[0]+"板卡没有返回任何数据");
                            return;
                        }else if (System.Int32.Parse(d1[3], System.Globalization.NumberStyles.HexNumber) == 1) //一块板子 只有一个数据的时候
                        {

                            int s = (System.Int32.Parse(d1[2], System.Globalization.NumberStyles.HexNumber) - 1) * 64 + System.Int32.Parse(d1[4], System.Globalization.NumberStyles.HexNumber);
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new ModifyButton_dg(ModifyButton),"ok", s);


                        }
                        else//一块板子 有多个数据
                        {
                            int s4 = (System.Int32.Parse(d1[2], System.Globalization.NumberStyles.HexNumber) - 1) * 64 + System.Int32.Parse(d1[4], System.Globalization.NumberStyles.HexNumber);
                            int s5 = (System.Int32.Parse(d1[2], System.Globalization.NumberStyles.HexNumber) - 1) * 64 + System.Int32.Parse(d1[5], System.Globalization.NumberStyles.HexNumber);

                            MessageBox.Show("板卡内存在短路:"+s4.ToString("X4")+"-"+s5.ToString("X4"));
                            return;

                        }

                    }



                }else  if (pindianBanka.Count == 2)
                {
                    if (serialPort_read.Contains("Pin,40," + pindianBanka[0])) //返回有几个板卡有数据
                    {
                        string[] d1 = serialPort_read.Split(',');
                        if (System.Int32.Parse(d1[3], System.Globalization.NumberStyles.HexNumber) == 0)
                        { //一块板子  没有数据

                            MessageBox.Show(pindianBanka[0] + "板卡没有返回任何数据");
                            return;
                        }
                        else if (System.Int32.Parse(d1[3], System.Globalization.NumberStyles.HexNumber) == 1) //一块板子 只有一个数据的时候
                        {

                            int s = (System.Int32.Parse(d1[2], System.Globalization.NumberStyles.HexNumber) - 1) * 64 + System.Int32.Parse(d1[4], System.Globalization.NumberStyles.HexNumber);
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new ModifyButton_dg(ModifyButton), "ng", s);


                        }
                        else//一块板子 有多个数据
                        {
                            int s4 = (System.Int32.Parse(d1[2], System.Globalization.NumberStyles.HexNumber) - 1) * 64 + System.Int32.Parse(d1[4], System.Globalization.NumberStyles.HexNumber);
                            int s5 = (System.Int32.Parse(d1[2], System.Globalization.NumberStyles.HexNumber) - 1) * 64 + System.Int32.Parse(d1[5], System.Globalization.NumberStyles.HexNumber);

                            MessageBox.Show("板卡内存在短路:" + s4.ToString("X4") + "-" + s5.ToString("X4"));
                            return;

                        }
                        Task task = new Task(() =>
                        {
                            Thread.Sleep(100);
                            _serialPort4.Write("$Cmd,40," + pindianBanka[1] + ",98*"); //成功了要数
                        });
                        //启动任务
                        task.Start();
                      
                        return;
                    }
                    if (serialPort_read.Contains("Pin,40," + pindianBanka[1])) //返回有几个板卡有数据
                    {
                        string[] d1 = serialPort_read.Split(',');
                        if (System.Int32.Parse(d1[3], System.Globalization.NumberStyles.HexNumber) == 0)
                        { //一块板子  没有数据

                            MessageBox.Show(pindianBanka[0] + "板卡没有返回任何数据");
                            return;
                        }
                        else if (System.Int32.Parse(d1[3], System.Globalization.NumberStyles.HexNumber) == 1) //一块板子 只有一个数据的时候
                        {

                            int s = (System.Int32.Parse(d1[2], System.Globalization.NumberStyles.HexNumber) - 1) * 64 + System.Int32.Parse(d1[4], System.Globalization.NumberStyles.HexNumber);
                            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new ModifyButton_dg(ModifyButton), "ng", s);


                        }
                        else//一块板子 有多个数据
                        {
                            int s4 = (System.Int32.Parse(d1[2], System.Globalization.NumberStyles.HexNumber) - 1) * 64 + System.Int32.Parse(d1[4], System.Globalization.NumberStyles.HexNumber);
                            int s5 = (System.Int32.Parse(d1[2], System.Globalization.NumberStyles.HexNumber) - 1) * 64 + System.Int32.Parse(d1[5], System.Globalization.NumberStyles.HexNumber);

                            MessageBox.Show("板卡内存在短路:" + s4.ToString("X4") + "-" + s5.ToString("X4"));
                            return;

                        }

                    }




                }











            }
            catch (Exception ex)
            {
                MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                logNet.WriteDebug("串口数据 error", ex.Message);
                logNet.WriteDebug("----------");
            }



        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_serialPort4.IsOpen)
            {
                Thread.Sleep(1000);
                _serialPort4.Close();
            }
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            currentStatus_lab.Content = "正在进行涂色...";
            startColor = "start";
            ColorPicker colorPicker = new ColorPicker(MyDesigner, this);
            colorPicker.Show();

        }

        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {
            currentStatus_lab.Content = "";
            startColor = "end";
        }
        //未命名元素选择
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                List<DesignerItem> designerItemSumsss = MyDesigner.Children.OfType<DesignerItem>().Where(s => string.IsNullOrEmpty(s.MuKuaiName) && s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();

                int countI = 0;
                MyDesigner.SelectionService.ClearSelection();
                foreach (var item in designerItemSumsss)
                {


                    MyDesigner.SelectionService.AddToSelection(item);
                    countI = countI + 1;




                }
                MessageBox.Show("当前未命名元素共有" + countI.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                logNet.WriteDebug("未命名元素查询 error", ex.Message);
                logNet.WriteDebug("----------");
            }
        }
        //命名查询
        private void MenuItem_Click_8(object sender, RoutedEventArgs e)
        {
            String PM = Microsoft.VisualBasic.Interaction.InputBox("请输入元素命名", "命名查询", "", -1, -1);
            if (PM != string.Empty)
            {

                try
                {
                    List<DesignerItem> designerItemSumsss = MyDesigner.Children.OfType<DesignerItem>().Where(s => s.MuKuaiName == PM).ToList();// && s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")


                    StringBuilder sb = new StringBuilder();
                    MyDesigner.SelectionService.ClearSelection();
                    foreach (var item in designerItemSumsss)
                    {


                        MyDesigner.SelectionService.AddToSelection(item);
                        DesignerItem fd = this.findHutao(item);
                        string hutaoName = string.Empty;
                        if (fd != null) hutaoName = fd.MuKuaiName;
                        sb.Append("N:" + item.MuKuaiName + " Z:" + item.ZuoBiaoName + " X:" + Convert.ToString(Convert.ToInt32(Canvas.GetLeft(item))) + " Y:" + Convert.ToString(Convert.ToInt32(Canvas.GetTop(item)))+" L:" + hutaoName);
                        sb.Append(Environment.NewLine);




                    }
                    MessageBox.Show(sb.ToString(), "查询明细");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                    logNet.WriteDebug("命名查询 error", ex.Message);
                    logNet.WriteDebug("----------");
                }
            }
        }

        //改变左侧属性框
        public void changeProperty(DesignerItem d)
        {
            //List<DesignerItem> designerItems = MyDesigner.SelectionService.CurrentSelection.OfType<DesignerItem>().ToList();
            

            id_txt.Text = d.ID.ToString();
            ZuoBiaoName_txt.Text = d.ZuoBiaoName;
            Xzuobiao_txt.Text = Convert.ToString(Convert.ToInt32(Canvas.GetLeft(d)));
            Yzuobiao_txt.Text = Convert.ToString(Convert.ToInt32(Canvas.GetTop(d)));
            address_txb.Text = "";
            DesignerItem dd = findHutao(d);
            if (dd != null)
            {
                itemGroup_txb.Text = dd.MuKuaiName;
            }
            else
            {
                itemGroup_txb.Text = "";
            }
          
            if (!string.IsNullOrEmpty(d.ZuoBiaoName))
            {
                try
                {
                    int s0 = System.Int32.Parse(d.ZuoBiaoName, System.Globalization.NumberStyles.HexNumber);
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
                    address_txb.Text = chu.ToString() + "-" + yu.ToString();
                }



                catch (Exception ex)
                {
                    MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                    logNet.WriteDebug("转换点坐标 error", ex.Message);
                    logNet.WriteDebug("----------");
                }

            }
            else
            {

            }

            BKImgSource_txt.Text = d.BKImgSource;

            itemName_txb.Text = d.MuKuaiName;
            itemName_txb.Select(0, itemName_txb.Text.Length);
            itemName_txb.Focus();




        }
        public void showChangeProperty(DesignerItem d)
        {
            id_txt.Text = d.ID.ToString();
            itemName_txb.Text = d.MuKuaiName;
            itemGroup_txb.Text = "";
            Xzuobiao_txt.Text = Convert.ToString(Convert.ToInt32(Canvas.GetLeft(d)));
            Yzuobiao_txt.Text = Convert.ToString(Convert.ToInt32(Canvas.GetTop(d)));
            ZuoBiaoName_txt.Text = d.ZuoBiaoName;
            address_txb.Text = d.ZuoBiaoName;
            BKImgSource_txt.Text = d.BKImgSource;
        }

        private void ItemName_txb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
             


                List<DesignerItem> designerItemSumsss = MyDesigner.Children.OfType<DesignerItem>().Where(s => s.ID.ToString() == id_txt.Text && s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();
                List<Connection> designerItemsConnections = MyDesigner.Children.OfType<Connection>().Where(s => s.StartPoint == designerItemSumsss[0].ZuoBiaoName || s.EndPoint == designerItemSumsss[0].ZuoBiaoName).ToList();
                if (designerItemsConnections.Count > 0)
                {
                    MessageBox.Show("该点已存在连接关系！禁止操作");
                    return;

                }

                if (itemName_txb.Text == "")
                {

                   
                    MessageBox.Show("命名失败");
                    return;
                }
                if (itemName_txb.Text == "K")
                {

                    designerItemSumsss[0].MuKuaiName = "K";
                    MessageBox.Show("修改成功");
                    return;
                }

                if (itemName_txb.Text == "#")
                {
                    designerItemSumsss[0].MuKuaiName = "";
                    MessageBox.Show("修改成功");
                    return;
                }
               
                //没有启用手工编程 
                if (!Convert.ToString(ManualProgram_lab.Content).Contains("手工编程"))
                {

                    List<DesignerItem> designerItems = MyDesigner.Children.OfType<DesignerItem>().Where(s => s.MuKuaiName == itemName_txb.Text && s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();

                    if (designerItems.Count > 1)
                {

                    MyDesigner.SelectionService.ClearSelection();
                    foreach (var item in designerItems)
                    {


                        MyDesigner.SelectionService.AddToSelection(item);

                    }
                    MessageBox.Show("已经存在" + designerItems.Count.ToString() + "个点");

                    return;
                }
                if (designerItemSumsss.Count == 1)
                {

                    if (itemName_txb.Text == "#")
                    {
                        designerItemSumsss[0].MuKuaiName = "";

                    }
                    else
                    {
                        designerItemSumsss[0].MuKuaiName = itemName_txb.Text;
                    }
                    MessageBox.Show("修改成功");
                }
                else
                {
                    MessageBox.Show("当前修改元素名称的个数超出索引");
                }


                }
                else//手工编程
                {
                    searchHowManyContainsConnections.Clear();
                    List<DesignerItem> designerItems = MyDesigner.Children.OfType<DesignerItem>().Where(s => s.MuKuaiName == itemName_txb.Text && !string.IsNullOrEmpty( s.ZuoBiaoName) && s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();

                    if (string.IsNullOrEmpty (designerItemSumsss[0].ZuoBiaoName) )
                    {
                        MessageBox.Show("该点没有Pin点！");
                        return;
                    }
                  
                   
                    if (itemName_txb.Text == "#")
                    {
                        designerItemSumsss[0].MuKuaiName = "";
                        MessageBox.Show("修改成功");
                    }
                    else
                    {
                        //生成连线
                        List<Connection> lineConnections = new List<Connection>();
                        List<int> allZuobiao = new List<int>();
                        allZuobiao.Add(System.Int32.Parse(designerItemSumsss[0].ZuoBiaoName, System.Globalization.NumberStyles.HexNumber)    );
                        foreach (DesignerItem  d in designerItems)
                        {

                            List<Connection> connectionsD = MyDesigner.Children.OfType<Connection>().Where(s => s.StartPoint == d.ZuoBiaoName || s.EndPoint == d.ZuoBiaoName).ToList();
                            if(connectionsD.Count == 0)
                            {
                                allZuobiao.Add(System.Int32.Parse(d.ZuoBiaoName, System.Globalization.NumberStyles.HexNumber));
                            }
                            else
                            {
                                searchHowManyConnection(connectionsD[0]);
                            }
                            foreach (Connection c in searchHowManyContainsConnections)
                            {
                                allZuobiao.Add(System.Int32.Parse(c.StartPoint, System.Globalization.NumberStyles.HexNumber));
                                allZuobiao.Add(System.Int32.Parse(c.EndPoint, System.Globalization.NumberStyles.HexNumber));
                            }
                           
                        }
                        //删除当前的线
                        foreach (Connection connection in searchHowManyContainsConnections)
                        {

                            MyDesigner.Children.Remove(connection);

                        }
                        MyDesigner.UpdateZIndex();
                        //allZuobiao 去重
                        List<int> allZuobiaoquchong = allZuobiao.Distinct().ToList();
                        int minValue = allZuobiaoquchong.Min();
                        for (int i = allZuobiaoquchong.Count - 1; i >= 0; i--)
                        {
                            if (allZuobiaoquchong[i] == minValue)
                            {
                                allZuobiaoquchong.Remove(minValue);
                                break;
                            }
                                
                        }
                       foreach(  int zuobiao in allZuobiaoquchong)
                        {
                            Connection s = MyDesigner.getConnectorBaseOnZuoBiaoName(minValue.ToString("X4"), zuobiao.ToString("X4"));
                        }
                        

                        designerItemSumsss[0].MuKuaiName = itemName_txb.Text;
                        MessageBox.Show("修改成功");
                    }







                }//手工编程






            }//key
        }
        private List<Connection> searchHowManyContainsConnections = new List<Connection>();
        //知道一条连线  找出所有相关的连线
        private void searchHowManyConnection(Connection conn)
        {

            searchHowManyContainsConnections.Add(conn);

            var connections = this.MyDesigner.Children.OfType<Connection>().Where(item => (item.StartPoint == conn.StartPoint || item.EndPoint == conn.EndPoint || item.StartPoint == conn.EndPoint || item.EndPoint == conn.StartPoint)).ToList();
            var exp = connections.Where(a => !searchHowManyContainsConnections.Exists(t => (a.ID.ToString()).Contains(t.ID.ToString()))).ToList();



            if (exp.Count > 0)
            {
                foreach (Connection itemexp in exp)
                //for (int i = 0; i < exp.Count; i++)
                {
                    searchHowManyContainsConnections.Add(itemexp);
                    searchHowManyConnection(itemexp);
                }




            }
            else
            {
                return;

            }


        }
        private void ImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo info = new ProcessStartInfo();

            info.FileName = string.Format(@"{0}\FSCapture\FSCapture.exe", System.Windows.Forms.Application.StartupPath);
            info.Arguments = "";
            info.WindowStyle = ProcessWindowStyle.Minimized;
            Process pro = Process.Start(info);
            //pro.WaitForExit();
        }
        //连线全部隐藏
        private void MenuItem_Click_9(object sender, RoutedEventArgs e)
        {
            List<Connection> connections = (from item in MyDesigner.Children.OfType<Connection>()
                                            select item).ToList();


            MessageBox.Show(connections.Count.ToString());
            foreach (var item in connections)
                item.Visibility = Visibility.Hidden;
        }
        //开始布点
        public string autoEllipse = string.Empty;
        private void MenuItem_Click_10(object sender, RoutedEventArgs e)
        {
            autoEllipse = "正在自动布点";
            currentEllipse_lab.Content = autoEllipse;

        }
        //结束布点
        private void MenuItem_Click_11(object sender, RoutedEventArgs e)
        {
            autoEllipse = string.Empty;
            currentEllipse_lab.Content = autoEllipse;
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                System.Diagnostics.Debug.Write(DateTime.Now.ToString());
                var designerItems = MyDesigner.SelectionService.CurrentSelection.OfType<DesignerItem>();
                if (designerItems != null && designerItems.Count() > 0)
                {
                    double minLeft = double.MaxValue;
                    double minTop = double.MaxValue;
                    foreach (DesignerItem item in designerItems)
                    {
                        double left = Canvas.GetLeft(item);
                        double top = Canvas.GetTop(item);

                        minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
                        minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);
                    }

                    foreach (DesignerItem item in designerItems)
                    {
                        double left = Canvas.GetLeft(item);
                        double top = Canvas.GetTop(item);

                        if (double.IsNaN(left)) left = 0;
                        if (double.IsNaN(top)) top = 0;

                        Canvas.SetLeft(item, left);


                        Canvas.SetTop(item, top - 1);
                    }

                    MyDesigner.InvalidateMeasure();//更新元素布局
                }
            }
            else if (e.Key == Key.Down)
            {
                System.Diagnostics.Debug.Write(DateTime.Now.ToString());
                var designerItems = MyDesigner.SelectionService.CurrentSelection.OfType<DesignerItem>();
                if (designerItems != null && designerItems.Count() > 0)
                {
                    double minLeft = double.MaxValue;
                    double minTop = double.MaxValue;
                    foreach (DesignerItem item in designerItems)
                    {
                        double left = Canvas.GetLeft(item);
                        double top = Canvas.GetTop(item);

                        minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
                        minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);
                    }

                    foreach (DesignerItem item in designerItems)
                    {
                        double left = Canvas.GetLeft(item);
                        double top = Canvas.GetTop(item);

                        if (double.IsNaN(left)) left = 0;
                        if (double.IsNaN(top)) top = 0;

                        Canvas.SetLeft(item, left);


                        Canvas.SetTop(item, top + 1);
                    }

                    MyDesigner.InvalidateMeasure();//更新元素布局
                }
            }


            else if (e.Key == Key.Left)
            {
                System.Diagnostics.Debug.Write(DateTime.Now.ToString());
                var designerItems = MyDesigner.SelectionService.CurrentSelection.OfType<DesignerItem>();
                if (designerItems != null && designerItems.Count() > 0)
                {
                    double minLeft = double.MaxValue;
                    double minTop = double.MaxValue;
                    foreach (DesignerItem item in designerItems)
                    {
                        double left = Canvas.GetLeft(item);
                        double top = Canvas.GetTop(item);

                        minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
                        minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);
                    }

                    foreach (DesignerItem item in designerItems)
                    {
                        double left = Canvas.GetLeft(item);
                        double top = Canvas.GetTop(item);

                        if (double.IsNaN(left)) left = 0;
                        if (double.IsNaN(top)) top = 0;

                        Canvas.SetLeft(item, left - 1);


                        Canvas.SetTop(item, top);
                    }

                    MyDesigner.InvalidateMeasure();//更新元素布局
                }
            }
            else if (e.Key == Key.Right)
            {
                System.Diagnostics.Debug.Write(DateTime.Now.ToString());
                var designerItems = MyDesigner.SelectionService.CurrentSelection.OfType<DesignerItem>();
                if (designerItems != null && designerItems.Count() > 0)
                {
                    double minLeft = double.MaxValue;
                    double minTop = double.MaxValue;
                    foreach (DesignerItem item in designerItems)
                    {
                        double left = Canvas.GetLeft(item);
                        double top = Canvas.GetTop(item);

                        minLeft = double.IsNaN(left) ? 0 : Math.Min(left, minLeft);
                        minTop = double.IsNaN(top) ? 0 : Math.Min(top, minTop);
                    }

                    foreach (DesignerItem item in designerItems)
                    {
                        double left = Canvas.GetLeft(item);
                        double top = Canvas.GetTop(item);

                        if (double.IsNaN(left)) left = 0;
                        if (double.IsNaN(top)) top = 0;

                        Canvas.SetLeft(item, left + 1);


                        Canvas.SetTop(item, top);
                    }

                    MyDesigner.InvalidateMeasure();//更新元素布局
                }
            }
            else if (e.Key == Key.Delete)
            {

                var designerItems = MyDesigner.SelectionService.CurrentSelection.OfType<DesignerItem>();
                var connections = MyDesigner.SelectionService.CurrentSelection.OfType<Connection>();

                if (designerItems.Count() > 0 || connections.Count() > 0)
                {



                    System.Windows.Forms.MessageBoxButtons messButton = System.Windows.Forms.MessageBoxButtons.OKCancel;
                    System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show("确定要删除" + designerItems.Count().ToString() + "个元素," + connections.Count().ToString() + "条连线", "删除", messButton);

                    if (dr == System.Windows.Forms.DialogResult.OK)//如果点击“确定”按钮

                    {
                        MyDesigner.DeleteCurrentSelection();


                    }
                    else//如果点击“取消”按钮
                    {



                    }

                }

            }
        }

        public void showWaitIco()
        {
            waitIco.Visibility = Visibility.Visible;
        }
        
        public void hideWaitIco(string str)
        {
            waitIco.Visibility = Visibility.Collapsed;
            filePath_lab.Content = "当前路径:"+str;
        }
        //命名清空
        private void MenuItem_Click_12(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBoxButtons messButton = System.Windows.Forms.MessageBoxButtons.OKCancel;
            System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show("确定要清空所有命名吗", "清空", messButton);

            if (dr == System.Windows.Forms.DialogResult.OK)//如果点击“确定”按钮

            {
                List<DesignerItem> items = MyDesigner.Children.OfType<DesignerItem>().Where(s => s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();

                if (items != null && items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        item.MuKuaiName = string.Empty;
                    }

                    MessageBox.Show("清空完毕");
                }

            }
            else//如果点击“取消”按钮
            {



            }





        }
        //全部删除连线
        private void MenuItem_Click_13(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBoxButtons messButton = System.Windows.Forms.MessageBoxButtons.OKCancel;
            System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show("确定要清空所有命名吗", "清空", messButton);

            if (dr == System.Windows.Forms.DialogResult.OK)//如果点击“确定”按钮

            {
                List<Connection> items = MyDesigner.Children.OfType<Connection>().ToList();

                if (items != null && items.Count > 0)
                {
                    foreach (var item in items)
                    {

                        //connection.

                        MyDesigner.Children.Remove(item);

                    }
                    MyDesigner.SelectionService.ClearSelection();
                    MyDesigner.UpdateZIndex();
                    MessageBox.Show("删除完毕");
                }

            }
            else//如果点击“取消”按钮
            {



            }
        }

        //快捷键
        //连线全部显示
        private void CommandBindingShow_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBindingShow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<Connection> connections = (from item in MyDesigner.Children.OfType<Connection>()
                                            select item).ToList();



            foreach (var item in connections)
                item.Visibility = Visibility.Visible;
        }


        //连线全部隐藏
        private void CommandBindingHide_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBindingHide_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<Connection> connections = (from item in MyDesigner.Children.OfType<Connection>()
                                            select item).ToList();
            foreach (var item in connections)
                item.Visibility = Visibility.Hidden;
        }

        //只选择连线
        private void CommandBindingconnectionsSelct_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBindingconnectionsSelct_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<Connection> designerItems = MyDesigner. SelectionService.CurrentSelection.OfType<Connection>().Where(s=> s.Visibility==Visibility.Visible).ToList();
            
            MyDesigner.SelectionService.ClearSelection();
            foreach(Connection  item in designerItems)
            {
                MyDesigner.SelectionService.AddToSelection(item);
            }
           

        }

        //mukuainame show 
        private void CommandBindingmukuaiNameShow_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBindingmukuaiNameShow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<DesignerItem> designerItems = MyDesigner.Children.OfType<DesignerItem>().Where(s => s.Content.ToString().Contains("System.Windows.Controls.TextBlock") ).ToList();
            //s.Visibility == Visibility.Visible


            foreach (var item in designerItems)
                if (item.Visibility == Visibility.Visible)
                {
                    item.Visibility = Visibility.Hidden;
                }
                else
                {
                    item.Visibility = Visibility.Visible;
                }
            
        }

        //信息统计
        //直线 就是 起始点和结束点  都只能找到一根线的 就是直线   
        //搭点就是  起始点可以找到 很多直线的   直线集合  去掉  左右两端有组合的  
        private void TongjiMenuItem_Click(object sender, RoutedEventArgs e)
        {
           
            tongji.taskexcute();
            tongji.Show();
        }
//输入一个点  找到 在那个护套里
        private  DesignerItem findHutao(DesignerItem d)
        {

            List<DesignerItem> designerItems = MyDesigner.Children.OfType<DesignerItem>().Where(s => (s.Content.ToString().Contains("System.Windows.Shapes.Path") && s.ParentID.ToString() == "00000000-0000-0000-0000-000000000000" && s.MuKuaiName.Length > 0 && !s.MuKuaiName.Contains("$#%")) || (s.Content.ToString().Contains("System.Windows.Controls.Canvas") && s.ParentID.ToString() == "00000000-0000-0000-0000-000000000000" && s.MuKuaiName.Length > 0 && !s.MuKuaiName.Contains("$#%"))).ToList();
            if (designerItems.Count > 0)
            {
                DesignerItem r = null; 
                foreach(DesignerItem item in designerItems)
                {
                    double x1 = Canvas.GetLeft(item);
                    double y1 = Canvas.GetTop(item);

                    double x2 = Canvas.GetLeft(item)+ item.Width;
                    double y2 = Canvas.GetTop(item)+item.Height;

                    if (  Canvas.GetLeft(d) >=x1  && Canvas.GetLeft(d)<=x2 && Canvas.GetTop(d)>=y1 && Canvas.GetTop(d) <= y2)
                    {
                        r=item;
                        break;
                    }

                }

                return r;
            }
            return null;


}
        //手工编程
        private void ManualProgram_menu_Click(object sender, RoutedEventArgs e)
        {
           
            // 检测激活码是否正确，没有文件，或激活码错误都算作激活失败
            ComputerInfo cc = new ComputerInfo();
            EncryptionHelper ee = new EncryptionHelper();
            string encryptComputer = ee.Encrypt(cc.GetComputerInfo(), "20070901");
            string md5 = ee.GetMD5String(encryptComputer);

            //cc.WriteFile( ee.GetMD5String(encryptComputer), string.Format(@"{0}\Authorize.txt", System.Windows.Forms.Application.StartupPath));
            if (!File.Exists(string.Format(@"{0}\AuthorizeLine.txt", System.Windows.Forms.Application.StartupPath)))
            {
                cc.WriteFile(encryptComputer, string.Format(@"{0}\AuthorizeLine.txt", System.Windows.Forms.Application.StartupPath));
                MessageBox.Show("系统检测到手工编程没有注册,请根据文件AuthorizeLine中的序列号进行注册。");
                ManualProgram_lab.Content = "";
            }
            else
            {
                if (cc.ReadFile(string.Format(@"{0}\AuthorizeLine.txt", System.Windows.Forms.Application.StartupPath)) == md5)
                {
                    if(  Convert.ToString( ManualProgram_lab.Content ).Contains("手工编程"))
                    {
                        ManualProgram_lab.Content = "";
                    }
                    else
                    {
                        ManualProgram_lab.Content = "启用手工编程...";
                    }


                    
                }
                else
                {
                    cc.WriteFile(encryptComputer, string.Format(@"{0}\AuthorizeLine.txt", System.Windows.Forms.Application.StartupPath));
                    MessageBox.Show("系统检测到手工编程没有注册,请根据文件AuthorizeLine中的序列号进行注册。");
                    ManualProgram_lab.Content = "";

                }
            }

        }

    }
}
