//
// ContextMenuDescriptor.cs
//

namespace Clf.Common.MenuHandling
{

  public record ContextMenuDescriptor ( ) 
  : MenuDescriptor(
    Title  : "ContextMenu",
    Parent : null
  ) {

    // This action gets invoked when any ActionMenuItem is clicked.
    // It provides an opportunity to cease showing the ContextMenu,
    // eg in a Blazor UI we'd set the visibility of the element that's
    // rendering the COntextMenu, to 'hidden'. In a WPF app that's not necessary
    // as the menu visual gets dismissed as soon as an item is clicked on.

    // TODO : Make this a one-time action,
    // that automatically gets set to null once it has fired ???

    private System.Action<ActionMenuItemDescriptor>? m_actionItemClickHandler = null ;
    public System.Action<ActionMenuItemDescriptor>? ActionItemClickHandler 
    {
      get => m_actionItemClickHandler ;
      set
      {
        m_actionItemClickHandler = value ;
      }
    }

    private System.Action<ClickableMenuItemDescriptor>? m_clickableItemClickHandler = null ;
    public System.Action<ClickableMenuItemDescriptor>? ClickableItemClickHandler 
    {
      get => m_clickableItemClickHandler ;
      set
      {
        m_clickableItemClickHandler = value ;
      }
    }

  }

}
