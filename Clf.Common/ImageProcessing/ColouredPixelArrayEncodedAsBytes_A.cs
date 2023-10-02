//
// ColouredPixelArrayEncodedAsBytes_A.cs
//

using FluentAssertions ;
using System.Diagnostics.CodeAnalysis ;

namespace Clf.Common.ImageProcessing
{
  
  //
  // This class implements the 'A' variant, which uses ordinary C# array access.
  //

  public partial class ColouredPixelArrayEncodedAsBytes_A : ColouredPixelArrayEncodedAsBytes_Base
  {

    public ColouredPixelArrayEncodedAsBytes_A (
      int width, 
      int height
    ) : 
    base(
      width,
      height
    ) { 
    }

    public ColouredPixelArrayEncodedAsBytes_A ( 
      LinearArrayHoldingPixelBytes grayScaleIntensityValues, 
      ColourMappingTable           colourMappingTable
    ) : 
    base(
      grayScaleIntensityValues, 
      colourMappingTable
    ) {
    }

    public override void LoadFromIntensityBytesArray (
      LinearArrayHoldingPixelBytes                                           grayScaleIntensityValues, 
      ColourMappingTable colourMappingTable
    ) {
      this.Width.Should().Be(grayScaleIntensityValues.Width) ; 
      this.Height.Should().Be(grayScaleIntensityValues.Height) ;
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
      SetPixel(
        x,
        y,
        RgbByteValues.FromEncodedARGB(colour_ARGB)
      ) ;
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

