//
// MultiBytePixelHelpers_16_bit_Tests.cs
//

using Xunit;
using Xunit.Abstractions;

using FluentAssertions;

using Clf.Common.ImageProcessing;
using System.Linq;
using System.Collections.Generic;

namespace ImageProcessing_Tests
{

  public class MultiBytePixelHelpers_16_bit_Tests
  {

    [Theory]
    [InlineData(
      new byte[]{
        0x00,0x01,
        0x02,0x03,
      },
      new ushort[]{
        0x0100,
        0x0302,
      },
      LeastOrMostSignificantByteFirst.LeastSignificantByteFirst
    )]
    [InlineData(
      new byte[]{
        0x00,0x01,
        0x02,0x03,
      },
      new ushort[]{
        0x0001,
        0x0203,
      },
      LeastOrMostSignificantByteFirst.MostSignificantByteFirst
    )]
    [InlineData(
      new byte[]{
      },
      new ushort[]{
      },
      LeastOrMostSignificantByteFirst.MostSignificantByteFirst
    )]
    public void CanDecodeByteArray_LSByteFirst ( 
      byte[]                          byteArray, 
      ushort[]                        valuesExpected,
      LeastOrMostSignificantByteFirst byteOrdering
    ) {
      ushort[] values = MultiBytePixelHelpers_16Bit.DecodeByteArray_SixteenBit(
        byteArray,
        byteOrdering
      ) ;
      values.Should().BeEquivalentTo(valuesExpected) ;
    }
    
  }

}

