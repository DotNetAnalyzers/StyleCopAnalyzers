using System.Collections.Generic;
namespace Example;

record Modern(int Value)
{
    public int[] Values => [1,2,3];
    public int M(object value) => value switch
    {
        string { Length: > 0 } text => text.Length,
        int number => number,
        _ => 0
    };
}
