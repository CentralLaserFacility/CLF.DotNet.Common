//
// OverlayClosedPolygonDescriptor.cs
//

using System.Collections.Generic;
using System.Linq;

namespace Clf.Common.ImageProcessing
{

  public record OverlayClosedPolygonDescriptor (
    RgbByteValues               Colour,
    bool                        Thick,
    IEnumerable<(int x, int y)> Points
  ) : 
  OverlayDescriptor(
    Colour,
    Thick
  ) { 

    public static OverlayClosedPolygonDescriptor Create (
      RgbByteValues colour,
      bool          thick,
      params int[]  values
    ) {
      return new OverlayClosedPolygonDescriptor(
        colour,
        thick,
        Helpers.GetValuesAsEnumerableOfTuples(values)
      ) ;
    }

    public static OverlayClosedPolygonDescriptor CreateTiltedBox (
      int           centreX, 
      int           centreY, 
      int           height, 
      int           width, 
      double        tiltAngle_anticlockwiseDegrees,
      RgbByteValues colour, 
      bool          thick
    ) {
      double tiltAngle_anticlockwiseRadians = tiltAngle_anticlockwiseDegrees * System.Math.PI / 180.0 ;
      double sinTheta = System.Math.Sin(tiltAngle_anticlockwiseRadians) ;
      double cosTheta = System.Math.Cos(tiltAngle_anticlockwiseRadians) ;
      int halfWidthX  = width / 2 ;
      int halfHeightY = height / 2 ;
      (int,int)[] cornerPoints = new[]{
        GetRotatedAndOffsettedPoint( -halfWidthX , -halfHeightY ),
        GetRotatedAndOffsettedPoint( -halfWidthX , +halfHeightY ),
        GetRotatedAndOffsettedPoint( +halfWidthX , +halfHeightY ),
        GetRotatedAndOffsettedPoint( +halfWidthX , -halfHeightY )
      } ;
      return new OverlayClosedPolygonDescriptor(
        colour,
        thick,
        cornerPoints
      ) ;
      ( int x, int y ) GetRotatedAndOffsettedPoint ( int x, int y ) 
      => (
        x : (int) (  x * cosTheta + y * sinTheta ) + centreX,
        y : (int) ( -x * sinTheta + y * cosTheta ) + centreY
      ) ;
    }

    public override void Draw ( ColouredPixelArrayEncodedAsBytes_Base pixelsArray )
    {
      if ( CanDraw )
      {
        if ( Points.Count() >= 2 ) 
        {
          var start = Points.Last() ;
          foreach ( var end in Points ) 
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

