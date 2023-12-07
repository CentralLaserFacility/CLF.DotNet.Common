//
// IHandlesMouseButtonEvents.cs
//

using Clf.Common.MenuHandling;

namespace Clf.Common.UI
{
  public interface IHandlesMouseButtonEvents
  {
    void HandleMouseRightButtonEvent_PopulatingContextMenu ( MenuDescriptor contextMenu ) ;
    void HandleMouseLeftButtonEvent ( ) ;
  }
}
