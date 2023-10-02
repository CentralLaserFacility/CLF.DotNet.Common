//
// Tiff_Tests.cs
//

using BitMiracle.LibTiff.Classic;
using FluentAssertions ;
using Xunit ;

using static Clf.Tiff.Helpers ;

namespace Clf_Tiff_Tests
{

  public partial class Tiff_Tests
  {

    //
    // These tests will write a bunch of TIFF files,
    // with path names starting with the following string:
    //
    // EDIT THIS PATH IF IT DOESN'T EXIST ON THE LOCAL MACHINE !!
    //

    private const string TiffImageFileRootPath = @"C:\temp\TiffTestImage" ;

    private readonly Xunit.Abstractions.ITestOutputHelper? m_output ;

    protected void WriteLine ( string message ) => m_output?.WriteLine(message) ;

    public Tiff_Tests ( Xunit.Abstractions.ITestOutputHelper? testOutputHelper )
    {
      m_output = testOutputHelper ;
    }

    private IReadOnlyList<int> GetRandomPixelIndices ( int nPixels, int nIndeces ) 
    {
      List<int> randomIndeces = new List<int>(
        capacity : nIndeces
      ) ;
      for ( int iIndex = 0 ; iIndex < nIndeces ; iIndex++ )
      {
        randomIndeces.Add(
          System.Random.Shared.Next(
            maxValue : nPixels - 1
          ) 
        ) ;
      }
      return randomIndeces ;
    }

    private T[] GetPixelsAtIndeces<T> ( T[] pixelArray, IReadOnlyList<int> indeces )
    {
      return indeces.Select(
        index => pixelArray[index]
      ).ToArray() ;
    }

    [Theory]
    [InlineData(Clf.Tiff.CompressionMode.LZW,Compression.LZW)]
    [InlineData(null,null)]
    public void CompressionMode_WorksAsExpected ( Clf.Tiff.CompressionMode? compressionMode, Compression? compressionTag )
    {
      compressionMode.AsTag().Should().Be(
        compressionTag 
        ?? Clf.Tiff.Helpers.AsTag(
             Clf.Tiff.Helpers.CompressionMode_Default
           )
      ) ;
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)] // Use this file as a source of TIFF data !!
    public void OpeningInvalidTiffFileThrowsException ( string? filePath ) 
    {
      filePath ??= GetSourceCodePath() ;
      FluentActions.Invoking(
        () => Clf.Tiff.Helpers.GetTiffInfoFromFile(filePath)
      ).Should().Throw<System.Exception>() ;
    }

    private static string GetSourceCodePath ( 
      [System.Runtime.CompilerServices.CallerFilePath] string? sourceCodePath = null 
    ) => sourceCodePath! ;

  }

}