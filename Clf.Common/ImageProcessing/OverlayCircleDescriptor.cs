//
// OverlayCircleDescriptor.cs
//

namespace Clf.Common.ImageProcessing
{

  public record OverlayCircleDescriptor (
    RgbByteValues Colour,
    bool          Thick,
    int           XCentre,
    int           YCentre,
    int           Radius
  ) : 
  OverlayDescriptor(
    Colour,
    Thick
  ) { 

    public override void Draw ( ColouredPixelArrayEncodedAsBytes_Base pixelsArray )
    {
      if ( CanDraw )
      {
        Helpers.VisitPixelsOnCircle(
          XCentre,
          YCentre,
          Radius,
          (x,y,c) => pixelsArray.SetPixel_ThinOrThick(x,y,Colour,Thick)
        ) ;
      }
    }
 
  }

}

