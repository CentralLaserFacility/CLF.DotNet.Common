//
// GraphDataCoordinateMapper.cs
//

namespace Clf.Common.Graphs
{

  // Maps from Data coordinates to ViewBox coordinates

  public record GraphDataCoordinateMapper(
    GraphAxisRange FromDataRange,
    GraphAxisRange ToViewBoxRange
  )
  {

    public double this[double from]
    => ToViewBoxRange.ValueAtFracAcross(
      FromDataRange.ComputeFracAcross(from)
    );

  }

}
