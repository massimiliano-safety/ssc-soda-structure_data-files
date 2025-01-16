using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

namespace SodaStructureDataFiles
{
    /// <summary>
    /// Interaktionslogik für UC_StatusBar.xaml
    /// </summary>
    public partial class UC_StatusBar : UserControl
    {
        public bool _isShowReferenz = false;
        public double _ShowReferenzValue = 0;
        public UC_StatusBar()
        {
            InitializeComponent();

            if (_isShowReferenz)
                PolyIndicator.Visibility = Visibility.Visible;
            else
                PolyIndicator.Visibility = Visibility.Hidden;

            draw_ShowReferenzValue();
        }

        public void draw_ShowReferenzValue()
        {
            try
            {
                double max = Convert.ToDouble(ValueMax.Text);
                double min = Convert.ToDouble(ValueMin.Text);
                double val = _ShowReferenzValue;

                double range_value = max - min;


                double X_center = GridReference.ActualWidth / 2;
                double Y_center = GridReference.ActualHeight;

                double pos = val / max;
                Point first = new Point(pos * GridReference.ActualWidth, Y_center);
                Point left = new Point((pos * GridReference.ActualWidth) - 3, 0);
                Point right = new Point((pos * GridReference.ActualWidth) + 3, 0);

                List<Point> data_points = new List<Point>();
                data_points.Add(left);
                data_points.Add(first);
                data_points.Add(right);

                PolyIndicator.Points = new PointCollection(data_points);
            }
            catch (Exception )
            {

            }
        }
        public void draw_value()
        {
            try
            {
                double max = Convert.ToDouble(ValueMax.Text);
                double min = Convert.ToDouble(ValueMin.Text);
                double val = Convert.ToDouble(Value.Text);

                if (val > max)
                    val = max;
                if (val < min)
                    val = min;

                double range_value = max - min;
                progress_bar.Minimum = 0;
                if (max > min)
                    progress_bar.Maximum = max - min;
                else
                    progress_bar.Maximum = min + 1;

                progress_bar.Value = val - min;


            }
            catch (Exception )
            {

            }
        }
        public void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isShowReferenz)
                PolyIndicator.Visibility = Visibility.Visible;
            else
                PolyIndicator.Visibility = Visibility.Hidden;

            draw_ShowReferenzValue();
        }

        [Browsable(true)]
        [Category("UCSettings")]
        public Brush RectFrontColor
        {
            get { return progress_bar.Foreground; }
            set { progress_bar.Foreground = value; }
        }

        [Browsable(true)]
        [Category("UCSettings")]
        public Brush RectBackColor
        {
            get { return progress_bar.Background; }
            set { progress_bar.Background = value; }
        }

        [Browsable(true)]
        [Category("UCSettings")]
        public bool ShowReferenz
        {
            get { return _isShowReferenz; }
            set
            {
                _isShowReferenz = value;
                if (_isShowReferenz)
                {
                    draw_ShowReferenzValue();
                    PolyIndicator.Visibility = Visibility.Visible;
                }
                else
                    PolyIndicator.Visibility = Visibility.Hidden;
            }
        }

        [Browsable(true)]
        [Category("UCSettings")]
        public double ShowReferenzValue
        {
            get { return _ShowReferenzValue; }
            set
            {
                _ShowReferenzValue = value;
                draw_ShowReferenzValue();
            }
        }

        [Browsable(true)]
        [Category("UCSettings")]
        public String VALUE_MIN
        {
            get { return ValueMin.Text; }
            set
            {
                ValueMin.Text = value;
                draw_value();
            }
        }

        [Browsable(true)]
        [Category("UCSettings")]
        public String VALUE_MAX
        {
            get { return ValueMax.Text; }
            set
            {
                ValueMax.Text = value;
                draw_value();
            }
        }

        [Browsable(true)]
        [Category("UCSettings")]
        public String VALUE
        {
            get { return Value.Text; }
            set
            {
                Value.Text = value;
                draw_value();
            }
        }

        [Browsable(true)]
        [Category("UCSettings")]
        public String Text_Name
        {
            get { return TextName.Text; }
            set { TextName.Text = value; }
        }

    }
}
