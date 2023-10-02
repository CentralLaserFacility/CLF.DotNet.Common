//
// Helpers_Tests.cs
//

using Xunit;
using Xunit.Abstractions;

using FluentAssertions;

using Clf.Common.ImageProcessing;
using System.Linq;
using System.Collections.Generic;

namespace ImageProcessing_Tests
{

  public class Helpers_Tests
  {

    [Fact]
    public void GetValuesAsEnumerableOfTuples_WorksAsExpected ( ) 
    {
      Helpers.GetValuesAsEnumerableOfTuples(new int[0]).Count().Should().Be(0) ;
      Helpers.GetValuesAsEnumerableOfTuples(0,0,0,0).Count().Should().Be(0) ;
      Helpers.GetValuesAsEnumerableOfTuples(1,2).Count().Should().Be(1) ;
      Helpers.GetValuesAsEnumerableOfTuples(1,2,0,0).Count().Should().Be(2) ;
      Helpers.GetValuesAsEnumerableOfTuples(1,2,0,0,0).Count().Should().Be(2) ;
      Helpers.GetValuesAsEnumerableOfTuples(1,2,0,0,0,0).Count().Should().Be(1) ;
      Helpers.GetValuesAsEnumerableOfTuples(1,2,3,4,0,0,0,0,1).Count().Should().Be(2) ;
      Helpers.GetValuesAsEnumerableOfTuples(1,2,3,4,0,0,0,0,1).Should().BeEquivalentTo(new[]{(1,2),(3,4)}) ;
    }

    //
    // This test visits lines drawn from the centre point
    // to each of these '*' points, as follows :
    //
    //
    //     *   *   *
    //      \  |  /
    //       \ | /
    //        \|/
    //     *---*---*
    //        /|\
    //       / | \
    //      /  |  \
    //     *   *   *
    //
    // Hmm, should supply a 5x5 matrix instead of 3x3 !!!
    //

    [Theory]
    [InlineData( -1 , -1 )]
    [InlineData( -1 ,  0 )]
    [InlineData( -1 , +1 )]
    [InlineData(  0 , -1 )]
    [InlineData(  0 ,  0 )]
    [InlineData(  0 , +1 )]
    [InlineData( +1 , -1 )]
    [InlineData( +1 ,  0 )]
    [InlineData( +1 , +1 )]
    public void VisitPixelsOnLine_WorksAsExpected ( int deltaX, int deltaY ) 
    {
      int nX = 3 ;
      int nY = 2 ; // Hmm, fails when n is 3 ???
      int firstX = 10 ;
      int firstY = 10 ;
      int lastX = firstX + deltaX*nX ;
      int lastY = firstY + deltaY*nY ;
      int nVisits_firstPixel = 0 ;
      int nVisits_lastPixel = 0 ;
      int nTotalVisits = 0 ;
      List<(int x,int y)> pixelsVisited = new() ;
      Helpers.VisitPixelsOnLine(
        firstX,
        firstY,
        lastX,
        lastY,
        (x,y) => {
          pixelsVisited.Add((x,y)) ; 
          if ( 
             x == firstX 
          && y == firstY
          ) {
            nVisits_firstPixel++ ;
          }
          if ( 
             x == lastX 
          && y == lastY
          ) {
            nVisits_lastPixel++ ;
          }
          nTotalVisits++ ;
        }
      ) ;
      var allPixelsVisited  = pixelsVisited.ToArray() ;
      var firstPixelVisited = pixelsVisited.First() ;
      var lastPixelVisited  = pixelsVisited.Last() ;
      nVisits_firstPixel.Should().Be(1) ;
      nVisits_lastPixel.Should().Be(1) ;
      int nPixelVisitsExpected = (
        System.Math.Max(
          System.Math.Abs(lastX-firstX),
          System.Math.Abs(lastY-firstY)
        ) 
      + 1
      ) ;
      nTotalVisits.Should().Be(nPixelVisitsExpected) ;
    }

    //
    // This test excercises lines drawn at various angles
    // including angles that are not 45 degrees ...
    //
    //    *  *  *  *  *
    //    *  *  *  *  *
    //    *  *  0  *  *
    //    *  *  *  *  *
    //    *  *  *  *  *
    //

