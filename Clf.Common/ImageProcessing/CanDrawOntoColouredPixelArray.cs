//
// CanDrawOntoColouredPixelArray.cs
//

namespace Clf.Common.ImageProcessing
{

  public abstract record CanDrawOntoColouredPixelArray
  {

    public bool CanDraw { get ; set ; } = true ;

    public abstract void Draw ( ColouredPixelArrayEncodedAsBytes_Base pixelsArray ) ;

  }

}

