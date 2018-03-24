using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace SearchStringInFolder
{
    public class FileSearch
    {
        public string FullPath { get; set; }
        public string Index { get; set; }
    }
    public partial class MainWindow : Window
    {
        private DirectoryInfo _dirInfo;
        private List<string> _filesToSearch = new List<string>();
        private List<int>[] _indexesOf;
        public string SelectedPath { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectDir_Click(object sender, RoutedEventArgs e)
        {
            _filesToSearch.Clear();
            var fldBd = new FolderBrowserDialog();
            var dialRes = fldBd.ShowDialog();
            if (dialRes == System.Windows.Forms.DialogResult.OK)
            {
                var dirPath = fldBd.SelectedPath;
                SelectedPath = dirPath;
                _dirInfo = new DirectoryInfo(dirPath);

                if (_dirInfo != null)
                {
                    var files = _dirInfo.GetFiles();

                    foreach (var newItemFiles in files)
                    {
                        _filesToSearch.Add(newItemFiles.FullName);
                    }
                }
                else
                {
                    _dirInfo = null;
                }
            }
        }

        //Добавление строчки в LIstView
        private void FillListView()
        {
            ListViewRes.Items.Clear();
            for (var i = 0; i < _filesToSearch.Count; i++)
            {
                var toListView = string.Join(",", new List<int>(_indexesOf[i]));
                ListViewRes.Items.Add(string.IsNullOrWhiteSpace(toListView)
                    ? new FileSearch {FullPath = _filesToSearch[i], Index = "Файл пуст"}
                    : new FileSearch {FullPath = _filesToSearch[i], Index = toListView});
            }
        }

        private void ButFind_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TbSearchString.Text == "")
                    throw new Exception("Введите строку для поиска!");
                if (_dirInfo == null)
                    throw new Exception("Директория не выбрана!");
                var filesCount = _filesToSearch.Count;
                var searchString = TbSearchString.Text;

                _indexesOf = new List<int>[filesCount];

                Parallel.For(0, filesCount, i =>
                {
                    using (var sReader = new StreamReader(_filesToSearch[i], Encoding.Unicode))
                    {
                        var fileLines = new List<string>();
                        while (!sReader.EndOfStream)
                        {
                            fileLines.Add(sReader.ReadLine());
                        }
                        _indexesOf[i] = Search(fileLines, searchString);
                    }
                });
                FillListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Поиск первого совпадения строки 
        private static List<int> Search(IEnumerable<string> strList, string pattern)
        {
            var temp = new List<string>(strList);
            return temp.Select(t => t.IndexOf(pattern, StringComparison.Ordinal)).ToList();
        }
    }
}
