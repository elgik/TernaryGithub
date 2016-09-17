using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TernaryMachine
{
    class Programm
    {
        static void Main(string[] args)
        {
            Machine m = new Machine();
            m.Parse(@"C:\Users\Egorov\Documents\Program.txt");
        }
    }
}
