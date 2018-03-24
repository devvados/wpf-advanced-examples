using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Async
{ 
    public class InException : Exception
    {
        public InException(string message) : base(message) { }
    }
    public partial class MainWindow : Window
    {
        static int inc, dec, res = 1;
        TimerCallback timer;
        Timer time;

        public MainWindow()
        {
            InitializeComponent();
        }
        public void Start(object state)
        {
            var mydel = new Func<int>(Inc);
            var mydel1 = new Func<int>(Dec);
            var delres = new Func<int>(Sub);

            IAsyncResult asr2 = delres.BeginInvoke(null, null);
            IAsyncResult asr = mydel.BeginInvoke(null, null);
            IAsyncResult asr1 = mydel1.BeginInvoke(null, null);

            tbIncRes.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (asr.IsCompleted)
                    tbIncRes.Text += mydel.EndInvoke(asr).ToString() + "\n";
                else Thread.Sleep(1000);
            }));
            tbDecRes.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (asr1.IsCompleted)
                    tbDecRes.Text += mydel1.EndInvoke(asr1).ToString() + "\n";
                else Thread.Sleep(1000);
            }));
            tbDif.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (asr2.IsCompleted)
                {
                    res = delres.EndInvoke(asr2);
                    tbDif.Text += res.ToString() + "\n";
                    if (res == 0) time.Dispose();
                }
                else Thread.Sleep(1000);
            }));
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                inc = Int32.Parse(tbInc.Text);
                dec = Int32.Parse(tbDec.Text);
                if ((inc % 2 != 0 && dec % 2 == 0) || (inc >= dec) || (inc % 2 == 0 && dec % 2 != 0))
                    throw new InException("Ошибка ввода параметров");
                timer = new TimerCallback(Start);
                time = new Timer(timer, null, 0, 1500);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static int Inc()
        {
            return inc++;
        }
        public static int Dec()
        {
            return dec--;
        }
        public static int Sub()
        {
            int c = inc - dec;
            return c;
        }
    }
}
