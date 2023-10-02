//
// Helpers_8_bit_greyscale_read.cs
//

using BitMiracle.LibTiff.Classic ;

namespace Clf.Tiff
{

  partial class Helpers
  {

    public static void ReadGreyscaleImageFromTiffFile ( 
      string     filePath, 
      out int    width, 
      out int    height,
      out byte[] pixelData
    ) {
      ReadGreyscaleImageFromTiffByteArray(
        System.IO.File.ReadAllBytes(filePath),
        out width, 
        out height,
        out pixelData
      ) ;
    }

    public static void ReadGreyscaleImageFromTiffFile_UsingStream ( 
      string     filePath, 
      out int    width, 
      out int    height,
      out byte[] pixelData
    ) {
      using ( var stream = OpenMemoryStreamFromFile(filePath) )
      {
        using ( var tiff = OpenTiffFromStream(stream) ) 
        {
          ReadGreyscaleImageFromTiff(
            tiff,
            out width, 
            out height,
            out pixelData
          ) ;
        }
      }
    }

    public static void ReadGreyscaleImageFromTiffByteArray ( 
      byte[]     tiffByteArray, 
      out int    width, 
      out int    height,
      out byte[] pixelData
    ) {
      using ( var stream = OpenMemoryStreamFromByteArray(tiffByteArray) )
      {
        using ( var tiff = OpenTiffFromStream(stream) ) 
        {
          ReadGreyscaleImageFromTiff(
            tiff,
            out width, 
            out height,
            out pixelData
          ) ;
        }
      }
    }

    private static void ReadGreyscaleImageFromTiff ( 
      BitMiracle.LibTiff.Classic.Tiff tiffToRead, 
      out int                         width, 
      out int                         height,
      out byte[]                      pixelData
    ) {
      int bitsPerSample = GetRequiredTagValue(tiffToRead,TiffTag.BITSPERSAMPLE);
      if ( bitsPerSample != 8 )
      {
        throw new System.ApplicationException(
          $"TIFF stream has {bitsPerSample} bit pixels, was expecting 8-bit pixels"
        ) ;
      }
      width  = GetRequiredTagValue(tiffToRead,TiffTag.IMAGEWIDTH);
      height = GetRequiredTagValue(tiffToRead,TiffTag.IMAGELENGTH);
      int imageSize = height * width ;
      pixelData = new byte[imageSize] ;
      byte[] bufferHoldingOneLineOfPixels = new byte[width] ;
      for ( int iLine = 0 ; iLine < height ; iLine++ )
      {
        // The 'ReadScanline' function takes a byte array,
        // and reads the entire content. So unfortunately
        // we have to take a copy of the pixel data.
        bool ok = tiffToRead.ReadScanline(
          buffer : bufferHoldingOneLineOfPixels,
          row    : iLine
        ) ;
        if ( ok is false )
        {
          throw new System.ApplicationException(
            $"Failed to read scan line #{iLine} for TIFF stream"
          ) ;
        }
        System.Array.Copy(
          sourceArray      : bufferHoldingOneLineOfPixels,
          sourceIndex      : 0,
          destinationArray : pixelData,
          destinationIndex : iLine * width,
          length           : width
        ) ;
      }
    }

  }

}
