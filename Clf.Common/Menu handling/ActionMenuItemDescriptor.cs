//
// MenuItemDescriptor.cs
//

namespace Clf.Common.MenuHandling
{

  public record ActionMenuItemDescriptor (
    MenuDescriptor Parent,
    string         ItemText,
    System.Action  Action,
    string?        TooltipText = null,
    bool           IsEnabled   = true
  ) : 
  ClickableMenuItemDescriptor(Parent,ItemText,TooltipText,IsEnabled)
  {
    public override void HandleClick ( bool? isChecked )
    {
      Action.Invoke() ;
    }
  }

}
