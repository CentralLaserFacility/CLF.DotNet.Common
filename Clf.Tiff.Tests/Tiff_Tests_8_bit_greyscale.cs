//
// TiffFile_Tests_8_bit_greyscale.cs
//

using Clf.Common.ImageProcessing ;
using FluentAssertions ;
using Xunit ;

namespace Clf_Tiff_Tests
{

  partial class Tiff_Tests
  {

    [Theory]
    [InlineData("AuPbSn40_8_bit.tif")]
    [InlineData("Cell_Colony_8_bit.tif")]
    [InlineData("Ivry-Gitlis_GREY.tif")]
    public void CanReadExistingGreyscaleTiffFile ( string tiffFileName )
    {
      string imageFilePath = (
        Clf.Common.PathUtilities.RootDirectoryHoldingDotNetGithubRepos
      + @$"DotNet.Common\Clf.Tiff.Tests\Images\{tiffFileName}" 
      ) ;
      Clf.Tiff.Helpers.ReadGreyscaleImageFromTiffFile(
        imageFilePath,
        out int widthReadBack,
        out int heightReadBack,
        out byte[] pixelDataReadBack
      ) ;
      Clf.Tiff.Helpers.GetTiffFileInfo(
        imageFilePath,
        WriteLine
      ) ;
    }

    [Theory]
    [InlineData(512,320)]
    [InlineData(512+3,320)]
    [InlineData(2000,1500,null)]
    [InlineData(2000,1500,Clf.Tiff.CompressionMode.NoCompression)]
    [InlineData(2000,1500,Clf.Tiff.CompressionMode.LZW)]
    [InlineData(2000,1500,Clf.Tiff.CompressionMode.ZIP)]
    public void CanWriteAndReadBackTiffFile_8_bit_greyscale ( 
      int                       width, 
      int                       height, 
      Clf.Tiff.CompressionMode? compressionMode = null 
    ) {
      compressionMode ??= Clf.Tiff.Helpers.CompressionMode_Default ;
      // Create pixel data that increases in brightness
      // along each row, so that we'll be easily able to
      // eyeball the resulting image in a tiff viewer ...
      byte[] eightBitPixelDataToWrite = new byte[width*height] ;
      int iPixel = 0 ;
      for ( int y = 0 ; y < height ; y++ )
      {
        for ( int x = 0 ; x < width ; x++ )
        {
          if ( y < 4  ) 
          {
            // Lines at the 'top' will be black
            // so that we can see them !!
            eightBitPixelDataToWrite[iPixel++] = (byte) 0 ;
          }
          else
          {
            eightBitPixelDataToWrite[iPixel++] = (byte) x ;
          }
        }
      }
      // Write the test image to a TIFF file
      string tiffFilePath = $"{TiffImageFileRootPath}_8_bit_greyscale_{width}x{height}_{compressionMode}.tif" ;
      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"Writing 8 bit TIFF image {width}x{height}_{compressionMode}",
          message => WriteLine(message)
        )
      ) {
        Clf.Tiff.Helpers.WriteGreyscaleImageAsTiffFile(
          width,
          height,
          eightBitPixelDataToWrite,
          tiffFilePath,
          compressionMode
        ) ;
      }
      Clf.Tiff.Helpers.GetTiffFileInfo(
        tiffFilePath,
        WriteLine
      ) ;
      int widthReadBack ;
      int heightReadBack ;
      byte[] pixelDataReadBack ;
      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"Reading 8 bit TIFF image {width}x{height}_{compressionMode}",
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
      //     $"Verifying 8 bit TIFF image {width}x{height}}}_{{compressionMode}} ; {pixelDataReadBack.Length} pixels",
      //     message => WriteLine(message)
      //   )
      // ) {
      //   pixelDataReadBack.Should().BeEquivalentTo(eightBitPixelDataToWrite) ;
      // }
      int nPixelsToVerify = 500 ;
      var pixelIndeces = GetRandomPixelIndices(eightBitPixelDataToWrite.Length,nPixelsToVerify) ;
      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"Verifying subset of 8 bit TIFF image {width}x{height}_{compressionMode} ; {nPixelsToVerify} of {pixelDataReadBack.Length} pixels",
          message => WriteLine(message)
        )
      ) {
        GetPixelsAtIndeces(
          pixelDataReadBack,
          pixelIndeces
        ).Should(
        ).BeEquivalentTo(
          GetPixelsAtIndeces(
            eightBitPixelDataToWrite,
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
        System.Array.Clear(eightBitPixelDataToWrite) ;
        foreach ( int i in pixelIndeces ) 
        {
          eightBitPixelDataToWrite[i] = 0xFF ;
        }
        Clf.Tiff.Helpers.WriteGreyscaleImageAsTiffFile(
          width,
          height,
          eightBitPixelDataToWrite,
          tiffFilePath.Replace(".tif","_testedBits.tif")
        ) ;
      } 
    }
    
  }

}