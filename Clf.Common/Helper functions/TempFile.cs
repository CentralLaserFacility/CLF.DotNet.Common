//
// TmpFIle.cs
//

namespace Clf.Common
{

  //
  // This represents a Temporary file that we want to be deleted
  // from the file system once the file has been used.
  //

  // Based on an implementation by Marc Gravell ...
  // https://stackoverflow.com/questions/400140/how-do-i-automatically-delete-temp-files-in-c
  
  public sealed class TempFile : System.IDisposable
  {

    private string m_tempFilePath ;

    public TempFile ( string? path = null )
    {
      m_tempFilePath = path ?? System.IO.Path.GetTempFileName() ;
    }

    public string Path => m_tempFilePath ;
    
    ~TempFile ( ) 
    { 
      Dispose(false) ; 
    }

    public void Dispose ( ) 
    { 
      Dispose(true) ; 
    }

    private void Dispose ( bool disposing )
    {
      if ( disposing )
      {
        System.GC.SuppressFinalize(this) ;                
      }
      if ( m_tempFilePath != null )
      {
        try 
        { 
          System.IO.File.Delete(m_tempFilePath) ; 
        }
        catch 
        { 
          // Best effort ...
        } 
        m_tempFilePath = null! ;
      }
    }

  }

}

