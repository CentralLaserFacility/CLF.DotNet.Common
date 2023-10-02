//
// PixelArrayBase.cs
//

using System.Linq;
using FluentAssertions;

namespace Clf.Common.ImageProcessing
{

  // Extract 'region-of-interest' ???
  // Pixels might not be 'byte' values !!!

  //
  // Hmm, maybe we need 'GetPixelOrNull' (or 'CanGetPixel()') for cases
  // where the x and y we supply might represent an out-of-bounds pixel,
  // and make 'GetPixel' assume that we'll always return a valid pixel value.
  //

  //
  // Naming convention ?
  //   cloned  = original.CreateClone_Resized() ;
  //   mutated = original.PerformResize() ;
  //

  internal abstract class PixelArrayBase
  { 

    public int Width { get ; protected set ; }

    public int Height { get ; protected set ;  }

    // Hmm, maybe this should be a class,
    // encapsulating Width and Height ???

    protected byte[]? m_imageBytesLinearArray = null ;

    // We can *always* access the linear array of image bytes,
    // because if it hasn't yet been created, we'll build it ...

    public byte[] ImageBytesLinearArray 
    => m_imageBytesLinearArray ??= BuildImageBytesLinearArray() ;

    protected PixelArrayBase (
      int width,
      int height
    ) {
      Width  = width ;
      Height = height ;
    }

    public PixelArray CreateClone ( )
    => new PixelArray(
      this.Width, 
      this.Height,
      Helpers.CreateCloneOfByteArray(
        this.ImageBytesLinearArray
      )
    ) ;

    private byte[] BuildImageBytesLinearArray ( )
    {
      // Build the entire array of image bytes
      // by evaluating each [x,y] pixel ...
      var imageBytesLinearArray = new byte[PixelCount] ;
      int iPixel = 0 ;
      for ( int y = 0 ; y < Height ; y++ )
      {
        for ( int x = 0 ; x < Width ; x++ )
        {
          byte? pixelValue = GetPixel(x,y) ;
          // Since we're only evaluating the pixel value at coordinates
          // that lie within the proper range of the image,
          // we know that the returned value will be non null ...
          pixelValue.Should().NotBeNull() ;
          imageBytesLinearArray[iPixel++] = pixelValue!.Value ;
        }
      }
      return imageBytesLinearArray ;
    }

    public abstract byte? GetPixel ( 
      int x, 
      int y
    ) ;

    public byte? this [ int x, int y ] => GetPixel(x,y) ;

    public int PixelCount => Width * Height ;

    public int MaxPixelIndex => PixelCount - 1 ;

    public byte[]? GetRowOfPixelsAtOffsetFromTop ( int y ) 
    {
      if ( y < Height ) 
      {
        byte[] rowOfPixels = new byte[Width] ;
        System.Array.Copy(
          sourceArray      : ImageBytesLinearArray,
          sourceIndex      : y * Width,
          destinationArray : rowOfPixels,
          destinationIndex : 0,
          length           : Width
        ) ;
        return rowOfPixels ;
      }
      else
      {
        return null ;
      }
    }

    public byte[]? GetColumnOfPixelsAtOffsetFromLeft ( int x ) 
    {
      if ( x < Width ) 
      {
        byte[] columnOfPixels = new byte[Height] ;
        for ( int y = 0 ; y < Height ; y++ )
        {
          columnOfPixels[y] = ImageBytesLinearArray[
            x 
          + y * Width
          ] ;
        }
        return columnOfPixels ;
      }
      else
      {
        return null ;
      }
    }

    public RotatedPixelArray CreateRotatedClone ( RotationFactor rotationFactor )
    => new RotatedPixelArray(
      this, 
      rotationFactor
    ) ;

    public PixelArrayBase CreateNormalisedClone ( byte? normalisationValue = null )
    {
      if ( normalisationValue == null )
      {
        normalisationValue = ImageBytesLinearArray.Max() ;
      }
      double referenceValue = normalisationValue.Value ;
      if ( referenceValue == 0 )
      {
        // Need to prevent a later divide-by-zero error !!!
        referenceValue = 1 ;
      }

      var normalizedByteArray = new byte[this.PixelCount] ;
      for ( int i = 0; i < this.PixelCount ; i++ )
      {
        byte value = ImageBytesLinearArray[i] ;
        double fractionalValue = value / referenceValue ;
        if ( fractionalValue >= 0.99 )
        {
          fractionalValue = 0.99 ;
        }
        normalizedByteArray[i] = (byte) ( fractionalValue * 255 ) ;
      }
      return new PixelArray(
        this.Width, 
        this.Height,
        normalizedByteArray
      ) ;
    }

    //
    // This creates a new instance, even if the size is the same ...
    //

