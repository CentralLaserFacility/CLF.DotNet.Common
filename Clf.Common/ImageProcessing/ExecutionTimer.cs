//
// ExecutionTimer.cs
//

namespace Clf.Common.ImageProcessing
{

  // TODO : move this into Clf.Common ...

  public class ExecutionTimer
  {

    private System.Diagnostics.Stopwatch m_stopwatch ;

    private static readonly double HowManyTicksPerSecond = System.Diagnostics.Stopwatch.Frequency ;

    public ExecutionTimer ( bool startImmediately )
    {
      m_stopwatch = (
        startImmediately 
        ? System.Diagnostics.Stopwatch.StartNew()
        : new System.Diagnostics.Stopwatch()
      ) ;
      ElapsedTime_DivisionFactor = 1 ;
    }

    public int ElapsedTime_DivisionFactor { get ; set ; }

    public static ExecutionTimer StartNew ( ) => new ExecutionTimer(
      startImmediately : true
    ) ;

    public void Start ( ) => m_stopwatch.Start() ;

    public bool IsRunning => m_stopwatch.IsRunning ;

    public void Stop ( ) => m_stopwatch.Stop() ;

    public void Stop_ConfiguringElapsedTimeDivisionFactor ( int elapsedTimeDivisionFactor )
    {
      m_stopwatch.Stop() ;
      ElapsedTime_DivisionFactor = elapsedTimeDivisionFactor ;
    }

    public void Reset ( ) => m_stopwatch.Reset() ;

    public void Restart ( )
    {
      m_stopwatch.Reset() ;
      m_stopwatch.Start() ;
    }

    private double m_elapsedTimeInSecsWhenLastQueryMade = 0.0 ;

    public bool HasAdvancedSinceLastQueryByAtLeast ( double nSecs )
    {
      double elapsedTimeInSecs = m_stopwatch.ElapsedTicks / HowManyTicksPerSecond ;
      if ( elapsedTimeInSecs >= m_elapsedTimeInSecsWhenLastQueryMade + nSecs )
      {
        m_elapsedTimeInSecsWhenLastQueryMade = elapsedTimeInSecs ;
        return true ;
      }
      else
      {
        return false ;
      }
    }

    public System.TimeSpan ElapsedTime => System.TimeSpan.FromSeconds(
      ElapsedTime_InSecs
    ) ;

    public double ElapsedTime_InSecs => (
      m_stopwatch.ElapsedTicks 
    / ( HowManyTicksPerSecond * ElapsedTime_DivisionFactor )
    ) ;

    public double ElapsedTime_InMilliSecs => ElapsedTime_InSecs * 1.0E3 ;

    public double ElapsedTime_InMicroSecs => ElapsedTime_InSecs * 1.0E6 ;

    public string ElapsedTime_InSecs_AsString => (
      // 1.234 secs
      ElapsedTime_InSecs.ToString("F3") // + " secs" ;
    ) ;

    public string ElapsedTime_InMinutes_AsString => (
      (
        ElapsedTime_InSecs / 60.0
      ).ToString("F1") // + " minutes" ;
    ) ;

    public string ElapsedTime_InMinutesOrSecs_AsString => (
      ElapsedTime_InSecs < 100.0
      ? ElapsedTime_InSecs_AsString + " seconds"
      : ElapsedTime_InMinutes_AsString + " minutes"
    ) ;

    public string ElapsedTime_InMilliSecs_AsString
    => string.Format(
      "{0:F3}mS",
      ElapsedTime_InMilliSecs
    ) ;

    public string ElapsedTime_InMicroSecs_AsString
    => string.Format(
      "{0:F3}uS",
      ElapsedTime_InMicroSecs
    ) ;

    public override string ToString ( ) => ElapsedTime_InSecs_AsString ;

    public string AsString => this.ToString() ;

  }

  public class ExecutionTimer_StartingImmediately : ExecutionTimer
  {
    public ExecutionTimer_StartingImmediately ( ) :
    base(startImmediately : true)
    { }
  }

  public class ExecutionTimer_WaitingForStartCommand : ExecutionTimer
  {
    public ExecutionTimer_WaitingForStartCommand ( ) :
    base(startImmediately:false)
    { }
  }

  public class ExecutionTimer_MeasuringTimeElapsed : ExecutionTimer_StartingImmediately, System.IDisposable
  {
    private System.Action<System.TimeSpan> m_completed ;
    public ExecutionTimer_MeasuringTimeElapsed ( System.Action<System.TimeSpan> completed )
    {
      m_completed = completed ;
    }
    public void Dispose ( )
    {
      m_completed(
        ElapsedTime
      ) ;
    }
  }

  public class ExecutionTimer_ShowingMillisecsElapsed : ExecutionTimer_StartingImmediately, System.IDisposable
  {
    private readonly string                m_operationBeingTimed ;
    private readonly System.Action<string> m_handleCompletionMessage ;
    public ExecutionTimer_ShowingMillisecsElapsed ( 
      string                operationBeingTimed,
      System.Action<string> handleCompletionMessage
    ) {
      m_operationBeingTimed     = operationBeingTimed ;
      m_handleCompletionMessage = handleCompletionMessage ;
    }
    public void Dispose ( )
    {
      string message = $"{m_operationBeingTimed} : {ElapsedTime.TotalMilliseconds:F2}mS" ;
      m_handleCompletionMessage(
        message
      ) ;
    }
  }

  public class ExecutionTimerEx_ShowingMillisecsElapsed : System.IDisposable
  {
    private string                         m_message ;
    private readonly System.Action<string> m_handleCompletionMessage ;
    public ExecutionTimerEx_ShowingMillisecsElapsed ( 
      string                operationBeingTimed,
      System.Action         actionToExecute,
      System.Action<string> handleCompletionMessage,
      int                   nRepeats                         = 10,
      bool                  runActionOnceBeforeTestsToWarmUp = true
    ) {
      m_handleCompletionMessage = handleCompletionMessage ;
      m_message                 = operationBeingTimed + " : " ;
      if ( runActionOnceBeforeTestsToWarmUp )
      {
        actionToExecute() ;
      }
      double aggregatedTime = 0.0 ;
      for ( int i = 0 ; i < nRepeats ; i++ ) 
      {
        using ( 
          var timer = new ExecutionTimer_MeasuringTimeElapsed(
            timeElapsed => {
              m_message += $" {timeElapsed.TotalMilliseconds:F2}" ;
              aggregatedTime += timeElapsed.TotalMilliseconds ;
            }
          ) 
        ) {
          actionToExecute() ;
        }
      }
      m_message += $" (mS) ; average ({nRepeats}) is {aggregatedTime/nRepeats:F2}" ;
    }
    public void Dispose ( )
    {
      m_handleCompletionMessage(
        m_message
      ) ;
    }
  }

}

