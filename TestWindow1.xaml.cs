
using HslCommunication.LogNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WireTestProgram.gasLock;
using WireTestProgram.HelperClass;

namespace WireTestProgram
{
    /// <summary>
    /// TestWindow1.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow1 : Window
    {
        private ILogNet logNet = new LogNetSingle(System.Windows.Forms.Application.StartupPath + "\\Logs\\DesignLog.txt");

        private string userNameSecutity = null;

        private int bankaQty = 0;
        //元素集合
        private List<Connection> connectionSum = null;
        private List<List<string>> connectionsMutilLine = null;
        private List<DesignerItem> designerItemSum = null;
        private List<Connection> connectionContainRs = null;
        private List<Connection> connectionContainDs = null;
        private List<Connection> connectionAdd = null;
        private List<string> shortPoints = null;



        //锁集合
        private List<SuoClass> suoClassSum = null;
        //电压集合
        private List<VoltageClass> voltageClassSum = null;
        //所有关联的二极管连线集合
        private List<Connection> searchHowManyContainDsConnections = null;

        //串口
        public System.IO.Ports.SerialPort _serialPort1;
        private string serialPort_read;
        //开关串口
        public System.IO.Ports.SerialPort _serialPortSwitch;

        //线程
        private static object objlock = new object();
        public AutoResetEvent mre = new AutoResetEvent(false);
        //task
        Task longTask = null;
        CancellationTokenSource cts = null;


        //测试条码
        private string currentBarcode = string.Empty;
        //测试设置
        private string scanModel = string.Empty;
        private string MemoryModel = string.Empty;
        private string shortBreak = string.Empty;
        private string serialPort = string.Empty;
        private string fmodelLength = string.Empty;
        private string fbarcodeLength = string.Empty;
        private string fbarcodeFront = string.Empty;
        private string switchcomport = string.Empty;


        public TestWindow1(string userNameSecutity, int bankaQty)
        {
            InitializeComponent();
            this.userNameSecutity = userNameSecutity;
            this.bankaQty = bankaQty;
            //int maxworkthreads, maxportthreads, minworkthreads, minportthreads;
            //ThreadPool.GetMaxThreads(out maxworkthreads, out maxportthreads);
            //ThreadPool.GetMinThreads(out minworkthreads, out minportthreads);
            //MessageBox.Show(maxworkthreads.ToString() + "\t" + maxportthreads.ToString());
            //MessageBox.Show(minworkthreads.ToString() + "\t" + minportthreads.ToString());
        }


        private bool isDragging;
        private Point startPosition;

        private void grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            var draggableElement = sender as UIElement;
            var clickPosition = e.GetPosition(this);

            var transform = draggableElement.RenderTransform as TranslateTransform;
            startPosition.X = clickPosition.X - transform.X;    //注意减号
            startPosition.Y = clickPosition.Y - transform.Y;

            draggableElement.CaptureMouse();
        }

