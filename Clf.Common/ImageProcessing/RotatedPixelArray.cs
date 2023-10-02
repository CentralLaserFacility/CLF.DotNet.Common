//
// RotatedPixelArray.cs
//

using FluentAssertions ;

namespace Clf.Common.ImageProcessing
{

  //
  // Hmm, maybe need to revisit this design ... is the
  // inheritance from a base class useful ???
  // 
  // We're thinking of the 'source' as being immutable,
  // but it's not necessarily useful to enforce that
  // as we might want to 'normalise' the pixels in place,
  // avoiding unnecessary array allocations ?
  //

  //
  // Alternative approach :
  //   Keep the 'original' pixels in their original array,
  //   and build a mapping matrix where each [x',y'] entry
  //   is a tuple [x,y] that tells you the where to get the
  //   mapped pixel in the original array.
  //

  internal class RotatedPixelArray : PixelArrayBase
  { 

    public PixelArrayBase SourcePixelArray { get ; }

    public RotationFactor RotationFactor { get ; }
    
    public RotatedPixelArray (
      PixelArrayBase sourcePixelArray,
      RotationFactor rotationFactor
    ) :
    base(
      ComputeRotatedWidth(sourcePixelArray,rotationFactor), 
      ComputeRotatedHeight(sourcePixelArray,rotationFactor)
    ) {
      SourcePixelArray = sourcePixelArray ;
      RotationFactor   = rotationFactor ;
      if ( RotationFactor is RotationFactor.None )
      {
        // Our 'rotated' instance has the same pixel arrangement
        // as the source, but let's be careful to allocate a new instance
        // of the array, as we'll do for actually-rotated variants.
        // By allocating the clone here, we avoid the time consuming operation
        // of remapping the pixels, which in this case would be the
        // trivial mapping [x,y] => [x,y]
        m_imageBytesLinearArray = Helpers.CreateCloneOfByteArray(
          sourcePixelArray.ImageBytesLinearArray
        ) ;
      }
    }

    private static int ComputeRotatedWidth ( 
      PixelArrayBase basePixelArray, 
      RotationFactor rotationFactor 
    ) => rotationFactor switch {
      // For these rotation factors, we swap the width and height ...
      RotationFactor.RotateClockwise90                                 => basePixelArray.Height,
      RotationFactor.RotateClockwise90_ThenMirrorAroundVerticalCentre  => basePixelArray.Height,
      RotationFactor.RotateClockwise270                                => basePixelArray.Height,
      _                                                                => basePixelArray.Width
    } ;

    private static int ComputeRotatedHeight ( 
      PixelArrayBase sourcePixelArray, 
      RotationFactor rotationFactor 
    ) => rotationFactor switch {
      // For these rotation factors, we swap the width and height ...
      RotationFactor.RotateClockwise90                                => sourcePixelArray.Width,
      RotationFactor.RotateClockwise90_ThenMirrorAroundVerticalCentre => sourcePixelArray.Width,
      RotationFactor.RotateClockwise270                               => sourcePixelArray.Width,
      _                                                               => sourcePixelArray.Height
    } ;

    public override byte? GetPixel ( 
      int x, 
      int y
    ) => (
      // If we've already calculated the 'rotated' pixel values
      // and populated the 'linear array', then we can subsequently
      // extract the required pixel from that array. Otherwise,
      // we'll compute the required value from the 'rotation factor'
      // and the 'source' PixelArray.
      m_imageBytesLinearArray == null
      ? ComputeRotatedPixel(x,y)
      : GetPixelOrNull(
          m_imageBytesLinearArray,
          Width,
          Height,
          x,
          y
        )
    ) ;

    private static byte? GetPixelOrNull ( 
      byte[] pixelsArray,
      int    width,
      int    height,
      int    x, 
      int    y
    ) {
      int pixelOffset = (
        x
      + y * width
      ) ;
      if ( 
         pixelOffset < 0 
      || pixelOffset >= pixelsArray.Length
      ) {
        // The pixel we'd read is outside
        // the bounds of the pixel array ...
        return null ;
      }
      return pixelsArray[pixelOffset] ;
    }

