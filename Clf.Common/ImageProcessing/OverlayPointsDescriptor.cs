//
// OverlayClosedPolygonDescriptor.cs
//

using System.Collections.Generic ;

namespace Clf.Common.ImageProcessing
{
  public record OverlayPointsDescriptor (
    RgbByteValues               Colour,
    bool                        Thick,
    IEnumerable<(int x, int y)> Points
  ) : 
  OverlayDescriptor(
    Colour,
    Thick
  ) { 

    public static OverlayPointsDescriptor Create (
      RgbByteValues colour,
      bool          thick,
      params int[]  values
    ) {
      return new OverlayPointsDescriptor(
        colour,
        thick,
        Helpers.GetValuesAsEnumerableOfTuples(values)
      ) ;
    }

    public override void Draw ( ColouredPixelArrayEncodedAsBytes_Base pixelsArray )
    {
      if ( CanDraw )
      {
        foreach ( var point in Points ) 
        {
          pixelsArray.SetPixel_ThinOrThick(
            point.x,
            point.y,
            Colour,
            Thick
          ) ;
        }
      }
    }

  }

}

