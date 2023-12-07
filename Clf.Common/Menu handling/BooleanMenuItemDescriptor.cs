//
// BooleanMenuItemDescriptor.cs
//

namespace Clf.Common.MenuHandling
{

  public record BooleanMenuItemDescriptor (
    MenuDescriptor      Parent,
    string              ItemText,
    bool                InitialValue,   
    System.Action<bool> OnClicked,
    string?             TooltipText = null,
    bool                IsEnabled   = true
  ) : 
  ClickableMenuItemDescriptor(Parent,ItemText,TooltipText,IsEnabled)
  {
    public override void HandleClick ( bool? isChecked )
    {
      OnClicked(
        isChecked ?? false
      ) ;
    }
  }

}
