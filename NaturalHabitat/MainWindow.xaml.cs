using System;
using System.Collections.Generic;
using System.Windows;

namespace NaturalHabitat
{
    public partial class MainWindow : Window
    {
        List<Animal> 
            _herbList = new List<Animal>(), 
            _carnList = new List<Animal>(), 
            _omnivList = new List<Animal>();
        
        public MainWindow()
        {
            InitializeComponent();
        }

        public double Max(double a, double b)
        {
            return a > b ? a : b;
        }

        private void AddToDockPanel(string s1, string s2, string s3)
        {
            string name = s1, pop = s2, feedType = s3;
            try
            {
                if (name != "" && pop != "" && feedType != "")
                {
                    if (feedType == "Травоядный")
                    {
                        TabItem0.Focus();
                        var newHerb = new HerbivorousAnimal(name, Convert.ToInt32(pop));
                        var herbType = new AnimalType(newHerb);

                        _herbList.Add(newHerb);
                        DockPanelHerbivorous.Children.Add(herbType);
                    }
                    if (feedType == "Плотоядный")
                    {
                        TabItem1.Focus();
                        var newCarn = new CarnivorousAnimal(name, Convert.ToInt32(pop));
                        var carnType = new AnimalType(newCarn);

                        _carnList.Add(newCarn);
                        DockPanelCarnivorous.Children.Add(carnType);
                    }
                    if (feedType == "Всеядный")
                    {
                        TabItem2.Focus();
                        var newOmniv = new OmnivorousAnimal(name, Convert.ToInt32(pop));
                        var omnivType = new AnimalType(newOmniv);
                         
                        _omnivList.Add(newOmniv);
                        DockPanelOmnivorous.Children.Add(omnivType);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void butAdd_Click(object sender, RoutedEventArgs e)
        {
            var createDialog = new CreateAnimal();
            createDialog.ShowDialog();
            try
            {
                var creatureParams = CreateAnimal.ReturnValues();
                
                if (string.IsNullOrWhiteSpace(creatureParams))
                    throw new Exception("Не заданы параметры нового существа!");
                
                var splitParams = creatureParams.Split(',');
                string name = splitParams[0], 
                       pop = splitParams[1], 
                       feedType = splitParams[2];

                AddToDockPanel(name, pop, feedType);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
