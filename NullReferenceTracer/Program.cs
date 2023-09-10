using System;
using NullReferenceTracer.ETW;

var nullReferenceEventProcessor = new EtwNullReferenceEventProcessor();
Console.WriteLine("NullReferenceTracer running (Press CTRL+C to exit)");
Console.CancelKeyPress += (_, cancelArgs) =>
{
    Console.WriteLine("CTRL+C pressed");
    nullReferenceEventProcessor.Dispose();
    cancelArgs.Cancel = true;
};

nullReferenceEventProcessor.Start();