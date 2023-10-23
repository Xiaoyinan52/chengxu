using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
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
using WireTestProgram.HelperClass;
using WireTestProgram.Register;

namespace WireTestProgram
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        //private HslCommunication.BasicFramework.SoftAuthorize softAuthorize = null;
        public Login()
        {
            InitializeComponent();
        }

        private void userName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                passWord.Clear();
                passWord.Focus();
            }
        }
        private void LoadHistroy()
        {
            string fileName = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"History.txt");
            if (File.Exists(fileName))
            {
                StreamReader reader = new StreamReader(fileName, Encoding.Default);
                string name = reader.ReadLine();
                while (name != null)
                {
                    //this.userName.AutoCompleteCustomSource.Add(name);
                    this.userName.Items.Add(name);
                    name = reader.ReadLine();
                }
                reader.Close();
            }

        }



        private int UserInputCheck()
        {
            // 保存登录名称
            string loginName = this.userName.Text.Trim();
            // 保存用户密码
            string userPwd = this.passWord.Password.Trim();

            // 开始验证
            if (string.IsNullOrEmpty(loginName))
            {
                this.joinPopupTextBlock.Text = "请您输入帐号后再登录";
                this.joinPopup.IsOpen = true;
                return 0;

            }
            else if (string.IsNullOrEmpty(userPwd))
            {

                this.joinPopupTextBlock.Text = "请您输入密码后再登录";
                this.joinPopup.IsOpen = true;
                return 0;
            }
            else if (userPwd.Length < 6)
            {
                this.joinPopupTextBlock.Text = "请您输入大于6位后再登录";
                this.joinPopup.IsOpen = true;
                return 0;
            }

            // 如果已通过以上所有验证则返回真
            return 1;
        }
        private void passWord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int s1 = UserInputCheck();
                if (s1 > 0)
                {
                    // 检测激活码是否正确，没有文件，或激活码错误都算作激活失败
                    ComputerInfo cc = new ComputerInfo();
                    EncryptionHelper ee = new EncryptionHelper();
                    string encryptComputer = ee.Encrypt(cc.GetComputerInfo(), "20070901");
                    string md5 = ee.GetMD5String(encryptComputer);
                  
                    //cc.WriteFile( ee.GetMD5String(encryptComputer), string.Format(@"{0}\Authorize.txt", System.Windows.Forms.Application.StartupPath));
                    if (!File.Exists(string.Format(@"{0}\license.txt", System.Windows.Forms.Application.StartupPath)))
                    {
                        //cc.WriteFile(encryptComputer, string.Format(@"{0}\license.txt", System.Windows.Forms.Application.StartupPath));
                        MessageBox.Show("系统检测到软件没有注册,请根据序列号进行注册。");
                        Authorize auth = new Authorize(encryptComputer);//用户名传递
                        auth.Show();
                        //// 授权失败，退出
                        //Environment.Exit(0);
                    }
                    else
                    {
                        if (cc.ReadFile(string.Format(@"{0}\license.txt", System.Windows.Forms.Application.StartupPath)) != md5)
                        {
                            LoGIin();//若是已授权，则转到登录界面
                        }
                        else
                        {
                            MessageBox.Show("系统检测到软件没有注册,请根据序列号进行注册。");
                            Authorize auth = new Authorize(encryptComputer);//用户名传递
                            auth.Show();
                        }
                    }

                }
            }
        }
  // 写登陆成功的用户名
        private void SaveHistroy()
        {
            string fileName = System.IO.Path.Combine(System.Windows.Forms. Application.StartupPath, @"History.txt");
            StreamWriter writer = new StreamWriter(fileName, false, Encoding.Default);
            foreach (string name in this.userName.Items  ) //向输入用户名的下拉框中填入已登录姓名的列表
            {

                writer.WriteLine(name);

            }
            if (!this.userName.Items.Contains(this.userName.Text))
            {
                writer.WriteLine(this.userName.Text);
            }
            writer.Flush();
            writer.Close();

        }
        private void LoGIin()//进入登录界面
        {
            //TODO


            string UserName = this.userName.Text.Trim();
            string PassWord = this.passWord.Password.Trim();

            if (UserName == "admin" && PassWord == "1")
            {
                
               SaveHistroy();
               this.Visibility = Visibility.Collapsed;
               MainView main = new MainView(UserName+","+"admin" );//用户名传递
               main.Show();
             }else{
                
                DBaccessHelp ex = new DBaccessHelp();
                DataTable dt = ex.ExecuteQuery("select userName,password,authority from userList where userName='" + UserName + "' order by userName");
                if (dt.Rows.Count > 0)
                {
                    if (UserName == dt.Rows[0][0].ToString() && PassWord == dt.Rows[0][1].ToString())
                    {
                        SaveHistroy();
                        this.Visibility = Visibility.Collapsed;
                        MainView main = new MainView(dt.Rows[0][0] + "," + dt.Rows[0][2]);
                        main.Show();
                    }
                }
                else
                {
                    MessageBox.Show("系统没有检测到该用户，请找管理员进行注册。");
                    // 授权失败，退出
                    //Environment.Exit(0);
                }
                ex.closeOleDbConnection(); 

                }


            


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            ThreadPool.SetMinThreads(100,100);
            //int maxworkthreads, maxportthreads, minworkthreads, minportthreads;

            //ThreadPool.GetMaxThreads(out maxworkthreads, out maxportthreads);
            //ThreadPool.GetMinThreads(out minworkthreads, out minportthreads);
            //MessageBox.Show(maxworkthreads.ToString() + "\t" + maxportthreads.ToString());
            //MessageBox.Show(minworkthreads.ToString() + "\t" + minportthreads.ToString());
            ComputerInfo cc = new ComputerInfo();
            EncryptionHelper ee = new EncryptionHelper();
           string  encryptComputer = ee.Encrypt(cc.GetComputerInfo(),"20070901");
           string md5=  ee.GetMD5String(encryptComputer);
            //cc.WriteFile( ee.GetMD5String(encryptComputer), string.Format(@"{0}\Authorize.txt", System.Windows.Forms.Application.StartupPath));
            if(!File.Exists(string.Format(@"{0}\license.txt", System.Windows.Forms.Application.StartupPath)))
            {
                //cc.WriteFile(encryptComputer, string.Format(@"{0}\Authorize.txt", System.Windows.Forms.Application.StartupPath));
                //MessageBox.Show("系统检测到软件没有注册,请根据文件Authorize中的序列号进行注册。");
                //// 授权失败，退出
                //Environment.Exit(0);
                MessageBox.Show("系统检测到软件没有注册,请根据序列号进行注册。");
                
                Authorize auth = new Authorize(encryptComputer);//用户名传递
                auth.Show();

            }
            else
            {
              if(  cc.ReadFile(string.Format(@"{0}\license.txt", System.Windows.Forms.Application.StartupPath)) == md5)
                {

                }
                else
                {
                    MessageBox.Show("系统检测到软件没有注册,请根据序列号进行注册。");
                    Authorize auth = new Authorize(encryptComputer);//用户名传递
                    auth.Show();
                }
            }

           


            /*
            softAuthorize = new HslCommunication.BasicFramework.SoftAuthorize();
            softAuthorize.FileSavePath = System.Windows.Forms.Application.StartupPath + @"\Authorize.txt"; // 设置存储激活码的文件，该存储是加密的
            softAuthorize.LoadByFile();

            // 检测激活码是否正确，没有文件，或激活码错误都算作激活失败
            if (!softAuthorize.IsAuthorizeSuccess(AuthorizeEncrypted))
            {
                // 显示注册窗口
                using (HslCommunication.BasicFramework.FormAuthorize form =
                    new HslCommunication.BasicFramework.FormAuthorize(
                        softAuthorize,
                        "请联系XXX获取激活码",
                        AuthorizeEncrypted))
                {
                    if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        // 授权失败，退出
                        Close();
                    }
                }
            }
         */
            //窗体拖动
            gmain.MouseMove += delegate(object sender_d, MouseEventArgs e_d)
            {
                if (e_d.LeftButton == MouseButtonState.Pressed)
                {
                    if (e_d.MouseDevice.Target is Control)
                        return;
                    this.DragMove();
                }

            };
            //窗体拖动
            LoadHistroy();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);//可以立即中断程序执行并退出
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ComputerInfo cc = new ComputerInfo();//初始化实例，用于获取电脑硬件信息
            EncryptionHelper ee = new EncryptionHelper();//初始化实例，生成
            string encryptComputer = ee.Encrypt(cc.GetComputerInfo(), "20070901");//获取电脑硬件信息，并与后面的字符串一起加密，获取加密字符串
            string md5 = ee.GetMD5String(encryptComputer);//获取加密字符串
            //cc.WriteFile( ee.GetMD5String(encryptComputer), string.Format(@"{0}\Authorize.txt", System.Windows.Forms.Application.StartupPath));
            if (!File.Exists(string.Format(@"{0}\license.txt", System.Windows.Forms.Application.StartupPath)))
            {
                //cc.WriteFile(encryptComputer, string.Format(@"{0}\Authorize.txt", System.Windows.Forms.Application.StartupPath));
                
                MessageBox.Show("系统检测到软件没有注册,请根据序列号进行注册。");
                Authorize auth = new Authorize(encryptComputer);//用户名传递
                auth.Show();
                // 授权失败，退出
                //Environment.Exit(0);
            }
            else
            {
                if (cc.ReadFile(string.Format(@"{0}\license.txt", System.Windows.Forms.Application.StartupPath)) == md5)
                {
                    LoGIin();//进入登录操作。
                }
                else
                {
                    //cc.WriteFile(encryptComputer, string.Format(@"{0}\Authorize.txt", System.Windows.Forms.Application.StartupPath));
                                
                    MessageBox.Show("系统检测到软件没有注册,请根据序列号进行注册。");
                    Authorize auth = new Authorize(encryptComputer);//用户名传递
                    auth.Show();
                    // 授权失败，退出
                    //Environment.Exit(0);
                }
            }
            // 检测激活码是否正确，没有文件，或激活码错误都算作激活失败
            //if (softAuthorize.IsAuthorizeSuccess(AuthorizeEncrypted))
            //{
            //    LoGIin();
            //}
            /*
            System.Diagnostics.Debug.WriteLine("main:"+Thread.CurrentThread.ManagedThreadId); // 主线程
            WaitingBox.WaitShow(this,
                             () =>
                             {
                                 System.Threading.Thread.Sleep(1500);//需要执行的方法体（需要自己做异常处理）
                                 System.Diagnostics.Debug.WriteLine("one:" + Thread.CurrentThread.ManagedThreadId);  // 其它线程
                             },
                             () =>
                             {
                                 System.Threading.Thread.Sleep(1500);//需要执行的方法体（需要自己做异常处理）
                                 System.Diagnostics.Debug.WriteLine("two:" + Thread.CurrentThread.ManagedThreadId);// 主线程
                             },
                             "警告！短路或型号错误,检测不合格！请检查...");
*/

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);//可以立即中断程序执行并退出
        }

        //private string AuthorizeEncrypted(string origin)
        //{
        //    // 此处使用了组件支持的DES对称加密技术
        //    return HslCommunication.BasicFramework.SoftSecurity.MD5Encrypt(origin, "07241016");
        //}

     
    }
}
