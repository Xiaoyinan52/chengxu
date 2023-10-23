using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// WaitingBox.xaml 的交互逻辑
    /// </summary>
    public partial class WaitingBox : Window
    {
        public string Text { get { return this.txtMessage.Text; } set { this.txtMessage.Text = value; } }

        private Action _Callback;
        private Action _Finish;

        public WaitingBox(Action callback , Action finish)
        {
            InitializeComponent( );
            this._Callback = callback;
            this._Finish = finish;
            this.Loaded += WaitingBox_Loaded;
        }

        void WaitingBox_Loaded(object sender , RoutedEventArgs e)
        {
            this._Callback.BeginInvoke(this.OnComplate , null);
        }

        private void OnComplate(IAsyncResult ar)
        {
            this.Dispatcher.Invoke(new Action(( ) =>
            {
                this.Close( );
                if (null != this._Finish)
                {
                    this._Finish.Invoke( );
                }
            }));
        }

        /// <summary>
        /// 显示等待框(模式窗体)
        /// </summary>
        /// <param name="owner">宿主视图元素</param>
        /// <param name="callback">需要执行的方法体（需要自己做异常处理）</param>
        /// <param name="finish">执行的方法体完成回调，可以为null</param>
        /// <param name="msg">等待提示信息</param>
        public static void WaitShow(FrameworkElement owner , Action callback , Action finish = null , string msg = "有一种幸福，叫做等待...")
        {
            WaitingBox win = new WaitingBox(callback , finish);
            Window pwin = Window.GetWindow(owner);
            win.Owner = pwin;
            win.Text = msg;
            var loc = owner.PointToScreen(new Point( ));
            win.Left = loc.X + (owner.ActualWidth - win.Width) / 2;
            win.Top = loc.Y + (owner.ActualHeight - win.Height) / 2;
            win.ShowDialog( );
        }

    }
}
