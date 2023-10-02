//
// MultiBytePixelHelpers_12Bit_old_01.cs
//

namespace Clf.Common.ImageProcessing
{

  //
  // Helper functions for extracting 12-bit pixel sequences
  // packed efficiently into a byte array, with 3 values.
  //

  //
  // Unsigned 12 bit pixel values :
  //
  // Let's represent a 12 bit value as two bytes :
  //
  //   +-----------------+-----------------+
  //   | 0 0 0 0 A A A A | a a a a a a a a |  
  //   +-----------------+-----------------+
  //     |                 |
  //     Most              Least
  //     significant       significant
  //     byte              byte
  //
  // We can pack two 12-bit values into three bytes.
  //
  // There are several possible formats for the underlying byte array,
  // depending on how the 4-bit 'nibbles' are assigned.
  // The most obvious format is :
  //
  //   +--------+--------+--------+--
  //   |aaaaaaaa|AAAAbbbb|bbbbBBBB|    aa_Ab_bB
  //   +--------+--------+--------+--
  //       0        1        2       
  //
  // Other reasonable possibilities are
  //
  //   +--------+--------+--------+--
  //   |aaaaaaaa|bbbbAAAA|bbbbBBBB|    aa_bA_bB
  //   +--------+--------+--------+--
  //
  //   +--------+--------+--------+--
  //   |aaaaaaaa|AAAAbbbb|BBBBbbbb|    aa_Ab_Bb 
  //   +--------+--------+--------+--
  //
  //   +--------+--------+--------+--
  //   |aaaaaaaa|bbbbAAAA|BBBBbbbb|    aa_bA_Bb  
  //   +--------+--------+--------+--
  //
  // Hmm, there are other possibiites if the first byte is AAAA ...
  // Ouch !!
  //

  public static class MultiBytePixelHelpers_12Bit_old_01
  {

    public enum NibbleFormat {
      aa_Ab_bB,
      aa_bA_bB,
      aa_Ab_Bb,
      aa_bA_Bb
    }

    public static System.UInt16[] DecodeByteArray_12BitPacked ( System.Byte[] data, NibbleFormat nibbleFormat ) 
    { 
      int nThreeByteSegments = data.Length / 3 ;
      int nResultPairs = nThreeByteSegments * 2 ;
      System.UInt16[] result = new System.UInt16[nResultPairs*2] ;
      int iSourceByte = 0 ;
      int iResultValue = 0 ;
      for ( int iResultPair = 0 ; iResultPair < nResultPairs ; iResultPair++ ) 
      {
        byte byte_0 = data[iSourceByte++] ;
        byte byte_1 = data[iSourceByte++] ;
        byte byte_2 = data[iSourceByte++] ;
        var (a12,b12) = DecodeThreeBytesIntoTwo12BitValues(
          byte_0, 
          byte_1, 
          byte_2,
          nibbleFormat
        ) ;
        result[iResultValue++] = a12 ;
        result[iResultValue++] = b12 ;
      }
      return result ;
    }

    public static (System.UInt16 a12, System.UInt16 b12) DecodeThreeBytesIntoTwo12BitValues ( 
      byte         byte_0, 
      byte         byte_1, 
      byte         byte_2, 
      NibbleFormat nibbleFormat 
    ) {
      UnpackNibbles ( 
        byte_1, 
        byte_2, 
        nibbleFormat,
        out byte A, 
        out byte b, 
        out byte B
      ) ;
      var a12 = (ushort) (
        byte_0
      | A << 8
      ) ;
      var b12 = (ushort) (
        b
      | B << 8
      ) ;
      return (a12,b12) ;
    }

    public static void UnpackNibbles ( 
      byte         byte_1, 
      byte         byte_2, 
      NibbleFormat nibbleFormat, 
      out byte     A, 
      out byte     b, 
      out byte     B 
    ) {
      A = 0 ; 
      b = 0 ; 
      B = 0 ;
    }

  }

}

