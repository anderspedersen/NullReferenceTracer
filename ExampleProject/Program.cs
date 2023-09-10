using System;
using System.Threading;
using ExampleProject;

try
{
    var a = new A();
    
    // Make sure method is tier-1 compiled, so we get optimized machine code
    for (var i = 0; i < 1_000_000_000; i++)
    {
        a.Run(throwNullReferenceException: false);
    }
    
    a.Run(throwNullReferenceException: true);
}
catch (Exception e)
{
    Console.WriteLine(e);
    Console.ReadLine();
}
