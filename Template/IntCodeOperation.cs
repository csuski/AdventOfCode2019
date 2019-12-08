using System;

namespace AdventOfCode
{
    public static class IntCodeOperationFactory
    {
        public static IntCodeOperation CreateIntCodeOperation(IntCode code, int currentInstruction)
        {
            ParseOpCode(code.Values[currentInstruction],
                out int operation, out int pMode1, out int pMode2, out int pMode3);
            var op = CreateIntCodeOperation(currentInstruction, operation,
                    ConvertToParamMode(new int[] { pMode1, pMode2, pMode3 }));
            return op;
        }

        private static void ParseOpCode(int code, out int operation, out int paramMode1,
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

        private static IntCodeOperation CreateIntCodeOperation(int currentInstruction, int opCode,
                ParameterMode[] parameterModes)
        {
            switch (opCode)
            {
                case 1:
                    return new AddOperation(opCode, currentInstruction, parameterModes);
                case 2:
                    return new MultiplyOperation(opCode, currentInstruction, parameterModes);
                case 3:
                    return new StoreInputOperation(opCode, currentInstruction, parameterModes);
                case 4:
                    return new OutputRegisterOperation(opCode, currentInstruction, parameterModes);
                case 5:
                case 6:
                    return new JumpOperation(opCode, currentInstruction, parameterModes);
                case 7:
                    return new LessThanOperation(opCode, currentInstruction, parameterModes);
                case 8:
                    return new EqualsOperation(opCode, currentInstruction, parameterModes);
                case 99:
                    return new HaltOperation(opCode, currentInstruction, parameterModes);
                default:
                    throw new Exception($"Unknown OpCode: {opCode}");
            }
        }

        private static ParameterMode[] ConvertToParamMode(int[] paramModeSettings)
        {
            var paramMode = new ParameterMode[paramModeSettings.Length];
            for (int i = 0; i < paramModeSettings.Length; i++)
            {
                paramMode[i] = paramModeSettings[i] == 0 ? ParameterMode.Position : ParameterMode.Immediate;
            }
            return paramMode;
        }
    }

    public enum ParameterMode { Position, Immediate }

    public interface IntCodeOperation
    {
        public int OpCode { get; }
        public int NextInstruction { get; }
        // Execute the operation, return the next instruction
        bool Execute(IntCode code, ComputerState state);
    }

    public abstract class IntCodeOperationBase : IntCodeOperation
    {
        protected ParameterMode[] ParameterModes { get; }

        public int OpCode { get; }
        public abstract int NextInstruction { get; }
        protected int InstructionLocation { get; }

        public abstract bool Execute(IntCode code, ComputerState state);

        protected int GetValue(ParameterMode mode, int val, IntCode code)
        {
            if (mode == ParameterMode.Immediate) return val;
            if (mode == ParameterMode.Position)
            {
                return code.Values[val];
            }
            throw new ArgumentException($"Unexpected mode {mode}.", nameof(mode));
        }

        protected IntCodeOperationBase(int opCode, int currentInstruction, ParameterMode[] parameterModes)
        {
            OpCode = opCode;
            InstructionLocation = currentInstruction;
            ParameterModes = parameterModes;
        }

        protected int GetParameter(int paramIdx, IntCode code)
        {
            return GetValue(ParameterModes[paramIdx], code.Values[InstructionLocation + 1 + paramIdx], code);
        }

        protected int GetRegister(int paramIdx, IntCode code)
        {
            if (ParameterModes[paramIdx] != ParameterMode.Position)
            {
                throw new Exception("Anything that uses a register, must have its Parameter Mode set to Position");
            }
            return code.Values[InstructionLocation + 1 + paramIdx];
        }
    }

    public abstract class MathOperation : IntCodeOperationBase
    {
        protected Func<int, int, int> _function;

        public override int NextInstruction => InstructionLocation + 4;

        protected MathOperation(int opCode, int currentInstruction, ParameterMode[] parameterModes,
            Func<int, int, int> function) : base(opCode, currentInstruction, parameterModes)
        {
            if (parameterModes.Length != 3)
            {
                throw new ArgumentException($"{nameof(AddOperation)} must have 3 parameter mode arguments, received {parameterModes.Length}",
                    nameof(parameterModes));
            }
            if (parameterModes[2] != ParameterMode.Position)
            {
                throw new ArgumentException($"3rd Parameter to {nameof(AddOperation)} must be a in Position Mode.",
                    nameof(parameterModes));
            }
            _function = function;
        }

