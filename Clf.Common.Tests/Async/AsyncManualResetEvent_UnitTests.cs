//
// AsyncManualResetEvent_StephenTaub_UnitTests.cs
//

using System ;
using System.Threading.Tasks ;
using System.Linq ;
using System.Threading ;
using System.Diagnostics.CodeAnalysis ;
using Xunit ;

namespace Async_UnitTests
{

  public class AsyncManualResetEvent_UnitTests
  {

    //
    // Attempts to ensure that a task never completes.
    // If the task takes a long time to complete, this method may not detect that it (incorrectly) completes.
    //

    public static async Task AssertNeverCompletesAsync ( Task task, int timeout = 500 )
    {
      _ = task ?? throw new ArgumentNullException(nameof(task)) ;

      // Wait for the task to complete, or the timeout to fire.
      var completedTask = await Task.WhenAny(
        task, 
        Task.Delay(timeout)
      ).ConfigureAwait(false) ;
      if ( completedTask == task )
      {
        throw new Exception("Task completed unexpectedly.") ;
      }
      // If the task didn't complete, attach a continuation that will
      // raise an exception on a random thread pool thread
      // if it ever does complete ...
      try
      {
        throw new Exception("Task completed unexpectedly.") ;
      }
      catch  (Exception ex )
      {
        var info = System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex) ; 
        var __ = task.ContinueWith(
          _ => info.Throw(), 
          TaskScheduler.Default
        ) ;
      }
    }

    [Fact]
    public async Task WaitAsync_Unset_IsNotCompleted ( )
    {
      var manualResetEvent = new Clf.Common.AsyncManualResetEvent() ;

      var task = manualResetEvent.Task ;

      await AssertNeverCompletesAsync(task) ;
    }

    [Fact]
    public async Task Wait_Unset_IsNotCompleted ( )
    {
      var manualResetEvent = new Clf.Common.AsyncManualResetEvent() ;

      var task = Task.Run(
        () => manualResetEvent.Task.Wait()
      ) ;

      await AssertNeverCompletesAsync(task) ;
    }

    [Fact]
    public async void WaitAsync_AfterSet_IsCompleted ( )
    {
      var manualResetEvent = new Clf.Common.AsyncManualResetEvent() ;

      manualResetEvent.TrySet() ;
      await manualResetEvent.Task ;

      Assert.True(manualResetEvent.Task.IsCompleted) ;
    }

    [Fact]
    public async Task WaitAsync_AfterReset_IsNotCompleted ( )
    {
      var manualResetEvent = new Clf.Common.AsyncManualResetEvent() ;

      manualResetEvent.TrySet() ;
      manualResetEvent.Reset() ;
      await AssertNeverCompletesAsync(manualResetEvent.Task) ;
    }

  }

}
