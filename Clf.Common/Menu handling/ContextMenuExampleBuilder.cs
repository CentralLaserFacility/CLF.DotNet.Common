//
// ContextMenuExampleBuilder.cs
//

namespace Clf.Common.MenuHandling
{

  public static class ContextMenuExampleBuilder
  {

    public static ContextMenuDescriptor BuildExample ( System.Action<string>? doAction = null )
    {
      doAction ??= (message) => System.Diagnostics.Debug.WriteLine(message) ;
      var contextMenu = new ContextMenuDescriptor() ;
      contextMenu.AddActionItem("Action #1",()=>doAction("Action #1")) ;
      contextMenu.AddActionItem("Action #2",()=>doAction("Action #2")) ;
      var nested_01 = contextMenu.AddNestedMenu("Nested 1") ;
      nested_01.AddActionItem("Action #3",()=>doAction("Action #3")) ;
      nested_01.AddSeparator() ;
      nested_01.AddActionItem("Action #4",()=>doAction("Action #4"),isEnabled:false) ;
      nested_01.AddBooleanItem("Boolean #5",(isChecked)=>doAction($"Boolean #5 {isChecked}"),false) ;
      nested_01.AddBooleanItem("Boolean #6",(isChecked)=>doAction($"Boolean #6 {isChecked}"),true) ;
      var nested_02 = nested_01.AddNestedMenu("Nested 2") ;
      nested_02.AddActionItem("Action #5",()=>doAction("Action #5")) ;
      nested_02.AddActionItem("Action #6",()=>doAction("Action #6")) ;
      return contextMenu ;
    }

    public static void Scan_VisitingMenuItems ( 
      MenuDescriptor            menuDescriptor,
      System.Action<int,string> onItemVisited,
      int                       level = 0
    ) {
      foreach ( MenuItemDescriptor item in menuDescriptor.ChildMenuItemDescriptors )
      {
        switch ( item )
        {
        case ActionMenuItemDescriptor actionItem:
          onItemVisited(level,actionItem.ItemText) ;
          break ;
        case MenuDescriptor nestedMenu:
          onItemVisited(level,nestedMenu.Title) ;
          Scan_VisitingMenuItems(
            nestedMenu,
            onItemVisited,
            level+1
          ) ;
          break ;
        case SeparatorMenuItemDescriptor separatorItem:
          onItemVisited(level,"separator") ;
          break ;
        }
      }
    }

  }

}
