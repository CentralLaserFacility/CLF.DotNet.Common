//
// PixelEncodingHelpers.cs
//

namespace Clf.Common.ImageProcessing
{

  public static class PixelEncodingHelpers
  {

    //
    // This is the Windows 'ARGB' format for encoding a colour in 32 bits,
    // as used in bitmaps and so on ...
    //
    //  +-------+-------+-------+-------+
    //  |   A   |   R   |   G   |   B   |
    //  +-------+-------+-------+-------+
    //     255                     LSB
    //    alpha
    //

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static uint EncodeARGB ( byte red, byte green, byte blue )
    => (uint) (
      ( 0xff  << 24 ) // A : most significant byte is 'alpha'
    | ( red   << 16 ) // R
    | ( green <<  8 ) // G
    | ( blue  <<  0 ) // B
    ) ;

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static void DecodeARGB ( uint encodedColour_ARGB, out byte red, out byte green, out byte blue )
    {
      blue  = (byte) ( encodedColour_ARGB >>  0 ) ;
      green = (byte) ( encodedColour_ARGB >>  8 ) ;
      red   = (byte) ( encodedColour_ARGB >> 16 ) ;
    }

    // [System.Runtime.CompilerServices.MethodImpl(
    //   System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    // )]
    // public static ( byte r, byte g, byte b ) DecodeARGBAsRgbTuple ( uint e ncodedColour_ARGB )
    // {
    //   DecodeARGB(
    //     encodedColour_ARGB,
    //     out byte red, 
    //     out byte green, 
    //     out byte blue 
    //   ) ;
    //   return (red,green,blue) ;
    // }

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static ( byte r, byte g, byte b ) DecodeARGBAsRgbTuple ( uint encodedColour_ARGB )
    => (
      r : (byte) ( encodedColour_ARGB >> 16 ),
      g : (byte) ( encodedColour_ARGB >>  8 ),
      b : (byte) ( encodedColour_ARGB >>  0 )
    ) ;

    //
    // The HTML DOM Canvas defines an image format that has bytes
    // in the following order :
    //
    //   Offsets into the byte-array passed to the DOM Canvas :
    //   
    //   | 000 | 001 | 002 | 003 | 004 | 005 | 006 | 007 | 008 | 009 | 010 | 011 | 
    //   +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+- - - -
    //   |  R  |  G  |  B  |  A  |  R  |  G  |  B  |  A  |  R  |  G  |  B  |  A  | 
    //   +-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+-----+- - - -
    //   |        pixel 0        |        pixel 1        |        pixel 2        |
    //
    // For best efficiency, we'll want to read and write pixels in 4-byte chunks
    // rather than accessing the array bytes individually.
    //
    // In order to write a 32-bit colour, it need to be encoded like this :
    //
    //  +-------+-------+-------+-------+
    //  |   A   |   B   |   G   |  R    |
    //  +-------+-------+-------+-------+
    //     255                     LSB
    //    alpha
    //
    // So we'll define helper methods that convert an integer representation
    // between one format and the other.
    //

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static uint EncodeABGR ( byte red, byte green, byte blue )
    => (uint) (
      ( 0xff  << 24 ) // A : most significant byte is 'alpha'
    | ( blue  << 16 ) // B
    | ( green <<  8 ) // G
    | ( red   <<  0 ) // R
    ) ;

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static void DecodeABGR ( uint encodedColour_ABGR, out byte red, out byte green, out byte blue )
    {
      red   = (byte) ( encodedColour_ABGR >>  0 ) ;
      green = (byte) ( encodedColour_ABGR >>  8 ) ;
      blue  = (byte) ( encodedColour_ABGR >> 16 ) ;
    }

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static ( byte r, byte g, byte b ) DecodeABGRAsRgbTuple ( uint encodedColour_AGBR )
    {
      DecodeABGR(
        encodedColour_AGBR,
        out byte red, 
        out byte green, 
        out byte blue 
      ) ;
      return (red,green,blue) ;
    }

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static uint ConvertToABGR ( uint encodedColour_ARGB )
    {
      #if true
        DecodeARGB(
          encodedColour_ARGB,
          out byte r,
          out byte g,
          out byte b
        ) ;
        return EncodeABGR(r,g,b) ;
      #else
        // Expansion of DecodeARGB()
        byte blue  = (byte) ( encodedColour_ARGB >>  0 ) ;
        byte green = (byte) ( encodedColour_ARGB >>  8 ) ;
        byte red   = (byte) ( encodedColour_ARGB >> 16 ) ;
        // Expansion of EncodeAGBR()
        return (uint) (
          ( 0xff  << 24 ) // A : most significant byte is 'alpha'
        | ( blue  << 16 ) // B
        | ( green <<  8 ) // G
        | ( red   <<  0 ) // R
        ) ;
      #endif
    }

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static uint ConvertToARGB ( uint encodedColour_ABGR )
    {
      #if true
        DecodeABGR(
          encodedColour_ABGR,
          out byte r,
          out byte g,
          out byte b
        ) ;
        return EncodeARGB(r,g,b) ;
      #else
        // Expansion of DecodeABGR()
        byte red   = (byte) ( encodedColour_ABGR >> 16 ) ;
        byte green = (byte) ( encodedColour_ABGR >>  8 ) ;
        byte blue  = (byte) ( encodedColour_ABGR >>  0 ) ;
        // Expansion of EncodeARGB()
        return (uint) (
          ( 0xff  << 24 ) // A : most significant byte is 'alpha'
        | ( red   << 16 ) // B
        | ( green <<  8 ) // G
        | ( blue  <<  0 ) // R
        ) ;
      #endif
    }

  }

}

