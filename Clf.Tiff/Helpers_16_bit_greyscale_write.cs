//
// Helpers_16_bit_greyscale_write.cs
//

using BitMiracle.LibTiff.Classic ;
using System.IO ;

namespace Clf.Tiff
{

  partial class Helpers
  {

    public static void WriteGreyscaleImageAsTiffFile ( 
      int              width, 
      int              height,
      ushort[]         pixelDataToWrite,
      string           filePath,
      CompressionMode? compressionMode = null
    ) {
      using ( var stream = new MemoryStream() )
      {
        using ( var tiff = CreateTiffWritingToStream(stream) )
        {
          WriteGreyscaleImageAsTiff(
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

    public static void WriteGreyscaleImageAsTiffFile_UsingStream ( 
      int              width, 
      int              height,
      ushort[]         pixelDataToWrite,
      string           filePath,
      CompressionMode? compressionMode = null
    ) {
      using ( var stream = new MemoryStream() )
      {
        using ( var tiff = CreateTiffWritingToStream(stream) )
        {
          WriteGreyscaleImageAsTiff(
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

    public static void WriteGreyscaleImageAsTiffByteArray ( 
      int              width, 
      int              height,
      ushort[]         pixelDataToWrite,
      out byte[]       tiffData,
      CompressionMode? compressionMode = null
    ) {
      using ( var stream = new MemoryStream() )
      {
        using ( var tiff = CreateTiffWritingToStream(stream) )
        {
          WriteGreyscaleImageAsTiff(
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

    public static void WriteGreyscaleImageAsTiff ( 
      int                             width, 
      int                             height,
      ushort[]                        sixteenBitPixelDataToWrite,
      BitMiracle.LibTiff.Classic.Tiff tiffToWrite, 
      CompressionMode?                compressionMode = null
    ) {
      if ( compressionMode == Clf.Tiff.CompressionMode.JPEG )
      {
        throw new System.ApplicationException(
          "JPEG compression is not supported for greyscale images"
        ) ;
      }

      tiffToWrite.SetField(TiffTag.IMAGEWIDTH        ,                  width  ) ;
      tiffToWrite.SetField(TiffTag.IMAGELENGTH       ,                 height  ) ;
      tiffToWrite.SetField(TiffTag.SAMPLESPERPIXEL   ,                      1  ) ;
      tiffToWrite.SetField(TiffTag.BITSPERSAMPLE     ,                     16  ) ;
      tiffToWrite.SetField(TiffTag.ORIENTATION       ,     Orientation.TOPLEFT ) ;
      tiffToWrite.SetField(TiffTag.ROWSPERSTRIP      ,                 height  ) ;
      tiffToWrite.SetField(TiffTag.PLANARCONFIG      ,    PlanarConfig.CONTIG  ) ; // Single image plane
      tiffToWrite.SetField(TiffTag.PHOTOMETRIC       , Photometric.MINISBLACK  ) ; // Min value is black
      tiffToWrite.SetField(TiffTag.FILLORDER         ,      FillOrder.MSB2LSB  ) ; // Most significant first
      tiffToWrite.SetField(TiffTag.COMPRESSION       , compressionMode.AsTag() ) ; 

      // These aren't required ...

      // tiffToWrite.SetField( TiffTag.XRESOLUTION    ,                   88.0 ) ;
      // tiffToWrite.SetField( TiffTag.YRESOLUTION    ,                   88.0 ) ;
      // tiffToWrite.SetField( TiffTag.RESOLUTIONUNIT ,     ResUnit.CENTIMETER ) ;

      int nBytesPerLine = width * 2 ;
      byte[] bufferHoldingOneLineOfPixels = new byte[nBytesPerLine] ;

      for ( int iLine = 0 ; iLine < height ; iLine++ )
      {

        // This 'Buffer.BlockCopy' blindly copies the bytes from 
        // the source array into the destination array, one line at a time,
        // without regard for the data types. This is nicely efficient but
        // it doesn't give us explicit control over the byte order.
        // However the byte order *is* correct.

        System.Buffer.BlockCopy(
          src       : sixteenBitPixelDataToWrite,
          srcOffset : iLine * nBytesPerLine, // Offset in *bytes* !!!
          dst       : bufferHoldingOneLineOfPixels,
          dstOffset : 0,
          count     : bufferHoldingOneLineOfPixels.Length
        ) ;

        // CopyOneLineOfPixels(
        //   sourceArray      : sixteenBitPixelDataToWrite,
        //   sourceLineNumber : iLine,
        //   sourceLineLength : width,
        //   destinationArray : bufferHoldingOneLineOfPixels
        // ) ;

        bool writeSucceeded = tiffToWrite.WriteScanline(
          buffer : bufferHoldingOneLineOfPixels,
          row    : iLine
        ) ;
        if ( writeSucceeded is false )
        {
          throw new System.ApplicationException(
            $"Failed to write scan line #{iLine} for TIFF"
          ) ;
        }

      }

      // Must explicitly 'flush' otherwise
      // no data gets written to the stream !!

      tiffToWrite.Flush() ;

      // This helper gives us explicit control over the byte ordering,
      // but even though it uses Span<> it takes longer than 'BlockCopy'.
      // void CopyOneLineOfPixels ( 
      //   ushort[] sourceArray, 
      //   int      sourceLineNumber,
      //   int      sourceLineLength, 
      //   byte[]   destinationArray 
      // ) {
      //   int offsetIntoSourceArray = sourceLineNumber * sourceLineLength ;
      //   ReadOnlySpan<ushort> sourceSpan = sourceArray.AsSpan(
      //     offsetIntoSourceArray,
      //     sourceLineLength
      //   ) ;
      //   var destinationSpan = destinationArray.AsSpan() ;
      //   int iDestinationPixel = 0 ;
      //   for ( int iSourcePixel = 0 ; iSourcePixel < sourceLineLength ; iSourcePixel++ )
      //   {
      //     ushort pixel = sourceSpan[iSourcePixel] ;
      //     destinationSpan[iDestinationPixel++] = (byte) ( pixel      ) ;
      //     destinationSpan[iDestinationPixel++] = (byte) ( pixel >> 8 ) ;
      //   }
      // }

    }

  }

}
