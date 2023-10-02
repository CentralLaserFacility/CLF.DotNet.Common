//
// TempFile_Tests.cs
//

using Xunit ;
using FluentAssertions ;
using Clf.Common ;

namespace Common_UnitTests
{

  public class TempFile_Tests
  {

    [Fact]
    public void TempFile_WorksAsExpected ( ) 
    {
      string filePath ;
      using ( var tmpFile = new TempFile() )
      {
        filePath = tmpFile.Path ;
        System.IO.File.Exists(tmpFile.Path).Should().BeTrue() ;
        using ( var file = System.IO.File.CreateText(tmpFile.Path) ) 
        {
          System.IO.File.Exists(tmpFile.Path).Should().BeTrue() ;
          file.Should().NotBeNull() ;
          file.WriteLine("Hello") ;
        }
        System.IO.File.Exists(tmpFile.Path).Should().BeTrue() ;
      }
      System.IO.File.Exists(filePath).Should().BeFalse() ;
    }

  }

}
