//
// CustomAttributeQueries.cs
//

using System.Collections.Generic ;
using System.Diagnostics.CodeAnalysis;
using System.Linq ;

// using Common.ExtensionMethods ;

namespace Clf.Common.ExtensionMethods
{

  // Attributes from MemberInfo

  public static partial class CustomAttributeQueries
  {

    // Getting all the custom attributes

    public static IEnumerable<T> GetAllCustomAttributesSpecifiedByMemberInfo<T> (
      this System.Reflection.MemberInfo memberInfo
    ) where T : System.Attribute
    {
      object[] attributes = memberInfo.GetCustomAttributes(
        typeof(T),
        false
      ).ToArray() ; ;
      foreach ( T customAttribute in attributes )
      {
        yield return customAttribute as T ;
      }
    }

    // Querying the existence of custom attributes of a specific type

    public static bool MemberInfoSpecifiesCustomAttribute<T> (
      this System.Reflection.MemberInfo memberInfo
    ) where T : System.Attribute
    => memberInfo.MemberInfoSpecifiesCustomAttributeOfType(typeof(T)) ;

    public static bool MemberInfoSpecifiesCustomAttributeOfType (
      this System.Reflection.MemberInfo memberInfo,
      System.Type                       attributeType
    ) => memberInfo.GetCustomAttributes(
      attributeType,
      inherit : true
    ).Any() ;

    // Querying the value of a specified Attribute (if it is present)

    public static bool MemberInfoSpecifiesCustomAttribute<T> (
      this System.Reflection.MemberInfo memberInfo,
      [NotNullWhen(true)] out T         attribute
    ) where T : System.Attribute
    {
      attribute = memberInfo.GetCustomAttributes(
        typeof(T),
        inherit : true
      ).OfType<T>().FirstOrDefault()! ;
      return attribute != null ;
    }

    public static T GetCustomAttributeSpecifiedByMemberInfo<T> (
      this System.Reflection.MemberInfo memberInfo
    ) where T : System.Attribute
    => memberInfo.GetCustomAttributes(
      typeof(T),
      inherit : true
    ).OfType<T>().First() ;

    public static T? GetCustomAttributeSpecifiedByMemberInfoOrNull<T> (
      this System.Reflection.MemberInfo memberInfo
    ) where T : System.Attribute
    => memberInfo.GetCustomAttributes(
      typeof(T),
      inherit : true
    ).OfType<T>().FirstOrDefault() ;

  }

  // Attributes from a Type

  public static partial class CustomAttributeQueries
  {

    public static bool TypeHasCustomAttribute<T> (
      this System.Type type
    ) where T : System.Attribute
    => type.GetCustomAttributes(typeof(T),true).Any() ;

  }

  // Attributes from an Enum value

  public static partial class EnumValueExtensions
  {

    public static bool SpecifiesCustomAttribute<T> (
      this System.Enum enumValue,
      out T            customAttribute
    ) where T : System.Attribute
    {
      System.Type enumType = enumValue.GetType() ;
      string valueAsString = enumValue.ToString() ;
      System.Reflection.FieldInfo fieldInfo = enumType.GetField(valueAsString)!.VerifiedAsNonNullInstance() ;
      return fieldInfo.MemberInfoSpecifiesCustomAttribute<T>(
        out customAttribute
      ) ;
    }

    public static T GetCustomAttribute<T> (
      this System.Enum enumValue
    ) 
    where T : System.Attribute
    {
      if (
        enumValue.SpecifiesCustomAttribute<T>(
          out T customAttribute
        ) 
      ) {
        return customAttribute ;
      }
      else
      {
        throw new System.ApplicationException("Custom attribute not found") ;
      }
    }

  }

}
