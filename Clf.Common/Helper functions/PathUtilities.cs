//
// PathUtilities.cs
//

namespace Clf.Common
{

  public static class PathUtilities 
  {

    public static string PathToExeDirectory => System.IO.Path.GetDirectoryName(
      System.Reflection.Assembly.GetEntryAssembly()?.Location 
    ) ?? "PathToExeDirectory_NOT_AVAILABLE" ; // Hmm, this should *always* be available ?? !!!

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
    //   C:\_repos\DotNet.ChannelAccess\Clf.ThinIoc.Server\bin\Debug\net6.0-windows
    //      |      |                    |                  |   |     |
    //      5      4                    3                  2   1     0
    // 
    // So if we step back 5 levels, we'll probably get to the directory that holds the repositories.
    //
    // From there (!!??) we can go 'up' into directories such as 'epics.dotnet/x64/Debug_DLL'
    //
    // -----------------------------
    //
    // Alternatively, we can discover the full file-name of this 'PathUtilities.cs' file,
    // which we know is located in the following directory :
    //
    //   'C:\_repos\DotNet.Common\Clf.Common\Helper functions'
    //       |      |             |          |
    //       3      2             1          0
    //
    // So stepping back 3 places from that directory, gets us to the 'repos' directory.
    //

    public static string RootDirectoryHoldingDotNetGithubRepos 
    => GetParentDirectoryOf(
      // PathToExeDirectory,
      // 5
      System.IO.Path.GetDirectoryName(
        GetSourceCodePath()
      )!,
      3
    ) ;

    private static string GetSourceCodePath ( 
      [System.Runtime.CompilerServices.CallerFilePath] string? pathUtilitiesSourceCodePath = null 
    ) => pathUtilitiesSourceCodePath! ;

  }

}
