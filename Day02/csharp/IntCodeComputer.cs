using System;

public class IntCodeComputer
{
    private IntCode _initalProgram;
    private IntCode _memory;
    private int _instructionPointer = 0;
    private bool _running;

    public IntCode Run(IntCode program)
    {
        _instructionPointer = 0;
        _initalProgram = program;
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
        return _memory;
    }

    private void Run()
    {
        _running = true;
        while (_running)
        {
            switch (_memory.State[_instructionPointer])
            {
                case 1:
                    Add();
                    break;
                case 2:
                    Multiply();
                    break;
                case 99:
                    Halt();
                    return;
                default:
                    throw new Exception($"Unknow OpCode: {_memory.State[_instructionPointer]}");
            }
        }
    }

    // Operations
    private void Add()
    {
        // value at address 1 plus value at address 2 stored in address 3
        int address1 = _memory.State[_instructionPointer + 1];
        int address2 = _memory.State[_instructionPointer + 2];
        int address3 = _memory.State[_instructionPointer + 3];

        _memory.State[address3] = _memory.State[address1] + _memory.State[address2];
        _instructionPointer += 4;
    }

    private void Multiply()
    {
        // value at address 1 times value at address 2 stored in address 3
        int address1 = _memory.State[_instructionPointer + 1];
        int address2 = _memory.State[_instructionPointer + 2];
        int address3 = _memory.State[_instructionPointer + 3];

        _memory.State[address3] = _memory.State[address1] * _memory.State[address2];
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