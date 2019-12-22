using System;
using System.Collections.Generic;

namespace AdventOfCode
{
    public class IntCode
    {
        private long[] _values;
        private Dictionary<long, long> _extendedMemory = new Dictionary<long, long>();

        public long GetValue(long idx)
        {
            if (idx < 0) throw new ArgumentOutOfRangeException(nameof(idx), idx, "Value cannot be negative");
            if (idx < _values.Length)
            {
                return _values[idx];
            }
            if (_extendedMemory.ContainsKey(idx)) return _extendedMemory[idx];
            return 0;
        }

        public void setValue(long idx, long value)
        {
            if (idx < 0) throw new ArgumentOutOfRangeException(nameof(idx), idx, "Value cannot be negative");
            if (idx < _values.Length)
            {
                _values[idx] = value;
            }
            else
            {
                _extendedMemory[idx] = value;
            }
        }

        public IntCode(string[] input)
        {
            _values = new long[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                _values[i] = long.Parse(input[i]);
            }
        }

        public IntCode(long[] input)
        {
            _values = new long[input.Length];
            Array.Copy(input, _values, input.Length);
        }

        public IntCode(string input) : this(input.Split(',')) { }

        public override string ToString()
        {
            string output = string.Empty;
            for (int i = 0; i < _values.Length; i++)
            {
                output += _values[i];
                if ((i + 1) % 40 == 0) output += Environment.NewLine;
                else output += " ";
            }
            output += Environment.NewLine + "Extended Memory:" + Environment.NewLine;
            foreach (var v in _extendedMemory)
            {
                output += $"{v.Key} = {v.Value}{Environment.NewLine}";
            }

            return output;
        }

        public static IntCode DeepCopy(IntCode toCopy)
        {
            return new IntCode(toCopy._values);
        }

        public string GetOutputString(List<long> parameters)
        {
            string output = string.Empty;
            for (int i = 0; i < _values.Length; i++)
            {
                if (parameters.Contains(i))
                {
                    output += "**" + _values[i] + "**";
                }
                else
                {
                    output += _values[i];
                }



                if ((i + 1) % 40 == 0) output += Environment.NewLine;
                else output += " ";
            }
            output += Environment.NewLine + "Extended Memory:" + Environment.NewLine;
            foreach (var v in _extendedMemory)
            {
                output += $"{v.Key} = {v.Value}{Environment.NewLine}";
            }

            return output;
        }
    }
}