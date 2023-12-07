//
// ContextMenuViewModels.cs
//

using System.Collections.Generic;
using System.Linq;

namespace Clf.Common.MenuHandling
{

  public static class ContextMenuViewModelExampleBuilder
  {

    public static ContextMenuViewModel BuildExampleDirectly ( System.Action<string>? doAction = null )
    {
      doAction ??= (message) => System.Diagnostics.Debug.WriteLine(message) ;
      var topLevelViewModel = new ContextMenuViewModel() ;
      topLevelViewModel.AddActionItem("Action #1",()=>doAction("Action #1")) ;
      topLevelViewModel.AddActionItem("Action #2",()=>doAction("Action #2")) ;
      var parent = topLevelViewModel.AddParentItem("Parent") ;
      parent.AddActionItem("Action #3",()=>doAction("Action #3")) ;
      parent.AddSeparator() ;
      parent.AddActionItem("Action #4",()=>doAction("Action #4"),isEnabled:false) ;
      var next = parent.AddParentItem("Next") ;
      return topLevelViewModel ;
    }

    public static IContextMenu BuildExampleUsingContextMenuHandler ( System.Action<string> doAction )
    {
      var contextMenuHandler = new ContextMenuHandler_BuildingViewModelsTree() ;
      var topLevelViewModel = contextMenuHandler.CreateContextMenu() ;
      topLevelViewModel.AddActionItem("Action #1",()=>doAction("Action #1")) ;
      topLevelViewModel.AddActionItem("Action #2",()=>doAction("Action #2")) ;
      var parent = topLevelViewModel.AddParentItem("Parent") ;
      parent.AddActionItem("Action #3",()=>doAction("Action #3")) ;
      parent.AddActionItem("Action #4",()=>doAction("Action #4"),isEnabled:false) ;
      return topLevelViewModel ;
    }
    public static void Scan ( 
      ContextMenuViewModel  contextMenuViewModel,
      System.Action<string> onItemVisited
    ) {
      foreach ( var item in contextMenuViewModel.Items )
      {
        switch ( item )
        {
        case ContextMenuActionItemViewModel actionItem:
          onItemVisited(actionItem.ItemText) ;
          break ;
        case ContextMenuParentItemViewModel parentItem:
          onItemVisited(parentItem.ItemText) ;
          Scan(parentItem.ContextMenuViewModel,onItemVisited) ;
          break ;
        case ContextMenuSeparatorViewModel separatorItem:
          onItemVisited("separator") ;
          break ;
        }
      }
    }
  }

  //
  // A ContextMenu holds a collection of ContextMenuItemBase instances,
  // which can be
  //   ContextMenuItem
  //   ContextMenuParentItem
  //   ContextMenuSeparator
  //
  // A ContextMenuActionItem represents an item that can trigger an Action.
  //
  // A ContextMenuParentItem holds a ContextMenu at the next level of nesting.
  //
  //        ContextMenu
  //     +----------------+
  //     | Action Item    |
  //     +----------------+
  //     | Action Item    |        ContextMenu
  //     +----------------+     +----------------+
  //     | Parent Item    |---->| Action Item    |
  //     +----------------+     +----------------+
  //     | Action Item    |     | Action Item    |
  //     +----------------+     +----------------+
  //                            | Parent Item    |----> etc
  //                            +----------------+ 
  //
  //

  public record ContextMenuItemViewModelBase
  { }
  
  public record ContextMenuViewModel : ContextMenuItemViewModelBase, IContextMenu
  {
    public ContextMenuViewModel? Parent ;
    public bool IsTopLevelMenu => Parent is null ;
    public int NestingLevel
    => (
      Parent is null
      ? 0
      : Parent.NestingLevel + 1
    ) ;
    public ContextMenuViewModel TopLevelMenu
    => (
      Parent is null
      ? this
      : Parent.TopLevelMenu
    ) ;
    private List<ContextMenuItemViewModelBase> m_items = new() ;
    public IEnumerable<ContextMenuItemViewModelBase> Items => m_items ;
    public void AddActionItem (
      string        itemText,
      System.Action action,
      bool          isEnabled = true
    ) {
      m_items.Add(
        new ContextMenuActionItemViewModel(itemText,action,isEnabled)
      ) ;
    }
    public void AddSeparator ( )
    {
      m_items.Add(
        new ContextMenuSeparatorViewModel()
      ) ;
    }
    public IContextMenu AddParentItem (
      string itemText
    ) {
      var nextLevelContextMenu = new ContextMenuViewModel(){
        Parent = this
      } ;
      m_items.Add(
        new ContextMenuParentItemViewModel(itemText,nextLevelContextMenu)
      ) ;
      return nextLevelContextMenu ;
    }
    public ContextMenuParentItemViewModel? GetContextMenuParentItemIfAvailable ( string name )
    => m_items.OfType<ContextMenuParentItemViewModel>().Where(
      item => item.ItemText == name 
    ).SingleOrDefault() ;
    public int ItemsCount => m_items.Count() ;
    public bool IsVisible { get ; set ; }
  }

  public record ContextMenuActionItemViewModel ( 
    string        ItemText,
    System.Action Action,
    bool          IsEnabled
  ) : ContextMenuItemViewModelBase
  { }
  
  public record ContextMenuParentItemViewModel (
    string               ItemText,
    ContextMenuViewModel ContextMenuViewModel
  ) : ContextMenuItemViewModelBase() ;

  public record ContextMenuSeparatorViewModel ( 
  ) : ContextMenuItemViewModelBase
  { }
  
}
