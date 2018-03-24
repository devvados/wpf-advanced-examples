using System;
using System.Collections.Generic;
using System.IO;
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
    /// <summary>
    /// Interaction logic for CreateFolderOrFile.xaml
    /// </summary>
    public partial class CreateFolderOrFile : Window
    {
        public static string FolderOrFile = "";
        public static string FileName = "";
        public CreateFolderOrFile()
        {
            InitializeComponent();
        }

        private void ButCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RbFile.IsChecked == false && RbFolder.IsChecked == false)
                    throw new Exception("Выберите, какой объект создать!");
                if(TbName.Text == "")
                    throw new Exception("Введите имя создаваемого объекта!");
                if (RbFolder.IsChecked == true)
                {
                    FolderOrFile = "DirectoryInfo";
                }
                else if (RbFile.IsChecked == true)
                {
                    FolderOrFile = "FileInfo";
                }
                FileName = TbName.Text;

                if (FolderOrFile !="" && FileName !="") Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
