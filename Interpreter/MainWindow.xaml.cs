using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Interpreter
{
    public class EmptyPolesException : Exception
    {
        public EmptyPolesException(string message) : base(message) { }
    }
    public class MaxRegisterException : Exception
    {
        public MaxRegisterException(string message) : base(message) { }
    }
    public class NumSystemException : Exception
    {
        public NumSystemException(string message) : base(message) { }
    }

    public partial class Main : Window
    { 
        //==============ИНИЦИАЛИЗАЦИЯ==============
        public string Path = "commands.dat", Path1 = "execute.txt", Comstring = "", Coord = "";
        public static int RowCount = 0, Num, Count = 0, Regrow = 0, RegClick = 0;
        //==============СТЕКИ======================
        Stack<Command> _actionCom, _actionComDraft;
        Stack<View> _actionView; Stack<string> _actionStr;
        //=========================================
        int[] _opsMas; List<int[]> _comsMas; Button[] _bts;
        public static string[] BtStr; Button[] _regbts;
        //==============СТРОЧКА====================
        List<View> _row; List<Label> _rowsNum;        
        List<CheckBox> _chExec, _chDel;
        List<Command> _draft, _finalCom;
        List<string> _coms; Dictionary<Button, string> _commands;
        
        public Main()
        {
            InitializeComponent();
            Init();

            _draft.Add(new Command());
            _row.Add(new View());

            Draw(RowCount);
            RowCount++;
        }
        private void Init()
        {
            //==============ИНИЦИАЛИЗАЦИЯ==============
            _actionCom = new Stack<Command>();
            _actionComDraft = new Stack<Command>();
            _actionView = new Stack<View>();
            _actionStr = new Stack<string>();
            _opsMas = new int[4];
            _comsMas = new List<int[]>();
            _row = new List<View>();
            _rowsNum = new List<Label>();
            _chExec = new List<CheckBox>();
            _chDel = new List<CheckBox>();
            _draft = new List<Command>();
            _finalCom = new List<Command>();
            _coms = new List<string>();
            _commands = new Dictionary<Button, string>();
            //============КНОПКИ КОМАНД================
            _bts = new[] { b0, b1, b2, b3, b4, b5, b6, b7, b8, b9 , b10,
                b11, b12, b13, b14, b15, b16, b17, b18, b19, b20, b21, b22, b23, b24 };
            for (int i = 0; i < _bts.Length; i++)
            {
                _bts[i] = new Button {Content = i.ToString()};
            }
            BtStr = new[] { "ВЫВОД", "NOT","OR", "AND", "XOR", "IMP", "COIMP", "EQV", "PIERCE", "SHEFFER",
                "ADD", "SUB", "MULT", "DIV", "MOD", "SWAP", "MOVE", "PRINT(cc)", "ENTER NUM", "2^P", "<<", ">>", "<<<", ">>>", "MOVE 2->1" };
            for (var i = 0; i < _bts.Length; i++)
                _commands.Add(_bts[i], BtStr[i]);
            //============КНОПКИ РЕГИСТРОВ=============
            _regbts = new Button[512];
            for (int i = 0; i < _regbts.Length; i++)
            {
                _regbts[i] = new Button
                {
                    Content = i.ToString(),
                    Width = 40,
                    Height = 20,
                    Background = Brushes.AliceBlue
                };
                _regbts[i].Click += RegButtons;
            }
            int but = 0, regrow = 0;
            while (but < _regbts.Length)
            {
                var rowDefinition = new RowDefinition {Height = new GridLength(30)};
                gridReg.RowDefinitions.Add(rowDefinition);

                for (var r = 0; r < 16; r++)
                {
                    Grid.SetColumn(_regbts[but], r);
                    Grid.SetRow(_regbts[but], regrow);
                    
                    gridReg.Children.Add(_regbts[but]);
                    but++;
                }
                regrow++;
            }
            //словарь для проверки команд
            tbCom.SpellCheck.CustomDictionaries.Add(new Uri(@"pack://application:,,,/Dictionary/ComDict.lex"));
            tbCom.SpellCheck.IsEnabled = true;
        }
        private void Draw(int k)
        {
            //==============ОТРИСОВКА ОДНОЙ СТРОЧКИ==============
            _rowsNum.Add(_row[k].num);
            _rowsNum[k].Content = (k + 1).ToString();
            Grid.SetColumn(_rowsNum[k], 0);
            Grid.SetRow(_rowsNum[k], k);
            gridCom.Children.Add(_rowsNum[k]);

            _chExec.Add(_row[k].chE);
            Grid.SetColumn(_chExec[k], 1);
            Grid.SetRow(_chExec[k], k);
            gridCom.Children.Add(_chExec[k]);

            for (int i = 2; i < gridCom.ColumnDefinitions.Count - 1; i++)
            {
                Grid.SetColumn(_row[k].tb[i - 2], i);
                Grid.SetRow(_row[k].tb[i - 2], k);
                gridCom.Children.Add(_row[k].tb[i - 2]);
            }

            _chDel.Add(_row[k].chD);
            Grid.SetColumn(_chDel[k], 6);
            Grid.SetRow(_chDel[k], k);
            gridCom.Children.Add(_chDel[k]);
        }
        private void Redraw()
        {
            for (int i = 0; i < RowCount; i++)
            {
                var rowDefinition = new RowDefinition {Height = new GridLength(30)};
                gridCom.RowDefinitions.Add(rowDefinition);
                Draw(i);
            }
        }

        private void RemoveNulls()
        {
            _coms.RemoveAll(item => item == null);
            _finalCom.RemoveAll(item => item == null);
            _row.RemoveAll(item => item == null);
            _rowsNum.RemoveAll(item => item == null);
            _draft.RemoveAll(item => item == null);
            _comsMas.RemoveAll(item => item == null); 
            _chDel.RemoveAll(item => item == null);
            _chExec.RemoveAll(item => item == null);
        }
        private void ClearValues()
        {
            gridCom.Children.Clear();
            gridCom.RowDefinitions.Clear();

            _rowsNum.Clear();
            _chDel.Clear();
            _chExec.Clear();

            tbCom.Clear();
        }
        private void FillMas()
        {
            for (int i = 0; i < _comsMas.Count; i++)
            {
                for (int m = 0; m < _row[i].tb.Length; m++)
                    _row[i].tb[m].Text = _comsMas[i][m].ToString();
            }
        }

        private void com_click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null) _row[RowCount - 1].tb[3].Text = btn.Content.ToString();
        }

        private void RegButtons(object sender, EventArgs e)
        {
            RegClick++;
            var btn = sender as Button;
            if (RegClick == 1)
            {
                if (btn != null) _row[RowCount - 1].tb[0].Text = btn.Content.ToString();
            }
            else if (RegClick == 2)
            {
                if (btn != null) _row[RowCount - 1].tb[1].Text = btn.Content.ToString();
            }
            else if (RegClick == 3)
            {
                if (btn != null) _row[RowCount - 1].tb[2].Text = btn.Content.ToString();
            }
        }

        private void NewCommand_Click(object sender, MouseButtonEventArgs e)
        {
            RegClick = 0;
            var max = Command.registers.Length - 1;
            try
            {
                if (_row[RowCount - 1].tb[0].Text == "" && _row[RowCount - 1].tb[1].Text == "" && _row[RowCount - 1].tb[2].Text == "" && _row[RowCount - 1].tb[3].Text == "")
                    throw new EmptyPolesException("Заполните все поля!");
                else
                {
                    try
                    {
                        int op1 = int.Parse(_row[RowCount - 1].tb[0].Text),
                        op2 = int.Parse(_row[RowCount - 1].tb[1].Text),
                        op3 = int.Parse(_row[RowCount - 1].tb[2].Text),
                        oper = int.Parse(_row[RowCount - 1].tb[3].Text);
                        _opsMas = new[] { op1, op2, op3, oper };
                        _comsMas.Add(_opsMas);

                        _draft[RowCount - 1] = new Command(op1, op2, op3, oper);
                        if (op1 > max || op2 > max || op3 > max)
                            throw new MaxRegisterException("Число не должно превышать максимальный индекс регистра!");
                        try
                        {
                            if (op1 == 1 && oper == 0)
                                throw new NumSystemException("Система счислления должна быть не меньше 2!");
                            if (op2 == 0 && oper == 13)
                                throw new DivideByZeroException();
                            
                            //пустая строчка для новой команды и ее отрисовка
                            _draft.Add(new Command());
                            _row.Add(new View());
                            Draw(RowCount);
                            //нарисуем новую строчку для ввода команды
                            var rowDefinition = new RowDefinition {Height = new GridLength(30)};
                            gridCom.RowDefinitions.Add(rowDefinition);

                            _finalCom.Add(new Command(_draft[RowCount - 1]));

                            int butNum = Convert.ToInt32(_row[RowCount - 1].tb[3].Text);
                            for (int i = 0; i < _row[RowCount - 1].tb.Length - 1; i++)
                                Comstring += _row[RowCount - 1].tb[i].Text + "\t";
                            Comstring += BtStr[butNum];
                            Comstring += "\n";

                            _coms.Add(Comstring);
                            tbCom.Text += Comstring;
                            Comstring = "";

                            RowCount++;
                        }
                        catch (EmptyPolesException em)
                        {
                            MessageBox.Show(em.Message, "Исключение");
                        }
                    }
                    catch (MaxRegisterException m)
                    {
                        MessageBox.Show(m.Message, "Исключение");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Исключение");
            }
        }
        private void bRefrText_Click(object sender, MouseButtonEventArgs e)
        {
            string[] lines = tbCom.Text.Split('\n');
            int lineslen = lines.Length;
            List<string[]>[] linesmas = new List<string[]>[lineslen];
            try
            {
                for (int i = 0; i < lines.Length - 1; i++)
                {
                    linesmas[i] = new List<string[]>();
                    string[] split = lines[i].Split('\t');
                    split[3] = _commands.FirstOrDefault(x => x.Value == split[3]).Key.Content.ToString();
                    linesmas[i].Add(split);
                }
                for (int i = 0; i < _coms.Count; i++)
                {
                    _coms[i] = lines[i] + "\n";
                }

                _opsMas = new[] { 0, 0, 0, 0 };
                _comsMas.Clear();
                _finalCom.Clear();
                _draft.Clear();
                for (int i = 0; i < linesmas.Length - 1; i++)
                {
                    foreach (string[] strmas in linesmas[i])
                    {
                        int iter = 0;
                        foreach (string str in strmas)
                        {
                            _opsMas[iter] = (Convert.ToInt32(str));
                            iter++;
                        }
                        _comsMas.Add((int[])_opsMas.Clone());
                        _draft.Add(new Command((int[])_opsMas.Clone()));
                        _finalCom.Add(new Command((int[])_opsMas.Clone()));
                    }
                }
                _draft.Add(new Command());
                //выкинем пустые ссылки из списков
                RemoveNulls();
                //очистим списки визуального представления
                ClearValues();
                //заполним массивы операндами
                FillMas();
                //нарисуем оставшиеся команды заново
                for (var i = 0; i < RowCount; i++)
                {
                    var rowDefinition = new RowDefinition {Height = new GridLength(30)};
                    gridCom.RowDefinitions.Add(rowDefinition);
                    Draw(i);
                }
                //заполним текстовое представление
                foreach (string s in _coms)
                {
                    tbCom.Text += s;
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Строка имеет неверный формат", "Исключение");
            }
        }
        private void bRefrGraph_Click(object sender, MouseButtonEventArgs e)
        {
            _opsMas = new[] { 0, 0, 0, 0 };
            _coms.Clear();
            _comsMas.Clear();
            _finalCom.Clear();
            _draft.Clear();
            for (int n = 0; n < _row.Count - 1; n++)
            {
                for (int i = 0; i < _row[n].tb.Length; i++)
                {
                    _opsMas[i] = Convert.ToInt32(_row[n].tb[i].Text);
                }
                _comsMas.Add((int[])_opsMas.Clone());
                _draft.Add(new Command((int[])_opsMas.Clone()));
                _finalCom.Add(new Command((int[])_opsMas.Clone()));
            }
            //пустая строка для новой команды
            _draft.Add(new Command());
            //очистим списки визуального представления
            ClearValues();
            //заполним массивы операндами
            FillMas();
            //нарисуем оставшиеся команды заново
            Redraw();
            //заполним список команд в текстовом виде
            foreach (Command com in _finalCom)
                _coms.Add(com.ToString());
            //заполним текстовое представление
            foreach (string s in _coms)
            {
                tbCom.Text += s;
            }
        }
        private void bDel_Click(object sender, MouseButtonEventArgs e)
        {
            int check = 0;
            MessageBoxResult result = MessageBox.Show("Действительно хотите удалить команды?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (_finalCom.Count == 0)
                        throw new Exception("Нет команд для удаления!");
                    foreach (CheckBox chb in _chDel)
                    {
                        if (chb.IsChecked == false) check++;
                        if (check == _chDel.Count) throw new Exception("Нет команд для удаления!");
                    }
                    //занулим удаляемые элементы
                    for (int i = 0; i < _finalCom.Count; i++)
                    {
                        if (_chDel[i].IsChecked == true)
                        {
                            RowCount--;
                            _coms[i] = null;
                            _finalCom[i] = null;
                            _row[i] = null;
                            _rowsNum[i] = null;
                            _draft[i] = null;
                            _comsMas[i] = null;
                            _chDel[i] = null;
                            _chExec[i] = null;
                        }
                    }
                    //выкинем пустые ссылки из списков
                    RemoveNulls();
                    //заполним массивы операндами
                    FillMas();
                    //очистим списки визуального представления
                    ClearValues();
                    //нарисуем оставшиеся команды заново
                    Redraw();
                    //заполним текстовое представление
                    foreach (string s in _coms)
                    {
                        tbCom.Text += s;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Исключение");
                }
            }
        }
        private void bUndo_Click(object sender, MouseButtonEventArgs e)
        {
            if (_finalCom.Count > 0)
            {
                _actionCom.Push(_finalCom.Last());
                _actionComDraft.Push(_finalCom.Last());
                _actionView.Push(_row.Last());
                _actionStr.Push(_coms.Last());
                _finalCom.Remove(_finalCom.Last());
                _row.Remove(_row.Last());
                _draft.Remove(_draft.Last());
                _coms.Remove(_coms.Last());
                RowCount--;
                //очистим списки визуального представления
                ClearValues();
                //нарисуем оставшиеся команды заново
                Redraw();
                //заполним текстовое представление
                foreach (string s in _coms)
                {
                    tbCom.Text += s;
                }
            }
        }
        private void bRedo_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (_actionCom.Count == 0) throw new Exception("Стек действий пуст!");
                _finalCom.Add(_actionCom.Pop());
                _draft.Add(_actionComDraft.Pop());
                _row.Add(_actionView.Pop());
                _coms.Add(_actionStr.Pop());
                RowCount++;
                //очистим списки визуального представления
                ClearValues();
                //нарисуем оставшиеся команды заново
                Redraw();
                //заполним текстовое представление
                foreach (string s in _coms)
                {
                    tbCom.Text += s;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Исключение");
            }
        }
        private void bExec_Click(object sender, MouseButtonEventArgs e)
        {
            Executer exec = new Executer();
            exec.ShowDialog();
        }
        private void Save_Click(object sender, MouseButtonEventArgs e)
        {
            int check = 0;
            File.WriteAllText(Path, "");
            MessageBoxResult result = MessageBox.Show("Сохранить выбранные команды?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    check += _chExec.Count(ch => ch.IsChecked == false);
                    if (check == _chExec.Count) throw new Exception("Нет команд для сохранения");

                    using (var writer = new BinaryWriter(File.Open(Path, FileMode.OpenOrCreate)))
                    {
                        // записываем в файл значение каждого поля структуры
                        for (int i = 0; i < _chExec.Count - 1; i++)
                        {
                            if (_chExec[i].IsChecked == true)
                            {
                                writer.Write(_finalCom[i].op1);
                                writer.Write(_finalCom[i].op2);
                                writer.Write(_finalCom[i].op3);
                                writer.Write(_finalCom[i].oper);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Исключение");
                }
            }
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            About ab = new About();
            ab.ShowDialog();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = Mouse.GetPosition(this);
            Coord = "X: " + position.X + "   " + "Y: " + position.Y;
            sbLab.Content = Coord;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ViewItem_Click(object sender, RoutedEventArgs e)
        {

        }        
    }
}
