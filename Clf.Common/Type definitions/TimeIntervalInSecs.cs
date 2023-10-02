//
// TimeIntervalInSecs.cs
//

namespace Clf.Common
{

  public record TimeIntervalInSecs ( double Value ) 
  {

    public System.TimeSpan AsTimeSpan => System.TimeSpan.FromSeconds(Value) ;

    public static implicit operator System.TimeSpan ( TimeIntervalInSecs timeIntervalInSecs )
    => timeIntervalInSecs.AsTimeSpan ;

    public override string ToString ( ) => $"{Value:F2} seconds" ;

  }

}
