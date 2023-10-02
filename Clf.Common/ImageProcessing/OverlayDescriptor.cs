//
// OverlayDescriptor.cs
//

namespace Clf.Common.ImageProcessing
{

  public abstract record OverlayDescriptor ( 
    RgbByteValues Colour, 
    bool          Thick = false 
  ) 
  : CanDrawOntoColouredPixelArray
  { 
  }

}

