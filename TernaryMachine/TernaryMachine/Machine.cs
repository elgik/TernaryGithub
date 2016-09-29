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
        private static ArrayList memory = new ArrayList(512);
        private static Dictionary<string, int> adressTable = new Dictionary<string, int>();
        private static Dictionary<string, string> typesTable = new Dictionary<string, string>();
        private static string program;
        public Machine() { }

        public void Parse(string dest)
        {
            program = File.ReadAllText(dest);
            string[] lines = Regex.Split(program, @";");
            for (int i = 0; i < lines.Length; i++) lines[i] = lines[i].Replace(" ", string.Empty).Replace("\r\n", string.Empty);
            Regex dataregex = new Regex(@"^(Integer|Float|Logic|Tryte)(\w*|(\w*(\,\w*)*))");
            foreach (var line in lines)
            {
                if (dataregex.IsMatch(line))
                {
                    ArrayList vars = new ArrayList();
                    if (Regex.IsMatch(line, @"^Integer"))
                    {
                        vars.AddRange(Regex.Split(line, @"^Integer|,|;"));
                        foreach (var v in vars)
                        {
                            if ((string)v != string.Empty)
                            {
                                memory.Add(new Integer());
                                adressTable.Add((string)v, memory.Count - 1);
                                typesTable.Add((string)v, "Integer");
                            }
                        }
                    }
                    else if (Regex.IsMatch(line, @"^Float"))
                    {
                        vars.AddRange(Regex.Split(line, @"^Float|,|;"));
                        foreach (var v in vars)
                        {
                            if ((string)v != string.Empty)
                            {
                                memory.Add(new Float());
                                adressTable.Add((string)v, memory.Count - 1);
                                typesTable.Add((string)v, "Float");
                            }
                        }
                    }
                }
            }
            foreach (var item in memory)
            {
                if(memory.Count == 0) { Console.WriteLine("Пусто"); break; }
                Console.WriteLine(item);
            }
            Console.ReadLine();
        }

        public void Run()
        {
            string[] lines = Regex.Split(program, @";");
            for (int i = 0; i < lines.Length; i++) lines[i] = lines[i].Replace(" ", string.Empty).Replace("\r\n", string.Empty);
            foreach (var l in lines) Console.WriteLine(l);
            ArrayList vars;
            foreach (var v in lines)
            {
                vars = new ArrayList();
                if (Regex.IsMatch(v, @"^(\w*=)"))
                {
                    if (Regex.IsMatch(v, @"^(\w*=)\d+"))
                    {
                        vars.AddRange(Regex.Split(v, @"="));
                        
                        switch (typesTable[(string)vars[0]])
                        {
                            case "Integer":
                                int temp = 0;
                                int.TryParse((string)vars[1], out temp);
                                memory[adressTable[(string)vars[0]]] = new Integer((long)temp);
                                break;
                            case "Float":
                                float t = 0;
                                float.TryParse((string)vars[1], out t);
                                Console.WriteLine(t);
                                memory[adressTable[(string)vars[0]]] = new Float(t);
                                break;
                            default:
                                Console.WriteLine("What");
                                break;
                        }
                        
                    }
                    else if (Regex.IsMatch(v, @"^(([a-z]|[A-Z])+=)(([a-z]|[A-Z])+)$"))
                    {
                        vars.AddRange(Regex.Split(v, @"="));
                        if (typesTable[(string)vars[0]] != typesTable[(string)vars[1]])
                            throw new InvalidCastException();
                        memory[adressTable[(string)vars[0]]] = memory[adressTable[(string)vars[1]]];
                    }
                    else if (Regex.IsMatch(v, @"^\w*=\w*\+\w*"))
                    {
                        vars.AddRange(Regex.Split(v, @"=|\+"));
                        if (typesTable[(string)vars[0]] != typesTable[(string)vars[1]] || typesTable[(string)vars[1]] != typesTable[(string)vars[2]])
                            throw new InvalidCastException();
                        switch (typesTable[(string)vars[0]])
                        {
                            case "Integer":
                                memory[adressTable[(string)vars[0]]] = (Integer)memory[adressTable[(string)vars[1]]] - (Integer)memory[adressTable[(string)vars[2]]];
                                break;
                            case "Float":
                                memory[adressTable[(string)vars[0]]] = (Float)memory[adressTable[(string)vars[1]]] - (Float)memory[adressTable[(string)vars[2]]];
                                break;
                            default:
                                Console.WriteLine("What");
                                break;
                        }
                    }
                    else if (Regex.IsMatch(v, @"^\w*=\w*-\w*"))
                    {
                        vars.AddRange(Regex.Split(v, @"=|-"));
                        if (typesTable[(string)vars[0]] != typesTable[(string)vars[1]] || typesTable[(string)vars[1]] != typesTable[(string)vars[2]])
                            throw new InvalidCastException();
                        switch (typesTable[(string)vars[0]])
                        {
                            case "Integer":
                                memory[adressTable[(string)vars[0]]] = (Integer)memory[adressTable[(string)vars[1]]] - (Integer)memory[adressTable[(string)vars[2]]];
                                break;
                            case "Float":
                                memory[adressTable[(string)vars[0]]] = (Float)memory[adressTable[(string)vars[1]]] - (Float)memory[adressTable[(string)vars[2]]];
                                break;
                            default:
                                Console.WriteLine("What");
                                break;
                        }
                    }
                    else if (Regex.IsMatch(v, @"^\w*=\w*\*\w*"))
                    {
                        vars.AddRange(Regex.Split(v, @"=|\*"));
                        if (typesTable[(string)vars[0]] != typesTable[(string)vars[1]] || typesTable[(string)vars[1]] != typesTable[(string)vars[2]])
                            throw new InvalidCastException();
                        switch (typesTable[(string)vars[0]])
                        {
                            case "Integer":
                                memory[adressTable[(string)vars[0]]] = (Integer)memory[adressTable[(string)vars[1]]] * (Integer)memory[adressTable[(string)vars[2]]];
                                break;
                            case "Float":
                                memory[adressTable[(string)vars[0]]] = (Float)memory[adressTable[(string)vars[1]]] * (Float)memory[adressTable[(string)vars[2]]];
                                break;
                            default:
                                Console.WriteLine("What");
                                break;
                        }
                    }
                    else if (Regex.IsMatch(v, @"^\w*=\w*/\w*"))
                    {
                        vars.AddRange(Regex.Split(v, @"=|/"));
                        if (typesTable[(string)vars[0]] != typesTable[(string)vars[1]] || typesTable[(string)vars[1]] != typesTable[(string)vars[2]])
                            throw new InvalidCastException();
                        switch (typesTable[(string)vars[0]])
                        {
                            case "Integer":
                                memory[adressTable[(string)vars[0]]] = (Integer)memory[adressTable[(string)vars[1]]] / (Integer)memory[adressTable[(string)vars[2]]];
                                break;
                            case "Float":
                                memory[adressTable[(string)vars[0]]] = (Float)memory[adressTable[(string)vars[1]]] / (Float)memory[adressTable[(string)vars[2]]];
                                break;
                            default:
                                Console.WriteLine("What");
                                break;
                        }
                        
                    }
                }
                else
                { 
                    if (Regex.IsMatch(v, @"^(Write)\w*"))
                    {                       
                        vars.AddRange(Regex.Split(v, @"^Write"));
                        if (typesTable[(string)vars[1]] == "Integer")
                        {
                            Integer temp = (Integer)memory[adressTable[(string)vars[1]]];
                            Console.WriteLine(temp);
                        }
                        else
                        {
                            Float temp = (Float)memory[adressTable[(string)vars[1]]];
                            Console.WriteLine(temp);
                        }
                    }
                }
            }
            Console.ReadLine();
        }
    }
}