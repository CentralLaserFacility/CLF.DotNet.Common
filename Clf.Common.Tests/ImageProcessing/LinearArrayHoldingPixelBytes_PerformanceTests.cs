//
// LinearArrayHoldingPixelBytes_PerformanceTests.cs
//

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

using Clf.Common.ImageProcessing ;

namespace ImageProcessing_Tests
{

  public class LinearArrayHoldingPixelBytes_PerformanceTests : WritesTestOutputMessages
  {

    public LinearArrayHoldingPixelBytes_PerformanceTests ( ITestOutputHelper outputHelper ) :
    base(outputHelper)
    { }

    public readonly byte[] g_dummyColourMap = new byte[256] ;

    [Theory]
    [InlineData(700,500,3)]
    [InlineData(1200,900,3)]
    [InlineData(2000,1600,3)]
    public void PerformanceTestsSucceed ( 
      int                   width, 
      int                   height, 
      int                   sizeReductionFactor
    ) {
      string imageSize = $"Size {width}x{height}" ;
      LinearArrayHoldingPixelBytes original = new LinearArrayHoldingPixelBytes(width,height) ;
      original.SetPixelInPlace(0,0,100) ;

      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"{imageSize} ; Creating a clone",
          WriteMessage
        )
      ) {
        var clone = original.CreateClone() ;
      }

      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"{imageSize} ; Rotate 90, creating a clone",
          WriteMessage
        )
      ) {
        var rotated = original.CreateRotatedClone(RotationFactor.RotateClockwise90) ;
      }

      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"{imageSize} ; Rotate 180, creating a clone",
          WriteMessage
        )
      ) {
        var rotated = original.CreateRotatedClone(RotationFactor.RotateClockwise180) ;
      }

      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"{imageSize} ; Mirror, creating a clone",
          WriteMessage
        )
      ) {
        var rotated = original.CreateRotatedClone(RotationFactor.MirrorAroundVerticalCentre) ;
      }

      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"{imageSize} ; Rotate and mirror, creating a clone",
          WriteMessage
        )
      ) {
        var rotatedThenMirrored = original.CreateRotatedClone(RotationFactor.RotateClockwise90_ThenMirrorAroundVerticalCentre) ;
      }

      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"{imageSize} ; No rotation (but creates a clone)",
          WriteMessage
        )
      ) {
        var rotatedThenMirrored = original.CreateRotatedClone(RotationFactor.None) ;
      }

      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"{imageSize} ; Reduce size by factor of {sizeReductionFactor}",
          WriteMessage
        )
      ) {
        var reduced = original.CreateResizedClone_NearestNeighbour(
          original.Width  / sizeReductionFactor,
          original.Height / sizeReductionFactor
        ) ;
      }

      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"{imageSize} ; Normalize, creating a clone",
          WriteMessage
        )
      ) {
        var clone = original.CreateNormalisedClone(120) ;
      }

      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"{imageSize} ; Normalize, in place",
          WriteMessage
        )
      ) {
        original.ApplyNormalisationInPlace(120) ;
      }

      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"{imageSize} ; Create clone of region",
          WriteMessage
        )
      ) {
        original.CreateCloneOfRegion(0,0,width/2,height/2) ;
      }

      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"{imageSize} ; Read all pixels, one by one",
          WriteMessage
        )
      ) {
        for ( int x = 0 ; x < width ; x++ )
        {
          for ( int y = 0 ; y < height ; y++ )
          {
            var pixel = original.GetPixel(x,y) ;
          }
        }
      }

      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"{imageSize} ; Write all pixels, one by one",
          WriteMessage
        )
      ) {
        for ( int x = 0 ; x < width ; x++ )
        {
          for ( int y = 0 ; y < height ; y++ )
          {
            original.SetPixelInPlace(x,y,100) ;
          }
        }
      }

      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"{imageSize} ; Write all pixels, same value",
          WriteMessage
        )
      ) {
        original.SetAllPixelsInPlace(100) ;
      }

      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"{imageSize} ; Write all pixels, using evaluation function",
          WriteMessage
        )
      ) {
        original.SetAllPixelsInPlace(
          (x,y) => 100
        ) ;
      }

      ColouredPixelArrayEncodedAsBytes_Base? colouredPixelArray = null ;

      var colourMap = ColourMappingTable.GreyScale ;
      colourMap.BuildAllCachedTables() ;

      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"{imageSize} ; Build coloured pixel array",
          WriteMessage
        )
      ) {
        // Clf.ImageManipulation.Helpers.BuildColouredPixelArrayEncodedAsBytes(
        //   original,
        //   g_dummyColourMap,
        //   g_dummyColourMap,
        //   g_dummyColourMap,
        //   ref colouredPixelArray
        // ) ;
        Helpers.BuildColouredPixelArrayEncodedAsBytes(
          original,
          colourMap,
          ref colouredPixelArray
        ) ;
      }

      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"{imageSize} ; Draw coloured box (thin)",
          WriteMessage
        )
      ) {
        var box = OverlayBoxDescriptor.FromCentrePoint(
          x      : width  / 2,
          y      : height / 2,
          height : width / 4,
          width  : height / 4,
          colour : RgbByteValues.Red,
          thick  : false
        ) ;
        box.Draw(colouredPixelArray!) ;
      }

      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"{imageSize} ; Draw coloured box (thick)",
          WriteMessage
        )
      ) {
        var box = OverlayBoxDescriptor.FromCentrePoint(
          x      : width  / 2,
          y      : height / 2,
          height : width / 4,
          width  : height / 4,
          colour : RgbByteValues.Red,
          thick  : true
        ) ;
        box.Draw(colouredPixelArray!) ;
      }

      using ( 
        var timer = new ExecutionTimerEx_ShowingMillisecsElapsed(
          $"{imageSize} ; Rotate 90, creating a clone",
          () => original.CreateRotatedClone(RotationFactor.RotateClockwise90),
          WriteMessage
        )
      ) {
      }

      using var _ = new ExecutionTimerEx_ShowingMillisecsElapsed(
        $"{imageSize} ; Rotate 90 then mirror, creating a clone",
        () => original.CreateRotatedClone(RotationFactor.RotateClockwise90_ThenMirrorAroundVerticalCentre),
        WriteMessage
      ) ;

    }
  }

}

