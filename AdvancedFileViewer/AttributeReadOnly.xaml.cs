using System;
using System.IO;
using System.Windows;

namespace AdvancedFileViewer
{
    public partial class AttributeReadOnly : Window
    {
        public static string SetReadOnly;
        public AttributeReadOnly()
        {
            InitializeComponent();
        }

        private void ButOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RbSet.IsChecked != true && RbOff.IsChecked != true)
                    throw new Exception("Отметье поле");
                
                if (RbSet.IsChecked == true)
                {
                    SetReadOnly = "Set";
                }
                else if (RbOff.IsChecked == true)
                {
                    SetReadOnly = "Off";
                }
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
