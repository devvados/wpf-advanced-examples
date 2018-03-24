using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ThreadInTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Timer _timer;
        private TimerCallback _tCallback;
        private List<string> _words;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            _tCallback = new TimerCallback(RefreshText);
            _timer = new Timer(_tCallback, null, 0, 10000);
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void RefreshText(object state)
        {
            _words = new List<string>();
            if (_words.Count > 0) _words.Clear();
            else
            {
                
            }
            
            TextBox.Dispatcher.Invoke(new Action((() =>
            {
                if (TextBox.Text != "")
                {
                    var splitText = TextBox.Text.Split(' ');
                    foreach (var str in splitText)
                        _words.Add(str);
                }
            })));

            listBox.Dispatcher.Invoke(new Action((() =>
            {
                listBox.Items.Clear();
                foreach (var str in _words)
                    listBox.Items.Add(str);
            })));
        }
    }
}
