using System;

namespace AdventOfCode
{
    public class IntCode
    {
        public int[] Values { get; }

        public IntCode(string[] input)
        {
            Values = new int[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                Values[i] = Int32.Parse(input[i]);
            }
        }

        public IntCode(int[] input)
        {
            Values = new int[input.Length];
            Array.Copy(input, Values, input.Length);
        }

        public IntCode(string input) : this(input.Split(',')) { }

        public override string ToString()
        {
            string output = string.Empty;
            for (int i = 0; i < Values.Length - 1; i++)
            {
                output += Values[i] + ",";
            }
            output += Values[Values.Length - 1];
            return output;
        }

        public static IntCode DeepCopy(IntCode toCopy)
        {
            return new IntCode(toCopy.Values);
        }
    }
}