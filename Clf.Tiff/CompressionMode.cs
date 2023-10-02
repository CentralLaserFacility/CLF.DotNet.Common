//
// CompressionMode.cs
//

namespace Clf.Tiff
{

  //
  // LZW and ZIP compression modes are 'lossless'
  // ie there is no degradation in image integrity.
  //
  // Compression reduces the file size by a significant factor,
  // at the expense of slower read and write times.
  //

  public enum CompressionMode {
    NoCompression,
    LZW, // Lossless
    ZIP, // Lossless
    JPEG // Lossy - available for colour images only
  }

  public static partial class Helpers
  {

    //
    // Specifying a 'CompressionMode' of 'null' gets you this 'Default' setting
    //

    public static CompressionMode CompressionMode_Default = CompressionMode.NoCompression ;

    internal static BitMiracle.LibTiff.Classic.Compression AsTag ( 
      this CompressionMode? compressionMode 
    ) 
    => (
      compressionMode switch {
        CompressionMode.NoCompression => BitMiracle.LibTiff.Classic.Compression.NONE,
        CompressionMode.LZW           => BitMiracle.LibTiff.Classic.Compression.LZW,
        CompressionMode.ZIP           => BitMiracle.LibTiff.Classic.Compression.DEFLATE,
        CompressionMode.JPEG          => BitMiracle.LibTiff.Classic.Compression.JPEG,
        null                          => AsTag(CompressionMode_Default),
        _                             => throw new System.Diagnostics.UnreachableException()
      } 
    ) ;

  }

}
