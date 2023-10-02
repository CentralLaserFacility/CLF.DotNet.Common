//
// EncodedPixelStructs.cs
//

using FluentAssertions ;

namespace Clf.Common.ImageProcessing
{

  public readonly struct EncodedPixelStruct_ARGB
  {

    public readonly uint ARGB ;
    
    public EncodedPixelStruct_ARGB ( uint argb )
    {
      ARGB = argb ;
    }

    public EncodedPixelStruct_ARGB ( byte red, byte green, byte blue ) : 
    this(
      PixelEncodingHelpers.EncodeARGB(red,green,blue)
    ) {
    }

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static EncodedPixelStruct_ARGB FromRgbTuple ( ( byte red, byte green, byte blue ) rgb )
    => new EncodedPixelStruct_ARGB( rgb.red, rgb.green, rgb.blue ) ;

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static EncodedPixelStruct_ARGB FromRgbByteValues ( RgbByteValues rgb )
    => new EncodedPixelStruct_ARGB ( rgb.R, rgb.G, rgb.B ) ;

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
    public EncodedPixelStruct_ABGR EncodedAsABGR ( ) => new EncodedPixelStruct_ABGR(
      PixelEncodingHelpers.ConvertToARGB(ARGB) 
    ) ;

  }

  public readonly struct EncodedPixelStruct_ABGR
  {

    public readonly uint ABGR ;
    
    public EncodedPixelStruct_ABGR ( uint abgr )
    {
      ABGR = abgr ;
    }

    public EncodedPixelStruct_ABGR ( byte red, byte green, byte blue ) : 
    this(
      PixelEncodingHelpers.EncodeABGR(red,green,blue)
    ) {
    }

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static EncodedPixelStruct_ABGR FromRgbTuple ( ( byte red, byte green, byte blue ) rgb )
    => new EncodedPixelStruct_ABGR ( rgb.red, rgb.green, rgb.blue ) ;

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public static EncodedPixelStruct_ABGR FromRgbByteValues ( RgbByteValues rgb )
    => new EncodedPixelStruct_ABGR ( rgb.R, rgb.G, rgb.B ) ;

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
    public EncodedPixelStruct_ARGB EncodedAsARGB ( ) => new EncodedPixelStruct_ARGB(
      PixelEncodingHelpers.ConvertToARGB(ABGR) 
    ) ;


  }

  public static class EncodedPixelStruct_Tests 
  {
    public static void RunTests ( System.Action<string> handleMessage )
    {
      var rgb = new RgbByteValues(1,2,3) ;

      var argb = EncodedPixelStruct_ARGB.FromRgbByteValues(rgb) ;
      argb.ARGB.Should().Be(0xff010203) ;
      {
        var rgb_tuple = argb.DecodedAsRgbTuple() ;
        rgb_tuple.Should().Be((1,2,3)) ;
        var rgb_byteValues = argb.DecodedAsRgbByteValues() ;
        rgb_byteValues.Should().Be(rgb) ;           
      }

      var abgr = EncodedPixelStruct_ABGR.FromRgbByteValues(rgb) ;
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
      handleMessage("Testing EncodedPixelStruct performance") ;

      int nElements = 2000 * 2000 ;
      var uintArray = new uint[nElements] ;
      var encodedPixelArray = new EncodedPixelStruct_ARGB[nElements] ;

      handleMessage(
        $"Sizes : {
          sizeof(uint)
        } {
          sizeof(EncodedPixelStruct_ARGB)
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
          $"Scanning an EncodedPixelStruct_ARGB array of {nElements} elements",
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
          $"Decoding an EncodedPixelStruct_ARGB array of {nElements} elements",
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

