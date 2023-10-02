//
// PixelIntensityMapper.cs
//

using FluentAssertions ;
using System.Collections.Generic ;
using System.Linq ;

namespace Clf.Common.ImageProcessing
{

  //
  // To provide control over the 'brightness' and 'contrast' of an image,
  // we need to transform the intensity values in the underlying image data
  // into 'mapped' pixel intensities between 0 and 255 :
  //
  //   |                                                 |
  //   | <------ original pixel intensity values ------> |
  //   |                                                 |
  //   |         for  8-bit pixels : 0 ..   255          |
  //   |         for 12-bit pixels : 0 ..  4095          |
  //   |         for 16-bit pixels : 0 .. 65535          |
  //   |                                                 |
  //   +-------------------------------------------------+
  //   |          MIN         MAX                        |
  //   |           |           |                         |
  // --+-----------+-----------+-------------------------+--
  //               |           |
  //            +-----------------+  
  //            | MAPPING FORMULA | 
  //            +-----------------+  
  //                    |
  //                    Mapped
  //                    pixel value
  //                    0..255
  //
  // Pixel values that lie between MIN and MAX are mapped
  // to values between 0 and 255, using linear interpolation.
  // Values outside that range are clipped to 0 or 255.
  //
  //    +-----------------------------+ 255
  //    |                          *  |
  //    |                       *     |
  //    |                    *        |
  //    |                 *           |  Mapped pixel
  //    |             [*]             |  intensity
  //    |           *                 |  0 .. 255
  //    |        *                    |
  //    |     *                       |
  //    |  *                          |
  //    +--|--|--|--|--|--|--|--|--|--+ 0
  //    |                             |
  //    |  Original pixel intensity   |
  //    |                             |
  //   MIN                           MAX
  //
  //
  // Typically the active area (MIN..MAX) is a small subset of the available range.
  //
  //    +-----------------------------+
  //    |         *                   |    High contrast
  //    |        *                    |    HIGH BRIGHTNESS 
  //    |      [*]                    |    because low-intensity pixels
  //    |      *                      |    produce mapped pixels of high value
  //    |     *                       |
  //    +-----------------------------+
  //
  //    +-----------------------------+
  //    |                           * |    High contrast    
  //    |                          *  |    LOW BRIGHTNESS
  //    |                        [*]  |    because only the originally bright
  //    |                        *    |    pixels come out as bright
  //    |                       *     |
  //    +-----------------------------+
  //
  //    +-----------------------------+
  //    |                 *           |    Low contrast
  //    |              *              |    
  //    |          [*]                |    Slope is shallow.
  //    |        *                    |
  //    |     *                       |
  //    +--*--------------------------+
  //

  //
  // BRIGHTNESS and CONTRAST
  //
  //    +-----------------------------+
  //    |         *                   |    High contrast
  //    |        *                    |    HIGH BRIGHTNESS because 
  //    |      [*]                    |    low-intensity pixels produce
  //    |      *                      |    mapped pixels of high value
  //    |     *                       |
  //    +-------|---------------------+
  //   1.0      |                    0.0
  //            |
  //            BRIGHTNESS correlates with the fractional position
  //            of the 'pivot point' relative to the
  //            RIGHT HAND EDGE of the available range.
  //            Values range from nominally 0.0 to 1.0.
  //            Values outside this range are also acceptable,
  //            as the 'mapping line' it describes might still overlap
  //            with the visible range of values, especially when the
  //            contrast is low and the slope is shallow.
  //
  // CONTRAST determines the 'slope'.
  //
  //   Nominal value is 1.
  //   Higher values => steep slope   => higher contrast.
  //   Lower  values => shallow slope => low contrast.
  //   
  //   ?? Specify the contrast in terms of the angle theta ??
  //   
  //   |        theta = 0          : lowest contrast
  //   | /      theta = 90 degrees : highest contrast
  //   |/theta
  //   +----------------
  //   
  //   Tan(theta) <==> SideOpposite_DeltaY / SideAdjacent_DeltaX
  //   
  //   For a given 'DeltaX', as we increase 'theta'
  //   Tan(theta) gets us a progressively increasing 'DeltaY'
  //   which tends to infinity as theta approaches 90 degrees.
  //   Rather than have slopes that go from zero to infinity,
  //   we'll have the range of our 'contrast' values go from
  //   a shallow slope (eg 0.1) to a steep slope (eg 10.0).
  //   
  //   From specified values of brightness and contrast, and knowing
  //   the expected 'range' of the input pixel intensities,
  //   we can compute the values of MIN and MAX.
  //

