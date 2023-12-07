//
// IProvidesToolTipContent.cs
//

using System.Collections.Generic;

namespace Clf.Common.UI
{

  public interface IProvidesToolTipContent
  {
  }

  public interface IProvidesTextualToolTipContent : IProvidesToolTipContent
  {
    IEnumerable<string> ToolTipTextLines { get ; }
    // IEnumerable<string> ToolTipTextLines => new []{
    //   "Tooltip info is not available",
    //   $"for this '{this.GetType().Name}'"
    // } ;
  }

  public interface IProvidesToolTipContentKey : IProvidesToolTipContent
  {
    // The tooltip provider publishes a textual 'key'
    // that we can use to look up the actual Content 
    // in a dictionary of some kind ...
    string ToolTipContentKey { get ; }
  }

}
