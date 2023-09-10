using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Etlx;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using Microsoft.Diagnostics.Tracing.Session;
using System;

namespace NullReferenceTracer.ETW
{
    internal class EtwNullReferenceEventProcessor : IDisposable
    {
        private TraceEventSession? _session;

        public void Start()
        {
            if (!(TraceEventSession.IsElevated() ?? false))
            {
                Console.WriteLine("To turn on ETW events you need to be Administrator, please run from an Admin process.");

                return;
            }

            _session = new TraceEventSession("NullReferenceTracerSession");
            _session.EnableKernelProvider(
                KernelTraceEventParser.Keywords.ImageLoad |
                KernelTraceEventParser.Keywords.Process
            );

            _session.EnableProvider(
                ClrTraceEventParser.ProviderGuid,
                TraceEventLevel.Error,
                (ulong)(ClrTraceEventParser.Keywords.Exception |
                        ClrTraceEventParser.Keywords.Stack));

            using var traceLogSource = TraceLog.CreateFromTraceEventSession(_session);

            traceLogSource.Clr.ExceptionStart += HandeExceptionEvent;

            traceLogSource.Process();
        }

        private static void HandeExceptionEvent(ExceptionTraceData data)
        {
            if (data.ExceptionType == "System.NullReferenceException")
            {
                var processId = data.ProcessID;
                var cs = data.CallStack();

                cs = SkipKernelAndRuntimeFrames(cs);

                Console.WriteLine($"Got NullReferenceException in process. PID={processId}");

                using (var debugger = new Debug.Debugger(processId))
                {
                    var method = debugger.GetMethodInfo(cs.CodeAddress.Address);
                    Console.WriteLine(method.ToString());
                }
            }
        }

        private static TraceCallStack SkipKernelAndRuntimeFrames(TraceCallStack cs)
        {
            while (cs.CodeAddress.ModuleName is "coreclr" or "ntdll" or "kernelbase")
            {
                cs = cs.Caller;
            }

            return cs;
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}
