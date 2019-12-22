using System;
using System.IO;
using System.Collections.Generic;

namespace AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].Equals("Part1",
              StringComparison.CurrentCultureIgnoreCase))
            {
                Part1(File.ReadAllText("input.txt"));
                return;
            }
            else if (args.Length > 0 && args[0].Equals("Part2",
              StringComparison.CurrentCultureIgnoreCase))
            {
                Part2(File.ReadAllText("input.txt"));
            }
            else
            {
                Test(testsPart1);
                Test(testsPart2);
            }
        }

        public class TestItem
        {
            public string Input;
            public string Result;

        }

        static TestItem[] testsPart1 = new TestItem[] {
            new TestItem() {
                Input = "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99",
                Result = ""
            },
            new TestItem() {
                Input = "1102,34915192,34915192,7,4,7,99,0",
                Result = ""
            },
            new TestItem() {
                Input = "104,1125899906842624,99",
                Result = "1125899906842624"
            }
        };

        static TestItem[] testsPart2 = new TestItem[] {
            new TestItem() {
                Input = "",
                Result = ""
            }
        };

        static void Test(TestItem[] testInput)
        {
            Console.WriteLine("TESTING...");
            IntCodeComputer computer = new IntCodeComputer();


            foreach (var test in testInput)
            {

                try
                {
                    Console.WriteLine($"Testing: {test.Input}");
                    var stopVal = computer.Run(new IntCode(test.Input), null);
                    Console.WriteLine(computer.GetState().Output);
                    while (stopVal != 99)
                    {
                        stopVal = computer.Resume(null);
                        Console.WriteLine(computer.GetState().Output);
                    }
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
                finally
                {

                    Console.WriteLine($"*****************");
                }
            }
        }

        static void Part1(string text)
        {
            Console.WriteLine("Part1...");
            IntCodeComputer computer = new IntCodeComputer();
            var stopVal = computer.Run(new IntCode(text), new List<int>(){1});
            Console.WriteLine(computer.GetState().Output);
        }

        static void Part2(string text)
        {
            Console.WriteLine("Part2...");
            IntCodeComputer computer = new IntCodeComputer();
            var stopVal = computer.Run(new IntCode(text), new List<int>(){2});
            Console.WriteLine(computer.GetState().Output);
        }
    }
}