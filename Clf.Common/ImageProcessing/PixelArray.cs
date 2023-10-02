//
// PixelArray.cs
//

using FluentAssertions ;

namespace Clf.Common.ImageProcessing
{

  //
  // Example :
  //
  //   Width  = 3 ; x is 0..2
  //   Height = 2 ; y is 0..1
  //
  //    +---+---+---+
  //    | 0 | 1 | 2 |  Offset for [x,y] into 1-D array
  //    +---+---+---+
  //    | 3 | 4 | 5 |  is ( y * Width ) + x      
  //    +---+---+---+
  //

  //
  // A PixelArray is the OWNER of the array-of-bytes that it has as a property.
  //

  internal class PixelArray : PixelArrayBase
  { 

    public PixelArray (
      int     width,
      int     height,
      byte[]? imageBytes = null
    ) :
    base(
      width,
      height
    ) {
      m_imageBytesLinearArray = (
         imageBytes
      ?? new byte[
          PixelCount
         ] 
      ) ;
    }

    public static PixelArray CreateEmptyInstance (
      int width,
      int height
    ) {
      PixelArray instance = new PixelArray(width,height) ;
      return instance ;
    }

    public static PixelArray CreateInstance_TakingOwnershipOfImageBytes (
      int     width,
      int     height,
      byte[]? imageBytes
    ) {
      PixelArray instance = new PixelArray(width,height,imageBytes) ;
      return instance ;
    }

    // public void LoadImageData (
    //   int    width,
    //   int    height,
    //   byte[] imageBytes
    // ) {
    //   if ( 
    //      width  != Width
    //   || height != Height
    //   ) {
    //     // The new image is not the same size
    //     // as the existing one
    //     Width  = width ;
    //     Height = height ;
    //   }
    //   imageBytes.Length.Should().Be(PixelCount) ;
    //   m_imageBytesLinearArray = imageBytes ;
    // }

    public void SetPixel ( 
      int  x, 
      int  y, 
      byte pixelValue
    ) {
      int pixelOffset = (
        x
      + y * Width
      ) ;
      if ( 
         pixelOffset < 0 
      || pixelOffset > MaxPixelIndex
      ) {
        // The pixel we'd write is outside
        // the bounds of the pixel array ...
        return ;
      }
      ImageBytesLinearArray[pixelOffset] = pixelValue ;
    }

    public override byte? GetPixel ( 
      int x, 
      int y
    ) {
      int pixelOffset = (
        x
      + y * Width
      ) ;
      if ( 
         pixelOffset < 0 
      || pixelOffset > MaxPixelIndex
      ) {
        // The pixel we'd read is outside
        // the bounds of the pixel array ...
        return null ;
      }
      return ImageBytesLinearArray[pixelOffset] ;
    }

  }

}

