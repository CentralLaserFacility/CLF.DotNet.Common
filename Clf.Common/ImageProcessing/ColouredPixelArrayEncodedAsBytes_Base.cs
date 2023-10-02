//
// ColouredPixelArrayEncodedAsBytes.cs
//

using FluentAssertions ;
using System.Diagnostics.CodeAnalysis ;

namespace Clf.Common.ImageProcessing
{

  //
  // This abstraction represents a rectangular collection of 'coloured' pixels,
  // width-times-height, where the pixel values are physically represented
  // as sequences of 4 bytes (R,G,B,A) at appropriate offsets within a
  // byte-array of appropriate size, which will be passed to the DOM Canvas.
  //
  //  Offsets into the byte-array passed to the DOM Canvas ...
  //
  //  | 000 | 001 | 002 | 003 | 004 | 005 | 006 | 007 | 008 | 009 | 010 | 011 | 
  //  +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+- - - -
  //  |  R  |  G  |  B  |  A  |  R  |  G  |  B  |  A  |  R  |  G  |  B  |  A  | 
  //  +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+- - - -
  //  |        pixel 0        |        pixel 1        |        pixel 2        |
  //
  // Three alternative implementations for pixel access are provided :
  //
  //  1. Accessing individual bytes via ordinary C# array access
  //  2. Accessing individual bytes via pointer arithmetic
  //  3. Accessing 4-byte groups via pointer arithmetic
  //
  // These should all give the same results, but be progressively faster.
  //

  //
  // !!!!! Should represent integer colours as a record struct !!!!!!
  //

  //
  // Three variants are implement as subclasses :
  //
  //   The 'A' variant uses ordinary C# access to the byte array.
  //
  //   The 'B' variant uses pointer-based 'byte *' access
  //   to write the individual elements of the byte array.
  //
  //   The 'C' variant uses pointer based 'uint *' access
  //   to write four bytes at a time.
  //

  public abstract partial class ColouredPixelArrayEncodedAsBytes_Base
  { 

    // public static ColouredPixelArrayEncodedAsBytes_Base CreateInstance (
    //   int width, 
    //   int height
    // ) => (
    //   new ColouredPixelArrayEncodedAsBytes_C(
    //     width, 
    //     height
    //   )
    // ) ;

    public static ColouredPixelArrayEncodedAsBytes_Base CreateInstance (
      LinearArrayHoldingPixelBytes grayScaleIntensityValues, 
      ColourMappingTable           colourMappingTable
    ) => (
      new ColouredPixelArrayEncodedAsBytes_A_B_C(
        grayScaleIntensityValues, 
        colourMappingTable
      )
    ) ;

    public int Width { get ; }

    public int Height { get ; }

    public int PixelCount => Width * Height ;

    public int MaxPixelIndex => PixelCount - 1 ;

    public byte[] ColouredImageBytesLinearArray { get ; }

    public ColouredPixelArrayEncodedAsBytes_Base (
      int width,
      int height
    ) {
      Width  = width ;
      Height = height ;
      ColouredImageBytesLinearArray = new byte[
        PixelCount * 4
      ] ;
    }

    public ColouredPixelArrayEncodedAsBytes_Base (
      LinearArrayHoldingPixelBytes grayScaleIntensityValues,
      ColourMappingTable           colourMappingTable
    ) : this(
      grayScaleIntensityValues.Width,
      grayScaleIntensityValues.Height
    ) {
      // Safe to call from base class ?
      // OK, because this is not a virtual function
      LoadFromIntensityBytesArray(
        grayScaleIntensityValues,
        colourMappingTable
      ) ;
    }

    // Build a clone of the ColouredImageBytesLinearArray
    // as an array of uint values encoded as ABGR

    public uint[] BuildCloneOfArrayColouredImageBytesLinearArray_ABGR ( )
    {
      var abgr = new uint[PixelCount] ;
      for ( int iPixel = 0 ; iPixel < PixelCount ; iPixel++ ) 
      {
        byte r = this.ColouredImageBytesLinearArray[iPixel*4 + 0] ; // R
        byte g = this.ColouredImageBytesLinearArray[iPixel*4 + 1] ; // G
        byte b = this.ColouredImageBytesLinearArray[iPixel*4 + 2] ; // B
        byte a = this.ColouredImageBytesLinearArray[iPixel*4 + 3] ; // A
        abgr[iPixel] = PixelEncodingHelpers.EncodeABGR(r,g,b) ;
      }
      return abgr ;
    }

