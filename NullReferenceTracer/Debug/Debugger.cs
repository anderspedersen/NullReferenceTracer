using System;
using System.Linq;
using Microsoft.Diagnostics.Runtime;

namespace NullReferenceTracer.Debug
{
    public sealed class Debugger : IDisposable
    {
        private readonly DataTarget _dataTarget;

        public Debugger(int processId)
        {
            _dataTarget = DataTarget.AttachToProcess(processId, true);
        }

        public MethodInfo GetMethodInfo(ulong instructionPointer)
        {
            var method = MethodInfo.Create(instructionPointer, _dataTarget);
            return method;
        }

        public void Dispose()
        {
            _dataTarget.Dispose();
        }
    }
}
