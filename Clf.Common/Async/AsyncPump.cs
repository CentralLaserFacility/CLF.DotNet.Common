//
// AsyncPump.cs
//

namespace Clf.Common
{

  //
  // Provides a message pump that supports running async methods on the current thread
  // 
  // https://devblogs.microsoft.com/pfxteam/await-synchronizationcontext-and-console-apps
  // https://devblogs.microsoft.com/pfxteam/await-synchronizationcontext-and-console-apps-part-2/
  // https://devblogs.microsoft.com/pfxteam/await-synchronizationcontext-and-console-apps-part-3/
  //
  // https://github.com/danielmarbach/AsyncTransactions/blob/master/AsyncTransactions/AsyncPump.cs
  //

  //
  // Provides a means for running an 'async' method such that
  // all of its continuations will run 'serialized' on the current thread.
  //
  // This can be helpful when executing 'async' methods in a console app,
  // or in a unit test framework that doesn't directly support async methods. 
  //

  //
  // Note that one could use the DispatcherSynchronisationContext from WPF,
  // from WindowsBase.dll. However that's only available on Windows.
  //

  public static partial class AsyncPump
  {

    public static void RunAsyncFunctionReturningTask ( 
      System.Func<System.Threading.Tasks.Task> asyncFunctionToRun 
    ) {
      // var thread = System.Environment.CurrentManagedThreadId ;
      var previousContext = System.Threading.SynchronizationContext.Current ;
      try
      {
        //
        // Establish our new context, and set it as 'Current' onto the current thread.
        // When our 'asyncFunctionToRun' invokes an async method, that method's 'awaits'
        // will see this context as Current, so it can Post that context. 
        //
        var newSingleThreadedContext = new SingleThreadSynchronizationContext() ;
        System.Threading.SynchronizationContext.SetSynchronizationContext(
          newSingleThreadedContext
        ) ;
        //
        // Invoke the function, and configure its 'continuation'
        // to notify the context when it completes, with a
        // call to 'NotifyNoMoreWorkItemsWillArrive'
        //
        System.Threading.Tasks.Task taskToContinue = asyncFunctionToRun() ;
        taskToContinue.ContinueWith(
          continuationAction : _ => newSingleThreadedContext.NotifyNoMoreWorkItemsWillArrive(),
          System.Threading.Tasks.TaskScheduler.Default
        ) ;
        // thread = System.Environment.CurrentManagedThreadId ;
        //
        // Go into a loop where we process the work items that get posted
        // to our work-items queue ; returning when we know there will be
        // no further items posted.
        //
        newSingleThreadedContext.RunProcessingLoopOnCurrentThread() ;
        //
        // We've returned, ie all work items have been processed.
        // Propagate any exception.
        //
        // Hmm, 'TaskAwaiter.GetResult()' is not meant to be used directly in application code.
        // However, it is pretty much equivalent to 'task.Wait()' but with the advantage
        // that if the task has faulted, the Exception will be raised directly rather
        // than being wrapped in an AggregateException.
        //
        // https://stackoverflow.com/questions/17284517/is-task-result-the-same-as-getawaiter-getresult
        //
        // A better name for 'GetResult()' might be 'check task for exceptions', 
        // since it actually doesn't 'get' a result when the Task returns a void result ...
        // For example for Task<int> our 'GetResult' does return an int ...
        // System.Threading.Tasks.Task<int> t = null!;
        // int r = t.GetAwaiter().GetResult() ;
        //
        // thread = System.Environment.CurrentManagedThreadId ;
        taskToContinue.GetAwaiter().GetResult() ;
        // thread = System.Environment.CurrentManagedThreadId ;
      }
      finally 
      { 
        // thread = System.Environment.CurrentManagedThreadId ;
        // Restore the previous context
        System.Threading.SynchronizationContext.SetSynchronizationContext(previousContext) ; 
      }
    }

    //
    // This variant lets us run an 'async void' function. Such a function doesn't return
    // a Task, so we can't set up a 'continuation' that will flag the function's completion.
    // Instead we implement the 'OperationStarted/OperationCompleted' functions.
    //
    // https://devblogs.microsoft.com/pfxteam/await-synchronizationcontext-and-console-apps-part-3/
    //

    public static void RunAsyncVoidAction ( System.Action asyncVoidFunctionToRun )
    {
      var previousContext = System.Threading.SynchronizationContext.Current ;
      try
      {
        var newSingleThreadedContext = new SingleThreadSynchronizationContext(
          doOperationCountTracking : true
        ) ;
        System.Threading.SynchronizationContext.SetSynchronizationContext(
          newSingleThreadedContext
        ) ;
        // We surround the async method invocation with calls to OperationStarted and OperationCompleted,
        // just in case the method is actually just a void method and not an 'async void' method,
        // in which case we need to make sure the operation count is greater than 0 for the duration
        // of the invocation.
        newSingleThreadedContext.OperationStarted() ;
        asyncVoidFunctionToRun() ;
        newSingleThreadedContext.OperationCompleted() ;
        // Our method has returned, but it might have posted work items
        // onto the queue - so let's process all of these unti there are none remaining.
        newSingleThreadedContext.RunProcessingLoopOnCurrentThread() ;
      }
      finally
      {
        System.Threading.SynchronizationContext.SetSynchronizationContext(previousContext) ;
      }
    }    
      
    public static async System.Threading.Tasks.Task Demo ( )
    {

      AsyncPump.RunAsyncFunctionReturningTask(
        async () => {
          int threadIdBefore = System.Environment.CurrentManagedThreadId ;
          await System.Threading.Tasks.Task.Delay(100).ConfigureAwait(
            continueOnCapturedContext : true
          ) ;
          int threadIdAfter = System.Environment.CurrentManagedThreadId ;
        }
      ) ;

      AsyncPump.RunAsyncVoidAction(
        async () => {
          int threadIdBefore = System.Environment.CurrentManagedThreadId ;
          await System.Threading.Tasks.Task.Delay(100).ConfigureAwait(
            continueOnCapturedContext : true
          ) ;
          int threadIdAfter = System.Environment.CurrentManagedThreadId ;
        }
      ) ;

      {
        int threadIdBefore = System.Environment.CurrentManagedThreadId ;
        await System.Threading.Tasks.Task.Delay(100).ConfigureAwait(
          continueOnCapturedContext : true
        ) ;
        int threadIdAfter = System.Environment.CurrentManagedThreadId ;
      }

    }

  }

}

