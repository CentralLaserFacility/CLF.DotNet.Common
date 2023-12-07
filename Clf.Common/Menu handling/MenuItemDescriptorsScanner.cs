//
// MenuItemDescriptorsScanner.cs
//

using System.Collections.Generic;

namespace Clf.Common.MenuHandling
{

  public static class MenuItemDescriptorsScanner 
  {

    public static void VisitAllItems ( 
      IEnumerable<MenuItemDescriptor>   menuItemDescriptors, 
      System.Action<MenuItemDescriptor> visit,
      bool                              recurseIntoNestedMenus = true
    ) {
      foreach ( var menuItemDescriptor in menuItemDescriptors )
      {
        visit(menuItemDescriptor) ;
        if ( menuItemDescriptor is MenuDescriptor nestedMenuDescriptor ) 
        {
          VisitAllItems(
            nestedMenuDescriptor.ChildMenuItemDescriptors,
            visit,
            recurseIntoNestedMenus          
          ) ;
        }
      }
    }

    public static void VisitAllNestedItems ( 
      MenuDescriptor                    menuDescriptor, 
      System.Action<MenuItemDescriptor> visit,
      bool                              recurseIntoNestedMenus = true
    ) {
      foreach ( var menuItemDescriptor in menuDescriptor.ChildMenuItemDescriptors )
      {
        visit(menuItemDescriptor) ;
        if ( menuItemDescriptor is MenuDescriptor nestedMenuDescriptor ) 
        {
          VisitAllNestedItems(
            nestedMenuDescriptor,
            visit,
            recurseIntoNestedMenus          
          ) ;
        }
      }
    }

  }

}
