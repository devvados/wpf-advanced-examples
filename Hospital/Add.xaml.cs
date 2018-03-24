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

namespace Hospital
{
    public class ConditionException : Exception
    {
        public ConditionException(string message) : base(message) { }
    }
    public partial class Add : Window
    {
        public Add()
        {
            InitializeComponent();
            tbId.TextChanged += HandleChar;
        }

        public void HandleChar(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (System.Text.RegularExpressions.Regex.IsMatch(tb.Text, "^[0-9]")) 
            {
                tb.Text = "";
            }
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            string cond = "";
            string name = tbId.Text;
            if(chCondition.IsChecked == true)
            {
                cond = "Здоров";
            }
            else
            {
                cond = "Болен";
            }
            tbId.Clear();
            chCondition.IsChecked = false;
            MainWindow.queue.Enqueue(new Patient(name, cond));      
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
