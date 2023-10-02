//
// Helpers.cs
//

using System.Linq ;
using FluentAssertions ;
using System.Collections.Generic ;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Clf.Common.ImageProcessing
{

  public static class Helpers
  {

    // This could be made *much* faster, if we were to write
    // groups of four bytes (from an encoded Int32 value)
    // using pointer arithmetic or a 'Span' ...

    ////// private static void BuildColouredPixelArrayEncodedAsBytes ( 
    //////   LinearArrayHoldingPixelBytes          grayScaleIntensityValues, 
    //////   byte[]                                R, 
    //////   byte[]                                G, 
    //////   byte[]                                B,
    //////   ref ColouredPixelArrayEncodedAsBytes_Base? pixelArrayEncodedAsBytes
    ////// ) {
    //////   // In many cases we'll be able to write the result into
    //////   // a previously allocated array, and avoid the overhead of
    //////   // creating a new one ...
    //////   if ( 
    //////      pixelArrayEncodedAsBytes?.Width  != grayScaleIntensityValues.Width 
    //////   || pixelArrayEncodedAsBytes?.Height != grayScaleIntensityValues.Height
    //////   ) {
    //////     // The pixelArray we've already created has a size that differs from
    //////     // the one we need, so let's allocate a new one. Note that we need to compare
    //////     // the Width and Height rather than just the total number of pixels ... gotcha !!
    //////     pixelArrayEncodedAsBytes = new ColouredPixelArrayEncodedAsBytes_ABC(
    //////       grayScaleIntensityValues.Width,
    //////       grayScaleIntensityValues.Height
    //////     ) ;
    //////     System.Diagnostics.Debug.WriteLine(
    //////       $"ColouredPixelArrayEncodedAsBytes has been reallocated ({pixelArrayEncodedAsBytes.Width} x {pixelArrayEncodedAsBytes.Height}) "
    //////     ) ;
    //////   }
    //////   pixelArrayEncodedAsBytes!.Width.Should().Be(grayScaleIntensityValues.Width) ; 
    //////   pixelArrayEncodedAsBytes!.Height.Should().Be(grayScaleIntensityValues.Height) ;
    //////   var iImageData = 0 ;
    //////   for ( int iGrayScaleData = 0 ; iGrayScaleData < grayScaleIntensityValues.PixelCount ; iGrayScaleData++ ) 
    //////   {
    //////     // Slow but sure ... and more efficient than using SetPixel ??
    //////     byte greyScaleIntensity = grayScaleIntensityValues.LinearArrayValues[iGrayScaleData] ;
    //////     byte red   = R[greyScaleIntensity] ;
    //////     byte green = G[greyScaleIntensity] ;
    //////     byte blue  = B[greyScaleIntensity] ;
    //////     pixelArrayEncodedAsBytes.ColouredImageBytesLinearArray[iImageData + 0] = red   ; // R
    //////     pixelArrayEncodedAsBytes.ColouredImageBytesLinearArray[iImageData + 1] = green ; // G
    //////     pixelArrayEncodedAsBytes.ColouredImageBytesLinearArray[iImageData + 2] = blue  ; // B
    //////     pixelArrayEncodedAsBytes.ColouredImageBytesLinearArray[iImageData + 3] = 255   ; // A
    //////     iImageData += 4 ;
    //////   }
    ////// }

    public static void BuildColouredPixelArrayEncodedAsBytes ( 
      LinearArrayHoldingPixelBytes               grayScaleIntensityValues,
      ColourMappingTable                         colourMappingTable,
      ref ColouredPixelArrayEncodedAsBytes_Base? colouredPixelArrayEncodedAsBytes
    ) {
      // In many cases we'll be able to write the result into
      // a previously allocated array, and avoid the overhead of
      // creating a new one ...
      if ( 
         colouredPixelArrayEncodedAsBytes?.Width  != grayScaleIntensityValues.Width 
      || colouredPixelArrayEncodedAsBytes?.Height != grayScaleIntensityValues.Height
      ) {
        // The pixelArray we've already created has a size that differs from
        // the one we need, so let's allocate a new one. Note that we need to compare
        // the Widths and Heights rather than just the total number of pixels ... gotcha !!
        // colouredPixelArrayEncodedAsBytes = new ColouredPixelArrayEncodedAsBytes_ABC(
        //   grayScaleIntensityValues.Width,
        //   grayScaleIntensityValues.Height
        // ) ;
        colouredPixelArrayEncodedAsBytes = ColouredPixelArrayEncodedAsBytes_Base.CreateInstance(
         grayScaleIntensityValues,
         colourMappingTable
        ) ;
        /*System.Diagnostics.Debug.WriteLine(
          $"ColouredPixelArrayEncodedAsBytes has been reallocated ({colouredPixelArrayEncodedAsBytes.Width} x {colouredPixelArrayEncodedAsBytes.Height}) "
        ) ;*/
      }
      else
      {
        // colouredPixelArrayEncodedAsBytes!.Width.Should().Be(grayScaleIntensityValues.Width) ; 
        // colouredPixelArrayEncodedAsBytes!.Height.Should().Be(grayScaleIntensityValues.Height) ;
        colouredPixelArrayEncodedAsBytes.LoadFromIntensityBytesArray(
          grayScaleIntensityValues, 
          colourMappingTable
        ) ;
      }
    }

    public static byte[] CreateCloneOfByteArray ( byte[] original )
    {
      // byte[] clone = new byte[original.Length] ;
      // System.Array.Copy(
      //   original,
      //   clone,
      //   original.Length
      // ) ;
      // return clone ;
      return (byte[]) original.Clone() ;
    }

    public static ushort[] CreateCloneOfUShortArray ( ushort[] original )
    {
      return (ushort[]) original.Clone() ;
    }

    //
    // ---------------------------------------------------------
    // OLD VERSIONS ...
    //

    // public static void BuildColouredPixelArrayEncodedAsBytes ( 
    //   IReadOnlyList<byte>                   grayScaleIntensityValues, 
    //   int                                   displayWidth, 
    //   int                                   displayHeight,
    //   byte[]                                R, 
    //   byte[]                                G, 
    //   byte[]                                B,
    //   ref ColouredPixelArrayEncodedAsBytes? pixelArrayEncodedAsBytes
    // ) {
    //   // In many cases we'll be able to write the result into
    //   // a previously allocated array, and avoid the overhead of
    //   // creating a new one ...
    //   if ( 
    //      pixelArrayEncodedAsBytes?.Width  != displayWidth 
    //   || pixelArrayEncodedAsBytes?.Height != displayHeight
    //   ) {
    //     // The pixelArray we've already created has a size that differs from
    //     // the one we need, so let's allocate a new one. Note that we need to compare
    //     // the Width and Height rather than just the total number of pixels ... gotcha !!
    //     pixelArrayEncodedAsBytes = new ColouredPixelArrayEncodedAsBytes(
    //       displayWidth,
    //       displayHeight
    //     ) ;
    //     System.Diagnostics.Debug.WriteLine(
    //       $"ColouredPixelArrayEncodedAsBytes has been reallocated /( {pixelArrayEncodedAsBytes.Width} /x {pixelArrayEncodedAsBytes.Height}) "
    //     ) ;
    //   }
    //   pixelArrayEncodedAsBytes!.Width.Should().Be(displayWidth) ; 
    //   pixelArrayEncodedAsBytes!.Height.Should().Be(displayHeight) ;
    //   var iImageData = 0 ;
    //   for ( int iGrayScaleData = 0 ; iGrayScaleData < grayScaleIntensityValues.Count ; //i GrayScaleData++ ) 
    //   {
    //     // Slow but sure ... and more efficient than using SetPixel ??
    //     byte greyScaleIntensity = grayScaleIntensityValues[iGrayScaleData] ;
    //     byte red   = R[greyScaleIntensity] ;
    //     byte green = G[greyScaleIntensity] ;
    //     byte blue  = B[greyScaleIntensity] ;
    //     pixelArrayEncodedAsBytes.ColouredImageBytesLinearArray[iImageData + 0] = red   ; // R
    //     pixelArrayEncodedAsBytes.ColouredImageBytesLinearArray[iImageData + 1] = green ; // G
    //     pixelArrayEncodedAsBytes.ColouredImageBytesLinearArray[iImageData + 2] = blue  ; // B
    //     pixelArrayEncodedAsBytes.ColouredImageBytesLinearArray[iImageData + 3] = 255   ; // A
    //     iImageData += 4 ;
    //   }
    // }

    // public static void PerformNormalisation ( 
    //   bool   autoNormalisation,
    //   byte   normalisationValue,
    //   byte[] imageIntensityValues 
    // ) {
    //   if ( autoNormalisation) 
    //   {
    //     normalisationValue = imageIntensityValues.Max() ;
    //   }
    //   double referenceValue = normalisationValue ;
    //   for ( int i = 0; i < imageIntensityValues.Length ; i++ )
    //   {
    //     byte value = imageIntensityValues[i] ;
    //     double fractionalValue = value / referenceValue ;
    //     if ( fractionalValue >= 0.99 )
    //     {
    //       fractionalValue = 0.99 ;
    //     }
    //     imageIntensityValues[i] = (byte) ( fractionalValue * 255 ) ;
    //   }
    // }

    // public static void ComputeResizedImage (
    //   byte[]          originalImage, 
    //   int             originalWidthX, // Hmm, we should be using a 'Dimensions' record here !!!
    //   int             originalHeightY, 
    //   int             resizedWidthX, 
    //   int             resizedHeightY,
    //   ref PixelArray? resizedImagePixelArray
    // ) {
    //   if ( 
    //      resizedWidthX  != resizedImagePixelArray?.Width
    //   || resizedHeightY != resizedImagePixelArray?.Height
    //   ) {
    //     // The 'resized' dimensions are different from the sizes
    //     // of the PixelArray that we've already allocated,
    //     // so we'll allocate a new PixelArray of the required dimensions
    //     if ( 
    //        resizedWidthX  == originalWidthX 
    //     && resizedHeightY == originalHeightY
    //     ) {
    //       // The resized dimensions are identical to the
    //       // dimensions of the original image, so our 'resized'
    //       // pixel array can just re-use that array of pixels
    //       resizedImagePixelArray = new PixelArray(
    //         resizedWidthX,
    //         resizedHeightY,
    //         originalImage
    //       ) ;
    //       return ;
    //     }
    //     else
    //     {
    //       resizedImagePixelArray = new PixelArray(
    //         resizedWidthX,
    //         resizedHeightY
    //       ) ;
    //     }
    //     System.Diagnostics.Debug.WriteLine(
    //       $"PixelArray has been reallocated ({resizedImagePixelArray.Width} x {resizedImagePixelArray.Height}) "
    //     ) ;
    //   }
    //   else
    //   {
    //     // The dimensions of our already allocated PixelArray,
    //     // into which we'll place the 'resized' result, match
    //     // the desired 'resized' dimensions.
    //   }
    //   resizedImagePixelArray.Width.Should().Be(resizedWidthX) ;
    //   resizedImagePixelArray.Height.Should().Be(resizedHeightY) ;
    //   byte[] resizedImageBytes = resizedImagePixelArray!.ImageBytesLinearArray ;
    //   // Compute a 'resized' image using 'nearest-neighbour' interpolation.
    //   // If we're computing a reduced-size image, where the resized width
    //   // is say 0.5 times the original image width, then our 'xStepFactor'
    //   // will be 2.0 ; that is, each 'resized' pixel will come from an 
    //   // original pixel at a position of x-times-xStepFactor, ie we skip
    //   // over some of the original pixels.
    //   // If we're computing a larger image, where the resized width
    //   // is say 2 times the original image width, then our 'xStepFactor'
    //   // will be 0.5 ; that is, each 'resized' pixel will come from an 
    //   // original pixel at a position of x-times-xStepFactor, ie we'll
    //   // repeat some of the original pixels when creating the new image.
    //   // Another way of thinking about this is that the 'step factor'
    //   // represents the amount by which the size is being *reduced*.
    //   // For a step factor of 2, the size will be reduced by a factor of 2.
    //   double xStepFactor = ( (double) originalWidthX  ) / resizedWidthX ;
    //   double yStepFactor = ( (double) originalHeightY ) / resizedHeightY ;
    //   for ( int yResized = 0 ; yResized < resizedHeightY ; yResized++ )
    //   {
    //     // Step along visiting each 'resized' pixel on the Y axis
    //     int originalIndexY = (int) ( yResized * yStepFactor ) ;
    //     for ( int xResized = 0 ; xResized < resizedWidthX ; xResized++ )
    //     {
    //       // Step along visiting each 'resized' pixel on the X axis
    //       int originalIndexX = (int) ( xResized * xStepFactor ) ;
    //       // Get the 'original' pixel value
    //       byte originalPixel = originalImage[
    //         originalIndexX 
    //       + originalIndexY * originalWidthX
    //       ] ;
    //       // Write the 'resized' pixel value
    //       resizedImageBytes[
    //         xResized 
    //       + yResized * resizedWidthX
    //       ] = originalPixel ;
    //     }
    //   }
    // }

    //
    // Verify that the pointer we're about to use to access an array element
    // lies within the memory area assigned to the array.
    //
    //     0   1   2   3
    //   +---+---+---+---+
    //   |   |   |   |   | 4 elements
    //   +---+---+---+---+
    //     |           |
    //     p          p+3
    //
    // This code is only active in a DEBUG build.
    //

    [System.Diagnostics.Conditional("DEBUG")]
    public unsafe static void VerifyArrayElementPointerValidity<T> ( 
      T * p, 
      T * pArrayFirstElement, 
      int nArrayElementsAllocated
    )  
    where T : unmanaged
    {
      // nint q = (nint) p ;
      // nint qArrayFirstElement = (nint) pArrayFirstElement ;
      // nint qArrayLastElement = qArrayFirstElement + nArrayElements - 1 ;
      // q.Should().BeInRange(qArrayFirstElement,qArrayLastElement) ;
      ( p >= pArrayFirstElement ).Should().BeTrue() ;
      ( p < pArrayFirstElement + nArrayElementsAllocated ).Should().BeTrue() ;
    }

    //
    // As above, but anticipating that we'll be writing several elements sequentially
    //

    [System.Diagnostics.Conditional("DEBUG")]
    public unsafe static void VerifyArrayElementPointerValidity<T> ( 
      T * pFirstElementToBeAccessed, 
      int nElementsToBeAccessed,
      T * pArrayFirstElement, 
      int nArrayElementsAllocated
    )  
    where T : unmanaged
    {
      ( pFirstElementToBeAccessed >= pArrayFirstElement ).Should().BeTrue() ;
      T * pLastElementToBeAccessed = pFirstElementToBeAccessed + nElementsToBeAccessed - 1 ;
      ( pLastElementToBeAccessed < pArrayFirstElement + nArrayElementsAllocated ).Should().BeTrue() ;
    }

    //
    // When we get PV values that represent a Contour (or the corners of a box)
    // they are provided as an array of signed 32 bit integers that has four
    // consecutive '0' values to mark the end of the data.
    //
    // Here we convert the data to a sequence of 'int' valued X/Y tuples.
    //
    // TODO : Move this to ChannelAccess ??
    //

    // This variant relies on .Net 7 where we can constrain T be be any numeric type

    public static IEnumerable<(T,T)> GetValuesAsEnumerableOfTuples<T> ( 
      params T[] valuePairs_terminatedByTwoZeroPairs 
    )
    where T : System.Numerics.INumber<T>
    {
      List<(T,T)> tuplesList = new() ;
      (T,T) zeroTuple = (T.Zero,T.Zero) ;
      (T,T)? previousTuple = null ;
      int iValue = 0 ;
      //
      // We step through the array of points, two items at a time,
      // adding an (x,y) tuple to our list at each step ; until
      // either (A) we come to the end of the array,
      // or (B) we detect that the tuple we're about to add
      // is (0,0), and the previously added tuple is also (0,0).
      // In that case we remove that most recently added tuple.
      for ( int iPair = 0 ; iPair < valuePairs_terminatedByTwoZeroPairs.Length / 2 ; iPair++ )
      {
        var tuple = (
          valuePairs_terminatedByTwoZeroPairs[iValue],
          valuePairs_terminatedByTwoZeroPairs[iValue+1]
        ) ;
        if (
           tuple         == zeroTuple
        && previousTuple == zeroTuple
        ) {
          // This is the 2nd consecutive 'zero' pair we've seen.
          // The first one has already been added to the list,
          // so we'll remove it
          return tuplesList.Take(
            tuplesList.Count() - 1
          ) ;
        }
        tuplesList.Add(tuple) ;
        previousTuple = tuple ;
        iValue += 2 ;
      }
      return tuplesList ;
    }

    // Original version was hard coded to work with 'int' values
    // public static IEnumerable<(int x,int y)> GetValuesAsEnumerableOfTuples ( params int[] valuePairs_terminatedByTwoZeroPairs )
    // {
    //   List<(int,int)> tuplesList = new() ;
    //   (int,int) zeroTuple = (0,0) ;
    //   (int,int)? previousTuple = null ;
    //   int iValue = 0 ;
    //   //
    //   // We step through the array of points, two items at a time,
    //   // adding an (x,y) tuple to our list at each step ; until
    //   // either (A) we come to the end of the array,
    //   // or (B) we detect that the tuple we're about to add
    //   // is (0,0), and the previously added tuple is also (0,0).
    //   // In that case we remove that most recently added tuple.
    //   for ( int iPair = 0 ; iPair < valuePairs_terminatedByTwoZeroPairs.Length / 2 ; iPair++ )
    //   {
    //     var tuple = (
    //       valuePairs_terminatedByTwoZeroPairs[iValue],
    //       valuePairs_terminatedByTwoZeroPairs[iValue+1]
    //     ) ;
    //     if (
    //        tuple         == zeroTuple
    //     && previousTuple == zeroTuple
    //     ) {
    //       // This is the 2nd consecutive 'zero' pair we've seen.
    //       // The first one has already been added to the list,
    //       // so we'll remove it
    //       return tuplesList.Take(
    //         tuplesList.Count() - 1
    //       ) ;
    //     }
    //     tuplesList.Add(tuple) ;
    //     previousTuple = tuple ;
    //     iValue += 2 ;
    //   }
    //   return tuplesList ;
    // }

    public static void VisitPixelsOnLine (
      int                    x1,
      int                    y1,
      int                    x2,
      int                    y2,
      System.Action<int,int> visitPixel
    ) {
      // Bresenham's algorithm
      if ( x1 > x2 )
      {
        Swap( ref x1, ref x2 ) ;
        Swap( ref y1, ref y2 ) ;
      }
      int dx = x2 - x1 ;
      int e = 0 ;
      if ( y1 <= y2 )
      {
        // Case where y increases
        int dy = y2 - y1 ;
        if ( dx >= dy )
        {
          for(;;)
          {
            visitPixel(x1,y1) ;
            if ( x1 == x2 )
              break ;
            x1++ ;
            e += 2 * dy ;
            if ( e > dx )
            {
              e -= dx * 2 ;
              y1++ ;
            }
          }
        }
        else
        {
          for(;;)
          {
            visitPixel(x1,y1) ;
            if ( y1 == y2 )
              break ;
            y1++ ;
            e += dx * 2 ;
            if ( e > dy )
            {
              e -= dy * 2 ;
              x1++ ;
            }
          }
        }
      }
      else
      {
        // Case where y decreases
        int dy = y1 - y2 ;
        if ( dx >= dy )
        {
          for(;;)
          {
            visitPixel(x1,y1) ;
            if ( x1 == x2 )
              break ;
            x1++ ;
            e += 2 * dy ;
            if ( e > dx )
            {
              e -= dx * 2 ;
              y1-- ;
            }
          }
        }
        else
        {
          for(;;)
          {
            visitPixel(x1,y1) ;
            if ( y1 == y2 )
              break ;
            y1-- ;
            e += dx * 2 ;
            if ( e > dy )
            {
              e -= dy * 2 ;
              x1++ ;
            }
          }
        }
      }
      // Local function
      void Swap ( 
        ref int a, 
        ref int b 
      ) {
        // a ^= b ;
        int x = a ;
        a = b ;
        b = x ;
      }
    }

    //
    // Algorithm used here is based on:
    // https://www.javatpoint.com/computer-graphics-bresenhams-circle-algorithm
    //
    // See also
    // https://www.geeksforgeeks.org/bresenhams-circle-drawing-algorithm/
    // https://en.wikipedia.org/wiki/Midpoint_circle_algorithm
    // https://banu.com/blog/7/drawing-circles/
    // https://www.gatevidyalay.com/bresenham-circle-drawing-algorithm/
    // https://imruljubair.github.io/teaching/material/CSE4203/Chapter%20-%208%20(part%20-%20B).pdf
    // https://uomustansiriyah.edu.iq/media/lectures/12/12_2020_06_26!11_47_57_PM.pdf
    //
    // Note that since we draw the circle in quadrants, we can
    // use the same approach to draw a rectangle with rounded corners !
    //

    public static void VisitPixelsOnCircle ( 
      int                        centreX, 
      int                        centreY, 
      int                        radius, 
      System.Action<int,int,int> visitPixel // x,y,octant
    ) {  
      int x = 0 ;
      int y = radius ;
      int d = 3 - ( 2 * radius ) ;  
      VisitPoint_EightWaySymmetric(x,y) ;  
      while ( x <= y )  
      {  
        if ( d <= 0 )  
        {  
          d = d + ( 4 * x ) + 6 ;  
        }  
        else  
        {  
          d = d + ( 4 *x ) - ( 4 * y ) + 10 ;  
          y = y - 1 ;  
        }  
        x = x + 1 ;  
        VisitPoint_EightWaySymmetric(x,y) ;  
      }  
      // Local function
      void VisitPoint_EightWaySymmetric ( int x, int y )  
      {  
        // if ( x == y )
        // {
        //   visitPixel ( centreX + x , centreY + y, 1 ) ;  
        //   visitPixel ( centreX + x , centreY - y, 2 ) ;  
        //   visitPixel ( centreX - x , centreY + y, 3 ) ;  
        //   visitPixel ( centreX - x , centreY - y, 4 ) ;  
        //   // visitPixel ( centreX + y , centreY + x, 5 ) ;  
        //   // visitPixel ( centreX + y , centreY - x, 6 ) ;  
        //   // visitPixel ( centreX - y , centreY + x, 8 ) ;  
        //   // visitPixel ( centreX - y , centreY - x, 7 ) ;  
        // }
        // else
        {
          visitPixel ( centreX + x , centreY + y, 1 ) ;  
          visitPixel ( centreX + x , centreY - y, 2 ) ;  
          visitPixel ( centreX - x , centreY + y, 3 ) ;  
          visitPixel ( centreX - x , centreY - y, 4 ) ;  
          visitPixel ( centreX + y , centreY + x, 5 ) ;  
          visitPixel ( centreX + y , centreY - x, 6 ) ;  
          visitPixel ( centreX - y , centreY + x, 8 ) ;  
          visitPixel ( centreX - y , centreY - x, 7 ) ;  
        }
      }     
    }  
  
    //
    // ATTEMPTING TO FIX THE PROBLEM WITH THE ABOVE ALGORITM,
    // WHICH VISITS CERTAIN PIXELS MORE THAN ONCE, AT THE ENDS OF THE ARCS.
    // A CLEAN SOLUTION FOR THAT IS ACTUALLY NOT STRAIGHTFORWARD.
    //
    // // https://iq.opengenus.org/bresenhams-circle-drawing-algorithm/
    // 
    // public static void VisitPixelsOnCircleEx ( 
    //   int                        centreX, 
    //   int                        centreY, 
    //   int                        radius, 
    //   System.Action<int,int,int,int> visitPixel // x,y,octant,offset
    // ) {  
    //   int x = 0 ;
    //   int y = radius ;
    //   int d = 3 - ( 2 * radius ) ;  
    //   int offset = 0 ;
    //   VisitPoint_EightWaySymmetric(x,y) ;
    //   while ( y >= x )  
    //   {  
    //     x++ ;  
    //     if ( d > 0 )  
    //     {  
    //       y-- ;  
    //       d = d + 4 * ( x - y ) + 10 ;
    //     }  
    //     else  
    //     {  
    //       d = d + ( 4 * x ) + 6 ;  
    //     }  
    //     offset++ ;
    //     VisitPoint_EightWaySymmetric(x,y) ;
    //   }
    //   // Local function
    //   void VisitPoint_EightWaySymmetric ( int x, int y )  
    //   {  
    //     if ( x == y )
    //     {
    //       visitPixel ( centreX + x , centreY + x, 1 , offset ) ;  
    //       visitPixel ( centreX + x , centreY - x, 2 , offset ) ;  
    //       visitPixel ( centreX - x , centreY + x, 3 , offset ) ;  
    //       visitPixel ( centreX - x , centreY - x, 4 , offset ) ;  
    //    // visitPixel ( centreX + y , centreY + x, 5 , offset ) ;  
    //    // visitPixel ( centreX + y , centreY - x, 6 , offset ) ;  
    //    // visitPixel ( centreX - y , centreY + x, 8 , offset ) ;  
    //    // visitPixel ( centreX - y , centreY - x, 7 , offset ) ;  
    //     }
    //     else if ( x == 0 )
    //     {
    //       visitPixel ( centreX     , centreY + y, 1 , offset ) ;  
    //       visitPixel ( centreX     , centreY - y, 2 , offset ) ;  
    //    // visitPixel ( centreX     , centreY + y, 3 , offset ) ;  
    //    // visitPixel ( centreX     , centreY - y, 4 , offset ) ;  
    //       visitPixel ( centreX + y , centreY, 5 , offset ) ;  
    //       visitPixel ( centreX - y , centreY, 6 , offset ) ;  
    //    // visitPixel ( centreX + y , centreY, 7 , offset ) ;  
    //    // visitPixel ( centreX - y , centreY, 8 , offset ) ;  
    //     }
    //     else if ( y == 0 )
    //     {
    //       visitPixel ( centreX + x , centreY    , 1 , offset ) ;  
    //    // visitPixel ( centreX + x , centreY    , 2 , offset ) ;  
    //       visitPixel ( centreX - x , centreY    , 3 , offset ) ;  
    //    // visitPixel ( centreX - x , centreY    , 4 , offset ) ;  
    //       visitPixel ( centreX     , centreY + x, 5 , offset ) ;  
    //    // visitPixel ( centreX     , centreY + x, 6 , offset ) ;  
    //       visitPixel ( centreX     , centreY - x, 7 , offset ) ;  
    //    // visitPixel ( centreX     , centreY - x, 8 , offset ) ;  
    //     }
    //     else
    //     {
    //       visitPixel ( centreX + x , centreY + y, 1 , offset ) ;  
    //       visitPixel ( centreX + x , centreY - y, 2 , offset ) ;  
    //       visitPixel ( centreX - x , centreY + y, 3 , offset ) ;  
    //       visitPixel ( centreX - x , centreY - y, 4 , offset ) ;  
    //       visitPixel ( centreX + y , centreY + x, 5 , offset ) ;  
    //       visitPixel ( centreX - y , centreY + x, 6 , offset ) ;  
    //       visitPixel ( centreX + y , centreY - x, 7 , offset ) ;  
    //       visitPixel ( centreX - y , centreY - x, 8 , offset ) ;  
    //     }
    //   }     
    // }  
    // 
    // //
    // // Alternative algorithm based on this article :
    // // https://www.codeproject.com/Articles/30686/Bresenham-s-Line-Algorithm-Revisited
    // //
    // 
    // public static void VisitPixelsOnCircle_old_01 ( 
    //   int                        centreX, 
    //   int                        centreY, 
    //   int                        radius, 
    //   System.Action<int,int,int> visitPixel // x,y,octant
    // ) {  
    // 
    //   int f     = 1 - radius ;
    //   int ddF_x = 1 ;
    //   int ddF_y = -2 * radius ;
    // 
    //   int x = 0 ;
    //   int y = radius ;
    // 
    //   visitPixel( centreX          , centreY + radius , 0 ) ;
    //   visitPixel( centreX          , centreY - radius , 0 ) ;
    //   visitPixel( centreX + radius , centreY          , 0 ) ;
    //   visitPixel( centreX + radius , centreY          , 0 ) ;
    // 
    //   while ( x < y )
    //   {
    //     if ( f >= 0 )
    //     {
    //       y-- ;
    //       ddF_y += 2 ;
    //       f += ddF_y ;
    //     }
    //     x++ ;
    //     ddF_x += 2 ;
    //     f += ddF_x ;
    //     visitPixel( centreX + x , centreY + y , 1 ) ;
    //     visitPixel( centreX - x , centreY + y , 2 ) ;
    //     visitPixel( centreX + x , centreY - y , 3 ) ;
    //     visitPixel( centreX - x , centreY - y , 4 ) ;
    //     visitPixel( centreX + y , centreY + x , 5 ) ;
    //     visitPixel( centreX - y , centreY + x , 6 ) ;
    //     visitPixel( centreX + y , centreY - x , 7 ) ;
    //     visitPixel( centreX - y , centreY - x , 8 ) ;
    //   }
    // }

    // //
    // // This reference provides a very thorough description of the algorithm,
    // // but it still suffers from the same issue - pixels are visited more than once !
    // // https://imruljubair.github.io/teaching/material/CSE4203/Chapter%20-%208%20(part%20-%20B).pdf
    // //
    // 
    // public static void MidpointCircle ( int radius, System.Action<int,int> PlotPoint )
    // {
    //   int x = 0 ;
    //   int y = radius ;
    //   int d = 1 - radius ;
    //   CirclePoints(x,y) ;
    //   while ( y > x )
    //   {
    //     if ( d < 0 ) 
    //     {
    //       d = d + 2 * x + 3 ;
    //     }
    //     else 
    //     { 
    //       d = d + 2 * ( x - y ) + 5 ;
    //       y = y - 1 ;
    //     }
    //     x = x + 1 ;
    //     CirclePoints(x,y) ;
    //   }
    //   void CirclePoints ( int x, int y )
    //   {
    //     PlotPoint (  x ,  y ) ;
    //     PlotPoint (  x , -y ) ;
    //     PlotPoint ( -x ,  y ) ;
    //     PlotPoint ( -x , -y ) ;
    //     PlotPoint (  y ,  x ) ;
    //     PlotPoint (  y , -x ) ;
    //     PlotPoint ( -y ,  x ) ;
    //     PlotPoint ( -y , -x ) ;
    //   }
    // }

  }

}

