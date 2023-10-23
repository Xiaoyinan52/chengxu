using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Windows.Threading;

namespace WireTestProgram
{
    /// <summary>
    /// ColorPicker.xaml 的交互逻辑
    /// </summary>
    public partial class ColorPicker : Window
    {
        private DesignerCanvas designer = null;
        private Window1 w1 = null;
        public ColorPicker(DesignerCanvas designer, Window1 w1)
        {
            InitializeComponent();
            this.designer = designer;
            this.w1 = w1;
            for (int i = 0; i < 56; i++)
            {
                KnowColors[i] = (System.Windows.Shapes.Rectangle)PanelLeft.Children[i];
            }


            SetAll.Clip = Triangle.Data;
            timer.Interval = TimeSpan.FromSeconds(0.01);
            timer.Tick += timer_Tick;


            InitialAll();
            InitialColor();
            System.Windows.Media.Color colortoshow = new System.Windows.Media.Color();
            colortoshow.A = Convert.ToByte(textBoxBrightness.Text);
            colortoshow.R = Convert.ToByte(textBoxRed.Text);
            colortoshow.G = Convert.ToByte(textBoxGreen.Text);
            colortoshow.B = Convert.ToByte(textBoxBlue.Text);
            ColorShow.Fill = new System.Windows.Media.SolidColorBrush(colortoshow);
            double xTOP = Convert.ToDouble(textBoxRed.Text);

            xTOP = Convert.ToDouble(textBoxBrightness.Text);

            xTOP = Convert.ToDouble(textBoxGreen.Text);

            xTOP = Convert.ToDouble(textBoxBlue.Text);
        }


        DispatcherTimer timer = new DispatcherTimer();

        public bool ifChoose = false;

        System.Windows.Shapes.Rectangle[] KnowColors = new System.Windows.Shapes.Rectangle[56];
        double MoveLength = 0;
        System.Windows.Point MouseNow = new System.Windows.Point();
        System.Windows.Point MouseDownP = new System.Windows.Point();
        FrameworkElement PTemp = new FrameworkElement();
        int NowSetting = 0;//0亮度 1红 2绿 3蓝 4All
        Canvas canvaNow = new Canvas();

        //确定添加到哪个
        private int clickCount = 0;








        void InitialAll()
        {
            Bitmap bitmaptemp = new Bitmap(10, 240);
            for (int i = 0; i <= 239; i++)
            {
                int RedValue = 0;
                int GreenValue = 0;
                int BlueValue = 0;
                int BrightValue = 255;
                if (i < 40)
                {
                    RedValue = 255;
                    BlueValue = 0;
                    GreenValue = Convert.ToInt16(i * 6.375);
                }
                else if (i < 80)
                {
                    GreenValue = 255;
                    BlueValue = 0;
                    RedValue = Convert.ToInt16(255 - (i - 40) * 6.375);
                }
                else if (i < 120)
                {
                    GreenValue = 255;
                    RedValue = 0;
                    BlueValue = Convert.ToInt16((i - 80) * 6.375);
                }
                else if (i < 160)
                {
                    BlueValue = 255;
                    RedValue = 0;
                    GreenValue = Convert.ToInt16(255 - (i - 120) * 6.375);
                }
                else if (i < 200)
                {
                    BlueValue = 255;
                    GreenValue = 0;
                    RedValue = Convert.ToInt16((i - 160) * 6.375);
                }
                else
                {
                    RedValue = 255;
                    GreenValue = 0;
                    BlueValue = Convert.ToInt16(255 - (i - 200) * 6.375);
                }
                System.Drawing.Color colorNow = System.Drawing.Color.FromArgb(BrightValue, RedValue, GreenValue, BlueValue);
                for (int j = 0; j < 10; j++)
                {
                    bitmaptemp.SetPixel(j, i, colorNow);
                }

            }
            MemoryStream stream = new MemoryStream();
            bitmaptemp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = stream;
            bitmapimage.EndInit();
            imageAll.Source = bitmapimage;
        }

        void InitialColor()
        {

        }


