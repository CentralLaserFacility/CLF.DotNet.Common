//
// MultiBytePixelHelpers_12Bit.cs
//

namespace Clf.Common.ImageProcessing
{

  //
  // Helper functions for extracting 12-bit pixel sequences
  // that have been packed efficiently into a byte array,
  // with two 12 bit values represented in each 3-byte (24-bit) chunk.
  //

  //
  // We can pack a 12-bit value into a 16 bit pair-of-bytes :
  //
  //     m/s byte     l/s byte
  //   +-----------+-----------+
  //   | --- | a02 | a01 | a00 |    Packed into 16 bits
  //   +-----------+-----------+
  //                       |
  //                       Least significant 4 bits
  //
  // But that leaves 4 bits wasted.
  //
  // We can pack TWO 12-bit values into three successive bytes.
  // There are several possible formats for the underlying byte array,
  // depending on how the 4-bit 'nibbles' are packed.
  //
  // One obvious format is :
  //
  //        0         1         2       
  //   +---------+---------+---------+--
  //   | a01 a00 | b01 b00 | a02 b02 |       'a01_a00_b01_b00_a02_b02'   
  //   +---------+---------+---------+--
  //        |         |         |
  //        |         |         Most significant nibbles of 'a' and 'b'
  //        |         |
  //        |         Least significant two nibbles of 'b'
  //        |         
  //        Least significant two nibbles of 'a'
  //
  // It's convenient to keep the 'lower' two nibbles' worth of each value
  // in a single byte, as that minimises the amount of bit manipulation we need.
  //
  // There are many other possibilities, for example
  //
  //   +---------+---------+---------+--
  //   | a01 a00 | a02 b02 | b01 b00 |    'a01_a00_a02_b02_b01_b00'
  //   +---------+---------+---------+--
  //

  public static class MultiBytePixelHelpers_12Bit
  {

    public enum NibbleFormat {
      a01_a00_b01_b00_a02_b02,
      // a01_a00_a02_b02_b01_b00
      // etc ...
    }

    public static System.UInt16[] DecodeByteArray_12BitPacked ( 
      System.Byte[] byteArray, 
      NibbleFormat  nibbleFormat 
    ) { 
      int nResultPairs = byteArray.Length / 3 ;
      System.UInt16[] result = new System.UInt16[nResultPairs*2] ;
      int iSourceByte = 0 ;
      int iResultValue = 0 ;
      for ( int iResultPair = 0 ; iResultPair < nResultPairs ; iResultPair++ ) 
      {
        byte byte_0 = byteArray[iSourceByte++] ;
        byte byte_1 = byteArray[iSourceByte++] ;
        byte byte_2 = byteArray[iSourceByte++] ;
        var (a,b) = DecodeThreeBytesIntoTwo12BitValues(
          byte_0, 
          byte_1, 
          byte_2,
          nibbleFormat
        ) ;
        result[iResultValue++] = a ;
        result[iResultValue++] = b ;
      }
      return result ;
    }

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static (System.UInt16 a, System.UInt16 b) DecodeThreeBytesIntoTwo12BitValues ( 
      byte         byte_0, 
      byte         byte_1, 
      byte         byte_2, 
      NibbleFormat nibbleFormat 
    ) {
      switch ( nibbleFormat )
      {
      case NibbleFormat.a01_a00_b01_b00_a02_b02:
        {
          UnpackNibbles(
            byteValue   : byte_2,
            lowerNibble : out byte b_02,
            upperNibble : out byte a_02
          ) ;
          var a12 = (ushort) (
            a_02 << 8
          | byte_0 // a01_a00
          ) ;
          var b12 = (ushort) (
            b_02 << 8
          | byte_1 // b01_b00
          ) ;
          return (a12,b12) ;
        }
      default:
        throw new System.NotImplementedException() ;
      }
    }

    //
    //     8-bit byte
    //   +-------------------+
    //   | x x x x | x x x x |
    //   +-------------------+
    //        |         |
    //        |         Lower nibble
    //        |
    //        Upper nibble
    //

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static void UnpackNibbles ( 
      byte     byteValue, 
      out byte lowerNibble,
      out byte upperNibble
    ) {
      // A bit-shift to the right will insert zero bits
      // at the left, because 'byte' is an unsigned type
      lowerNibble = (byte) ( byteValue & 0x0F ) ; 
      upperNibble = (byte) ( byteValue >> 4 ) ; 
    }

  }

}

