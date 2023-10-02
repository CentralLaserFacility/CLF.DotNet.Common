//
// ColouredPixelArrayEncodedAsBytes_B.cs
//

#define VERIFY_ARRAY_ELEMENT_POINTER_VALIDITY_NO

using FluentAssertions ;
using System.Diagnostics.CodeAnalysis ;

namespace Clf.Common.ImageProcessing
{
  
  //
  // This class implements the 'B' variant.
  //

  public partial class ColouredPixelArrayEncodedAsBytes_B : ColouredPixelArrayEncodedAsBytes_Base
  {

    public ColouredPixelArrayEncodedAsBytes_B (
      int width, 
      int height
    ) : 
    base(
      width,
      height
    ) { 
    }

    public ColouredPixelArrayEncodedAsBytes_B ( 
      LinearArrayHoldingPixelBytes                                           grayScaleIntensityValues, 
      ColourMappingTable colourMappingTable
    ) : 
    base(
      grayScaleIntensityValues, 
      colourMappingTable
    ) {
    }

    public unsafe override void LoadFromIntensityBytesArray (
      LinearArrayHoldingPixelBytes                                           grayScaleIntensityValues, 
      ColourMappingTable colourMappingTable
    ) {
      this.Width.Should().Be(grayScaleIntensityValues.Width) ; 
      this.Height.Should().Be(grayScaleIntensityValues.Height) ;
      //
      // Suppose we have a 32-bit integer value 0x04030201
      // ie
      //
      //    +---+---+---+---+
      //    | 4 | 3 | 2 | 1 |    <== This is the encoding format we need !!
      //    +---+---+---+---+
      //      A   B   G   R
      //
      // When we write this into memory, the result is
      //
      //    +---+---+---+---+- - -
      //    | 1 | 2 | 3 | 4 | 
      //    +---+---+---+---+- - -
      //      R   G   B   A
      //
      // ... as required by the DOM Canvas that we'll be writing to.
      //
      uint[] colourTable_ABGR = colourMappingTable.IntegerValuesTable_ABGR ;
      fixed ( byte * pGreyScaleIntensityArray = grayScaleIntensityValues.m_linearArray )
      {
        byte * pGreyScaleData = pGreyScaleIntensityArray ;
        fixed ( byte * pColouredImageBytesLinearArray = ColouredImageBytesLinearArray )
        {
          // Write one byte at a time
          var rgb = colourMappingTable.ByteArrays_RGB ;
          byte * pByteToWrite = pColouredImageBytesLinearArray ;
          for ( int iPixel = 0 ; iPixel < grayScaleIntensityValues.PixelCount ; iPixel++ ) 
          {
            byte greyScaleIntensity = *pGreyScaleData++ ;
            byte r = rgb.R[greyScaleIntensity] ;
            byte g = rgb.G[greyScaleIntensity] ;
            byte b = rgb.B[greyScaleIntensity] ;
            // var (r,g,b) = (
            //    (byte, byte, byte) ( 0,0,0 )
            //   // colourMappingTable.GetColourBytesAtIntensityValue(greyScaleIntensity) 
            // ) ;
            // uint colour_ABGR = colourTable_ABGR[greyScaleIntensity] ;
            // var (r,g,b) = ColourDescriptor.DecodeABGR(colour_ABGR) ;
            #if VERIFY_ARRAY_ELEMENT_POINTER_VALIDITY
              Helpers.VerifyArrayElementPointerValidity(
                pByteToWrite,
                4,
                pColouredImageBytesLinearArray,
                ColouredImageBytesLinearArray.Length
              ) ;
            #endif
            (*pByteToWrite++) = r ;
            (*pByteToWrite++) = g ;
            (*pByteToWrite++) = b ;
            (*pByteToWrite++) = 255 ;
          }
        }
      }
    }

    public override void SetPixel ( int x, int y, RgbByteValues colour )
    {
      if ( 
        ! CanGetPixelOffset(
          x,
          y,
          out int pixelOffset 
        )
      ) {
        return ;
      }
      int byteOffset = pixelOffset * 4 ;
      ColouredImageBytesLinearArray[byteOffset+0] = colour.R ;
      ColouredImageBytesLinearArray[byteOffset+1] = colour.G ;
      ColouredImageBytesLinearArray[byteOffset+2] = colour.B ;
      ColouredImageBytesLinearArray[byteOffset+3] = 255 ;
    }

    public unsafe override void SetPixel_ARGB ( 
      int  x, 
      int  y, 
      uint colour_ARGB
    ) {
      if ( 
        ! CanGetPixelOffset(
          x,
          y,
          out int pixelOffset 
        )
      ) {
        return ;
      }
      int byteOffset = pixelOffset * 4 ;
      PixelEncodingHelpers.DecodeARGB(
        colour_ARGB,
        out byte r, 
        out byte g, 
        out byte b
      ) ;
      fixed ( byte * pColouredImageBytesLinearArray = ColouredImageBytesLinearArray )
      {
        byte * pPixelByte = pColouredImageBytesLinearArray + byteOffset ;
        #if VERIFY_ARRAY_ELEMENT_POINTER_VALIDITY
          Helpers.VerifyArrayElementPointerValidity(
            pFirstElementToBeAccessed : pPixelByte,
            nElementsToBeAccessed     : 4,
            pArrayFirstElement        : pColouredImageBytesLinearArray,
            nArrayElementsAllocated   : ColouredImageBytesLinearArray.Length
          ) ;
        #endif
        (*pPixelByte++) = r ;
        (*pPixelByte++) = g ;
        (*pPixelByte++) = b ;
        (*pPixelByte++) = 255 ;
      }
    }

    public override bool CanGetPixel ( 
      int                                    x, 
      int                                    y, 
      [NotNullWhen(true)] out RgbByteValues? pixelColour
    ) {
      if ( 
        ! CanGetPixelOffset(
          x,
          y,
          out int pixelOffset 
        )
      ) {
        // The pixel we'd read is outside
        // the bounds of the pixel array ...
        pixelColour = null ;
        return false ;
      }
      int byteOffset = pixelOffset * 4 ;
      pixelColour = new RgbByteValues(
        R : ColouredImageBytesLinearArray[byteOffset+0],
        G : ColouredImageBytesLinearArray[byteOffset+1],
        B : ColouredImageBytesLinearArray[byteOffset+2]
      ) ;
      return true ;
    }

    public override bool CanGetPixel (
      int                           x, 
      int                           y, 
      [NotNullWhen(true)] out uint? pixelColour_ARGB
    ) {
      var _ = CanGetPixel(
        x,
        y,
        out RgbByteValues? pixelColour
      ) ;
      pixelColour_ARGB = pixelColour?.AsPackedInteger_ARGB ;
      return pixelColour_ARGB != null ;
    }

  }

}

