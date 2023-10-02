//
// AsyncManualResetEvent_T.cs
//

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;

namespace Clf.Common
{

  //
  // Based on a blog post from Stephen Taub :
  // https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-1-asyncmanualresetevent/
  //

  public sealed class AsyncManualResetEvent<TResult>
  {

    private volatile TaskCompletionSource<TResult> m_taskCompletionSource = new(
      TaskCreationOptions.RunContinuationsAsynchronously
    ) ;

    public AsyncManualResetEvent ( )
    { }

    public Task<TResult> Task => m_taskCompletionSource.Task ;

    public void Set ( TResult result ) => m_taskCompletionSource.SetResult(result) ;

    public bool TrySet ( TResult result ) => m_taskCompletionSource.TrySetResult(result) ;

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
             new TaskCompletionSource<TResult>(), 
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