    public PixelArrayBase CreateResizedClone_NearestNeighbour ( 
      int resizedWidthX, 
      int resizedHeightY
    ) {
      if ( 
         resizedWidthX  == Width
      && resizedHeightY == Height
      ) {
        return this.CreateClone() ;
      }
      else
      { 
        // The 'resized' dimensions are different from the sizes
        // of the PixelArray that we've already allocated,
        // so we'll allocate a new PixelArray of the required dimensions
        var resizedPixelArray = new PixelArray(
          resizedWidthX,
          resizedHeightY
        ) ;
        System.Diagnostics.Debug.WriteLine(
          $"PixelArray has been reallocated ({resizedPixelArray.Width} x {resizedPixelArray.Height}) "
        ) ;
        resizedPixelArray.Width.Should().Be(resizedWidthX) ;
        resizedPixelArray.Height.Should().Be(resizedHeightY) ;
        byte[] resizedImageBytes = resizedPixelArray.ImageBytesLinearArray ;
        // Compute a 'resized' image using 'nearest-neighbour' interpolation.
        // If we're computing a reduced-size image, where the resized width
        // is say 0.5 times the original image width, then our 'xStepFactor'
        // will be 2.0 ; that is, each 'resized' pixel will come from an 
        // original pixel at a position of x-times-xStepFactor, ie we skip
        // over some of the original pixels.
        // If we're computing a larger image, where the resized width
        // is say 2 times the original image width, then our 'xStepFactor'
        // will be 0.5 ; that is, each 'resized' pixel will come from an 
        // original pixel at a position of x-times-xStepFactor, ie we'll
        // repeat some of the original pixels when creating the new image.
        // Another way of thinking about this is that the 'step factor'
        // represents the amount by which the size is being *reduced*.
        // For a step factor of 2, the size will be reduced by a factor of 2.
        double xStepFactor = ( (double) this.Width  ) / resizedWidthX ;
        double yStepFactor = ( (double) this.Height ) / resizedHeightY ;
        int iResized = 0 ;
        for ( int yResized = 0 ; yResized < resizedHeightY ; yResized++ )
        {
          // Step along visiting each 'resized' pixel on the Y axis
          int originalIndexY = (int) ( yResized * yStepFactor ) ;
          for ( int xResized = 0 ; xResized < resizedWidthX ; xResized++ )
          {
            // Step along visiting each 'resized' pixel on the X axis
            int originalIndexX = (int) ( xResized * xStepFactor ) ;
            // Get the 'original' pixel value
            byte originalPixel = this.ImageBytesLinearArray[
              originalIndexX 
            + originalIndexY * this.Width
            ] ;
            // Write the 'resized' pixel value
            resizedImageBytes[
              iResized++
            //   xResized 
            // + yResized * resizedWidthX
            ] = originalPixel ;
          }
        }
        return resizedPixelArray ;
      }
    }

    private PixelArrayBase Resized_NearestNeighbour_old_01 ( 
      int resizedWidthX, 
      int resizedHeightY
    ) {
      if ( 
         resizedWidthX  == Width
      && resizedHeightY == Height
      ) {
        // Hmm, in this case we're not creating a new instance.
        // BUT MAYBE WE SHOULD RETURN A NEW INSTANCE WHOSE VALUE ARRAY
        // IS THE SAME AS THE ORIGINAL ???
        return this ;
      }
      else
      { 
        // The 'resized' dimensions are different from the sizes
        // of the PixelArray that we've already allocated,
        // so we'll allocate a new PixelArray of the required dimensions
        var resizedPixelArray = new PixelArray(
          resizedWidthX,
          resizedHeightY
        ) ;
        System.Diagnostics.Debug.WriteLine(
          $"PixelArray has been reallocated ({resizedPixelArray.Width} x {resizedPixelArray.Height}) "
        ) ;
        resizedPixelArray.Width.Should().Be(resizedWidthX) ;
        resizedPixelArray.Height.Should().Be(resizedHeightY) ;
        byte[] resizedImageBytes = resizedPixelArray.ImageBytesLinearArray ;
        // Compute a 'resized' image using 'nearest-neighbour' interpolation.
        // If we're computing a reduced-size image, where the resized width
        // is say 0.5 times the original image width, then our 'xStepFactor'
        // will be 2.0 ; that is, each 'resized' pixel will come from an 
        // original pixel at a position of x-times-xStepFactor, ie we skip
        // over some of the original pixels.
        // If we're computing a larger image, where the resized width
        // is say 2 times the original image width, then our 'xStepFactor'
        // will be 0.5 ; that is, each 'resized' pixel will come from an 
        // original pixel at a position of x-times-xStepFactor, ie we'll
        // repeat some of the original pixels when creating the new image.
        // Another way of thinking about this is that the 'step factor'
        // represents the amount by which the size is being *reduced*.
        // For a step factor of 2, the size will be reduced by a factor of 2.
        double xStepFactor = ( (double) this.Width  ) / resizedWidthX ;
        double yStepFactor = ( (double) this.Height ) / resizedHeightY ;
        int iResized = 0 ;
        for ( int yResized = 0 ; yResized < resizedHeightY ; yResized++ )
        {
          // Step along visiting each 'resized' pixel on the Y axis
          int originalIndexY = (int) ( yResized * yStepFactor ) ;
          for ( int xResized = 0 ; xResized < resizedWidthX ; xResized++ )
          {
            // Step along visiting each 'resized' pixel on the X axis
            int originalIndexX = (int) ( xResized * xStepFactor ) ;
            // Get the 'original' pixel value
            byte originalPixel = this.ImageBytesLinearArray[
              originalIndexX 
            + originalIndexY * this.Width
            ] ;
            // Write the 'resized' pixel value
            resizedImageBytes[
              iResized++
            //   xResized 
            // + yResized * resizedWidthX
            ] = originalPixel ;
          }
        }
        return resizedPixelArray ;
      }
    }

  }

}

