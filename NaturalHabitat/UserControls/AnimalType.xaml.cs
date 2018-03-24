using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NaturalHabitat
{
    public partial class AnimalType : UserControl
    {
        public AnimalType()
        {
            InitializeComponent();
        }

        public AnimalType(Animal animal)
        {
            InitializeComponent();
            
            GroupBox.Header = animal.Name;
            TextBlockPop.Foreground = animal.Population > 100 ? Brushes.White : Brushes.Black;

            if (animal.Nurture == "Плотоядный")
            {
                PbPopulation.Foreground = Brushes.Red;
            }
            else if (animal.Nurture == "Всеядный")
            {
                PbPopulation.Foreground = Brushes.Blue;
            }

            PbPopulation.Value = animal.Population;
            TextBlockPop.Text = animal.Population.ToString();
        }

        private void AnimalType_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Вы кликнули на контрол!");
        }
    }
}
