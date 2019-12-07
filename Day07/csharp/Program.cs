using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

namespace Day07
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
                var text = File.ReadAllText("input.txt");
                var r = RunComputerInPhases(text, new int[] {0,4,2,1,3});
                Console.WriteLine(r);



                //Test();
            }
        }

        public class TestItem
        {
            public string Input;
            public int[] PhaseSequence;
            public int Result;

        }

        static TestItem[] tests = new TestItem[] {
            new TestItem() {
                Input = "3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0",
                PhaseSequence = new int[] {4,3,2,1,0},
                Result = 43210
            },
            new TestItem() {
                Input = "3,23,3,24,1002,24,10,24,1002,23,-1,23,101,5,23,23,1,24,23,23,4,23,99,0,0",
                PhaseSequence = new int[] {0,1,2,3,4},
                Result = 54321
            },
            new TestItem() {
                Input = "3,31,3,32,1002,32,10,32,1001,31,-2,31,1007,31,0,33,1002,33,7,33,1,33,31,31,1,32,31,31,4,31,99,0,0,0",
                PhaseSequence = new int[] {1,0,4,3,2},
                Result = 65210
            }
        };

        static void Test()
        {
            Console.WriteLine("TESTING...");
            var computer = new IntCodeComputer();
            foreach (var test in tests)
            {
                var result = RunComputerInPhases(test.Input, test.PhaseSequence);
                if (result != test.Result)
                {
                    Console.WriteLine("***TEST FAILED***");
                    Console.WriteLine($"Input: {test.Input}");
                    Console.WriteLine($"Result: {result}");
                    Console.WriteLine($"Expected Result: {test.Result}");
                }
                else
                {
                    Console.WriteLine("***Test Passed***");
                }
            }
        }

        static int RunComputerInPhases(string input, int[] phaseSequence)
        {
            var computer = new IntCodeComputer();
            var code = new IntCode(input);
            var result = 0;
            foreach (var p in phaseSequence)
            {
                result = computer.Run(code, new List<int> { p, result });
            }
            return result;
        }

        static string WriteSequence(int[] sequence) {
            return $"{sequence[0]},{sequence[1]},{sequence[2]},{sequence[3]},{sequence[4]}";
        }

        static void Part1(string text)
        {
            Console.WriteLine("Part1...");
            var sequence = new PhaseSequence();
            var maxVal = 0;
            var bestSequence = new int[] {-1, -1, -1, -1, -1};
            foreach(var s in sequence) {
                var result = RunComputerInPhases(text, s);
                if(result > maxVal) {
                    maxVal = result;
                    bestSequence = s;
                }
                Console.WriteLine($"{WriteSequence(s)}, {result}");
            }
            Console.WriteLine($"Best Sequences = {WriteSequence(bestSequence)}");
            Console.WriteLine($"Best Value = {maxVal}");
        }
        static void Part2(string text)
        {
            /*  Console.WriteLine("Part2...");
              var computer = new IntCodeComputer();
              var program = new IntCode(text);
              var result = computer.Run(program, 5);
              Console.WriteLine(result.ToString());*/
        }
    }

    public class PhaseSequence : IEnumerable<int[]>
    {
        private int _a, _b, _c, _d, _e;
        private const int _startPhase = 0;
        private const int _endPhase = 4;

        public IEnumerator<int[]> GetEnumerator()
        {
            for (_e = _startPhase; _e <= _endPhase; _e++)
            {
                for (_d = _startPhase; _d <= _endPhase; _d++)
                {
                    if(_d == _e) continue;
                    for (_c = _startPhase; _c <= _endPhase; _c++)
                    {
                        if(_c == _d || _c == _e) continue;
                        for (_b = _startPhase; _b <= _endPhase; _b++)
                        {
                            if(_b == _c || _b == _d || _b == _e) continue;
                            for (_a = _startPhase; _a <= _endPhase; _a++)
                            {
                                if(_a == _b || _a == _c || _a == _d || _a == _e) continue;
                                yield return new int[] {_a, _b, _c, _d, _e};
                            }
                        }
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

