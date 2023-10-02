//
// Helpers_32_bit_colour.cs
//

using BitMiracle.LibTiff.Classic ;

namespace Clf.Tiff
{

  partial class Helpers
  {

    public static void ReadColourImageFromTiffFile ( 
      string     filePath, 
      out int    width, 
      out int    height,
      out uint[] pixelData
    ) {
      ReadColourImageFromTiffByteArray(
        System.IO.File.ReadAllBytes(filePath),
        out width, 
        out height,
        out pixelData
      ) ;
    }


    public static void ReadColourImageFromTiffFile_UsingStream ( 
      string     filePath, 
      out int    width, 
      out int    height,
      out uint[] pixelData
    ) {
      using ( var stream = OpenMemoryStreamFromFile(filePath) )
      {
        using ( var tiff = OpenTiffFromStream(stream) ) 
        {
          ReadColourImageFromTiff(
            tiff,
            out width, 
            out height,
            out pixelData
          ) ;
        }
      }
    }

    public static void ReadColourImageFromTiffByteArray ( 
      byte[]     tiffByteArray, 
      out int    width, 
      out int    height,
      out uint[] pixelData
    ) {
      using ( var stream = OpenMemoryStreamFromByteArray(tiffByteArray) )
      {
        using ( var tiff = OpenTiffFromStream(stream) ) 
        {
          ReadColourImageFromTiff(
            tiff,
            out width, 
            out height,
            out pixelData
          ) ;
        }
      }
    }

    //
    // We assume the TIFF file contains 24-bit RGB pixel data,
    // and write '0xFF' into the top byte of each returned value,
    // representing an alpha (opacity) value of 1.
    //

    private static void ReadColourImageFromTiff ( 
      BitMiracle.LibTiff.Classic.Tiff tiffToRead, 
      out int                         width, 
      out int                         height,
      out uint[]                      colouredPixelsArray
    ) {
      int bitsPerSample   = Clf.Tiff.Helpers.GetRequiredTagValue(tiffToRead,TiffTag.BITSPERSAMPLE) ;
      int samplesPerPixel = Clf.Tiff.Helpers.GetRequiredTagValue(tiffToRead,TiffTag.SAMPLESPERPIXEL) ;
      int photometric     = Clf.Tiff.Helpers.GetRequiredTagValue(tiffToRead,TiffTag.PHOTOMETRIC) ;
      if ( bitsPerSample != 8 )
      {
        throw new System.ApplicationException(
          $"TIFF has {bitsPerSample} bit pixels, was expecting 8-bit pixels"
        ) ;
      }
      if ( samplesPerPixel != 3 )
      {
        throw new System.ApplicationException(
          $"TIFF has {samplesPerPixel} samples per pixel, was expecting 3"
        ) ;
      }
      if ( photometric != (int)Photometric.RGB )
      {
        throw new System.ApplicationException(
          $"TIFF has photometric mode of {(Photometric)photometric}, was expecting RGB"
        ) ;
      }
      width  = Clf.Tiff.Helpers.GetRequiredTagValue(tiffToRead,TiffTag.IMAGEWIDTH);
      height = Clf.Tiff.Helpers.GetRequiredTagValue(tiffToRead,TiffTag.IMAGELENGTH);
      int nColouredImagePixels = height * width ;
      colouredPixelsArray = new uint[nColouredImagePixels] ;
      int nBytesPerLine = width * 3 ;
      byte[] bufferHoldingOneLineOfPixelBytes = new byte[nBytesPerLine] ;
      int iColouredImagePixel = 0 ;
      for ( int iLine = 0 ; iLine < height ; iLine++ )
      {
        // The 'ReadScanline' function takes a byte array,
        // and reads the entire content. So unfortunately
        // we have to take a copy of the pixel data.
        bool ok = tiffToRead.ReadScanline(
          buffer : bufferHoldingOneLineOfPixelBytes,
          row    : iLine
        ) ;
        if ( ok is false )
        {
          throw new System.ApplicationException(
            $"Failed to read scan line #{iLine} for TIFF"
          ) ;
        }
        int iPixelByte = 0 ;
        for ( int iColouredPixelOnLine = 0 ; iColouredPixelOnLine < width ; iColouredPixelOnLine++ ) 
        {
          byte red   = bufferHoldingOneLineOfPixelBytes[iPixelByte++] ;
          byte green = bufferHoldingOneLineOfPixelBytes[iPixelByte++]  ;
          byte blue  = bufferHoldingOneLineOfPixelBytes[iPixelByte++] ;
          colouredPixelsArray[iColouredImagePixel++] = Clf.Common.ImageProcessing.PixelEncodingHelpers.EncodeARGB(
            red,
            green, 
            blue
          ) ;
        }
      }
    }

  }

}
