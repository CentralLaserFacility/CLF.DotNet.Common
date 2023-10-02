//
// FileUtilities_Tests.cs
//

using Xunit ;
using FluentAssertions ;
using Clf.Common ;

namespace Common_UnitTests
{

  public class FileUtilities_Tests
  {

    [Fact]
    public void CanWriteIntegerValuesToFileAndReadThemBack ( )
    {
      using ( var file = new TempFile() )
      {
        var itemsWritten = new int[]{1,2,3} ;
        FileUtilities.WriteNumericValuesToFile(
          itemsWritten,
          file.Path
        ) ;
        var itemsRead = FileUtilities.ReadNumericValuesFromTextFile_OnePerLine<int>(file.Path) ;
        itemsRead.Should().BeEquivalentTo(itemsWritten) ;
      }
    }

    [Fact]
    public void CanWriteDoubleValuesToFileAndReadThemBack ( )
    {
      using ( var file = new TempFile() )
      {
        var itemsWritten = new double[]{1,2,3} ;
        FileUtilities.WriteNumericValuesToFile(
          itemsWritten,
          file.Path
        ) ;
        var itemsRead = FileUtilities.ReadNumericValuesFromTextFile_OnePerLine<double>(file.Path) ;
        itemsRead.Should().BeEquivalentTo(itemsWritten) ;
      }
    }

  }

}
