//
// Win32.cs
//

using System.Runtime.InteropServices ;

namespace Clf.Common
{
  
  public static class Win32
  {

    [System.Runtime.InteropServices.DllImport("Kernel32")]
    // https://docs.microsoft.com/en-gb/windows/win32/api/winbase/nf-winbase-setdlldirectorya
    public static extern bool SetDllDirectory ( [In] [Optional] [MarshalAs(UnmanagedType.LPStr)] string? pathToSearch ) ;

    [System.Runtime.InteropServices.DllImport("Kernel32")]
    // https://docs.microsoft.com/en-us/windows/win32/api/libloaderapi/nf-libloaderapi-loadlibrarya
    public static extern System.IntPtr LoadLibrary ( [In] [MarshalAs(UnmanagedType.LPStr)] string fileNameOrFullPathToFile ) ;

    [System.Runtime.InteropServices.DllImport("Kernel32")]
    // https://docs.microsoft.com/en-us/windows/win32/api/errhandlingapi/nf-errhandlingapi-getlasterror
    public static extern System.UInt32 GetLastError ( ) ;

    // https://www.pinvoke.net/default.aspx/kernel32.formatmessage

    [DllImport("kernel32.dll")]
    private static extern int FormatMessage ( 
      FORMAT_MESSAGE                dwFlags, 
      System.IntPtr                 lpSource, 
      uint                          dwMessageId, 
      uint                          dwLanguageId, 
      out System.Text.StringBuilder msgOut, // Bizarre but necessary ...
      int                           nSize, 
      System.IntPtr                 arguments
    ) ;

    private enum FORMAT_MESSAGE : uint
    {
      ALLOCATE_BUFFER = 0x00000100,
      IGNORE_INSERTS  = 0x00000200,
      FROM_SYSTEM     = 0x00001000,
      ARGUMENT_ARRAY  = 0x00002000,
      FROM_HMODULE    = 0x00000800,
      FROM_STRING     = 0x00000400
    }

    public static string GetLastErrorAsString ( uint lastError )
    {
      if ( 0 == lastError )
      {
        return "" ;
      }
      else
      {
        int size_ignored = FormatMessage(
          FORMAT_MESSAGE.ALLOCATE_BUFFER | FORMAT_MESSAGE.FROM_SYSTEM | FORMAT_MESSAGE.IGNORE_INSERTS,
          System.IntPtr.Zero, 
          lastError, 
          0, 
          out var msgOut, 
          256, 
          System.IntPtr.Zero
        ) ;
        return msgOut.ToString().Trim() ;
      }
    }

  }

}
