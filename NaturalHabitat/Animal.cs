namespace NaturalHabitat
{
    public abstract class Animal
    {
        private string _name;
        private string _nurture;
        private int _population;

        protected Animal(string name, string nurt, int pop)
        {
            Name = name;
            Nurture = nurt;
            Population = pop;
        }

        public string Name { get; set; }
        public string Nurture { get; set; }
        public int Population { get; set; }
    }

    class HerbivorousAnimal : Animal
    {
        public HerbivorousAnimal(string name, int pop) : base (name, "Травоядный", pop) { }
    }

    class CarnivorousAnimal : Animal
    {
        public CarnivorousAnimal(string name, int pop) : base(name, "Плотоядный", pop) { }
    }

    class OmnivorousAnimal : Animal
    {
        public OmnivorousAnimal(string name, int pop) : base(name, "Всеядный", pop) { }
    }
}
