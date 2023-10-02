//
// GraphPointEx.cs
//

namespace Clf.Common.Graphs
{

  public record GraphPointEx(double X, double Y)
  {
    public string AsSvgString => $"{X},{Y}";
  }

}
