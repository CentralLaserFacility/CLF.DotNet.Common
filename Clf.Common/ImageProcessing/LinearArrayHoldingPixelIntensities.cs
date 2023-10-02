//
// LinearArrayHoldingPixelIntensities.cs
//

using System.Collections.Generic ;
using System.Linq ;
using FluentAssertions ;

namespace Clf.Common.ImageProcessing
{

  //
  // This class owns an array-of-values that represents the intensities
  // of the pixels in a 2-D image of a specified Width and Height.
  //
  // Example :
  //
  //   Width  = 3 ; x is 0..2
  //   Height = 2 ; y is 0..1
  //
  // Logical representation as a 2-D image :
  //
  //    +---+---+---+
  //    | 0 | 1 | 2 |  Offset into 1-D array for [x,y]
  //    +---+---+---+
  //    | 3 | 4 | 5 |  is : x + ( y * Width )     
  //    +---+---+---+
  //
  // Physical representation as a 1-D array of values :
  //
  //    +---+---+---+---+---+---+
  //    | 0 | 1 | 2 | 3 | 4 | 5 |  
  //    +---+---+---+---+---+---+
  //
  // The 'intensities' array is either
  // (A) allocated elsewhere and passed in as a constructor argument,
  // or (B) is allocated when the instance is created, and initialised
  // with intensity values of zero.
  // 
  // In some scenarios it's useful to work with individual pixels identified
  // by their [x,y] coordinates. In other scenarios, such as normalisation,
  // we want to operate on all the pixels without regard for their position.
  //
  // The array elements can be mutated in place by explicitly calling methods
  // such as SetPixel, SetAllPixels. Read-only access to the underlying sequence
  // of array elements is provided via the 'LinearArrayValues' property.
  //
  // The following 'high level' operations are supported :
  //
  //   Resizing      : create a 'resized' clone of the array.
  //
  //   Rotation      : create a clone that is rotated by a multiple of 90 degrees,
  //                   and/or mirrored.
  //
  //   Normalisation : either creating a clone, or applying a scaling factor in place.
  //

  //
  // This supports 12-bit and 16-bit intensity values.
  //
  // With .net 7 we'll be able to use 'INumeric' and coalesce this with the 'byte' version.
  // 

  public partial class LinearArrayHoldingPixelIntensities 
  {

    public static LinearArrayHoldingPixelIntensities Create_WithRandomValues (
      int width,
      int height,
      int maxValue
    ) {
      return new LinearArrayHoldingPixelIntensities(
        width,  
        height,
        (x,y) => (ushort) System.Random.Shared.Next(maxValue)
      ) ;
    }

    public static LinearArrayHoldingPixelIntensities Create_WithRandomValues (
      int width,
      int height,
      int maxValue,
      int patchSize
    ) {
      int nPatchesX = width / patchSize ;
      int nPatchesY = height / patchSize ;
      var patchValues = new ushort[nPatchesX+1,nPatchesY+1] ;
      for ( int y = 0 ; y <= nPatchesY ; y++ )
      {
        for ( int x = 0 ; x <= nPatchesX ; x++ )
        {
          patchValues[x,y] = (ushort) System.Random.Shared.Next(maxValue) ;
        }
      }
      return new LinearArrayHoldingPixelIntensities(
        width,  
        height,
        (x,y) => patchValues[
          x / patchSize,
          y / patchSize
        ]
      ) ;
    }

    public int Width { get ; }

    public int Height { get ; }

    public int PixelCount => Width * Height ;

    public int MaxPixelIndex => PixelCount - 1 ;

    public IReadOnlyList<ushort> LinearArrayValues => m_linearArray ;

    public int InstanceNumber {  get ; }

    internal ushort[] m_linearArray ; // This is directly accessed via an unsafe pointer ...

    private static int m_nInstancesCreated = 0 ;

    // Create an instance, allocating a new array of appropriate size,
    // with all the pixel values initialised to zero.

    public LinearArrayHoldingPixelIntensities (
      int                        width,
      int                        height
    ) {
      InstanceNumber = m_nInstancesCreated++ ;
      Width  = width ;
      Height = height ;
      m_linearArray = new ushort[PixelCount] ;
    }

    // Create an instance, allocating a new array of appropriate size
    // with the option of initialising each pixel according to its position.

    public LinearArrayHoldingPixelIntensities (
      int                         width,
      int                         height,
      System.Func<int,int,ushort> evaluatePixelFunc
    ) :
    this(
      width,
      height
    ) {
      SetAllPixelsInPlace(evaluatePixelFunc) ;
    }

