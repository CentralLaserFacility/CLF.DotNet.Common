//
// Log.cs
//

using Clf.Common.ExtensionMethods;
using System.Collections.Generic;

namespace Clf.Common.Utils
{

  //
  // Actually the 'settings' merely affect the way the generated messages will be
  // handled, and this is always done in the Platform-specific handler.
  //
  // Not all of these flags are relevant on every platform !
  //

  public sealed class LogMessageAcceptor
  {

    public System.Action<string> AcceptMessage = delegate { } ;
    // Once this is defined, we can use '+=' syntax
    // eg myMessageAcceptor += "message" ;
    public static LogMessageAcceptor operator + ( LogMessageAcceptor self, string s )
    {
      Clf.Common.Utils.Log.Message(s) ;
      return self ;
    }
  }

  public static partial class Log
  {

    public static System.Action<string>? WriteLineAction { get ; set ; }

    public static void Message ( string text ) => WriteLine(text) ;

    public static void WriteLine ( string text )
    {
      // WriteLineAction?.Invoke(text) ?? System.Diagnostics.Debug.WriteLine(text) ;
      if ( WriteLineAction != null )
      {
        WriteLineAction(text) ;
      }
      else
      {
        System.Diagnostics.Debug.WriteLine(text) ;
      }
    }

    public static void WriteLines ( params string[] textLines )
    {
      WriteLines(textLines) ;
    }

    public static void WriteLines ( IEnumerable<string> textLines )
    {
      textLines.ForEachItem(
        (line) => WriteLine(line)
      ) ;
    }

  }

}
