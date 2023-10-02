//
// Tiff_Tests_16_bit_greyscale.cs
//

using Clf.Common.ImageProcessing ;
using FluentAssertions ;
using Xunit ;

namespace Clf_Tiff_Tests
{

  partial class Tiff_Tests
  {

    [Theory]
    [InlineData(512,320)]
    [InlineData(512+1,320)]
    [InlineData(512+3,320)]
    [InlineData(2000,1500,null)]
    [InlineData(2000,1500,Clf.Tiff.CompressionMode.NoCompression)]
    [InlineData(2000,1500,Clf.Tiff.CompressionMode.LZW)]
    [InlineData(2000,1500,Clf.Tiff.CompressionMode.ZIP)]
    public void CanWriteAndReadBackTiffFile_16_bit ( 
      int                       width, 
      int                       height, 
      Clf.Tiff.CompressionMode? compressionMode = null 
    ) {
      compressionMode ??= Clf.Tiff.Helpers.CompressionMode_Default ;
      // Create pixel data that increases in brightness
      // along each row, so that we'll be easily able to
      // eyeball the resulting image in a tiff viewer ...
      ushort[] sixteenBitPixelDataToWrite = new ushort[width*height] ;
      int iPixel = 0 ;
      for ( int y = 0 ; y < height ; y++ )
      {
        for ( int x = 0 ; x < width ; x++ )
        {
          if ( y < 4  ) 
          {
            // Lines at the 'top' will be black
            // so that we can see them, and verify that the image
            // is the right way up !!
            sixteenBitPixelDataToWrite[iPixel++] = (ushort) 0 ;
          }
          else
          {
            sixteenBitPixelDataToWrite[iPixel++] = (ushort) ( x << 8 ) ;
          }
        }
      }
      // Write the test image to a TIFF file
      string tiffFilePath = $"{TiffImageFileRootPath}_16_bit_greyscale_{width}x{height}_{compressionMode}.tif" ;
      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"Writing 16 bit TIFF image {width}x{height}",
          message => WriteLine(message)
        )
      ) {
        Clf.Tiff.Helpers.WriteGreyscaleImageAsTiffFile(
          width,
          height,
          sixteenBitPixelDataToWrite,
          tiffFilePath,
          compressionMode
        ) ;
      }
      Clf.Tiff.Helpers.GetTiffFileInfo(
        tiffFilePath,
        WriteLine
      ) ;
      // Make sure that we can recreate an equivalent iamge from the TIFF file
      int widthReadBack ;
      int heightReadBack ;
      ushort[] pixelDataReadBack ;
      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"Reading 16 bit TIFF image {width}x{height}_{compressionMode}",
          message => WriteLine(message)
        )
      ) {
        // Make sure that we can recreate an equivalent bitmap from the TIFF file
        Clf.Tiff.Helpers.ReadGreyscaleImageFromTiffFile(
          tiffFilePath,
          out widthReadBack,
          out heightReadBack,
          out pixelDataReadBack
        ) ;
      }
      widthReadBack.Should().Be(width) ;
      heightReadBack.Should().Be(height) ;
      // FluentAssertions take upwards of TEN SECONDS to compare
      // all the 163,000 pixels - so instead of comparing each and every one,
      // we'll just compare a smaller number of pixels chosen at random
      // using ( 
      //   var timer = new ExecutionTimer_ShowingMillisecsElapsed(
      //     $"Verifying 16 bit TIFF image {width}x{height} ; {pixelDataReadBack.Length} pixels",
      //     message => m_output?.WriteLine(message)
      //   )
      // ) {
      //   pixelDataReadBack.Should().BeEquivalentTo(sixteenBitPixelDataToWrite) ;
      // }
      int nPixelsToVerify = 500 ;
      var pixelIndeces = GetRandomPixelIndices(
        sixteenBitPixelDataToWrite.Length,
        nPixelsToVerify
      ) ;
      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"Verifying subset of 16 bit TIFF image {width}x{height}_{compressionMode} ; {nPixelsToVerify} of {pixelDataReadBack.Length} pixels",
          message => WriteLine(message)
        )
      ) {
        GetPixelsAtIndeces(
          pixelDataReadBack,
          pixelIndeces
        ).Should(
        ).BeEquivalentTo(
          GetPixelsAtIndeces(
            sixteenBitPixelDataToWrite,
            pixelIndeces
          )
        ) ;
      }
      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"Creating TIFF image {width}x{height}_{compressionMode} showing {nPixelsToVerify} tested pixels",
          message => WriteLine(message)
        )
      ) {
        // Clear to black, then set the 'tested' pixels to white
        System.Array.Clear(sixteenBitPixelDataToWrite) ;
        foreach ( int i in pixelIndeces ) 
        {
          sixteenBitPixelDataToWrite[i] = (ushort) 0xFFFF ;
        }
        Clf.Tiff.Helpers.WriteGreyscaleImageAsTiffFile(
          width,
          height,
          sixteenBitPixelDataToWrite,
          tiffFilePath.Replace(".tif","_testedBits.tif")
        ) ;
      } 
    }

  }

}