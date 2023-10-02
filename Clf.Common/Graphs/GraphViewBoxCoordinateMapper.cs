//
// GraphViewBoxCoordinateMapper.cs
//

namespace Clf.Common.Graphs
{

  // Maps from ViewBox coordinates to Data coordinates

  public record GraphViewBoxCoordinateMapper(
    GraphAxisRange FromViewBoxRange,
    GraphAxisRange ToDataRange
  )
  {

    public double this[double from]
    => ToDataRange.ValueAtFracAcross(
      FromViewBoxRange.ComputeFracAcross(from)
    );

  }

}
