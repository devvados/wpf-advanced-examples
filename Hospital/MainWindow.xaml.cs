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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hospital
{
    public partial class MainWindow : Window
    {
        public static Queue<Patient> queue = new Queue<Patient>();
        public static List<Patient> cabinet = new List<Patient>();
        public static List<Doctor> docs;
        public static int docCount = 0;
        int patCount = 1;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddPatient_Click(object sender, RoutedEventArgs e)
        {
            Add wnd = new Add();
            wnd.ShowDialog();
        }

        private void Begin_Click(object sender, RoutedEventArgs e)
        {
            int enterCabinet = 0, docBuisy = 0;
            try
            {
                if (tbQueue.Text == "")
                    throw new Exception("Очередь не составлена!");
                Patient p = queue.Peek();
                //проверка состояний пациентов в смотровой перед входом
                foreach(Patient pat in cabinet)
                    if (p.Condition == pat.Condition)
                        enterCabinet++;
                if (enterCabinet == cabinet.Count)
                    cabinet.Add(p);
                else throw new Exception("Нельзя зайти в кабинет, нужно подождать!");
                //проверка на занятость докторов
                foreach (Doctor doc in docs)
                    if (doc.Buisy == true)
                        docBuisy++;
                if (docBuisy == docs.Count) throw new Exception("Все доктора заняты!");
                else
                {
                    for (int i = 0; i < docs.Count; i++)
                        if (docs[i].Buisy == false)
                        {
                            docs[i].Buisy = true;
                            Patient pat = queue.Dequeue();
                            docs[i].GetPatient(pat);
                            docs[i].FreePatient(pat);

                            docs[i].Buisy = false;
                            cabinet.Remove(pat);
                        }
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Исключение");
            }
        }

        private void CreateQueue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (queue.Count == 0) throw new Exception("Очередь пуста!");
                Doc wnd = new Doc();
                wnd.ShowDialog();

                docs = new List<Doctor>();
                for (int i = 0; i < docCount; i++)
                    docs.Add(new Doctor());

                foreach (Patient pat in queue)
                {
                    tbQueue.Text += patCount.ToString() + ") " + pat.ToString() + "\n";
                    patCount++;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
