using System;
using System.IO;

namespace Interpreter
{
    public class EmptyCommand : Exception
    {
        public EmptyCommand(string message) : base(message) { }
    }
    internal class Command 
    {
        static Action[] coms = new Action[25];
        public static int[] registers = new int[512];

        public  int op1 { get; set; }

        public  int op2 { get; set; }

        public  int op3 { get; set; }

        public  int oper { get; set; }

        public Command()
        {  
            op1 = 0; op2 = 0; op3 = 0; oper = 0;
        }
        public Command(Command com)
        {
            op1 = com.op1;
            op2 = com.op2;
            op3 = com.op3;
            oper = com.oper;
        }
        public Command(int[] mas)
        {
            op1 = mas[0];
            op2 = mas[1];
            op3 = mas[2];
            oper = mas[3];
        }
        public Command(int o1, int o2, int o3, int op) 
        {
            op1 = o1;
            op2 = o2;
            op3 = o3;
            oper = op;
        }
        public Command(string o1, string o2, string o3, string op)
        {
            op1 = Convert.ToInt32(o1);
            op2 = Convert.ToInt32(o2);
            op3 = Convert.ToInt32(o3);
            oper = Convert.ToInt32(op);
        }

        public void InitInterpreter()
        {
            coms[0] = () => { };
            coms[1] = () => { registers[op3] = ~registers[op1]; };
            coms[2] = () => { registers[op3] = registers[op1] | registers[op2]; };
            coms[3] = () => { registers[op3] = registers[op1] & registers[op2]; };
            coms[4] = () => { registers[op3] = registers[op1] ^ registers[op2]; };
            coms[5] = () => { registers[op3] = (~registers[op1]) | registers[op2]; };
            coms[6] = () => { registers[op3] = registers[op1] & (~registers[op2]); };
            coms[7] = () => {
                registers[op3] = (registers[op1] & registers[op2]) &
                    (~(registers[op1] | registers[op2]));
            };
            coms[8] = () => { registers[op3] = ~(registers[op1] | registers[op2]); };
            coms[9] = () => { registers[op3] = ~(registers[op1] & registers[op2]); };
            coms[10] = () => { registers[op3] = registers[op1] + registers[op2]; };
            coms[11] = () => { registers[op3] = registers[op1] - registers[op2]; };
            coms[12] = () => { registers[op3] = registers[op1] * registers[op2]; };
            coms[13] = () => { registers[op3] = registers[op1] / registers[op2]; };
            coms[14] = () => { registers[op3] = registers[op1] % registers[op2]; };
            coms[15] = () => {
                registers[op1] ^= registers[op2];
                registers[op2] = registers[op1] ^ registers[op2];
                registers[op1] ^= registers[op2];
            };
            coms[16] = () => { };
            coms[17] = () => {
                Console.WriteLine("1-й операнд ({0}cc) = {1}", registers[op2],
                    ToCC(registers[op1], registers[op2]));
            };
            coms[18] = () => {
                Console.WriteLine("Введите значение для 1-го операнда: ");
                registers[op1] = Convert.ToInt32(Console.ReadLine());
            };
            coms[19] = () => { registers[op3] = registers[op1] & (~registers[op1] + 1); };
            coms[20] = () => { registers[op3] = registers[op1] << registers[op2]; };
            coms[21] = () => { registers[op3] = registers[op1] >> registers[op2]; };
            coms[22] = () => {
                registers[op3] = (registers[op1] >> registers[op2]) |
                    (registers[op1] << (32 - registers[op2]));
            };
            coms[23] = () => {
                registers[op3] = (registers[op1] << registers[op2]) |
                    (registers[op1] >> (32 - registers[op2]));
            };
            coms[24] = () => { registers[op1] = registers[op2]; };

        }
        public static void Interprete(Command com)
        {
            com.InitInterpreter();
            Print(com);
            coms[com.oper]();
        }

        public override string ToString()
        {
            return op1 + "\t" + op2 + "\t" + op3 + "\t" + Main.BtStr[oper] + "\n";
        }
        public static void Print(Command c)
        {
            var path1 = "execute.txt";
            var comNum = Main.Num;
            using (var sw = new StreamWriter(path1, true, System.Text.Encoding.Default))
            {
                if (c.oper == 0)
                {
                    foreach (var r in registers)
                    {
                        string newNum = ToCC(r, c.op1);
                        sw.Write(newNum + " ");
                    }
                    sw.WriteLine();
                    comNum++;
                }
                else {
                    foreach (var r in registers)
                    {
                        sw.Write(r + " ");
                    }
                    sw.WriteLine();
                    comNum++;
                }
            }
        }
        public static int[] Clone(int[] mas)
        {
            int[] mas1 = new int[mas.Length];
            Array.Copy(mas, mas1, mas.Length);
            return mas1;
        }
        public static string ToCC(int x, int cc)
        {
            int c;
            if (cc == 2 || cc == 16 || cc == 8)
                return Convert.ToString(x, cc);
            string res = "", abc = "0123456789ABCDEFGHIJKLMNOPQESTUVWXYZ";
            if (x < cc)
                return abc[x].ToString();
            while (x != 0)
            {
                c = x % cc;
                res = abc[c] + res;
                x /= cc;
            }
            return res;
        }
        public static int[] SplitInt(int command)
        {
            var a = ((1 << 5) - 1);
            var b = ((1 << 9) - 1);
            var c = 27;
            var d = 23;
            var mas = new int[4];
            var newIn = command;

            mas[3] = newIn & a;
            newIn = (newIn >> 5) | (newIn << c);
            mas[2] = newIn & b;
            newIn = (newIn >> 9) | (newIn << d);
            mas[1] = newIn & b;
            newIn = (newIn >> 9) | (newIn << d);
            mas[0] = newIn & b;

            return mas;
        }
    }
}
