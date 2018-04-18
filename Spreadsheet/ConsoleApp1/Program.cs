using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionTree;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            void Menu()
            {
                StringBuilder menu = new StringBuilder();
                menu.AppendLine("Select an option: ");
                menu.AppendLine("\t a.Enter in variables(must be done before evaluating)");
                menu.AppendLine("\t b.Enter equation ");
                menu.AppendLine("\t c.Evaluate");
                menu.AppendLine("\t d.Clear Equation");
                menu.AppendLine("\t e.Clear Variables");
                menu.AppendLine("\t q.Quit");
                Console.WriteLine(menu);
            }
            Dictionary<string, double> variables = new Dictionary<string, double>();
            string equation = string.Empty;
            char selection = '0';
            while (selection != 'q')
            {
                if (equation != string.Empty)
                {
                    Console.WriteLine("Current equation = " + equation);
                }
                Menu();
                selection = char.Parse(Console.ReadLine());
                selection = char.ToLower(selection);
                switch (selection)
                {
                    case ('a'):
                        string varname = string.Empty;
                        int value;
                        Console.Clear();
                        Console.WriteLine("Enter in the variable name: ");
                        varname = Console.ReadLine();
                        varname = varname.Replace(" ", string.Empty);
                        Console.WriteLine("Enter in the variable value: ");
                        value = int.Parse(Console.ReadLine());
                        Console.WriteLine("New Variable " + varname + " = " + value.ToString());
                        try { variables.Add(varname, value); }
                        catch { variables.Remove(varname); variables.Add(varname, value); }
                        Console.Clear();
                        break;
                    case ('b'):
                        Console.Clear();
                        variables.Clear();
                        Console.WriteLine("Enter equation: ");
                        equation = Console.ReadLine();
                        equation = equation.Replace(" ", string.Empty);
                        Console.Clear();
                        break;
                    case ('c'):
                        Console.WriteLine("Equations evaluates to : ");
                        ExpTree exptree = new ExpTree(equation);
                        foreach (string key in variables.Keys)
                        {
                            exptree.SetVAr(key, variables[key]);
                        }
                        Console.WriteLine(exptree.Eval());
                        Console.WriteLine("Enter any key to continue...");
                        Console.ReadLine();
                        Console.Clear();
                        break;
                    case ('d'):
                        equation = string.Empty;
                        Console.Clear();
                        break;
                    case ('e'):
                        variables.Clear();
                        Console.Clear();
                        break;
                    default:
                        Console.Clear();
                        break;

                }

            }

        }
    }
}
