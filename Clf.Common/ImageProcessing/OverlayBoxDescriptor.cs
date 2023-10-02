//
// OverlayBoxDescriptor.cs
//

namespace Clf.Common.ImageProcessing
{

  public record OverlayBoxDescriptor (
    OverlayLineDescriptor_Horizontal TopHorizontalLine,
    OverlayLineDescriptor_Horizontal BottomHorizontalLine,
    OverlayLineDescriptor_Vertical   LeftVerticalLine,
    OverlayLineDescriptor_Vertical   RightVerticalLine
  ) 
  : CanDrawOntoColouredPixelArray
  {

    public static OverlayBoxDescriptor FromCentrePoint ( 
      int           x, 
      int           y, 
      int           height, 
      int           width, 
      RgbByteValues colour, 
      bool          thick
    ) => new (
      TopHorizontalLine    : OverlayLineDescriptor_Horizontal .FromCentrePoint( x           , y - height/2 , width  , colour, thick ),
      BottomHorizontalLine : OverlayLineDescriptor_Horizontal .FromCentrePoint( x           , y + height/2 , width  , colour, thick ),
      LeftVerticalLine     : OverlayLineDescriptor_Vertical   .FromCentrePoint( x - width/2 , y            , height , colour, thick ),
      RightVerticalLine    : OverlayLineDescriptor_Vertical   .FromCentrePoint( x + width/2 , y            , height , colour, thick )
    ) ;

    public override void Draw ( ColouredPixelArrayEncodedAsBytes_Base pixelsArray )
    {
      if ( CanDraw )
      {
        TopHorizontalLine.Draw(pixelsArray);
        BottomHorizontalLine.Draw(pixelsArray);
        LeftVerticalLine.Draw(pixelsArray);
        RightVerticalLine.Draw(pixelsArray);
      }
    }

  }

}

