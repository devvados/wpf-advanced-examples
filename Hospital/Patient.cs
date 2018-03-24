using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital
{
    public class Doctor
    {
        bool isBuisy;
        public bool Buisy
        {
            get
            {
                return isBuisy;
            }
            set
            {
                isBuisy = value;
            }
        }
        public Doctor()
        {
            Buisy = false;
        }
        public void GetPatient(Patient pat)
        {
            Buisy = true;
        }
        public void FreePatient(Patient pat)
        {
            pat.Condition = "Здоров";
        }
    }
    public class Patient
    {
        string name;
        string condition;
        public Patient()
        {
            Name = "";
            string[] con = new string[] { "Здоров", "Болен" };
            
            Random rand = new Random();
            Condition = con[rand.Next(0, 1)];
        }
        public Patient(string name, string cond)
        {
            Name = name;
            Condition = cond;
        }
        public override string ToString()
        {
            return "Имя: " + name + "; Состояние: " + condition;
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string Condition
        {
            get
            {
                return condition;
            }
            set
            {
                condition = value;
            }
        }
    }
}
