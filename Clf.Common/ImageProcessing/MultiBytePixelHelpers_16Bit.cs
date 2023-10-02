//
// MultiBytePixelHelpers_16Bit.cs
//

namespace Clf.Common.ImageProcessing
{

  //
  // Helper functions for extracting 16-bit pixel sequences
  // from a byte array.
  //

  //
  // Unsigned 16 bit pixel values :
  //
  // Let's represent the 16 bit value as two bytes, 'a' and 'b' :
  //
  //     Most              Least
  //     significant       significant
  //     byte              byte
  //     |                 |
  //   +-----------------+-----------------+
  //   | b b b b b b b b | a a a a a a a a |  
  //   +-----------------+-----------------+
  //     |                 |         | | |
  //     32768             128       4 2 1
  //
  // There are two possible formats for the underlying byte array :
  //
  //   SixteenBit_LSByteFirst
  //   +--------+--------+--------+--------+--------+--------+---
  //   |aaaaaaaa|bbbbbbbb|aaaaaaaa|bbbbbbbb|aaaaaaaa|bbbbbbbb| etc
  //   +--------+--------+--------+--------+--------+--------+---
  //       0        1        2        3        4        5
  //
  //   SixteenBit_MSByteFirst
  //   +--------+--------+--------+--------+--------+--------+---
  //   |bbbbbbbb|aaaaaaaa|bbbbbbbb|aaaaaaaa|bbbbbbbb|aaaaaaaa| etc
  //   +--------+--------+--------+--------+--------+--------+---
  //       0        1        2        3        4        5
  //
  // Pairs of bytes represent a single 16 bit pixel.
  // The first byte of a pair ('a') could represent either the
  // least significant part of a 16 bit value, or the most significant part.
  //
  // If the pixel values have only 12 bits of significance, we'll simply
  // decode them as 16 bits and then mask the upper 4 bits to zero.
  //

  public enum LeastOrMostSignificantByteFirst {
    LeastSignificantByteFirst,
    MostSignificantByteFirst
  }

  public static class MultiBytePixelHelpers_16Bit
  {

    public static System.UInt16[] DecodeByteArray_SixteenBit ( 
      System.Byte[]                   byteArray,
      LeastOrMostSignificantByteFirst byteOrdering
    ) {
      return byteOrdering switch {
      LeastOrMostSignificantByteFirst.LeastSignificantByteFirst
        => DecodeByteArray_SixteenBit_LSByteFirst(byteArray),
      LeastOrMostSignificantByteFirst.MostSignificantByteFirst
        => DecodeByteArray_SixteenBit_MSByteFirst(byteArray),
      _ => throw new System.NotImplementedException()
      } ;
    }

    public static System.UInt16[] DecodeByteArray_SixteenBit_LSByteFirst ( 
      System.Byte[] byteArray 
    ) {
      System.UInt16[] result = new System.UInt16[byteArray.Length/2] ;
      int iSourceByte = 0 ;
      for ( int iResultValue = 0 ; iResultValue < result.Length ; iResultValue++ ) 
      {
        byte byte_0 = byteArray[iSourceByte++] ;
        byte byte_1 = byteArray[iSourceByte++] ;
        result[iResultValue] = BuildSixteenBitValue_LSByteFirst(
          byte_0,
          byte_1
        ) ;
      }
      return result ;
    }

    public static System.UInt16[] DecodeByteArray_SixteenBit_MSByteFirst ( 
      System.Byte[] byteArray 
    ) {
      System.UInt16[] result = new System.UInt16[byteArray.Length/2] ;
      int iSourceByte = 0 ;
      for ( int iResultValue = 0 ; iResultValue < result.Length ; iResultValue++ ) 
      {
        byte byte_0 = byteArray[iSourceByte++] ;
        byte byte_1 = byteArray[iSourceByte++] ;
        result[iResultValue] = BuildSixteenBitValue_MSByteFirst(
          byte_0,
          byte_1
        ) ;
      }
      return result ;
    }

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static System.UInt16 BuildSixteenBitValue_LSByteFirst ( 
      byte byte_0_leastSignificant, 
      byte byte_1_mostSignificant 
    ) {
      return (ushort) (
        ( byte_1_mostSignificant  << 8 ) 
      | ( byte_0_leastSignificant << 0 )
      ) ;
    }

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static System.UInt16 BuildSixteenBitValue_MSByteFirst ( 
      byte byte_0_mostSignificant, 
      byte byte_1_leastSignificant 
    ) {
      return (ushort) (
        ( byte_0_mostSignificant  << 8 ) 
      | ( byte_1_leastSignificant << 0 )
      ) ;
    }

  }

}

