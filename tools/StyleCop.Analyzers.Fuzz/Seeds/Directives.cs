#define ENABLED
using System;

partial class Directives
{
#region Members
    public int x;
#if ENABLED
    public void M(){ x++; }
#else
    broken syntax in disabled text
#endif
#endregion
}
