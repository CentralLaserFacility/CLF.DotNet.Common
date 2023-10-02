//
// FileUtilities.cs
//

using System.Collections.Generic ;

namespace Clf.Common
{

  public static class FileUtilities
  {

    public static void WriteNumericValuesToFile<T> ( 
      this IEnumerable<T> values,
      string              filePath
    )
    where T : System.Numerics.INumber<T>
    {
      using ( var file = System.IO.File.CreateText(filePath) )
      {
        foreach ( var value in values ) 
        {
          file.WriteLine(
            value.ToString()
          ) ;
        }
      }
    }

    public static IReadOnlyList<T> ReadNumericValuesFromTextFile_OnePerLine<T> ( 
      string filePath
    )
    where T : System.Numerics.INumber<T>
    {
      List<T> values = new() ;
      string[] lines = System.IO.File.ReadAllLines(filePath) ;
      foreach ( string line in lines ) 
      {
        if ( 
          T.TryParse(
            line,
            System.Globalization.NumberStyles.Number,
            null,
            out T? value
          )
        ) {
          values.Add(
            value
          ) ;
        }
      }
      return values.AsReadOnly() ;
    }

  }

}
