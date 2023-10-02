//
// Helpers_tiff_info.cs
//

using BitMiracle.LibTiff.Classic ;

namespace Clf.Tiff
{

   public record TiffFileInfo (
     int                                     width,
     int                                     height,
     int                                     bitsPerSample,
     int                                     samplesPerPixel,
     BitMiracle.LibTiff.Classic.Photometric  photometric,
     BitMiracle.LibTiff.Classic.Compression  compression,
     BitMiracle.LibTiff.Classic.Orientation? orientation
  ) {
    public void WriteToLineHandler ( System.Action<string> handleLine )
    {
      handleLine($"Width             : {width}") ;
      handleLine($"Height            : {height}") ;
      handleLine($"Bits per sample   : {bitsPerSample}") ;
      handleLine($"Samples per pixel : {samplesPerPixel}") ;
      handleLine($"Photometric       : {photometric}") ;
      handleLine($"Compression       : {compression}") ;
      handleLine($"Orientation       : {(orientation.HasValue?orientation.Value:"UNKNOWN")}") ;
    }
  }

  public partial class Helpers
  {

    // Throws an exception on error ...

    public static TiffFileInfo GetTiffInfoFromFile ( string filePath ) 
    {
      using ( var stream = OpenMemoryStreamFromFile(filePath) )
      {
        using ( var tiff = OpenTiffFromStream(stream) ) 
        {
          return GetTiffInfo(tiff) ;
        }
      }
    }

    // Throws an exception on error ...

    public static TiffFileInfo GetTiffInfoFromTiffByteArray ( byte[] tiffByteArray ) 
    {
      using ( var stream = OpenMemoryStreamFromByteArray(tiffByteArray) )
      {
        using ( var tiff = OpenTiffFromStream(stream) ) 
        {
          return GetTiffInfo(tiff) ;
        }
      }
    }

    private static TiffFileInfo GetTiffInfo ( BitMiracle.LibTiff.Classic.Tiff tiff ) 
    {
      return new TiffFileInfo(
        width           : Clf.Tiff.Helpers.GetRequiredTagValue(tiff, TiffTag.IMAGEWIDTH),
        height          : Clf.Tiff.Helpers.GetRequiredTagValue(tiff, TiffTag.IMAGELENGTH),
        bitsPerSample   : Clf.Tiff.Helpers.GetRequiredTagValue(tiff, TiffTag.BITSPERSAMPLE),
        samplesPerPixel : Clf.Tiff.Helpers.GetRequiredTagValue(tiff, TiffTag.SAMPLESPERPIXEL),
        photometric     : (BitMiracle.LibTiff.Classic.Photometric)Clf.Tiff.Helpers.GetRequiredTagValue(tiff, TiffTag.PHOTOMETRIC),
        compression     : (BitMiracle.LibTiff.Classic.Compression)Clf.Tiff.Helpers.GetRequiredTagValue(tiff, TiffTag.COMPRESSION),
        orientation     : GetOptionalTagValue<BitMiracle.LibTiff.Classic.Orientation>(
          tiff,
          TiffTag.ORIENTATION,
          tagValue => (BitMiracle.LibTiff.Classic.Orientation) tagValue
        )
      ) ;
    }

    // public static void GetTiffFileInfo (
    //   string                                      filePath, 
    //   out int                                     width,
    //   out int                                     height,
    //   out int                                     bitsPerSample,
    //   out int                                     samplesPerPixel,
    //   out BitMiracle.LibTiff.Classic.Photometric  photometric,
    //   out BitMiracle.LibTiff.Classic.Compression  compression,
    //   out BitMiracle.LibTiff.Classic.Orientation? orientation
    // ) {
    //   using ( BitMiracle.LibTiff.Classic.Tiff? tiff = BitMiracle.LibTiff.Classic.Tiff.Open(filePath,"r") )
    //   {
    //     if ( tiff is null )
    //     {
    //       throw new System.ApplicationException(
    //         $"Failed to open TIFF file {filePath}"
    //       ) ;
    //     }
    //     width           = Clf.Tiff.Helpers.GetRequiredTagValue(tiff, TiffTag.IMAGEWIDTH);
    //     height          = Clf.Tiff.Helpers.GetRequiredTagValue(tiff, TiffTag.IMAGELENGTH);
    //     bitsPerSample   = Clf.Tiff.Helpers.GetRequiredTagValue(tiff, TiffTag.BITSPERSAMPLE);
    //     samplesPerPixel = Clf.Tiff.Helpers.GetRequiredTagValue(tiff, TiffTag.SAMPLESPERPIXEL);
    //     photometric     = (BitMiracle.LibTiff.Classic.Photometric)Clf.Tiff.Helpers.GetRequiredTagValue(tiff, TiffTag.PHOTOMETRIC);
    //     compression     = (BitMiracle.LibTiff.Classic.Compression)Clf.Tiff.Helpers.GetRequiredTagValue(tiff, TiffTag.COMPRESSION);
    //     // Orientation seems to be not supported in all TIFF files
    //     orientation = GetOptionalTagValue(
    //       tiff,
    //       TiffTag.ORIENTATION,
    //       tagValue => (BitMiracle.LibTiff.Classic.Orientation) tagValue
    //     ) ;
    //   }
    // }

    public static void GetTiffFileInfo (
      string                filePath,
      System.Action<string> handleLine
    ) {
      try
      {
        handleLine(
          $"TiffInfo from {filePath}"
        ) ;
        GetTiffInfoFromFile(filePath).WriteToLineHandler(
          line => handleLine(
            $"  {line}"
          ) 
        ) ;
      }
      catch ( System.Exception x ) 
      {
        handleLine(
          $"Failed to read TiffInfo from {filePath}"
        ) ;
        handleLine(
          $"Exception : {x.Message}"
        ) ;
      }
      // GetTiffFileInfo(
      //   filePath,
      //   out int                                     width,
      //   out int                                     height,
      //   out int                                     bitsPerSample,
      //   out int                                     samplesPerPixel,
      //   out BitMiracle.LibTiff.Classic.Photometric  photometric,
      //   out BitMiracle.LibTiff.Classic.Compression  compression,
      //   out BitMiracle.LibTiff.Classic.Orientation? orientation
      // ) ;
      // handleLine($"File '{filePath}'") ;
      // handleLine($"  Width             : {width}") ;
      // handleLine($"  Height            : {height}") ;
      // handleLine($"  Bits per sample   : {bitsPerSample}") ;
      // handleLine($"  Samples per pixel : {samplesPerPixel}") ;
      // handleLine($"  Photometric       : {photometric}") ;
      // handleLine($"  Compression       : {compression}") ;
      // handleLine($"  Orientation       : {(orientation.HasValue?orientation.Value:"UNKNOWN")}") ;
    }

  }

}
