//
// RgbByteValues.cs
//

namespace Clf.Common.ImageProcessing
{

  public record struct RgbByteValues ( byte R, byte G, byte B )
  {

    public static readonly RgbByteValues Red   = new ( 255 , 0   , 0   ) ;
    public static readonly RgbByteValues Green = new ( 0   , 255 , 0   ) ;
    public static readonly RgbByteValues Blue  = new ( 0   , 0   , 255 ) ;

    public static readonly RgbByteValues Black = new (   0 , 0   , 0   ) ;
    public static readonly RgbByteValues White = new ( 255 , 255 , 255 ) ;
    public static readonly RgbByteValues Grey  = new ( 127 , 127 , 127 ) ;

    public static readonly RgbByteValues LightGrey   = new ( 200 , 200 , 200 ) ;
    public static readonly RgbByteValues LightBlue   = new ( 200 , 200 , 255 ) ;
    public static readonly RgbByteValues LightGreen  = new ( 220 , 255 , 220 ) ;

    public static RgbByteValues FromHexEncodedString ( 
      string hexEncodedString_RGB // Format is '#rrggbb' or "0xrrggbb"
    ) {
      hexEncodedString_RGB = hexEncodedString_RGB.Replace("0x","#") ;
      return new RgbByteValues(
        byte.Parse( hexEncodedString_RGB.Substring(1,2), System.Globalization.NumberStyles.HexNumber ),
        byte.Parse( hexEncodedString_RGB.Substring(3,2), System.Globalization.NumberStyles.HexNumber ),
        byte.Parse( hexEncodedString_RGB.Substring(5,2), System.Globalization.NumberStyles.HexNumber )
      ) ;
    }
    
    public static RgbByteValues FromRgbTuple ( ( byte r, byte g, byte b ) rgb )
    => new RgbByteValues(rgb.r,rgb.g,rgb.b) ;

    public ( byte r, byte g, byte b ) AsRgbTuple => (R,G,B) ;

    public static RgbByteValues FromEncodedARGB ( uint colour_ARGB )
    => FromRgbTuple(
      PixelEncodingHelpers.DecodeARGBAsRgbTuple(colour_ARGB)
    ) ;

    public static RgbByteValues FromEncodedABGR ( uint colour_ABGR )
    => FromRgbTuple(
      PixelEncodingHelpers.DecodeABGRAsRgbTuple(colour_ABGR)
    ) ;

    private uint? m_integerValue_ARGB = null ;

    public uint AsPackedInteger_ARGB => m_integerValue_ARGB ??= PixelEncodingHelpers.EncodeARGB(R,G,B) ;

    private uint? m_integerValue_ABGR = null ;
    public uint AsPackedInteger_ABGR => m_integerValue_ABGR ??= PixelEncodingHelpers.EncodeABGR(R,G,B) ;

    public static void DoTests ( )
    {
      var r = RgbByteValues.FromHexEncodedString("#ff0000") ;
      var g = RgbByteValues.FromHexEncodedString("#00ff00") ;
      var b = RgbByteValues.FromHexEncodedString("#0000ff") ;
      var r_int = r.AsPackedInteger_ARGB ;
    }

  }

}

