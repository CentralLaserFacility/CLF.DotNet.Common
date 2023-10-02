//
// ColouredPixelArrayEncodedAsBytes_A_B_C.cs
//

using FluentAssertions;

namespace Clf.Common.ImageProcessing
{

  partial class ColouredPixelArrayEncodedAsBytes_A_B_C
  { 

    public bool IsValidX ( int x )
    => ( 
       x >= 0
    && x <  Width
    ) ;

    public bool IsValidY ( int y )
    => ( 
       y >= 0
    && y <  Height
    ) ;

    public int ClipToRange ( ref int i, int min, int max )
    {
      if ( i < min )
      {
        i = min ;
      }
      else if ( i > max )
      {
        i = max ;
      }
      return i ;
    }

    public bool CanEnsureValidX ( ref int x )
    => IsValidX( 
      ClipToRange(
        ref x,
        0,
        Width
      )
    ) ;

    public bool CanEnsureValidX ( ref int xStart, ref int xEnd )
    => (
       CanEnsureValidX ( ref xStart )
    && CanEnsureValidX ( ref xEnd   )
    ) ;

    public bool CanEnsureValidY ( ref int y )
    => IsValidY( 
      ClipToRange(
        ref y,
        0,
        Height
      )
    ) ;

    public bool CanEnsureValidY ( ref int yStart, ref int yEnd )
    => (
       CanEnsureValidY ( ref yStart )
    && CanEnsureValidY ( ref yEnd   )
    ) ;

    public void DrawHorizontalLine ( 
      int  xStart, 
      int  xEnd, 
      int  y,
      uint colour_ARGB
    ) {
      if (
         CanEnsureValidX ( ref xStart, ref xEnd )
      && IsValidY        ( y                    )
      ) {
        CanGetPixelOffset(
          xStart,
          y,
          out int pixelOffset 
        ).Should().BeTrue() ;
        CanGetPixelOffset(
          xEnd,
          y,
          out int pixelOffset_xEnd 
        ).Should().BeTrue() ;
      }
      // Hmm, we'll draw this with direct access !!
      for ( int x = xStart ; x <= xEnd ; x++ ) 
      {
        SetPixel_ARGB(x,y,colour_ARGB) ;
      }
    }

    public void DrawVerticalLine ( 
      int  x,
      int  yStart,
      int  yEnd,
      uint colour_ARGB
    ) {
      if (
         IsValidX        ( x                    )
      && CanEnsureValidX ( ref yStart, ref yEnd )
      ) {
        CanGetPixelOffset(
          x,
          yStart,
          out int pixelOffset 
        ).Should().BeTrue() ;
        CanGetPixelOffset(
          x,
          yEnd,
          out int pixelOffset_xEnd 
        ).Should().BeTrue() ;
      }
      // Hmm, we'll draw this with direct access !!
      for ( int y = yStart ; y <= yEnd ; y++ ) 
      {
        SetPixel_ARGB(x,y,colour_ARGB) ;
      }
    }

  }

}

