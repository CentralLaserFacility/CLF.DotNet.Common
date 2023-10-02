//
// Object_T_ExtensionMethods.cs
//

using System.Diagnostics.CodeAnalysis;
using FluentAssertions ;

namespace Clf.Common.ExtensionMethods
{

  public static partial class Object_T_ExtensionMethods
  {

    // We'll avoid using 'As<>' since this conflicts with
    // an extension method defined in FluentAssertions ...
    // and we really need to have 'using FluentAssertions'

    public static T TreatedAs<T> ( this object x )
    where T : class
    => (T) x ;

    public static T Verified<T> ( this T value, System.Action<T> verify  )
    {
      verify(value) ;
      return value ;
    }

    public static T Verified<T> ( this T value, System.Func<T,bool> verify  )
    {
      if ( verify(value) == false )
      {
        throw new System.ApplicationException("Verification failed") ;
      }
      return value ;
    }

    public static T VerifiedAsNonNullInstanceOf<T> ( this object? x )
    {
      if ( x is T value )
      {
        return value ;
      }
      else
      {
        throw new System.ApplicationException(
          $"Value is null or not of type {typeof(T).Name}"
        ) ;
      }
    }

    public static T VerifiedAsNonNullInstance<T> (
      this T? x,
      string? exceptionMessage = null
    )
    where T : class
    {
      if ( x is null )
      {
        throw new System.ApplicationException(
          exceptionMessage ?? $"Instance is unexpectedly null"
        ) ;
      }
      else
      {
        return x ;
      }
    }

    public static T VerifiedAsNonNullValue<T> (
      this T? x,
      string? exceptionMessage = null
    )
    where T : struct
    {
      if ( x is null )
      {
        throw new System.ApplicationException(
          exceptionMessage ?? $"Value is unexpectedly null"
        ) ;
      }
      else
      {
        return x.Value ;
      }
    }

    public static T? VerifiedSameInstanceAs<T> (
      this T? x,
      T?      other
    )
    where T : class
    {
      if ( ReferenceEquals(x,other) is false )
      {
        throw new System.ApplicationException(
          $"Values were expected to be the same"
        ) ;
      }
      return x ;
    }

    public static T VerifiedEqualTo<T> (
      this T x,
      T      other
    )
    where T : System.IEquatable<T>
    {
      if ( x.Equals(other) is false )
      {
        throw new System.ApplicationException(
          $"Values were expected to be the same"
        ) ;
      }
      return x ;
    }

    public static T? VerifiedSameAs<T> (
      this T? x,
      T?      other
    )
    where T : class
    {
      if ( ReferenceEquals(x,other) is false )
      {
        throw new System.ApplicationException(
          $"Values were expected to be the same"
        ) ;
      }
      return x ;
    }

    public static bool ValueIs<T> ( this object? x, T expectedValue ) where T : System.IEquatable<T>
    {
      if ( x is T value )
      {
        return value.Equals(expectedValue) ;
      }
      else
      {
        return false ;
      }
    }

    public static T Plinged<T> ( this T? item ) where T : class 
    => item.VerifiedAsNonNullInstance() ;

    public static T Plinged<T> ( this T? item ) where T : struct 
    => (
      item.HasValue
      ? item.Value
      : throw new System.ApplicationException("Unexpected null value")
    ) ;

    // [return:NotNull]
    // public static T PlingedX ( [MaybeNull] this T item ) // where T : class 
    // => (
    //   item is System.Nullable nullable
    //   ? ( nullable.)
    //   .VerifiedAsNonNullInstance() ;

    public static System.ApplicationException AsUnexpectedValueException<T> ( 
      this T value 
    ) {
      return new System.ApplicationException(
        $"Unexpected value {value}"
      ) ;
    }

    public static System.IntPtr VerifiedAsNonNullPointer ( this System.IntPtr p )
    {
      p.Should().NotBe(System.IntPtr.Zero) ;
      return p ;
    }

  }

}
