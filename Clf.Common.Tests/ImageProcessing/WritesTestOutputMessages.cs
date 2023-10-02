//
// WritesTestOutputMessages.cs
//

using System.Collections.Generic ;
using System.Linq ;

namespace ImageProcessing_Tests
{

  // Move this to Clf.Common.TestHelpers ?

  public abstract class WritesTestOutputMessages : System.IDisposable
  {

    private readonly Xunit.Abstractions.ITestOutputHelper m_outputHelper ;

    private static List<string> g_testResults = new() ;

    protected void WriteMessage ( string message ) 
    {
      m_outputHelper.WriteLine(message) ;
      g_testResults.Add(message) ;
    }

    protected WritesTestOutputMessages ( Xunit.Abstractions.ITestOutputHelper outputHelper )
    {
      m_outputHelper = outputHelper ;
    }

    public virtual void Dispose ( )
    {
      if ( g_testResults.Any() )
      {
        var resultSummary = string.Join(
          "\r\n",
          g_testResults
        ) ;
        string testName = this.GetType().Name ;
        #if DEBUG
          string build = "DEBUG" ;
        #else
          string build = "RELEASE" ;
        #endif
        try
        {
          System.IO.File.WriteAllText(
            $@"C:\temp\{build}_{testName}.txt",
            resultSummary
          ) ;
        }
        catch
        { 
          // In case C:\temp doesn't exist ...
        }
      }
    }

  }

}

