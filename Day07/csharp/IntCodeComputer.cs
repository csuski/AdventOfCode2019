using System;
using System.Collections.Generic;

public class IntCodeComputer
{
    private IntCode _initalProgram;
    private IntCode _memory;
    private int _instructionPointer = 0;
    private bool _running;
    private List<int> _input;
    private int _currentInput = 0;
    public int Output {get; private set;}
    private int _stopReason;    // Use op code we stopped at, 99 for halt, 4 for output

    public int Run(IntCode program, List<int> input)
    {
        _instructionPointer = 0;
        _initalProgram = program;
        _input = input;
        _currentInput = 0;
        _memory = IntCode.DeepCopy(program);
        try
        {
            Run();
        }
        catch (Exception)
        {
            DumpMemory();
            throw;
        }
        return _stopReason;
    }

    public int Resume(List<int> input) {
        _input = input;
        _currentInput = 0;
        try
        {
            Run();
        }
        catch (Exception)
        {
            DumpMemory();
            throw;
        }
        return _stopReason;
    }

    private void Run()
    {
        _running = true;
        while (_running)
        {
            ParseOpCode(_memory.State[_instructionPointer], out int operation,
                out int paramMode1, out int paramMode2, out int paramMode3);
            _stopReason = operation;    // if we stop, the stop reason is the opcode.
            switch (operation)
            {
                case 1:
                    Add(paramMode1, paramMode2);
                    break;
                case 2:
                    Multiply(paramMode1, paramMode2);
                    break;
                case 3:
                    StoreInput();
                    break;
                case 4:
                    Output = OutputRegister(paramMode1);
                    _running = false; // stop on output
                    break;
                case 5:
                    JumpIfTrue(paramMode1, paramMode2);
                    break;
                case 6:
                    JumpIfFalse(paramMode1, paramMode2);
                    break;
                case 7:
                    LessThanOp(paramMode1, paramMode2);
                    break;
                case 8:
                    EqualsOp(paramMode1, paramMode2);
                    break;
                case 99:
                    Halt();
                    return;
                default:
                    throw new Exception($"Unknow OpCode: {_memory.State[_instructionPointer]}");
            }
        }
    }

    private void ParseOpCode(int code, out int operation, out int paramMode1,
        out int paramMode2, out int paramMode3)
    {
        operation = code % 100;
        code /= 100;
        paramMode1 = code % 10;
        code /= 10;
        paramMode2 = code % 10;
        code /= 10;
        paramMode3 = code % 10;
    }

    private int GetValue(int address, int paramMode)
    {
        if (paramMode == 0)
        {
            return _memory.State[address];
        }
        else if (paramMode == 1)
        {
            return address;
        }
        throw new Exception($"Only param mode 0 and 1 are accepted. Received {paramMode}");
    }

    // Operations
    private void Add(int paramMode1, int paramMode2)
    {
        // value at address 1 plus value at address 2 stored in address 3
        int address1 = _memory.State[_instructionPointer + 1];
        int address2 = _memory.State[_instructionPointer + 2];
        int address3 = _memory.State[_instructionPointer + 3];

        _memory.State[address3] = GetValue(address1, paramMode1) +
            GetValue(address2, paramMode2);
        _instructionPointer += 4;
    }

    private void Multiply(int paramMode1, int paramMode2)
    {
        // value at address 1 times value at address 2 stored in address 3
        int address1 = _memory.State[_instructionPointer + 1];
        int address2 = _memory.State[_instructionPointer + 2];
        int address3 = _memory.State[_instructionPointer + 3];

        _memory.State[address3] = GetValue(address1, paramMode1) *
            GetValue(address2, paramMode2);
        _instructionPointer += 4;
    }

    private void StoreInput()
    {
        if(_currentInput >= _input.Count)
            throw new Exception("Ran out of input for input operation");
        var input = _input[_currentInput++];
        int address = _memory.State[_instructionPointer + 1];
        _memory.State[address] = input;
        _instructionPointer += 2;
    }

    private int OutputRegister(int paramMode)
    {
        int address = _memory.State[_instructionPointer + 1];
        var output = paramMode == 0 ? _memory.State[address] : address;
        _instructionPointer += 2;
        return output;
    }

    private void JumpIfTrue(int paramMode1, int paramMode2)
    {
        Func<int, bool> isNotZero = (v) => v != 0;
        JumpIf(paramMode1, paramMode2, isNotZero);
    }

    private void JumpIfFalse(int paramMode1, int paramMode2)
    {
        Func<int, bool> isZero = (v) => v == 0;
        JumpIf(paramMode1, paramMode2, isZero);
    }

    private void JumpIf(int paramMode1, int paramMode2, Func<int, bool> func)
    {
        int address1 = _memory.State[_instructionPointer + 1];
        int address2 = _memory.State[_instructionPointer + 2];
        var val = GetValue(address1, paramMode1);
        var val2 = GetValue(address2, paramMode2);

        if (func.Invoke(val))
        {
            _instructionPointer = val2;
        }
        else
        {
            _instructionPointer += 3;
        }
    }

    private void LessThanOp(int paramMode1, int paramMode2)
    {
        Func<int, int, bool> lessThan = (v1, v2) => v1 < v2;
        StoreComparison(paramMode1, paramMode2, lessThan);
    }

    private void EqualsOp(int paramMode1, int paramMode2)
    {
        Func<int, int, bool> equals = (v1, v2) => v1 == v2;
        StoreComparison(paramMode1, paramMode2, equals);
    }

    private void StoreComparison(int paramMode1, int paramMode2, Func<int, int, bool> func)
    {
        int address1 = _memory.State[_instructionPointer + 1];
        int address2 = _memory.State[_instructionPointer + 2];
        int address3 = _memory.State[_instructionPointer + 3];
        var val = GetValue(address1, paramMode1);
        var val2 = GetValue(address2, paramMode2);
        _memory.State[address3] = func.Invoke(val, val2) ? 1 : 0;
        _instructionPointer += 4;
    }

    private void Halt()
    {
        _running = false;
        _instructionPointer += 1;
    }

    private void DumpMemory()
    {
        Console.WriteLine($"Current Memory: {_memory.ToString()}");
        Console.WriteLine($"Current Instruction: {_instructionPointer}");
        Console.WriteLine($"Running : {_running.ToString()}");
    }
}