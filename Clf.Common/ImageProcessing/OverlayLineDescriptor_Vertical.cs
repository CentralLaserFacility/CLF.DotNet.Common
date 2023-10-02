//
// OverlayLineDescriptor_Vertical.cs
//

namespace Clf.Common.ImageProcessing
{

  public record OverlayLineDescriptor_Vertical (
    RgbByteValues Colour,
    bool          Thick,
    int           X,
    int           YStart,
    int           YEnd
  ) : 
  OverlayDescriptor(
    Colour,Thick
  ) { 

    public static OverlayLineDescriptor_Vertical FromCentrePoint ( 
      int           X, 
      int           Y, 
      int           Height, 
      RgbByteValues colour,
      bool          thick 
    ) => new OverlayLineDescriptor_Vertical(
      colour,
      thick,
      X,
      Y - Height / 2,
      Y + Height / 2
    ) ;

    public override void Draw ( ColouredPixelArrayEncodedAsBytes_Base pixelsArray )
    {
      if ( CanDraw )
      {
        if ( Thick )
        {
          for ( int y = YStart - 1 ; y <= YEnd + 1 ; y++ )
          {
            pixelsArray.SetPixel(
              X,
              y,
              Colour
            ) ;
            pixelsArray.SetPixel(
              X - 1,
              y,
              Colour
            );
            pixelsArray.SetPixel(
              X + 1,
              y,
              Colour
            ) ;
          }
        }
        else
        {
          for ( int y = YStart ; y <= YEnd ; y++ )
          {
            pixelsArray.SetPixel(
              X,
              y,
              Colour
            ) ;
          }
        }
      }
    }

  }

}

