//
// Helpers_32_bit_colour.cs
//

using BitMiracle.LibTiff.Classic ;

namespace Clf.Tiff
{

  partial class Helpers
  {

    public static void WriteColourImageAsTiffFile ( 
      int              width, 
      int              height,
      uint[]           pixelDataToWrite,
      string           filePath,
      CompressionMode? compressionMode = null
    ) {
      WriteColourImageAsTiffByteArray(
        width, 
        height,
        pixelDataToWrite,
        out byte[] tiffData,
        compressionMode
      ) ;
      System.IO.File.WriteAllBytes(filePath,tiffData) ;
    }

    public static void WriteColourImageAsTiffFile_UsingStream ( 
      int              width, 
      int              height,
      uint[]           pixelDataToWrite,
      string           filePath,
      CompressionMode? compressionMode = null
    ) {
      using ( var stream = new System.IO.MemoryStream() )
      {
        using ( var tiff = CreateTiffWritingToStream(stream) )
        {
          WriteColourImageAsTiff(
            width, 
            height,
            pixelDataToWrite,
            tiff,
            compressionMode 
          ) ;
          WriteStreamToFile(
            stream,
            filePath
          ) ;
        }
      }      
    }

    public static void WriteColourImageAsTiffByteArray ( 
      int              width, 
      int              height,
      uint[]           pixelDataToWrite,
      out byte[]       tiffData,
      CompressionMode? compressionMode = null
    ) {
      using ( var stream = new System.IO.MemoryStream() )
      {
        using ( var tiff = CreateTiffWritingToStream(stream) )
        {
          WriteColourImageAsTiff(
            width, 
            height,
            pixelDataToWrite,
            tiff,
            compressionMode 
          ) ;
          tiffData = stream.ToArray() ;
        }
      }      
    }

    public static void WriteColourImageAsTiff ( 
      int                             width, 
      int                             height,
      uint[]                          colourPixelDataToWrite,
      BitMiracle.LibTiff.Classic.Tiff tiffToWrite, 
      Clf.Tiff.CompressionMode?       compressionMode = null
    ) {
      // We have to get the 32-bit pixel data into
      // the form of a byte array with 3 bytes per pixel
      int nColourPixels = width * height ;
      byte[] pixelBytesRGB = new byte[nColourPixels*3] ;
      int iPixelByteToWrite = 0 ;
      for ( int iColourPixelToRead = 0 ; iColourPixelToRead < nColourPixels ; iColourPixelToRead++ ) 
      {
        Clf.Common.ImageProcessing.PixelEncodingHelpers.DecodeARGB(
          colourPixelDataToWrite[iColourPixelToRead],
          out byte red,
          out byte green, 
          out byte blue
        ) ;
        pixelBytesRGB[iPixelByteToWrite++] = red ;
        pixelBytesRGB[iPixelByteToWrite++] = green ;
        pixelBytesRGB[iPixelByteToWrite++] = blue ;
      }
      WriteColourImageAsFile_ThreeBytesPerPixel(
        width,
        height,
        pixelBytesRGB,
        tiffToWrite,       
        compressionMode
      ) ;
    }

    private static void WriteColourImageAsFile_ThreeBytesPerPixel ( 
      int                             width, 
      int                             height,
      byte[]                          pixelDataToWrite_threeBytesPerPixel_RGB,
      BitMiracle.LibTiff.Classic.Tiff tiffToWrite, 
      Clf.Tiff.CompressionMode?       compressionMode = null 
    ) {

      tiffToWrite.SetField( TiffTag.IMAGEWIDTH      ,                   width ) ;
      tiffToWrite.SetField( TiffTag.IMAGELENGTH     ,                  height ) ;
      tiffToWrite.SetField( TiffTag.SAMPLESPERPIXEL ,                       3 ) ;
      tiffToWrite.SetField( TiffTag.BITSPERSAMPLE   ,                       8 ) ;
      tiffToWrite.SetField( TiffTag.ROWSPERSTRIP    ,                  height ) ;
      tiffToWrite.SetField( TiffTag.PLANARCONFIG    ,     PlanarConfig.CONTIG ) ;
      tiffToWrite.SetField( TiffTag.PHOTOMETRIC     ,         Photometric.RGB ) ;
      tiffToWrite.SetField( TiffTag.COMPRESSION     , compressionMode.AsTag() ) ;
      tiffToWrite.SetField( TiffTag.FILLORDER       ,       FillOrder.MSB2LSB ) ;

      // tiffToWrite.SetField( TiffTag.XRESOLUTION     ,                   88.0 ) ;
      // tiffToWrite.SetField( TiffTag.YRESOLUTION     ,                   88.0 ) ;
      // tiffToWrite.SetField( TiffTag.RESOLUTIONUNIT  ,     ResUnit.CENTIMETER ) ;

      tiffToWrite.SetField( TiffTag.ORIENTATION , Orientation.TOPLEFT ) ;

      byte[] bufferHoldingOneLineOfPixels = new byte[width * 3] ;
      for ( int iScanLine = 0 ; iScanLine < height ; iScanLine++ )
      {
        // The 'WriteScanline' function takes a byte array,
        // and writes the entire content. So unfortunately
        // we have to take a copy of the pixel data.
        System.Array.Copy(
          sourceArray      : pixelDataToWrite_threeBytesPerPixel_RGB,
          sourceIndex      : iScanLine * width * 3,
          destinationArray : bufferHoldingOneLineOfPixels,
          destinationIndex : 0,
          length           : width * 3
        ) ;
        bool ok = tiffToWrite.WriteScanline(
          buffer : bufferHoldingOneLineOfPixels,
          row    : iScanLine
        ) ;
        if ( ok is false )
        {
          throw new System.ApplicationException(
            $"Failed to write TIFF scan line #{iScanLine}"
          ) ;
        }
      }

      tiffToWrite.Flush() ;

    }

  }

}
