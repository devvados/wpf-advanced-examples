using System;
using System.Windows;
using System.Threading;
using System.Windows.Controls;

namespace Pool
{
    public partial class MainWindow : Window
    {
        private readonly ListBox[] _lBox;
        private int _i = 0;
        public MainWindow()
        {
            InitializeComponent();
            _lBox = new[] { LBox1, LBox2, LBox3 };
    }

        private void ButStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TbMax.Text == "")
                    throw new Exception("Введите максимальное значение!");

                int nWorkerThreads;
                int nCompletionThreads;

                ThreadPool.GetMaxThreads(out nWorkerThreads, out nCompletionThreads);
                TbThreads.Text = "Максимальное количество доступных потоков: " + nCompletionThreads.ToString() + "\nПоток: ";

                for (var i = 0; i < 3; i++)
                {
                    ThreadPool.QueueUserWorkItem(InsertValue, i);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }        
        }
        void InsertValue(object state)
        {
            var max = 0;
            var val = (int)state;

            TbMax.Dispatcher.Invoke(new Action(() =>
            {
                max = int.Parse(TbMax.Text);
            }));
            var lb = _lBox[val];

            //Пишем в ListBox
            for (var i = 0; i < max; i++)
            {
                lb.Dispatcher.Invoke(new Action(() =>
                {
                    lb.Items.Add(i);
                }));
            }
            TbMax.Dispatcher.Invoke(new Action(()=>
            {
                TbThreads.Text += "Поток " + Thread.CurrentThread.ManagedThreadId.ToString() + "; ";
            }));

            //Засыпание потока на 1-10 секунд
            var rand = new Random();
            var temp = rand.Next(1, 10);
            Thread.Sleep(temp);
        }
    }
}
