//
// ColouredPixelArrayEncodedAsBytes_C.cs
//

#define VERIFY_ARRAY_ELEMENT_POINTER_VALIDITY_NO

using FluentAssertions ;
using System.Diagnostics.CodeAnalysis ;

namespace Clf.Common.ImageProcessing
{
  
  //
  // This class implements the 'C' variant.
  //

  public partial class ColouredPixelArrayEncodedAsBytes_C : ColouredPixelArrayEncodedAsBytes_Base
  {

    public ColouredPixelArrayEncodedAsBytes_C (
      int width, 
      int height
    ) : 
    base(
      width,
      height
    ) { 
    }

    public ColouredPixelArrayEncodedAsBytes_C ( 
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
      // Suppose we have a 32-bit integer value 0x04030201 #
      // representing a colour, ie
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
          uint * pFourColouredImageBytes_ABGR = (uint*) pColouredImageBytesLinearArray ;
          for ( int iPixel = 0 ; iPixel < grayScaleIntensityValues.PixelCount ; iPixel++ ) 
          {
            byte greyScaleIntensity = *pGreyScaleData++ ;
            uint colour = colourTable_ABGR[greyScaleIntensity] ;
            #if VERIFY_ARRAY_ELEMENT_POINTER_VALIDITY
              Helpers.VerifyArrayElementPointerValidity(
                pFourColouredImageBytes_ABGR,
                (uint *) pColouredImageBytesLinearArray,
                ColouredImageBytesLinearArray.Length / 4
              ) ;
            #endif
            (*pFourColouredImageBytes_ABGR++) = colour ;
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
        uint * pFourColouredImageBytes_ABGR = (uint*) pPixelByte ;
        #if VERIFY_ARRAY_ELEMENT_POINTER_VALIDITY
          Helpers.VerifyArrayElementPointerValidity(
            pFourColouredImageBytes_ABGR,
            (uint *) pColouredImageBytesLinearArray,
            ColouredImageBytesLinearArray.Length / 4
          ) ;
        #endif
        (*pFourColouredImageBytes_ABGR) = PixelEncodingHelpers.ConvertToABGR(colour_ARGB) ;
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

