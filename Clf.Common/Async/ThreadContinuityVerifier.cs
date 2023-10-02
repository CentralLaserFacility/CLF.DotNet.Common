//
// ThreadContinuityVerifier.cs
//

using FluentAssertions ;

namespace Clf.Common
{

  public sealed class ThreadContinuityVerifier : System.IDisposable
  {

    private readonly int m_originalThreadId = System.Environment.CurrentManagedThreadId ;

    public void Dispose ( )
    {
      System.Environment.CurrentManagedThreadId.Should().Be(m_originalThreadId) ;
    }

  }

}

