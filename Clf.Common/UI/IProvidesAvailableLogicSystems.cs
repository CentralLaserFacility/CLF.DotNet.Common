// 
// IProvidesAvailableLogicSystems.cs
//

using System.Collections.Generic;

namespace Clf.Common.UI
{

  public interface IProvidesAvailableLogicSystems 
  {
    IEnumerable<System.Type> AvailableLogicSystemTypes { get ; }
  }

}