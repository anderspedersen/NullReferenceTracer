using System;
using System.Runtime.CompilerServices;

namespace ExampleProject;

public class A
{
    private readonly string _filler1 = "Hello";
    private int _filler2 = 2;
    private readonly int[] _filler3 = Array.Empty<int>();
    private readonly B _b = new();

    [MethodImpl(MethodImplOptions.NoInlining)]
    public int Run(bool throwNullReferenceException)
    {
        if (throwNullReferenceException)
        {
            _filler2 += 3;
        }
        else
        {
            _filler2 += 5;
        }

        foreach (var fill in _filler3)
        {
            Console.WriteLine(fill);
        }

        if (throwNullReferenceException)
        {
            _b.AccessNullField();
        }

        return _filler2;
    }
}