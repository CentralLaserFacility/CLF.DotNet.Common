//
// ClickableMenuItemDescriptor.cs
//

namespace Clf.Common.MenuHandling
{

  public abstract record ClickableMenuItemDescriptor (
    MenuDescriptor Parent,
    string         ItemText,
    string?        TooltipText,
    bool           IsEnabled
  ) : 
  MenuItemDescriptor(Parent)
  {
    // public override bool IsAvailable => IsEnabled ;
    public abstract void HandleClick ( bool? isChecked ) ;
  }

}
