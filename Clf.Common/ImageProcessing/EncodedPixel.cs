//
// EncodedPixel.cs
//

using FluentAssertions ;

// Clf.ImageManipulation.EncodedPixel_Tests.RunTests(
//   handleMessage : (message) => performanceTestResults.Add(message)
// ) ;
// 
// Clf.ImageManipulation.EncodedPixelStruct_Tests.RunTests(
//   handleMessage : (message) => performanceTestResults.Add(message)
// ) ;

namespace Clf.Common.ImageProcessing
{

  public readonly record struct EncodedPixel_ARGB ( uint ARGB )
  {

    public EncodedPixel_ARGB ( byte red, byte green, byte blue ) : 
    this(
      PixelEncodingHelpers.EncodeARGB(red,green,blue)
    ) {
    }

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static EncodedPixel_ARGB FromRgbTuple ( ( byte red, byte green, byte blue ) rgb )
    => new EncodedPixel_ARGB( rgb.red, rgb.green, rgb.blue ) ;

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static EncodedPixel_ARGB FromRgbByteValues ( RgbByteValues rgb )
    => new EncodedPixel_ARGB ( rgb.R, rgb.G, rgb.B ) ;

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public ( byte r, byte g, byte b ) DecodedAsRgbTuple ( ) 
    // => PixelEncodingHelpers.DecodeARGBAsRgbTuple(ARGB) ;
    => (
      r : (byte) ( ARGB >> 16 ),
      g : (byte) ( ARGB >>  8 ),
      b : (byte) ( ARGB >>  0 )
    ) ;

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public RgbByteValues DecodedAsRgbByteValues ( ) 
    => RgbByteValues.FromRgbTuple(
      this.DecodedAsRgbTuple()
    ) ;

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public EncodedPixel_ABGR EncodedAsABGR ( ) => new EncodedPixel_ABGR(
      PixelEncodingHelpers.ConvertToARGB(ARGB) 
    ) ;

  }

  public readonly record struct EncodedPixel_ABGR ( uint ABGR )
  {

    public EncodedPixel_ABGR ( byte red, byte green, byte blue ) : 
    this(
      PixelEncodingHelpers.EncodeABGR(red,green,blue)
    ) {
    }

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static EncodedPixel_ABGR FromRgbTuple ( ( byte red, byte green, byte blue ) rgb )
    => new EncodedPixel_ABGR ( rgb.red, rgb.green, rgb.blue ) ;

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static EncodedPixel_ABGR FromRgbByteValues ( RgbByteValues rgb )
    => new EncodedPixel_ABGR ( rgb.R, rgb.G, rgb.B ) ;

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public ( byte r, byte g, byte b ) DecodedAsRgbTuple ( ) => PixelEncodingHelpers.DecodeABGRAsRgbTuple(ABGR) ;

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public RgbByteValues DecodedAsRgbByteValues ( ) 
    => RgbByteValues.FromRgbTuple(
      this.DecodedAsRgbTuple()
    ) ;

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public EncodedPixel_ARGB EncodedAsARGB ( ) => new EncodedPixel_ARGB(
      PixelEncodingHelpers.ConvertToARGB(ABGR) 
    ) ;


  }

  public static class EncodedPixel_Tests 
  {
    public static unsafe void RunTests ( System.Action<string> handleMessage )
    {

      var rgb = new RgbByteValues(1,2,3) ;

      var argb = EncodedPixel_ARGB.FromRgbByteValues(rgb) ;
      argb.ARGB.Should().Be(0xff010203) ;
      {
        var rgb_tuple = argb.DecodedAsRgbTuple() ;
        rgb_tuple.Should().Be((1,2,3)) ;
        var rgb_byteValues = argb.DecodedAsRgbByteValues() ;
        rgb_byteValues.Should().Be(rgb) ;           
      }

      var abgr = EncodedPixel_ABGR.FromRgbByteValues(rgb) ;
      abgr.ABGR.Should().Be(0xff030201) ;
      {
        var rgb_tuple = abgr.DecodedAsRgbTuple() ;
        rgb_tuple.Should().Be((1,2,3)) ;
        var rgb_byteValues = argb.DecodedAsRgbByteValues() ;
        rgb_byteValues.Should().Be(rgb) ;           
      }

      RunPerformanceTests(handleMessage) ;

    }

    public static unsafe void RunPerformanceTests ( System.Action<string> handleMessage )
    {
      handleMessage("Testing EncodedPixel performance") ;

      int nElements = 2000 * 2000 ;
      var uintArray = new uint[nElements] ;
      var encodedPixelArray = new EncodedPixel_ARGB[nElements] ;

      handleMessage(
        $"Sizes : {
          sizeof(uint)
        } {
          sizeof(EncodedPixel_ARGB)
        }"
      ) ;

      using ( 
        var timer = new ExecutionTimerEx_ShowingMillisecsElapsed(
          $"Scanning a uint array of {nElements} elements",
          () => {
            uint sum = 0 ;
            for ( int i = 0 ; i < nElements ; i++ )
            {
              sum += uintArray[i] ;
            }
          },
          handleMessage
        )
      ) {
      }

      using ( 
        var timer = new ExecutionTimerEx_ShowingMillisecsElapsed(
          $"Scanning an EncodedPixel_ARGB array of {nElements} elements",
          () => {
            uint sum = 0 ;
            for ( int i = 0 ; i < nElements ; i++ )
            {
              sum += encodedPixelArray[i].ARGB ;
            }
          },
          handleMessage
        )
      ) {
      }

      using ( 
        var timer = new ExecutionTimerEx_ShowingMillisecsElapsed(
          $"Decoding a uint array of {nElements} elements",
          () => {
            for ( int i = 0 ; i < nElements ; i++ )
            {
              var _ = PixelEncodingHelpers.DecodeARGBAsRgbTuple(uintArray[i]) ;
            }
          },
          handleMessage
        )
      ) {
      }

      using ( 
        var timer = new ExecutionTimerEx_ShowingMillisecsElapsed(
          $"Decoding an EncodedPixel_ARGB array of {nElements} elements",
          () => {
            for ( int i = 0 ; i < nElements ; i++ )
            {
              var _ = encodedPixelArray[i].DecodedAsRgbTuple() ;
            }
          },
          handleMessage
        )
      ) {
      }

      using ( 
        var timer = new ExecutionTimerEx_ShowingMillisecsElapsed(
          $"Decoding a uint array of {nElements} elements AGAIN ...",
          () => {
            for ( int i = 0 ; i < nElements ; i++ )
            {
              var _ = PixelEncodingHelpers.DecodeARGBAsRgbTuple(uintArray[i]) ;
            }
          },
          handleMessage
        )
      ) {
      }

    }

  }

}