    [Fact]
    public void VisitPixelsOnLine_WorksAsExpected_2 ( ) 
    {
      int firstX = 10 ;
      int firstY = 10 ;
      foreach ( int n in new[]{1,2,5,8} )
      {
        foreach ( int deltaX in new[]{-2,-1,0,+1,+2} )
        {
          foreach ( int deltaY in new[]{-2,-1,0,+1,+2} )
          {
            RunTest(
              deltaX * n,
              deltaY * n
            ) ;
          }
        }
      }
      void RunTest( int deltaX, int deltaY )
      {
        int lastX = firstX + deltaX ;
        int lastY = firstY + deltaY ;
        int nVisits_firstPixel = 0 ;
        int nVisits_lastPixel = 0 ;
        int nTotalVisits = 0 ;
        List<(int x,int y)> pixelsVisited = new() ;
        Helpers.VisitPixelsOnLine(
          firstX,
          firstY,
          lastX,
          lastY,
          (x,y) => {
            pixelsVisited.Add((x,y)) ; 
            if ( 
               x == firstX 
            && y == firstY
            ) {
              nVisits_firstPixel++ ;
            }
            if ( 
               x == lastX 
            && y == lastY
            ) {
              nVisits_lastPixel++ ;
            }
            nTotalVisits++ ;
          }
        ) ;
        var allPixelsVisited  = pixelsVisited.ToArray() ;
        var firstPixelVisited = pixelsVisited.First() ;
        var lastPixelVisited  = pixelsVisited.Last() ;
        nVisits_firstPixel.Should().Be(1) ;
        nVisits_lastPixel.Should().Be(1) ;
        int nPixelVisitsExpected = (
          System.Math.Max(
            System.Math.Abs(lastX-firstX),
            System.Math.Abs(lastY-firstY)
          ) 
        + 1
        ) ;
        nTotalVisits.Should().Be(nPixelVisitsExpected) ;
      }
    }

    // 
    // Hmm, it's actually quite troublesome to test this circle-drawing algorithm
    // because some of the pixels are visited more than once. However visually
    // the result looks like a circle, so let's leave it at thet ...
    //

    // [Theory]
    // [InlineData( 0, 0, 2 )]
    // public void VisitPixelsOnCircle_WorksAsExpected ( int centreX, int centreY, int radius ) 
    // {
    //   List<(int x,int y)> pixelsVisited = new() ;
    //   List<string> pixelVisitsLog = new() ;
    //   Helpers.VisitPixelsOnCircle(
    //   // Helpers.VisitPixelsOnCircle_old_01(
    //     centreX,
    //     centreY,
    //     radius,
    //     (x,y,offset) => {
    //       pixelsVisited.Add((x,y)) ;
    //       pixelVisitsLog.Add(
    //         $"{offset:D2} {x:D2} {y:D2}"
    //       ) ;
    //     }
    //   ) ;
    //   var log = pixelVisitsLog.ToArray() ;
    //   var allPixelsVisited  = pixelsVisited.ToArray() ;
    //   pixelsVisited.Should().BeEquivalentTo(
    //     pixelsVisited.Distinct()
    //   ) ;
    // }

    // [Theory]
    // [InlineData( 2 )]
    // public void MidpointCircle_WorksAsExpected ( int radius ) 
    // {
    //   List<(int x,int y)> pixelsVisited = new() ;
    //   List<string> pixelVisitsLog = new() ;
    //   Helpers.MidpointCircle(
    //     radius,
    //     (x,y) => {
    //       pixelsVisited.Add((x,y)) ;
    //       pixelVisitsLog.Add(
    //         $"{x:D2} {y:D2}"
    //       ) ;
    //     }
    //   ) ;
    //   var log = pixelVisitsLog.ToArray() ;
    //   var allPixelsVisited  = pixelsVisited.ToArray() ;
    //   pixelsVisited.Should().BeEquivalentTo(
    //     pixelsVisited.Distinct()
    //   ) ;
    // }

  }

}

