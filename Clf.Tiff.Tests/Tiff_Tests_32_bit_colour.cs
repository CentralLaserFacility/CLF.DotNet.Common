//
// TiffFile_Tests_32_bit_colour.cs
//

using Clf.Common.ImageProcessing ;
using FluentAssertions ;
using Xunit ;

namespace Clf_Tiff_Tests
{

  partial class Tiff_Tests
  {

    [Theory]
    [InlineData("Ivry-Gitlis.tif")]
    public void CanReadAndWriteBackColourTiffFile ( string tiffFileName )
    {
      string imageFilePath = (
        Clf.Common.PathUtilities.RootDirectoryHoldingDotNetGithubRepos
      + @$"DotNet.Common\Clf.Tiff.Tests\Images\{tiffFileName}" 
      ) ;
      Clf.Tiff.Helpers.ReadColourImageFromTiffFile(
        imageFilePath,
        out int   widthReadBack,
        out int   heightReadBack,
        out uint[] pixelDataReadBack
      ) ;
      Clf.Tiff.Helpers.GetTiffFileInfo(
        imageFilePath,
        WriteLine
      ) ;
      Clf.Tiff.Helpers.WriteColourImageAsTiffFile(
        widthReadBack,
        heightReadBack,
        pixelDataReadBack,
        $"{TiffImageFileRootPath}_ColourTiff_written_{tiffFileName}"
      ) ;
      Clf.Tiff.Helpers.GetTiffFileInfo(
        $"{TiffImageFileRootPath}_ColourTiff_written_{tiffFileName}",
        WriteLine
      ) ;
    }

