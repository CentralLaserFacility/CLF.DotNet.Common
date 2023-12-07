//
// IProvidesStackingOrderIndicatorZ.cs
//

namespace Clf.Common.UI
{

  //
  // Sometimes graphical items overlap.
  //
  // We draw things that have higher 'Z' numbers
  // stacked 'on top' of lower numbered items.
  //
  //    +--------------+
  //    | Z = 0        | LOWEST Z AT THE BOTTOM
  //    |              |
  //    |   +-------------------+
  //    |   | Z = 3             | HIGHEST Z AT TOP OF STACK
  //    |   |                   |
  //    |   |                   |--+
  //    |   |                   |  |
  //    |   +-------------------+  |
  //    |      |                   |
  //    |      | Z = 2             |
  //    |      +-------------------+
  //    |              |
  //    +--------------+
  //

  // IProvidesStackingSequenceNumber_Z_Index ??

  public interface IProvidesStackingOrderIndicatorZ
  {
    // Higher numbers => 'on top'
    int StackingOrderZ { get ; }
  }

}
