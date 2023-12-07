//
// EnumHelpers.cs
//

using System.Collections.Generic;
using System.Linq;

namespace Clf.Common.Utils
{

  public static class EnumHelpers
  {

    public static TEnum? GetEnumValueFromStringOrNull<TEnum> (
      this string? stringValueOrNull
    ) 
    where TEnum : struct, System.Enum
    => (
      // Maybe testing explicitly for 'null' is unnecessary,
      // because if a string isn't recognised, we'll be returning
      // a null value anyway ... 
      stringValueOrNull == "null"
      ? null
      : (
        System.Enum.TryParse<TEnum>(
          value      : stringValueOrNull,
          ignoreCase : true,
          result     : out var enumValueToReturn
        ) 
        ? enumValueToReturn 
        : null
      ) 
    ) ;

    /// <summary>
    /// Get enumeration member from integer value
    /// </summary>
    /// <param name="enumType"></param>
    /// <param name="valueId"></param>
    /// <returns>Returns type System.Enum or null </returns>
    public static object? GetEnumValueFromIntegerOrNull(System.Type enumType, int valueId)
    {
        return System.Enum.ToObject(enumType, valueId);
    }

    /// <summary>
    /// Get enumeration member from string value
    /// </summary>
    /// <param name="enumType"></param>
    /// <param name="name"></param>
    /// <returns>Returns type System.Enum or null </returns>
    public static object? GetEnumValueFromStringOrNull(System.Type enumType, string name)
    {
        if (System.Enum.TryParse(enumType, name, out var choice))
            return choice;
        return null;
    }

    public static TEnum? GetEnumValueFromIntegerOrNull<TEnum> (
      this int? integerValueOrNull
    ) 
    where TEnum : struct, System.Enum
    {
      TEnum? enumValueToReturn = null ;
      if ( 
         integerValueOrNull != null
      && System.Enum.IsDefined(
           typeof(TEnum),
           integerValueOrNull
         ) 
      ) {
        enumValueToReturn = (TEnum) System.Enum.ToObject(
          typeof(TEnum), 
          integerValueOrNull
        ) ;
      }
      return enumValueToReturn ;
    }

    public static TEnum? GetEnumValueFromObjectOrNull<TEnum> (
      this object? objectValueOrNull
    ) 
    where TEnum : struct, System.Enum
    => objectValueOrNull switch {
      TEnum enumValueProvided => enumValueProvided,
      int ordinalValueProvided => GetEnumValueFromIntegerOrNull<TEnum>(
        ordinalValueProvided
      ),
      string stringValueProvided => GetEnumValueFromStringOrNull<TEnum>(
        stringValueProvided
      ),
      _ => null
    } ;

    public static IEnumerable<int> GetIntegerValuesForType<TEnum> ( 
    ) where TEnum : struct, System.Enum
    => System.Enum.GetValues<TEnum>().Select(
      value => System.Convert.ToInt32(value)
    ) ;

    public static IEnumerable<(string,int)> GetNamesAndIntegerValuesForType<TEnum> ( 
    ) where TEnum : struct, System.Enum
    => System.Linq.Enumerable.Zip(
      System.Enum.GetNames<TEnum>(),
      System.Enum.GetValues<TEnum>().Select(
        value => System.Convert.ToInt32(value)
      )
    ) ;

    public static IEnumerable<string> GetNamesAndIntegerValuesAsStringsForType<TEnum> ( 
      string format = "{0} (#{1})"
    ) where TEnum : struct, System.Enum
    => System.Enum.GetValues<TEnum>().Select(
      value => GetNameAndIntegerValueAsString(value,format)
    ) ;

    public static TEnum GetNextCyclicValue<TEnum> ( 
      this TEnum value 
    ) where TEnum : struct, System.Enum
    {
      List<int> integerValuesAvailable = GetIntegerValuesForType<TEnum>().ToList() ;
      int currentIntegerValue = GetIntegerValue(value) ;
      int currentIndex = integerValuesAvailable.IndexOf(currentIntegerValue) ;
      int nextIndex = ( currentIndex + 1 ) % integerValuesAvailable.Count ;
      int nextIntegerValue = integerValuesAvailable[nextIndex] ;
      return (TEnum) System.Enum.ToObject(
        typeof(TEnum), 
        nextIntegerValue
      ) ;
    }

    // ======================================

    public static IEnumerable<int> GetIntegerValuesForEnumType ( 
      System.Type enumType
    ) {
      foreach ( object value in System.Enum.GetValues(enumType) )
      {
        yield return System.Convert.ToInt32(value) ;
      }
    }

    public static IEnumerable<(string,int)> GetNamesAndIntegerValuesForEnumType ( 
      System.Type enumType
    ) => (
      System.Linq.Enumerable.Zip(
        System.Enum.GetNames(enumType),
        GetValuesForEnumType(enumType).Select(
          (object value) => System.Convert.ToInt32(value)
        )
      )
    ) ;

    public static IEnumerable<System.Enum> GetValuesForEnumType ( System.Type enumType )
    {
      if ( enumType.IsEnum ) 
      {
        System.Array values = System.Enum.GetValues(enumType) ;
        foreach ( System.Enum value in values ) 
        {
          yield return value ;
        }
      }
    }

    public static int GetIntegerValue ( 
      this System.Enum value 
    ) => System.Convert.ToInt32(value) ;

    public static int? GetIntegerValueOrNull ( 
      this System.Enum? value 
    ) => (
      value is null 
      ? null 
      : System.Convert.ToInt32(value) 
    ) ;

    public static string GetNameOrNull ( 
      this System.Enum? value 
    ) => value?.ToString() ?? "null" ;

    public static (string,int)? GetNameAndIntegerValueOrNull ( 
      this System.Enum? value 
    ) => (
      value is null 
      ? null 
      : (value.ToString()!,System.Convert.ToInt32(value)) 
    ) ;

    public static string GetNameAndIntegerValueAsString ( 
      this System.Enum? value,
      string            format = "{0} (#{1})"
    ) {
      (string name,int integerValue)? nameAndIntegerValue = GetNameAndIntegerValueOrNull(value) ;
      return (
        nameAndIntegerValue.HasValue
        // ? $"{nameAndIntegerValue!.Value.name} (#{nameAndIntegerValue!.Value.integerValue})"
        ? string.Format(format,nameAndIntegerValue!.Value.name,nameAndIntegerValue!.Value.integerValue)
        : "null"
      ) ;
    }

    public static IEnumerable<string> GetNamesAndIntegerValuesAsStringsForEnumType ( 
      System.Type enumType,
      string      format = "{0} (#{1})"
    ) => GetValuesForEnumType(enumType).Select(
      value => GetNameAndIntegerValueAsString(value,format)
    ) ;

    public static object? GetNextCyclicValueFromEnum ( 
      this object? value 
    ) {
      if ( value is System.Enum enumValue )
      {
        System.Type enumValueType = enumValue.GetType() ;
        int integerValue = GetIntegerValue(enumValue) ;
        List<int> integerValuesAvailable = GetIntegerValuesForEnumType(enumValueType).ToList() ;
        int currentIndex = integerValuesAvailable.IndexOf( integerValue ) ;
        int nextIndex = ( currentIndex + 1 ) % integerValuesAvailable.Count ;
        int nextIntegerValue = integerValuesAvailable[nextIndex] ;
        return System.Enum.ToObject(
          enumValueType, 
          nextIntegerValue
        ) ;
      }
      else
      { 
        return null ; 
      }
    }

  }

}
