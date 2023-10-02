//
// ColourDescriptor_Tests.cs
//

using Xunit;
using Xunit.Abstractions;

using FluentAssertions;

using Clf.Common.ImageProcessing;

namespace ImageProcessing_Tests
{

  public class ColourDescriptor_Tests
  {

    [Fact]
    public void AllTestsSucceed ( )
    {
      ColourDescriptor colourDescriptor = ColourDescriptor.FromByteValues(
        R : 1,
        G : 2,
        B : 3
      ) ;
      colourDescriptor.R.Should().Be(1) ;
      colourDescriptor.G.Should().Be(2) ;
      colourDescriptor.B.Should().Be(3) ;

      uint argb = colourDescriptor.AsEncodedInteger_ARGB ;
      argb.Should().Be(0xff010203) ;
      PixelEncodingHelpers.DecodeARGBAsRgbTuple(argb).Should().Be((1,2,3)) ;

      uint abgr = colourDescriptor.AsEncodedInteger_ABGR ;
      abgr.Should().Be(0xff030201) ;
      PixelEncodingHelpers.DecodeABGRAsRgbTuple(abgr).Should().Be((1,2,3)) ;

      uint argb_asABGR = PixelEncodingHelpers.ConvertToABGR(argb) ;
      argb_asABGR.Should().Be(abgr) ;   

      uint abgr_asARGB = PixelEncodingHelpers.ConvertToARGB(abgr) ;
      abgr_asARGB.Should().Be(argb) ;   
    }

  }

}