        #region 设置细条属性
        private void Setting_MouseDown(object sender, MouseButtonEventArgs e)
        {
            canvaNow = sender as Canvas;
            Canvas canvaTemp = new Canvas();
            switch (canvaNow.Name)
            {
                case "BrightSetting":
                    {

                        break;
                    }
                case "RedSetting":
                    {

                        break;
                    }
                case "GreenSetting":
                    {

                        break;
                    }
                case "BlueSetting":
                    {
                        //canvaTemp = SetBlue;
                        //NowSetting = 3;
                        break;
                    }
                default:
                    {
                        canvaTemp = SetAll;
                        NowSetting = 4;
                        break;
                    }
            }
            canvaNow.Cursor = Cursors.Hand;
            PTemp = e.Source as FrameworkElement;
            MouseDownP = Mouse.GetPosition(PTemp);
            canvaTemp.Margin = new Thickness(0, MouseDownP.Y, 0, 0);
            //canvaTemp.Margin = new Thickness(0, Convert.ToInt16(MouseDownP.Y), 0, 0);
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                timer.Start();
            }
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            MoveLength = 0;
            Canvas canvaTemp = new Canvas();
            TextBox textTemp = new TextBox();
            int PanelFlag = 0;
            switch (NowSetting)
            {
                case 0:
                    {

                        textTemp = textBoxBrightness;
                        break;
                    }
                case 1://红
                    {

                        //SetAll.Margin=new Thickness(0,0,0,0);
                        break;
                    }
                case 2://绿
                    {

                        //SetAll.Margin = new Thickness(0, 80, 0, 0);
                        break;
                    }
                case 3://蓝
                    {
                        //canvaTemp = SetBlue;
                        //textTemp = textBoxBlue;
                        //SetAll.Margin = new Thickness(0, 160, 0, 0);
                        break;
                    }
                default:
                    {
                        canvaTemp = SetAll;
                        PanelFlag = 1;
                        break;
                    }
            }
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (PanelFlag == 0)
                {
                    MouseNow = Mouse.GetPosition(PTemp);
                    MoveLength = MouseNow.Y - MouseDownP.Y;
                    if (canvaTemp.Margin.Top + MoveLength >= 255)
                    {
                        //canvaTemp.Margin = new Thickness(0, 255, 0, 0);
                        textTemp.Text = (255).ToString();
                    }
                    else if (canvaTemp.Margin.Top + MoveLength >= 0)
                    {
                        //canvaTemp.Margin = new Thickness(0, canvaTemp.Margin.Top + MoveLength, 0, 0);
                        textTemp.Text = (Convert.ToInt16(canvaTemp.Margin.Top + MoveLength)).ToString();
                    }
                    else
                    {
                        //canvaTemp.Margin = new Thickness(0, 0, 0, 0);
                        textTemp.Text = (0).ToString();
                    }
                    MouseDownP = MouseNow;
                }
                else
                {
                    MouseNow = Mouse.GetPosition(PTemp);
                    MoveLength = MouseNow.Y - MouseDownP.Y;
                    if (canvaTemp.Margin.Top + MoveLength >= 239)
                    {
                        canvaTemp.Margin = new Thickness(0, 239, 0, 0); ;
                    }
                    else if (canvaTemp.Margin.Top + MoveLength >= 0)
                    {
                        canvaTemp.Margin = new Thickness(0, canvaTemp.Margin.Top + MoveLength, 0, 0);
                    }
                    else
                    {
                        canvaTemp.Margin = new Thickness(0, 0, 0, 0);
                    }
                    MouseDownP = MouseNow;
                    if (canvaTemp.Margin.Top < 40)
                    {
                        textBoxRed.Text = (255).ToString();
                        textBoxBlue.Text = (0).ToString();
                        textBoxGreen.Text = (Convert.ToInt16(canvaTemp.Margin.Top * 6.375)).ToString();
                    }
                    else if (canvaTemp.Margin.Top < 80)
                    {
                        textBoxGreen.Text = (255).ToString();
                        textBoxBlue.Text = (0).ToString(); ;
                        textBoxRed.Text = (Convert.ToInt16(255 - (canvaTemp.Margin.Top - 40) * 6.375)).ToString();
                    }
                    else if (canvaTemp.Margin.Top < 120)
                    {
                        textBoxGreen.Text = (255).ToString();
                        textBoxRed.Text = (0).ToString(); ;
                        textBoxBlue.Text = (Convert.ToInt16((canvaTemp.Margin.Top - 80) * 6.375)).ToString();
                    }
                    else if (canvaTemp.Margin.Top < 160)
                    {
                        textBoxBlue.Text = (255).ToString();
                        textBoxRed.Text = (0).ToString(); ;
                        textBoxGreen.Text = (Convert.ToInt16(255 - (canvaTemp.Margin.Top - 120) * 6.375)).ToString();
                    }
                    else if (canvaTemp.Margin.Top < 200)
                    {
                        textBoxBlue.Text = (255).ToString();
                        textBoxGreen.Text = (0).ToString(); ;
                        textBoxRed.Text = (Convert.ToInt16((canvaTemp.Margin.Top - 160) * 6.375)).ToString();
                    }
                    else
                    {
                        textBoxRed.Text = (255).ToString();
                        textBoxGreen.Text = (0).ToString(); ;
                        textBoxBlue.Text = (Convert.ToInt16(255 - (canvaTemp.Margin.Top - 200) * 6.375)).ToString();
                    }
                }

            }
            else
            {
                timer.Stop();
                canvaNow.Cursor = Cursors.Arrow;
            }
        }
        #endregion

        #region 属性值改变
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox textTemp = sender as TextBox;
                if (textTemp.Text.Length == 0)
                {
                    textTemp.Text = "0";
                }
                if ((textTemp.Text[textTemp.Text.Length - 1] > '9') || (textTemp.Text[textTemp.Text.Length - 1] < '0'))
                {
                    textTemp.Text = textTemp.Text.Remove(textTemp.Text.Length - 1);
                }
                if ((textTemp.Text[0] == '0') && (textTemp.Text.Length > 1))
                {
                    textTemp.Text = textTemp.Text.Remove(0, 1);
                }
                if (Convert.ToInt32(textTemp.Text) > 255)
                {
                    textTemp.Text = "255";
                }
                textTemp.SelectionStart = textTemp.Text.Length;

                switch (textTemp.Name)
                {
                    case "textBoxRed":
                        {

                            break;
                        }
                    case "textBoxGreen":
                        {

                            break;
                        }
                    case "textBoxBlue":
                        {
                            //double xTOP = Convert.ToDouble(textTemp.Text);
                            //SetBlue.Margin = new Thickness(0, xTOP, 0, 0);
                            break;
                        }
                    case "textBoxBrightness":
                        {

                            break;
                        }
                    default: break;
                }

                System.Windows.Media.Color colortoshow = new System.Windows.Media.Color();
                colortoshow.A = Convert.ToByte(textBoxBrightness.Text);
                colortoshow.R = Convert.ToByte(textBoxRed.Text);
                colortoshow.G = Convert.ToByte(textBoxGreen.Text);
                colortoshow.B = Convert.ToByte(textBoxBlue.Text);
                ColorShow.Fill = new System.Windows.Media.SolidColorBrush(colortoshow);



            }
            catch
            {

            }

        }
        #endregion
        private int selectCount = 0;
        private string color01 = string.Empty;
        private string color02 = string.Empty;
        private void rectangle42_MouseDown(object sender, MouseButtonEventArgs e)
        {
            for (int i = 0; i < 56; i++)
            {
                KnowColors[i].Stroke = new System.Windows.Media.SolidColorBrush(Colors.Black);
                KnowColors[i].StrokeThickness = 1;
            }
            System.Windows.Shapes.Rectangle recTemp = sender as System.Windows.Shapes.Rectangle;
            recTemp.Stroke = new System.Windows.Media.SolidColorBrush(Colors.Black);
            recTemp.StrokeThickness = 2;
            ColorShow.Fill = recTemp.Fill;
            string texttemp = (recTemp.Fill).ToString();
            System.Windows.Media.Color colorTemp = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(texttemp);
            textBoxBrightness.Text = colorTemp.A.ToString();
            textBoxRed.Text = colorTemp.R.ToString();
            textBoxGreen.Text = colorTemp.G.ToString();
            textBoxBlue.Text = colorTemp.B.ToString();
//填充颜色
selectCount+=1;
if (selectCount == 1)
{
    color01 = texttemp;
}
            if(selectCount==2)
            {
                //
                color02 = texttemp;
                //
                List<DesignerItem>  selectedDesignerItems =
                                designer.SelectionService.CurrentSelection.OfType<DesignerItem>().Where(s => s.Content.ToString().Contains("System.Windows.Shapes.Ellipse")).ToList();
                if (selectedDesignerItems.Count == 1)
                {
                    // MessageBox.Show(selectedDesignerItems[0].Content.ToString());
                    //Ellipse nodeCircle = new Ellipse();
                    Ellipse nodeCircle = (Ellipse)selectedDesignerItems[0].Content;
                    nodeCircle.Stroke = new SolidColorBrush(Colors.Black);
                    LinearGradientBrush brush = new LinearGradientBrush();
                    brush.StartPoint = new System.Windows.Point(0.5, 0);
                    brush.EndPoint = new System.Windows.Point(0.5, 1);
                    System.Windows.Media.Color color = new System.Windows.Media.Color();
                    System.Windows.Media.Color color03 = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color01);
                    color.A = color03.A;
                    color.B = color03.B;
                    color.G = color03.G;
                    color.R = color03.R;
                    GradientStop gs1 = new GradientStop();
                    gs1.Offset = 0;
                    gs1.Color = color;
                    brush.GradientStops.Add(gs1);

                    GradientStop gs2 = new GradientStop();
                    gs2.Offset = 0.6;
                    gs2.Color = color;
                    brush.GradientStops.Add(gs2);

                    GradientStop gs3 = new GradientStop();
                    gs3.Offset = 0.61;
                    gs3.Color = Colors.White;
                    brush.GradientStops.Add(gs3);

                    System.Windows.Media.Color color2 = new System.Windows.Media.Color();
                    System.Windows.Media.Color color04 = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color02);
                    color2.A = color04.A;
                    color2.B = color04.B;
                    color2.G = color04.G;
                    color2.R = color04.R;
                    GradientStop gs4 = new GradientStop();
                    gs4.Offset = 0.61;
                    gs4.Color = color2;
                    //因为GradientStops属性返回一个GradientStopCollection对象，而且GradientStopCollection类实现了IList接口
                    brush.GradientStops.Add(gs4);//所以这句代码的本质是IList list = brush.GradientStops; list.Add(gs3);



                    GradientStop gs5 = new GradientStop();
                    gs5.Offset = 1;
                    gs5.Color = color2;
                    //因为GradientStops属性返回一个GradientStopCollection对象，而且GradientStopCollection类实现了IList接口
                    brush.GradientStops.Add(gs5);//所以这句代码的本质是IList list = brush.GradientStops; list.Add(gs3);



                    nodeCircle.Fill = brush;

                    //selectedDesignerItems[0].Content = nodeCircle;
                }
                //全部初始化
                selectCount = 0;
            }


        }

        private void button1_Click(object sender, RoutedEventArgs e)//确定
        {
            clickCount += 1;
            if (clickCount < 9)
            {
                if (clickCount == 1)
                {
                    System.Windows.Media.Color colortoshow = new System.Windows.Media.Color();
                    colortoshow.A = Convert.ToByte(textBoxBrightness.Text);
                    colortoshow.R = Convert.ToByte(textBoxRed.Text);
                    colortoshow.G = Convert.ToByte(textBoxGreen.Text);
                    colortoshow.B = Convert.ToByte(textBoxBlue.Text);
                    rectangle71.Fill = new System.Windows.Media.SolidColorBrush(colortoshow);
                }
                else if (clickCount == 2)
                {
                    System.Windows.Media.Color colortoshow = new System.Windows.Media.Color();
                    colortoshow.A = Convert.ToByte(textBoxBrightness.Text);
                    colortoshow.R = Convert.ToByte(textBoxRed.Text);
                    colortoshow.G = Convert.ToByte(textBoxGreen.Text);
                    colortoshow.B = Convert.ToByte(textBoxBlue.Text);
                    rectangle72.Fill = new System.Windows.Media.SolidColorBrush(colortoshow);
                }
                else if (clickCount == 3)
                {
                    System.Windows.Media.Color colortoshow = new System.Windows.Media.Color();
                    colortoshow.A = Convert.ToByte(textBoxBrightness.Text);
                    colortoshow.R = Convert.ToByte(textBoxRed.Text);
                    colortoshow.G = Convert.ToByte(textBoxGreen.Text);
                    colortoshow.B = Convert.ToByte(textBoxBlue.Text);
                    rectangle73.Fill = new System.Windows.Media.SolidColorBrush(colortoshow);
                }
                else if (clickCount == 4)
                {
                    System.Windows.Media.Color colortoshow = new System.Windows.Media.Color();
                    colortoshow.A = Convert.ToByte(textBoxBrightness.Text);
                    colortoshow.R = Convert.ToByte(textBoxRed.Text);
                    colortoshow.G = Convert.ToByte(textBoxGreen.Text);
                    colortoshow.B = Convert.ToByte(textBoxBlue.Text);
                    rectangle74.Fill = new System.Windows.Media.SolidColorBrush(colortoshow);
                }
                else if (clickCount == 5)
                {
                    System.Windows.Media.Color colortoshow = new System.Windows.Media.Color();
                    colortoshow.A = Convert.ToByte(textBoxBrightness.Text);
                    colortoshow.R = Convert.ToByte(textBoxRed.Text);
                    colortoshow.G = Convert.ToByte(textBoxGreen.Text);
                    colortoshow.B = Convert.ToByte(textBoxBlue.Text);
                    rectangle75.Fill = new System.Windows.Media.SolidColorBrush(colortoshow);
                }
                else if (clickCount == 6)
                {
                    System.Windows.Media.Color colortoshow = new System.Windows.Media.Color();
                    colortoshow.A = Convert.ToByte(textBoxBrightness.Text);
                    colortoshow.R = Convert.ToByte(textBoxRed.Text);
                    colortoshow.G = Convert.ToByte(textBoxGreen.Text);
                    colortoshow.B = Convert.ToByte(textBoxBlue.Text);
                    rectangle76.Fill = new System.Windows.Media.SolidColorBrush(colortoshow);
                }
                else if (clickCount == 7)
                {
                    System.Windows.Media.Color colortoshow = new System.Windows.Media.Color();
                    colortoshow.A = Convert.ToByte(textBoxBrightness.Text);
                    colortoshow.R = Convert.ToByte(textBoxRed.Text);
                    colortoshow.G = Convert.ToByte(textBoxGreen.Text);
                    colortoshow.B = Convert.ToByte(textBoxBlue.Text);
                    rectangle77.Fill = new System.Windows.Media.SolidColorBrush(colortoshow);
                }
                else if (clickCount == 8)
                {
                    System.Windows.Media.Color colortoshow = new System.Windows.Media.Color();
                    colortoshow.A = Convert.ToByte(textBoxBrightness.Text);
                    colortoshow.R = Convert.ToByte(textBoxRed.Text);
                    colortoshow.G = Convert.ToByte(textBoxGreen.Text);
                    colortoshow.B = Convert.ToByte(textBoxBlue.Text);
                    rectangle78.Fill = new System.Windows.Media.SolidColorBrush(colortoshow);
                }

            }
            else
            {
                clickCount = 1;
                System.Windows.Media.Color colortoshow = new System.Windows.Media.Color();
                colortoshow.A = Convert.ToByte(textBoxBrightness.Text);
                colortoshow.R = Convert.ToByte(textBoxRed.Text);
                colortoshow.G = Convert.ToByte(textBoxGreen.Text);
                colortoshow.B = Convert.ToByte(textBoxBlue.Text);
                rectangle71.Fill = new System.Windows.Media.SolidColorBrush(colortoshow);
            }



        }

        private void button2_Click(object sender, RoutedEventArgs e)//取消
        {
            ifChoose = false;
            // this.Close();
            this.Width = 260;
        }

        private void zidingyi_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Width = 480;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Width = 260;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ifChoose = true;
            //ColorShow.Background//最终确定的颜色，直接赋值即可

        }


    }
}
