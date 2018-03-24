using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

namespace AdvancedFileViewer
{
    /// <summary>
    /// Interaction logic for Code.xaml
    /// </summary>
    public partial class Code : Window
    {
        public Code()
        {
            InitializeComponent();
        }

        private void ButCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RbTripleDes.IsChecked == true)
                {
                    MainWindow.SelectedMethod = RbTripleDes.Content.ToString();
                }
                else if (RbRijndael.IsChecked == true)
                {
                    MainWindow.SelectedMethod = RbRijndael.Content.ToString();
                }
                else if (RbRc2.IsChecked == true)
                {
                    MainWindow.SelectedMethod = RbRc2.Content.ToString();
                }
                else if (RbRsa.IsChecked == true)
                {
                    MainWindow.SelectedMethod = RbRsa.Content.ToString();
                }
                else
                {
                    throw new Exception("Выберите метод шифрования!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
