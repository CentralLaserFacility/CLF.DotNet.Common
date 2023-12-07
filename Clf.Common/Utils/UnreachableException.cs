//
// UnreachableException.cs
//

namespace Clf.Common.Utils
{
  public class UnreachableException : System.ApplicationException 
  {
    public UnreachableException ( string message = "" ) : 
    base( 
      message
    ) {
    } 
  }

}

