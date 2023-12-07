// 
// IProvidesMenuDescriptors.cs
//

using Clf.Common.MenuHandling;

namespace Clf.Common.UI
{

  //
  // IMPROVE_THIS : provides access to a MenuDescriptor that has been created
  // by client code, eg as a top level menu owned by the app.
  // THIS WAS A STOPGAP - DO IT DIFFERENTLY !!!
  //

  public interface IProvidesMenuDescriptors
  {

    // This API returns a MenuDescriptor that client code can add items to.
    // In the UI, this might map to a MenuItem in a top-level menu.

    MenuDescriptor? GetMenuDescriptorIfAvailable ( string name ) => null ;

    public static readonly IProvidesMenuDescriptors Default = new DefaultInstance() ;

    public class DefaultInstance : IProvidesMenuDescriptors
    {
    }

  }

}