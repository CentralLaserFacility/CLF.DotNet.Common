//
// Helpers.cs
//

using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using Clf.Common.ExtensionMethods;
using Clf.Common.Utils;
using Clf.Common;

namespace Clf.Common.Utils
{

  public static partial class Helpers
  {

    public static string ReadAllTextFromFile ( string fileName )
    {
      return System.IO.File.ReadAllText(fileName) ;
    }

    public static void WriteStringToFile ( string s, string fileName )
    {
      System.IO.Path.GetDirectoryName(
        fileName
      )!.EnsureDirectoryExists() ;
      System.IO.File.WriteAllText(fileName,s) ;
    }

    public static string TempFileRootPathForThisApp { get ; set ; } = "C:\\temp\\" ;

    public static string BuildFileSpecificationBasedOnTempFileRootPath ( string fileName ) => TempFileRootPathForThisApp + fileName ;

    public static string BuildMultiLineString (
      System.Action<
        System.Action<string>
      > writeSummary
    ) {
      System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder() ;
      writeSummary(
        (line) => stringBuilder.AppendLine(line)
      ) ;
      return stringBuilder.ToString() ;
    }

    public static IEnumerable<string> ScanFileNamesInDirectory ( string pathToDirectory, string searchPattern = "*.*" )
    => (
      System.IO.Directory.Exists(pathToDirectory)
      ? System.IO.Directory.EnumerateFiles(
          pathToDirectory,
          searchPattern
        ).Select(
          fullName => fullName.Substring(
            pathToDirectory.Length  
          )
        )
      : Enumerable.Empty<string>()
    ) ;

    private static System.Diagnostics.Process StartProcess ( string fullPathToEXE, params string[] cmdLineParams )
    {
      try
      {
        return System.Diagnostics.Process.Start(
          fullPathToEXE,
          cmdLineParams.ToDelimitedList(
            itemsSeparator : " "
          )
        ) ;
      }
      catch ( System.ApplicationException x )
      {
        throw new System.ApplicationException(
          $"Failed to start '{fullPathToEXE}' : {x}"
        ) ;
      }
    }

    public static System.Diagnostics.Process StartWindowsNotepad ( string fileName )
    {
      return StartProcess(
        "Notepad.exe",
        fileName
      ) ;
    }  
    
    public static string PathToNotepadPlusPlus = @"C:\Program Files\Notepad++\Notepad++.exe" ;

    // https://npp-user-manual.org/docs/command-prompt/

    public static System.Diagnostics.Process StartNotepadPlusPlus_AtLineNumber ( string fileName, int lineNumber_oneBased )
    {
      string args = $"-n{lineNumber_oneBased} {fileName}" ;
      return StartProcess(
        PathToNotepadPlusPlus,
        args
      ) ;
    }  
    
    public static System.Diagnostics.Process StartNotepadPlusPlus_AtFirstOccurrenceOfText ( 
      string fileName, 
      string textToFind 
    ) {
      if (
        ReadAllTextFromFile(
          fileName
        ).CanLocateText(
          textToFind,
          out var lineNumber_oneBased
        )
      ) {
        string args = $"-n{lineNumber_oneBased} {fileName}" ;
        return StartProcess(
          @"C:\Program Files\Notepad++\Notepad++.exe",
          args
        ) ;
      }
      else
      {
        string args = $"{fileName}" ;
        return StartProcess(
          @"C:\Program Files\Notepad++\Notepad++.exe",
          args
        ) ;
      }
    }  
    
    public static System.Diagnostics.Process StartNotepadPlusPlus ( string fileName )
    {
      string args = $"{fileName}" ;
      return StartProcess(
        @"C:\Program Files\Notepad++\Notepad++.exe",
        args
      ) ;
    }  
    
    // When called from C# code in a file called 'MyCode.cs',
    // this returns the full path to that source file.

    public static string GetSourceFilePath ( 
      [System.Runtime.CompilerServices.CallerFilePath] string? sourceCodePath = null 
    ) => sourceCodePath.VerifiedAsNonNullInstance() ;

    public static ( string path, int lineNumber ) GetSourceFilePathAndLineNumber ( 
      [System.Runtime.CompilerServices.CallerFilePath]            string? sourceCodePath = null, 
      [System.Runtime.CompilerServices.CallerLineNumberAttribute] int?    sourceCodeLine = null 
    ) => (sourceCodePath.VerifiedAsNonNullInstance(),sourceCodeLine??0) ;

    // Hmm, the returned string doesn't have '/' as the last character ??
    // This can return "" as well as a valid string ...

    public static string GetSourceFileDirectory ( 
      [System.Runtime.CompilerServices.CallerFilePath] string? sourceCodePath = null 
    ) => System.IO.Path.GetDirectoryName(
      sourceCodePath
    ).VerifiedAsNonNullInstance() ;

    public static string CompressMultiLineExpressionToSingleLine ( 
      string expressionTextLines
    ) {
      // Should remove comments !
      return new string(
        expressionTextLines.ToCharArray().Where(
          ch => ! char.IsWhiteSpace(ch)
        ).ToArray() 
      ) ;
    }

    public static bool IsNumericType ( this System.Type type )
    {
      System.Type iNumberType = typeof(INumber<>) ;
      return type.GetInterfaces().Any(
        implementedInterface => (
           implementedInterface.IsGenericType 
        && implementedInterface.GetGenericTypeDefinition() == iNumberType
        )
      ) ;
    }

  }

}
