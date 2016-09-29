using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TernaryMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            Machine m = new Machine();
            m.Parse(@"C:\Users\Egorov\Desktop\Program1.txt");
            m.Run();
        }
    }
}
