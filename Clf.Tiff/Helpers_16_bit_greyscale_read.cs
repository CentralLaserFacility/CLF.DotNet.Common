//
// Helpers_16_bit_greyscale_read.cs
//

using BitMiracle.LibTiff.Classic ;

namespace Clf.Tiff
{

  partial class Helpers
  {

    public static void ReadGreyscaleImageFromTiffFile ( 
      string       filePath, 
      out int      width, 
      out int      height,
      out ushort[] pixelData
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
      out ushort[] pixelData
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
      byte[]       tiffByteArray, 
      out int      width, 
      out int      height,
      out ushort[] pixelData
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

    public static void ReadGreyscaleImageFromTiff ( 
      BitMiracle.LibTiff.Classic.Tiff tiffToRead, 
      out int                         width, 
      out int                         height,
      out ushort[]                    sixteenBitPixelDataToRead
    ) {
      int bitsPerSample = Clf.Tiff.Helpers.GetRequiredTagValue(tiffToRead,TiffTag.BITSPERSAMPLE) ;
      if ( bitsPerSample != 16 )
      {
        throw new System.ApplicationException(
          $"TIFF data has {bitsPerSample} bit pixels, was expecting 16-bit pixels"
        ) ;
      }
      width  = Clf.Tiff.Helpers.GetRequiredTagValue(tiffToRead,TiffTag.IMAGEWIDTH) ;
      height = Clf.Tiff.Helpers.GetRequiredTagValue(tiffToRead,TiffTag.IMAGELENGTH) ;
      int imageSize = height * width ;
      sixteenBitPixelDataToRead = new ushort[imageSize] ;
      int nBytesPerLine = width * 2 ;
      byte[] bufferHoldingOneLineOfPixels = new byte[nBytesPerLine] ;
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
        System.Buffer.BlockCopy(
          src       : bufferHoldingOneLineOfPixels,
          srcOffset : 0,
          dst       : sixteenBitPixelDataToRead,
          dstOffset : iLine * nBytesPerLine, // Offset in *bytes* !!!
          count     : bufferHoldingOneLineOfPixels.Length
        ) ;
      }
    }

  }

}
