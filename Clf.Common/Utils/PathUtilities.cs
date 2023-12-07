//
// PathUtilities.cs
//

namespace Clf.Common.Utils
{

  public static class PathUtilities 
  {

    public static string PathToExeDirectory => System.IO.Path.GetDirectoryName(
      System.Reflection.Assembly.GetEntryAssembly()?.Location 
    )! ; // Hmm, this should *always* be available ?? !!!

    public static string GetParentDirectoryOf ( string pathToDirectory, int nStepsBack = 1 )
    {
      // string parentDirectory = System.IO.Path.Combine(
      //   pathToDirectory,
      //   @"..\"
      // ) ;
      string parentDirectory = System.IO.Path.GetFullPath(
        path     : @"..\",
        basePath : pathToDirectory
      ) ;
      if ( nStepsBack == 1 )
      {
        return parentDirectory ;
      }
      else
      {
        return GetParentDirectoryOf(
          parentDirectory,
          nStepsBack - 1
        ) ;
      }
    }

    //
    // Hmm, a bit wicked but there we go.
    //
    // With the folder structure we're using, the current EXE directory
    // will be something like
    //
    //   C:\_repos\MyRepoName\MyProjectName\bin\Debug\net7.0-windows\MyExe.exe
    //      |      |          |             |   |     |              
    //      5      4          3             2   1     0
    //                                                |
    //                                                PathToExeDirectory
    //
    // So if we step back 5 levels, we'll probably get to
    // the '_repos' directory that holds the repositories.
    //
    // From there (!!??) we can go 'up' into directories such as 'epics.dotnet/x64/Debug_DLL'
    //
    // -----------------------------
    //
    // Alternatively, we can discover the full file-name of this 'PathUtilities.cs' file,
    // which we know is located in the 'Utils' directory :
    //
    //   'C:\_repos\DotNet.MachineSafety\Common\Utils\PathUtilities.cs'
    //       |      |                    |      |
    //       3      2                    1      0
    //
    // So stepping back 3 places from that directory, gets us to the 'repos' directory.
    //

    public static string RootDirectoryHoldingDotNetGithubRepos 
    => GetParentDirectoryOf(
      System.IO.Path.GetDirectoryName(
        GetPathUtilitiesSourceCodePath()
      )!,
      3
    ) ;

    //
    // This returns the full path of the source code file in which this function resides.
    // It's defined in 'PathUtilities.cs', so typically it returns something like
    //
    //   'C:\_repos\DotNet.MachineSafety\Common\Utils\PathUtilities.cs'
    //
    // ... depending on where the project is located.
    //
    // That's not necessarily very useful, but the code here serves
    // as an example of how to use the [CallerFilePath] attribute.
    //
    // IT'S NOT AT ALL USEFUL IF THE LIBRARY IS OBTAINED VIA NUGET !!!
    //
    // In other source code, eg 'XX.cs', just copy-and-paste this function as a part
    // of the 'XX' class definition, so that it will return the path to that 'XX.cs' file.
    //

    private static string GetPathUtilitiesSourceCodePath ( 
      [System.Runtime.CompilerServices.CallerFilePath] string? pathUtilitiesSourceCodePath = null 
    ) => pathUtilitiesSourceCodePath! ;

  }
}
