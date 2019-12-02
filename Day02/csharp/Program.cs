using System;
using System.IO;

namespace Day2
{
    class Program
    {
        static string[] tests  = new string[] {
            "1,0,0,0,99", 
            "2,3,0,3,99",
            "2,4,4,5,99,0",
            "1,1,1,4,99,5,6,0,99"};

        static void Main(string[] args)
        {
            //Test();
            //Part1(File.ReadAllText("input.txt"));
            Part2(File.ReadAllText("input.txt"));
        }

        static void Test() {
            foreach(var test in tests) {
                int[] positions = ConvertToIntArray(test.Split(','));
                StartProgram(positions);
                PrintState(positions);
            }
        }

        static void Part1(string text) {
            int[] positions = ConvertToIntArray(text.Split(','));
            // Manual fixup
            positions[1] = 12;
            positions[2] = 2;
            StartProgram(positions);
            PrintState(positions);
        }

        static void PrintState(int[] state) {
            for(int i =0; i < state.Length-1;i++) {
                Console.Write(state[i] + ",");
            }
            Console.WriteLine(state[state.Length-1]);
        }

        static int[] ConvertToIntArray(string[] values) {
            int[] positions = new int[values.Length];
            for(int i =0; i < values.Length; i++) {
                positions[i] = Int32.Parse(values[i]);
            }
            return positions;
        }

        static void StartProgram(int[] state) {
            int current = 0;
            while(true) {
                switch(state[current]) {
                    case 1:
                        Add(current, state);
                        current += 4;
                        break;
                    case 2:
                        Multiply(current, state);
                        current += 4;
                        break;
                    case 99:
                        return;
                    default:
                        throw new Exception($"Unknow OpCode: {state[current]}");
                }
            }
        }

        static void Add(int cur, int[] state) {
            int valLocation1 = state[cur+1];
            int valLocation2 = state[cur+2];
            int registerLocation = state[cur+3];

            state[registerLocation] = state[valLocation1] + state[valLocation2];
        }

        static void Multiply(int cur, int[] state) {
            int valLocation1 = state[cur+1];
            int valLocation2 = state[cur+2];
            int registerLocation = state[cur+3];

            state[registerLocation] = state[valLocation1] * state[valLocation2];
        }

        static void Part2(string text) {
            // Run a brute force while I think about it
            var answer = 19690720;
            int[] positions = ConvertToIntArray(text.Split(','));
            var max = positions.Length - 2;

            for(int noun =0; noun < max; noun++) {
                for(int verb = 0; verb < max; verb++) {
                    positions[1] = noun;
                    positions[2] = verb;
                    try {
                        var input = new int[positions.Length];
                        Array.Copy(positions, input, positions.Length);
                        StartProgram(input);
                        if(input[0] == answer) {
                            Console.WriteLine($"Found answer with noun = {noun} and verb = {verb}");
                            return;
                        }
                        else {
                            Console.WriteLine($"noun = {noun} and verb = {verb} failed");
                        }
                    }
                    catch(Exception) {
                        Console.WriteLine($"noun = {noun} and verb = {verb} exception");
                    }
                }
            }
        }
    }
}
