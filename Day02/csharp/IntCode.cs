using System;

public class IntCode
{
    public int[] State { get; }

    public IntCode(string[] input)
    {
        State = new int[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            State[i] = Int32.Parse(input[i]);
        }
    }

    public IntCode(int[] input)
    {
        State = new int[input.Length];
        Array.Copy(input, State, input.Length);
    }

    public IntCode(string input) : this(input.Split(',')) { }

    public override string ToString()
    {
        string output = string.Empty;
        for (int i = 0; i < State.Length - 1; i++)
        {
            output += State[i] + ",";
        }
        output += State[State.Length - 1];
        return output;
    }

    public static IntCode DeepCopy(IntCode toCopy)
    {
        return new IntCode(toCopy.State);
    }
}