using System;
using System.Collections.Generic;
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

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для Parameters.xaml
    /// </summary>
    public partial class Parameters : Window
    {
        MainWindow Main;
        ToolTip tip = new ToolTip();
        Binding binding = new Binding();

        public Parameters()
        {
            InitializeComponent();
            this.Loaded += WindowLoaded;
            TimerTextBlock.Visibility = Visibility.Hidden;
            TBSize.Visibility = Visibility.Hidden;
            LBSize.Visibility = Visibility.Hidden;
            TBSize_Copy.Visibility = Visibility.Hidden;
            LBSize_Copy.Visibility = Visibility.Hidden;
            TBSize_Copy1.Visibility = Visibility.Hidden;
            LBSize_Copy1.Visibility = Visibility.Hidden;

            foreach (RadioButton r in stackPanel.Children)
            {
                r.Checked += radiobutton_Checked;
            }
        }

        void radiobutton_Checked(object sender, EventArgs e)
        {
            foreach (RadioButton r in stackPanel.Children)
            {
                if (r.Equals(sender))
                {
                    switch (r.Content.ToString())
                    {
                        case ("Модель 1"):
                            TBSize.Visibility = Visibility.Visible;
                            LBSize.Visibility = Visibility.Visible;

                            TBSize_Copy.Visibility = Visibility.Hidden;
                            LBSize_Copy.Visibility = Visibility.Hidden;

                            TBSize_Copy1.Visibility = Visibility.Hidden;
                            LBSize_Copy1.Visibility = Visibility.Hidden;
                            break;
                        case ("Модель 2"):
                            TBSize.Visibility = Visibility.Visible;
                            LBSize.Visibility = Visibility.Visible;

                            TBSize_Copy.Visibility = Visibility.Visible;
                            LBSize_Copy.Visibility = Visibility.Visible;
                            LBSize_Copy.Text = "Предел";

                            TBSize_Copy1.Visibility = Visibility.Hidden;
                            LBSize_Copy1.Visibility = Visibility.Hidden;
                            break;
                        case ("Модель 3"):
                            TBSize.Visibility = Visibility.Visible;
                            LBSize.Visibility = Visibility.Visible;

                            TBSize_Copy.Visibility = Visibility.Visible;
                            LBSize_Copy.Visibility = Visibility.Visible;

                            TBSize_Copy1.Visibility = Visibility.Hidden;
                            LBSize_Copy1.Visibility = Visibility.Hidden;
                            LBSize_Copy.Text = "Предел";
                            break;
                        case ("Модель 4"):
                            TBSize.Visibility = Visibility.Visible;
                            LBSize.Visibility = Visibility.Visible;

                            TBSize_Copy.Visibility = Visibility.Visible;
                            LBSize_Copy.Visibility = Visibility.Visible;
                            LBSize_Copy.Text = "Предел";

                            TBSize_Copy1.Visibility = Visibility.Hidden;
                            LBSize_Copy1.Visibility = Visibility.Hidden;
                            break;
                        case ("Модель 4+"):
                            TBSize.Visibility = Visibility.Visible;
                            LBSize.Visibility = Visibility.Visible;

                            TBSize_Copy.Visibility = Visibility.Visible;
                            LBSize_Copy.Visibility = Visibility.Visible;
                            LBSize_Copy.Text = "Предел";

                            TBSize_Copy1.Visibility = Visibility.Visible;
                            LBSize_Copy1.Visibility = Visibility.Visible;
                            LBSize_Copy1.Text = "Длинна";
                            break;
                        case ("Барабаши"):
                            TBSize.Visibility = Visibility.Visible;
                            LBSize.Visibility = Visibility.Visible;

                            TBSize_Copy.Visibility = Visibility.Visible;
                            LBSize_Copy.Visibility = Visibility.Visible;
                            LBSize_Copy.Text = "Ребра";
                            //TBSize_Copy.Visibility = Visibility.Hidden;
                            //LBSize_Copy.Visibility = Visibility.Hidden;

                            TBSize_Copy1.Visibility = Visibility.Hidden;
                            LBSize_Copy1.Visibility = Visibility.Hidden;
                            //LBSize_Copy1.Text = "Частота";
                            break;
                        case ("Активная переписка"):
                            TBSize.Visibility = Visibility.Visible;
                            LBSize.Visibility = Visibility.Visible;

                            TBSize_Copy.Visibility = Visibility.Visible;
                            LBSize_Copy.Visibility = Visibility.Visible;
                            LBSize_Copy.Text = "Ребра";
                            //TBSize_Copy.Visibility = Visibility.Hidden;
                            //LBSize_Copy.Visibility = Visibility.Hidden;

                            TBSize_Copy1.Visibility = Visibility.Visible;
                            LBSize_Copy1.Visibility = Visibility.Visible;
                            LBSize_Copy1.Text = "Частота";
                            break;
                        case ("Лучшие друзья"):
                            TBSize.Visibility = Visibility.Visible;
                            LBSize.Visibility = Visibility.Visible;

                            TBSize_Copy.Visibility = Visibility.Visible;
                            LBSize_Copy.Visibility = Visibility.Visible;
                            LBSize_Copy.Text = "Доверие";

                            TBSize_Copy1.Visibility = Visibility.Visible;
                            LBSize_Copy1.Visibility = Visibility.Visible;
                            LBSize_Copy1.Text = "Частота";
                            break;
                        default:
                            TBSize.Visibility = Visibility.Hidden;
                            LBSize.Visibility = Visibility.Hidden;

                            TBSize_Copy.Visibility = Visibility.Hidden;
                            LBSize_Copy.Visibility = Visibility.Hidden;

                            TBSize_Copy1.Visibility = Visibility.Hidden;
                            LBSize_Copy1.Visibility = Visibility.Hidden;
                            break;
                    }
                    break;
                }
            }
        }
        private void button_Click(object sender, RoutedEventArgs e) //Random message
        {
            Main.RandomMessage();
            Main.Focus();
        }
        void WindowLoaded(object sender, EventArgs e)
        {
            Main = this.Owner as MainWindow;
            UpdatePosition();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            slider.Value = 0;
            Main.TimerIntervalChange(slider.Value);
            Main.log.LogBox.Text = "Log:";

            string Numbers = "0123456789";
            bool NotANumber = false;
            for (int i = 0; i < TBSize.Text.Length; i++)
                if (!Numbers.Contains(TBSize.Text[i]))
                {
                    NotANumber = true;
                    break;
                }
            for (int i = 0; i < TBSize_Copy.Text.Length; i++)
                if (!Numbers.Contains(TBSize_Copy.Text[i]))
                {
                    NotANumber = true;
                    break;
                }
            for (int i = 0; i < TBSize_Copy1.Text.Length; i++)
                if (!Numbers.Contains(TBSize_Copy1.Text[i]))
                {
                    NotANumber = true;
                    break;
                }

            foreach (RadioButton r in stackPanel.Children)
            {
                if ((bool)r.IsChecked)
                {
                    switch (r.Content.ToString())
                    {
                        case ("Модель 0"):
                            Main.RemoveAll();
                            Main.State = Models.Model_0;
                            Main.log.LogBox.Text += "\n" + Main.State;
                            Main.TimerStart();
                            break;
                        case ("Модель 1"):
                            if (!NotANumber && TBSize.Text.Length > 0)
                            {
                                Main.RemoveAll();
                                Main.State = Models.Model_1;
                                Main.log.LogBox.Text += "\n" + Main.State;
                                Main.RandomMessage(Convert.ToInt16(TBSize.Text));
                                Main.TimerStart();
                            }
                            else
                                MessageBox.Show("Введите целое число!");
                            break;
                        case ("Модель 2"):
                            if (!NotANumber && TBSize.Text.Length > 0 && TBSize_Copy.Text.Length > 0)
                            {
                                Main.RemoveAll();
                                Main.State = Models.Model_2;
                                Main.limit = Convert.ToInt16(TBSize_Copy.Text);
                                Main.log.LogBox.Text += "\n" + Main.State;
                                Main.RandomMessage(Convert.ToInt16(TBSize.Text), true);
                                Main.TimerStart();
                            }
                            else
                                MessageBox.Show("Введите корректные числа!");
                            break;
                        case ("Модель 3"):
                            if (!NotANumber && TBSize.Text.Length > 0 && TBSize_Copy.Text.Length > 0)
                            {
                                Main.RemoveAll();
                                Main.State = Models.Model_3;
                                Main.limit = Convert.ToInt16(TBSize_Copy.Text);
                                Main.log.LogBox.Text += "\n" + Main.State;
                                Main.RandomMessage(Convert.ToInt16(TBSize.Text), true);
                                Main.TimerStart();
                            }
                            else
                                MessageBox.Show("Введите корректные числа!");
                            break;
                        case ("Модель 4"):
                            if (!NotANumber && TBSize.Text.Length > 0 && TBSize_Copy.Text.Length > 0)
                            {
                                Main.RemoveAll();
                                Main.State = Models.Model_4;
                                Main.limit = Convert.ToInt16(TBSize_Copy.Text);
                                Main.log.LogBox.Text += "\n" + Main.State;
                                Main.RandomMessage(Convert.ToInt16(TBSize.Text), true);
                                Main.TimerStart();
                            }
                            else
                                MessageBox.Show("Введите корректные числа!");
                            break;
                        case ("Барабаши"):
                            if (!NotANumber && TBSize.Text.Length > 0 && TBSize_Copy.Text.Length > 0)
                            {
                                Main.RemoveAll();
                                Main.State = Models.Model_Barabasi;
                                //Main.period = Convert.ToInt16(TBSize_Copy1.Text);
                                Main.numEdges = Convert.ToInt16(TBSize_Copy.Text);
                                Main.log.LogBox.Text += "\n" + Main.State;
                                Main.RandomMessage(Convert.ToInt16(TBSize.Text), true);
                                Main.TimerStart();
                            }
                            else
                                MessageBox.Show("Введите корректные числа!");
                            break;
                        case ("Активная переписка"):
                            if (!NotANumber && TBSize.Text.Length > 0 && TBSize_Copy.Text.Length > 0 && TBSize_Copy1.Text.Length > 0)
                            {
                                Main.RemoveAll();
                                Main.State = Models.Model_Barabasi_Plus;
                                Main.period = Convert.ToInt16(TBSize_Copy1.Text);
                                Main.numEdges = Convert.ToInt16(TBSize_Copy.Text);
                                Main.log.LogBox.Text += "\n" + Main.State;
                                Main.RandomMessage(Convert.ToInt16(TBSize.Text), true);
                                Main.TimerStart();
                            }
                            else
                                MessageBox.Show("Введите корректные числа!");
                            break;
                        case ("Лучшие друзья"):
                            if (!NotANumber && TBSize.Text.Length > 0 && TBSize_Copy.Text.Length > 0 && TBSize_Copy1.Text.Length > 0)
                            {
                                Main.RemoveAll();
                                Main.State = Models.Model_Barabasi_Plus_Plus;
                                Main.period = Convert.ToInt16(TBSize_Copy1.Text);
                                Main.trustLevel = Convert.ToInt16(TBSize_Copy.Text);
                                Main.log.LogBox.Text += "\n" + Main.State;
                                Main.RandomMessage(Convert.ToInt16(TBSize.Text), true);
                                Main.TimerStart();
                            }
                            else
                                MessageBox.Show("Введите корректные числа!");
                            break;
                    }
                    break;
                }
            }
            Main.Focus();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Main.TimerStop();
            slider.Value = 0;
            Main.TimerIntervalChange(slider.Value);
            Main.Focus();
        }
        public void UpdatePosition()
        {
            this.Left = Main.Left + Main.Width;
            this.Top = Main.Top;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Main.Paused)
                Main.TimerStart();
            else
                Main.TimerPause();

            slider.Value = 0;
            Main.TimerIntervalChange(slider.Value);
            Main.Paused = !Main.Paused;
            Main.Focus();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            string Numbers = "0123456789";
            bool NotANumber = false;
            for (int i = 0; i < TBNode.Text.Length; i++)
                if (!Numbers.Contains(TBNode.Text[i]))
                {
                    NotANumber = true;
                    break;
                }
            if (!NotANumber && Convert.ToInt16(TBNode.Text) > 0)
            {
                Main.TimerStop();
                Main.N = Convert.ToInt16(TBNode.Text);
                Main.Node_.RemoveRange(0, Main.Node_.Count);
                Main.binding.RemoveRange(0, Main.binding.Count);
                Main.tips.RemoveRange(0, Main.tips.Count);
                for (int i = 0; i < Main.EllipseList.Count; i++)
                    Main.canvas.Children.Remove(Main.EllipseList[i]);
                Main.EllipseList.RemoveRange(0, Main.EllipseList.Count);
                Main.RemoveAll();
                Main.InitializeNodes();
                Main.draw();
                Main.drawLine();
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Main.TimerIntervalChange(slider.Value);
        }
    }
}
