//
// Assert.cs
//

using Clf.Common.ExtensionMethods;

namespace Clf.Common.Utils
{

  public static class Assert
  {

    public static event System.Action? AssertFailed_Event ;

    // By default, ie unless enabled in the 'App', assertion failures do nothing.
    // This is because we need to prevent assertion failures from causing Visual Studio
    // to crash when we're rebuilding a file that is open in the Designer, for example ...
    // SO - WE NEED TO EXPLICITLY ACTIVATE ASSERTIONS IN THE MAIN APP !!!

    public static bool AssertionsActive       { get ; set ; } = true ;

    public static bool LogSucessfulAssertions { get ; set ; } = false ;

    public static void IsTrue ( bool x, string? optionalMessage = null )
    {
      if ( x == false )
      {
         if (
           ! AssertionsActive
         ) {
           // We occasionally get assertion failures when the project
           // is being built, if the Designer happens to be open ...
           // causes Visual Studio to crash, not useful !!
           System.Diagnostics.Debug.WriteLine(
             "ASSERT failed, but Assertions are not active !!"
           ) ;
           return ;
         }
         AssertFailed_Event?.Invoke() ;
          bool throwException = true ;
          // Can set a breakpoint here, and clear the
          // 'throwException' flag manually if we think
          // it's actually safe to carry on ...
          if ( throwException )
          {
            if ( optionalMessage.IsNullOrEmpty() )
              throw new System.ApplicationException("Assertion failed") ;
            else
              throw new System.ApplicationException($"Assertion failed : {optionalMessage}") ;
          }
      }
      else if ( LogSucessfulAssertions )
      {
        System.Diagnostics.Debug.WriteLine("Assertion succeeded") ;
      }
    }

  }

}

