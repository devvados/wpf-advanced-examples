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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;

namespace AdvancedFileViewer
{
    public partial class MoveOrCopyDestination : Window
    {
        public static DirectoryInfo Dir;
        public MoveOrCopyDestination()
        {
            InitializeComponent();
        }

        private void ButSelectPath_Click(object sender, RoutedEventArgs e)
        {
            var fldBr = new FolderBrowserDialog();
            var selected = fldBr.ShowDialog();
            if (selected == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    var dirPath = fldBr.SelectedPath;
                    if (dirPath != null)
                    {
                        Dir = new DirectoryInfo(dirPath);
                    }
                    else throw new Exception("Выберите папку - назначение");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            Close();
        }
    }
}
