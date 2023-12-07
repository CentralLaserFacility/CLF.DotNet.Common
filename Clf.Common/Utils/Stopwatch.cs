//
// Stopwatch.cs
//

namespace Clf.Common.Utils
{
  public class Stopwatch : InvokesActionOnDispose
  { 
    public Stopwatch ( [System.Runtime.CompilerServices.CallerMemberName] string? message = null, System.Action<int>? onDisposeAction = null )
    {
      DebugHelpers.WriteDebugLines(
        $"{message} ..."
      ) ;
      System.Diagnostics.Stopwatch stopwatch = new() ;
      stopwatch.Start ();
      onDisposeAction ??= (elapsedMilliseconds) => {
        DebugHelpers.WriteDebugLines(
          $"{message} took {elapsedMilliseconds} millisecs"
        ) ;
      } ;
      base.DisposeAction = () => {
        stopwatch.Stop() ;
        int elapsedMilliseconds = (int) stopwatch.ElapsedMilliseconds ;
        DebugHelpers.WriteDebugLines(
          $"{message} took {elapsedMilliseconds} millisecs"
        ) ;
        onDisposeAction?.Invoke(elapsedMilliseconds) ;
      } ;
      stopwatch.Start() ;
    }

  }

}
