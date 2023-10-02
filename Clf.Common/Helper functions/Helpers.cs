//
// Helpers.cs
//

namespace Clf.Common
{

  public static partial class Helpers
  {

    // Note that if T is a reference type,
    // all the elements will be set to refer
    // to the instance passed in as 'valueForAllElements' ...
    // Hmm, is this really what we want ? YES !!!

    public static T[] CreateArrayOfObjects<T> ( 
      int nElements, 
      T   valueForAllElements = default(T)! 
    ) {
      var array = (T[]) System.Array.CreateInstance(
        typeof(T),
        nElements
      ) ;
      System.Array.Fill(
        array,
        valueForAllElements
      ) ;
      return array ;
      // object CreateArrayOfObjects ( object valueForAllElements )
      // {
      //   var array = System.Array.CreateInstance(
      //     typeof(T),
      //     nElements
      //   ) ;
      //   // Can't use 'System.Array.Fill' because
      //   // that API expects the type T to be known ...
      //   //   System.Array.Fill(
      //   //     array,
      //   //     valueForAllElements
      //   //   ) ;
      //   for ( int i = 0 ; i < nElements ; i++ )
      //   {
      //     array.SetValue(
      //       value : valueForAllElements,
      //       index : i
      //     ) ;
      //   }
      //   return array ;
      // }

    }

    // Note that if the object is a reference type,
    // all the elements will be set to refer
    // to the instance passed in ...
    // Hmm, is this really what we want ?

    public static System.Array CreateArrayOfObjects ( 
      int         nElements, 
      System.Type typeOfObject, 
      object?     valueForAllElements 
    ) {
      var array = System.Array.CreateInstance(
        typeOfObject,
        nElements
      ) ;
      // Hmm, is there a more efficient way to do this ??
      // TODO : HOW DOES THIS PLAY WITH VALUE TYPES ???
      for ( int i = 0 ; i < nElements ; i++ )
      {
        array.SetValue(
          value : valueForAllElements,
          index : i
        ) ;
      }
      return array ;
    }

    //
    // Create an array that is an 'expanded' version of the original,
    // where the elements of the new array are filled in by visiting 
    // the 'original' elements in a cyclic sequence.
    //
    // [ 1 2 3 ] => [ 1 2 3  1 2 3  1 2 ]
    //

    public static System.Array CreateExpandedArrayOfObjects ( 
      System.Array originalArray,
      int          nExpandedElements 
    ) {
      System.Type typeOfObject = originalArray.GetType().GetElementType()! ;
      var expandedArray = System.Array.CreateInstance(
        typeOfObject,
        nExpandedElements
      ) ;
      int iSource = 0 ;
      int nSourceElements = originalArray.Length ;
      if ( nSourceElements == 1 )
      {
        object valueForAllElements = originalArray.GetValue(0)! ;
        for ( int i = 0 ; i < nExpandedElements ; i++ )
        {
          expandedArray.SetValue(
            value : valueForAllElements,
            index : i
          ) ;
        }
      }
      else
      {
        for ( int iTarget = 0 ; iTarget < nExpandedElements ; iTarget++ )
        {
          object sourceElement = originalArray.GetValue( 
            iSource++ % nSourceElements 
          )! ;
          expandedArray.SetValue(
            value : sourceElement,
            index : iTarget
          ) ;
        }
      }
      return expandedArray ;
    }

  }

}