  //
  // We can characterise the 'brightness' and 'contrast'
  // in terms of the relative 'X' position of the 'pivot point'
  // of the linear mapping (shown as [*] above) and the
  // relative slope of the line.
  //
  // The 'nominal' values are :
  //
  //  PivotPosition = 0.5    // Affects the BRIGHTNESS ; lower values => less bright
  //  Slope         = 1.0    // Affects the CONTRACT
  //

  public class PixelIntensityMapper
  {

    public ImagePixelBitDepth OriginalPixelBitDepth { get ; private set ; }

    public int ImagePixelMaxValue { get ; private set ; }

    private int[]? m_originalPixelValues = null ;
    public IEnumerable<int> OriginalPixelValues => ( m_originalPixelValues ??= Enumerable.Range(0,ImagePixelMaxValue).ToArray() ) ; 

    private byte[]? m_mappedPixelValues = null ;
    public IEnumerable<byte> MappedPixelValues => ( 
      m_mappedPixelValues 
      ??= OriginalPixelValues.Select(
        originalPixelValue => GetMappedPixelValue(originalPixelValue)
      ).ToArray()
    ) ; 

    //
    // The thresholds will have been computed to give us a mapping
    // that provides a desired Brightness and Contrast_AsSlopeOfMappingLine.
    //

    public int LowerThreshold        { get ; private set ; }
    public int UpperThreshold        { get ; private set ; }
    public int DeltaBeteenThresholds { get ; private set ; }

    public PixelIntensityMapper ( 
      ImagePixelBitDepth imagePixelBitDepth
    ) {
      OriginalPixelBitDepth = imagePixelBitDepth ;
      ImagePixelMaxValue = (
          OriginalPixelBitDepth switch {
        ImagePixelBitDepth.EightBits   => 1 << 8,   // 256
        ImagePixelBitDepth.TwelveBits  => 1 << 12,  // 4096
        ImagePixelBitDepth.SixteenBits => 1 << 16,  // 65536
        _ => throw new System.NotSupportedException()
        } 
      - 1 
      ) ;
      SetThresholds(
        0,
        ImagePixelMaxValue
      ) ;
    }

    public PixelIntensityMapper ( 
      ImagePixelBitDepth imagePixelBitDepth,
      int                lowerThreshold, 
      int                upperThreshold 
    ) :
    this(imagePixelBitDepth)
    {
      SetThresholds(
        lowerThreshold,
        upperThreshold
      ) ;
    }

    public double Brightness 
    {
      get 
      {
        // The brighness is determined by the fractional position
        // along the X axis of the 'pivot' point, with a high brightness
        // being associated with a pivot point near the orgin of the X axis.
        double pivotPointAlongX = ( UpperThreshold + LowerThreshold ) / 2.0 ;
        double frac01_alongX = pivotPointAlongX / ImagePixelMaxValue ;
        return ( 1.0 - frac01_alongX ) ;
      }
      set
      {
        SetBrightnessAndContrast(
          value,
          Contrast_AsSlopeOfMappingLine
        ) ;
      }
    }

