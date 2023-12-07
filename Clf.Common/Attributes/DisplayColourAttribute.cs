//
// DisplayAsAttribute.cs
//

namespace Clf.Common.Attributes
{

  [System.AttributeUsage(System.AttributeTargets.All)]
  public sealed class DisplayColourAttribute : System.Attribute
  {

    public readonly string DisplayColour ; // Name, or hex encoded string

    public DisplayColourAttribute ( string displayColour )
    {
      DisplayColour = displayColour ;
    }



  }

}