    public uint[] BuildCloneOfColouredImageBytesLinearArray_ARGB ( )
    {
      var argb = new uint[PixelCount] ;
      for ( int iPixel = 0 ; iPixel < PixelCount ; iPixel++ ) 
      {
        byte r = this.ColouredImageBytesLinearArray[iPixel*4 + 0] ; // R
        byte g = this.ColouredImageBytesLinearArray[iPixel*4 + 1] ; // G
        byte b = this.ColouredImageBytesLinearArray[iPixel*4 + 2] ; // B
        byte a = this.ColouredImageBytesLinearArray[iPixel*4 + 3] ; // A
        argb[iPixel] = PixelEncodingHelpers.EncodeARGB(r,g,b) ;
      }
      return argb ;
    }

    public unsafe abstract void LoadFromIntensityBytesArray (
      LinearArrayHoldingPixelBytes grayScaleIntensityValues,
      ColourMappingTable           colourMappingTable
    ) ;

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    protected bool CanGetPixelOffset (
      int     x,
      int     y,
      out int pixelOffset
    ) {
      if ( 
         x < 0
      || y < 0
      || x >= Width
      || y >= Height
      ) {
        // The pixel we'd write is outside
        // the bounds of the image ...
        pixelOffset = -1 ;
        return false ;
      }
      pixelOffset = (
        x
      + y * Width
      ) ;
      #if DEBUG
        pixelOffset.Should().BeInRange(0,MaxPixelIndex) ;
      #endif
      return true ;
    }

    public virtual void SetAllPixels ( RgbByteValues colour )
    {
      for ( int y = 0 ; y < Height ; y++ )
      {
        for ( int x = 0 ; x < Width ; x++ )
        {
          SetPixel(x,y,colour) ;
        }
      }
    }

    public abstract void SetPixel ( 
      int           x, 
      int           y, 
      RgbByteValues colour
    ) ;

    public void SetPixel_ThinOrThick ( 
      int           x, 
      int           y, 
      RgbByteValues colour,
      bool          thick
    ) {
      if ( thick )
      {
        // Set the immediately adjacent pixels
        // as well as the one at (x,y).
        // This provides a cheap-and-cheerful way
        // of drawing thicker-than-usual lines.
        // It's not very performant, as when drawing a line
        // we'll be setting pixels more than once ... however
        // for our purposes it's probably adequate ??
        SetPixel( x - 1 , y - 1 , colour ) ;
        SetPixel( x - 1 , y + 0 , colour ) ;
        SetPixel( x - 1 , y + 1 , colour ) ;
        SetPixel( x + 0 , y - 1 , colour ) ;
        SetPixel( x + 0 , y + 0 , colour ) ;
        SetPixel( x + 0 , y + 1 , colour ) ;
        SetPixel( x + 1 , y - 1 , colour ) ;
        SetPixel( x + 1 , y + 0 , colour ) ;
        SetPixel( x + 1 , y + 1 , colour ) ;
      }
      else
      {
        SetPixel(x,y,colour) ;
      }
    }

    public unsafe abstract void SetPixel_ARGB ( 
      int  x, 
      int  y, 
      uint colour_ARGB
    ) ;

    public abstract bool CanGetPixel ( 
      int                                    x, 
      int                                    y,
      [NotNullWhen(true)] out RgbByteValues? pixelColour
    ) ;

    public abstract bool CanGetPixel ( 
      int                           x, 
      int                           y,
      [NotNullWhen(true)] out uint? pixelColour_ARGB
    ) ;

    public bool PixelColourIs ( 
      int            x, 
      int            y,
      RgbByteValues? expectedPixelColour
    ) {
      bool pixelExists = CanGetPixel ( 
        x, 
        y,
        out RgbByteValues? actualPixelColour
      ) ;
      return actualPixelColour == expectedPixelColour ;
    }

    public bool PixelColourIs ( 
      int   x, 
      int   y,
      uint? expectedPixelColour_ARGB
    ) {
      bool pixelExists = CanGetPixel( 
        x, 
        y,
        out uint? actualPixelColour_ARGB
      ) ;
      return actualPixelColour_ARGB == expectedPixelColour_ARGB ;
    }

  }

}

