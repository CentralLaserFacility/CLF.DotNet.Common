//
// ColouredPixelArrayEncodedAsBytes_ABC.cs
//
#define VERIFY_ARRAY_ELEMENT_POINTER_VALIDITY_NO

using System.Diagnostics.CodeAnalysis ;
using FluentAssertions ;

namespace Clf.Common.ImageProcessing
{

  public partial class ColouredPixelArrayEncodedAsBytes_A_B_C : ColouredPixelArrayEncodedAsBytes_Base
  { 

    public static bool ACCESS_BYTE_ARRAY_USING_UNSAFE_POINTERS = true ;
    public static bool WRITE_PIXELS_IN_FOUR_BYTE_CHUNKS        = true ;

    private ColouredPixelArrayEncodedAsBytes_A_B_C (
      int width,
      int height
    ) :
    base(
      width,
      height
    ) {
    }

    public ColouredPixelArrayEncodedAsBytes_A_B_C (
      LinearArrayHoldingPixelBytes grayScaleIntensityValues,
      ColourMappingTable colourMappingTable
    ) : 
    base(
      grayScaleIntensityValues,
      colourMappingTable
    ) {
      // LoadFromIntensityBytesArray(
      //   grayScaleIntensityValues,
      //   colourMappingTable
      // ) ;
    }

