using System;

namespace ExampleProject;

public class B
{
    private int _filler1;
    private C _nullObject;

    public void AccessNullField()
    {
        Console.WriteLine(_nullObject.Value);
    }
}