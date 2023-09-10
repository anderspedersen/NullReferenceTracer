using System;
using System.Collections.Generic;
using System.Text;
using Iced.Intel;
using Decoder = Iced.Intel.Decoder;

namespace NullReferenceTracer.Debug;

public sealed class Instructions
{
    private readonly List<Instruction> _instructions;

    private static readonly NasmFormatter _formatter = new NasmFormatter
    {
        Options =
        {
            DigitSeparator = "`",
            FirstOperandCharIndex = 10
        }
    };

    private Instructions(List<Instruction> instructions)
    {
        _instructions = instructions;
    }

    public static Instructions Create(byte[] machineCode, ulong startAddress)
    {
        var codeReader = new ByteArrayCodeReader(machineCode);
        var decoder = Decoder.Create(64, codeReader);
        decoder.IP = startAddress;
        var endRip = decoder.IP + (uint) machineCode.Length;
        var instructions = new List<Instruction>();

        while (decoder.IP < endRip)
        {
            instructions.Add(decoder.Decode());
        }
        
        return new Instructions(instructions);
    }

    public string ToString(ulong markedInstructionAddress)
    {
        var sb = new StringBuilder();
        var formatter = _formatter;
        var output = new StringOutput();
        foreach (var instruction in _instructions)
        {
            formatter.Format(instruction, output);
            if (instruction.IP == markedInstructionAddress)
            {
                sb.Append("* ");
            }

            sb.Append(instruction.IP.ToString("X16"));
            sb.Append(' ');
            sb.Append(output.ToStringAndReset());
            sb.AppendLine();
        }
        
        return sb.ToString();
    }
}