    public double Contrast_AsSlopeOfMappingLine 
    {
      get 
      {
        // If the 'span' between the thresholds gets smaller,
        // the Contrast_AsSlopeOfMappingLine gets bigger ...
        double span = UpperThreshold - LowerThreshold ;
        double maxAvailableSpan = ImagePixelMaxValue ;
        return ( 
          maxAvailableSpan
        / span
        ) ;
      }
      set
      {
        SetBrightnessAndContrast(
          Brightness,
          value
        ) ;
      }
    }

    public void SetThresholds ( int lowerThreshold, int upperThreshold ) 
    {
      upperThreshold.Should().BeGreaterThan(lowerThreshold) ;
      LowerThreshold        = lowerThreshold ;
      UpperThreshold        = upperThreshold ;
      DeltaBeteenThresholds = UpperThreshold - LowerThreshold + 1 ;
      // Changing the mapping means we'll need to recompute 
      // the (lazily evaluated) 'mapped-pixel-values' array.
      m_mappedPixelValues = null ;
    }

    public void SetBrightnessAndContrast ( double brightness, double contrast ) 
    {
      // Brightness determines the fractional position
      // of the 'pivot point' from the right hand edge of the X axis.
      double pivotPointAlongX = ( 1.0 - brightness ) * ImagePixelMaxValue ;
      double span_minToMax = ImagePixelMaxValue / contrast ;
      int lowerThreshold = (int) ( pivotPointAlongX - span_minToMax / 2.0 ) ;
      int upperThreshold = (int) ( pivotPointAlongX + span_minToMax / 2.0 ) ;
      SetThresholds(
        lowerThreshold, 
        upperThreshold
      ) ;
    }

    // We map the incoming pixel value, which can be either outside
    // the range or inside it, into an 8-bit value for display.

    public byte GetMappedPixelValue ( int incomingValue )
    {
      if ( m_mappedPixelValues != null ) 
      {
        // We have a complete set of 'cached' values
        try
        {
          // Hmm, does this try/catch impose
          // an unacceptable overhead ??
          return m_mappedPixelValues[incomingValue] ;
        }
        catch
        {
          return 1 ;
        }
      }
      // Compute the value here ...
      if ( incomingValue < LowerThreshold )
      {
        incomingValue = LowerThreshold ;
      }
      else if ( incomingValue > UpperThreshold )
      {
        incomingValue = UpperThreshold ;
      }
      double delta = incomingValue - LowerThreshold ;
      double frac01 = delta / DeltaBeteenThresholds ;
      return (byte) ( frac01 * 255 ) ;
    }

    public void ApplyTransformation_ConvertingIntensitiesToByteValues ( 
      LinearArrayHoldingPixelIntensities linearArrayHoldingPixelIntensities,
      out LinearArrayHoldingPixelBytes   linearArrayHoldingPixelBytes
    ) {
      linearArrayHoldingPixelBytes = new(
        linearArrayHoldingPixelIntensities.Width,
        linearArrayHoldingPixelIntensities.Height
      ) ;
      ApplyTransformation_ConvertingIntensitiesToByteValues(
        linearArrayHoldingPixelIntensities,
        linearArrayHoldingPixelBytes
      ) ;
    }

    public void ApplyTransformation_ConvertingIntensitiesToByteValues ( 
      LinearArrayHoldingPixelIntensities linearArrayHoldingPixelIntensities,
      LinearArrayHoldingPixelBytes       linearArrayHoldingPixelBytes
    ) {
      linearArrayHoldingPixelIntensities.PixelCount.Should().Be(linearArrayHoldingPixelBytes.PixelCount) ;
      ushort[] sourceArray = linearArrayHoldingPixelIntensities.m_linearArray ;
      byte[] targetArray = linearArrayHoldingPixelBytes.m_linearArray ;
      targetArray.Length.Should().Be(sourceArray.Length) ;
      int nPixels = sourceArray.Length ;
      for ( int iPixel = 0 ; iPixel < nPixels ; iPixel++ ) 
      {
        targetArray[iPixel] = GetMappedPixelValue(
          sourceArray[iPixel]
        ) ;
      }
    }

  }

}


