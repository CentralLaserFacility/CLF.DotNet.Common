//
// RotationFactor.cs
//

namespace Clf.Common.ImageProcessing
{

  //
  // Is it useful to mix 'Rotation' and 'Mirroring' like this ??
  //
  // Would be cleaner as distinct operations that we can chain together.
  // However it's more efficient to do the operations in a single pass.
  //

  public enum RotationFactor {
    None,                                             //  0 1 2 ; 3 4 5
    RotateClockwise90,                                //  3 0   ; 4 1   ; 5 2
    RotateClockwise180,                               //  5 4 3 ; 2 1 0
    RotateClockwise270,                               //  2 5   ; 1 4   ; 0 3
    MirrorAroundVerticalCentre,                       //  2 1 0 ; 5 4 3
    MirrorAroundHorizontalCentre,                     //  3 4 5 ; 0 1 2
    RotateClockwise90_ThenMirrorAroundVerticalCentre, //  3 0   ; 4 1   ; 5 2
    RotateAntiClockwise90  = RotateClockwise270,
    RotateAntiClockwise180 = RotateClockwise180,
    RotateAntiClockwise270 = RotateClockwise90,
    // Names used in the PV ...
    Rot90       = RotateClockwise90,
    Rot180      = RotateClockwise180,
    Rot270      = RotateClockwise270,
    Rot90Mirror = RotateClockwise90_ThenMirrorAroundVerticalCentre
  }

  // public enum MirrorFactor {
  //   MirrorAroundVerticalCentre,    //  2 1 0 ; 5 4 3
  //   MirrorAroundHorizontalCentre,  //  3 4 5 ; 0 1 2
  // }

}

