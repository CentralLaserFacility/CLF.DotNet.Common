//
// ContextMenuDescriptor.cs
//

using Clf.Common.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;

namespace Clf.Common.MenuHandling
{

  //
  // ?? Could lazily evaluate child items, when parent is clicked ??
  //

  //
  // A MenuDescriptor holds an ordered list of MenuItemDescriptor instances,
  // which can be
  //   MenuItemDescriptor
  //     ClickableMenuItemDescriptor
  //       ActionMenuItemDescriptor
  //       BooleanMenuItemDescriptor
  //     MenuDescriptor (nested)
  //     SeparatorMenuItemDescriptor
  //
  // An ActionItem represents an item that can trigger an Action.
  //
  // A Nested Item holds a ContextMenu at the next level of nesting.
  //
  //        ContextMenu
  //     +----------------+
  //     | Action Item    |
  //     +----------------+
  //     | Action Item    |        ContextMenu
  //     +----------------+     +----------------+
  //     | Nested Item    |---->| Action Item    |
  //     +----------------+     +----------------+
  //     | Action Item    |     | Action Item    |
  //     +----------------+     +----------------+
  //                            | Parent Item    |----> etc
  //                            +----------------+ 
  //
  //

  public record MenuDescriptor ( 
    string          Title, 
    MenuDescriptor? Parent // Null if we're the 'root' menu
  ) : 
  MenuItemDescriptor(Parent)
  {

    // public override bool IsAvailable => ChildMenuItemDescriptors.Any() ;

    public bool IsVisible { get ; set ; } = false ;

    public bool IsTopLevelMenu => Parent is null ;

    private List<MenuItemDescriptor> m_menuItemDescriptors = new() ;

    public IEnumerable<MenuItemDescriptor> ChildMenuItemDescriptors => m_menuItemDescriptors ;

    public void ClearAllChildItems ( ) => m_menuItemDescriptors.Clear() ; 

    public void AddActionItem (
      string        itemText,
      System.Action action,
      string?       tooltipText = null,
      bool          isEnabled   = true
    ) {
      m_menuItemDescriptors.Add(
        new ActionMenuItemDescriptor(
          this,
          itemText,
          action,
          tooltipText,
          isEnabled
        )
      ) ;
    }

    public void AddBooleanItem (
      string              itemText,
      System.Action<bool> action,
      bool                initialValue,
      string?             tooltipText = null,
      bool                isEnabled   = true
    ) {
      m_menuItemDescriptors.Add(
        new BooleanMenuItemDescriptor(
          this,
          itemText,
          initialValue,
          action,
          tooltipText,
          isEnabled
        )
      ) ;
    }

    // public void AddTriStateBooleanItem (
    //   string               itemText,
    //   System.Action<bool?> action,
    //   bool?                initialValue,
    //   bool                 isEnabled = true
    // ) {
    //   m_menuItemDescriptors.Add(
    //     new TriStateCheckBoxMenuItemDescriptor(
    //       this,
    //       itemText,
    //       initialValue,
    //       action,
    //       isEnabled
    //     )
    //   ) ;
    // }

    public void AddCancelAction ( )
    {
      AddSeparator() ;
      AddActionItem(
        "Cancel",
        () => { }
      ) ;
    }

    //
    // In future we might be able to use this 'category info'
    // and display it as a 'subtitle' in the separator ??
    //
    // When we Add a separator, the separator will not necessarily
    // be displayed on the final menu. For example if we invoke AddSeparator
    // in the expectation of adding further items, but then don't add any items
    // before calling 'AddSeparator' again, the first 'separator' will be ignored.
    //

    public void AddSeparator ( string? categoryInfo = null )
    {
      if ( 
         m_menuItemDescriptors.IsEmpty()
      && categoryInfo is null
      ) {
        // An empty separator as the first item is pointless,
        // so don't add it ...
        return ;
      }
      var lastItem = m_menuItemDescriptors.LastOrDefault() ;
      if ( lastItem is SeparatorMenuItemDescriptor separatorAsLastItem )
      {
        // The last item is already a separator. 
        // We must have added it in the expectation that further items
        // would be added, but in fact we didn't add any.
        if ( separatorAsLastItem.CategoryInfo is null )
        {
          // Leave that existing separator in place,
          // but update the 'CategoryInfo' ...
          separatorAsLastItem.SetCategoryInfo(categoryInfo) ;
        }
        else
        {
          // The 'category info' was not null,
          // so let's leave that empty separator in place
          AddNewSeparator() ;
        }
      }
      else
      {
        // At least one item has been added since
        // the previous separator, so add a new one ...
        AddNewSeparator() ;
      }
      void AddNewSeparator ( )
      {
        m_menuItemDescriptors.Add(
          new SeparatorMenuItemDescriptor(this){
            CategoryInfo = categoryInfo
          }
        ) ;
      }
    }

    // public void AddProvisionalSeparator ( )
    // {
    //   AddSeparator() ;
    // }
    // 
    // public void ConfirmOrRemoveProvisionalSeparator ( )
    // {
    //   var lastItem = m_menuItemDescriptors.LastOrDefault() ;
    //   if ( lastItem is SeparatorMenuItemDescriptor separatorAsLastItem )
    //   {
    //     m_menuItemDescriptors.Remove(separatorAsLastItem) ;
    //   }
    // } 

    public MenuDescriptor AddNestedMenu ( string title ) 
    {
      var nestedMenu = new MenuDescriptor(
        Title  : title,
        Parent : this
      ) ;
      m_menuItemDescriptors.Add(nestedMenu) ;
      return nestedMenu ;
    }

    // EXPERIMENT !!!

    public MenuDescriptor AddNestedMenu ( MenuDescriptor nestedMenu ) 
    {
      m_menuItemDescriptors.Add(
        nestedMenu with {
          Parent = this
        }
      ) ;
      return nestedMenu ;
    }

    public MenuDescriptor? GetNestedMenuDescriptorIfAvailable ( string name )
    => m_menuItemDescriptors.OfType<MenuDescriptor>(
    ).Where(
      item => item.Title == name 
    ).SingleOrDefault() ;

    public int ItemsCount => m_menuItemDescriptors.Count() ;

    // public void VisitAllChildItems ( System.Action<MenuItemDescriptor> visit )
    // {
    //   foreach ( var menuItem in m_menuItemDescriptors )
    //   {
    //     visit(menuItem) ;
    //   }
    // }

    public void VisitAllChildItems ( 
      System.Action<MenuItemDescriptor> visit//,
      // bool                              recurseIntoNestedMenus = true
    ) {
      foreach ( var menuItemDescriptor in m_menuItemDescriptors )
      {
        visit(menuItemDescriptor) ;
        // if ( menuItemDescriptor is MenuDescriptor nestedMenuDescriptor ) 
        // {
        //   nestedMenuDescriptor.VisitAllChildItems(
        //     visit,
        //     recurseIntoNestedMenus          
        //   ) ;
        // }
      }
    }

    //
    // Builds a hierarchical textual display
    // of the tree of MenuItemDescriptors
    // hosted by a MenuDescriptor.
    //

    public string TextualSummary 
    {
      get
      {
        System.Text.StringBuilder stringBuilder = new() ;
        VisitNestedMenuItemDescriptors(
          this,
          (level,s) => {
            string indents = new string(' ',level*2) ;
            stringBuilder.AppendLine(
              indents + s
            ) ;
          }
        ) ;
        return stringBuilder.ToString() ;
      }
    }

    public static void VisitNestedMenuItemDescriptors ( 
      MenuDescriptor            menuDescriptor,
      System.Action<int,string> onItemVisited,
      int                       level = 0
    ) {
      foreach ( MenuItemDescriptor item in menuDescriptor.ChildMenuItemDescriptors )
      {
        switch ( item )
        {
        case ActionMenuItemDescriptor actionItem:
          onItemVisited(
            level,
            actionItem.ItemText + " (ActionMenuItemDescriptor)"
          ) ;
          break ;
        case MenuDescriptor nestedMenu:
          onItemVisited(
            level,
            nestedMenu.Title + " (MenuDescriptor)"
          ) ;
          VisitNestedMenuItemDescriptors(
            nestedMenu,
            onItemVisited,
            level+1
          ) ;
          break ;
        case SeparatorMenuItemDescriptor separatorItem:
          onItemVisited(
            level,
            "--------- (SeparatorMenuItemDescriptor)"
          ) ;
          break ;
        }
      }
    }

  }

}