    [Theory]
    [InlineData(30,20,0x0000FF)]
    [InlineData(30,20,0x00FF00)]
    [InlineData(30,20,0xFF0000)]
    [InlineData(30,20,0xFFFF00)]
    [InlineData(30,20,0x00FFFF)]
    [InlineData(30,20,0x808080)]
    [InlineData(30,20,0x800080,Clf.Tiff.CompressionMode.NoCompression)]
    [InlineData(30,20,0x808000,Clf.Tiff.CompressionMode.LZW)]
    [InlineData(30,20,0x008080,Clf.Tiff.CompressionMode.ZIP)]
    [InlineData(30,20,0x008080,Clf.Tiff.CompressionMode.JPEG)]
    public void CanWriteAndReadBackColourTiffFile ( 
      int                       width, 
      int                       height, 
      uint                      colour, 
      Clf.Tiff.CompressionMode? compressionMode = null
    ) {
      compressionMode ??= Clf.Tiff.Helpers.CompressionMode_Default ;
      uint[] colourPixelDataToWrite = new uint[width*height] ;
      int iPixel = 0 ;
      for ( int y = 0 ; y < height ; y++ )
      {
        for ( int x = 0 ; x < width ; x++ )
        {
          // The most significant byte is 0xFF
          // representing an 'alpha' opacity value
          // of maximum opacity.
          // Lines at the 'top' will be black
          // so that we can see them and confirm that the
          // image is the right way up !!
          colourPixelDataToWrite[iPixel++] = (uint) (
            0xFF000000
          | (
              y < 4
              ? 0
              : colour
            )
          ) ;
        }
      }
      // Write the test image to a TIFF file
      string tiffFilePath = $"{TiffImageFileRootPath}_colour_{width}x{height}_{colour:X6}_{compressionMode}.tif" ;
      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"Writing colour TIFF image {width}x{height}_{colour:X6}_{compressionMode}",
          message => WriteLine(message)
        )
      ) {
        Clf.Tiff.Helpers.WriteColourImageAsTiffFile(
          width,
          height,
          colourPixelDataToWrite,
          tiffFilePath,
          compressionMode
        ) ;
      }
      Clf.Tiff.Helpers.GetTiffFileInfo(
        tiffFilePath,
        WriteLine
      ) ;
      // Make sure that we can recreate an equivalent bitmap from the TIFF file
      int widthReadBack ;
      int heightReadBack ;
      uint[] colourPixelsArrayReadBack ;
      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"Reading colour TIFF image {width}x{height}_{compressionMode}",
          message => WriteLine(message)
        )
      ) {
        Clf.Tiff.Helpers.ReadColourImageFromTiffFile(
          tiffFilePath,
          out widthReadBack,
          out heightReadBack,
          out colourPixelsArrayReadBack
        ) ;
      }
      widthReadBack.Should().Be(width) ;
      heightReadBack.Should().Be(height) ;
      if ( compressionMode == Clf.Tiff.CompressionMode.JPEG ) 
      {
        // Don't check for exact equality of the read-back pixels,
        // as JPEG compression isn't lossless and won't give us back
        // exactly the same pixel values ...
        return ;
      }
      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"Verifying colour TIFF image {width}x{height}_{compressionMode} ; {colourPixelsArrayReadBack.Length} pixels",
          message => m_output?.WriteLine(message)
        )
      ) {
        colourPixelsArrayReadBack.Should().BeEquivalentTo(colourPixelDataToWrite) ;
      }
    }


    [Theory]
    [InlineData(2000,1500,Clf.Tiff.CompressionMode.NoCompression)]
    [InlineData(2000,1500,Clf.Tiff.CompressionMode.LZW)]
    [InlineData(2000,1500,Clf.Tiff.CompressionMode.ZIP)]
    [InlineData(2000,1500,Clf.Tiff.CompressionMode.JPEG)]
    public void CanWriteAndReadBackLargeRandomColourTiffFile ( 
      int                      width, 
      int                      height, 
      Clf.Tiff.CompressionMode compressionMode
    ) {
      uint[] colourPixelDataToWrite = new uint[width*height] ;
      int iPixel = 0 ;
      for ( int y = 0 ; y < height ; y++ )
      {
        for ( int x = 0 ; x < width ; x++ )
        {
          // The most significant byte is 0xFF
          // representing an 'alpha' opacity value
          // of maximum opacity.
          colourPixelDataToWrite[iPixel++] = (uint) (
            0xFF000000
          | System.Random.Shared.Next(0,0xFFFFFF)
          ) ;
        }
      }
      // Write the test image to a TIFF file
      string tiffFilePath = $"{TiffImageFileRootPath}_colour_{width}x{height}_random_{compressionMode}.tif" ;
      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"Writing colour TIFF image {width}x{height}_random_{compressionMode}",
          message => WriteLine(message)
        )
      ) {
        Clf.Tiff.Helpers.WriteColourImageAsTiffFile(
          width,
          height,
          colourPixelDataToWrite,
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
      uint[] colourPixelsArrayReadBack ;
      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"Reading colour TIFF image {width}x{height}_random_{compressionMode}",
          message => WriteLine(message)
        )
      ) {
        Clf.Tiff.Helpers.ReadColourImageFromTiffFile(
          tiffFilePath,
          out widthReadBack,
          out heightReadBack,
          out colourPixelsArrayReadBack
        ) ;
      }
      widthReadBack.Should().Be(width) ;
      heightReadBack.Should().Be(height) ;
    }

    [Theory]
    [InlineData(2000,1500,0xFF0000,Clf.Tiff.CompressionMode.NoCompression)]
    [InlineData(2000,1500,0xFF0000,Clf.Tiff.CompressionMode.LZW)]
    [InlineData(2000,1500,0xFF0000,Clf.Tiff.CompressionMode.ZIP)]
    [InlineData(2000,1500,0xFF0000,Clf.Tiff.CompressionMode.JPEG)]
    public void CanWriteAndReadBackLargeColourTiffFile ( 
      int                      width, 
      int                      height, 
      uint                     colour, 
      Clf.Tiff.CompressionMode compressionMode
    ) {
      uint[] colourPixelDataToWrite = new uint[width*height] ;
      int iPixel = 0 ;
      for ( int y = 0 ; y < height ; y++ )
      {
        for ( int x = 0 ; x < width ; x++ )
        {
          // The most significant byte is 0xFF
          // representing an 'alpha' opacity value
          // of maximum opacity.
          colourPixelDataToWrite[iPixel++] = (uint) (
            0xFF000000
          | colour
          ) ;
        }
      }
      // Write the test image to a TIFF file
      string tiffFilePath = $"{TiffImageFileRootPath}_colour_{width}x{height}_{colour:X6}_{compressionMode}.tif" ;
      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"Writing colour TIFF image {width}x{height}_{colour:X6}_{compressionMode}",
          message => WriteLine(message)
        )
      ) {
        Clf.Tiff.Helpers.WriteColourImageAsTiffFile(
          width,
          height,
          colourPixelDataToWrite,
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
      uint[] colourPixelsArrayReadBack ;
      using ( 
        var timer = new ExecutionTimer_ShowingMillisecsElapsed(
          $"Reading colour TIFF image {width}x{height}_{compressionMode}",
          message => WriteLine(message)
        )
      ) {
        Clf.Tiff.Helpers.ReadColourImageFromTiffFile(
          tiffFilePath,
          out widthReadBack,
          out heightReadBack,
          out colourPixelsArrayReadBack
        ) ;
      }
      widthReadBack.Should().Be(width) ;
      heightReadBack.Should().Be(height) ;
    }

  }

}