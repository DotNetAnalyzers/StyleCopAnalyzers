using System;
namespace Example
{
    // keep-before-type
    class Trivia
    {
        int b; // keep-after-field
        /* keep-before-method */ void M()
        {
            if (b > 0) // keep-after-condition
            {
                b++; /* keep-after-statement */
            }
        }
        // keep-before-property
        public int A { get; set; }
    }
}
