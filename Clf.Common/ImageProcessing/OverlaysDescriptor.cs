//
// OverlaysDescriptor.cs
//

using Clf.Common.ExtensionMethods ;

namespace Clf.Common.ImageProcessing
{

  public record OverlaysDescriptor ( 
    params CanDrawOntoColouredPixelArray?[] Overlays
  ) : 
  CanDrawOntoColouredPixelArray
  {

    public override void Draw ( ColouredPixelArrayEncodedAsBytes_Base colouredPixelsArray ) 
    {
      if ( CanDraw )
      {
        Overlays.ForEachItem(
          overlay => overlay?.Draw(colouredPixelsArray)
        ) ;
      }
    }

  }

}

