using HslCommunication.LogNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO.Ports;
using System.Linq;
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
using WireTestProgram.HelperClass;

namespace WireTestProgram.gasLock
{
    /// <summary>
    /// GasLockPinDian.xaml 的交互逻辑
    /// </summary>
    public partial class GasLockPinDian : MyMacClass
    {
        private static object objlock = new object();


        private List<TestConnectionPoint> li1 = new List<TestConnectionPoint> { };
        private List<TestConnectionPoint> li2 = new List<TestConnectionPoint> { };

        private ILogNet logNet = new LogNetSingle(System.Windows.Forms.Application.StartupPath + "\\Logs\\DesignLog.txt");
        //串口
        public System.IO.Ports.SerialPort _serialPort1;
        private string serialPort_read;
        //task同步
        public AutoResetEvent mre = new AutoResetEvent(false);
        //task
        Task longTask = null;
        CancellationTokenSource cts = null;
        //最后结果
        // private int successResult = 0;
        private LockStudy lc;
        private List<DesignerItem> designerItemSum;
        private List<DesignerItem> designerItemAllPoints;
        private string userName;
        private int bankaQty;
    
        public GasLockPinDian(LockStudy lc,List<DesignerItem> designerItemSum, string userName,  int bankaQty, List<DesignerItem> designerItemAllPoints)
        {
            InitializeComponent();
            this.lc = lc;
            this.designerItemSum = designerItemSum;
            this.designerItemAllPoints = designerItemAllPoints;
            this.userName = userName;
            this.bankaQty = bankaQty;
           

        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          //左侧模块名称列表
            DataTable tblDatas = new DataTable("Datas");
            DataColumn dc = null;
            dc = tblDatas.Columns.Add("mkName", Type.GetType("System.String"));
            DataRow newRow;
            for (int i = 0; i < designerItemSum.Count; i++)
            {
                newRow = tblDatas.NewRow();
                newRow["mkName"] = designerItemSum[i].MuKuaiName;
                tblDatas.Rows.Add(newRow);
            }
            DataView  dataView1= tblDatas.DefaultView;
           // dataView1.Sort = "CustomerID";
            lstProducts.ItemsSource = dataView1;
               

            ICollectionView view = CollectionViewSource.GetDefaultView(lstProducts.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("mkName", ListSortDirection.Ascending));

                try
                {
                    DBaccessHelp db = new DBaccessHelp();
                    DataTable dt = db.ExecuteQuery("select comport from setTable where setFlag='set' ");
                    db.closeOleDbConnection();

                    _serialPort1 = new System.IO.Ports.SerialPort();
                    _serialPort1.PortName = dt.Rows[0][0].ToString();
                    _serialPort1.BaudRate = 115200;
                    _serialPort1.Parity = System.IO.Ports.Parity.None;
                    _serialPort1.DataBits = 8;
                    _serialPort1.StopBits = StopBits.One;
                    _serialPort1.ReadTimeout = 500;
                    _serialPort1.WriteTimeout = 500;
                    _serialPort1.Open();
                    this._serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.post_DataReceived);



                }
                catch
                {
                    MessageBox.Show("串口名为空,无法打开!", "提示");
                }

            
            

        }
        //private Thread threadStarTest;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //List<string> a = li1.Distinct().ToList();
            //for (int i = 0; i < a.Count; i++)
            //{
            //    MessageBox.Show(a[i]);
            //}
               

            //开始执行要数据
            startzuobiaoName.Text = string.Empty;
            endzuobiaoName.Text = string.Empty;
            info_txt.Text = string.Empty;
            infocount_txt.Text = string.Empty;

            cts = new CancellationTokenSource();
            longTask = new Task(() => trmain("Task0", bankaQty, cts.Token), cts.Token);
            longTask.Start();

        }

