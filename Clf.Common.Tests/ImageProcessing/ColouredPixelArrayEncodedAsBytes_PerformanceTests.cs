//
// ColouredPixelArrayEncodedAsBytes_PerformanceTests.cs
//

using Xunit ;
using Xunit.Abstractions ;

using Clf.Common.ImageProcessing ;

namespace ImageProcessing_Tests
{

  public class ColouredPixelArrayEncodedAsBytes_PerformanceTests : WritesTestOutputMessages
  {

    public ColouredPixelArrayEncodedAsBytes_PerformanceTests ( ITestOutputHelper outputHelper ) :
    base(outputHelper)
    { }

    [Theory]
    [InlineData(700,500)]
    [InlineData(1200,900)]
    [InlineData(2000,1500)]
    public void PerformanceTestsSucceed ( 
      int width, 
      int height 
    ) {

      RunPerformanceTests( useDirectMemoryAccess : false ) ;
      RunPerformanceTests( useDirectMemoryAccess : true  ) ;

      void RunPerformanceTests ( bool useDirectMemoryAccess )
      {
        string buildType = (
          #if DEBUG
            "DEBUG"
          #else
            "RELEASE"
          #endif
        ) ;
        string messagePrefix = $"({buildType} build) : Size {width}x{height} ; directAccess={useDirectMemoryAccess}" ;
        ColouredPixelArrayEncodedAsBytes_A_B_C.ACCESS_BYTE_ARRAY_USING_UNSAFE_POINTERS = useDirectMemoryAccess ;
        LinearArrayHoldingPixelBytes grayScaleIntensityValues = new LinearArrayHoldingPixelBytes(width,height) ;
        var colourMap = ColourMappingTable.ForColourMapOption(
          ColourMapOption.Cool
        ) ;
        colourMap.BuildAllCachedTables() ;

        ColouredPixelArrayEncodedAsBytes_Base? colouredPixelArrayEncodedAsBytes = null ;

        using ( 
          var timer = new ExecutionTimer_ShowingMillisecsElapsed(
            $"{messagePrefix} ; Loading a ColouredPixelArray",
            WriteMessage
          )
        ) {
          colouredPixelArrayEncodedAsBytes = ColouredPixelArrayEncodedAsBytes_Base.CreateInstance(
            grayScaleIntensityValues,
            colourMap
          ) ;
        }

        using ( 
          var timer = new ExecutionTimer_ShowingMillisecsElapsed(
            $"{messagePrefix} ; Drawing a box",
            WriteMessage
          )
        ) {
          OverlayBoxDescriptor box = OverlayBoxDescriptor.FromCentrePoint(
            width/2,
            height/2,
            width/2,
            height/2,
            RgbByteValues.Red,
            false
          ) ;
          box.Draw(
            colouredPixelArrayEncodedAsBytes!
          ) ;
        }
      
      }

    }

    [Theory]
    [InlineData(700,1000)]
    public void ComparativePerformanceTestsSucceed (       
      int width,
      int height
    ) {

      byte[] slowButSureResult = RunPerformanceTests(1) ;

      byte[] directByteAccessResult = RunPerformanceTests(2) ;
      // directByteAccessResult.Should().BeEquivalentTo(slowButSureResult) ;

      byte[] directIntAccessResult = RunPerformanceTests(3) ;
      // directIntAccessResult.Should().BeEquivalentTo(slowButSureResult) ;

      byte[] RunPerformanceTests ( int byteAccessOption )
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
          new byte[width*height]
        ) ;
        // All colour map entries are black
        var colourMapEntries = new RgbByteValues[256] ;
        var colourMap = new ColourMappingTable(
          colourMapEntries
        ) ;
        colourMap.BuildAllCachedTables() ;
        // var x = colourMap.IntegerValuesTable_ABGR ;
        ColouredPixelArrayEncodedAsBytes_Base colouredPixelArrayEncodedAsBytes = null! ;
        using ( 
          var timer = new ExecutionTimerEx_ShowingMillisecsElapsed(
            $"{width}x{height} ; Creating a ColouredPixelArrayEncodedAsBytes (option #{byteAccessOption})",
            () => {
              colouredPixelArrayEncodedAsBytes = ColouredPixelArrayEncodedAsBytes_Base.CreateInstance(
                intensityBytes,
                colourMap
              ) ;
            },
            WriteMessage
          )
        ) {
        }
        return colouredPixelArrayEncodedAsBytes!.ColouredImageBytesLinearArray ;
      }
    }

  }

}

