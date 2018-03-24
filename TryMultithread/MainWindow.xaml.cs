using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TryMultithread
{
    public partial class MainWindow : Window
    {
        private Task _task1, _task2;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartProgressBar()
        {
            ProgressBar.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                ProgressBar.IsIndeterminate = true;
            }));
        }

        private void ChangeText()
        {
            TbText.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                TbText.Text = "Начали";
            }));
        }

        private void BStart_OnClick(object sender, RoutedEventArgs e)
        {
            _task1 = Task.Factory.StartNew(StartProgressBar);
            _task2 = Task.Factory.StartNew(ChangeText);
        }

        private void BStop_OnClick(object sender, RoutedEventArgs e)
        {
            _task1.Dispose();
            _task2.Dispose();
            ProgressBar.IsIndeterminate = false;
            TbText.Text = "Конец";
        }
    }
}
