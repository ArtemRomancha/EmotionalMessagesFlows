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
    /// Логика взаимодействия для Log.xaml
    /// </summary>
    public partial class Log : Window
    {
        MainWindow Main;
        public Log()
        {
            InitializeComponent();
            this.Loaded += WindowLoaded;
        }
        void WindowLoaded(object sender, EventArgs e)
        {
            Main = this.Owner as MainWindow;
            UpdatePosition();
            FormMatrix();
            Main.Focus();
        }
        public void UpdatePosition()
        {
            this.Left = Main.Left - this.Width;
            this.Top = Main.Top;
        }
        public void FormMatrix()
        {
            string Matrix = "     ";
            int index = -1;
            for (int i = 0; i < Main.Node_.Count; i++)
                if (i < 10)
                    Matrix += "  " + i + "   ";
                else
                    Matrix += i + "  ";
            Matrix += "\n";
            for (int i = 0; i < Main.Node_.Count; i++)
            {
                if (i < 10)
                    Matrix += "  " + i + "  ";
                else
                    Matrix += i + "  ";
                for (int f = 0; f < Main.Node_.Count; f++)
                {
                    if (Main.Node_[i].Edges.Count > 0)
                    {
                        for (int j = 0; j < Main.Node_[i].Edges.Count; j++)
                            if (Main.Node_[i].Edges[j].Equals(Main.Node_[f]))
                            {
                                index = j;
                                break;
                            }
                        if (index != -1)
                        {
                            if (Main.Node_[i].EdgeWeight[index] < 10)
                                Matrix += " " + Main.Node_[i].EdgeWeight[index];
                            else
                                Matrix += Main.Node_[i].EdgeWeight[index];
                            index = -1;
                            if (f >= 10)
                                Matrix += "   ";
                            else
                                Matrix += "    ";
                        }
                        else
                            if (f >= 10)
                                Matrix += " 0   ";
                            else
                                Matrix += " 0    ";
                    }
                    else
                        if (f >= 10)
                            Matrix += " 0   ";
                        else
                            Matrix += " 0    ";
                }
                Matrix += "\n";
            }
            MatrixBox.Text = Matrix;
        }
        public void AddLog(string log)
        {
            LogBox.Text += log;
        }
    }
}
