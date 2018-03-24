using System;
using System.Windows;

namespace NaturalHabitat
{
    public partial class CreateAnimal : Window
    {
        static string _name, _pop, _type;

        public CreateAnimal()
        {
            InitializeComponent();
            //ComBoxFeedType.SelectedIndex = 0;
        }

        private void ButRandom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TbName.Text == "" || ComBoxFeedType.Text == "")
                    throw new Exception("Укажите название и тип питания!");
                else
                {
                    var randPop = new Random();
                    _name = TbName.Text;
                    _type = ComBoxFeedType.Text;
                    _pop = randPop.Next(400, 800).ToString();

                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static string ReturnValues()
        {
            return _name + ',' + _pop + ',' + _type;
        }

        private void ButCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TbName.Text == "" || TbPopulation.Text == "" || ComBoxFeedType.SelectedItem.ToString() == "")
                    throw new Exception("Заполните поля!");
                _name = TbName.Text;
                _pop = TbPopulation.Text;
                _type = ComBoxFeedType.Text;

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
