using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace SpellCheckerApp
{
    public partial class MainWindow
    {
        Dictionary<string, int> _tempSortedDictionary;
        public static Dictionary<string, int> UniqueWordsDictionary;
        private List<int> _uniqueWordCount, _substringIndexes;
        List<string> _uniqueWords, _sug;
        private int _searchNext = 0;

        static Action[] _searchParemeters;
        private TimerCallback _timerCallback;
        private Timer _timer;

        readonly List<Label> _words;
        readonly List<ProgressBar> _stats;
        private int _rowCounter;

        #region Свойства (файл со словарем и текстами)
        public string Word = "";
        public string SearchSeq = "";
        public string MainText = "";
        public string TextToInsert { get; set; }
        public string SavedTextPath { get; set; }
        public string LoadTextPath { get; set; }
        public string DictTxtPath { get; set; }
        public string DictPath { get; set; }
        public bool CheckClicked { get; set; }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            InitializeAll();

            _words = new List<Label>();
            _stats = new List<ProgressBar>();
            
            //на случай, если понадобится именно подчеркивание
            //неправильных слов. Иначе долго подгружается словарь
            //TbInput.SpellCheck.IsEnabled = true;
            TbInput.ContextMenu = NewContextMenu();
        }

        //Инициализация
        public void InitializeAll()
        {
            _tempSortedDictionary = new Dictionary<string, int>();
            UniqueWordsDictionary = new Dictionary<string, int>();
            _searchParemeters = new Action[4];
            _substringIndexes = new List<int>();

            CheckClicked = false;
            TextToInsert = @"TextToInsert.txt";
            SavedTextPath  = @"SavedText.txt";
            LoadTextPath = @"TextToLoad.txt";
            DictTxtPath  = @"C:\Users\Вадим\Documents\Visual Studio 2015\Projects\6\SpellCheckerApp\SpellCheckerApp\dictionary.txt";
            DictPath  = @"C:\Users\Вадим\Documents\Visual Studio 2015\Projects\6\SpellCheckerApp\SpellCheckerApp\dictionary.lex";

            foreach (var ff in System.Drawing.FontFamily.Families)
            {
                CbFontFamily.Items.Add(ff.Name);
            }
            for (var i = 10; i < 25; i++)
            {
                CbFontSize.Items.Add(i);
            }
            CbFontFamily.SelectedIndex = 0;
            CbFontSize.SelectedIndex = 0;

            TbInput.MouseDoubleClick += TbInput_MouseDoubleClick;

            TbInput.FontFamily = new FontFamily(CbFontFamily.Items[0].ToString());
            TbInput.FontSize = Convert.ToUInt32(CbFontSize.Items[0].ToString());
        }

        #region Удаление пунктуации
        public string RemovePunct(string s)
        {
            var temp = s;
            //заменяем ненужные символы
            temp = temp.Replace('\n', ' ');
            temp = temp.Replace('\r', ' ');
            temp = temp.Replace('\t', ' ');
            //строим строку без пунктуации
            var sb = new StringBuilder();
            foreach (var c in temp.Where(c => char.IsLetter(c) || c == '-' || char.IsSeparator(c)))
            {
                sb.Append(c);
            }
            temp = sb.ToString().ToLower();

            return temp;
        }
        #endregion
        #region Загрузка или сохранение текста
        private void Append_Click(object sender, RoutedEventArgs e)
        {
            string insertedText;
            using (var sr = new StreamReader(TextToInsert, Encoding.Unicode))
            {
                insertedText = sr.ReadToEnd();
            }
            var caretInd = TbInput.CaretIndex;
            insertedText = RemovePunct(insertedText);
            TbInput.Text = TbInput.Text.Insert(caretInd, insertedText);
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            using (var sw = new StreamWriter(SavedTextPath))
            {
                sw.Write(TbInput.Text);
            }
        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            TbInput.Clear();
            //Indexes = new List<int>();
            using (var sw = new StreamReader(LoadTextPath, Encoding.Unicode))
            {
                TbInput.Text = sw.ReadToEnd();
            }
            //MainText = TbInput.Text.ToLower();
            _timerCallback = RefreshText;
            _timer = new Timer(_timerCallback, null, 0, 5000);
        }
        #endregion
        #region Проверка текста
        private void RefreshText(object state)
        {
            string[] strNoPunct = null;

            TbInput.Dispatcher.Invoke((() =>
            {
                MainText = TbInput.Text.ToLower();
                strNoPunct = RemovePunct(TbInput.Text).Split(' ');
            }));

            CheckClicked = true;
            UniqueWordsDictionary.Clear();
            //CreateContextMenu();
            foreach (var temp in strNoPunct)
            {
                var pattern = temp;
                if (UniqueWordsDictionary.ContainsKey(pattern))
                {
                    UniqueWordsDictionary[pattern]++;
                }
                else
                {
                    UniqueWordsDictionary.Add(pattern, 1);
                }
            }
            UniqueWordsDictionary.Remove("-");
            UniqueWordsDictionary.Remove("");
            UniqueWordsDictionary.Remove(" ");
            _uniqueWords = UniqueWordsDictionary.Keys.ToList();
            _uniqueWordCount = UniqueWordsDictionary.Values.ToList();
        }
        //private void Check_Click(object sender, RoutedEventArgs e)
        //{
        //    Task.Factory.StartNew(RefreshText);
        //    RefreshText();
        //    InitSearchTasks();
        //    CreateContextMenu();
        //}
        private void TbInput_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var spe = TbInput.GetSpellingError(0);
            if (spe != null)
                _sug = new List<string>(spe.Suggestions);

            var caret = Convert.ToInt32(TbInput.CaretIndex);

            var start = caret;
            while (char.IsLetter(TbInput.Text[caret]) && char.IsLetter(TbInput.Text[caret + 1]))
            {
                ++caret;
            }
            var end = caret;
            //составим слово
            Word = TbInput.Text.Substring(start, end - start + 1);
            CreateContextMenu();
        }
        #endregion
        
        #region Контекстное меню
        private ContextMenu NewContextMenu()
        {
            var cm = new ContextMenu();
            var m1 = new MenuItem {Header = "Добавить"};
            m1.Click += AddToDictionary; 
            cm.Items.Add(m1);
            return cm;
        }
        private void CreateContextMenu()
        {
            TbInput.ContextMenu = NewContextMenu();
            var caretIndex = TbInput.CaretIndex;
            var cmdIndex = 0;
            var spellingError = TbInput.GetSpellingError(caretIndex);
            if (spellingError == null) return;
            foreach (var mi in spellingError.Suggestions.Select(str => new MenuItem
            {
                Header = str,
                FontWeight = FontWeights.Bold,
                Command = EditingCommands.CorrectSpellingError,
                CommandParameter = str,
                CommandTarget = TbInput
            }))
            {
                TbInput.ContextMenu.Items.Insert(cmdIndex, mi);
                cmdIndex++;
            }
        }
        #endregion
        
        #region Добавление слова в пользовательский словарь
        private void AddToDictionary(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Word != "")
                {
                    //ловко меняем расширение файла, чтоб записать туда, что хочу
                    File.Move(DictPath, DictTxtPath);
                    using (var sw = new StreamWriter(DictTxtPath, true, Encoding.Unicode))
                    {
                        sw.WriteLine(Word);
                    }
                    File.Move(DictTxtPath, DictPath);
                }
                else if (Word == "") throw new Exception("Выделите слово двойным кликом!");
                Word = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Диаграмма
        private void InitDiagramControls()
        {
            _tempSortedDictionary = UniqueWordsDictionary.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            _rowCounter = 0;
            GridDiagram.Children.Clear();
            GridDiagram.RowDefinitions.Clear();
            _words.Clear();
            _stats.Clear();
            _uniqueWordCount = _tempSortedDictionary.Values.ToList();
            _uniqueWords = _tempSortedDictionary.Keys.ToList();

            for (var i = 0; i < _uniqueWords.Count; i++)
            {
                if (i == 100) break;
                _words.Add(new Label());
                _stats.Add(new ProgressBar());
                _words[i].Height = 25;
                _words[i].FontSize = 10;
                _words[i].Content = _uniqueWords[i].ToUpper() + ": " + _uniqueWordCount[i];
                _words[i].HorizontalContentAlignment = HorizontalAlignment.Left;
                _words[i].VerticalContentAlignment = VerticalAlignment.Center;

                double wordCount = _uniqueWordCount[i];
                double max = _tempSortedDictionary.Values.ToList()[0];
                _stats[i].Foreground = Brushes.BlueViolet;
                _stats[i].Width = 170;
                _stats[i].Minimum = 0;
                _stats[i].Maximum = 100;
                _stats[i].Value = (wordCount / max) * 100;
            }
        }
        private void createDiagram_Click(object sender, RoutedEventArgs e)
        {
            InitDiagramControls();
            var keys = _tempSortedDictionary.Keys.ToList();

            foreach (var str in _tempSortedDictionary.Keys)
            {
                var sb = new StringBuilder();
                foreach (var t in str.Where(t => char.IsLetter(t) || t == '-'))
                {
                    sb.Append(t);
                }
                keys.Add(sb.ToString());
            }
            keys.Remove("");     
            DrawDiagram();
        }
        private void NewRow()
        {
            var rowDefinition = new RowDefinition {Height = new GridLength(25)};
            GridDiagram.RowDefinitions.Add(rowDefinition);

            Grid.SetColumn(_words[_rowCounter], 0);
            Grid.SetRow(_words[_rowCounter], _rowCounter);
            Grid.SetColumn(_stats[_rowCounter], 1);
            Grid.SetRow(_stats[_rowCounter], _rowCounter);
            GridDiagram.Children.Add(_words[_rowCounter]);
            GridDiagram.Children.Add(_stats[_rowCounter]);
            _rowCounter++;
        }
        private void DrawDiagram()
        {
            foreach (var lab in _words)
            {
                NewRow();
            }
        }
        #endregion
        #region Поиск редких слов и так далее
        private static Dictionary<string, int> SortDictionaryByValue(Dictionary<string, int> dict)
        {
            var tmp = dict.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            return tmp;
        }
        private int SearchStringInText(string search)
        {
            var index = MainText.LastIndexOf(search, StringComparison.Ordinal);
            MainText = MainText.Remove(index, search.Length);
            return index;
        }
        #endregion
        #region Отображение каждого совпадения 
        private void bAnalyse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TbSequence.Text == "")
                    throw new Exception("Введите последовательность!");
                if (TbInput.Text == "")
                    throw new Exception("Заполните поле ввода!");
                MainText = TbInput.Text.ToLower();
                SearchSeq = TbSequence.Text.ToLower();
                SearchCompare();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SearchCompare()
        {
            _substringIndexes.Clear();
            try
            {
                while (MainText.IndexOf(SearchSeq, StringComparison.Ordinal) != -1)
                {
                    var getIndex = SearchStringInText(SearchSeq);
                    _substringIndexes.Add(getIndex);
                }
                _substringIndexes.Reverse();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void nextSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_substringIndexes.Count == 0)
                    throw new Exception("Совпадений нет!");
                try
                {
                    if (_searchNext == _substringIndexes.Count)
                        throw new Exception("Больше совпадений нет!");
                    TbInput.Focus();
                    TbInput.Select(_substringIndexes[_searchNext], TbSequence.Text.Length);
                    _searchNext++;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Настройки шрифта
        private void BSetFont_Click(object sender, RoutedEventArgs e)
        {
            if (CbFontFamily.SelectedIndex != -1 && CbFontSize.SelectedIndex != -1)
            {
                TbInput.FontSize = Convert.ToInt32(CbFontSize.SelectedItem.ToString());
                TbInput.FontFamily = new FontFamily(CbFontFamily.SelectedItem.ToString());
            }
        }
        #endregion
        
        #region Поиск
        private void InitSearchTasks()
        {
            var sortedDictionary = SortDictionaryByValue(UniqueWordsDictionary);
            var keys = new List<string>();
            var values = sortedDictionary.Values.ToList();

            var shortWord = new List<string>();
            var longWord = new List<string>();
            var repWord = new List<string>();
            var rareWord = new List<string>();

            foreach (var str in sortedDictionary.Keys)
            {
                var sb = new StringBuilder();
                foreach (var t in str.Where(t => char.IsLetter(t) || t == '-'))
                {
                    sb.Append(t);
                }
                keys.Add(sb.ToString());
            }
            keys.Remove("");
            sortedDictionary.Remove("");
            var keysSortedByLength = keys.OrderBy(str => str.Length).ToList();

            _searchParemeters[0] = () =>
            {
                shortWord.Clear();
                CbShortWord.Items.Clear();
                //поиск самого короткого слова
                var count = keysSortedByLength.Count;
                shortWord.Add(keysSortedByLength[0]);
                for (var i = 1; i < count; i++)
                {
                    if (shortWord[0].Length == keysSortedByLength[i].Length)
                    {
                        shortWord.Add(keysSortedByLength[i]);
                    }
                    else
                        break;
                }
                foreach (var s in shortWord)
                {
                    var sb = new StringBuilder();
                    foreach (var t in s.Where(t => !char.IsPunctuation(t)))
                    {
                        sb.Append(t);
                    }
                    CbShortWord.Items.Add(s + ": " + sb.Length);
                }
                CbShortWord.SelectedIndex = 0;
            };
            _searchParemeters[1] = () =>
            {
                longWord.Clear();
                CbLongWord.Items.Clear();
                //поиск самого длинного слова
                var count = keysSortedByLength.Count;
                longWord.Add(keysSortedByLength[count - 1]);
                for (var i = count - 2; i > 0; i--)
                {
                    if (longWord[0].Length == keysSortedByLength[i].Length)
                    {
                        longWord.Add(keysSortedByLength[i]);
                    }
                    else
                        break;
                }
                foreach (var s in longWord)
                {
                    var sb = new StringBuilder();
                    foreach (var c in s.Where(c => !char.IsPunctuation(c)))
                    {
                        sb.Append(c);
                    }
                    CbLongWord.Items.Add(s + ": " + sb.Length);
                }
                CbLongWord.SelectedIndex = 0;
            };
            _searchParemeters[2] = () =>
            {
                repWord.Clear();
                CbRepWord.Items.Clear();
                //поиск самого частого слова
                var count = sortedDictionary.Count - 1;
                var present = sortedDictionary[keys[count]];

                var presentkey = keys[count];
                repWord.Add(presentkey + ": " + present);

                for (var i = count - 1; i > 0; i--)
                {
                    var next = sortedDictionary[keys[i]];
                    if (present == values[i])
                    {
                        var nextKey = keys[i];
                        repWord.Add(nextKey + ": " + next);
                    }
                    else break;
                }
                foreach (var word in repWord)
                {
                    CbRepWord.Items.Add(word);
                }
                CbRepWord.SelectedIndex = 0;
            };
            _searchParemeters[3] = () =>
            {
                rareWord.Clear();
                CbRareWord.Items.Clear();
                //поиск самого редкого слова
                var count = sortedDictionary.Count - 1;
                var present = sortedDictionary[keys[0]];

                var presentkey = keys[0];
                rareWord.Add(presentkey + ": " + present);

                for (var i = 1; i < count; i++)
                {
                    var next = sortedDictionary[keys[i - 1]];
                    if (present == values[i])
                    {
                        var nextKey = keys[i];
                        rareWord.Add(nextKey + ": " + next);
                    }
                    else break;
                }
                foreach (var word in rareWord)
                {
                    CbRareWord.Items.Add(word);
                }
                CbRareWord.SelectedIndex = 0;
            };
        }
        private void bStartSearch_Click(object sender, RoutedEventArgs e)
        {
            InitSearchTasks();

            _searchNext = 0;
            _substringIndexes.Clear();
            var notCheckedCount = 0;
            var checkedTask = new List<CheckBox>
            {
                ChbShortWord,
                ChbLongWord,
                ChbRepWord,
                ChbRareWord
            };
            try
            {
                if (TbInput.Text == "")
                    throw new Exception("Заполните поле ввода!");
                //if (CheckClicked != true)
                //    throw new Exception("Текст нужно проверить перед поиском!");
                try
                {
                    notCheckedCount += checkedTask.Count(t => t.IsChecked != true);
                    if (notCheckedCount == checkedTask.Count)
                        throw new Exception("Отметьте хотя бы одно поле!");
                    try
                    {
                        for (var i = 0; i < checkedTask.Count; i++)
                        {
                            if (checkedTask[i].IsChecked == true)
                            {
                                StartSearch(i);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private static void StartSearch(int i)
        {
            //инициализируем поиск по параметрам
            _searchParemeters[i]();
        }
        #endregion
        #region Создание или удаление таймера
        private void TbInput_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TbInput.Text != "")
            {
                if (_timer == null)
                {
                    _timerCallback = RefreshText;
                    _timer = new Timer(_timerCallback, null, 0, 5000);
                }
            }
            if (TbInput.Text == "") _timer.Dispose();
        }
        #endregion
    }
}