    public void LoadLinearArrayWithAscendingValues_ForTesting ( )
    {
      for ( int i = 0 ; i < m_linearArray.Length ; i++ )
      {
        m_linearArray[i] = (ushort) i ;
      }
    }

    // Create an instance, taking ownership of the array passed in.
    // It is assumed that client code will not retain a reference to
    // this original array, and that henceforth it will only be accessed
    // via this class.

    public LinearArrayHoldingPixelIntensities (
      int    width,
      int    height,
      ushort[] linearArrayThatWeWillTakeOwnershipOf
    ) {
      InstanceNumber = m_nInstancesCreated++ ;
      Width  = width ;
      Height = height ;
      linearArrayThatWeWillTakeOwnershipOf.Length.Should().Be(PixelCount) ;
      m_linearArray = linearArrayThatWeWillTakeOwnershipOf ;
    }

    public ushort MaxValue => m_linearArray.Max() ;

    public ushort MinValue => m_linearArray.Min() ;

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public int GetOffsetOfPixelAt ( 
      int x, 
      int y
    ) {
      int pixelOffset = (
        x
      + y * Width
      ) ;
      if ( 
         pixelOffset < 0 
      || pixelOffset > MaxPixelIndex
      ) {
        // The pixel we're asking for is
        // the bounds of the pixel array ...
        throw new System.IndexOutOfRangeException() ;
      }
      return pixelOffset ;
    }

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public ( int X, int Y ) GetPixelCoordinatesFromOffset ( int pixelOffset )
    {
      // if ( 
      //    pixelOffset < 0 
      // || pixelOffset > MaxPixelIndex
      // ) {
      //   // The pixel we're asking for is
      //   // the bounds of the pixel array ...
      //   throw new System.IndexOutOfRangeException() ;
      // }
      return (
        X : pixelOffset % Width,
        Y : pixelOffset / Width
      ) ;
    }

    // Extract pixel values

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public bool CanGetPixel ( 
      int      x, 
      int      y, 
      out ushort pixelValue
    ) {
      int pixelOffset = (
        x
      + y * Width
      ) ;
      if ( 
         pixelOffset < 0 
      || pixelOffset > MaxPixelIndex
      ) {
        // The pixel we'd get is outside
        // the bounds of the pixel array ...
        pixelValue = 0 ;
        return false ;
      }
      pixelValue = m_linearArray[pixelOffset] ;
      return true ;
    }

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public ushort GetPixel ( 
      int x, 
      int y
    ) {
      int pixelOffset = (
        x
      + y * Width
      ) ;
      return m_linearArray[pixelOffset] ;
    }

    public ushort this [ int x, int y ] => GetPixel(x,y) ;

    public IReadOnlyList<ushort> GetRowOfPixelsAtOffsetFromTop ( int y ) 
    {
      ushort[] rowOfPixels = new ushort[Width] ;
      if ( y < Height ) 
      {
        System.Array.Copy(
          sourceArray      : m_linearArray,
          sourceIndex      : y * Width,
          destinationArray : rowOfPixels,
          destinationIndex : 0,
          length           : Width
        ) ;
      }
      return rowOfPixels ;
    }

    public IReadOnlyList<ushort> GetColumnOfPixelsAtOffsetFromLeft ( int x ) 
    {
      ushort[] columnOfPixels = new ushort[Height] ;
      if ( x < Width ) 
      {
        for ( int y = 0 ; y < Height ; y++ )
        {
          columnOfPixels[y] = m_linearArray[
            x 
          + y * Width
          ] ;
        }
      }
      return columnOfPixels ;
    }

    // Mutate the instance, setting pixel values 'in place'

