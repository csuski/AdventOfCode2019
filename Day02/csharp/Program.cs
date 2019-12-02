using System;
using System.IO;

namespace Day2
{
    class Program
    {
        static string[] tests = new string[] {
            "1,0,0,0,99",
            "2,3,0,3,99",
            "2,4,4,5,99,0",
            "1,1,1,4,99,5,6,0,99"};

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

        static void Test()
        {
            Console.WriteLine("TESTING...");
            var computer = new IntCodeComputer();
            foreach (var test in tests)
            {
                var t = new IntCode(test);
                Console.WriteLine($"Input = {t.ToString()}");
                var result = computer.Run(t);
                Console.WriteLine($"Output = {result.ToString()}");
            }
        }

        static void Part1(string text)
        {
            Console.WriteLine("Part1...");
            var computer = new IntCodeComputer();
            var program = new IntCode(text);
            // Manual fixup
            program.State[1] = 12;
            program.State[2] = 2;
            var result = computer.Run(program);
            Console.WriteLine(result.ToString());
        }

        static void Part2(string text)
        {
            Console.WriteLine("Part2...");
            var computer = new IntCodeComputer();
            // Run a brute force while I think about it
            var answer = 19690720;
            var program = new IntCode(text);
            var max = program.State.Length - 2;

            for (int noun = 0; noun < max; noun++)
            {
                for (int verb = 0; verb < max; verb++)
                {
                    program.State[1] = noun;
                    program.State[2] = verb;
                    try
                    {
                        var result = computer.Run(program);

                        if (result.State[0] == answer)
                        {
                            Console.WriteLine($"Found answer with noun = {noun} and verb = {verb}");
                            return;
                        }
                        else
                        {
                            Console.WriteLine($"noun = {noun} and verb = {verb} failed");
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"noun = {noun} and verb = {verb} exception");
                    }
                }
            }
        }
    }
}