    public unsafe override void LoadFromIntensityBytesArray (
      LinearArrayHoldingPixelBytes grayScaleIntensityValues,
      ColourMappingTable colourMappingTable
    ) {
      this.Width.Should().Be(grayScaleIntensityValues.Width) ; 
      this.Height.Should().Be(grayScaleIntensityValues.Height) ;
      if ( ACCESS_BYTE_ARRAY_USING_UNSAFE_POINTERS )
      {
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
            if ( WRITE_PIXELS_IN_FOUR_BYTE_CHUNKS )
            {
              uint * pFourColouredImageBytes_ABGR = (uint*) pColouredImageBytesLinearArray ;
              for ( int iPixel = 0 ; iPixel < grayScaleIntensityValues.PixelCount ; iPixel++ ) 
              {
                byte greyScaleIntensity = *pGreyScaleData++ ;
                uint colour = colourTable_ABGR[greyScaleIntensity] ;
                #if VERIFY_ARRAY_ELEMENT_POINTER_VALIDITY
                  VerifyArrayElementPointerValidity(
                    pFourColouredImageBytes_ABGR,
                    (uint *) pColouredImageBytesLinearArray,
                    ColouredImageBytesLinearArray.Length / 4
                  ) ;
                #endif
                (*pFourColouredImageBytes_ABGR++) = colour ;
              }
            }
            else
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
                VerifyArrayElementPointerValidity(pByteToWrite,pColouredImageBytesLinearArray,ColouredImageBytesLinearArray.Length) ;
                #endif
                (*pByteToWrite++) = r ;
                #if VERIFY_ARRAY_ELEMENT_POINTER_VALIDITY
                VerifyArrayElementPointerValidity(pByteToWrite,pColouredImageBytesLinearArray,ColouredImageBytesLinearArray.Length) ;
                #endif
                (*pByteToWrite++) = g ;
                #if VERIFY_ARRAY_ELEMENT_POINTER_VALIDITY
                VerifyArrayElementPointerValidity(pByteToWrite,pColouredImageBytesLinearArray,ColouredImageBytesLinearArray.Length) ;
                #endif
                (*pByteToWrite++) = b ;
                #if VERIFY_ARRAY_ELEMENT_POINTER_VALIDITY
                VerifyArrayElementPointerValidity(pByteToWrite,pColouredImageBytesLinearArray,ColouredImageBytesLinearArray.Length) ;
                #endif
                (*pByteToWrite++) = 255 ;
              }
            }
          }
        }
      }
      else
      {
        // Slow-but-sure method writing individual bytes ...
        var colourMappingByteArrays = colourMappingTable.ByteArrays_RGB ;
        byte[] R = colourMappingByteArrays.R ; 
        byte[] G = colourMappingByteArrays.G ; 
        byte[] B = colourMappingByteArrays.B ;
        var iImageData = 0 ;
        for ( int iGrayScaleData = 0 ; iGrayScaleData < grayScaleIntensityValues.PixelCount ; iGrayScaleData++ ) 
        {
          byte greyScaleIntensity = grayScaleIntensityValues.LinearArrayValues[iGrayScaleData] ;
          byte red   = R[greyScaleIntensity] ;
          byte green = G[greyScaleIntensity] ;
          byte blue  = B[greyScaleIntensity] ;
          this.ColouredImageBytesLinearArray[iImageData + 0] = red   ; // R
          this.ColouredImageBytesLinearArray[iImageData + 1] = green ; // G
          this.ColouredImageBytesLinearArray[iImageData + 2] = blue  ; // B
          this.ColouredImageBytesLinearArray[iImageData + 3] = 255   ; // A
          iImageData += 4 ;
        }
      }

    }

    public override void SetPixel ( 
      int           x, 
      int           y, 
      RgbByteValues colour
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
      ColouredImageBytesLinearArray[byteOffset+0] = colour.R ;
      ColouredImageBytesLinearArray[byteOffset+1] = colour.G ;
      ColouredImageBytesLinearArray[byteOffset+2] = colour.B ;
      ColouredImageBytesLinearArray[byteOffset+3] = 255 ;
    }

    public static unsafe void WriteToArrayElement<T> ( 
      ref T * p, 
      T       valueToWrite,
      T *     pArrayStart, 
      T *     pArrayEnd
    ) where T : unmanaged
    {
      throw new System.NotImplementedException() ;
    }

    //
    // Verify that the pointer we're about to use to access an array element
    // lies within the memory area assigned to the array.
    //
    //     0   1   2   3
    //   +---+---+---+---+
    //   |   |   |   |   | 4 elements
    //   +---+---+---+---+
    //     |           |
    //     p          p+3
    //
    // This code is only active in a DEBUG build.
    //

    [System.Diagnostics.Conditional("DEBUG")]
    public unsafe static void VerifyArrayElementPointerValidity<T> ( 
      T * p, 
      T * pArrayFirstElement, 
      int nArrayElements
    )  
    where T : unmanaged
    {
      // nint q = (nint) p ;
      // nint qArrayFirstElement = (nint) pArrayFirstElement ;
      // nint qArrayLastElement = qArrayFirstElement + nArrayElements - 1 ;
      // q.Should().BeInRange(qArrayFirstElement,qArrayLastElement) ;
      ( p >= pArrayFirstElement ).Should().BeTrue() ;
      ( p < pArrayFirstElement + nArrayElements ).Should().BeTrue() ;
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
      if ( ACCESS_BYTE_ARRAY_USING_UNSAFE_POINTERS )
      {
        fixed ( byte * pColouredImageBytesLinearArray = ColouredImageBytesLinearArray )
        {
          byte * pPixelByte = pColouredImageBytesLinearArray + byteOffset ;
          if ( WRITE_PIXELS_IN_FOUR_BYTE_CHUNKS )
          {
            uint * pFourColouredImageBytes_ABGR = (uint*) pPixelByte ;
            #if VERIFY_ARRAY_ELEMENT_POINTER_VALIDITY
              VerifyArrayElementPointerValidity(
                pFourColouredImageBytes_ABGR,
                (uint *) pColouredImageBytesLinearArray,
                ColouredImageBytesLinearArray.Length / 4
              ) ;
            #endif
            (*pFourColouredImageBytes_ABGR) = PixelEncodingHelpers.ConvertToABGR(colour_ARGB) ;
          }
          else
          {
            #if VERIFY_ARRAY_ELEMENT_POINTER_VALIDITY
            VerifyArrayElementPointerValidity(pPixelByte,pColouredImageBytesLinearArray,ColouredImageBytesLinearArray.Length) ;
            #endif
            (*pPixelByte++) = r ;
            #if VERIFY_ARRAY_ELEMENT_POINTER_VALIDITY
            VerifyArrayElementPointerValidity(pPixelByte,pColouredImageBytesLinearArray,ColouredImageBytesLinearArray.Length) ;
            #endif
            (*pPixelByte++) = g ;
            #if VERIFY_ARRAY_ELEMENT_POINTER_VALIDITY
            VerifyArrayElementPointerValidity(pPixelByte,pColouredImageBytesLinearArray,ColouredImageBytesLinearArray.Length) ;
            #endif
            (*pPixelByte++) = b ;
            #if VERIFY_ARRAY_ELEMENT_POINTER_VALIDITY
            VerifyArrayElementPointerValidity(pPixelByte,pColouredImageBytesLinearArray,ColouredImageBytesLinearArray.Length) ;
            #endif
            (*pPixelByte++) = 255 ;
          }
        }
      }
      else
      {
        ColouredImageBytesLinearArray[byteOffset+0] = r ;
        ColouredImageBytesLinearArray[byteOffset+1] = g ;
        ColouredImageBytesLinearArray[byteOffset+2] = b ;
        ColouredImageBytesLinearArray[byteOffset+3] = 255 ;
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
      if ( 
        ! CanGetPixelOffset(
          x,
          y,
          out int pixelOffset 
        )
      ) {
        // The pixel we'd read is outside
        // the bounds of the pixel array ...
        pixelColour_ARGB = null ;
        return false ;
      }
      int byteOffset = pixelOffset * 4 ;
      pixelColour_ARGB = new RgbByteValues(
        R : ColouredImageBytesLinearArray[byteOffset+0],
        G : ColouredImageBytesLinearArray[byteOffset+1],
        B : ColouredImageBytesLinearArray[byteOffset+2]
      ).AsPackedInteger_ARGB ;
      return true ;
    }

  }

}

