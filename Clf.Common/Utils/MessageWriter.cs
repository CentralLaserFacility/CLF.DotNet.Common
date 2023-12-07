//
// MessageWriter.cs
//

namespace Clf.Common.Utils
{

  public sealed class MessageWriter : IMessageWriter
  {

    private readonly System.Action<string> m_writePhysicalMessageLogLine ;

    public void WriteMessageLogLine ( string messageLine = "" )
    {
      m_writePhysicalMessageLogLine(messageLine) ;
    }

    public MessageWriter ( System.Action<string> writeMessageLogLine ) 
    {
      m_writePhysicalMessageLogLine = writeMessageLogLine ;
    }

  }

}
