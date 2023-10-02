//
// DescriptionAttribute.cs
//

namespace Clf.Common
{

  [System.AttributeUsage(System.AttributeTargets.All)]
  public sealed class DescriptionAttribute : System.Attribute
  {

    public readonly string Description ;

    public DescriptionAttribute ( string description )
    {
      Description = description ;
    }

  }

}

