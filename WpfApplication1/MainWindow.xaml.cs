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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public struct Removal
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
    public enum Models { Model_0, Model_1, Model_2, Model_3, Model_4, Model_Barabasi, Model_Barabasi_Plus, Model_Barabasi_Plus_Plus, };
    public partial class MainWindow : Window
    {
        #region parameters
        Models state = Models.Model_1; //Текущее модель 
        public Models State //свойство для доступа к состоянию
        {
            get { return state; }
            set { state = value; }
        }

        DispatcherTimer timer = new DispatcherTimer(); //Таймер симуляции 
        DispatcherTimer MoveTimer = new DispatcherTimer(); //Timer движения сообщения

        public double N = 5; //Размер сети 
        List<Node> Nodes = new List<Node>(); //Узлы сети 
        public List<Ellipse> EllipseList = new List<Ellipse>(); //Эллипсы отображающие узлы сети
        List<Line> links = new List<Line>(); //Ребра
        List<int[]> linkNode = new List<int[]>(); //Соответствие ребра узлу 
        public List<ToolTip> tips = new List<ToolTip>(); //Подсказки к эллипсам 
        public List<Binding> binding = new List<Binding>(); //Связь данных с эллипсами 
        List<ToolTip> tipsLine = new List<ToolTip>(); //Подсказки к ребрам 
        List<Binding> bindingLine = new List<Binding>(); //Связь данных с ребрами 

        TimeSpan Stopwatch = new TimeSpan(0, 0, 0); //Секундомер симуляции        

        public Canvas canvas = new Canvas(); //Полотно для отрисовки 
        Removal removal = new Removal(); //Сдвижка относительно угла экрана 

        bool MouseDownBool = false; //Поднят эллипс
        public bool Paused = false; //Пауза симуляции  

        int EllipseIndex = -1; //Индекс выделенного эллипса 
        public int limit = 0; //Лимит памяти (сколько времени храним сообщения)
        public int lastEdge = -1; //Номер последнего узла, которое получило сообщение
        int counter = 0; //Счетчик в модели Барабаши
        public int period = -1; //Период добавления пользователей
        public int numEdges = -1; //Кол-во ребер у нового узла в модели Барабаши
        public int trustLevel = 1; //Уровень доверия        

        Label label = new Label(); //Поле дляя вывода вспомогательной информации в главном окне

        Parameters par = new Parameters(); //Окно параметров 
        public Log log = new Log(); //Окно логов 

        public List<Node> Node_ //Свойство для доступа к узлам из вне
        {
            get { return Nodes; }
        }

        List<Ellipse> message = new List<Ellipse>(); //Отрисовка кружка сообщения сообщения 
        //List<int> messageWaySegment = new List<int>(); //Текущий сегмент сообщения
        List<int[]> agents = new List<int[]>();
        List<double[]> segment = new List<double[]>();
        List<int[]> numSegment = new List<int[]>();
        //bool messageMoveing = false;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            Grid.Children.Add(canvas); //Добавления полотна
            this.PreviewMouseMove += MouseMoveUpdater; //Добавление обработчков событий для реакции на движение мышкой
            this.PreviewMouseMove += EllipseMouseMove;
            this.Loaded += WindowInitialized;

            canvas.Children.Add(label);
            Canvas.SetRight(label, 100);

            InitializeNodes(); //Инициализация узлов

            draw(); //Отрисовка узлов
            drawLine(); //Отрисовка ребер
            label.Visibility = Visibility.Hidden; //Спрятать доп информацию

            UpdateInformation(); //Обновить доп информацию
            timer.Tick += new EventHandler(TimerTick); //Обработчик срабатывания таймера 
            timer.Interval = new TimeSpan(0, 0, 1); //Установка интервала таймера

            MoveTimer.Tick += new EventHandler(MoveMessage);
        }
        public void InitializeNodes() //Инициализация ребер
        {
            for (int i = 0; i < N; i++)
                Nodes.Add(new Node(i));
            /*         
            Random r = new Random();
            for (int i = 0; i < 500; i++) 
            {
                int f = r.Next(0, Nodes.Count), g = r.Next(0, Nodes.Count);
                if (f != g)
                    Nodes[f].SendMessage(Nodes[g]);
            }*/
        }
        #region Drawing
        public void draw() //Отрисовка эллипсов
        {
            double size = 40; //Размер эллипса
            double Fi = 360 / N; //Угол в полярных координатах
            double radius = (Height - 100) / 2; //Радиус окружности 
            double[] shift = new double[2]; //Сдвиг относительно центра окна
            shift[0] = this.Width / 2 - size; //по х
            shift[1] = this.Height / 2 - size; //по у

            for (int i = 0; i < Nodes.Count; i++)
            {
                EllipseList.Add(new Ellipse());
                EllipseList[i].Width = size;
                EllipseList[i].Height = size;
                EllipseList[i].Fill = Brushes.LightBlue; //Цвет внутренностей эллипса
                EllipseList[i].Stroke = Brushes.DarkBlue; //Цвет контура
                EllipseList[i].StrokeThickness = 3; //Толщина контура
                //EllipseList[i].StrokeDashCap = PenLineCap.Round;
                //EllipseList[i].StrokeDashArray = new DoubleCollection(new double[] { 3, 2 });

                double x = radius * Math.Cos(Math.PI * Fi * i / 180), y = radius * Math.Sin(Math.PI * Fi * i / 180);

                Canvas.SetLeft(EllipseList[i], x + shift[0]); //Положение эллипса на экране по х
                Canvas.SetTop(EllipseList[i], y + shift[1]); //По у

                binding.Add(new Binding()); //Зависимость для подсветки номера вершины
                binding[i].Source = this.Nodes[i];
                binding[i].Path = new PropertyPath("No");

                tips.Add(new ToolTip()); //Подсказка всплывающая
                tips[tips.Count - 1].SetBinding(ContentControl.ContentProperty, binding[i]);
                tips[tips.Count - 1].Background = Brushes.White;
                tips[tips.Count - 1].HasDropShadow = true;
                EllipseList[i].ToolTip = tips[tips.Count - 1];

                EllipseList[i].MouseDown += EllipseMouseDown;
                this.MouseUp += EllipseMouseUp;
            }
        }
        public void drawLine() //Отрисовка ребер 
        {
            for (int i = 0; i < links.Count; i++)
                canvas.Children.Remove(links[i]);
            for (int i = 0; i < EllipseList.Count; i++)
                canvas.Children.Remove(EllipseList[i]);

            links.RemoveRange(0, links.Count);
            linkNode.RemoveRange(0, linkNode.Count);
            bindingLine.RemoveRange(0, bindingLine.Count);
            tipsLine.RemoveRange(0, tipsLine.Count);

            bool repeat = false; //Нарисовано ли такое же ребро между ребрами

            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int f = 0; f < Nodes[i].Edges.Count; f++)
                {
                    int g = 0;
                    for (; g < Nodes.Count; g++)
                        if (Nodes[i].Edges[f].Equals(Nodes[g]))
                            break;

                    for (int j = 0; j < links.Count; j++)
                        if ((links[j].X1 == Canvas.GetLeft(EllipseList[i]) + EllipseList[i].Width / 2 && links[j].Y1 == Canvas.GetTop(EllipseList[i]) + EllipseList[i].Height / 2) || ((links[j].X2 == Canvas.GetLeft(EllipseList[i]) + EllipseList[i].Width / 2 && links[j].Y2 == Canvas.GetTop(EllipseList[i]) + EllipseList[i].Height / 2)))
                            if ((links[j].X1 == Canvas.GetLeft(EllipseList[g]) + EllipseList[g].Width / 2 && links[j].Y1 == Canvas.GetTop(EllipseList[g]) + EllipseList[g].Height / 2) || ((links[j].X2 == Canvas.GetLeft(EllipseList[g]) + EllipseList[g].Width / 2 && links[j].Y2 == Canvas.GetTop(EllipseList[g]) + EllipseList[g].Height / 2)))
                            {
                                repeat = true;
                                break;
                            }

                    if (!repeat)
                    {
                        links.Add(new Line());
                        canvas.Children.Add(links[links.Count - 1]);
                        //links[links.Count - 1].StrokeDashCap = PenLineCap.Round;
                        //links[i].StrokeDashArray = new DoubleCollection(new double[] { 3, 4 });
                        links[links.Count - 1].Stroke = Brushes.Black; //Цвет ребра
                        links[links.Count - 1].StrokeThickness = 5; //Толщина ребра
                        links[links.Count - 1].X1 = Canvas.GetLeft(EllipseList[i]) + EllipseList[i].Width / 2; //Точки начала и конца 
                        links[links.Count - 1].Y1 = Canvas.GetTop(EllipseList[i]) + EllipseList[i].Height / 2;
                        links[links.Count - 1].X2 = Canvas.GetLeft(EllipseList[g]) + EllipseList[g].Width / 2;
                        links[links.Count - 1].Y2 = Canvas.GetTop(EllipseList[g]) + EllipseList[g].Height / 2;
                        linkNode.Add(new int[2]);
                        linkNode[linkNode.Count - 1][0] = i;
                        linkNode[linkNode.Count - 1][1] = g;

                        bindingLine.Add(new Binding());
                        bindingLine[bindingLine.Count - 1].Source = this.Nodes[i].EdgeWeight[f];

                        tipsLine.Add(new ToolTip());
                        tipsLine[tipsLine.Count - 1].SetBinding(ContentControl.ContentProperty, bindingLine[bindingLine.Count - 1]);
                        tipsLine[tipsLine.Count - 1].Background = Brushes.White;
                        tipsLine[tipsLine.Count - 1].HasDropShadow = true;

                        links[links.Count - 1].ToolTip = tipsLine[tipsLine.Count - 1];
                    }
                    repeat = false;
                }
            }
            for (int i = 0; i < EllipseList.Count; i++)
            {
                canvas.Children.Add(EllipseList[i]);
            }
        }
        #endregion
        #region Handlers
        void UpdateLines() //Обновление линий при перетягивании
        {
            for (int i = 0; i < linkNode.Count; i++)
            {
                links[i].X1 = Canvas.GetLeft(EllipseList[linkNode[i][0]]) + EllipseList[linkNode[i][0]].Width / 2;
                links[i].Y1 = Canvas.GetTop(EllipseList[linkNode[i][0]]) + EllipseList[linkNode[i][0]].Height / 2;
                links[i].X2 = Canvas.GetLeft(EllipseList[linkNode[i][1]]) + EllipseList[linkNode[i][1]].Width / 2;
                links[i].Y2 = Canvas.GetTop(EllipseList[linkNode[i][1]]) + EllipseList[linkNode[i][1]].Height / 2;
            }
        }
        void UpdateInformation(int edge = -1) //Delete Обновление информации в лэйбле
        {
            label.Content = "Mouse Down Bool = " + MouseDownBool + "\n" +
                            "Removal = (" + removal.X + "; " + removal.Y + ")\n" +
                            "Ellipse index = " + EllipseIndex + "\n" +
                            "Mouse position = (" + Mouse.GetPosition(this).X + "; " + Mouse.GetPosition(this).Y + ").\n" + 
                            "Move Timer interval (ms) " + MoveTimer.Interval.TotalMilliseconds + "\n" +
                            "timer interval (ms)" + timer.Interval.TotalMilliseconds;

            if (edge != -1)
                label.Content += "\nEdge #" + edge;
        }
        void EllipseMouseMove(object sender, MouseEventArgs e) //Перетаскивание эллипса
        {
            if (MouseDownBool)
            {
                Canvas.SetLeft(EllipseList[EllipseIndex], Mouse.GetPosition(this).X - removal.X);
                Canvas.SetTop(EllipseList[EllipseIndex], Mouse.GetPosition(this).Y - removal.Y);
                UpdateLines();
            }

            UpdateInformation();
        }
        void EllipseMouseDown(object sender, MouseButtonEventArgs e) //Обработка нажатия на эллипс
        {
            MouseDownBool = true;
            for (int i = 0; i < EllipseList.Count; i++)
            {
                if (EllipseList[i].Equals(sender))
                {
                    EllipseIndex = i;
                    removal.X = Mouse.GetPosition(this).X - Canvas.GetLeft(EllipseList[i]);
                    removal.Y = Mouse.GetPosition(this).Y - Canvas.GetTop(EllipseList[i]);
                    break;
                }
            }
            UpdateInformation();
        }
        void EllipseMouseUp(object sender, MouseButtonEventArgs e) //Обработка отпускания эллипса
        {
            MouseDownBool = false;
            EllipseIndex = -1;
            removal.X = 0;
            removal.Y = 0;
            UpdateInformation();
        }
        void MouseMoveUpdater(object sender, MouseEventArgs e) //Обновление информации с движением мишки
        {
            UpdateInformation();
        }
        void WindowInitialized(object sender, EventArgs e) //Создание вспомогательных окон
        {
            par.Owner = this;
            par.Show();

            log.Owner = this;
            log.Show();
        }
        public void TimerStart() //Включение таймера
        {
            if (timer.Interval != null)
                timer.Start();
            log.FormMatrix();
        }
        public void TimerStop() //Выключение таймера
        {
            if (timer.IsEnabled)
                timer.Stop();
            Stopwatch = Stopwatch.Subtract(Stopwatch);
            par.TimerTextBlock.Text = Stopwatch.ToString();
        }
        public void TimerPause() //Пауза
        {
            if (timer.IsEnabled)
                timer.Stop();
        }
        public void TimerIntervalChange(double index)
        {
            if (index >= 1)
                index = 1000 / (index * 5);
            else
                if(index <= -1)
                index = 1000 * Math.Abs(index);
            if (Math.Abs(index) < 1)
                index = 1000;

            timer.Interval = new TimeSpan(0, 0, 0, 0, Convert.ToInt16(index));
        }
        void TimerTick(object sender, EventArgs e)
        {
            int f = 0, g = 0;
            switch (state)
            {
                case (Models.Model_0):
                    Model_0(out f, out g);
                    break;
                case (Models.Model_1):
                    Model_1(out f, out g);
                    break;
                case (Models.Model_2):
                    Model_2(out f, out g);
                    break;
                case (Models.Model_3):
                    Model_3(out f, out g);
                    break;
                case (Models.Model_4):
                    Model_4(out f, out g);
                    break;
                case (Models.Model_Barabasi):
                    Model_Barabasi(out f, out g);
                    break;
                case (Models.Model_Barabasi_Plus):
                    Model_Barabasi_Plus(out f, out g);
                    break;
                case (Models.Model_Barabasi_Plus_Plus):
                    Model_Barabasi_Plus_Plus(out f, out g);
                    break;
            }

            drawLine();
            Stopwatch = Stopwatch.Add(new TimeSpan(0, 0, 1)); //Обновление данных секундомера
            if (g != -1)
                log.LogBox.Text += "\n" + Stopwatch + "\tSender: " + f + ", Reciever: " + g + " "; //Вывод данных в лог
            if (!par.TimerTextBlock.IsVisible)
                par.TimerTextBlock.Visibility = Visibility.Visible;
            par.TimerTextBlock.Text = Stopwatch.ToString(); //Вывод таймера
            log.FormMatrix(); //Построение матрицы весов
        } //Срабатывание таймера
        private void Window_LocationChanged(object sender, EventArgs e) //Привязка к главному окну
        {
            if (this.IsActive)
            {
                log.UpdatePosition();
                par.UpdatePosition();
            }
        }
        #endregion
        #region Models
        void Model_0(out int f, out int g)
        {
            Random r = new Random();

            do
            {
                g = r.Next(0, Nodes.Count);
                f = r.Next(0, Nodes.Count);
            } while (g == f);

            //Nodes[f].SendMessage(Nodes[g]);
            SendMessage(f, g);
        }
        void Model_1(out int f, out int g)
        {
            Random r = new Random();

            g = 0;
            f = r.Next(0, Nodes.Count);

            if (Nodes[f].EdgeWeight.Count == 0)
            {
                do
                {
                    g = r.Next(0, Nodes.Count);
                } while (g == f);
                //Nodes[f].SendMessage(Nodes[g]);
                SendMessage(f, g);
            }
            else
            {
                for (int i = 0; i < Nodes[f].EdgeWeight.Count; i++)
                {
                    if (Nodes[f].EdgeWeight[g] >= Nodes[f].EdgeWeight[i])
                        g = i;
                }
                for (int i = 0; i < Nodes.Count; i++)
                    if (Nodes[i].Equals(Nodes[f].Edges[g]))
                    {
                        g = i;
                        break;
                    }
                //Nodes[f].SendMessage(Nodes[g]);
                SendMessage(f, g);
            }
        }
        void Model_2(out int f, out int g)
        {
            Random r = new Random();

            g = 0;
            f = r.Next(0, Nodes.Count);

            if (Nodes[f].EdgeWeight.Count == 0)
            {
                do
                {
                    g = r.Next(0, Nodes.Count);
                } while (g == f);

                //Nodes[f].SendInformalMessage(Nodes[g], Stopwatch);
                SendMessage(f, g, true);
            }
            else
            {
                for (int i = 0; i < Nodes[f].EdgeWeight.Count; i++)
                {
                    if (Nodes[f].EdgeWeight[g] >= Nodes[f].EdgeWeight[i])
                        g = i;
                }
                for (int i = 0; i < Nodes.Count; i++)
                    if (Nodes[i].Equals(Nodes[f].Edges[g]))
                    {
                        g = i;
                        break;
                    }
                //Nodes[f].SendInformalMessage(Nodes[g], Stopwatch);
                SendMessage(f, g, true);
            }

            UtilizeAllNodes();
        }
        void Model_3(out int f, out int g)
        {
            Random r = new Random();

            g = 0;
            if (lastEdge == -1)
                f = r.Next(0, Nodes.Count);
            else
                f = lastEdge;

            if (Nodes[f].EdgeWeight.Count == 0)
            {
                do
                {
                    g = r.Next(0, Nodes.Count);
                } while (g == f);

                //Nodes[f].SendInformalMessage(Nodes[g], Stopwatch);
                SendMessage(f, g, true);
            }
            else
            {
                for (int i = 0; i < Nodes[f].EdgeWeight.Count; i++)
                {
                    if (Nodes[f].EdgeWeight[g] >= Nodes[f].EdgeWeight[i])
                        g = i;
                }
                for (int i = 0; i < Nodes.Count; i++)
                    if (Nodes[i].Equals(Nodes[f].Edges[g]))
                    {
                        g = i;
                        break;
                    }
                //Nodes[f].SendInformalMessage(Nodes[g], Stopwatch);
                SendMessage(f, g, true);
            }

            if (lastEdge == -1)
                lastEdge = g;
            else
                lastEdge = -1;

            UtilizeAllNodes();
        }
        void Model_4(out int f, out int g)
        {
            Random r = new Random();

            List<int> history = new List<int>();

            g = 0;
            if (lastEdge == -1)
            {
                f = r.Next(0, Nodes.Count);
                log.LogBox.Text += "\nRandom Sender:" + f;
            }
            else
                f = lastEdge;

            if (Nodes[f].EdgeWeight.Count == 0)
            {
                do
                {
                    g = r.Next(0, Nodes.Count);
                    log.LogBox.Text += "\nRandom Receiver:" + g;
                } while (g == f);

                //Nodes[f].SendInformalMessage(Nodes[g], Stopwatch);
                SendMessage(f, g, true);
            }
            else
            {
                int Random = r.Next(1, Nodes[f].EdgeWeight.Count);

                for (int j = 0; j < Random; j++)
                {
                    int receiver;
                    do
                    {
                        receiver = r.Next(Nodes[f].EdgeWeight.Count);
                        for (int i = 0; i < Nodes.Count; i++)
                            if (Nodes[i].Equals(Nodes[f].Edges[receiver]))
                            {
                                receiver = i;
                                break;
                            }
                    } while (history.Contains(receiver) || receiver == f);

                    history.Add(receiver);
                    //Nodes[f].SendInformalMessage(Nodes[receiver], Stopwatch);
                    SendMessage(f, receiver, true);
                    log.LogBox.Text += "\n" + Stopwatch + " G Sender: " + f + ", Reciever: " + receiver + " ";
                }
                g = -1;
            }

            if (g == -1)
            {
                lastEdge = history[r.Next(0, history.Count)];
            }
            else
                lastEdge = g;

            UtilizeAllNodes();
        }
        void Model_Barabasi(out int f, out int g)
        {
            Random r = new Random();
            List<int> Recipients = new List<int>();

            N++;
            Nodes.Add(new Node(Nodes.Count));

            int max = 0;
            int number = numEdges; //Число вершин с которыми надо связать

            for (int i = 0; i < Nodes.Count - 1; i++)
                if (Nodes[i].Degree() > max)
                    max = Nodes[i].Degree();

            do
            {
                for (int i = 0; i < Nodes.Count - 1; i++)
                    if (Nodes[i].Degree() == max)
                        Recipients.Add(i);

                if (Recipients.Count == number)
                {
                    for (int i = 0; i < Recipients.Count; i++)
                    {
                        //Nodes[Nodes.Count - 1].SendMessage(Nodes[Recipients[i]]);
                        SendMessage(Nodes.Count - 1, Recipients[i]);
                        log.AddLog("\n" + Stopwatch + " B Sender: " + (Nodes.Count - 1) + ", Reciever: " + Recipients[i] + " ");
                    }
                    break;
                }
                if (Recipients.Count > number)
                {
                    List<int> history = new List<int>();
                    int index;
                    do
                    {
                        do
                        {
                            index = r.Next(0, Recipients.Count);
                        } while (history.Contains(index));

                        history.Add(index);
                        //Nodes[Nodes.Count - 1].SendMessage(Nodes[Recipients[index]]);
                        SendMessage(Nodes.Count - 1, Recipients[index]);
                        log.AddLog("\n" + Stopwatch + " B Sender: " + (Nodes.Count - 1) + ", Reciever: " + Recipients[index] + " ");
                        --number;
                    } while (number > 0);
                    break;
                }
                if (Recipients.Count < number)
                {
                    for (int i = 0; i < Recipients.Count; i++)
                    {
                        //Nodes[Nodes.Count - 1].SendMessage(Nodes[Recipients[i]]);
                        SendMessage(Nodes.Count - 1, Recipients[i]);
                        log.AddLog("\n" + Stopwatch + " B Sender: " + (Nodes.Count - 1) + ", Reciever: " + Recipients[i] + " ");
                    }
                    Recipients.RemoveRange(0, Recipients.Count);
                    max--;
                }
            } while (true);

            binding.RemoveRange(0, binding.Count);
            tips.RemoveRange(0, tips.Count);
            for (int i = 0; i < EllipseList.Count; i++)
                canvas.Children.Remove(EllipseList[i]);
            EllipseList.RemoveRange(0, EllipseList.Count);

            for (int i = 0; i < message.Count; i++)
            {
                canvas.Children.Remove(message[i]);
                message.RemoveAt(i);
                agents.RemoveAt(i);
                if(i < segment.Count)
                    segment.RemoveAt(i);
                i--;
            }

            draw();
            f = -1;
            g = -1;
        }
        void Model_Barabasi_Plus(out int f, out int g)
        {
            Random r = new Random();
            List<int> Recipients = new List<int>();

            if (period == counter)
            {
                counter = 0;
                N++;
                Nodes.Add(new Node(Nodes.Count));

                int max = 0;
                int number = numEdges; //Число вершин с которыми надо связать

                for (int i = 0; i < Nodes.Count - 1; i++)
                    if (Nodes[i].Degree() > max)
                        max = Nodes[i].Degree();

                do
                {
                    for (int i = 0; i < Nodes.Count - 1; i++)
                        if (Nodes[i].Degree() == max)
                            Recipients.Add(i);

                    if (Recipients.Count == number)
                    {
                        for (int i = 0; i < Recipients.Count; i++)
                        {
                            //Nodes[Nodes.Count - 1].SendMessage(Nodes[Recipients[i]]);
                            SendMessage(Nodes.Count - 1, Recipients[i]);
                            log.AddLog("\n" + Stopwatch + " B Sender: " + (Nodes.Count - 1) + ", Reciever: " + Recipients[i] + " ");
                        }
                        break;
                    }
                    if (Recipients.Count > number)
                    {
                        List<int> history = new List<int>();
                        int index;
                        do
                        {
                            do
                            {
                                index = r.Next(0, Recipients.Count);
                            } while (history.Contains(index));

                            history.Add(index);
                            //Nodes[Nodes.Count - 1].SendMessage(Nodes[Recipients[index]]);
                            SendMessage(Nodes.Count - 1, Recipients[index]);
                            log.AddLog("\n" + Stopwatch + " B Sender: " + (Nodes.Count - 1) + ", Reciever: " + Recipients[index] + " ");
                            --number;
                        } while (number > 0);
                        break;
                    }
                    if (Recipients.Count < number)
                    {
                        for (int i = 0; i < Recipients.Count; i++)
                        {
                            //Nodes[Nodes.Count - 1].SendMessage(Nodes[Recipients[i]]);
                            SendMessage(Nodes.Count - 1, Recipients[i]);
                            log.AddLog("\n" + Stopwatch + " B Sender: " + (Nodes.Count - 1) + ", Reciever: " + Recipients[i] + " ");
                        }
                        Recipients.RemoveRange(0, Recipients.Count);
                        max--;
                    }
                } while (true);

                for (int i = 0; i < message.Count; i++)
                {
                    canvas.Children.Remove(message[i]);
                    message.RemoveAt(i);
                    agents.RemoveAt(i);
                    if (i < segment.Count)
                        segment.RemoveAt(i);
                    i--;
                }

                binding.RemoveRange(0, binding.Count);
                tips.RemoveRange(0, tips.Count);
                for (int i = 0; i < EllipseList.Count; i++)
                    canvas.Children.Remove(EllipseList[i]);
                EllipseList.RemoveRange(0, EllipseList.Count);
                draw();
            }
            else
            {
                do
                {
                    g = r.Next(0, Nodes.Count);
                    f = r.Next(0, Nodes.Count);
                } while (g == f);

                //Nodes[f].SendMessage(Nodes[g]);
                SendMessage(f, g);
                log.AddLog("\n" + Stopwatch + " B Sender: " + f + ", Reciever: " + g + " ");
                counter++;
            }
            f = -1;
            g = -1;
        }
        void Model_Barabasi_Plus_Plus(out int f, out int g)
        {
            Random r = new Random();
            g = 0;

            if (period == counter)
            {
                counter = 0;
                N++;
                Nodes.Add(new Node(Nodes.Count));
                f = r.Next(0, Nodes.Count - 1);
                g = Nodes.Count - 1;
                //Nodes[f].SendInformalMessage(Nodes[g], Stopwatch);
                SendMessage(f, g, true);
                binding.RemoveRange(0, binding.Count);
                tips.RemoveRange(0, tips.Count);
                for (int i = 0; i < EllipseList.Count; i++)
                    canvas.Children.Remove(EllipseList[i]);
                EllipseList.RemoveRange(0, EllipseList.Count);
                log.LogBox.Text += "\nAdd new Edge #" + (N - 1);

                for (int i = 0; i < message.Count; i++)
                {
                    canvas.Children.Remove(message[i]);
                    message.RemoveAt(i);
                    agents.RemoveAt(i);
                    if (i < segment.Count)
                        segment.RemoveAt(i);
                    i--;
                }

                draw();
            }
            else
            {
                f = r.Next(0, Nodes.Count);
                if (Nodes[f].EdgeWeight.Count == 0)
                {
                    do
                    {
                        g = r.Next(0, Nodes.Count);
                    } while (g == f);

                    //Nodes[f].SendInformalMessage(Nodes[g], Stopwatch);
                    SendMessage(f, g, true);
                }
                else
                {
                    g = r.Next(0, Nodes[f].EdgeWeight.Count);
                    if (Nodes[f].EdgeWeight[g] > trustLevel)
                    {
                        log.AddLog("\nTrust level achived");
                        int temp = 0;
                        for (int i = 0; i < Nodes.Count; i++)
                        {
                            if (Nodes[i].Equals(Nodes[f].Edges[g]))
                            {
                                g = i;
                                break;
                            }
                        }
                        for (int i = 0; i < Nodes[g].EdgeWeight.Count; i++)
                        {
                            if (Nodes[g].EdgeWeight[temp] <= Nodes[g].EdgeWeight[i] && g != f)
                                temp = i;
                        }
                        for (int i = 0; i < Nodes.Count; i++)
                        {
                            if (Nodes[i].Equals(Nodes[g].Edges[temp]))
                            {
                                temp = i;
                                break;
                            }
                        }
                        if (!Nodes[f].Edges.Contains(Nodes[temp]) && f != temp)
                        {
                            g = temp;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Nodes.Count; i++)
                        {
                            if (Nodes[i].Equals(Nodes[f].Edges[g]))
                            {
                                g = i;
                                break;
                            }
                        }
                    }
                    //Nodes[f].SendInformalMessage(Nodes[g], Stopwatch);
                    SendMessage(f, g, true);
                    counter++;
                }
            }
        }
        #endregion

        public void RandomMessage(int number = 1, bool informal = false) //Случайное сообщение 
        {
            Random r = new Random();
            for (int i = 0; i < number; i++)
            {
                int f, g;
                do
                {
                    g = r.Next(0, Nodes.Count);
                    f = r.Next(0, Nodes.Count);
                } while (g == f);

                if (g == f)
                    log.LogBox.Text += "ALARM!";

                if (informal) //Если нам нужно хранить дату, то отправляем ее 
                    SendMessage(f, g, true);
                else
                    SendMessage(f, g);

                log.LogBox.Text += "\n" + Stopwatch + " R Sender: " + f + ", Reciever: " + g;
                drawLine();
            }
        }
        public void RemoveAll(bool del = false)
        {
            for (int i = 0; i < links.Count; i++)
                canvas.Children.Remove(links[i]);

            links.RemoveRange(0, links.Count);
            linkNode.RemoveRange(0, linkNode.Count);
            bindingLine.RemoveRange(0, bindingLine.Count);
            tipsLine.RemoveRange(0, tipsLine.Count);
            if (!del)
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    Nodes[i].Edges.RemoveRange(0, Nodes[i].Edges.Count);
                    Nodes[i].EdgeWeight.RemoveRange(0, Nodes[i].EdgeWeight.Count);
                }
                TimerStop();
            }
        } //Удалить все данные
        void UtilizeAllNodes()
        {
            string logger = "";
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Utilize(limit, Stopwatch, out logger);
                log.AddLog(logger);
            }
        } //Удалить устаревшие связи
        void SendMessage(int senderNo, int receiverNo, bool informal = false)
        {
            if (informal)
                Nodes[senderNo].SendInformalMessage(Nodes[receiverNo], Stopwatch);
            else
                Nodes[senderNo].SendMessage(Nodes[receiverNo]);

            
            MoveTimer.Interval = new TimeSpan(0, 0, 0, 0, 30);

            double speed = timer.Interval.TotalMilliseconds / MoveTimer.Interval.TotalMilliseconds;

            agents.Add(new int[] { senderNo, receiverNo }); //запоминаем отправителя и получателя
            message.Add(new Ellipse());
            message[message.Count - 1].Width = 20; //Параметры эллипса
            message[message.Count - 1].Height = 20;
            message[message.Count - 1].Fill = Brushes.Green; //Цвет внутренностей эллипса
            
            numSegment.Add(new int[] { 0, Convert.ToInt16(speed) });

            //Canvas.SetLeft(message[message.Count - 1], Canvas.GetLeft(EllipseList[agents[agents.Count - 1][0]]) + EllipseList[agents[agents.Count - 1][0]].Width / 2 - message[message.Count - 1].Width / 2);
            //Canvas.SetTop(message[message.Count - 1], Canvas.GetTop(EllipseList[agents[agents.Count - 1][0]]) + EllipseList[agents[agents.Count - 1][0]].Height / 2 - message[message.Count - 1].Height / 2);
            Canvas.SetTop(message[message.Count - 1], 0);
            
            //double segment1 = (Canvas.GetLeft(EllipseList[agents[agents.Count - 1][1]]) - Canvas.GetLeft(EllipseList[agents[agents.Count - 1][0]])) / speed;
            //double segment2 = (Canvas.GetTop(EllipseList[agents[agents.Count - 1][1]]) - Canvas.GetTop(EllipseList[agents[agents.Count - 1][0]])) / speed;

            //segment.Add(new double[] { segment1, segment2 });

            if (!MoveTimer.IsEnabled)            
                MoveTimer.Start();
        } //Вспомогательный метод отправки сообщения для отображения
        void MoveMessage(object sender, EventArgs e) //Движение сообщения
        {
            for (int i = 0; i < message.Count; i++)
            {
                if(Canvas.GetTop(message[i]) == 0)
                {
                    double speed = timer.Interval.TotalMilliseconds / MoveTimer.Interval.TotalMilliseconds;

                    canvas.Children.Add(message[i]);
                    Canvas.SetLeft(message[i], Canvas.GetLeft(EllipseList[agents[i][0]]) + EllipseList[agents[i][0]].Width / 2 - message[i].Width / 2);
                    Canvas.SetTop(message[i], Canvas.GetTop(EllipseList[agents[i][0]]) + EllipseList[agents[i][0]].Height / 2 - message[i].Height / 2);

                    double segment1 = (Canvas.GetLeft(EllipseList[agents[i][1]]) - Canvas.GetLeft(EllipseList[agents[i][0]])) / speed;
                    double segment2 = (Canvas.GetTop(EllipseList[agents[i][1]]) - Canvas.GetTop(EllipseList[agents[i][0]])) / speed;

                    segment.Add(new double[] { segment1, segment2 });
                }
                if (Math.Abs(Canvas.GetLeft(message[i]) - Canvas.GetLeft(EllipseList[agents[i][1]])) < 20 && Math.Abs(Canvas.GetTop(message[i]) - Canvas.GetTop(EllipseList[agents[i][1]])) < 20)
                {
                    canvas.Children.Remove(message[i]);
                    message.RemoveAt(i);
                    agents.RemoveAt(i);
                    segment.RemoveAt(i);
                    i--;
                }
                else
                {
                    Canvas.SetLeft(message[i], Canvas.GetLeft(message[i]) + segment[i][0]);
                    Canvas.SetTop(message[i], Canvas.GetTop(message[i]) + segment[i][1]);
                }
            }
            if (message.Count == 0)
                MoveTimer.Stop();
        }
    }
}

