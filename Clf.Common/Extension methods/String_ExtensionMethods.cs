//
// String_ExtensionMethods.cs
//

using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace Clf.Common.ExtensionMethods
{

  public static class String_ExtensionMethods
  {

    public static bool MatchesAny (
      this string         nameProvided,
      IEnumerable<string> options
    ) => (
      options.Where(
        option => nameProvided.ToLower() == option.ToLower()
      ).Any()
    ) ;

    public static bool MatchesAny (
      this string     nameProvided,
      params string[] options
    ) => (
      nameProvided.MatchesAny(
        options as IEnumerable<string>
      )
    ) ;

    public static string Repeated ( this string s, int nRepeats )
    {
      var stringBuilder = new System.Text.StringBuilder(
        s.Length
      * nRepeats
      ) ;
      while ( nRepeats-- > 0 )
      {
        stringBuilder.Append(s) ;
      }
      return stringBuilder.ToString() ;
    }

    public static string PrefixedWith ( this string s, string prefix )
    => (
      prefix + s
    ) ;

    public static bool IsNullOrEmpty ( this string? s )
    // => s?.Length == 0 ?? true ; // WRONG !!!
    // => s?.Length.Equals(0) ?? true ;
    => (
      string.IsNullOrEmpty(s)
    ) ;

    public static bool IsEmpty ( this string s )
    => (
      s.Length == 0
    ) ;

    public static bool LooksLikeInteger ( this string s )
    => s.TryParseAsValue(
      out int? _
    ) ;

    public static string EnclosedInDoubleQuotes ( this string s )
    => (
      $"\"{s}\""
    ) ;

    public static string EnclosedInBrackets ( this string s, string brackets = "[]" )
    => (
      brackets.Length == 2
      ? $"{brackets[0]}{s}{brackets[1]}"
      : s
    ) ;

    public static string WithEnclosingQuotesOrBracketsStripped (
      this string s,
      string?     validPairs = null
    ) {
      validPairs ??= "''\"\"()[]{}<>" ;
      s = s.Trim() ;
      if ( ! string.IsNullOrEmpty(s) )
      {
        char firstCh = s[0] ;
        char lastCh  = s[^1] ;
        char[] validPairsArray = validPairs.ToCharArray() ;
        for ( int j = 0 ; j < validPairsArray.Length ; j += 2 )
        {
          // If we succeed in finding the first/last pair as an entry in our list,
          // it means that those characters *were* bracketing the string
          // so we should remove them and return the trimmed result
          if (
             validPairsArray[j]   == firstCh
          && validPairsArray[j+1] == lastCh
          ) {
            s = s[
              1 .. ^1
            ] ;
            break ;
          }
        }
      }
      return s ;
    }

    [return:NotNullIfNotNullAttribute("s")]
    public static string? PrefixedBy ( this string? s, string prefix )
    => (
      s == null
      ? null
      : prefix + s
    ) ;

  }

}
