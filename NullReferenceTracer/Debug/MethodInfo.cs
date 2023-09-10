using System;
using System.Collections.Generic;
using System.Linq;
using Iced.Intel;
using Microsoft.Diagnostics.Runtime;

namespace NullReferenceTracer.Debug;

public sealed class MethodInfo
{
    private readonly string _signature;
    private readonly ulong _instructionPointer;
    private readonly Instructions _instructions;

    public static MethodInfo Create(ulong instructionPointer, DataTarget dataTarget)
    {
        var clrMethod = GetClrMethod(instructionPointer, dataTarget);
        var instructions = GetMethodInstructions(instructionPointer, dataTarget, clrMethod);
        var signature = clrMethod.Signature ?? "Method signature not found!";
        var method = new MethodInfo(instructionPointer, instructions, signature);
        return method;
    }

    private static Instructions GetMethodInstructions(ulong instructionPointer, DataTarget dataTarget, ClrMethod method)
    {
        var startAddress = method.NativeCode;
        var endAddress = method.ILOffsetMap.Max(entry => entry.EndAddress);

        ulong codeLength;
        if (endAddress > startAddress)
        {
            codeLength = endAddress - startAddress + 1;
        }
        else
        {
            // Ugly workaround! .NET 7 and later runtimes do not return the correct endAddress.
            // In that case we just read 2x the bytes from method start to the exception and
            // hope it is enough.
            codeLength = (instructionPointer - startAddress) * 2;
        }

        var machineCode = new byte[codeLength];

        for (ulong i = 0; i < codeLength; i++)
        {
            machineCode[i] = dataTarget.DataReader.Read<byte>(startAddress + i);
        }

        return Instructions.Create(machineCode, startAddress);
    }

    private static ClrMethod GetClrMethod(ulong instructionPointer, DataTarget dataTarget)
    {
        foreach (var runtime in dataTarget.ClrVersions.Select(x => x.CreateRuntime()))
        {
            var method = runtime.GetMethodByInstructionPointer(instructionPointer);
            if (method is not null)
                return method;
        }

        throw new ArgumentException($"No .NET method found at address: {instructionPointer:X16}", nameof(instructionPointer));
    }

    private MethodInfo(ulong instructionPointer, Instructions instructions, string signature)
    {
        _instructionPointer = instructionPointer;
        _instructions = instructions;
        _signature = signature;
    }

    public override string ToString()
    {
        return _signature + Environment.NewLine + _instructions.ToString(_instructionPointer);
    }
}