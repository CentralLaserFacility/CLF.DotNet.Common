//
// Type_ExtensionMethods.cs
//

using System.Linq ;
using System.Diagnostics.CodeAnalysis ;

namespace Clf.Common.ExtensionMethods
{

  public static partial class Type_ExtensionMethods
  {

    public static bool IsNullableType (
      this System.Type                     type,
      [NotNullWhen(true)] out System.Type? underlyingType
    )  => (
      underlyingType = System.Nullable.GetUnderlyingType(type)
    ) != null ;

    public static string GetTypeName ( this System.Type type, string nameToReturnIfNull = "(null)" )
    {
      string typeName ;
      try
      {
        if ( type is null )
        {
          typeName = nameToReturnIfNull ;
        }
        else if ( type.IsGenericParameter )
        {
          typeName = type.Name ;
        }
        else if ( type.IsArray )
        {
          typeName =  (
            GetTypeName(
              type.GetElementType()!
            )
          + "[]".Repeated(
              type.GetArrayRank()
            )
          ) ;
        }
        else if ( type.IsGenericType )
        {
          string result = type.Namespace + "." + type.Name.Split('`')[0] + "<" ;
          System.Type[] genericArguments = type.GetGenericArguments() ;
          foreach ( System.Type T in genericArguments )
          {
            result += (
              T.IsGenericParameter // T.ContainsGenericParameters
              ? T.Name
              : GetTypeName(T)
            ) + "," ;
          }
          typeName = result.TrimEnd(',') + ">" ;
        }
        else
        {
          typeName =  type.Namespace + "." + type.Name ;
        }
      }
      catch ( System.ApplicationException x )
      {
        typeName = type.Name + x ;
      }
      return (
        typeName
        .Replace( typeof( bool   ).FullName!.VerifiedAsNonNullInstance() , "bool"   )
        .Replace( typeof( byte   ).FullName!.VerifiedAsNonNullInstance() , "byte"   )
        .Replace( typeof( short  ).FullName!.VerifiedAsNonNullInstance() , "short"  )
        .Replace( typeof( int    ).FullName!.VerifiedAsNonNullInstance() , "int"    )
        .Replace( typeof( long   ).FullName!.VerifiedAsNonNullInstance() , "long"   )
        .Replace( typeof( float  ).FullName!.VerifiedAsNonNullInstance() , "float"  )
        .Replace( typeof( double ).FullName!.VerifiedAsNonNullInstance() , "double" )
        .Replace( typeof( string ).FullName!.VerifiedAsNonNullInstance() , "string" )
        .Replace( typeof( char   ).FullName!.VerifiedAsNonNullInstance() , "char"   )
      ) ;
    }

  }

}
