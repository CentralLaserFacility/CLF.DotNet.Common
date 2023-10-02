//
// ColourMappingTable_Tests.cs
//

using Xunit ;
using Xunit.Abstractions ;

using FluentAssertions ;

using Clf.Common.ImageProcessing ;
using Clf.Common.ExtensionMethods ;

namespace ImageProcessing_Tests
{

  public class ColourMappingTable_Tests // : WritesTestOutputMessages
  {

    // public ColourMappingTable_Tests ( ITestOutputHelper outputHelper ) :
    // base(outputHelper)
    // { }

    [Fact]
    public void AllTestsSucceed ( )
    {
      {
        ColourMappingTable greyTable = ColourMappingTable.GreyScale ;
        var rgb_0 = greyTable.GetColourBytesAtIntensityValue(0) ;
        rgb_0.Should().Be((0,0,0)) ;
        var rgb_255 = greyTable.GetColourBytesAtIntensityValue(255) ;
        rgb_255.Should().Be((255,255,255)) ;
      }
      {
        ColourMappingTable redTable = ColourMappingTable.Red ;
        var rgb_0 = redTable.GetColourBytesAtIntensityValue(0) ;
        rgb_0.Should().Be((0,0,0)) ;
        var rgb_255 = redTable.GetColourBytesAtIntensityValue(255) ;
        rgb_255.Should().Be((255,0,0)) ;
      }
      {
        ColourMappingTable greenTable = ColourMappingTable.Green ;
        var rgb_0 = greenTable.GetColourBytesAtIntensityValue(0) ;
        rgb_0.Should().Be((0,0,0)) ;
        var rgb_255 = greenTable.GetColourBytesAtIntensityValue(255) ;
        rgb_255.Should().Be((0,255,0)) ;
      }
      {
        ColourMappingTable blueTable = ColourMappingTable.Blue ;
        var rgb_0 = blueTable.GetColourBytesAtIntensityValue(0) ;
        rgb_0.Should().Be((0,0,0)) ;
        var rgb_255 = blueTable.GetColourBytesAtIntensityValue(255) ;
        rgb_255.Should().Be((0,0,255)) ;
      }
      // Verify that all ColourMap options are accessible
      System.Enum.GetValues<ColourMapOption>().ForEachItem(
        colourMapOption => {
          var table = ColourMappingTable.ForColourMapOption(colourMapOption) ;
          table.Should().NotBeNull() ;
          var rgb_0 = table.GetColourBytesAtIntensityValue(0) ;
          var rgb_255 = table.GetColourBytesAtIntensityValue(255) ;
        }
      ) ;
    }

  }

}
