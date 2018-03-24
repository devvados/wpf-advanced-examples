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

namespace CashAccount
{
    public partial class InfoWindow : Window
    {
        public TextBox[] Info;
        public RadioButton[] rad;

        public InfoWindow()
        {
            InitializeComponent();
            Info = new TextBox[] { Info1, Info2, Info3, Info4, Info5 };
            rad = new RadioButton[] { rb1, rb2, rb3, rb4, rb5 };
            foreach (TextBox control in Info)
                control.IsReadOnly = true;
        }

        public void button_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < rad.Length; i++)
            {
                if(rad[i].IsChecked == true)
                {
                    Info[i].Text = "";
                }
            }
        }
    }
}
