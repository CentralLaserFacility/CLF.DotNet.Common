﻿//
// Helpers.cs
//

using Clf.Common.ExtensionMethods;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Clf.Common
{

  public static partial class Helpers
  {

    // Note that if T is a reference type,
    // all the elements will be set to refer
    // to the instance passed in as 'valueForAllElements' ...
    // Hmm, is this really what we want ? YES !!!

    public static T[] CreateArrayOfObjects<T>(
      int nElements,
      T valueForAllElements = default(T)!
    )
    {
      var array = (T[])System.Array.CreateInstance(
        typeof(T),
        nElements
      );
      System.Array.Fill(
        array,
        valueForAllElements
      );
      return array;

    }

    // Note that if the object is a reference type,
    // all the elements will be set to refer
    // to the instance passed in ...
    // Hmm, is this really what we want ?

    public static System.Array CreateArrayOfObjects(
      int nElements,
      System.Type typeOfObject,
      object? valueForAllElements
    )
    {
      var array = System.Array.CreateInstance(
        typeOfObject,
        nElements
      );
      // Hmm, is there a more efficient way to do this ??
      // TODO : HOW DOES THIS PLAY WITH VALUE TYPES ???
      for (int i = 0; i < nElements; i++)
      {
        array.SetValue(
          value: valueForAllElements,
          index: i
        );
      }
      return array;
    }

    //
    // Create an array that is an 'expanded' version of the original,
    // where the elements of the new array are filled in by visiting 
    // the 'original' elements in a cyclic sequence.
    //
    // [ 1 2 3 ] => [ 1 2 3  1 2 3  1 2 ]
    //

    public static System.Array CreateExpandedArrayOfObjects(
      System.Array originalArray,
      int nExpandedElements
    )
    {
      System.Type typeOfObject = originalArray.GetType().GetElementType()!;
      var expandedArray = System.Array.CreateInstance(
        typeOfObject,
        nExpandedElements
      );
      int iSource = 0;
      int nSourceElements = originalArray.Length;
      if (nSourceElements == 1)
      {
        object valueForAllElements = originalArray.GetValue(0)!;
        for (int i = 0; i < nExpandedElements; i++)
        {
          expandedArray.SetValue(
            value: valueForAllElements,
            index: i
          );
        }
      }
      else
      {
        for (int iTarget = 0; iTarget < nExpandedElements; iTarget++)
        {
          object sourceElement = originalArray.GetValue(
            iSource++ % nSourceElements
          )!;
          expandedArray.SetValue(
            value: sourceElement,
            index: iTarget
          );
        }
      }
      return expandedArray;
    }

    //
    // Throws an exception if we fail to create
    //

    public static void EnsureDirectoryExists(this string directory)
    {
      if (!System.IO.Directory.Exists(directory))
      {
        System.IO.Directory.CreateDirectory(directory);
      }
    }

    public static string ToDelimitedList(
      this IEnumerable<string> itemNames,
      string itemsSeparator = ","
    ) => (
      string.Join(
        itemsSeparator,
        itemNames
      )
    );

    public static bool CanLocateText(
      this string s,
      string textToFind,
      [NotNullWhen(true)] out int? lineNumber_oneBased
    )
    {
      int pos = s.IndexOf(textToFind);
      if (pos == -1)
      {
        lineNumber_oneBased = null;
      }
      else
      {
        lineNumber_oneBased = 1;
        while (pos > 0)
        {
          if (s[pos] == '\n')
          {
            lineNumber_oneBased++;
          }
          pos--;
        }
      }
      return lineNumber_oneBased != null;
    }

    public static string ReadAllTextFromFile(string fileName)
    {
      return System.IO.File.ReadAllText(fileName);
    }

    public static void WriteStringToFile(string s, string fileName)
    {
      System.IO.Path.GetDirectoryName(
        fileName
      )!.EnsureDirectoryExists();
      System.IO.File.WriteAllText(fileName, s);
    }

    public static string TempFileRootPathForThisApp { get; set; } = "C:\\temp\\";

    public static string BuildFileSpecificationBasedOnTempFileRootPath(string fileName) => TempFileRootPathForThisApp + fileName;

    public static string BuildMultiLineString(
      System.Action<
        System.Action<string>
      > writeSummary
    )
    {
      System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
      writeSummary(
        (line) => stringBuilder.AppendLine(line)
      );
      return stringBuilder.ToString();
    }

    public static IEnumerable<string> ScanFileNamesInDirectory(string pathToDirectory, string searchPattern = "*.*")
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
    );

    private static System.Diagnostics.Process StartProcess(string fullPathToEXE, params string[] cmdLineParams)
    {
      try
      {
        return System.Diagnostics.Process.Start(
          fullPathToEXE,
          cmdLineParams.ToDelimitedList(
            itemsSeparator: " "
          )
        );
      }
      catch (System.ApplicationException x)
      {
        throw new System.ApplicationException(
          $"Failed to start '{fullPathToEXE}' : {x}"
        );
      }
    }

    public static System.Diagnostics.Process StartWindowsNotepad(string fileName)
    {
      return StartProcess(
        "Notepad.exe",
        fileName
      );
    }

    public static string PathToNotepadPlusPlus = @"C:\Program Files\Notepad++\Notepad++.exe";

    // https://npp-user-manual.org/docs/command-prompt/

    public static System.Diagnostics.Process StartNotepadPlusPlus_AtLineNumber(string fileName, int lineNumber_oneBased)
    {
      string args = $"-n{lineNumber_oneBased} {fileName}";
      return StartProcess(
        PathToNotepadPlusPlus,
        args
      );
    }

    public static System.Diagnostics.Process StartNotepadPlusPlus_AtFirstOccurrenceOfText(
      string fileName,
      string textToFind
    )
    {
      if (
        ReadAllTextFromFile(
          fileName
        ).CanLocateText(
          textToFind,
          out var lineNumber_oneBased
        )
      )
      {
        string args = $"-n{lineNumber_oneBased} {fileName}";
        return StartProcess(
          @"C:\Program Files\Notepad++\Notepad++.exe",
          args
        );
      }
      else
      {
        string args = $"{fileName}";
        return StartProcess(
          @"C:\Program Files\Notepad++\Notepad++.exe",
          args
        );
      }
    }

    public static System.Diagnostics.Process StartNotepadPlusPlus(string fileName)
    {
      string args = $"{fileName}";
      return StartProcess(
        @"C:\Program Files\Notepad++\Notepad++.exe",
        args
      );
    }

    // When called from C# code in a file called 'MyCode.cs',
    // this returns the full path name of that source file.

    public static string GetSourceFilePath(
      [System.Runtime.CompilerServices.CallerFilePath] string? sourceCodePath = null
    ) => sourceCodePath.VerifiedAsNonNullInstance();

    public static (string path, int lineNumber) GetSourceFilePathAndLineNumber(
      [System.Runtime.CompilerServices.CallerFilePath] string? sourceCodePath = null,
      [System.Runtime.CompilerServices.CallerLineNumberAttribute] int? sourceCodeLine = null
    ) => (sourceCodePath.VerifiedAsNonNullInstance(), sourceCodeLine ?? 0);

    // Hmm, the returned string doesn't have '/' as the last character ??
    // This can return "" as well as a valid string ...

    public static string GetSourceFileDirectory(
      [System.Runtime.CompilerServices.CallerFilePath] string? sourceCodePath = null
    ) => System.IO.Path.GetDirectoryName(
      sourceCodePath
    ).VerifiedAsNonNullInstance();

    public static string CompressMultiLineExpressionToSingleLine(
      string expressionTextLines
    )
    {
      // Should remove comments !
      return new string(
        expressionTextLines.ToCharArray().Where(
          ch => !char.IsWhiteSpace(ch)
        ).ToArray()
      );
    }
  }

}
