//
// OverlayCrossDescriptor.cs
//

namespace Clf.Common.ImageProcessing
{

  public record OverlayCrossDescriptor (
    OverlayLineDescriptor_Horizontal HorizontalLine,
    OverlayLineDescriptor_Vertical   VerticalLine
  ) : 
  CanDrawOntoColouredPixelArray
  {

    public static OverlayCrossDescriptor FromCentrePoint ( 
      int           x, 
      int           y, 
      int           heightAndWidth, 
      RgbByteValues colour,
      bool          thick
    ) => new OverlayCrossDescriptor(
      HorizontalLine : OverlayLineDescriptor_Horizontal .FromCentrePoint(x,y,heightAndWidth,colour,thick),
      VerticalLine   : OverlayLineDescriptor_Vertical   .FromCentrePoint(x,y,heightAndWidth,colour,thick)
    ) ;

    public override void Draw ( ColouredPixelArrayEncodedAsBytes_Base pixelsArray )
    {
      if ( CanDraw )
      {
        HorizontalLine.Draw(pixelsArray) ;
        VerticalLine.Draw(pixelsArray) ;
      }
    }

  }

}

