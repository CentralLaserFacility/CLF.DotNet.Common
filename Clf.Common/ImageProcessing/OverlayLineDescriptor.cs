//
// OverlayLineDescriptor.cs
//

namespace Clf.Common.ImageProcessing
{

  public record OverlayLineDescriptor (
    RgbByteValues Colour,
    bool          Thick,
    int           XStart,
    int           YStart,
    int           XEnd,
    int           YEnd
  ) : 
  OverlayDescriptor(
    Colour,
    Thick
  ) { 

    public override void Draw ( ColouredPixelArrayEncodedAsBytes_Base pixelsArray )
    {
      if ( CanDraw )
      {
        Helpers.VisitPixelsOnLine(
          XStart,
          YStart,
          XEnd,
          YEnd,
          (x,y) => pixelsArray.SetPixel_ThinOrThick(x,y,Colour,Thick)
        ) ;
      }
    }

  }

}

