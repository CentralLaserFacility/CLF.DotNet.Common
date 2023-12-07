//
// TypeConversionHelpers.cs
//

namespace Clf.Common.Utils
{
  public static class TypeConversionHelpers
  {
    public static bool CanPerformTypeConversionFromObject ( 
      object?     incomingValueOrNull,
      System.Type desiredType,
      out object? convertedValueOrNull
    ) {
      // We perform conversions to the desired Type
      // if the provided value is 'compatible'.
      if ( incomingValueOrNull is null ) 
      {
        convertedValueOrNull = null ;
      }
      else
      {
        object incomingValue = incomingValueOrNull ;
        if ( desiredType == typeof(bool) )
        {
          convertedValueOrNull = PerformTypeConversionFromObject_ToBoolOrNull(
            incomingValue
          ) ;
        }
        else if ( desiredType.IsEnum )
        {
          convertedValueOrNull = PerformTypeConversionFromObject_ToEnumOrNull(
            incomingValue,
            desiredType
          ) ;
        }
        else 
        {
          try
          {
            convertedValueOrNull = System.Convert.ChangeType(
              incomingValue,
              desiredType
            ) ;
          }
          catch ( System.Exception x ) 
          { 
            convertedValueOrNull = null ;
          }
        }
      }
      return (
        convertedValueOrNull != null 
      ) ;
    }

    private static bool? PerformTypeConversionFromObject_ToBoolOrNull (
      object incomingValue
    ) {
      if ( incomingValue is bool incomingBoolValue ) 
      {
        return incomingBoolValue ;
      }
      else if ( incomingValue is int incomingIntValue ) 
      {
        // If the incoming value is an int and we're expecting bool,
        // treat 0/1 as false/true and anything else as null ...
        return incomingIntValue switch {
          0 => false,
          1 => true,
          _ => (bool?) null
        } ;
      }
      else if ( incomingValue is short incomingShortValue ) 
      {
        // If the incoming value is an int and we're expecting bool,
        // treat 0/1 as false/true and anything else as null ...
        return incomingShortValue switch {
          0 => false,
          1 => true,
          _ => (bool?) null
        } ;
      }
      else if ( incomingValue is double incomingDoubleValue ) 
      {
        // If the incoming value is a double and we're expecting bool,
        // treat 0 as false and anything else as true ...
        return (
          incomingDoubleValue != 0.0 
        ) ;
      }
      else if ( incomingValue is float incomingFloatValue ) 
      {
        // If the incoming value is a double and we're expecting bool,
        // treat 0 as false and anything else as true ...
        return (
          incomingFloatValue != 0.0f 
        ) ;
      }
      else if ( incomingValue is string incomingStringValue ) 
      {
        incomingStringValue = incomingStringValue.ToLower() ;
        if ( incomingStringValue == "null" )
        {
          return null ;
        }
        else
        {
          // These are the strings we might receive from
          // Channels that are publishing 'boolean' values !!!
          return incomingStringValue switch {

            "false" => false,
            "true"  => true,

            "f"     => false,
            "t"     => true,

            "0"     => false,
            "1"     => true,

            "no"    => false,
            "yes"   => true,

            "out"   => false,
            "in"    => true,

            _       => null
          } ;
          // bool? ReturnNull_LoggingStringValue ( string s )
          // {
          //   System.Console.WriteLine(
          //     $"**** Incoming string value '{s}' not recognised as boolean ; PV is '{channelName}'"
          //   ) ;
          //   return null ;
          // }
        }
      }
      else
      {
        return null ;
      }
    }

    private static System.Enum? PerformTypeConversionFromObject_ToEnumOrNull (
      object      incomingValue,
      System.Type desiredEnumType
    ) {
      try 
      {
        if ( incomingValue is string enumName ) 
        {
          if (
            System.Enum.TryParse(
              desiredEnumType,
              enumName,
              out object? enumValue 
            )
          ) {
            return enumValue as System.Enum ;
          }
          else
          { 
            return null ; 
          }
        }
        else
        {
          // Provided the incoming value is an integer type,
          // this conversion to an Enum will succeed, even if the
          // value is NOT one of the values specified in the 
          // declaration of the enumerated type.
          System.Enum valueAsEnum = (System.Enum) System.Enum.ToObject(
            desiredEnumType,
            incomingValue
          ) ; 
          // Aha, we can find out whether or not the value we've converted
          // is one of the values specified in the declaration ...
          if ( 
            System.Enum.IsDefined(
              desiredEnumType,
              valueAsEnum
            ) 
          ) {
            // Yes, it's a value that was specified
            // in the declaration of the enum type
            return valueAsEnum ;
          }
          else
          {
            // Not a recognised value
            return null ;
          }
        }
      }
      catch ( System.Exception x )
      { 
        return null ; 
      }
    }

  }

}
