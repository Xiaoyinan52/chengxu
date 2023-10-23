using HslCommunication.LogNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using WireTestProgram.Controls;

namespace WireTestProgram
{

    //These attributes identify the types of the named parts that are used for templating
    [TemplatePart(Name = "PART_DragThumb", Type = typeof(DragThumb))]
    [TemplatePart(Name = "PART_ResizeDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ConnectorDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    public class DesignerItem : ContentControl, ISelectable, IGroupable
    {
        private ILogNet logNet = new LogNetSingle(System.Windows.Forms.Application.StartupPath + "\\Logs\\DesignLog.txt");
        #region ID
        private Guid id;
        public Guid ID
        {
            get { return id; }
        }
        #endregion

        #region ParentID
        public Guid ParentID
        {
            get { return (Guid)GetValue(ParentIDProperty); }
            set { SetValue(ParentIDProperty, value); }
        }
        public static readonly DependencyProperty ParentIDProperty = DependencyProperty.Register("ParentID", typeof(Guid), typeof(DesignerItem));

        public StringBuilder ItemContent { get; set; }

        public string BKImgSource
        {
            get { return (string)GetValue(BKImgSourceProperty); }
            set { SetValue(BKImgSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BKImgSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BKImgSourceProperty =
            DependencyProperty.Register("BKImgSource", typeof(string), typeof(DesignerItem));



        public string LeftBK
        {
            get { return (string)GetValue(LeftBKProperty); }
            set { SetValue(LeftBKProperty, value); }
        }

        //Using a DependencyProperty as the backing store for LeftBK.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftBKProperty =
        DependencyProperty.Register("LeftBK", typeof(string), typeof(DesignerItem));



        public string MuKuaiName
        {
            get { return (string)GetValue(MuKuaiNameProperty); }
            set { SetValue(MuKuaiNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MuKuaiName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MuKuaiNameProperty =
            DependencyProperty.Register("MuKuaiName", typeof(string), typeof(DesignerItem));



        public string ZuoBiaoName
        {
            get { return (string)GetValue(ZuoBiaoNameProperty); }
            set { SetValue(ZuoBiaoNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZuoBiaoName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZuoBiaoNameProperty =
            DependencyProperty.Register("ZuoBiaoName", typeof(string), typeof(DesignerItem));
        // 中间点显示
        public bool ConnectorVisble
        {
            get { return (bool)GetValue(ConnectorVisbleProperty); }
            set { SetValue(ConnectorVisbleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZuoBiaoName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectorVisbleProperty =
            DependencyProperty.Register("ConnectorVisble", typeof(bool), typeof(DesignerItem), new PropertyMetadata(false));



        #endregion

        #region IsGroup
        public bool IsGroup
        {
            get { return (bool)GetValue(IsGroupProperty); }
            set { SetValue(IsGroupProperty, value); }
        }
        public static readonly DependencyProperty IsGroupProperty =
            DependencyProperty.Register("IsGroup", typeof(bool), typeof(DesignerItem));
        #endregion

        #region IsSelected Property

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected",
                                       typeof(bool),
                                       typeof(DesignerItem),
                                       new FrameworkPropertyMetadata(false));

        #endregion

        #region DragThumbTemplate Property

        // can be used to replace the default template for the DragThumb  是左边的Path
        public static readonly DependencyProperty DragThumbTemplateProperty =
            DependencyProperty.RegisterAttached("DragThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetDragThumbTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(DragThumbTemplateProperty);
        }

        public static void SetDragThumbTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(DragThumbTemplateProperty, value);
        }

        #endregion

        #region ConnectorDecoratorTemplate Property

        // can be used to replace the default template for the ConnectorDecorator  特殊自定义的图形  像五角星 中的  Connector点模板
        public static readonly DependencyProperty ConnectorDecoratorTemplateProperty =
            DependencyProperty.RegisterAttached("ConnectorDecoratorTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetConnectorDecoratorTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(ConnectorDecoratorTemplateProperty);
        }

        public static void SetConnectorDecoratorTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(ConnectorDecoratorTemplateProperty, value);
        }

        #endregion

        #region IsDragConnectionOver

        // while drag connection procedure is ongoing and the mouse moves over 
        // this item this value is true; if true the ConnectorDecorator is triggered
        // to be visible, see template
        public bool IsDragConnectionOver
        {
            get { return (bool)GetValue(IsDragConnectionOverProperty); }
            set { SetValue(IsDragConnectionOverProperty, value); }
        }
        public static readonly DependencyProperty IsDragConnectionOverProperty =
            DependencyProperty.Register("IsDragConnectionOver",
                                         typeof(bool),
                                         typeof(DesignerItem),
                                         new FrameworkPropertyMetadata(false));

        #endregion

        static DesignerItem()
        {
            // set the key to reference the style for this control
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
        }

        public DesignerItem(Guid id)
        {
            this.id = id;
            this.ItemContent = new StringBuilder();
            this.Loaded += new RoutedEventHandler(DesignerItem_Loaded);
        }

        public DesignerItem()
            : this(Guid.NewGuid())
        {
        }


        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;
            if (designer.Tag.ToString() == "design" && e.LeftButton == MouseButtonState.Pressed)
            {
                Task.Run(() =>
            {

                Thread.Sleep(400);
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
         (Action)delegate ()
         {


             Window1 w1 = GetWindow1(designer) as Window1;
             List<Connection> connections = (from item in designer.Children.OfType<Connection>()
                                             select item).ToList();
             Connector connector = designer.GetConnector(this.ID, "Center");
             List<Connection> list = connector.Connections;
             designer.searchConnections.Clear();

             if (list.Count > 0)
             {
                 designer.searchConnection(list[0]);
                 var exp = connections.Where(a => !designer.searchConnections.Exists(t => (a.ID.ToString()).Contains(t.ID.ToString()))).ToList();
                 // Console.WriteLine("--查找connections集合中存在，而list不存在的数据--");
                 foreach (var item in exp)
                 {
                     item.Visibility = Visibility.Hidden;
                 }
                 foreach (var item in designer.searchConnections)
                 {
                     item.Visibility = Visibility.Visible;
                 }
             }


             if (this.Content.ToString().Contains("System.Windows.Shapes.Ellipse"))
             {
                 w1.changeProperty(this);
             }
             else
             {

                 w1.showChangeProperty(this);

             }




         });

            });
            }
            // update selection
            if (designer != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                    if (this.IsSelected)
                    {

                        designer.SelectionService.RemoveFromSelection(this);

                    }
                    else
                    {

                        designer.SelectionService.AddToSelection(this);
                    }
                else if (!this.IsSelected)
                {

                    designer.SelectionService.SelectItem(this);
                }
                Focus();
            }
            //单击改变坐标颜色
            //DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;





            e.Handled = false;




        }
        //private ColorPicker colorPick = null;





        //双击事件 进行pin点
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);


            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;
                if (designer.Tag.ToString() == "design" && this.Content.ToString().Contains("System.Windows.Shapes.Ellipse"))
                {

                    Window1 w1 = GetWindow1(designer) as Window1;

                    List<DesignerItem> ds = designer.SelectionService.CurrentSelection.OfType<DesignerItem>().Where(s => s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();
                    if (ds.Count != 1)
                    {
                        MessageBox.Show("Pin点只能选中一个点！！！");
                        return;
                    }
                    Connector selectConnector = designer.GetConnector(ds[0].ID, "Center");
                    if (selectConnector.Connections.Count > 0)
                    {
                        MessageBox.Show("当前点存在连接关系，禁止pin点操作！！！");
                        return;
                    }


                    if (w1._serialPort4.IsOpen)
                    {
                        Thread.Sleep(200);
                        //MessageBox.Show(w1.bankaQty.ToString("X2"));
                        w1._serialPort4.Write("$Cmd,35," + w1.bankaQty.ToString("X2") + ",98*");
                    }
                    else
                    {
                        MessageBox.Show("通信失败！！！");
                    }


                }
                else if (designer.Tag.ToString() == "design" && this.Content.ToString().Contains("System.Windows.Shapes.Path"))  //在相应的未知产生一个圆
                {
                    Window1 w1 = GetWindow1(designer) as Window1;
                    if (w1.autoEllipse == "正在自动布点")
                    {


                        Point position = e.GetPosition(this);
                        Object content = XamlReader.Load(XmlReader.Create(string.Format(@"{0}\ellipse.xml", System.Windows.Forms.Application.StartupPath)));
                        DesignerItem newItem = new DesignerItem();
                        newItem.Content = content;
                        newItem.ItemContent.Append(XamlWriter.Save(newItem.Content));
                        newItem.Width = 8;
                        newItem.Height = 8;
                        newItem.MuKuaiName = "";
                        newItem.BKImgSource = "";
                        newItem.ConnectorVisble = false;
                        double placeleft = Canvas.GetLeft(this);
                        double placetop = Canvas.GetTop(this);
                        Canvas.SetLeft(newItem, placeleft + position.X - 4);
                        Canvas.SetTop(newItem, placetop + position.Y - 4);
                        Canvas.SetZIndex(newItem, designer.Children.Count);
                        designer.Children.Add(newItem);
                        designer.SetConnectorDecoratorTemplate(newItem);
                        designer.SelectionService.SelectItem(newItem);
                        newItem.Focus();





                    }


                }
            }


        }
        private Window1 GetWindow1(DependencyObject element)
        {
            while (element != null && !(element is Window1))
                element = VisualTreeHelper.GetParent(element);

            return element as Window1;
        }
        //取得内容下面的子控件 这一步给内容里的控件 加拖拽  是为了 鼠标在内容上 也可以实现拖拽
        void DesignerItem_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {



                if (base.Template != null)
                {
                    ContentPresenter contentPresenter =
                        this.Template.FindName("PART_ContentPresenter", this) as ContentPresenter;
                    if (contentPresenter != null && VisualTreeHelper.GetChildrenCount(contentPresenter) > 0)
                    {





                        UIElement contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;
                        if (contentVisual != null)
                        {
                            DragThumb thumb = this.Template.FindName("PART_DragThumb", this) as DragThumb;
                            if (thumb != null)
                            {
                                ControlTemplate template =
                                    DesignerItem.GetDragThumbTemplate(contentVisual) as ControlTemplate;
                                if (template != null)
                                    thumb.Template = template;
                            }
                        }


                    }
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("系统出现可预知错误，请到日志查看相关记录！！！");
                logNet.WriteDebug("打开文件 error", ex.Message);
                logNet.WriteDebug("----------");

            }



        }



    }
}
