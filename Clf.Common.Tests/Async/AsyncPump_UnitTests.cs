//
// AsyncPump_UnitTests.cs
//

using Xunit ;
using FluentAssertions ;
using static FluentAssertions.FluentActions ;

using Clf.Common.ExtensionMethods ;
using System.Linq ;

namespace Common_UnitTests
{

  public sealed class AsyncPump_UnitTests
  {

    private readonly Xunit.Abstractions.ITestOutputHelper? m_output ;

    public AsyncPump_UnitTests ( Xunit.Abstractions.ITestOutputHelper? testOutputHelper )
    {
      m_output = testOutputHelper ;
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AsyncPump_ContinueOnCapturedContext_WorksAsExpected ( bool continueOnCapturedContext )
    {
      int testThreadId = System.Environment.CurrentManagedThreadId ;
      m_output?.WriteLine($"Test is running on thread #{testThreadId} with continueOnCapturedContext={continueOnCapturedContext}") ;
      Clf.Common.AsyncPump.RunAsyncFunctionReturningTask(
        async () => {
          int initialThreadId = System.Environment.CurrentManagedThreadId ;
          m_output?.WriteLine($"WORK : initial thread is #{initialThreadId}") ;
          for ( int i = 0 ; i < 5 ; i++ )
          {
            int currentThreadId = System.Environment.CurrentManagedThreadId ;
            m_output?.WriteLine($"WORK iteration {i} : thread is #{currentThreadId}") ;
            await System.Threading.Tasks.Task.Delay(
              100
            ).ConfigureAwait(
              continueOnCapturedContext : continueOnCapturedContext
            ) ;
            int threadIdAfterAwait = System.Environment.CurrentManagedThreadId ;
            m_output?.WriteLine($"WORK iteration {i} : thread after 'await' is #{threadIdAfterAwait}") ;
            if ( continueOnCapturedContext )
            {
              threadIdAfterAwait.Should().Be(initialThreadId) ;
            }
            else
            {
              threadIdAfterAwait.Should().NotBe(initialThreadId) ;
            }
          }
        }
      ) ;
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async void AsyncPump_OnWorkerThread_WorksAsExpected ( bool continueOnCapturedContext )
    {
      int testThreadId = System.Environment.CurrentManagedThreadId ;
      m_output?.WriteLine($"Outer test is running on thread #{testThreadId} with continueOnCapturedContext={continueOnCapturedContext}") ;
      await System.Threading.Tasks.Task.Run(
        () => {
          AsyncPump_ContinueOnCapturedContext_WorksAsExpected(continueOnCapturedContext) ;
        }
      ) ;
    }

    [Fact]
    public void CanInvokeWorkerThreadEventsOnMainThread_WithAwaitForTimerTask ( )
    {
      Clf.Common.AsyncPump.RunAsyncFunctionReturningTask(
        async () => {
          int initialThreadId = System.Environment.CurrentManagedThreadId ;
          m_output?.WriteLine($"Main thread is #{initialThreadId}") ;
          var mainSyncContext = System.Threading.SynchronizationContext.Current ;
          System.Action<int> actionInvokedOnTimer = i => {
            m_output?.WriteLine(
              $"Action was invoked on thread #{System.Environment.CurrentManagedThreadId} with argument {i}"
            ) ;
          } ;
          await System.Threading.Tasks.Task.Run(
            async () => {
               await System.Threading.Tasks.Task.Run(
                 () => {
                   for ( int i = 0 ; i < 4 ; i++ )
                   {
                     System.Threading.Tasks.Task.Delay(
                       100
                     ).ConfigureAwait(
                       continueOnCapturedContext : false
                     ) ;
                     int j = i ;
                     mainSyncContext?.Post(
                       d: (state) => {
                         actionInvokedOnTimer(j) ;
                       },
                       null
                     ) ;
                   }
                 }
              ) ;
            }
          ) ;
          m_output?.WriteLine($"Task running timer loop has completed, now on thread #{System.Environment.CurrentManagedThreadId}") ;
        }
      ) ;
    }

    [Fact]
    public void CanInvokeWorkerThreadEventsOnMainThread_WaitingForFinishedEvent ( )
    {
      Clf.Common.AsyncPump.RunAsyncFunctionReturningTask(
        async () => {
          // This simulates our 'main' routine.
          // We kick off a worker thread that invokes our 'action', marshalled
          // to our main thread. It sends 4 events, then sets a 'finished' flag and exits.
          // Our main thread waits for the 'finished' flag.
          int mainThreadId = System.Environment.CurrentManagedThreadId ;
          m_output?.WriteLine($"Main thread is #{mainThreadId}") ;
          var mainSyncContext = System.Threading.SynchronizationContext.Current ;
          System.Action<int,int> actionInvokedOnTimer = (i,workerThreadId) => {
            m_output?.WriteLine(
              $"Action was invoked on thread #{System.Environment.CurrentManagedThreadId} with argument {i}, from worker thread #{workerThreadId}"
            ) ;
            System.Environment.CurrentManagedThreadId.Should().Be(mainThreadId) ;
          } ;
          Clf.Common.AsyncManualResetEvent finished = new() ;
          var workerTask_runningTimer = System.Threading.Tasks.Task.Run(
            async () => {
               await System.Threading.Tasks.Task.Run(
                 () => {
                   // This loop will run on a worker thread
                   for ( int i = 0 ; i < 4 ; i++ )
                   {
                     System.Threading.Tasks.Task.Delay(
                       100
                     ).ConfigureAwait(
                       continueOnCapturedContext : false
                     ) ;
                     // Must make local copies of the arguments we're going
                     // to send to the recipient, because of the way
                     // lambda capture works ...
                     int i_copy = i ;
                     int workerThreadId = System.Environment.CurrentManagedThreadId ;
                     mainSyncContext?.Post(
                       d: (state) => {
                         actionInvokedOnTimer(
                           i_copy,
                           workerThreadId
                         ) ;
                       },
                       state : null
                     ) ;
                   }
                   finished.TrySet() ;
                 }
              ) ;
            }
          ) ;
          m_output?.WriteLine($"Waiting for 'finished' event, now on thread #{System.Environment.CurrentManagedThreadId}") ;
          await finished.Task ;
          m_output?.WriteLine($"Finished, now on thread #{System.Environment.CurrentManagedThreadId}") ;
          workerTask_runningTimer.IsCompleted.Should().BeTrue() ;
        }
      ) ;
    }

  }

}