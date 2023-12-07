//
// IContextMenuHandler.cs
//

namespace Clf.Common.MenuHandling
{

  public interface IContextMenuHandler
  {
    IContextMenu CreateContextMenu ( ) ;
    IContextMenuParentItem? GetContextMenuParentItemIfAvailable ( string name ) ;
  }

  public interface IContextMenu
  {
    // IMPROVE_THIS : add 'isChecked' ??
    void AddActionItem (
      string        itemText,
      System.Action action,
      bool          isEnabled = true
    ) ;
    void AddSeparator ( ) ;
    IContextMenu AddParentItem (
      string itemText
    ) ;
    int ItemsCount { get ; }
  }

  public interface ITopLevelContextMenu : IContextMenu
  {
    void Show ( ) ;
  }

  public interface IContextMenuParentItem : IContextMenu
  {
  }

}
