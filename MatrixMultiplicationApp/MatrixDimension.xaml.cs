using System;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace MatrixMultiplicationApp
{
    public partial class MatrixDimension : Window
    {
        public MatrixDimension()
        {
            InitializeComponent();
            TbDim.Focus();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void bDim_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TbDim.Text == "")
                    throw new Exception("Введите размерность");
                else
                {
                    MainWindow.N = Convert.ToInt32(TbDim.Text);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
