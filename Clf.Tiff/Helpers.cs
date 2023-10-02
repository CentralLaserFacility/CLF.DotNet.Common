//
// Helpers.cs
//

//
// This code uses the open source 'LibTiff.Net' library
//   https://bitmiracle.com/libtiff/
// via a Nuget package (see the .csproj file)
//
// As required by the 'LibTiff.Net' license at
// https://bitmiracle.github.io/libtiff.net/help/articles/license.html
// we include the relevant copyright notice :
// Copyright (c) 2008-2019, Bit Miracle
//  

//
// TIFF, Tag Image File Format, FAQ
// https://www.awaresystems.be/imaging/tiff/faq.html
//
// TIFF Tag Reference, Baseline TIFF Tags
// https://www.awaresystems.be/imaging/tiff/tifftags/baseline.html
//

//
// c# - Writing RGB values of the TIFF tiff with LibTiff - Stack Overflow
// https://stackoverflow.com/questions/70056644/writing-rgb-values-of-the-tiff-tiff-with-libtiff
//
// LibTiff docs
// https://bitmiracle.github.io/libtiff.net/help/articles/KB/grayscale-color.html#writing-a-color-tiff
//

using BitMiracle.LibTiff.Classic ;
using System.IO ;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Clf.Tiff.Tests")]

namespace Clf.Tiff
{

  public static partial class Helpers
  {

    internal static int GetRequiredTagValue ( 
      this BitMiracle.LibTiff.Classic.Tiff image,
      TiffTag                              tag
    ) => (
      image.GetField(tag)[0].ToInt()
    ) ;

    internal static bool CanGetOptionalTagValue ( 
      this BitMiracle.LibTiff.Classic.Tiff image,
      TiffTag   tag,
      out int   tagValue
    ) {
      FieldValue[]? fieldValues = image.GetField(tag) ;
      tagValue = (
        fieldValues is null
        ? -1 
        : fieldValues[0].ToInt()
      ) ;
      return tagValue != -1 ;
    }

    internal static T? GetOptionalTagValue<T> ( 
      this BitMiracle.LibTiff.Classic.Tiff image,
      TiffTag                              tag,
      System.Func<int,T>                   getTagFromInt
    ) where T : struct
    {
      FieldValue[]? fieldValues = image.GetField(tag) ;
      if ( fieldValues is null ) 
      {
        return null ;
      }
      else
      {
        return getTagFromInt(
          fieldValues[0].ToInt() 
        ) ;
      }
    }

    // -----------------------

    private static BitMiracle.LibTiff.Classic.Tiff CreateTiffWritingToStream ( MemoryStream stream  )
    {
      return BitMiracle.LibTiff.Classic.Tiff.ClientOpen(
        "output",
        "w",
        stream,
        new TiffStream()
      ) ;
    }

    private static void WriteStreamToFile ( Stream streamToBeWritten, string filePath )
    {
      streamToBeWritten.Position = 0 ;
      using ( 
        var fileStream = new FileStream(
          filePath,
          FileMode.Create,
          FileAccess.Write
        ) 
      ) {
        streamToBeWritten.CopyTo(fileStream) ;
      }
    }

    // --------------------

    private static BitMiracle.LibTiff.Classic.Tiff OpenTiffFromStream ( Stream stream )
    {
      return BitMiracle.LibTiff.Classic.Tiff.ClientOpen(
        "input",
        "r",
        stream,
        new TiffStream()
      ) ?? throw new System.ApplicationException("Failed to open TIFF") ;
    }

    private static MemoryStream OpenMemoryStreamFromFile ( string filePath )
    {
      byte[] byteArray = System.IO.File.ReadAllBytes(filePath) ;
      return new MemoryStream(byteArray) ;
    }

    private static MemoryStream OpenMemoryStreamFromByteArray ( byte[] byteArray )
    {
      return new MemoryStream(byteArray) ;
    }

  }

}
