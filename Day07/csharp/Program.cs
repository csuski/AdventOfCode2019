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
                Test(tests, new PhaseSequence(0, 4), false);
                Test(tests2, new PhaseSequence(5, 9), true);
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

        static TestItem[] tests2 = new TestItem[] {
            new TestItem() {
                Input = "3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5",
                PhaseSequence = new int[] {9,8,7,6,5},
                Result = 139629729
            },
            new TestItem() {
                Input = "3,52,1001,52,-5,52,3,53,1,52,56,54,1007,54,5,55,1005,55,26,1001,54,-5,54,1105,1,12,1,53,54,53,1008,54,0,55,1001,55,1,55,2,53,55,53,4,53,1001,56,-1,56,1005,56,6,99,0,0,0,0,10",
                PhaseSequence = new int[] {9,7,8,5,6},
                Result = 18216
            },
        };

        static void Test(TestItem[] testInput, PhaseSequence sequence, bool feedbackMode)
        {
            Console.WriteLine("TESTING...");
            foreach (var test in testInput)
            {
                var maxVal = 0;
                var bestSequence = new int[] { -1, -1, -1, -1, -1 };
                foreach (var s in sequence)
                {
                    var result = feedbackMode ? RunComputerInFeedbackMode(test.Input, s) :
                                RunComputerInPhases(test.Input, s);
                    if (result > maxVal)
                    {
                        maxVal = result;
                        bestSequence = s;
                    }
                }

                if (maxVal != test.Result)
                {
                    Console.WriteLine("***TEST FAILED***");
                    Console.WriteLine($"Input: {test.Input}");
                    Console.WriteLine($"Result: {maxVal}");
                    Console.WriteLine($"Sequence: {WriteSequence(bestSequence)}");
                    Console.WriteLine($"Expected Result: {test.Result}");
                    Console.WriteLine($"Expected Sequence: {WriteSequence(test.PhaseSequence)}");
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
                computer.Run(code, new List<int> { p, result });
                result = computer.Output;
            }
            return result;
        }

        static int RunComputerInFeedbackMode(string input, int[] phaseSequence)
        {
            var computer = new IntCodeComputer[phaseSequence.Length];
            var code = new IntCode(input);
            var result = 0;
            var stopCode = 0;
            while (stopCode != 99)
            {
                for (int i = 0; i < phaseSequence.Length; i++)
                {
                    if(computer[i] == null) {
                        computer[i] = new IntCodeComputer();
                        stopCode = computer[i].Run(code, new List<int> {phaseSequence[i], result});
                    }
                    else {
                        stopCode = computer[i].Resume(new List<int> {result});
                    }
                    result = computer[i].Output;
                }
            }
            return result;
        }

        static string WriteSequence(int[] sequence)
        {
            return $"{sequence[0]},{sequence[1]},{sequence[2]},{sequence[3]},{sequence[4]}";
        }

        static void Part1(string text)
        {
            Console.WriteLine("Part1...");
            var sequence = new PhaseSequence(0, 4);
            var maxVal = 0;
            var bestSequence = new int[] { -1, -1, -1, -1, -1 };
            foreach (var s in sequence)
            {
                var result = RunComputerInPhases(text, s);
                if (result > maxVal)
                {
                    maxVal = result;
                    bestSequence = s;
                }
            }
            Console.WriteLine($"Best Sequences = {WriteSequence(bestSequence)}");
            Console.WriteLine($"Best Value = {maxVal}");
        }
        static void Part2(string text)
        {
            Console.WriteLine("Part2...");
            var sequence = new PhaseSequence(5, 9);
            var maxVal = 0;
            var bestSequence = new int[] { -1, -1, -1, -1, -1 };
            foreach (var s in sequence)
            {
                var result = RunComputerInFeedbackMode(text, s);
                if (result > maxVal)
                {
                    maxVal = result;
                    bestSequence = s;
                }
            }
            Console.WriteLine($"Best Sequences = {WriteSequence(bestSequence)}");
            Console.WriteLine($"Best Value = {maxVal}");
        }
    }

    public class PhaseSequence : IEnumerable<int[]>
    {
        private int _a, _b, _c, _d, _e;
        private int _startPhase = 0;
        private int _endPhase = 4;

        public PhaseSequence(int start, int end)
        {
            _startPhase = start;
            _endPhase = end;
        }

        public IEnumerator<int[]> GetEnumerator()
        {
            for (_e = _startPhase; _e <= _endPhase; _e++)
            {
                for (_d = _startPhase; _d <= _endPhase; _d++)
                {
                    if (_d == _e) continue;
                    for (_c = _startPhase; _c <= _endPhase; _c++)
                    {
                        if (_c == _d || _c == _e) continue;
                        for (_b = _startPhase; _b <= _endPhase; _b++)
                        {
                            if (_b == _c || _b == _d || _b == _e) continue;
                            for (_a = _startPhase; _a <= _endPhase; _a++)
                            {
                                if (_a == _b || _a == _c || _a == _d || _a == _e) continue;
                                yield return new int[] { _a, _b, _c, _d, _e };
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