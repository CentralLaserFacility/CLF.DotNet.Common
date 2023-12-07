// 
// ILogEntriesCollection.cs
//

namespace Clf.Common.Utils
{
  public interface ILogEntriesCollection
  {
    void AddLogEntry(string messageLine);

    void Clear();
  }
}