//
// String_ExtensionMethods_Parsing.cs
//

using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace Clf.Common.ExtensionMethods
{

  // NOTE : THE PARSING METHODS MAY BE FLAKEY, FURTHER WORK NEEDED ...

  // IN SOME PLACES WE'RE WICKEDLY USING THE '!' OPERATOR ...
  // LOOKING FORWARD TO HAVING '!!' IN C# 11 :)

  public static class String_ExtensionMethods_Parsing
  {

    //
    // TODO : Instead of 'System.Convert', which throws
    // a FormatException on failure, use TryParse() instead ??
    //
    // Also see these (28!) suggestions :
    // https://stackoverflow.com/questions/2961656/generic-tryparse
    // https://stackoverflow.com/questions/1271562/binary-string-to-integer
    //

    // TODO : THIS NEEDS SOME WORK !!!
    // Nullable ??? Numeric vs string types etc ???
    // ParsedAsNumeric ???

    [return:MaybeNull]
    public static object ParsedAs ( 
      this string stringValue, // Special value 'null' => null value ; hmm, a bit dodgy ...
      System.Type desiredType 
    ) {
      if (
        desiredType.IsNullableType(
          out System.Type? underlyingType
        )
      ) {
        // We have a nullable value type ...
        if (
           stringValue.IsEmpty()
        || stringValue == "null"
        ) {
          return null ;
        }
        else
        {
          return stringValue.ParsedAs(underlyingType) ;
        }
      }
      // Not a nullable value type ...
      if ( desiredType == typeof(string) )
      {
        // Simple, we're trying to create
        // an instance of a 'string'
        return stringValue ;
      }
      if ( desiredType.IsClass )
      {
        // We're trying to create
        // an instance of a class
        if ( stringValue == "null" )
        {
          return null ;
        }
      }
      try
      {
        if ( desiredType.IsEnum )
        {
          return System.Enum.Parse(
            desiredType,
            stringValue,
            ignoreCase : true
          ) ;
        }
        else if ( desiredType == typeof(int) )
        {
          // https://theburningmonk.com/2010/02/converting-hex-to-int-in-csharp/
          int fromBase = 10 ;
          if ( stringValue.StartsWith("0x",System.StringComparison.InvariantCulture) )
          {
            fromBase = 16 ;
            stringValue = stringValue.Substring(startIndex:2) ;
          }
          else if ( stringValue.StartsWith("0b",System.StringComparison.InvariantCulture) )
          {
            fromBase = 2 ;
            stringValue = stringValue.Substring(startIndex:2) ;
          }
          // Hmm, would be nice to use 'int.Parse()' here as that wouldn't
          // throw an exception - however that only supports base 10.
          // Given that we've checked for '0b' however ...
          return System.Convert.ToInt32(stringValue,fromBase) ;
        }
        else if ( desiredType == typeof(short) )
        {
          int fromBase = 10 ;
          if ( stringValue.StartsWith("0x",System.StringComparison.InvariantCulture) )
          {
            fromBase = 16 ;
            stringValue = stringValue.Substring(startIndex:2) ;
          }
          else if ( stringValue.StartsWith("0b",System.StringComparison.InvariantCulture) )
          {
            fromBase = 2 ;
            stringValue = stringValue.Substring(startIndex:2) ;
          }
          return System.Convert.ToInt16(stringValue,fromBase) ;
        }
        else if ( desiredType == typeof(byte) )
        {
          int fromBase = 10 ;
          if ( stringValue.StartsWith("0x",System.StringComparison.InvariantCulture) )
          {
            fromBase = 16 ;
            stringValue = stringValue.Substring(startIndex:2) ;
          }
          else if ( stringValue.StartsWith("0b",System.StringComparison.InvariantCulture) )
          {
            fromBase = 2 ;
            stringValue = stringValue.Substring(startIndex:2) ;
          }
          return System.Convert.ToByte(stringValue,fromBase) ;
        }
        else
        {
          return System.Convert.ChangeType(
            stringValue,
            desiredType
          ) ;
        }
      }
      catch ( System.Exception )
      {
        // For a 'bool' result we also allow
        // integer values of 1 and 0 ...
        if ( desiredType == typeof(bool) )
        {
          if ( stringValue.MatchesAny("1","t","y","yes") )
          {
            return "true".ParsedAs<bool>() ;
          }
          else if ( stringValue.MatchesAny("0","f","n","no") )
          {
            return "false".ParsedAs<bool>() ;
          }
        }
        throw new System.ApplicationException(
          $"Failed to parse '{stringValue}' as a value of type '{desiredType.Name}'"
        ) ;
      }
    }

    // Hmm, we're allowing 'T' to be any type at all
    // including types such as Nullable<int> ...

    [return:MaybeNull]
    public static T ParsedAs<T> ( this string value )
    => (
      (T) value.ParsedAs(
        typeof(T)
      )! // .Plinged() // Now supported in C# 10 ???
    ) ;

    // RENAME : TryParseAs ...

    public static bool CanParseAs<T> ( 
      this string       valueAsString, 
      [MaybeNull] out T parsedValue  
    )
    {
      try 
      {
        parsedValue = (T) valueAsString.ParsedAs(
          typeof(T)
        )! ;
        return true ;
      }
      catch
      {
        parsedValue = default ;
        return false ;
      }
    }

    public static void ParseOrThrow<T> ( this string value, out T parsedValue  )
    {
      try 
      {
        parsedValue = (T) value.ParsedAs(
          typeof(T)
        ).Plinged() ;
      }
      catch
      {
        throw ;
      }
    }

    // RENAME => TryParseAs ...

    public static bool CanParseAs<T> ( this string value, System.Action<T> parseSucceededAction  )
    {
      try 
      {
        var parsedValue = (T) value.ParsedAs(
          typeof(T)
        ).Plinged() ;
        parseSucceededAction(parsedValue) ;
        return true ;
      }
      catch ( System.Exception x )
      {
        x.ToString(); //TODO: Handle exception in Log... suppressing warning
        return false ;
      }
    }

    public static bool CouldParseAs<T> ( this string value )
    {
      try
      {
        var _ = value.ParsedAs<T>() ;
        return true ;
      }
      catch
      {
        return false ;
      }
    }

    public static T? ParsedAsNullableValue<T> ( this string value ) where T : struct
    {
      if (
         value.IsEmpty()
      || value == "null"
      ) {
        return default ;
      }
      else
      {
        return value.ParsedAs<T>() ;
      }
    }

    public static bool TryParseAsValue<T> ( 
      this string                s, 
      [NotNullWhen(true)] out T? value 
    ) where T : struct
    {
      try
      {
        value = s.ParsedAs<T>() ; 
        return true ;
      }
      catch
      {
        value = default ;
        return false ;
      }
    }

  }

}
