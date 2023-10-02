//
// MultiBytePixelHelpers_12_bit_Tests.cs
//

using Xunit;
using Xunit.Abstractions;

using FluentAssertions;

using Clf.Common.ImageProcessing;
using System.Linq;
using System.Collections.Generic;

namespace ImageProcessing_Tests
{

  public class MultiBytePixelHelpers_12_bit_Tests
  {
    
    [Theory]
    [InlineData(0x12,0x01,0x02)]
    [InlineData(0xFE,0x0F,0x0E)]
    public void CanDecodeByteAsNibbles ( byte byteValue, byte upperNibbleExpected, byte lowerNibbleExpected )
    {
      MultiBytePixelHelpers_12Bit.UnpackNibbles(
        byteValue,
        out byte lowerNibble,
        out byte upperNibble
      ) ;
      lowerNibble.Should().Be(lowerNibbleExpected) ;
      upperNibble.Should().Be(upperNibbleExpected) ;
    }

    [Theory]
    [InlineData(new byte[]{0x21,0x00,0x30},0x0321,0x0000)]
    [InlineData(new byte[]{0x00,0x21,0x03},0x0000,0x0321)]
    [InlineData(new byte[]{0x00,0x54,0x06},0x0000,0x0654)]
    [InlineData(new byte[]{0x21,0x54,0x36},0x0321,0x0654)]
    public void CanDecodeThreeBytesAsTwoTwelveBitValues ( 
      byte[] bytes, 
      ushort firstValueExpected, 
      ushort secondValueExpected 
    ) {
      var (firstValue,secondValue) = MultiBytePixelHelpers_12Bit.DecodeThreeBytesIntoTwo12BitValues(
        bytes[0],
        bytes[1],
        bytes[2],
        MultiBytePixelHelpers_12Bit.NibbleFormat.a01_a00_b01_b00_a02_b02 
      ) ;
      firstValue.Should().Be(firstValueExpected) ;
      secondValue.Should().Be(secondValueExpected) ;
    }

    [Theory]
    [InlineData(
      new byte[]{
        0x21,0x54,0x36,
      },
      new ushort[]{
        0x0321,0x0654,
      },
      MultiBytePixelHelpers_12Bit.NibbleFormat.a01_a00_b01_b00_a02_b02    
    )]
    [InlineData(
      new byte[]{
        0x21,0x54,0x36,
        0x00,0x00,0x00,
        0x21,0x54,0x36,
      },
      new ushort[]{
        0x0321,0x0654,
        0x0000,0x0000,
        0x0321,0x0654,
      },
      MultiBytePixelHelpers_12Bit.NibbleFormat.a01_a00_b01_b00_a02_b02    
    )]
    [InlineData(
      new byte[]{
      },
      new ushort[]{
      },
      MultiBytePixelHelpers_12Bit.NibbleFormat.a01_a00_b01_b00_a02_b02    
    )]
    public void CanDecodeByteArrayAsSequenceOfTwelveBitValues ( 
      byte[]                                   bytes, 
      ushort[]                                 valuesExpected,
      MultiBytePixelHelpers_12Bit.NibbleFormat nibbleFormat
    ) {
      var values = MultiBytePixelHelpers_12Bit.DecodeByteArray_12BitPacked(
        bytes,
        nibbleFormat 
      ) ;
      values.Should().BeEquivalentTo(valuesExpected) ;
    }

  }

}

