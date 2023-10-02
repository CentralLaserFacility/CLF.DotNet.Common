//
// AsyncManualResetEvent.cs
//

using FluentAssertions;
using System.Threading ;
using System.Threading.Tasks ;

namespace Clf.Common
{

  //
  // Based on a blog post from Stephen Taub :
  // https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-1-asyncmanualresetevent/
  //
  // This is basically a 'TaskCompletionSource' with a 'reset' capability.
  //

  //
  // IDEA : useful to be able to know how long we were waiting ???
  //

  public sealed class AsyncManualResetEvent
  {

    // This non-generic constructor is available in Net6.0 but not in NetStandard2.0

    private volatile TaskCompletionSource m_taskCompletionSource = new(
      TaskCreationOptions.RunContinuationsAsynchronously // ???????????? CHECK THIS ...
    ) ;

    public AsyncManualResetEvent ( bool initiallySet = false )
    {
      if ( initiallySet )
      {
        Set() ;
      }
    }

    //
    // Returning our 'Task' as a property encourages the following usage
    // which lets us specify a timeout and/or a cancellation token
    // via the Task.WaitAsync() overloads available in .Net 6
    //
    //   await myEvent.TaskRepresentingEventHasFired.WaitAsync(
    //     System.TimeSpan.FromMilliseconds(1000)
    //   ) ;
    //
    //   public Task WaitAsync ( System.TimeSpan timeout ) => m_tcs.Task.WaitAsync(timeout) ;
    //   
    //   public Task WaitAsync ( CancellationToken token ) => m_tcs.Task.WaitAsync(token) ;
    //   
    //   public Task WaitAsync ( System.TimeSpan timeout, CancellationToken token )
    //   => m_tcs.Task.WaitAsync(timeout,token) ;
    //   

    public Task Task => m_taskCompletionSource.Task ;

    public void Set ( ) => m_taskCompletionSource.SetResult() ;

    public bool TrySet ( ) => m_taskCompletionSource.TrySetResult() ;

    public bool IsSet => m_taskCompletionSource.Task.IsCompleted ;

    public bool IsNotSet => IsSet is false ;

    public void Reset ( )
    {
      while ( true )
      {
        var tcs_current = m_taskCompletionSource ;
        if (
           ! tcs_current.Task.IsCompleted 
        || Interlocked.CompareExchange(
             ref m_taskCompletionSource, 
             new TaskCompletionSource(), 
             tcs_current
           ) == tcs_current
        ) {
          IsSet.Should().BeFalse() ;
          return ;
        }
      }
    }  
    
  }

}
