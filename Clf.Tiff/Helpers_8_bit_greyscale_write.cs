//
// Helpers_8_bit_greyscale_write.cs
//

using BitMiracle.LibTiff.Classic ;

namespace Clf.Tiff
{

  partial class Helpers
  {

    public static void WriteGreyscaleImageAsTiffFile ( 
      int              width, 
      int              height,
      byte[]           pixelDataToWrite,
      string           filePath,
      CompressionMode? compressionMode = null
    ) {
      WriteGreyscaleImageAsTiffByteArray(
        width, 
        height,
        pixelDataToWrite,
        out byte[] tiffData,
        compressionMode
      ) ;
      System.IO.File.WriteAllBytes(filePath,tiffData) ;
    }

    public static void WriteGreyscaleImageAsTiffFile_UsingStream ( 
      int              width, 
      int              height,
      byte[]           pixelDataToWrite,
      string           filePath,
      CompressionMode? compressionMode = null
    ) {
      using ( var stream = new System.IO.MemoryStream() )
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
      // using ( 
      //   var tiff = TiffFactory.CreateTiffAsStream(
      //    out var memoryStream
      //   )
      // ) {
      //   WriteGreyscaleImageAsTiff(
      //     width, 
      //     height,
      //     pixelDataToWrite,
      //     tiff,
      //     compressionMode 
      //   ) ;
      //   TiffFactory.WriteStreamToFile(
      //     memoryStream,
      //     filePath
      //   ) ;
      //   memoryStream.Dispose() ;
      // }      
    }

    public static void WriteGreyscaleImageAsTiffByteArray ( 
      int              width, 
      int              height,
      byte[]           pixelDataToWrite,
      out byte[]       tiffData,
      CompressionMode? compressionMode = null
    ) {
      using ( var stream = new System.IO.MemoryStream() )
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
      // using ( 
      //   var tiff = TiffFactory.CreateTiffAsStream(
      //    out var memoryStream
      //   )
      // ) {
      //   WriteGreyscaleImageAsTiff(
      //     width, 
      //     height,
      //     pixelDataToWrite,
      //     tiff,
      //     compressionMode 
      //   ) ;
      //   tiffData = memoryStream.ToArray() ;
      //   memoryStream.Dispose() ;
      // }      
    }

    private static void WriteGreyscaleImageAsTiff ( 
      int                             width, 
      int                             height,
      byte[]                          pixelDataToWrite,
      BitMiracle.LibTiff.Classic.Tiff tiffToWrite, 
      CompressionMode?                compressionMode = null
    ) {

      if ( compressionMode == Clf.Tiff.CompressionMode.JPEG )
      {
        throw new System.ApplicationException(
          "JPEG compression is not supported for greyscale images"
        ) ;
      }

      tiffToWrite.SetField(TiffTag.IMAGEWIDTH      , width                   ) ;
      tiffToWrite.SetField(TiffTag.IMAGELENGTH     , height                  ) ;
      tiffToWrite.SetField(TiffTag.SAMPLESPERPIXEL , 1                       ) ;
      tiffToWrite.SetField(TiffTag.BITSPERSAMPLE   , 8                       ) ;
      tiffToWrite.SetField(TiffTag.ROWSPERSTRIP    , height                  ) ;
      tiffToWrite.SetField(TiffTag.PLANARCONFIG    , PlanarConfig.CONTIG     ) ;
      tiffToWrite.SetField(TiffTag.PHOTOMETRIC     , Photometric.MINISBLACK  ) ;
      tiffToWrite.SetField(TiffTag.FILLORDER       , FillOrder.MSB2LSB       ) ;
      tiffToWrite.SetField(TiffTag.COMPRESSION     , compressionMode.AsTag() ) ;
      tiffToWrite.SetField(TiffTag.ORIENTATION     , Orientation.TOPLEFT     ) ;

      // tiffToWrite.SetField( TiffTag.XRESOLUTION     ,                   88.0 ) ;
      // tiffToWrite.SetField( TiffTag.YRESOLUTION     ,                   88.0 ) ;
      // tiffToWrite.SetField( TiffTag.RESOLUTIONUNIT  ,     ResUnit.CENTIMETER ) ;

      byte[] bufferHoldingOneLineOfPixels = new byte[width] ;
      for ( int iLine = 0 ; iLine < height ; iLine++ )
      {
        // The 'WriteScanline' function takes a byte array,
        // and writes the entire content. So unfortunately
        // we have to take a copy of the pixel data.
        System.Array.Copy(
          sourceArray      : pixelDataToWrite,
          sourceIndex      : iLine * width,
          destinationArray : bufferHoldingOneLineOfPixels,
          destinationIndex : 0,
          length           : width
        ) ;
        bool ok = tiffToWrite.WriteScanline(
          buffer : bufferHoldingOneLineOfPixels,
          row    : iLine
        ) ;
      }

      // Must explicitly 'flush' otherwise
      // no data gets written to the stream !!

      tiffToWrite.Flush() ;
    }

  }

}
