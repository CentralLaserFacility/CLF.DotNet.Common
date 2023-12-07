//
// DisposeAction.cs
//

namespace Clf.Common.Utils
{

  public sealed class DisposeAction : System.IDisposable
  {

    private System.Action m_disposeAction ;

    public DisposeAction ( System.Action disposeAction )
    {
      m_disposeAction = disposeAction ;
    }

    public void Dispose ( )
    {
      m_disposeAction.Invoke() ;
    }

  }

}
