//
// ColourDescriptor.cs
//

namespace Clf.Common.ImageProcessing
{

  //
  // Describes a colour in terms of RGB weights from 0.0 to 255.0
  //

  //
  // !!!!! Should represent integer colours as a record struct !!!!!!
  //

  public record struct ColourDescriptor ( double R, double G, double B )
  {

    public const double MAX = (
      255.0 
      // 254.9999 
    ) ;
    
    public const double HALF = (
      128.0 
    ) ;
    
    //                                                                      RED  GREEN   BLUE
    //                                                                        |      |      |
    public static readonly ColourDescriptor Red    = new ColourDescriptor ( MAX ,    0 ,    0 ) ;
    public static readonly ColourDescriptor Green  = new ColourDescriptor (   0 ,  MAX ,    0 ) ;
    public static readonly ColourDescriptor Blue   = new ColourDescriptor (   0 ,    0 ,  MAX ) ;

    public static readonly ColourDescriptor Cyan   = new ColourDescriptor (   0 ,  MAX ,  MAX ) ;
    public static readonly ColourDescriptor Yellow = new ColourDescriptor ( MAX ,  MAX ,    0 ) ;

    public static readonly ColourDescriptor Magenta = new ColourDescriptor ( MAX ,  MAX ,    0 ) ;
    public static readonly ColourDescriptor Orange  = new ColourDescriptor ( MAX , HALF ,    0 ) ;
    
    public static readonly ColourDescriptor Black = new ColourDescriptor  (   0 ,    0 ,    0 ) ;
    public static readonly ColourDescriptor White = new ColourDescriptor  ( MAX ,  MAX ,  MAX ) ;

    // https://www.canva.com/colors/color-meanings/lime-green/
    public static readonly ColourDescriptor Lime    = ColourDescriptor.FromByteValues(0x32,0xCD,0x32) ;

    public static ColourDescriptor FromByteValues ( byte R, byte G, byte B )
    {
      return new(R,G,B) ;
    }

    public static ColourDescriptor FromHexEncodedString ( string hexEncodedString_RGB )
    {
      // throw new System.ApplicationException("Just testing") ;
      var rgb = RgbByteValues.FromHexEncodedString(hexEncodedString_RGB) ;
      return new ColourDescriptor(
        rgb.R,
        rgb.G, 
        rgb.B
      ) ;
    }

    public static ColourDescriptor Interpolate ( ColourDescriptor from, ColourDescriptor to, double Frac01 )
    {
      return new(
        from.R + Frac01 * ( to.R - from.R ),
        from.G + Frac01 * ( to.G - from.G ),
        from.B + Frac01 * ( to.B - from.B )
      ) ;
    }

    //
    // !!!!! Should represent integer colours as a record struct !!!!!!
    //

    // [System.Runtime.CompilerServices.MethodImpl(
    //   System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    // )]
    public uint AsEncodedInteger_ARGB => (
      PixelEncodingHelpers.EncodeARGB(
        (byte) R,
        (byte) G,
        (byte) B
      )
    ) ;

    // [System.Runtime.CompilerServices.MethodImpl(
    //   System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    // )]
    public uint AsEncodedInteger_ABGR => (
      PixelEncodingHelpers.EncodeABGR(
        (byte) R,
        (byte) G,
        (byte) B
      )
    ) ;

  }

}

