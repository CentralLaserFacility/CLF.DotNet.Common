//
// InvokesActionOnDispose.cs
//

namespace Clf.Common.Utils
{

  public abstract class InvokesActionOnDispose : System.IDisposable
  {

    protected System.Action? DisposeAction ;

    protected InvokesActionOnDispose ( )
    {
    }

    public void Dispose ( )
    {
      DisposeAction?.Invoke() ;
    }

  }

}
