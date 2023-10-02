//
// Object_ExtensionMethods.cs
//

using System.Collections.Generic ;
using System.Diagnostics.CodeAnalysis ;
using System.Linq ;

namespace Clf.Common.ExtensionMethods
{

  public static partial class Object_ExtensionMethods
  {

    public static string ClassName ( this object x )
    => (
      x?.GetType().GetTypeName() ?? "(null)"
    ) ;

    public static string GetTypeName (
      this object objectInstance_or_type,
      string      nameToReturnIfNull      = "(null)"
    ) {
      if ( objectInstance_or_type is null )
      {
        return nameToReturnIfNull ;
      }
      else
      {
        System.Type type = (
          objectInstance_or_type is System.Type suppliedType
          ? suppliedType
          : objectInstance_or_type.GetType()
        ) ;
        return type.GetTypeName() ;
      }
    }

    public static string ToString_AllowingNull (
      this object? x_canBeNull
    ) {
      if ( x_canBeNull == null )
      {
        return "(null)" ;
      }
      object x = x_canBeNull ;
      if ( x is System.Type )
      {
        return x.GetTypeName() ;
      }
      else if ( x is System.ApplicationException exception )
      {
        return exception.Message ;
      }
      else if ( x is string s )
      {
        return s ;
      }
      else
      {
        System.Type type = x.GetType() ;
        if (
           type.IsPrimitive
        || type.IsEnum
        ) {
          return x.ToString()!.VerifiedAsNonNullInstance() ;
        }
        else if ( type.IsArray )
        {
          return string.Format(
            "(Array - {0} elements",
            ( x as System.Array )!.Length.ToString()
          ) ;
        }
        else
        {
          return x.GetOverriddenToStringText() ;
        }
      }
    }

    public static string GetOverriddenToStringText ( this object instance )
    {
      if ( instance == null )
      {
        return "(null)" ;
      }
      else
      {
        string instanceAsString = instance.ToString()!.VerifiedAsNonNullInstance() ;
        if ( instanceAsString == instance.GetType().ToString() )
        {
          // Calling 'ToString()' on the instance has just returned the type name,
          // so we can infer that 'ToString()' has not been overridden..
          // Just return the type name in brackets.
          return instanceAsString.EnclosedInBrackets("()") ;
        }
        else
        {
          return instanceAsString ;
        }
      }
    }

    public static TEnum AsEnumType<TEnum> ( this object value ) 
    where TEnum : struct, System.Enum
    {
      System.Type desiredEnumType = typeof(TEnum) ;
      try
      {
        if ( System.Enum.IsDefined(desiredEnumType,value) )
        {
          return (TEnum) System.Enum.ToObject(
            desiredEnumType,
            value
          ) ;
        }
        else
        {
          throw new System.ApplicationException() ;
        }
      }
      catch ( System.Exception x )
      {
        x.ToString(); //TODO: Handle exception in Log... suppressing warning
        // 'IsDefined' will throw an exception if the type of the 'value'
        // is the wrong kind of integer, eg if we pass in an Int32 when
        // the enum type is declared as being represented as Int16 ...
        throw ;
      }
    }

  }

}
