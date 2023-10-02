//
// AssociatedTypeAttribute.cs
//

namespace Clf.Common
{

  public class AssociatedTypeAttribute : System.Attribute
  {

    public readonly System.Type? AssociatedType ;

    public AssociatedTypeAttribute ( System.Type? type )
    { 
      AssociatedType = type ;
    }

  }

}
