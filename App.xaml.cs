using HslCommunication.LogNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WireTestProgram
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private ILogNet logNet = new LogNetSingle(System.Windows.Forms.Application.StartupPath + "\\Logs\\DesignLog.txt");
        public App()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }
        private static System.Threading.Mutex mutex;

        protected override void OnStartup(StartupEventArgs e)
        {

            mutex = new System.Threading.Mutex(true, "OnlyRun_CRNS");
            if (mutex.WaitOne(0, false))
            {
                base.OnStartup(e);
                System.Windows.Forms.Application.EnableVisualStyles();
            }
            else
            {
                MessageBox.Show("程序已经在运行！", "提示");
                this.Shutdown();
            }
        }
       // UI线程的未处理异常
        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {

            
            
            MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
            logNet.WriteDebug("打开文件 error", e.Exception.ToString());
            logNet.WriteDebug("----------");
            e.Handled = true;//使用这一行代码告诉运行时，该异常被处理了，不再作为UnhandledException抛出了。
        }
        //非UI线程抛出的未处理异常
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
        
           MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
           logNet.WriteDebug("打开文件 error", ex.Message.ToString());
           logNet.WriteDebug("----------");
        }


    }
}
