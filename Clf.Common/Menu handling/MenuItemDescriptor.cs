//
// MenuItemDescriptor.cs
//

namespace Clf.Common.MenuHandling
{

  //
  // This describes a MenuItem, which can be
  //
  //  - an ActionMenuItem
  //  - a Separator
  //  - a nested Menu ...
  //

  public abstract record MenuItemDescriptor ( MenuDescriptor? Parent )
  { 

    // public abstract bool IsAvailable { get ; } // ??? Does this work ???

    public int NestingLevel
    => (
      Parent is null
      ? 0
      : Parent.NestingLevel + 1
    ) ;

    public MenuDescriptor TopLevelParentMenu
    => (
      Parent is null
      ? this as MenuDescriptor
      : Parent.TopLevelParentMenu
    )! ;

  }

}
