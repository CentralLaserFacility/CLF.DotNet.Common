//
// ExtensionMethods.cs
//

using System.Collections.Generic ;
using System.Diagnostics.CodeAnalysis ;
using System.Linq ;

namespace Clf.Common.ExtensionMethods
{

  public static class IEnumerable_ExtensionMethods
  {

    public static bool IsEmpty<T> ( this IEnumerable<T>? sequence )
    => sequence?.Any() == false ;

    public static void ForEachItem<T> ( this IEnumerable<T> sequence, System.Action<T> action )
    {
      foreach ( var item in sequence )
      {
        action(item) ;
      }
    }

    public static void ForEachItem<T> ( this IEnumerable<T> sequence, System.Action<T,int> action )
    {
      int j = 0 ;
      foreach ( var item in sequence )
      {
        action(item,j++) ;
      }
    }

  }

}
