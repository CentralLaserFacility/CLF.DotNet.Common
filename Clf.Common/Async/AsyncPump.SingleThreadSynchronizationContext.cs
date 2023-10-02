//
// AsyncPump.SingleThreadSynchronizationContext.cs
//

using System.Collections.Generic ;

namespace Clf.Common
{

  public static partial class AsyncPump
  {

    //
    // https://devblogs.microsoft.com/pfxteam/await-synchronizationcontext-and-console-apps
    // https://devblogs.microsoft.com/pfxteam/await-synchronizationcontext-and-console-apps-part-2/
    // https://devblogs.microsoft.com/pfxteam/await-synchronizationcontext-and-console-apps-part-3/
    //

    //
    // Our asynchronous method will be 'producing' work items ('actions to be executed'),
    // and our pumping loop will be 'consuming' them from the queue and executing them.
    //
    // We have ...
    // - a 'Post' method that adds work items to the queue
    // - a method that sits in a consuming loop, removing each work item and processing it
    // - a method that lets the queue know that no more work will arrive,
    //   allowing the consuming loop to exit once the queue is empty.
    //

    public sealed class SingleThreadSynchronizationContext : System.Threading.SynchronizationContext
    {

      //
      // Used by the 'AsyncPump' class
      //

      //
      // This queue stores the work items that are yet to be processed.
      //
      // BlockingCollection<T> encapsulates not only a queue, but also the synchronization necessary
      // to coordinate between a producer adding elements to that queue and a consumer removing them,
      // including blocking the consumer attempting a removal while the queue is empty.
      //

      //
      // TODO : Better to use a 'System.Threading.Channels.Channel' here ?
      //

      //
      // TODO : implement the 'Send' API
      //        use a subclass to provide the 'async void' variant,
      //        instead of the constructor flag
      //

      private readonly System.Collections.Concurrent.BlockingCollection<
        KeyValuePair<System.Threading.SendOrPostCallback,object?>
      > m_queueOfWorkItems ;

      private readonly System.Threading.Thread m_processingThread ;

      public SingleThreadSynchronizationContext ( bool doOperationCountTracking = false ) : 
      this(
        doOperationCountTracking,
        new System.Collections.Concurrent.BlockingCollection<
          KeyValuePair<System.Threading.SendOrPostCallback,object?>
        >(), 
        System.Threading.Thread.CurrentThread
      ) {
      }

      private SingleThreadSynchronizationContext (
        bool doOperationCountTracking, 
        System.Collections.Concurrent.BlockingCollection<
          KeyValuePair<System.Threading.SendOrPostCallback,object?>
        > queue, 
        System.Threading.Thread currentThread
      ) {
        m_doOperationCountTracking = doOperationCountTracking ;
        m_queueOfWorkItems         = queue ;
        m_processingThread         = currentThread ;
      }

      //
      // Dispatch a message to the synchronization context.
      //
      // The work provided to the Post method comes in the form of two objects :
      // 1. a SendOrPostCallback delegate that represents the work to be performed
      // 2. a 'state' object to be passed into that delegate when it's invoked
      //

      public override void Post ( System.Threading.SendOrPostCallback actionToExecute, object? state )
      {
        var thread = System.Environment.CurrentManagedThreadId ;
        m_queueOfWorkItems.Add(
          new(actionToExecute,state)
        ) ;
      }

      //
      // A 'Send' would block the calling thread
      // until the execution of the action has completed. 
      //

      public override void Send ( System.Threading.SendOrPostCallback actionToExecute, object? state )
      {
        throw new System.NotSupportedException("Synchronous 'send' is not supported") ;
      }

      public override System.Threading.SynchronizationContext CreateCopy ( )
      {
        // Necessary, as the default implementation
        // returns an empty SynchronizationContext object ...
        return new SingleThreadSynchronizationContext(
          m_doOperationCountTracking,
          m_queueOfWorkItems, 
          m_processingThread
        ) ;
      }

      public bool IsRunningProcessingLoop { get ; private set ; }

      public int HowManyWorkItemsProcessed { get ; private set ; } = 0 ;

      // Run a loop to process all the queued work items

      public void RunProcessingLoopOnCurrentThread ( )
      {
        var thread = System.Environment.CurrentManagedThreadId ;
        IsRunningProcessingLoop = true ;
        while ( 
          m_queueOfWorkItems.TryTake(
            out var workItem, 
            System.Threading.Timeout.Infinite
          )
        ) {
          try
          {
            thread = System.Environment.CurrentManagedThreadId ;
            workItem.Key(workItem.Value) ;    
            thread = System.Environment.CurrentManagedThreadId ;
          }
          catch ( System.Exception x )
          {
            x.ToString(); //TODO: Handle exception in Log... suppressing warning
          }
          HowManyWorkItemsProcessed++ ;
        }
        thread = System.Environment.CurrentManagedThreadId ;
        IsRunningProcessingLoop = false ;
      }

      // Notifies the context that no more work will arrive

      public void NotifyNoMoreWorkItemsWillArrive ( ) 
      { 
        m_queueOfWorkItems.CompleteAdding() ; 
      }

      //
      // Additional code to support 'async void' functions.
      //
      // We react appropriately to calls to 'OperationStarted' and 'OperationCompleted',
      // maintaining a count of how many outstanding operations there are, such that
      // when the count reaches 0, we call Complete, just as before we called Complete
      // when the async method’s Task completed.
      //

      private readonly bool m_doOperationCountTracking = false ;

      private int m_operationCount = 0 ;

      public override void OperationStarted ( )
      {
        if ( m_doOperationCountTracking )
        {
          System.Threading.Interlocked.Increment(
            ref m_operationCount
          ) ;
        }
      }

      public override void OperationCompleted ( )
      {
        if ( m_doOperationCountTracking )
        {
          if ( System.Threading.Interlocked.Decrement( ref m_operationCount ) == 0 )
          {
            // The count has gone to zero,
            // so there'll be no further work to do
            NotifyNoMoreWorkItemsWillArrive() ;
          }
        }
      }
      
    }

  }

}
