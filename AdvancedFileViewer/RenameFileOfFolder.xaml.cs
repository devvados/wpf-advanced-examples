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

namespace AdvancedFileViewer
{
    public partial class RenameFileOfFolder : Window
    {
        public static string NewFileName;
        public RenameFileOfFolder()
        {
            InitializeComponent();
        }

        private void ButOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(TbNewName.Text == "")
                    throw new Exception("Задайте новое имя!");
                NewFileName = TbNewName.Text;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TbNewName_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            if (Char.IsLetter(c))
                e.Handled = false;
            else
                e.Handled = true;

            base.OnPreviewTextInput(e);
        }
    }
}
