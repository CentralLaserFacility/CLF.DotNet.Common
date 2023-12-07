//
// DebugHelpers.cs
//

using Clf.Common.ExtensionMethods;

namespace Clf.Common.Utils
{

  public static class DebugHelpers
  { 

    [System.Diagnostics.Conditional("DEBUG")]
    public static void WriteDebugLines ( params string[] lines )
    {
      lines.ForEachItem(
        line => WriteDebugLine(line)
      ) ;
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void WriteDebugLine ( string line )
    {
      System.Diagnostics.Debug.WriteLine(line) ;
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void DontWriteDebugLines ( params string[] lines )
    {
      // Don't write, but can put a breakpoint here ...
    }

    public static void DebugBreak ( )
    {
      System.Diagnostics.Debugger.Break() ;
    }

  }

}
