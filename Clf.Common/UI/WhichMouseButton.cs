//
// WhichMouseButton.cs
//

namespace Clf.Common.UI
{


  public enum MouseEventType {
    LeftButtonDown,
    RightButtonDown,
    MouseMoved
  } 

  //
  // Hmm, tempting to include additional properties here,
  // such as the current position and the identity of the
  // object at that point. But ...
  //  - if we specify the position, what coordinate system ?
  //  - how to represent the type of the object at the pointer position ?
  //  - was there a 'CoincidenceStatusChange' ?
  // Let's keep it simple, but retain the 'descriptor'
  // so that additional properties *could* be added easily.
  //

  public record MouseEventDescriptor ( 
    MouseEventType MouseEventType 
  ) ;

}
