using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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

namespace CashAccount
{
    public class MyException : Exception
    {
        public string msg = "";
        public MyException(string message)
        {
            msg = message;
        }
    }
    public partial class MainWindow : Window
    {
        public CheckBox[] check;
        public TextBox[] name, count, cost;
        InfoWindow info;
        int countcheck = 0;

        public MainWindow()
        {
            InitializeComponent();
            info = new InfoWindow();
            info.Show();

            InitControls();
            InvisibleControls();
        }
        public void InitControls()
        {
            check = new CheckBox[] { cb1, cb2, cb3, cb4, cb5 };
            name = new TextBox[] { name1, name2, name3, name4, name5 };
            count = new TextBox[] { num1, num2, num3, num4, num5 };
            cost = new TextBox[] { cost1, cost2, cost3, cost4, cost5 };
        }

        public void InvisibleControls()
        {
            label.Visibility = Visibility.Hidden;
            label1.Visibility = Visibility.Hidden;
            label2.Visibility = Visibility.Hidden;
            for (int i = 0; i < 5; i++)
            {
                check[i].Visibility = Visibility.Hidden;
                name[i].Visibility = Visibility.Hidden;
                count[i].Visibility = Visibility.Hidden;
                cost[i].Visibility = Visibility.Hidden;
            }
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            int rb_count = 1, check_count = 1;
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    if (rb_count == 5)
                        throw new MyException("You must select an account!");
                    if (info.rad[i].IsChecked == true)
                    {
                        info.Info[i].Text = "";
                        for (int j = 0; j < 5; j++)
                        {
                            if (check_count == 5)
                                throw new MyException("You must create 1 or more positions!");
                            if (check[j].IsChecked == true)
                            {
                                if (name[j].Text != "" && count[j].Text != "" && cost[j].Text != "")
                                {
                                    info.Info[i].Text += check[j].Content + ". " + name[j].Text + "\n" + "Total = "
                                        + Convert.ToDouble(count[j].Text) * Convert.ToDouble(cost[j].Text) + "\n";
                                }
                                else throw new MyException("Fill all the boxes in checked lines!");
                            }
                            else check_count++;
                        }
                    }
                    else rb_count++;
                }
            }
            catch(MyException ex)
            {
                MessageBox.Show(ex.msg, "Exception");
            }
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            label.Visibility = Visibility.Visible;
            label1.Visibility = Visibility.Visible;
            label2.Visibility = Visibility.Visible;
            for (int i = 0; i < 5; i++)
            {
                if (check[i].Visibility == Visibility.Hidden)
                {
                    check[i].Visibility = Visibility.Visible;
                    name[i].Visibility = Visibility.Visible;
                    count[i].Visibility = Visibility.Visible;
                    cost[i].Visibility = Visibility.Visible;
                    break;
                }
            }
            countcheck++;
        }

        private void del_Click(object sender, RoutedEventArgs e)
        {
            
            for(int i = 0; i < 5; i++)
            {
                if (check[i].IsChecked == true)
                {
                    check[i].Visibility = Visibility.Hidden;
                    check[i].IsChecked = false;
                    name[i].Visibility = Visibility.Hidden;
                    name[i].Text = "";
                    count[i].Visibility = Visibility.Hidden;
                    count[i].Text = "";
                    cost[i].Visibility = Visibility.Hidden;
                    cost[i].Text = "";
                    countcheck--;
                }
                if(countcheck==0)
                {
                    label.Visibility = Visibility.Hidden;
                    label1.Visibility = Visibility.Hidden;
                    label2.Visibility = Visibility.Hidden;
                }
            }
        }
    }
}
