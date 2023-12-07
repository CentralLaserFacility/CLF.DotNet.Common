//
// ContextMenuHandler_BuildingViewModelsTree.cs
//

namespace Clf.Common.MenuHandling
{

  public class ContextMenuHandler_BuildingViewModelsTree : IContextMenuHandler
  {
    public IContextMenu CreateContextMenu ( )
    {
      return new ContextMenu_BuildingViewModelsTree() ;
    }
    public IContextMenuParentItem? GetContextMenuParentItemIfAvailable ( string name ) => null ;
  }

  public class ContextMenu_BuildingViewModelsTree : IContextMenu
  {
    private ContextMenuViewModel m_topLevelViewModel = new ContextMenuViewModel() ;
    public ContextMenu_BuildingViewModelsTree ( )
    { }
    public void AddActionItem (
      string        itemText,
      System.Action action,
      bool          isEnabled
    ) {
      m_topLevelViewModel.AddActionItem(
        itemText, 
        action, 
        isEnabled
      ) ;  
    }
    public void AddSeparator ( )
    {
      m_topLevelViewModel.AddSeparator() ;  
    }
    public IContextMenu AddParentItem (
      string itemText
    ) {
      return m_topLevelViewModel.AddParentItem(itemText) ;
    }
    public int ItemsCount => 0 ;
    public void Show ( )
    {
      m_topLevelViewModel.IsVisible = true ;
    }
  }
}
