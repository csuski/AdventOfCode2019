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
                Input = "",
                Result = ""
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
            foreach (var test in testInput)
            {
                if (!test.Input.Equals(test.Result))
                {
                    Console.WriteLine("***TEST FAILED***");
                    Console.WriteLine($"Input: {test.Input}");
                    Console.WriteLine($"Expected Result: {test.Result}");
                }
                else
                {
                    Console.WriteLine("***Test Passed***");
                }
            }
        }

        static void Part1(string text)
        {
            Console.WriteLine("Part1...");


        }

        static void Part2(string text)
        {
            Console.WriteLine("Part2...");

        }
    }
}