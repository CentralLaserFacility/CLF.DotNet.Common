//
// PathUtilities_Tests.cs
//

using Xunit;
using FluentAssertions;
using static FluentAssertions.FluentActions;

using Clf.Common.ExtensionMethods;
using System.Linq;

namespace Common_UnitTests
{

  public class PathUtilities_Tests
  {

    [Fact]
    void PathUtilities_RootDirectoryHoldingDotNetGithubRepos_works_as_expected ( )
    {
      string repoDirectory = Clf.Common.PathUtilities.RootDirectoryHoldingDotNetGithubRepos ;
      // With any luck, if we navigate from the 'repos' directory into the sub-directory
      // that contains this 'test' project, and to this file, we'll find that it matches ...
      GetSourceCodePath().Should().Be(
        repoDirectory 
      + @"CLF.DotNet.Common\Clf.Common.Tests\Helper functions\PathUtilities_Tests.cs"
      ) ;
    }

    private static string GetSourceCodePath ( 
      [System.Runtime.CompilerServices.CallerFilePath] string? pathUtilitiesSourceCodePath = null 
    ) => pathUtilitiesSourceCodePath! ;

  }

}