        private void grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            var draggableElement = sender as UIElement;
            draggableElement.ReleaseMouseCapture();
        }

        private void grid_MouseMove(object sender, MouseEventArgs e)
        {
            var draggableElement = sender as UIElement;
            if (isDragging && draggableElement != null)
            {
                Point currentPosition = e.GetPosition(this.Parent as UIElement);
                var transform = draggableElement.RenderTransform as TranslateTransform;

                transform.X = currentPosition.X - startPosition.X;
                transform.Y = currentPosition.Y - startPosition.Y;
            }
        }

        private List<List<string>> listGroup;
        private List<List<Connection>> listGroupD;
        private int lineCount = 0;
        private void openFile_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                suo_txt.Text = "";
                valueShow_txt.Text = "";

                connectionAdd = null;
                suoClassSum = null;
                voltageClassSum = null;
                searchHowManyContainDsConnections = null;

                connectionAdd = new List<Connection>();
                suoClassSum = new List<SuoClass>();
                voltageClassSum = new List<VoltageClass>();
                searchHowManyContainDsConnections = new List<Connection>();
                //多线集合
                connectionsMutilLine = new List<List<string>>();
                //短路点
                shortPoints = new List<string>();

                MyDesigner2.OpenFile();
                //圆圈集合
                designerItemSum = MyDesigner2.Children.OfType<DesignerItem>().Where(s => (s.Content.ToString().Contains("System.Windows.Shapes.Ellipse"))).ToList();
                //连线集合
                // connectionSum = MyDesigner2.Children.OfType<Connection>().Where(s => (!s.RDType.Contains("D")) && (!s.RDType.Contains("RD"))).ToList();
                connectionSum = MyDesigner2.Children.OfType<Connection>().ToList();
                lineCount = connectionSum.Count();
                //电阻集合
                connectionContainRs = MyDesigner2.Children.OfType<Connection>().Where(s => s.RDType == "R" || s.RDType == "RD").ToList();
                //二极管集合
                connectionContainDs = MyDesigner2.Children.OfType<Connection>().Where(s => s.RDType == "D" || s.RDType == "RD").ToList();
                //说明  连线 只包含两种  一种是直线  另一种就是二极管连线    电阻只是这两种连线中的属性  最后判断电阻
                                                                                                                                         //所有二极管相关联的线
                if (connectionContainDs.Count > 0)
                {
                    for (int i = 0; i < connectionContainDs.Count; i++)
                    {
                        this.searchHowManyConnection(connectionContainDs[i]);
                    }
                }


               
                //多线集合
                //connectionsMutilLine = MyDesigner2.Children.OfType<Connection>().ToList();
                if (connectionSum != null)
                {
                  foreach(DesignerItem  designerItemItem in designerItemSum)
                    {
                        //多线的个数
                        Connector ctr = MyDesigner2.GetConnector(designerItemItem.ID, "Center");
                        List<Connection> connectionMGS = ctr.Connections.Where(t => !t.MutilLineCount.Contains("2020")).ToList(); 
                        if (connectionMGS.Count > 1)
                        {
                            List<string> ls = new List<string>();
                            foreach (Connection c in connectionMGS)
                            {
                                ls.Add(c.StartPoint);
                                ls.Add(c.EndPoint);
                            }
                            connectionsMutilLine.Add(ls);

                        }
                    }

                }
                //MessageBox.Show(   Newtonsoft.Json.JsonConvert.SerializeObject(connectionsMutilLine) );
                //这边是先找到 多线  然后再去除去二极管  找到所有的直线   因为  二极管可能存在多线内
                //导通集合 除去所有二极管相关联连线之外  所有的连线  
                connectionSum = connectionSum.Where(a => !searchHowManyContainDsConnections.Exists(t => (a.ID.ToString()).Contains(t.ID.ToString()))).ToList();




                /*
                //验证
                    MessageBox.Show(searchHowManyContainDsConnections.Count.ToString());
                    List<Connection> connections = (from item in this.MyDesigner2.Children.OfType<Connection>()
                                                    select item).ToList();
                    //var exp = connections.Where(a => !searchHowManyContainDsConnections.Exists(t => (a.ID.ToString()).Contains(t.ID.ToString()))).ToList();
                    var exp = connections.Where(a => !connectionSum.Exists(t => (a.ID.ToString()).Contains(t.ID.ToString()))).ToList();
                    // Console.WriteLine("--查找connections集合中存在，而list不存在的数据--");
                    foreach (var item in exp)
                    {
                        item.Visibility = Visibility.Hidden;
                    }
                */





                if (connectionContainRs.Count > 0)
                {

                    for (int i = 0; i < connectionContainRs.Count; i++)
                    {
                        VoltageClass voltage = new VoltageClass();
                        voltage.RDType = connectionContainRs[i].RDType;
                        voltage.RDirection= connectionContainRs[i].RDDirection;
                        double b = 0;
                        if (double.TryParse(connectionContainRs[i].RDValue, out b) == false) //判断是否可以转换为整型
                        {
                            b = 0;
                        }
                        voltage.RDValue = b;

                        voltage.SmallPoint = connectionContainRs[i].StartPoint;
                        voltage.BigPoint = connectionContainRs[i].EndPoint;
                        voltage.SmallPointValue = 0;
                        //大点集合
                        List<TestVoltagePoint> bigPoints = new List<TestVoltagePoint>();
                        List<string> rightPoints = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(connectionContainRs[i].RightPoints);
                        if (rightPoints != null && rightPoints.Count > 0)
                        {
                            for (int r = 0; r < rightPoints.Count; r++)
                            {
                                TestVoltagePoint testVoltagePoint = new TestVoltagePoint();
                                testVoltagePoint.TestPoint = rightPoints[r];
                                testVoltagePoint.VoltageValue = 0;
                                bigPoints.Add(testVoltagePoint);
                            }

                        }

                        voltage.BigPoints = bigPoints;


                        voltage.VisibleFlag = true;
                        voltage.ISConnection = true;


                        voltageClassSum.Add(voltage);

                    }




                }


                //锁集合SuoClassSum
                DBaccessHelp2 db = new DBaccessHelp2();
                DataTable dt = db.ExecuteQuery("select  SuoName,StartPoint,EndPoint from  SuoSource where FStatus='启用' and fmodel='" + MyDesigner2.fmodelInsert + "' ");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        SuoClass suo = new SuoClass();
                        suo.SuoName = Convert.ToString(dt.Rows[i][0]);
                        suo.StartPoint = Convert.ToString(dt.Rows[i][1]);
                        suo.EndPoint = Convert.ToString(dt.Rows[i][2]);
                        suo.VisibleFlag = true;
                        suoClassSum.Add(suo);
                        suo_txt.Text += Convert.ToString(dt.Rows[i][0]) + "  ";
                    }
                }
                db.closeOleDbConnection();
                //System.Diagnostics.Debug.WriteLine("锁集合:" + Newtonsoft.Json.JsonConvert.SerializeObject(suoClassSum));




                //MessageBox.Show("连线：" + connectionSum.Count.ToString() + " 电阻：" + connectionContainRs.Count.ToString() + "二极管：" + connectionContainDs.Count.ToString() + " 元素：" + designerItemSum.Count.ToString());
                connection_lab.Content = "连线" + (lineCount + connectionContainDs.Count).ToString() + "      电阻" + connectionContainRs.Count.ToString() + "     二极管" + connectionContainDs.Count.ToString();
                filePath_lab.Content = MyDesigner2.fmodelInsert;

                //电阻 二极管 下发多少次
                rrdd = 0;
                status_lab.Content = "就绪";
                //电阻组合
                List<string> liststringOfRs = new List<string>();
                if (connectionContainRs.Count > 0)
                {
                    for (int i = 0; i < connectionContainRs.Count; i++)
                    {
                        List<string> rightPoints = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(connectionContainRs[i].RightPoints);
                        if (rightPoints != null && rightPoints.Count > 0)
                        {
                            for (int r = 0; r < rightPoints.Count; r++)
                            {
                                if (connectionContainRs[i].RDDirection == "1")
                                {
                                    liststringOfRs.Add(connectionContainRs[i].StartPoint + rightPoints[r]);
                                }
                                else
                                {
                                    liststringOfRs.Add(rightPoints[r]+connectionContainRs[i].StartPoint );
                                }
                               

                            }

                        }

                    }


                }
                
                
                //下发电阻
                //分组 10个一组
                listGroup = null;
                listGroup = new List<List<string>>();
                int j = 10;
                for (int i = 0; i < liststringOfRs.Count; i += 10)
                {
                    List<string> cList = new List<string>();
                    cList = liststringOfRs.Take(j).Skip(i).ToList();
                    j += 10;
                    listGroup.Add(cList);
                }
                //分组完成后  整合字符串
                if (listGroup.Count > 0)
                {

                    for (int i = 0; i < listGroup.Count; i++)
                    {
                        Thread.Sleep(2000);

                        if (i == 0)
                        {
                            string str0 = "$Cmd,02,T";
                            for (int k = 0; k < (listGroup[i]).Count; k++)
                            {
                                str0 += "R" + (listGroup[i])[k];
                            }
                            str0 += ",98*";

                            _serialPort1.Write(str0); //要电压  分组
                                                      //MessageBox.Show(str0);
                        }
                        else
                        {
                            string str1 = "$Cmd,02,N";
                            for (int k = 0; k < listGroup[i].Count; k++)
                            {
                                str1 += "R" + (listGroup[i])[k];
                            }
                            str1 += ",98*";

                            _serialPort1.Write(str1); //要电压

                        }


                    }
                }

                //下发二极管
                //分组 10个一组
                listGroupD = null;
                listGroupD = new List<List<Connection>>();
                int pj = 10;
                for (int pi = 0; pi < connectionContainDs.Count; pi += 10)
                {
                    List<Connection> cListD = new List<Connection>();
                    cListD = connectionContainDs.Take(pj).Skip(pi).ToList();
                    pj += 10;
                    listGroupD.Add(cListD);
                }
                //分组完成后  整合字符串
                if (listGroupD.Count > 0)
                {

                    for (int i = 0; i < listGroupD.Count; i++)
                    {
                        Thread.Sleep(2000);

                        if (i == 0)
                        {
                            string str0 = "$Cmd,02,";
                            if (listGroup.Count > 0)   //下发电阻的时候有R就清空 所以不要T了 
                            {
                                str0 += "N";
                            }
                            else
                            {
                                str0 += "T";
                            }
                            for (int k = 0; k < (listGroupD[i]).Count; k++)
                            {
                                str0 += "G" + (listGroupD[i])[k].StartPoint + (listGroupD[i])[k].EndPoint;
                            }
                            str0 += ",98*";
                            _serialPort1.Write(str0); //要电压  分组
                                                      //MessageBox.Show(str0);
                        }
                        else
                        {
                            string str1 = "$Cmd,02,N";
                            for (int k = 0; k < listGroupD[i].Count; k++)
                            {
                                str1 += "G" + (listGroupD[i])[k].StartPoint + (listGroupD[i])[k].EndPoint;
                            }
                            str1 += ",98*";
                            _serialPort1.Write(str1); //要电压
                                                      //MessageBox.Show(str1);
                        }


                    }
                }

                startTest_menuItem.IsEnabled = true;


            }
            catch (Exception ex)
            {
                MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                logNet.WriteDebug("打开文件 error", ex.Message);
                logNet.WriteDebug("----------");
            }


        }
        //电阻 二极管 下发多少次
        private int rrdd = 0;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // userName_lab.Content = "当前登录用户：" + userNameSecutity.Split(',')[0]; 
                DBaccessHelp db00 = new DBaccessHelp();
                DataTable dt00 = db00.ExecuteQuery("select  scanModel,memoryModel,shortBreak,comport ,fmodelLength,fbarcodeLength,fbarcodeFront,switchcomport   from setTable where setFlag='set' ");
                db00.closeOleDbConnection();
                scanModel = dt00.Rows[0][0].ToString();
                MemoryModel = dt00.Rows[0][1].ToString();
                shortBreak = dt00.Rows[0][2].ToString();
                serialPort = dt00.Rows[0][3].ToString();
                fmodelLength = dt00.Rows[0][4].ToString();
                fbarcodeLength = dt00.Rows[0][5].ToString();
                fbarcodeFront = dt00.Rows[0][6].ToString();
                switchcomport = dt00.Rows[0][7].ToString();
                //打开串口
                _serialPort1 = new System.IO.Ports.SerialPort();
                _serialPort1.PortName = serialPort;
                _serialPort1.BaudRate = 115200;
                _serialPort1.Parity = System.IO.Ports.Parity.None;
                _serialPort1.DataBits = 8;
                _serialPort1.StopBits = StopBits.One;
                _serialPort1.ReadTimeout = 500;
                _serialPort1.WriteTimeout = 500;
                _serialPort1.Open();
                this._serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.post_DataReceivedModify);
                //打开 开关串口
                if (switchcomport != "关闭")
                {
                    _serialPortSwitch = new System.IO.Ports.SerialPort();
                    _serialPortSwitch.PortName = switchcomport;
                    _serialPortSwitch.BaudRate = 9600;
                    _serialPortSwitch.Parity = System.IO.Ports.Parity.None;
                    _serialPortSwitch.DataBits = 8;
                    _serialPortSwitch.StopBits = StopBits.One;
                    _serialPortSwitch.ReadTimeout = 500;
                    _serialPortSwitch.WriteTimeout = 500;
                    _serialPortSwitch.Open();
                }





                //判断扫描规格
                if (scanModel == "扫描规格")
                {
                    openFile_MenuItem.IsEnabled = false;
                    startTest_menuItem.IsEnabled = true;
                    FModelWindow2 w2 = new FModelWindow2(this, MyDesigner2);
                    if (w2.ShowDialog() == true)
                    {



                        suo_txt.Text = "";
                        valueShow_txt.Text = "";

                        connectionAdd = null;
                        suoClassSum = null;
                        voltageClassSum = null;
                        searchHowManyContainDsConnections = null;

                        connectionAdd = new List<Connection>();
                        suoClassSum = new List<SuoClass>();
                        voltageClassSum = new List<VoltageClass>();
                        searchHowManyContainDsConnections = new List<Connection>();
                        //多线集合
                        connectionsMutilLine = new List<List<string>>();
                        //短路点
                        shortPoints = new List<string>();

                        // MyDesigner2.OpenFile();
                        //圆圈集合
                        designerItemSum = MyDesigner2.Children.OfType<DesignerItem>().Where(s => (s.Content.ToString().Contains("System.Windows.Shapes.Ellipse"))).ToList();
                        //连线集合
                        connectionSum = MyDesigner2.Children.OfType<Connection>().ToList();
                        lineCount = connectionSum.Count();
                        //电阻集合
                        connectionContainRs = MyDesigner2.Children.OfType<Connection>().Where(s => s.RDType == "R" || s.RDType == "RD").ToList();
                        //二极管集合
                        connectionContainDs = MyDesigner2.Children.OfType<Connection>().Where(s => s.RDType == "D" || s.RDType == "RD").ToList();//说明  连线 只包含两种  一种是直线  另一种就是二极管连线    电阻只是这两种连线中的属性  最后判断电阻
                                                                                                                                                 //所有二极管相关联的线
                        if (connectionContainDs.Count > 0)
                        {
                            for (int i = 0; i < connectionContainDs.Count; i++)
                            {
                                this.searchHowManyConnection(connectionContainDs[i]);
                            }
                        }



                        //多线集合
                        //connectionsMutilLine = MyDesigner2.Children.OfType<Connection>().ToList();
                        if (connectionSum != null)
                        {
                            foreach (DesignerItem designerItemItem in designerItemSum)
                            {
                                //多线的个数
                                Connector ctr = MyDesigner2.GetConnector(designerItemItem.ID, "Center");
                                List<Connection> connectionMGS = ctr.Connections.Where(t => !t.MutilLineCount.Contains("2020")).ToList();
                                if (connectionMGS.Count > 1)
                                {
                                    List<string> ls = new List<string>();
                                    foreach (Connection c in connectionMGS)
                                    {
                                        ls.Add(c.StartPoint);
                                        ls.Add(c.EndPoint);
                                    }
                                    connectionsMutilLine.Add(ls);

                                }
                            }
                          


                        }
                        //导通集合 除去所有二极管相关联连线之外  所有的连线
                        connectionSum = connectionSum.Where(a => !searchHowManyContainDsConnections.Exists(t => (a.ID.ToString()).Contains(t.ID.ToString()))).ToList();


                        /*
                        //验证
                            MessageBox.Show(searchHowManyContainDsConnections.Count.ToString());
                            List<Connection> connections = (from item in this.MyDesigner2.Children.OfType<Connection>()
                                                            select item).ToList();
                            //var exp = connections.Where(a => !searchHowManyContainDsConnections.Exists(t => (a.ID.ToString()).Contains(t.ID.ToString()))).ToList();
                            var exp = connections.Where(a => !connectionSum.Exists(t => (a.ID.ToString()).Contains(t.ID.ToString()))).ToList();
                            // Console.WriteLine("--查找connections集合中存在，而list不存在的数据--");
                            foreach (var item in exp)
                            {
                                item.Visibility = Visibility.Hidden;
                            }
                        */

                        if (connectionContainRs.Count > 0)
                        {

                            for (int i = 0; i < connectionContainRs.Count; i++)
                            {
                                VoltageClass voltage = new VoltageClass();
                                voltage.RDType = connectionContainRs[i].RDType;
                                voltage.RDirection = connectionContainRs[i].RDDirection;
                                double b = 0;
                                if (double.TryParse(connectionContainRs[i].RDValue, out b) == false) //判断是否可以转换为整型
                                {
                                    b = 0;
                                }
                                voltage.RDValue = b;

                                voltage.SmallPoint = connectionContainRs[i].StartPoint;
                                voltage.BigPoint = connectionContainRs[i].EndPoint;
                                voltage.SmallPointValue = 0;
                                //大点集合
                                List<TestVoltagePoint> bigPoints = new List<TestVoltagePoint>();
                                List<string> rightPoints = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(connectionContainRs[i].RightPoints);
                                if (rightPoints != null && rightPoints.Count > 0)
                                {
                                    for (int r = 0; r < rightPoints.Count; r++)
                                    {
                                        TestVoltagePoint testVoltagePoint = new TestVoltagePoint();
                                        testVoltagePoint.TestPoint = rightPoints[r];
                                        testVoltagePoint.VoltageValue = 0;
                                        bigPoints.Add(testVoltagePoint);
                                    }

                                }

                                voltage.BigPoints = bigPoints;


                                voltage.VisibleFlag = true;
                                voltage.ISConnection = true;


                                voltageClassSum.Add(voltage);

                            }




                        }


                        //锁集合SuoClassSum
                        DBaccessHelp2 db001 = new DBaccessHelp2();
                        DataTable dt001 = db001.ExecuteQuery("select  SuoName,StartPoint,EndPoint from  SuoSource where FStatus='启用' and fmodel='" + MyDesigner2.fmodelInsert + "'");

                        if (dt001.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt001.Rows.Count; i++)
                            {
                                SuoClass suo = new SuoClass();
                                suo.SuoName = Convert.ToString(dt001.Rows[i][0]);
                                suo.StartPoint = Convert.ToString(dt001.Rows[i][1]);
                                suo.EndPoint = Convert.ToString(dt001.Rows[i][2]);
                                suo.VisibleFlag = true;
                                suoClassSum.Add(suo);
                                suo_txt.Text += Convert.ToString(dt001.Rows[i][0]) + "  ";
                            }
                        }
                        db001.closeOleDbConnection();
                        //System.Diagnostics.Debug.WriteLine("锁集合:" + Newtonsoft.Json.JsonConvert.SerializeObject(suoClassSum));




                        //MessageBox.Show("连线：" + connectionSum.Count.ToString() + " 电阻：" + connectionContainRs.Count.ToString() + "二极管：" + connectionContainDs.Count.ToString() + " 元素：" + designerItemSum.Count.ToString());
                        connection_lab.Content = "连线" + (lineCount + connectionContainDs.Count).ToString() + "      电阻" + connectionContainRs.Count.ToString() + "     二极管" + connectionContainDs.Count.ToString();
                        filePath_lab.Content = MyDesigner2.fmodelInsert;

                        //电阻 二极管 下发多少次
                        rrdd = 0;
                        status_lab.Content = "就绪";
                        //电阻组合
                        List<string> liststringOfRs = new List<string>();
                        if (connectionContainRs.Count > 0)
                        {
                            for (int i = 0; i < connectionContainRs.Count; i++)
                            {
                                List<string> rightPoints = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(connectionContainRs[i].RightPoints);
                                if (rightPoints != null && rightPoints.Count > 0)
                                {
                                    for (int r = 0; r < rightPoints.Count; r++)
                                    {

                                        liststringOfRs.Add(connectionContainRs[i].StartPoint + rightPoints[r]);

                                    }

                                }

                            }


                        }


                        //下发电阻
                        //分组 10个一组
                        listGroup = null;
                        listGroup = new List<List<string>>();
                        int j = 10;
                        for (int i = 0; i < liststringOfRs.Count; i += 10)
                        {
                            List<string> cList = new List<string>();
                            cList = liststringOfRs.Take(j).Skip(i).ToList();
                            j += 10;
                            listGroup.Add(cList);
                        }
                        //分组完成后  整合字符串
                        if (listGroup.Count > 0)
                        {

                            for (int i = 0; i < listGroup.Count; i++)
                            {
                                Thread.Sleep(2000);

                                if (i == 0)
                                {
                                    string str0 = "$Cmd,02,T";
                                    for (int k = 0; k < (listGroup[i]).Count; k++)
                                    {
                                        str0 += "R" + (listGroup[i])[k];
                                    }
                                    str0 += ",98*";

                                    _serialPort1.Write(str0); //要电压  分组
                                                              //MessageBox.Show(str0);
                                }
                                else
                                {
                                    string str1 = "$Cmd,02,N";
                                    for (int k = 0; k < listGroup[i].Count; k++)
                                    {
                                        str1 += "R" + (listGroup[i])[k];
                                    }
                                    str1 += ",98*";

                                    _serialPort1.Write(str1); //要电压

                                }


                            }
                        }


                        //下发二极管
                        //分组 10个一组
                        listGroupD = null;
                        listGroupD = new List<List<Connection>>();
                        int pj = 10;
                        for (int pi = 0; pi < connectionContainDs.Count; pi += 10)
                        {
                            List<Connection> cListD = new List<Connection>();
                            cListD = connectionContainDs.Take(pj).Skip(pi).ToList();
                            pj += 10;
                            listGroupD.Add(cListD);
                        }
                        //分组完成后  整合字符串
                        if (listGroupD.Count > 0)
                        {

                            for (int i = 0; i < listGroupD.Count; i++)
                            {
                                Thread.Sleep(2000);

                                if (i == 0)
                                {
                                    string str0 = "$Cmd,02,";
                                    if (listGroup.Count > 0)
                                    {
                                        str0 += "N";
                                    }
                                    else
                                    {
                                        str0 += "T";
                                    }
                                    for (int k = 0; k < (listGroupD[i]).Count; k++)
                                    {
                                        str0 += "G" + (listGroupD[i])[k].StartPoint + (listGroupD[i])[k].EndPoint;
                                    }
                                    str0 += ",98*";
                                    _serialPort1.Write(str0); //要电压  分组
                                                              //MessageBox.Show(str0);
                                }
                                else
                                {
                                    string str1 = "$Cmd,02,N";
                                    for (int k = 0; k < listGroupD[i].Count; k++)
                                    {
                                        str1 += "G" + (listGroupD[i])[k].StartPoint + (listGroupD[i])[k].EndPoint;
                                    }
                                    str1 += ",98*";
                                    _serialPort1.Write(str1); //要电压
                                                              //MessageBox.Show(str1);
                                }


                            }
                        }

                        startTest_menuItem.IsEnabled = true;

                        w2.Close();






                    }
                    else
                    {

                        this.Close();

                        return;
                    }
                }

            }
            catch (Exception ex)
            {

                openFile_MenuItem.IsEnabled = false;
                MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                logNet.WriteDebug("打开文件 error", ex.Message);
                logNet.WriteDebug("----------");

            }




        }

        //task  执行函数体
        public void trmain(string name, int BanQty, CancellationToken token)
        {
            int loopFlag = BanQty + 2;

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                for (int x = 0; x <= loopFlag; x++)
                {
                    // _serialPort1.DiscardOutBuffer();
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    if (x == 0)
                    {
                        //listGroup listGroupD
                        if (listGroup.Count == 0)
                        {
                            continue;
                        }
                        _serialPort1.Write("$Cmd,03," + BanQty.ToString("X2") + ",98*"); //要电压


                    }
                    else if (x == 1)
                    {
                        if (listGroupD.Count == 0)
                        {
                            continue;
                        }
                       List<Connection> lcd= searchHowManyContainDsConnections.Where(s => s.Visibility == Visibility.Visible).ToList();
                        if (lcd.Count == 0)
                        {
                            continue;
                        }
                        _serialPort1.Write("$Cmd,05," + BanQty.ToString("X2") + ",98*");//要二极管

                    }
                    else if (x == 2)
                    {

                        _serialPort1.Write("$Cmd,01," + BanQty.ToString("X2") + ",98*");//要导通

                    }
                    else if (x > 2)
                    {

                        _serialPort1.Write("$Cmd,3A," + (x - 2).ToString("X2") + ",98*");

                    }
                    mre.WaitOne(15000);


                }
            }



        }
        //串口接收数据
        private void post_DataReceivedModify(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {

                serialPort_read = string.Empty;
                //serialPort_read = _serialPort1.ReadExisting();
                serialPort_read = _serialPort1.ReadTo("*");
                //电阻返回的数据
                if (serialPort_read.Contains("Msg,02,00,98") || serialPort_read.Contains("Msg,02," + bankaQty.ToString("X2") + ",98"))
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (Action)delegate ()
                        {
                            lock (objlock)
                            {
                                rrdd += 1;
                        //要执行的代码逻辑
                        status_lab.Content = "下发电阻、二极管共" + rrdd.ToString() + "组";

                            }

                        });
                }
                else if (serialPort_read.Contains("Msg,10,00,98") || serialPort_read.Contains("Msg,10," + bankaQty.ToString("X2") + ",98"))//印章不处理
                {

                }
                else if (serialPort_read.Contains("Msg,11,00,98") || serialPort_read.Contains("Msg,11," + bankaQty.ToString("X2") + ",98"))//印章不处理
                {

                }
                else
                {
                    TestUI testUI = new TestUI();
                    testUI.WhichBanka = 0;
                    List<TestConnectionPoint> testConnectionPoints = new List<TestConnectionPoint>();//直线    
                    List<TestConnectionPoint> testConnectionDPoints = new List<TestConnectionPoint>();//二极管
                    List<TestVoltagePoint> testVoltagePoints = new List<TestVoltagePoint>();//电阻
                                                                                            //有逗号 就拆开
                                                                                            //if (serialPort_read.Contains("$$Msg")){
                                                                                            //    testUI.WhichBanka = 1;
                                                                                            //}
                    if (serialPort_read.IndexOf("Error") > 0 && serialPort_read.IndexOf(",") > 0)
                    {

                        string[] result = serialPort_read.Split(',');
                        testUI.WhichBanka = System.Int32.Parse(result[2], System.Globalization.NumberStyles.HexNumber);//第几块板卡


                        //拆开以后 判断有没有数据
                        if (System.Int32.Parse(result[3], System.Globalization.NumberStyles.HexNumber) > 0)
                        {
                            //循环 直线 导通
                            for (int i = 4; i < System.Int32.Parse(result[3], System.Globalization.NumberStyles.HexNumber) + 4; i++)
                            {
                                if (result[i].Contains("?"))
                                {
                                    continue;
                                }
                                string start = ((System.Int32.Parse(result[2], System.Globalization.NumberStyles.HexNumber) - 1) * 64 + System.Int32.Parse(result[i].Substring(0, 2), System.Globalization.NumberStyles.HexNumber)).ToString("X4");
                                string end = result[i].Substring(2, 4);
                                //把连接点添加到 一个集合中去

                                TestConnectionPoint testConnectionPoint = new TestConnectionPoint();
                                testConnectionPoint.startPoint = start;
                                testConnectionPoint.endPoint = end;
                                testConnectionPoints.Add(testConnectionPoint);

                            }

                            //循环  二极管 导通 第一个点是系统点  第二个点 板卡点  并且有方向
                            int PointerD1 = Array.IndexOf(result, "##");
                            int PointerD2 = Array.IndexOf(result, "!!");
                            if (PointerD1 >= 0 && PointerD2 >= 0 && (PointerD2 - PointerD1) > 1)
                            {
                                int DPointCount = PointerD2 - PointerD1 - 1;
                                int startFor = PointerD1 + 1;
                                for (int i = startFor; i < (startFor + DPointCount); i++)
                                {

                                    string start = result[i].Substring(0, 4);
                                    string end = ((System.Int32.Parse(result[2], System.Globalization.NumberStyles.HexNumber) - 1) * 64 + System.Int32.Parse(result[i].Substring(4, 4), System.Globalization.NumberStyles.HexNumber)).ToString("X4");

                                    //System.Diagnostics.Debug.WriteLine("二极管起始终止坐标:"+ start+end);

                                    //把连接点添加到 一个集合中去

                                    TestConnectionPoint testConnectionPoint = new TestConnectionPoint();
                                    testConnectionPoint.startPoint = start;
                                    testConnectionPoint.endPoint = end;
                                    testConnectionDPoints.Add(testConnectionPoint);
                                }

                            }
                            //循环 电压值  先判断 有没有电压值
                            int Pointer1 = Array.IndexOf(result, "!!");
                            int Pointer2 = Array.IndexOf(result, "98");
                            if (Pointer1 >= 0 && Pointer2 >= 0 && (Pointer2 - Pointer1) > 1)
                            {
                                int VoltagePointCount = Pointer2 - Pointer1 - 1;
                                int startFor = Pointer1 + 1;
                                for (int i = startFor; i < (startFor + VoltagePointCount); i++)
                                {

                                    string VoltagePoint = result[i].Substring(0, 4);
                                    string VoltageValue = result[i].Substring(5, 4);

                                    //System.Diagnostics.Debug.WriteLine("电阻坐标点和电压值:" + VoltagePoint + VoltageValue);

                                    double VoltageFloatValue = 0;
                                    if (double.TryParse(VoltageValue, out VoltageFloatValue) == false) //判断是否可以转换为float
                                    {
                                        VoltageFloatValue = 0;
                                    }
                                    TestVoltagePoint testVoltagePoint = new TestVoltagePoint();
                                    testVoltagePoint.TestPoint = VoltagePoint;
                                    testVoltagePoint.VoltageValue = VoltageFloatValue;
                                    testVoltagePoints.Add(testVoltagePoint);
                                }

                            }
                            //以上把直线  二极管  电压 全部取到一个集合中







                        }//没有数据就不判断了


                    }
                    testUI.TestConnectionPoints = testConnectionPoints;
                    testUI.TestConnectionDPoints = testConnectionDPoints;
                    testUI.TestVoltagePoints = testVoltagePoints;
                    Thread.Sleep(2);
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new ModifyButton_dg(ModifyButton), testUI);



                }
                //System.Diagnostics.Debug.WriteLine(serialPort_read);
            }
            catch (Exception ex)
            {


                if (longTask != null)
                {

                    cts.Cancel();
                    //  System.Diagnostics.Debug.WriteLine("结束测试任务状态 : " + longTask.Status);

                }
                Thread.Sleep(100);
                mre.Set();
                MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                logNet.WriteDebug("通信 error", ex.Message);
                logNet.WriteDebug("data ", serialPort_read);
                logNet.WriteDebug("----------");


            }
        }

        //第一步：修改UI 进行消线的处理
        private void ModifyButton(TestUI testUI)
        {
            //第一块板卡 清空短路线
            if (testUI.WhichBanka == 1)
            {
                //要执行的代码逻辑
                if (connectionAdd.Count > 0)
                {

                    //System.Diagnostics.Debug.WriteLine("START01");
                    for (int i = connectionAdd.Count - 1; i >= 0; i--)
                    {

                        //connection.
                        if (MyDesigner2.Children.Contains(connectionAdd[i]))
                            MyDesigner2.Children.Remove(connectionAdd[i]);

                    }
                    connectionAdd.Clear();

                }

                if (shortPoints.Count > 0)
                {
                    shortPoints.Clear();
                }
                //第一块板卡的时候 把电阻值给清空
                for (int i = 0; i < voltageClassSum.Count; i++)
                {
                    voltageClassSum[i].SmallPointValue = 0;
                    List<TestVoltagePoint> bigPoints = voltageClassSum[i].BigPoints;
                    for (int j = 0; j < bigPoints.Count; j++)
                    {
                        bigPoints[j].VoltageValue = 0;
                    }

                }


            }
            //开始消电阻  收集每块板卡的电压值  最后一块板卡 才能电阻消线
            if (testUI.TestVoltagePoints.Count > 0)
            {
                for (int i = 0; i < testUI.TestVoltagePoints.Count; i++)
                {
                    string testPoint = testUI.TestVoltagePoints[i].TestPoint;
                    double voltageValue = testUI.TestVoltagePoints[i].VoltageValue;

                    if (voltageClassSum.Count > 0)
                    {
                        for (int m = 0; m < voltageClassSum.Count; m++)
                        {
                            if (voltageClassSum[m].SmallPoint == testPoint)
                            {
                                voltageClassSum[m].SmallPointValue = voltageValue;
                            }

                            List<TestVoltagePoint> bigPoints = voltageClassSum[m].BigPoints;
                            for (int j = 0; j < bigPoints.Count; j++)
                            {
                                if (bigPoints[j].TestPoint == testPoint)
                                {
                                    bigPoints[j].VoltageValue = voltageValue;
                                }

                            }


                        }


                    }
                }
                

            }

            //消电阻  这边只消电阻   和导通线没有关系 电阻在主干 方向是0  电阻在分支 方向是1
            //一坨搭点内有电阻  电阻在主干 方向是0  电阻在分支 方向是1
            if (testUI.WhichBanka == bankaQty)
            {

                //判断两侧电压是不齐全了   齐全了  开始计算
                List<VoltageClass> vol01 = voltageClassSum.Where(s => (s.VisibleFlag == true)).ToList();
                if (vol01 != null && vol01.Count > 0)
                {
                    foreach (VoltageClass itemvol01 in vol01)
                    //for (int n = 0; n < vol01.Count; n++)
                    {

                        //VoltageClass voltageClass = itemvol01;
                        List<double> listValue = new List<double>();
                        List<TestVoltagePoint> bigPoints = itemvol01.BigPoints;
                        double pingjunsum = 0;
                        for (int j = 0; j < bigPoints.Count; j++)
                        {
                            if (bigPoints[j].VoltageValue > 0.5)
                            {
                                pingjunsum = pingjunsum + bigPoints[j].VoltageValue;
                                listValue.Add(bigPoints[j].VoltageValue);
                            }

                        }
                        double testRValue = 0;
                        if (listValue.Count > 0)
                        {
                            double pingjun = pingjunsum / listValue.Count;
                           
                            if (itemvol01.RDirection == "0")
                            {
                                testRValue = (pingjun-itemvol01.SmallPointValue ) / (itemvol01.SmallPointValue) * 1000;
                            }
                            else
                            {
                                testRValue = (itemvol01.SmallPointValue - pingjun) / (pingjun * listValue.Count) * 1000;
                            }
                           
                           
                        }


                        double xiavalue = itemvol01.RDValue - itemvol01.RDValue * 0.1;
                        double shangvalue = itemvol01.RDValue + itemvol01.RDValue * 0.1;

                        //System.Diagnostics.Debug.WriteLine("计算电阻:" + testRValue+"  " +xiavalue+"  "+ shangvalue);
                        //如果存在这跟线  就把他类型为R的线消失掉  为什么类型为R  因为 还有类型为RD  既有电阻 还有二极管的
                        if (testRValue <= shangvalue && testRValue >= xiavalue)
                        {
                            List<Connection> r01 = connectionContainRs.Where(s => (s.StartPoint == itemvol01.SmallPoint && s.EndPoint == itemvol01.BigPoint && s.Visibility == Visibility.Visible)).ToList(); //筛选//电阻
                            if (r01 != null && r01.Count >= 1)  //存在直线
                            {

                                if (itemvol01.RDType == "R")
                                {
                                    if (r01[0].Visibility == Visibility.Visible)
                                    {
                                        r01[0].Visibility = Visibility.Hidden;

                                    }
                                }
                            }
                            itemvol01.VisibleFlag = false;
                            valueShow_txt.Text = itemvol01.SmallPoint + "-" + itemvol01.BigPoint + ":" + Convert.ToInt32(testRValue).ToString();
                        }

                        else
                        {
                            valueShow_txt.Text = itemvol01.SmallPoint + "-" + itemvol01.BigPoint + ":" + Convert.ToInt32(testRValue).ToString();
                        }
                    }
                }

            }
            //消电阻



          // MessageBox.Show(Newtonsoft.Json.JsonConvert.SerializeObject(testUI.TestConnectionPoints));
            //开始消线
            if (testUI.TestConnectionPoints.Count > 0)
            {

                int suoShowFalg = 0;
                //for (int i = 0; i < testUI.TestConnectionPoints.Count; i++)
                foreach (TestConnectionPoint itemTestConnectionPoints in testUI.TestConnectionPoints)
                {
                    string start = itemTestConnectionPoints.startPoint;
                    string end = itemTestConnectionPoints.endPoint;
                    //1. 在锁集合
                    List<SuoClass> suo01 = suoClassSum.Where(s => ((s.StartPoint == start && s.EndPoint == end) || (s.StartPoint == end && s.EndPoint == start))).ToList(); //筛选
                    if (suo01 != null && suo01.Count > 0)
                    {
                        //要执行的代码逻辑
                        suo01[0].VisibleFlag = false;
                        suoShowFalg = 1;
                    }
                    else //不在锁集合 2.直线
                    {
                        List<Connection> c01 = connectionSum.Where(s => ((s.StartPoint == start && s.EndPoint == end) || (s.StartPoint == end && s.EndPoint == start)) && s.RDType == "L").ToList(); //筛选
                        if (c01.Count >= 1)  //存在 直线
                        {
                            //判断不是右侧集合点
                            bool a = false;
                            bool b = false;
                            foreach (VoltageClass itemRR in voltageClassSum)
                            {
                                List<TestVoltagePoint> bigPoints = itemRR.BigPoints;
                                
                                foreach (TestVoltagePoint tt in bigPoints)
                                {
                                    if (tt.TestPoint ==start)
                                    {
                                        a = true;
                                    }
                                    if (tt.TestPoint == end)
                                    {
                                        b = true;
                                    }
                                }
                            }
                            //单向
                            if (c01[0].Visibility == Visibility.Visible &&  a==false &&  b==false)
                            {
                                c01[0].Visibility = Visibility.Hidden;
                            }


                        }
                        else   //不在直线集合 3.存在某一坨电阻中
                        {
                            if (this.judgeInRCollections(start, end) &&  !judgeInMutilLines(start, end)) //存在一坨电阻中 且不在多线内 就是左右两个组合中某两个点 
                            {
                                
                                List<Connection> r02 = connectionContainRs.Where(s => s.RDType == "R").ToList();
                                if (r02.Count > 0)
                                {
                                    for (int j = 0; j < r02.Count; j++)
                                    {

                                        if (r02[j].Visibility == Visibility.Hidden)
                                        //为什么要等到电阻的线消失以后才能消直线呢 AB消失以后 才能说明 左右两端的各自小点才是通的
                                        {


                                            List<Connection> rightAllVis = connectionSum.Where(s => s.StartPoint == r02[j].EndPoint && s.RDType == "L" && s.Visibility == Visibility.Visible).ToList(); //筛选
                                            //右端的线
                                            List<string> leftPoints = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(r02[j].LeftPoints);
                                            List<string> rightPoints = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(r02[j].RightPoints);
                                            //正向

                                            foreach (string lft in leftPoints)
                                            {
                                                if (rightAllVis.Count == 0) break;

                                                foreach (string rps in rightPoints)
                                                {
                                                    if ((lft + rps) == (start + end) || (lft + rps) == (end + start))
                                                    {
                                                        List<Connection> c03 = connectionSum.Where(s => ((s.StartPoint == rightPoints[0] && s.EndPoint == rps) || (s.StartPoint == rps && s.EndPoint == rightPoints[0])) && s.RDType == "L").ToList(); //筛选&& s.Visibility == Visibility.Visible
                                                        if (c03.Count > 0)  //存在 直线
                                                        {
                                                            //单向
                                                            foreach (Connection c03item in c03)
                                                            {
                                                                double rightPointsRValue = 0;
                                                                double rpsRValue = 0;
                                                                double chazhia = 0;
                                                                foreach (VoltageClass itemRR in voltageClassSum)
                                                                {
                                                                    List<TestVoltagePoint> bigPoints = itemRR.BigPoints;
                                                                    foreach (TestVoltagePoint tt in bigPoints)
                                                                    {
                                                                        if (tt.TestPoint == rightPoints[0])
                                                                        {
                                                                            rightPointsRValue = tt.VoltageValue;
                                                                        }
                                                                        if (tt.TestPoint == rps)
                                                                        {
                                                                            rpsRValue = tt.VoltageValue;
                                                                        }
                                                                    }
                                                                }
                                                                chazhia = rightPointsRValue - rpsRValue;

                                                                if (rpsRValue > 0.3 && (chazhia > -0.2 &&  chazhia < 0.2))
                                                                {
                                                                    if (c03item.Visibility == Visibility.Visible)
                                                                        c03item.Visibility = Visibility.Hidden;
                                                                }
                                                                else
                                                                {
                                                                    c03item.Visibility = Visibility.Visible;
                                                                }


                                                            }



                                                        }

                                                    }

                                                }//正向

                                            }



                                        }
                                    }//for



                                }
                            }
                            else //不在一坨电阻中 4.存在某一坨二极管中
                            {
                                if (!judgeInDCollections(start, end))//不在二极管集合中 5 有可能是一坨搭点啊  
                                {
                                    if (!judgeInMutilLines(start, end))//不在一坨搭点中6.短路 下位机 直线点如果出现在二极管中 不处理  不出现 那就是短路了
                                    {


                                        //短路
                                        //和这个点相关联的 线 再重新显示出来
                                        List<Connection> c05 = (from item in this.connectionSum
                                                                where (item.StartPoint == start || item.EndPoint == end || item.StartPoint == end || item.EndPoint == start) && !item.RDType.Contains("R")
                                                                select item).ToList();
                                        if (c05.Count > 0)   //显示出来
                                        {
                                            
                                                foreach(Connection itemc05 in c05)
                                                {
                                                    itemc05.Visibility = Visibility.Visible;
                                                }
                                                    
                                            

                                        }
                                        //画短路线
                                        List<DesignerItem> d1 = (from item in this.designerItemSum
                                                                 where item.ZuoBiaoName == start || item.ZuoBiaoName == end
                                                                 select item).ToList();

                                        if (d1.Count == 2)
                                        {

                                            //和这两个点
                                            Connector sourceConnector = MyDesigner2.GetConnector(d1[0].ID, "Center");
                                            Connector sinkConnector = MyDesigner2.GetConnector(d1[1].ID, "Center");

                                            Connection newConnection = new Connection(sourceConnector, sinkConnector);

                                            newConnection.StartPoint = start;
                                            newConnection.EndPoint = end;
                                            newConnection.PathGeometry = new PathGeometry();
                                            PathFigure figure = new PathFigure();
                                            figure.StartPoint = sourceConnector.Position;
                                            figure.Segments.Add(new LineSegment(sinkConnector.Position, true));
                                            newConnection.PathGeometry.Figures.Add(figure);
                                            Canvas.SetZIndex(newConnection, MyDesigner2.Children.Count);
                                            MyDesigner2.Children.Add(newConnection);
                                            connectionAdd.Add(newConnection);
                                            newConnection.ApplyTemplate();
                                            System.Windows.Shapes.Path cd = newConnection.Template.FindName("PART_ConnectionPath", newConnection) as System.Windows.Shapes.Path;
                                            cd.Stroke = new SolidColorBrush(Colors.Green);

                                        }
                                        else
                                        {
                                            string str0001 = start + end;
                                            string str0002 = end + start;
                                            if (!shortPoints.Contains(str0001) && !shortPoints.Contains(str0002))
                                            {
                                                shortPoints.Add(str0001);
                                                suo_txt.Text = (start + end) + "短路 " + suo_txt.Text;
                                            }



                                        }

                                        //短路

                                    }//不在一坨搭点中6.短路 下位机 直线点如果出现在二极管中 不处理  不出现 那就是短路了

                                }

                            }//不在直线集合 4.存在某一坨二极管中




                        } //不在直线集合 3.存在某一坨电阻中



                    }//不在锁集合 2.直线


                }

                //显示锁 看是不是
                if (suoShowFalg == 1)
                {
                    suo_txt.Text = "";
                    if (shortPoints.Count != 0)
                    {
                        suo_txt.Text = (shortPoints[0] + "短路 ") + suo_txt.Text;
                    }
                    List<SuoClass> suo02 = suoClassSum.Where(s => (s.VisibleFlag == true)).ToList();
                    if (suo02.Count > 0)
                    {
                        foreach (SuoClass itemsuo02 in suo02)
                        {
                            suo_txt.Text = suo_txt.Text + " " + Convert.ToString(itemsuo02.SuoName);
                        }




                    }

                }



            }//开始消线

            //开始消二极管

            if (testUI.TestConnectionDPoints.Count > 0)
            {

                foreach (TestConnectionPoint itemTestConnectionDPoints in testUI.TestConnectionDPoints)
                //for (int i = 0; i < testUI.TestConnectionDPoints.Count; i++)
                {

                    string start = itemTestConnectionDPoints.startPoint;
                    string end = itemTestConnectionDPoints.endPoint;
                    //二极管中的直线
                    List<Connection> c05 = searchHowManyContainDsConnections.Where(s => ((s.StartPoint == end && s.EndPoint == start) || (s.StartPoint == start && s.EndPoint == end)) && s.RDType == "L" && s.Visibility == Visibility.Visible).ToList(); //筛选
                    if (c05.Count >= 1)  //存在 直线
                    {

                        if (c05[0].Visibility == Visibility.Visible)
                        {
                            c05[0].Visibility = Visibility.Hidden;

                        }
                    }
                    //二极管集合 二极管
                    List<Connection> c06 = searchHowManyContainDsConnections.Where(s => ((s.StartPoint == start && s.EndPoint == end && s.RDDirection == "1") || (s.StartPoint == end && s.EndPoint == start && s.RDDirection == "0")) && s.RDType == "D" && s.Visibility == Visibility.Visible).ToList(); //筛选

                    if (c06.Count >= 1)  //存在 直线
                    {
                        foreach (Connection item06 in c06)
                        {
                            item06.Visibility = Visibility.Hidden;
                        }

                    }

                    //二极管集合中的  二极管加电阻
                    List<Connection> c07 = searchHowManyContainDsConnections.Where(s => ((s.StartPoint == start && s.EndPoint == end && s.RDDirection == "1") || (s.StartPoint == end && s.EndPoint == start && s.RDDirection == "0")) && s.RDType == "RD" && s.Visibility == Visibility.Visible).ToList(); //筛选
                    List<VoltageClass> vol02 = voltageClassSum.Where(s => (s.VisibleFlag == false && ((s.SmallPoint == start && s.BigPoint == end) || (s.SmallPoint == end && s.BigPoint == start)))).ToList();
                    if (c07.Count >= 1 && vol02.Count > 0)  //存在 直线
                    {
                        foreach (Connection item07 in c07)
                        {

                            item07.Visibility = Visibility.Hidden;

                           
                        }
                           
                    }

                }//所有二极管点集合


            }
            //end开始消二极管


            //判断是不是成功了
            //要执行的代码逻辑
            //判断线是否全部消掉

            List<Connection> c2 = connectionSum.Where(s => (s.Visibility == Visibility.Visible)).ToList(); //筛选

            //判断锁是不全部消掉
            List<SuoClass> suo03 = suoClassSum.Where(s => (s.VisibleFlag == true)).ToList();
            //判断 电阻  有没有全部消掉
            List<Connection> C03 = connectionContainRs.Where(s => (s.Visibility == Visibility.Visible)).ToList();
            //判断 二极管  有没有全部消掉
            List<Connection> C04 = this.searchHowManyContainDsConnections.Where(s => (s.Visibility == Visibility.Visible)).ToList();

            if (c2.Count == 0 && connectionAdd.Count == 0 && suo03.Count == 0 && C03.Count == 0 && C04.Count == 0 && shortPoints.Count == 0)
            {

                suo_txt.Text = "";
                _serialPort1.Write("$Cmd,10,00,98*"); //要电压
                //执行的方法体完成回调，可以为null
                if (longTask != null)
                {

                    cts.Cancel();
                    //System.Diagnostics.Debug.WriteLine("测试成功任务状态 : " + longTask.Status);

                }
                Thread.Sleep(100);
                //将发送数据的线程打开 取消掉
                mre.Set();
                //保存数据
                SaveData sd = new SaveData(); 
                sd.insertEx("insert into testRecord (BarCode,Fmodel,Remark,UserName,Infomation,CheckTime) values ('" + currentBarcode + "','" + filePath_lab.Content + "','-1','" + userNameSecutity.Split(',')[0] + "','合格','"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')");
                sd.closeConn();
                sd.Dispose();
                //发送开关信号的信息
                //if (_serialPortSwitch!=null   &&      _serialPortSwitch.IsOpen)
                //{

                //    string writeString = "FF06000800141DD9";  //两秒
                //    byte[] a = HexStringToByteArray(writeString);
                //    //MessageBox.Show(ByteArrayToHexString(a));
                //    _serialPortSwitch.Write(a, 0, a.Length);
                //    Thread.Sleep(200);
                //}

                //开始弹框 
                WaitingBox.WaitShow(this,
                         () =>
                         {
                             System.Threading.Thread.Sleep(1300);//需要执行的方法体（需要自己做异常处理）
                         },
                         null,
                         "检 测 合 格！请继续...");


                if (scanModel == "扫描规格")
                {
                    FModelWindow fmodel = new FModelWindow(MyDesigner2.fmodelInsert, fmodelLength);
                    if (fmodel.ShowDialog() == true)
                    {
                        FNumberWindow fnumber = new FNumberWindow(fbarcodeLength, fbarcodeFront);
                        if (fnumber.ShowDialog() == true)
                        {

                            currentBarcode = fnumber.fnumberTxb.Text;
                            //全部还原
                            refreshAll();

                            cts = new CancellationTokenSource();
                            longTask = Task.Factory.StartNew(() => trmain("Task0", bankaQty, cts.Token), cts.Token);



                        }
                        else
                        {
                            //全部还原
                            refreshAll();
                        }
                        fnumber.Close();
                    }
                    else
                    {
                        //全部还原
                        refreshAll();
                    }
                    fmodel.Close();
                }
                else if (scanModel == "扫描条码")
                {
                    FNumberWindow fnumber = new FNumberWindow(fbarcodeLength, fbarcodeFront);
                    if (fnumber.ShowDialog() == true)
                    {
                        currentBarcode = fnumber.fnumberTxb.Text;

                        //全部还原
                        refreshAll();
                        cts = new CancellationTokenSource();
                        longTask = Task.Factory.StartNew(() => trmain("Task0", bankaQty, cts.Token), cts.Token);



                    }
                    else
                    {
                        //全部还原
                        refreshAll();
                    }

                    fnumber.Close();
                }
                else
                {
                    //全部还原
                    refreshAll();
                    Thread.Sleep(1200);
                    cts = new CancellationTokenSource();
                    longTask = Task.Factory.StartNew(() => trmain("Task0", bankaQty, cts.Token), cts.Token);

                    //System.Diagnostics.Debug.WriteLine(longTask.Status);
                }

            }
            else//如没有测试成功 
            {
                if ((c2.Count != 0 || connectionAdd.Count != 0 || C04.Count != 0) && MemoryModel == "弃用")
                {
                    //直线还原
                    foreach (Connection item in this.connectionSum)
                    {
                        item.Visibility = Visibility.Visible;
                    }
                    //二极管   全部还原
                    foreach (Connection item in this.searchHowManyContainDsConnections)
                    {
                        item.Visibility = Visibility.Visible;
                    }

                }

                if (shortBreak == "必须" && (connectionAdd.Count != 0 || shortPoints.Count != 0))
                {

                    //执行的方法体完成回调，可以为null
                    if (longTask != null)
                    {

                        cts.Cancel();
                        // System.Diagnostics.Debug.WriteLine("测试成功任务状态 : " + longTask.Status);

                    }
                    Thread.Sleep(100);
                    //将发送数据的线程打开 取消掉
                    mre.Set();

                    string strartEnd = string.Empty;
                    if (connectionAdd.Count != 0)
                    {
                        strartEnd = "短路:" + connectionAdd[0].StartPoint + connectionAdd[0].EndPoint;
                    }
                    else if (shortPoints.Count != 0)
                    {
                        strartEnd = "短路:" + shortPoints[0];
                    }

                    SaveData sd = new SaveData();
                    sd.insertEx("insert into testRecord (BarCode,Fmodel,Remark,UserName,Infomation,Infomation2,CheckTime) values ('" + currentBarcode + "','" + filePath_lab.Content + "','0','" + userNameSecutity.Split(',')[0] + "','测试不合格','" + strartEnd + "', '"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' )");
                    sd.closeConn();
                    sd.Dispose();

                    //if (_serialPortSwitch.IsOpen)
                    //{

                    //    string writeString = "FF060008000A9DD1";
                    //    byte[] a = HexStringToByteArray(writeString);
                    //    //MessageBox.Show(ByteArrayToHexString(a));
                    //    _serialPortSwitch.Write(a, 0, a.Length);
                    //}

                    WaitingBox.WaitShow(this,
                             () =>
                             {
                                 System.Threading.Thread.Sleep(1300);//需要执行的方法体（需要自己做异常处理）
                             },
                             null,
                             "警告！导通不良,检测不合格！请检查...");

                    //弹出一个框 输入密码后才能解锁
                    ShortLockForm shortLock = new ShortLockForm();
                    shortLock.ShowDialog();

                    //弹出一个框 输入密码后才能解锁
                    if (scanModel == "扫描规格")
                    {
                        FModelWindow fmodel = new FModelWindow(MyDesigner2.fmodelInsert, fmodelLength);
                        if (fmodel.ShowDialog() == true)
                        {
                            FNumberWindow fnumber = new FNumberWindow(fbarcodeLength, fbarcodeFront);
                            if (fnumber.ShowDialog() == true)
                            {
                                currentBarcode = fnumber.fnumberTxb.Text;

                                //全部还原
                                refreshAll();
                                cts = new CancellationTokenSource();
                                longTask = Task.Factory.StartNew(() => trmain("Task0", bankaQty, cts.Token), cts.Token);



                            }
                            else
                            {
                                //全部还原
                                refreshAll();
                            }
                            fnumber.Close();
                        }
                        else
                        {
                            //全部还原
                            refreshAll();
                        }
                        fmodel.Close();
                    }
                    else if (scanModel == "扫描条码")
                    {
                        FNumberWindow fnumber = new FNumberWindow(fbarcodeLength, fbarcodeFront);
                        if (fnumber.ShowDialog() == true)
                        {
                            currentBarcode = fnumber.fnumberTxb.Text;
                            //全部还原
                            refreshAll();
                            cts = new CancellationTokenSource();
                            longTask = Task.Factory.StartNew(() => trmain("Task0", bankaQty, cts.Token), cts.Token);

                            // System.Diagnostics.Debug.WriteLine(longTask.Status);

                        }
                        else
                        {
                            //全部还原
                            refreshAll();
                        }

                        fnumber.Close();
                    }
                    else
                    {
                        currentBarcode = string.Empty;
                        //全部还原
                        refreshAll();
                        Thread.Sleep(1200);
                        cts = new CancellationTokenSource();
                        longTask = Task.Factory.StartNew(() => trmain("Task0", bankaQty, cts.Token), cts.Token);

                        //System.Diagnostics.Debug.WriteLine(longTask.Status);
                    }
                }
                else
                {
                    //将发送数据的线程打开 继续要数据

                    mre.Set();
                }





            }
            //System.Diagnostics.Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(testUI.WhichBanka));
            //System.Diagnostics.Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(testUI.TestConnectionPoints));
            //System.Diagnostics.Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(testUI.TestConnectionDPoints));
            //System.Diagnostics.Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(testUI.TestVoltagePoints));




        }
        //第二步：声明第一步方法的委托
        private delegate void ModifyButton_dg(TestUI testUI);

        //第三步：调用委托
        /*
        private void Calldelgate( ){ 
        在Windows窗体应用程序中使用this.Invoke 在WPF应用程序中使用this.Dispatcher.Invoke
        this.Invoke( new ModifyButton_dg( ModifyButton ) ,new object[]{false});
        }
        */










        private void startTest_menuItem_Click(object sender, RoutedEventArgs e)
        {
            zongtanMenuItem.IsEnabled = false;

            if (scanModel == "扫描规格")
            {
                FModelWindow fmodel = new FModelWindow(MyDesigner2.fmodelInsert, fmodelLength);
                if (fmodel.ShowDialog() == true)
                {
                    //调用图号

                    FNumberWindow fnumber = new FNumberWindow(fbarcodeLength, fbarcodeFront);
                    if (fnumber.ShowDialog() == true)
                    {
                        currentBarcode = fnumber.fnumberTxb.Text;
                        //全部还原
                        refreshAll();
                        cts = new CancellationTokenSource();
                        longTask = Task.Factory.StartNew(() => trmain("Task0", bankaQty, cts.Token), cts.Token);

                        //System.Diagnostics.Debug.WriteLine(longTask.Status);

                    }
                    else
                    {
                        //全部还原
                        refreshAll();
                    }

                    fnumber.Close();
                }
                else
                {
                    //全部还原
                    refreshAll();
                }
                fmodel.Close();
            }
            else if (scanModel == "扫描条码")
            {
                FNumberWindow fnumber = new FNumberWindow(fbarcodeLength, fbarcodeFront);
                if (fnumber.ShowDialog() == true)
                {
                    currentBarcode = fnumber.fnumberTxb.Text;

                    //全部还原
                    refreshAll();
                    cts = new CancellationTokenSource();
                    longTask = Task.Factory.StartNew(() => trmain("Task0", bankaQty, cts.Token), cts.Token);

                    // System.Diagnostics.Debug.WriteLine(longTask.Status);

                }
                else
                {
                    //全部还原
                    refreshAll();
                }

                fnumber.Close();
            }
            else
            {
                currentBarcode = string.Empty;
                //全部还原
                refreshAll();
                Thread.Sleep(1200);
                cts = new CancellationTokenSource();
                longTask = Task.Factory.StartNew(() => trmain("Task0", bankaQty, cts.Token), cts.Token);


            }
            startTest_menuItem.IsEnabled = false;
            openFile_MenuItem.IsEnabled = false;

            stopTest_menuItem.IsEnabled = true;
        }

        private void stopTest_menuItem_Click(object sender, RoutedEventArgs e)
        {

            if (longTask != null)
            {

                cts.Cancel();
                //  System.Diagnostics.Debug.WriteLine("结束测试任务状态 : " + longTask.Status);

            }
            //将发送数据的线程打开 取消掉
            mre.Set();

            startTest_menuItem.IsEnabled = true;
            zongtanMenuItem.IsEnabled = true;

        }

        private void refreshAll()
        {
            mre.Reset();

            //直线还原
            foreach (Connection item in this.connectionSum)
            {
                item.Visibility = Visibility.Visible;
            }
            //值显示  清空
            valueShow_txt.Text = "";
            //锁全部还原
            suo_txt.Text = "";
            foreach (SuoClass item in this.suoClassSum)
            {
                item.VisibleFlag = true;

                suo_txt.Text += item.SuoName + "  ";
            }

            //电阻   全部还原
            foreach (Connection item in this.connectionContainRs)
            {
                item.Visibility = Visibility.Visible;
            }
            foreach (VoltageClass item in this.voltageClassSum)
            {
                item.SmallPointValue = 0;
                item.VisibleFlag = true;
                item.ISConnection = true;
                List<TestVoltagePoint> list = item.BigPoints;
                if (list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].VoltageValue = 0;
                    }
                }
            }
            //二极管   全部还原
            foreach (Connection item in this.searchHowManyContainDsConnections)
            {
                item.Visibility = Visibility.Visible;
            }
            //短路信息 清空

            //要执行的代码逻辑
            if (connectionAdd.Count > 0)
            {

                //System.Diagnostics.Debug.WriteLine("START01");
                for (int i = connectionAdd.Count - 1; i >= 0; i--)
                {

                    //connection.
                    if (MyDesigner2.Children.Contains(connectionAdd[i]))
                        MyDesigner2.Children.Remove(connectionAdd[i]);

                }
                connectionAdd.Clear();

            }

            if (shortPoints.Count > 0)
            {
                shortPoints.Clear();
            }
            //第一块板卡的时候 把电阻值给清空
            for (int i = 0; i < voltageClassSum.Count; i++)
            {
                voltageClassSum[i].SmallPointValue = 0;
                List<TestVoltagePoint> bigPoints = voltageClassSum[i].BigPoints;
                for (int j = 0; j < bigPoints.Count; j++)
                {
                    bigPoints[j].VoltageValue = 0;
                }

            }





        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {

                if (longTask != null)
                {

                    cts.Cancel();


                }
                //将发送数据的线程打开 取消掉
                mre.Set();
                if (_serialPort1 != null && _serialPort1.IsOpen)
                {
                    Thread.Sleep(2000);

                    _serialPort1.Close();

                    if (_serialPortSwitch != null && _serialPortSwitch.IsOpen)
                    {
                        _serialPortSwitch.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }



        //知道一条连线  找出所有相关的连线
        private void searchHowManyConnection(Connection conn)
        {

            searchHowManyContainDsConnections.Add(conn);

            var connections = this.MyDesigner2.Children.OfType<Connection>().Where(item => (item.StartPoint == conn.StartPoint || item.EndPoint == conn.EndPoint || item.StartPoint == conn.EndPoint || item.EndPoint == conn.StartPoint)).ToList();
            var exp = connections.Where(a => !searchHowManyContainDsConnections.Exists(t => (a.ID.ToString()).Contains(t.ID.ToString()))).ToList();



            if (exp.Count > 0)
            {
                foreach (Connection itemexp in exp)
                //for (int i = 0; i < exp.Count; i++)
                {
                    searchHowManyContainDsConnections.Add(itemexp);
                    searchHowManyConnection(itemexp);
                }




            }
            else
            {
                return;

            }


        }
        //给出两个坐标点  判断在不在电阻的  左右两点中

        private bool judgeInRCollections(string point1, string point2)
        {
            bool a = false;
            List<Connection> r02 = connectionContainRs.Where(s => s.RDType == "R").ToList();
            if (r02.Count > 0)
            {
                foreach (Connection itemr02 in r02)
                //for (int j = 0; j < r02.Count; j++)
                {
                    Connection c02 = itemr02;
                    List<string> leftPoints = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(c02.LeftPoints);
                    List<string> rightPoints = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(c02.RightPoints);
                    if ((leftPoints.Contains(point1) && rightPoints.Contains(point2)) || (leftPoints.Contains(point2) && rightPoints.Contains(point1)) || (leftPoints.Contains(point1) && leftPoints.Contains(point2)) || (rightPoints.Contains(point1) && rightPoints.Contains(point2)))
                    {
                        a = true;
                        break;
                    }

                }



            }
            return a;
        }
        //给出两个坐标点  判断在不在二极管集合中  
        private bool judgeInDCollections(string point1, string point2)
        {
            bool a = false;
            List<Connection> r02 = connectionContainDs.Where(s => s.RDType == "D" || s.RDType == "RD").ToList();
            if (r02.Count > 0)
            {
                foreach (Connection itemr02 in r02)
                //for (int j = 0; j < r02.Count; j++)
                {
                    Connection c02 = itemr02;
                    List<string> leftPoints = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(c02.LeftPoints);
                    List<string> rightPoints = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(c02.RightPoints);
                    if ((leftPoints.Contains(point1) && rightPoints.Contains(point2)) || (leftPoints.Contains(point2) && rightPoints.Contains(point1)) || (leftPoints.Contains(point1) && leftPoints.Contains(point2)) || (rightPoints.Contains(point1) && rightPoints.Contains(point2)))
                    {
                        a = true;
                        break;
                    }

                }
            }
            return a;
        }

        //判断字符串在不在 某个new List<List<string>>()
        private bool judgeInListstring(string p, List<List<string>> ls)
        {
            bool a = false;
            if (ls != null && ls.Count > 0)
            {
                foreach (List<string> itemls in ls)
                //for (int i = 0; i < ls.Count; i++)
                {
                    if (itemls.Contains(p))
                    {
                        a = true;
                        break;
                    }
                }
            }
            return a;


        }
        //判断 两个点在不在多线内
        private bool judgeInMutilLines(string point1, string point2)
        {
            bool a = false;

            if (connectionsMutilLine.Count > 0)
            {
                foreach (List<string> itemconnectionsMutilLine in connectionsMutilLine)
                //for (int j = 0; j < connectionsMutilLine.Count; j++)
                {


                    if (itemconnectionsMutilLine.Contains(point1) && itemconnectionsMutilLine.Contains(point2))
                    {
                        a = true;
                        break;
                    }

                }



            }
            return a;
        }



        private void uploadInfoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            UploadInfoWindow uploadInfoWindow = new UploadInfoWindow(userNameSecutity + "," + currentBarcode + "," + filePath_lab.Content);
            uploadInfoWindow.Show();
        }
        /// 把十六进制字符串转换成字节型(方法1)
        public byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }
        /// 字节数组转16进制字符串
        public string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();

        }
        //public void showWaitIco()
        //{
        //    waitIco.Visibility = Visibility.Visible;
        //}

        //public void hideWaitIco()
        //{
        //    waitIco.Visibility = Visibility.Collapsed;
        //}

        private void ZongtanMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _serialPort1.Write("$Cmd,11,00,98*"); //要电压
        }

        //mukuainame show 
        private void CommandBindingmukuaiNameShow_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBindingmukuaiNameShow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            List<DesignerItem> designerItems = MyDesigner2.Children.OfType<DesignerItem>().Where(s => s.Content.ToString().Contains("System.Windows.Controls.TextBlock")).ToList();
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

        
    }
}
//然后Ctrl+K,F