//
// OverlayOpenPolygonDescriptor.cs
//

using System.Collections.Generic;
using System.Linq;

namespace Clf.Common.ImageProcessing
{

  public record OverlayOpenPolygonDescriptor (
    RgbByteValues               Colour,
    bool                        Thick,
    IEnumerable<(int x, int y)> Points
  ) : 
  OverlayDescriptor(
    Colour,
    Thick
  ) { 

    public static OverlayOpenPolygonDescriptor Create (
      RgbByteValues colour,
      bool          thick,
      params int[]  values
    ) {
      return new OverlayOpenPolygonDescriptor(
        colour,
        thick,
        Helpers.GetValuesAsEnumerableOfTuples(values)
      ) ;
    }

    public override void Draw ( ColouredPixelArrayEncodedAsBytes_Base pixelsArray )
    {
      if ( CanDraw )
      {
        if ( Points.Count() >= 2 ) 
        {
          var start = Points.First() ;
          foreach ( var end in Points.Skip(1) ) 
          {
            Helpers.VisitPixelsOnLine(
              start.x,
              start.y,
              end.x,
              end.y,
              (x,y) => pixelsArray.SetPixel_ThinOrThick(x,y,Colour,Thick)
            ) ;
            start = end ;
          }
        }
      }
    }

  }

}