    private byte? ComputeRotatedPixel ( 
      int x, 
      int y
    ) => RotationFactor switch {
    RotationFactor.None => (
      //
      //  0 1 2   =>   0 1 2
      //  3 4 5        3 4 5
      //
      SourcePixelArray.GetPixel(
        x,
        y
      )
    ),
    RotationFactor.RotateClockwise90 => (
      //
      //  0 1 2   =>   3 0
      //  3 4 5        4 1
      //               5 2
      //
      // Consider how we obtain the pixels for the new 'top' row, where y'=0.
      // As x' goes from 0 to Width-1 (ie 0..1), we select the pixels
      // that used to reside in the left hand column, in reverse order.
      // That is, we access original pixels [0,1] then [0,0], ie values 3,0.
      //
      SourcePixelArray.GetPixel(
        y,
        Width-1-x
      )
    ),
    RotationFactor.RotateClockwise90_ThenMirrorAroundVerticalCentre => (
      //
      //  0 1 2   =>   3 0   =>   0 3
      //  3 4 5        4 1        1 4
      //               5 2        2 5
      //
      // Consider how we obtain the pixels for the new 'top' row, where y'=0.
      // As x' goes from 0 to Width-1 (ie 0..1), we select the pixels
      // that used to reside in the left hand column, in original order.
      // That is, we access original pixels [0,0] then [0,1], ie values 0,3.
      //
      SourcePixelArray.GetPixel(
        y,
        x
      )
    ),
    RotationFactor.RotateClockwise180 => (
      //
      //  0 1 2   =>   3 0    =>   5 4 3
      //  3 4 5        4 1         2 1 0
      //               5 2
      //
      // Consider how we obtain the pixels for the new 'top' row, where y'=0.
      // As x' goes from 0 to Width-1 (ie 0..1..2), we select the pixels
      // that used to reside in the 'bottom' row, in reverse order,
      // ie values 5,4,3.
      //
      SourcePixelArray.GetPixel(
        Width-1-x,
        Height-1-y
      )
    ),
    RotationFactor.RotateClockwise270 => (
      //
      //  0 1 2   =>   3 0    =>   5 4 3  =>   2 5
      //  3 4 5        4 1         2 1 0       1 4
      //               5 2                     0 3
      //
      // Consider how we obtain the pixels for the new 'top' row, where y'=0.
      // As x' goes from 0 to Width-1 (ie 0..1), we select the pixels
      // that used to reside in the right hand column, in reverse order.
      // That is, we access original pixels [2,0] then [1,0], ie values 2,5.
      //
      SourcePixelArray.GetPixel(
        Height-1-y, 
        x
      )
    ),
    RotationFactor.MirrorAroundVerticalCentre => (
      //
      //  0 1 2   =>   2 1 0
      //  3 4 5        5 4 3
      //                    
      // Consider how we obtain the pixels for the new 'top' row, where y'=0.
      // As x' goes from 0 to Width-1 (ie 0..1..2), we select the pixels
      // that used to reside in that 'top' row, in reverse order,
      // ie values 2,1,0.
      //
      SourcePixelArray.GetPixel(
        Width-1-x,
        y
      )
    ),
    RotationFactor.MirrorAroundHorizontalCentre => (
      //
      //  0 1 2   =>   3 4 5
      //  3 4 5        0 1 2
      //                    
      // Consider how we obtain the pixels for the new 'top' row, where y'=0.
      // As x' goes from 0 to Width-1 (ie 0..1..2), we select the pixels
      // that used to reside in the 'bottom' row, in original order,
      // ie values 2,1,0.
      //
      SourcePixelArray.GetPixel(
        x,
        Height-1-y
      )
    ),
    _ => throw new System.ApplicationException()
    } ;

