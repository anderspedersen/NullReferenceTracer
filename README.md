# NullReferenceTracer
NullReferenceTracer is a small tool for debugging "impossible" NullReferenceExceptions in .NET.

Sometimes NullReferenceExceptions will have stack traces that claims the NullReferenceException happened where it should not be possible. This is usually caused by
compiler optimizations such as inlining, which will cause .NET to point to the wrong line number and/or method. If the NullReferenceException only happens once a
week in production it can be a very slow process to find the cause of the NullReferenceException. NullReferenceTracer circumvents this problem by ignoring the debugging
symbols and looking directly at the assembly code.

## How does it work?
NullReferenceExceptions are normally thrown automatically by the .NET runtime when an instruction is trying to read from memory location 0x0 or close to it. NullReferenceTracer
uses Event Tracing for Windows (ETW) to trace this event and find the exact value of the instruction pointer where the NullReferenceException was thrown. It then attaches a debugger
to the process to get the method where the exception was thrown, which it then disassembles and prints to the screen, and with some basic assembly language knowledge, it should be
fairly easy to determine what caused the NullReferenceException.

If you don't know assembly I have written a [small guide using the ExampleProject](Guide/Guide.md) from this solution.