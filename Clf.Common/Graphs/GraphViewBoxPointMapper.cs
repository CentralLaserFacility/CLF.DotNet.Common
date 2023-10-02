//
// GraphViewBoxPointMapper.cs
//

using Clf.Common.Graphs;

namespace Clf.Blazor.Complex.IntensityMap.Helpers
{

  // Transforms coordinates from 'ViewBox' coordinates to 'Data' coordinates.
  // Useful when we want to map a mouse-point position to the corresponding
  // coordinates in the displayed data.

  public record GraphViewBoxPointMapper ( 
    GraphAxisRange FromViewBoxRangeX,
    GraphAxisRange FromViewBoxRangeY,
    GraphAxisRange ToDataRangeX,
    GraphAxisRange ToDataRangeY
  ) {

    public GraphPointEx this [ GraphPointEx from ]
    => (
      new (
        ToDataRangeX.ValueAtFracAcross(
          FromViewBoxRangeX.ComputeFracAcross(from.X)
        ),
        ToDataRangeY.ValueAtFracAcross(
          FromViewBoxRangeY.ComputeFracAcross(from.Y)
        )
      )
    ) ;

    public double MapX ( double x )
    => ToDataRangeX.ValueAtFracAcross(
      FromViewBoxRangeX.ComputeFracAcross(x)
    ) ;

    public double MapY ( double y )
    => ToDataRangeY.ValueAtFracAcross(
      FromViewBoxRangeY.ComputeFracAcross(y)
    ) ;

    // The 'inverse' mapper gets us from
    // Data coordinates to ViewBox coordinates

    public GraphDataPointMapper Inverse 
    => new(
      FromDataRangeX  : ToDataRangeX,
      FromDataRangeY  : ToDataRangeY,
      ToViewBoxRangeX : FromViewBoxRangeX,
      ToViewBoxRangeY : FromViewBoxRangeY
    ) ;

    public static void DoTests ( )
    {
      var mapper = new GraphViewBoxPointMapper(
        FromViewBoxRangeX : new(0.0,1.0),
        FromViewBoxRangeY : new(0.0,2.0),
        ToDataRangeX      : new(10.0,110.0),
        ToDataRangeY      : new(20.0,250.0)
      ) ;
      GraphPointEx data = new( 
        mapper.FromViewBoxRangeX.MidPoint,
        mapper.FromViewBoxRangeY.MidPoint
      ) ;
      var mapped = mapper[data] ;
      var data2 = mapper.Inverse[mapped] ;
    }

  }

}
