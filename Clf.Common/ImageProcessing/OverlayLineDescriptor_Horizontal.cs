//
// OverlayLineDescriptor_Horizontal.cs
//

namespace Clf.Common.ImageProcessing
{

  public record OverlayLineDescriptor_Horizontal (
    RgbByteValues Colour,
    bool          Thick,
    int           XStart,
    int           XEnd,
    int           Y
  ) : 
  OverlayDescriptor(
    Colour,
    Thick
  ) { 

    public static OverlayLineDescriptor_Horizontal FromCentrePoint ( 
      int           x, 
      int           y, 
      int           width, 
      RgbByteValues colour,
      bool          thick
    ) => new OverlayLineDescriptor_Horizontal(
      colour,
      thick,
      x - width / 2,
      x + width / 2,
      y
    ) ;

    public override void Draw ( ColouredPixelArrayEncodedAsBytes_Base pixelsArray )
    {
      if ( CanDraw )
      {
        if ( Thick )
        {
          for ( int x = XStart - 1 ; x <= XEnd + 1 ; x++ )
          {
            pixelsArray.SetPixel(
              x,
              Y,
              Colour
            ) ;
            pixelsArray.SetPixel(
              x,
              Y - 1,
              Colour
            ) ;
            pixelsArray.SetPixel(
              x,
              Y + 1,
              Colour
            ) ;
          }

        }
        else
        {
          for ( int x = XStart ; x <= XEnd ; x++ )
          {
            pixelsArray.SetPixel(
              x,
              Y,
              Colour
            ) ;
          }
        }
      }
    }

  }

}

