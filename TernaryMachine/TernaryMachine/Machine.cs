using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TernaryCore;

namespace TernaryMachine
{
    public class Machine
    {
        private ArrayList datamemory = new ArrayList(512);
        private ArrayList actionsmemory = new ArrayList(512);

        public Machine() { }

        public void Parse(string dest)
        {
            string program = File.ReadAllText(dest);
            string[] lines = Regex.Split(program, @";");
            for (int i = 0; i < lines.Length; i++) lines[i] = lines[i].Replace(" ", string.Empty).Replace("\r\n", string.Empty);
            Regex dataregex = new Regex(@"^(Integer|Float)(\w*|(\w*(\,\w*)*))");
            foreach (var line in lines)
            {
                if (dataregex.IsMatch(line))
                    if(Regex.IsMatch(line, @"^Integer")) Console.WriteLine(line);
            }
            Console.ReadLine();
        }
    }
}