    public bool CanSetPixelInPlace ( 
      int  x, 
      int  y, 
      ushort pixelValue
    ) {
      int pixelOffset = (
        x
      + y * Width
      ) ;
      if ( 
         pixelOffset < 0 
      || pixelOffset > MaxPixelIndex
      ) {
        // The pixel we'd write is outside
        // the bounds of the pixel array ...
        return false ;
      }
      m_linearArray[pixelOffset] = pixelValue ;
      return true ;
    }

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public void SetPixelInPlace ( 
      int  x, 
      int  y, 
      ushort pixelValue
    ) {
      int pixelOffset = (
        x
      + y * Width
      ) ;
      m_linearArray[pixelOffset] = pixelValue ;
    }

    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining
    )]
    public void SetPixelInPlace_AtOffset ( 
      int  pixelOffset, 
      ushort pixelValue
    ) {
      m_linearArray[pixelOffset] = pixelValue ;
    }

    // Set all pixels, copying values from the source

    public void SetAllPixelsFromSource ( IEnumerable<ushort> source )
    {
      source.Count().Should().Be(m_linearArray.Length) ;
      System.Array.Copy(
        source.ToArray(),
        m_linearArray,
        m_linearArray.Length
      ) ;
    }

    public void SetAllPixelsInPlace ( System.Func<int,int,ushort> getPixelValueFunc ) 
    {
      for ( int i = 0 ; i < m_linearArray.Length ; i++ ) 
      {
        var (X,Y) = GetPixelCoordinatesFromOffset(i) ;
        m_linearArray[i] = getPixelValueFunc(
          X,
          Y
        ) ;
      }
    }

    public void SetAllPixelsInPlace ( ushort pixelValue ) 
    {
      for ( int i = 0 ; i < m_linearArray.Length ; i++ ) 
      {
        m_linearArray[i] = pixelValue ;
      }
    }

    public void SetAllPixelsInPlace ( IReadOnlyList<ushort> pixelValues ) 
    {
      for ( int i = 0 ; i < m_linearArray.Length ; i++ ) 
      {
        m_linearArray[i] = pixelValues[i] ;
      }
    }

    public void ApplyNormalisationInPlace ( ushort pixelValueToUseAsReference ) 
    {
      NormaliseToReferenceValue(
        m_linearArray,
        pixelValueToUseAsReference
      ) ;
    }

    public void ApplyTransformationInPlace ( System.Func<ushort,ushort> transformationFunc ) 
    {
      for ( int i = 0 ; i < m_linearArray.Length ; i++ ) 
      {
        m_linearArray[i] = transformationFunc(
          m_linearArray[i]
        ) ;
      }
    }

    // Create a clone ...

    public LinearArrayHoldingPixelIntensities CreateClone ( ) 
    => new LinearArrayHoldingPixelIntensities(
      Width,
      Height,
      Helpers.CreateCloneOfUShortArray(m_linearArray)
    ) ;

    public LinearArrayHoldingPixelIntensities CreateTransformedClone (
      System.Func<ushort,ushort> transformationFunc
    ) {
      ushort[] transformedValues = new ushort[m_linearArray.Length] ;
      for ( int i = 0 ; i < m_linearArray.Length ; i++ ) 
      {
        transformedValues[i] = transformationFunc(
          m_linearArray[i]
        ) ;
      }
      return new LinearArrayHoldingPixelIntensities(
        Width,
        Height,
        transformedValues
      ) ;
    }

    //
    // A 'normalised' image is one where the values have been multiplied
    // by a factor that makes a pixel whose original intensity was
    // the specified 'reference' value, have a 'maximum' intensity value ie 255.
    // Pixels whose value exceed that are also set to maximum intensity.
    //
    // If the reference value is not specified, we use the maximum value in the image.
    //

    public LinearArrayHoldingPixelIntensities CreateNormalisedClone ( ushort pixelValueToUseAsReference )
    {
      var clone = CreateClone() ;
      NormaliseToReferenceValue(
        clone.m_linearArray,
        pixelValueToUseAsReference
      ) ;
      return clone ;
    }

    private static void NormaliseToReferenceValue ( 
      ushort[] pixelArray, 
      ushort   pixelValueToUseAsReference
    ) {
      if ( pixelValueToUseAsReference == 0 )
      {
        // Edge case - all 'normalised' values are 255 ...
        for ( int i = 0 ; i < pixelArray.Length ; i++ )
        {
          pixelArray[i] = 255 ;
        }
      }
      else
      {
        double referenceValue = pixelValueToUseAsReference ;
        for ( int i = 0 ; i < pixelArray.Length ; i++ )
        {
          ushort pixelValue = pixelArray[i] ;
          double pixelValueAsFractionOfReference = pixelValue / referenceValue ;
          pixelArray[i] = (ushort) (
            pixelValueAsFractionOfReference >= 1.0
            ? 65535
            : pixelValueAsFractionOfReference * 65535
          ) ;
        }
      }
    }
    
    // Count the number of pixels at successive intensity values.

    public IReadOnlyList<int> BuildHistogram ( )
    {
      var histogram = new int[ushort.MaxValue+1] ;
      for ( int i = 0 ; i < PixelCount ; i++ )
      {
        ushort pixelValue = m_linearArray[i] ;
        histogram[pixelValue]++ ;
      }
      return histogram ;
    }

    public LinearArrayHoldingPixelIntensities CreateResizedClone_NearestNeighbour ( 
      int resizedWidthX, 
      int resizedHeightY
    ) {
      if ( 
         resizedWidthX  == Width
      && resizedHeightY == Height
      ) {
        return this.CreateClone() ;
      }
      else
      { 
        // The 'resized' dimensions are different from the sizes
        // of the PixelArray that we've already allocated,
        // so we'll allocate a new PixelArray of the required dimensions,
        // with all values initially set to zero.
        var resizedPixelArray = new LinearArrayHoldingPixelIntensities(
          resizedWidthX,
          resizedHeightY
        ) ;
        // Compute our 'resized' image using 'nearest-neighbour' interpolation.
        // If we're computing a reduced-size image, where the resized width
        // is say 0.5 times the original image width, then our 'xStepFactor'
        // will be 2.0 ; that is, each 'resized' pixel will come from an 
        // original pixel at a position of x-times-xStepFactor, ie we skip
        // over some of the original pixels.
        // If we're computing a larger image, where the resized width
        // is say 2 times the original image width, then our 'xStepFactor'
        // will be 0.5 ; that is, each 'resized' pixel will come from an 
        // original pixel at a position of x-times-xStepFactor, ie we'll
        // repeat some of the original pixels when creating the new image.
        // Another way of thinking about this is that the 'step factor'
        // represents the amount by which the size is being *reduced*.
        // For a step factor of 2, the size will be reduced by a factor of 2.
        double xStepFactor = ( (double) this.Width  ) / resizedWidthX ;
        double yStepFactor = ( (double) this.Height ) / resizedHeightY ;
        int iResized = 0 ;
        for ( int yResized = 0 ; yResized < resizedHeightY ; yResized++ )
        {
          // Step along each horizontal row of the resized image
          int originalIndexY = (int) ( 
            yResized * yStepFactor 
          ) ;
          for ( int xResized = 0 ; xResized < resizedWidthX ; xResized++ )
          {
            // Step along visiting each 'resized' pixel on the X axis
            int originalIndexX = (int) ( 
              xResized * xStepFactor 
            ) ;
            // Get the 'original' pixel value
            ushort originalPixel = this.m_linearArray[
              originalIndexX 
            + originalIndexY * this.Width
            ] ;
            // Write the 'resized' pixel value
            resizedPixelArray.m_linearArray[
              iResized++
            ] = originalPixel ;
          }
        }
        return resizedPixelArray ;
      }
    }

    public LinearArrayHoldingPixelIntensities CreateRotatedClone ( RotationFactor rotationFactor )
    {
      return (
        rotationFactor is RotationFactor.None
        ? CreateClone()
        : ComputeRotatedClone()
      ) ;
      LinearArrayHoldingPixelIntensities ComputeRotatedClone ( )
      {
        return new LinearArrayHoldingPixelIntensities(
          ComputeRotatedWidth(),
          ComputeRotatedHeight(),
          ComputeRotatedPixel
        ) ;
      }
      int ComputeRotatedWidth ( 
      ) => rotationFactor switch {
        // For these rotation factors, we swap the width and height ...
        RotationFactor.RotateClockwise90                                 => Height,
        RotationFactor.RotateClockwise90_ThenMirrorAroundVerticalCentre  => Height,
        RotationFactor.RotateClockwise270                                => Height,
        // Otherwise, the width and height stay the same ...
        _                                                                => Width
      } ;
      int ComputeRotatedHeight ( 
      ) => rotationFactor switch {
        // For these rotation factors, we swap the width and height ...
        RotationFactor.RotateClockwise90                                => Width,
        RotationFactor.RotateClockwise90_ThenMirrorAroundVerticalCentre => Width,
        RotationFactor.RotateClockwise270                               => Width,
        // Otherwise, the width and height stay the same ...
        _                                                               => Height
      } ;
      ushort ComputeRotatedPixel ( 
        int xRotated, // 0 .. Width-1
        int yRotated  // 0 .. Height-1
      ) => rotationFactor switch {
      RotationFactor.None => (
        //
        //  0 1 2   =>   0 1 2
        //  3 4 5        3 4 5
        //
        GetPixel(
          xRotated,
          yRotated
        )
      ),
      RotationFactor.RotateClockwise90 => (
        //
        //  0 1 2   =>   3 0
        //  3 4 5        4 1
        //               5 2
        //
        // Consider how we obtain the pixels for the new 'top' row, where y'=0.
        // As x' goes from 0 to Width'-1 (ie 0..1), we select the pixels
        // that used to reside in the left hand column, in reverse order.
        // That is, we access original pixels [0,1] then [0,0], ie values 3,0.
        //
        GetPixel(
          yRotated,          // 0
          Height-1-xRotated  // 0,1 => 1,0
        )
      ),
      RotationFactor.RotateClockwise90_ThenMirrorAroundVerticalCentre => (
        //
        //  0 1 2   =>   3 0   =>   0 3
        //  3 4 5        4 1        1 4
        //               5 2        2 5
        //
        // Consider how we obtain the pixels for the new 'top' row, where y'=0.
        // As x' goes from 0 to Width-1 (ie 0..1), we select the pixels
        // that used to reside in the left hand column, in original order.
        // That is, we access original pixels [0,0] then [0,1], ie values 0,3.
        //
        GetPixel(
          yRotated,
          xRotated
        )
      ),
      RotationFactor.RotateClockwise180 => (
        //
        //  0 1 2   =>   3 0    =>   5 4 3
        //  3 4 5        4 1         2 1 0
        //               5 2
        //
        // Consider how we obtain the pixels for the new 'top' row, where y'=0.
        // As x' goes from 0 to Width-1 (ie 0..1..2), we select the pixels
        // that used to reside in the 'bottom' row, in reverse order,
        // ie values 5,4,3.
        //
        GetPixel(
          Width-1-xRotated,
          Height-1-yRotated
        )
      ),
      RotationFactor.RotateClockwise270 => (
        //
        //  0 1 2   =>   3 0    =>   5 4 3  =>   2 5
        //  3 4 5        4 1         2 1 0       1 4
        //               5 2                     0 3
        //
        // Consider how we obtain the pixels for the new 'top' row, where y'=0.
        // As x' goes from 0 to Width-1 (ie 0..1), we select the pixels
        // that used to reside in the right hand column, in reverse order.
        // That is, we access original pixels [2,0] then [1,0], ie values 2,5.
        //
        GetPixel(
          Width-1-yRotated, 
          xRotated
        )
      ),
      RotationFactor.MirrorAroundVerticalCentre => (
        //
        //  0 1 2   =>   2 1 0
        //  3 4 5        5 4 3
        //                    
        // Consider how we obtain the pixels for the new 'top' row, where y'=0.
        // As x' goes from 0 to Width-1 (ie 0..1..2), we select the pixels
        // that used to reside in that 'top' row, in reverse order,
        // ie values 2,1,0.
        //
        GetPixel(
          Width-1-xRotated,
          yRotated
        )
      ),
      RotationFactor.MirrorAroundHorizontalCentre => (
        //
        //  0 1 2   =>   3 4 5
        //  3 4 5        0 1 2
        //                    
        // Consider how we obtain the pixels for the new 'top' row, where y'=0.
        // As x' goes from 0 to Width-1 (ie 0..1..2), we select the pixels
        // that used to reside in the 'bottom' row, in original order,
        // ie values 2,1,0.
        //
        GetPixel(
          xRotated,
          Height-1-yRotated
        )
      ),
      _ => throw new System.ApplicationException()
      } ;
    }

    // Selects a 'region-of-interest',
    // eg for zooming in onto a section of an image.
    // Might be useful !

    public LinearArrayHoldingPixelBytes CreateCloneOfRegion (
      int topLeftX,
      int topLeftY,
      int width,
      int height
    ) {
      var clone = new LinearArrayHoldingPixelBytes(width,height) ;
      int sourceOffset = this.GetOffsetOfPixelAt(
        topLeftX,
        topLeftY
      ) ;
      int destinationOffset = 0 ;
      for ( int y = 0 ; y < height ; y++ ) 
      {  
        System.Array.Copy(
          this.m_linearArray,
          sourceOffset,
          clone.m_linearArray,
          destinationOffset,
          width
        ) ;
        sourceOffset      += this.Width ;
        destinationOffset += clone.Width ;
      }
      return clone ;
    }

    // public void ApplyTransformation ( 
    //   PixelTransformationHelpers.PixelIntensityMapper intensityMapper,
    //   LinearArrayHoldingPixelBytes                    linearArrayHoldingPixelBytes
    // ) {
    //   for ( int iPixel = 0 ; iPixel < m_linearArray.Length ; iPixel++ ) 
    //   {
    //     linearArrayHoldingPixelBytes.m_linearArray[iPixel] = intensityMapper.GetMappedPixelValue(
    //       this.m_linearArray[iPixel]
    //     ) ;
    //   }
    // }

  }

}

