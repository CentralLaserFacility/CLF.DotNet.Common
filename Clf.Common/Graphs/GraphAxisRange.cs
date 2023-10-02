//
// GraphAxisRange.cs
//

using System.Collections.Generic;

namespace Clf.Common.Graphs
{

  public record GraphAxisRange(double Min, double Max)
  {

    public double Span => Max - Min;

    public double MidPoint => ValueAtFracAcross(0.5);

    public double ComputeFracAcross(double value)
    => (value - Min) / Span;

    public double ValueAtFracAcross(double frac01)
    =>
      Min
    + frac01 * Span
    ;

    public IEnumerable<double> TickCoordinates(int n = 11)
    {
      double tickSpacing = Span / (n - 1);
      for (double y = Min; y < Max; y += tickSpacing)
      {
        yield return y;
      }
      yield return Max;
    }

  }

}