//主线程处理数据

        private void ModifyButton(TestUI testUI)
        {
            //第一块板卡 清空短路线
            if (testUI.WhichBanka == 1)
            {
                //要执行的代码逻辑
                li1.Clear();
                li2.Clear();
            }
            if (testUI.TestConnectionPoints.Count > 0)
            {
                for (int i = 0; i < testUI.TestConnectionPoints.Count; i++)
                {
                    li1.Add(testUI.TestConnectionPoints[i]);
                    li2.Add(testUI.TestConnectionPoints[i]);
                }

            }
            //最后一个板卡 判断 
            
            if (testUI.WhichBanka == bankaQty)
            {
               
                if (li1.Count == 0)
                {
                    MessageBox.Show("没有数据！！！");
                }
                else if (li1.Count > 0)
                {

                    DBaccessHelp db2 = new DBaccessHelp();
                    DataTable suoOfTai = db2.ExecuteQuery("select  StartPoint, EndPoint   from SuoSource");
                    db2.closeOleDbConnection();



                    for (int i = li1.Count-1; i >=0; i--)
                        {
                            TestConnectionPoint testConnectionPoint = li1[i];
                        //System.Diagnostics.Debug.WriteLine(i.ToString()+"   "+testConnectionPoint.startPoint + "dada" + testConnectionPoint.endPoint);

                        if (suoOfTai!=null  && suoOfTai.Rows.Count > 0)
                            {
                                for (int j = 0; j < suoOfTai.Rows.Count; j++)
                                {

                                    if ((Convert.ToString(suoOfTai.Rows[j][0]) == testConnectionPoint.startPoint && Convert.ToString(suoOfTai.Rows[j][1]) == testConnectionPoint.endPoint) || (Convert.ToString(suoOfTai.Rows[j][0]) == testConnectionPoint.endPoint && Convert.ToString(suoOfTai.Rows[j][1]) == testConnectionPoint.startPoint))
                                    {
                                        li1.Remove(testConnectionPoint);
                                    }


                                }
                            }
                            //取出连线的物理坐标
                            if (designerItemAllPoints != null && designerItemAllPoints.Count > 0)
                            {
                                for (int j = 0; j < designerItemAllPoints.Count; j++)
                                {
                                    //if(testConnectionPoint.startPoint== Convert.ToString(connections[j].StartPoint)  || testConnectionPoint.startPoint== Convert.ToString(connections[j].EndPoint)  || testConnectionPoint.endPoint == Convert.ToString(connections[j].StartPoint) || testConnectionPoint.endPoint == Convert.ToString(connections[j].EndPoint) )
                                    if (testConnectionPoint.startPoint == Convert.ToString(designerItemAllPoints[j].ZuoBiaoName)  || testConnectionPoint.endPoint == Convert.ToString(designerItemAllPoints[j].ZuoBiaoName))

                                    {
                                        li1.Remove(testConnectionPoint);
                                    }


                                }


                            }




                            }



                    List<string> oldpoints = new List<string>();
                    for (int i = 0; i < li2.Count; i++)
                    {
                        oldpoints.Add(li2[i].startPoint + "," + li2[i].endPoint);
                    }

                    if (li1.Count > 0)
                    {
                        startzuobiaoName.Text = li1[0].startPoint;
                        endzuobiaoName.Text = li1[0].endPoint;
                    }
                    else
                    {

                        MessageBox.Show(Convert.ToString(Newtonsoft.Json.JsonConvert.SerializeObject(oldpoints))+" 当前数据已经在锁集合中存在！！！");
                    }

                    List<string> allpoints = new List<string>();
                    if (li1.Count > 0)
                    {
                        for (int i = 0; i < li1.Count; i++)
                        {
                            allpoints.Add(li1[i].startPoint + "," + li1[i].endPoint);
                        }
                    }
                   

                    info_txt.Text = Convert.ToString ( Newtonsoft.Json.JsonConvert.SerializeObject(allpoints) );
                    infocount_txt.Text = Convert.ToString(allpoints.Count);
                    infoall_txt.Text = Convert.ToString(Newtonsoft.Json.JsonConvert.SerializeObject(oldpoints));
                }


                //执行的方法体完成回调，可以为null
                if (longTask != null)
                {

                    cts.Cancel();
                    //System.Diagnostics.Debug.WriteLine("测试成功任务状态 : " + longTask.Status);

                }
                Thread.Sleep(100);
                //将发送数据的线程打开 取消掉
              
            }

            mre.Set();

        }

        //第二步：声明第一步方法的委托
        private delegate void ModifyButton_dg(TestUI testUI);

        //串口接收数据
        private void post_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                serialPort_read = string.Empty;
                //serialPort_read = _serialPort1.ReadExisting();
                serialPort_read = _serialPort1.ReadTo("*");


                TestUI testUI = new TestUI();
                testUI.WhichBanka = 0;
                List<TestConnectionPoint> testConnectionPoints = new List<TestConnectionPoint>();//直线    
                List<TestConnectionPoint> testConnectionDPoints = new List<TestConnectionPoint>();//二极管
                List<TestVoltagePoint> testVoltagePoints = new List<TestVoltagePoint>();//电阻
                //有逗号 就拆开
                if (serialPort_read.IndexOf("Error") > 0 && serialPort_read.IndexOf(",") > 0)
                {
                    string[] result = serialPort_read.Split(',');
                    testUI.WhichBanka = System.Int32.Parse(result[2], System.Globalization.NumberStyles.HexNumber);//第几块板卡
                    //拆开以后 判断有没有数据
                    if (System.Int32.Parse(result[3], System.Globalization.NumberStyles.HexNumber) > 0)
                    {
                        //所有有数据的点 都加到list 中
                        for (int i = 4; i < System.Int32.Parse(result[3], System.Globalization.NumberStyles.HexNumber) + 4; i++)
                        {
                            string start = ((System.Int32.Parse(result[2], System.Globalization.NumberStyles.HexNumber) - 1) * 64 + System.Int32.Parse(result[i].Substring(0, 2), System.Globalization.NumberStyles.HexNumber)).ToString("X4");
                            string end = result[i].Substring(2, 4);

                            TestConnectionPoint testConnectionPoint = new TestConnectionPoint();
                            testConnectionPoint.startPoint = start;
                            testConnectionPoint.endPoint = end;
                           
                            testConnectionPoints.Add(testConnectionPoint);


                        }
                    }//没有数据就不判断了



                }

                testUI.TestConnectionPoints = testConnectionPoints;
                testUI.TestConnectionDPoints = testConnectionDPoints;
                testUI.TestVoltagePoints = testVoltagePoints;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new ModifyButton_dg(ModifyButton), testUI);


            }
            catch (Exception ex)
            {



                mre.Set();
                MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                logNet.WriteDebug("通信 error", ex.Message);
                logNet.WriteDebug("----------");


            }
           // System.Diagnostics.Debug.WriteLine(serialPort_read);  
            
        }

      

        //task  执行函数体
        public void trmain(string name, int BanQty, CancellationToken token)
        {
            int loopFlag = BanQty + 1;


            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                for (int x = 0; x <= loopFlag; x++)
                {
                    _serialPort1.DiscardOutBuffer();
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    if (x == 0)
                    {

                        _serialPort1.Write("$Cmd,03," + BanQty.ToString("X2") + ",98*"); //要电压

                    }
                    else if (x == 1)
                    {

                        _serialPort1.Write("$Cmd,01," + BanQty.ToString("X2") + ",98*");
                    }
                    else if (x > 1)
                    {

                        _serialPort1.Write("$Cmd,3A," + (x - 1).ToString("X2") + ",98*");

                    }



                    mre.WaitOne(12000);

                }
            }


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //DialogResult = true;
            string str = returnType();
            if (string.IsNullOrEmpty(str))
            {
                MessageBox.Show("请选择锁的类型！");
                return;
            }
                if (!string.IsNullOrEmpty(startzuobiaoName.Text) && !string.IsNullOrEmpty(endzuobiaoName.Text) && !string.IsNullOrEmpty(mokuaiName.Text) && !string.IsNullOrEmpty(suoName.Text))
            {
                DBaccessHelp db0 = new DBaccessHelp();
                DataTable dt0 = db0.ExecuteQuery("select *   from  SuoSource where  SuoName='" + suoName.Text + "' or StartPoint='" + startzuobiaoName.Text + "' or  EndPoint='" +startzuobiaoName.Text + "' or StartPoint='" + endzuobiaoName.Text + "' or  EndPoint='" + endzuobiaoName.Text + "' ");
                db0.closeOleDbConnection();
                if (dt0.Rows.Count > 0)
                {
                    MessageBox.Show("警告，锁已经存在，请检查锁名称、起始坐标、终止坐标！！！");
                    return;
                }

                DBaccessHelp db = new DBaccessHelp();
                int s = db.insertEx("delete from  SuoSource where   StartPoint= '" +startzuobiaoName.Text + "' and  EndPoint='" +endzuobiaoName.Text + "'  ");
                db.closeOleDbConnection();

                DBaccessHelp db2 = new DBaccessHelp();
                int s2 = db2.insertEx("insert into SuoSource (MKName,SuoName, SuoType , StartPoint, EndPoint,DesignUser )    values  ( '" + mokuaiName.Text + "','" + suoName.Text + "','" + str + "' ,'" + startzuobiaoName.Text + "','" + endzuobiaoName.Text + "' ,'" + userName + "') ");
                db2.closeOleDbConnection();

                DBaccessHelp db3 = new DBaccessHelp();
                DataTable dt = db3.ExecuteQuery("select MKName,SuoName, SuoType , StartPoint, EndPoint,FStatus,DesignUser ,designDate   from  SuoSource order by SuoType ,MKName,SuoName ");
                db3.closeOleDbConnection();
                lc.gridMolds.ItemsSource = null;
                lc.gridMolds.ItemsSource = dt.DefaultView;
                if (s2 > 0)
                {
                    MessageBox.Show("保存成功");
                }
                else
                {
                    MessageBox.Show("保存失败");
                }
               

            }
            }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (_serialPort1.IsOpen) _serialPort1.Close();

           
            


        }

        private string returnType()
        {
            string str = string.Empty;
            if (mkRad.IsChecked == true)
            {
                str = "模块锁";
            }
            else if (qmRad.IsChecked == true)
            {
                str = "气密锁";
            }
            else if (kkRad.IsChecked == true)
            {
                str = "卡扣锁";
            }
            return str;
        }










    }
}
