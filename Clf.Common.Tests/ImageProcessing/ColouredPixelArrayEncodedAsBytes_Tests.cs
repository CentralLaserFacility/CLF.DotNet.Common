//
// ColouredPixelArrayEncodedAsBytes_Tests.cs
//

using FluentAssertions ;
using Xunit ;
using Xunit.Abstractions;

using Clf.Common.ImageProcessing ;

namespace ImageProcessing_Tests
{

  public class ColouredPixelArrayEncodedAsBytes_Tests 
  {

    [Fact]
    public void IntegrityTestsOnSmallImageSucceeds ( ) 
    {

      int width  = 3 ; 
      int height = 2 ;

      byte[] slowButSureResult = RunUnitTests(1) ;

      byte[] directByteAccessResult = RunUnitTests(2) ;
      directByteAccessResult.Should().BeEquivalentTo(slowButSureResult) ;

      byte[] directIntAccessResult = RunUnitTests(3) ;
      directIntAccessResult.Should().BeEquivalentTo(slowButSureResult) ;
      
      byte[] RunUnitTests ( int byteAccessOption )
      {
        
        (
          ColouredPixelArrayEncodedAsBytes_A_B_C.ACCESS_BYTE_ARRAY_USING_UNSAFE_POINTERS,
          ColouredPixelArrayEncodedAsBytes_A_B_C.WRITE_PIXELS_IN_FOUR_BYTE_CHUNKS
        ) = byteAccessOption switch {
        1 => (false,false),
        2 => (true,false),
        3 => (true,true),
        _ => throw new System.NotImplementedException()
        } ;
        
        LinearArrayHoldingPixelBytes intensityBytes = new LinearArrayHoldingPixelBytes(
          width,
          height,
          new byte[6]{0,1,2,3,4,5}
        ) ;
        // All colour map entries are black apart from 0..5
        var colourMapEntries = new RgbByteValues[256] ;
        colourMapEntries[0] = RgbByteValues.Red ;
        colourMapEntries[1] = RgbByteValues.Green ;
        colourMapEntries[2] = RgbByteValues.Blue ;
        colourMapEntries[3] = RgbByteValues.Black ;
        colourMapEntries[4] = RgbByteValues.Grey ;
        colourMapEntries[5] = RgbByteValues.White ;
        var colourMap = new ColourMappingTable(
          colourMapEntries
        ) ;
        colourMap.BuildAllCachedTables() ;
        ColouredPixelArrayEncodedAsBytes_Base colouredPixelArrayEncodedAsBytes ;
        // using ( 
        //   var timer = new ExecutionTimer_ShowingMillisecsElapsed(
        //     $"{width}x{height} ; Creating a ColouredPixelArrayEncodedAsBytes /( option /#{byteAccessOption})",
        //     WriteMessage
        //   )
        // ) {
          colouredPixelArrayEncodedAsBytes = ColouredPixelArrayEncodedAsBytes_Base.CreateInstance(
            intensityBytes,
            colourMap
          ) ;
        // }
        colouredPixelArrayEncodedAsBytes.CanGetPixel(
          0,
          0,
          out RgbByteValues? pixel_0_0
        ).Should().BeTrue() ;
        pixel_0_0.Should().NotBeNull() ;
        pixel_0_0!.Value.Should().Be(RgbByteValues.Red) ;
        colouredPixelArrayEncodedAsBytes.PixelColourIs( 0,0, RgbByteValues.Red   ).Should().BeTrue() ;
        colouredPixelArrayEncodedAsBytes.PixelColourIs( 1,0, RgbByteValues.Green ).Should().BeTrue() ;
        colouredPixelArrayEncodedAsBytes.PixelColourIs( 2,0, RgbByteValues.Blue  ).Should().BeTrue() ;
        colouredPixelArrayEncodedAsBytes.PixelColourIs( 0,1, RgbByteValues.Black ).Should().BeTrue() ;
        colouredPixelArrayEncodedAsBytes.PixelColourIs( 1,1, RgbByteValues.Grey  ).Should().BeTrue() ;
        colouredPixelArrayEncodedAsBytes.PixelColourIs( 2,1, RgbByteValues.White ).Should().BeTrue() ;
        colouredPixelArrayEncodedAsBytes.CanGetPixel(
          -1,
          0,
          out RgbByteValues? _
        ).Should().BeFalse() ;
        colouredPixelArrayEncodedAsBytes.CanGetPixel(
          3,
          0,
          out RgbByteValues? _
        ).Should().BeFalse() ;
        colouredPixelArrayEncodedAsBytes.CanGetPixel(
          0,
          -1,
          out RgbByteValues? _
        ).Should().BeFalse() ;
        colouredPixelArrayEncodedAsBytes.CanGetPixel(
          0,
          2,
          out RgbByteValues? _
        ).Should().BeFalse() ;
        return colouredPixelArrayEncodedAsBytes.ColouredImageBytesLinearArray ;
      }
    }

  }

}