    public static void RunSimpleTest ( )
    {
      PixelArray original = new PixelArray(3,2,new byte[]{0,1,2,3,4,5}) ;
      string a  = $"{original[0,0]} {original[1,0]} {original[2,0]}" ;
      string b  = $"{original[0,1]} {original[1,1]} {original[2,1]}" ;
      original.ImageBytesLinearArray.Should().BeEquivalentTo(new byte[]{0,1,2,3,4,5}) ;
      {
        original.GetRowOfPixelsAtOffsetFromTop(1).Should().BeEquivalentTo(new byte[]{3,4,5}) ;
        original.GetColumnOfPixelsAtOffsetFromLeft(1).Should().BeEquivalentTo(new byte[]{1,4}) ;
      }
      {
        var rotated = new RotatedPixelArray(original,RotationFactor.RotateClockwise90) ;
        string aa = $"{rotated[0,0]} {rotated[1,0]} ; expected 3 0" ;
        string bb = $"{rotated[0,1]} {rotated[1,1]} ; expected 4 1" ;
        string cc = $"{rotated[0,2]} {rotated[1,2]} ; expected 5 2" ;
        rotated.ImageBytesLinearArray.Should().BeEquivalentTo(new byte[]{3,0,4,1,5,2}) ;
      }
      {
        var rotated = new RotatedPixelArray(original,RotationFactor.RotateClockwise180) ;
        string aa = $"{rotated[0,0]} {rotated[1,0]} {rotated[2,0]} ; expected 5 4 3" ;
        string bb = $"{rotated[0,1]} {rotated[1,1]} {rotated[2,1]} ; expected 2 1 0" ;
        rotated.ImageBytesLinearArray.Should().BeEquivalentTo(new byte[]{5,4,3,2,1,0}) ;
      }
      {
        var rotated = new RotatedPixelArray(original,RotationFactor.RotateClockwise270) ;
        string aa = $"{rotated[0,0]} {rotated[1,0]} ; expected 2 5" ;
        string bb = $"{rotated[0,1]} {rotated[1,1]} ; expected 1 4" ;
        string cc = $"{rotated[0,2]} {rotated[1,2]} ; expected 0 3" ;
        rotated.ImageBytesLinearArray.Should().BeEquivalentTo(new byte[]{2,5,1,4,0,3}) ;
      }
      {
        var mirrored = new RotatedPixelArray(original,RotationFactor.MirrorAroundVerticalCentre) ;
        string aa = $"{mirrored[0,0]} {mirrored[1,0]} {mirrored[2,0]} ; expected 2 1 0" ;
        string bb = $"{mirrored[0,1]} {mirrored[1,1]} {mirrored[2,1]} ; expected 5 4 3" ;
        mirrored.ImageBytesLinearArray.Should().BeEquivalentTo(new byte[]{2,1,0,5,4,3}) ;
      }
      {
        var mirrored = new RotatedPixelArray(original,RotationFactor.MirrorAroundHorizontalCentre) ;
        string aa = $"{mirrored[0,0]} {mirrored[1,0]} {mirrored[2,0]} ; expected 3 4 5" ;
        string bb = $"{mirrored[0,1]} {mirrored[1,1]} {mirrored[2,1]} ; expected 0 1 2" ;
        mirrored.ImageBytesLinearArray.Should().BeEquivalentTo(new byte[]{3,4,5,0,1,2}) ;
      }
      {
        var rotatedThenMirrored = new RotatedPixelArray(original,RotationFactor.RotateClockwise90_ThenMirrorAroundVerticalCentre) ;
        string aa = $"{rotatedThenMirrored[0,0]} {rotatedThenMirrored[1,0]} ; expected 0 3" ;
        string bb = $"{rotatedThenMirrored[0,1]} {rotatedThenMirrored[1,1]} ; expected 1 4" ;
        string cc = $"{rotatedThenMirrored[0,2]} {rotatedThenMirrored[1,2]} ; expected 2 5" ;
        rotatedThenMirrored.ImageBytesLinearArray.Should().BeEquivalentTo(new byte[]{0,3,1,4,2,5}) ;
      }
      {
        original.CreateRotatedClone(
          RotationFactor.RotateClockwise90
        ).CreateRotatedClone(
          RotationFactor.RotateClockwise90
        ).ImageBytesLinearArray.Should().BeEquivalentTo(
            original.CreateRotatedClone(
            RotationFactor.RotateClockwise180
          ).ImageBytesLinearArray 
        ) ;
      }
    }

  }

}

