using System;

namespace AdventOfCode
{
    public static class IntCodeOperationFactory
    {
        public static IntCodeOperation CreateIntCodeOperation(IntCode code, long currentInstruction)
        {
            ParseOpCode(code.GetValue(currentInstruction),
                out int operation, out int pMode1, out int pMode2, out int pMode3);
            var op = CreateIntCodeOperation(currentInstruction, operation,
                    ConvertToParamMode(new int[] { pMode1, pMode2, pMode3 }));
            return op;
        }

        private static void ParseOpCode(long code, out int operation, out int paramMode1,
            out int paramMode2, out int paramMode3)
        {
            operation = (int)code % 100;
            code /= 100;
            paramMode1 = (int)code % 10;
            code /= 10;
            paramMode2 = (int)code % 10;
            code /= 10;
            paramMode3 = (int)code % 10;
        }

        private static IntCodeOperation CreateIntCodeOperation(long currentInstruction, int opCode,
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
                case 9:
                    return new RelativeBaseOffsetOperation(opCode, currentInstruction, parameterModes);
                default:
                    throw new Exception($"Unknown OpCode: {opCode}");
            }
        }

        private static ParameterMode[] ConvertToParamMode(int[] paramModeSettings)
        {
            var paramMode = new ParameterMode[paramModeSettings.Length];
            for (int i = 0; i < paramModeSettings.Length; i++)
            {
                switch (paramModeSettings[i])
                {
                    case 0:
                        paramMode[i] = ParameterMode.Position;
                        break;
                    case 1:
                        paramMode[i] = ParameterMode.Immediate;
                        break;
                    case 2:
                        paramMode[i] = ParameterMode.Relative;
                        break;
                    default:
                        throw new Exception($"Unknown parameter mode '{paramModeSettings[i]}'");
                }
            }
            return paramMode;
        }
    }

    public enum ParameterMode { Position, Immediate, Relative }

    public interface IntCodeOperation
    {
        public int OpCode { get; }
        public long NextInstruction { get; }
        // Execute the operation, return the next instruction
        bool Execute(IntCode code, ComputerState state);
    }

    public abstract class IntCodeOperationBase : IntCodeOperation
    {
        protected ParameterMode[] ParameterModes { get; }

        public int OpCode { get; }
        public abstract long NextInstruction { get; }
        protected long InstructionLocation { get; }

        public abstract bool Execute(IntCode code, ComputerState state);

        protected long GetValue(ParameterMode mode, long val, IntCode code, long relativeBase)
        {
            if (mode == ParameterMode.Immediate) return val;
            if (mode == ParameterMode.Position)
            {
                return code.GetValue(val);
            }
            if (mode == ParameterMode.Relative)
            {
                return code.GetValue(relativeBase + val);
            }
            throw new ArgumentException($"Unexpected mode {mode}.", nameof(mode));
        }

        protected IntCodeOperationBase(int opCode, long currentInstruction, ParameterMode[] parameterModes)
        {
            OpCode = opCode;
            InstructionLocation = currentInstruction;
            ParameterModes = parameterModes;
        }

        protected long GetParameter(int paramIdx, IntCode code, long relativeBase)
        {
            return GetValue(ParameterModes[paramIdx], code.GetValue(InstructionLocation + 1 + paramIdx), code, relativeBase);
        }

        protected long GetRegister(int paramIdx, IntCode code, long relativeBase)
        {
            if (ParameterModes[paramIdx] == ParameterMode.Position)
            {
                return code.GetValue(InstructionLocation + 1 + paramIdx);    
            }
            if (ParameterModes[paramIdx] == ParameterMode.Relative)
            {
                return relativeBase + code.GetValue(InstructionLocation + 1 + paramIdx);    
            }
            throw new Exception("Anything that uses a register, must have its Parameter Mode set to Position or Relative");
            
        }

        public override string ToString() {
            return $"{GetType().ToString()} - Parameters = {ParameterModes.Length}";
        }
    }

    public abstract class MathOperation : IntCodeOperationBase
    {
        protected Func<long, long, long> _function;

        public override long NextInstruction => InstructionLocation + 4;

        protected MathOperation(int opCode, long currentInstruction, ParameterMode[] parameterModes,
            Func<long, long, long> function) : base(opCode, currentInstruction, parameterModes)
        {
            if (parameterModes.Length != 3)
            {
                throw new ArgumentException($"{nameof(AddOperation)} must have 3 parameter mode arguments, received {parameterModes.Length}",
                    nameof(parameterModes));
            }
            if (parameterModes[2] == ParameterMode.Immediate)
            {
                throw new ArgumentException($"3rd Parameter to {nameof(AddOperation)} must not be in immediate Mode.",
                    nameof(parameterModes));
            }
            _function = function;
        }

        public override bool Execute(IntCode code, ComputerState state)
        {
            var paramet1 = GetParameter(0, code, state.RelativeBase);
            var paramet2 = GetParameter(1, code, state.RelativeBase);
            var register = GetRegister(2, code, state.RelativeBase);

            code.setValue(register, _function.Invoke(paramet1, paramet2));
            return true;
        }
    }

    public class AddOperation : MathOperation
    {
        public AddOperation(int opCode, long currentInstruction, ParameterMode[] parameterModes) :
            base(opCode, currentInstruction, parameterModes, (v1, v2) => v1 + v2)
        { }
    }

    public class MultiplyOperation : MathOperation
    {
        public MultiplyOperation(int opCode, long currentInstruction, ParameterMode[] parameterModes) :
            base(opCode, currentInstruction, parameterModes, (v1, v2) => v1 * v2)
        { }
    }

    public class StoreInputOperation : IntCodeOperationBase
    {
        public StoreInputOperation(int opCode, long currentInstruction, ParameterMode[] parameterModes) :
            base(opCode, currentInstruction, parameterModes)
        {
        }

        public override long NextInstruction => InstructionLocation + 2;

        public override bool Execute(IntCode code, ComputerState state)
        {
            var input = state.Input.Dequeue();
            var register = GetRegister(0, code, state.RelativeBase);
            code.setValue(register, input);
            return true;
        }
    }

    public class OutputRegisterOperation : IntCodeOperationBase
    {
        public OutputRegisterOperation(int opCode, long currentInstruction, ParameterMode[] parameterModes) :
            base(opCode, currentInstruction, parameterModes)
        {
        }

        public override long NextInstruction => InstructionLocation + 2;

        public override bool Execute(IntCode code, ComputerState state)
        {
            var value = GetParameter(0, code, state.RelativeBase);
            state.Output = value;
            return false;
        }
    }

    public class JumpOperation : IntCodeOperationBase
    {
        protected bool _jumpIfTrue;

        public JumpOperation(int opCode, long currentInstruction, ParameterMode[] parameterModes) :
            base(opCode, currentInstruction, parameterModes)
        {
            if (opCode == 5) _jumpIfTrue = true;
            else _jumpIfTrue = false;
            _nextInstruction = currentInstruction;
        }

        private long _nextInstruction;
        public override long NextInstruction => _nextInstruction;

        public override bool Execute(IntCode code, ComputerState state)
        {
            var value = GetParameter(0, code, state.RelativeBase);
            var jumpRegister = GetParameter(1, code, state.RelativeBase);
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
        protected Func<long, long, bool> _function;

        public override long NextInstruction => InstructionLocation + 4;

        protected ComparisonOperation(int opCode, long currentInstruction, ParameterMode[] parameterModes,
            Func<long, long, bool> function) : base(opCode, currentInstruction, parameterModes)
        {
            if (parameterModes.Length != 3)
            {
                throw new ArgumentException($"{nameof(AddOperation)} must have 3 parameter mode arguments, received {parameterModes.Length}",
                    nameof(parameterModes));
            }
            if (parameterModes[2] == ParameterMode.Immediate)
            {
                throw new ArgumentException($"3rd Parameter to {nameof(AddOperation)} must not be in immediate Mode.",
                    nameof(parameterModes));
            }
            _function = function;
        }

        public override bool Execute(IntCode code, ComputerState state)
        {
            var paramet1 = GetParameter(0, code, state.RelativeBase);
            var paramet2 = GetParameter(1, code, state.RelativeBase);
            var register = GetRegister(2, code, state.RelativeBase);

            code.setValue(register, _function.Invoke(paramet1, paramet2) ? 1 : 0);
            return true;
        }
    }

    public class LessThanOperation : ComparisonOperation
    {
        public LessThanOperation(int opCode, long currentInstruction, ParameterMode[] parameterModes) :
            base(opCode, currentInstruction, parameterModes, (v1, v2) => v1 < v2)
        { }
    }

    public class EqualsOperation : ComparisonOperation
    {
        public EqualsOperation(int opCode, long currentInstruction, ParameterMode[] parameterModes) :
            base(opCode, currentInstruction, parameterModes, (v1, v2) => v1 == v2)
        { }
    }

    public class RelativeBaseOffsetOperation : IntCodeOperationBase
    {
        public RelativeBaseOffsetOperation(int opCode, long currentInstruction, ParameterMode[] parameterModes) :
            base(opCode, currentInstruction, parameterModes)
        {
        }

        public override long NextInstruction => InstructionLocation + 2;

        public override bool Execute(IntCode code, ComputerState state)
        {
            var parameter = GetParameter(0, code, state.RelativeBase);
            state.RelativeBase += parameter;
            return true;
        }
    }

    public class HaltOperation : IntCodeOperationBase
    {
        public HaltOperation(int opCode, long currentInstruction, ParameterMode[] parameterModes)
             : base(opCode, currentInstruction, parameterModes) { }

        public override long NextInstruction => -1;

        public override bool Execute(IntCode code, ComputerState state)
        {
            return false;
        }
    }

}