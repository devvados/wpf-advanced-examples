using System;
using System.IO;
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

namespace Interpreter
{
    public class EmptyCommandsException : Exception
    {
        public EmptyCommandsException(string message) : base(message) { }
    }
    public partial class Executer : Window
    {
        int iterator = 0;
        public string path = "commands.dat", path1 = "execute.txt", newtext = "";
        static List<string> coms = new List<string>();
        static List<Command> comsList = new List<Command>();

        public Executer()
        {
            Random rand = new Random();
            for (int i = 0; i < Command.registers.Length; i++)
                Command.registers[i] = rand.Next(10, 200);
            InitializeComponent();
        }

        private void bStart_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(path1, true, System.Text.Encoding.Default))
            {
                sw.WriteLine(new string('-', 50));
                sw.WriteLine("Запуск: " + DateTime.Now);
                sw.WriteLine(new string('-', 50));
            }
            int num = 0;
            try
            {
                if (comsList.Count < 1)
                    throw new EmptyCommandsException("Команды не загружены!");
                else
                {
                    while (num < comsList.Count)
                    {
                        Command.Interprete(comsList[num]);
                        num++;
                    }
                }
            }
            catch (EmptyCommandsException em)
            {
                MessageBox.Show(em.Message);
            }
            System.Diagnostics.Process.Start(path1);
        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            tbCom.Clear();
            coms.Clear();
            comsList.Clear();

            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                // пока не достигнут конец файла
                // считываем каждое значение из файла
                while (reader.PeekChar() > -1)
                {
                    comsList.Add(new Command(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()));
                }
            }
            foreach (Command com in comsList)
            {
                int opNum = comsList[iterator].oper;
                newtext += (iterator+1).ToString() + ")\t";
                newtext += comsList[iterator].op1.ToString() + "\t" +
                    comsList[iterator].op2.ToString() + "\t" +
                    comsList[iterator].op3.ToString() + "\t" +
                    Main.BtStr[opNum];
                newtext += "\n";

                coms.Add(newtext);
                newtext = "";
                iterator++;
            }
            foreach (string s in coms)
                tbCom.Text += s;
        }
    }
}
