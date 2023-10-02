//
// GraphDataPointMapper.cs
//

using Clf.Common.Graphs;

namespace Clf.Blazor.Complex.IntensityMap.Helpers
{

  // Maps from Data coordinates to ViewBox coordinates,
  // useful when we want to map a data point to the
  // corresponding 'pixel' coordinates in the display area.

  public record GraphDataPointMapper ( 
    GraphAxisRange FromDataRangeX,
    GraphAxisRange FromDataRangeY,
    GraphAxisRange ToViewBoxRangeX,
    GraphAxisRange ToViewBoxRangeY
  ) {

    public GraphPointEx this [ GraphPointEx from ]
    => (
      new (
        ToViewBoxRangeX.ValueAtFracAcross(
          FromDataRangeX.ComputeFracAcross(from.X)
        ),
        ToViewBoxRangeY.ValueAtFracAcross(
          FromDataRangeY.ComputeFracAcross(from.Y)
        )
      )
    ) ;

    public double MapX ( double x )
    => ToViewBoxRangeX.ValueAtFracAcross(
      FromDataRangeX.ComputeFracAcross(x)
    ) ;

    public double MapY ( double y )
    => ToViewBoxRangeY.ValueAtFracAcross(
      FromDataRangeY.ComputeFracAcross(y)
    ) ;

    // The 'inverse' mapper gets us from
    // ViewBox coordinates to Data coordinates

    public GraphViewBoxPointMapper Inverse 
    => new(
      FromViewBoxRangeX : ToViewBoxRangeX,
      FromViewBoxRangeY : ToViewBoxRangeY,
      ToDataRangeX      : FromDataRangeX,
      ToDataRangeY      : FromDataRangeY
    ) ;

    public static void DoTests ( )
    {
      var mapper = new GraphDataPointMapper(
        FromDataRangeX  : new(0.0,1.0),
        FromDataRangeY  : new(0.0,2.0),
        ToViewBoxRangeX : new(10.0,110.0),
        ToViewBoxRangeY : new(20.0,250.0)
      ) ;
      GraphPointEx data = new( 
        mapper.FromDataRangeX.MidPoint,
        mapper.FromDataRangeY.MidPoint
      ) ;
      var mapped = mapper[data] ;
      var data2 = mapper.Inverse[mapped] ;
    }

  }

}
