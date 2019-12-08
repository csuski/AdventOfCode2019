using System;
using System.Collections.Generic;

namespace AdventOfCode
{
    public class ComputerState
    {
        public bool Running { get; set; }
        public Queue<int> Input { get; } = new Queue<int>();
        public int Output { get; set; }
        public int InstructionPointer { get; set; }
        public int StopInstruction { get; set; } = -1;
    }

    public class IntCodeComputer
    {
        // A copy of the origina unmodified program given to the compute to run.
        private IntCode _initalProgram;

        // The running program
        private IntCode _memory;

        // The internal state of the computer
        private ComputerState _state;

        public ComputerState GetState() { return _state; }

        public IntCode GetMemory() { return _memory; }

        // Run a program from a new state with the given input
        public int Run(IntCode program, List<int> input)
        {
            _state = new ComputerState();
            _initalProgram = IntCode.DeepCopy(program);
            _memory = IntCode.DeepCopy(program);
            if (input != null)
            {
                foreach (var v in input)
                {
                    _state.Input.Enqueue(v);
                }
            }
            try
            {
                Run();
            }
            catch (Exception)
            {
                DumpMemory();
                throw;
            }
            return _state.StopInstruction;
        }

        // Resume the run from the last position (assuming it wasn't halted)
        // Replace the input with the new input unless the optional addInput
        // is true. Then use the input as additional input.
        public int Resume(List<int> input, bool addInput = false)
        {
            if (input != null)
            {
                if (!addInput)
                {
                    _state.Input.Clear();
                }
                foreach (var v in input)
                {
                    _state.Input.Enqueue(v);
                }
            }
            try
            {
                Run();
            }
            catch (Exception)
            {
                DumpMemory();
                throw;
            }
            return _state.StopInstruction;
        }

        private void Run()
        {
            _state.Running = true;
            while (_state.Running)
            {
                var operation = IntCodeOperationFactory.CreateIntCodeOperation(_memory, _state.InstructionPointer);
                if (!operation.Execute(_memory, _state))
                {
                    _state.Running = false;
                    _state.StopInstruction = operation.OpCode; // if we stop, the stop reason is the opcode.
                }
                _state.InstructionPointer = operation.NextInstruction;
            }
        }

        private void DumpMemory()
        {
            Console.WriteLine($"Current Memory: {_memory}");
            Console.WriteLine($"Current Instruction: {_state.InstructionPointer}");
            Console.WriteLine($"Running : {_state.Running}");
        }
    }
}