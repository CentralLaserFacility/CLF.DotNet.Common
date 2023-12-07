//
// SeparatorMenuItemDescriptor.cs
//

namespace Clf.Common.MenuHandling
{

  public record SeparatorMenuItemDescriptor ( 
    MenuDescriptor Parent 
  ) : 
  MenuItemDescriptor(Parent) 
  {
    // public override bool IsAvailable => true ;
    public string? CategoryInfo { get ; set ; } = null ;
    public void SetCategoryInfo ( string? categoryInfo )
    {
      CategoryInfo = categoryInfo ;
    }
  }

}