        public override bool Execute(IntCode code, ComputerState state)
        {
            var paramet1 = GetParameter(0, code);
            var paramet2 = GetParameter(1, code);
            var register = GetRegister(2, code);

            code.Values[register] = _function.Invoke(paramet1, paramet2);
            return true;
        }
    }

    public class AddOperation : MathOperation
    {
        public AddOperation(int opCode, int currentInstruction, ParameterMode[] parameterModes) :
            base(opCode, currentInstruction, parameterModes, (v1, v2) => v1 + v2)
        { }
    }

    public class MultiplyOperation : MathOperation
    {
        public MultiplyOperation(int opCode, int currentInstruction, ParameterMode[] parameterModes) :
            base(opCode, currentInstruction, parameterModes, (v1, v2) => v1 * v2)
        { }
    }

    public class StoreInputOperation : IntCodeOperationBase
    {
        public StoreInputOperation(int opCode, int currentInstruction, ParameterMode[] parameterModes) :
            base(opCode, currentInstruction, parameterModes)
        {
        }

        public override int NextInstruction => InstructionLocation + 2;

        public override bool Execute(IntCode code, ComputerState state)
        {
            var input = state.Input.Dequeue();
            var register = GetRegister(0, code);
            code.Values[register] = input;
            return true;
        }
    }

    public class OutputRegisterOperation : IntCodeOperationBase
    {
        public OutputRegisterOperation(int opCode, int currentInstruction, ParameterMode[] parameterModes) :
            base(opCode, currentInstruction, parameterModes)
        {
        }

        public override int NextInstruction => InstructionLocation + 2;

        public override bool Execute(IntCode code, ComputerState state)
        {
            var value = GetParameter(0, code);
            state.Output = value;
            return false;
        }
    }

    public class JumpOperation : IntCodeOperationBase
    {
        protected bool _jumpIfTrue;

        public JumpOperation(int opCode, int currentInstruction, ParameterMode[] parameterModes) :
            base(opCode, currentInstruction, parameterModes)
        {
            if (opCode == 5) _jumpIfTrue = true;
            else _jumpIfTrue = false;
            _nextInstruction = currentInstruction;
        }

        private int _nextInstruction;
        public override int NextInstruction => _nextInstruction;

        public override bool Execute(IntCode code, ComputerState state)
        {
            var value = GetParameter(0, code);
            var jumpRegister = GetParameter(1, code);
            if ((_jumpIfTrue && value != 0) ||
               (!_jumpIfTrue && value == 0))
            {
                _nextInstruction = jumpRegister;
            }
            else
            {
                _nextInstruction += 3;
            }
            return true;
        }
    }

    public abstract class ComparisonOperation : IntCodeOperationBase
    {
        protected Func<int, int, bool> _function;

        public override int NextInstruction => InstructionLocation + 4;

        protected ComparisonOperation(int opCode, int currentInstruction, ParameterMode[] parameterModes,
            Func<int, int, bool> function) : base(opCode, currentInstruction, parameterModes)
        {
            if (parameterModes.Length != 3)
            {
                throw new ArgumentException($"{nameof(AddOperation)} must have 3 parameter mode arguments, received {parameterModes.Length}",
                    nameof(parameterModes));
            }
            if (parameterModes[2] != ParameterMode.Position)
            {
                throw new ArgumentException($"3rd Parameter to {nameof(AddOperation)} must be a in Position Mode.",
                    nameof(parameterModes));
            }
            _function = function;
        }

        public override bool Execute(IntCode code, ComputerState state)
        {
            var paramet1 = GetParameter(0, code);
            var paramet2 = GetParameter(1, code);
            var register = GetRegister(2, code);

            code.Values[register] = _function.Invoke(paramet1, paramet2) ? 1 : 0;
            return true;
        }
    }

    public class LessThanOperation : ComparisonOperation {
        public LessThanOperation(int opCode, int currentInstruction, ParameterMode[] parameterModes) : 
            base(opCode, currentInstruction, parameterModes, (v1, v2) => v1 < v2)  {}
    }

    public class EqualsOperation : ComparisonOperation {
        public EqualsOperation(int opCode, int currentInstruction, ParameterMode[] parameterModes) : 
            base(opCode, currentInstruction, parameterModes, (v1, v2) => v1 == v2)  {}
    }

    public class HaltOperation : IntCodeOperationBase
    {
        public HaltOperation(int opCode, int currentInstruction, ParameterMode[] parameterModes)
             : base(opCode, currentInstruction, parameterModes) {}

        public override int NextInstruction => -1;

        public override bool Execute(IntCode code, ComputerState state)
        {
            return false;
        }
    }

}