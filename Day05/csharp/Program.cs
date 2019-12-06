using System;
using System.IO;

namespace csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].Equals("Part1",
              StringComparison.CurrentCultureIgnoreCase))
            {
                Part1(File.ReadAllText("input.txt"));

            }
            else if (args.Length > 0 && args[0].Equals("Part2",
              StringComparison.CurrentCultureIgnoreCase))
            {
                Part2(File.ReadAllText("input.txt"));
            }
            else
            {
                Test();
            }
        }

        static string[] tests = new string[] {
            "3,9,8,9,10,9,4,9,99,-1,8",
            "3,9,7,9,10,9,4,9,99,-1,8",
            "3,3,1108,-1,8,3,4,3,99",
            "3,3,1107,-1,8,3,4,3,99",
            "3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9",
            "3,3,1105,-1,9,1101,0,0,12,4,12,99,1",
            "3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99"
            };

        static void Test()
        {
            Console.WriteLine("TESTING...");
            var computer = new IntCodeComputer();
            foreach (var test in tests)
            {
                var t = new IntCode(test);
                Console.WriteLine($"Input = {t.ToString()}");
                var result = computer.Run(t, 9);
                Console.WriteLine($"Output = {result.ToString()}");
            }
        }

        static void Part1(string text)
        {
            Console.WriteLine("Part1...");
            var computer = new IntCodeComputer();
            var program = new IntCode(text);
            var result = computer.Run(program, 1);
            Console.WriteLine(result.ToString());
        }
        static void Part2(string text)
        {
            Console.WriteLine("Part2...");
            var computer = new IntCodeComputer();
            var program = new IntCode(text);
            var result = computer.Run(program, 5);
            Console.WriteLine(result.ToString());
        }
    }
}
