//
// PixelIntensityMapper_Tests.cs
//

using Clf.Common.ImageProcessing ;
using System.Linq ;

using Xunit ;

using FluentAssertions ;

namespace ImageProcessing_Tests
{

  public class PixelIntensityMapper_Tests
  {

    [Theory]
    [InlineData(ImagePixelBitDepth.EightBits,255)]
    [InlineData(ImagePixelBitDepth.TwelveBits,4095)]
    [InlineData(ImagePixelBitDepth.SixteenBits,65535)]
    public void PixelIntensityMapper_WorksAsExpected ( ImagePixelBitDepth bitDepth, int expectedImagePixelMaxValue ) 
    {
      var pixelIntensityMapper = new PixelIntensityMapper(
        imagePixelBitDepth : bitDepth
      ) ;
      pixelIntensityMapper.ImagePixelMaxValue.Should().Be(expectedImagePixelMaxValue) ;
      double brightness = pixelIntensityMapper.Brightness ; // Nominally 0.5
      double contrast = pixelIntensityMapper.Contrast_AsSlopeOfMappingLine ;     // Nominally 1.0
      // TODO : CHECK THIS ... SHOULD BE EXACTLY TWO ???
      pixelIntensityMapper.Contrast_AsSlopeOfMappingLine = 2.0 ;                 // Should not affect Brightness
      pixelIntensityMapper.Brightness = brightness * 1.5 ;  // Should not affect Contrast_AsSlopeOfMappingLine
      var mapping = pixelIntensityMapper.MappedPixelValues.ToArray() ;
    }

  }

}

