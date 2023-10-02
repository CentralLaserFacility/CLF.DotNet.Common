//
// LinearArrayHoldingPixelBytes.cs
//

using System.Collections.Generic ;
using System.Linq ;
using FluentAssertions ;
using Xunit ;
using Xunit.Abstractions ;

using Clf.Common.ImageProcessing ;

namespace ImageProcessing_Tests
{

  public class LinearArrayHoldingPixelBytes_Tests
  {

    [Fact]
    public void TestsSucceed ( ) 
    {

      LinearArrayHoldingPixelBytes original = new LinearArrayHoldingPixelBytes(3,2,new byte[]{0,1,2,3,4,5}) ;
      string a  = $"{original[0,0]} {original[1,0]} {original[2,0]}" ;
      string b  = $"{original[0,1]} {original[1,1]} {original[2,1]}" ;
      original.LinearArrayValues.Should().BeEquivalentTo(new byte[]{0,1,2,3,4,5}) ;

      {
        original.GetRowOfPixelsAtOffsetFromTop(1).Should().BeEquivalentTo(new byte[]{3,4,5}) ;
        original.GetColumnOfPixelsAtOffsetFromLeft(1).Should().BeEquivalentTo(new byte[]{1,4}) ;
      }

      {
        for ( int i = 0 ; i < original.PixelCount ; i++ )
        {
          var xy = original.GetPixelCoordinatesFromOffset(i) ;
          int offset = original.GetOffsetOfPixelAt(xy.X,xy.Y) ;
          offset.Should().Be(i) ;
        }
      }

      {
        var rotated = original.CreateRotatedClone(RotationFactor.None) ;
        // rotated.InstanceNumber.Should().Be(original.InstanceNumber+1) ;
        string aa = $"{rotated[0,0]} {rotated[1,0]} {rotated[2,0]} ; expected 0 1 2" ;
        string bb = $"{rotated[0,1]} {rotated[1,1]} {rotated[2,1]} ; expected 3 4 5" ;
        rotated.LinearArrayValues.Should().BeEquivalentTo(new byte[]{0,1,2,3,4,5}) ;
      }

      {
        var rotated = original.CreateRotatedClone(RotationFactor.RotateClockwise90) ;
        // rotated.InstanceNumber.Should().Be(original.InstanceNumber+2) ;
        string aa = $"{rotated[0,0]} {rotated[1,0]} ; expected 3 0" ;
        string bb = $"{rotated[0,1]} {rotated[1,1]} ; expected 4 1" ;
        string cc = $"{rotated[0,2]} {rotated[1,2]} ; expected 5 2" ;
        rotated.LinearArrayValues.Should().BeEquivalentTo(new byte[]{3,0,4,1,5,2}) ;
      }

      {
        var rotated = original.CreateRotatedClone(RotationFactor.RotateClockwise180) ;
        string aa = $"{rotated[0,0]} {rotated[1,0]} {rotated[2,0]} ; expected 5 4 3" ;
        string bb = $"{rotated[0,1]} {rotated[1,1]} {rotated[2,1]} ; expected 2 1 0" ;
        rotated.LinearArrayValues.Should().BeEquivalentTo(new byte[]{5,4,3,2,1,0}) ;
      }

      {
        var rotated = original.CreateRotatedClone(RotationFactor.RotateClockwise270) ;
        string aa = $"{rotated[0,0]} {rotated[1,0]} ; expected 2 5" ;
        string bb = $"{rotated[0,1]} {rotated[1,1]} ; expected 1 4" ;
        string cc = $"{rotated[0,2]} {rotated[1,2]} ; expected 0 3" ;
        rotated.LinearArrayValues.Should().BeEquivalentTo(new byte[]{2,5,1,4,0,3}) ;
      }

      {
        var mirrored = original.CreateRotatedClone(RotationFactor.MirrorAroundVerticalCentre) ;
        string aa = $"{mirrored[0,0]} {mirrored[1,0]} {mirrored[2,0]} ; expected 2 1 0" ;
        string bb = $"{mirrored[0,1]} {mirrored[1,1]} {mirrored[2,1]} ; expected 5 4 3" ;
        mirrored.LinearArrayValues.Should().BeEquivalentTo(new byte[]{2,1,0,5,4,3}) ;
      }

      {
        var mirrored = original.CreateRotatedClone(RotationFactor.MirrorAroundHorizontalCentre) ;
        string aa = $"{mirrored[0,0]} {mirrored[1,0]} {mirrored[2,0]} ; expected 3 4 5" ;
        string bb = $"{mirrored[0,1]} {mirrored[1,1]} {mirrored[2,1]} ; expected 0 1 2" ;
        mirrored.LinearArrayValues.Should().BeEquivalentTo(new byte[]{3,4,5,0,1,2}) ;
      }

      {
        var rotatedThenMirrored = original.CreateRotatedClone(RotationFactor.RotateClockwise90_ThenMirrorAroundVerticalCentre) ;
        string aa = $"{rotatedThenMirrored[0,0]} {rotatedThenMirrored[1,0]} ; expected 0 3" ;
        string bb = $"{rotatedThenMirrored[0,1]} {rotatedThenMirrored[1,1]} ; expected 1 4" ;
        string cc = $"{rotatedThenMirrored[0,2]} {rotatedThenMirrored[1,2]} ; expected 2 5" ;
        rotatedThenMirrored.LinearArrayValues.Should().BeEquivalentTo(new byte[]{0,3,1,4,2,5}) ;
      }

      {
        original.CreateRotatedClone(
          RotationFactor.RotateClockwise90
        ).CreateRotatedClone(
          RotationFactor.RotateClockwise90
        ).LinearArrayValues.Should().BeEquivalentTo(
            original.CreateRotatedClone(
            RotationFactor.RotateClockwise180
          ).LinearArrayValues 
        ) ;
      }

      {
        var expanded = original.CreateResizedClone_NearestNeighbour(
          original.Width  * 2,
          original.Height * 2
        ) ;
        expanded.LinearArrayValues.Should().BeEquivalentTo(
          new byte[]{
            0,0,1,1,2,2,
            0,0,1,1,2,2,
            3,3,4,4,5,5,
            3,3,4,4,5,5
          }
        ) ;
        var restored = expanded.CreateResizedClone_NearestNeighbour(
          original.Width,
          original.Height
        ) ;
        restored.LinearArrayValues.Should().BeEquivalentTo(original.LinearArrayValues) ;
      }

      {
        var expanded = original.CreateResizedClone_NearestNeighbour(
          original.Width  * 3,
          original.Height * 3
        ) ;
        expanded.LinearArrayValues.Should().BeEquivalentTo(
          new byte[]{
            0,0,0,1,1,1,2,2,2,
            0,0,0,1,1,1,2,2,2,
            0,0,0,1,1,1,2,2,2,
            3,3,3,4,4,4,5,5,5,
            3,3,3,4,4,4,5,5,5,
            3,3,3,4,4,4,5,5,5
          }
        ) ;
        var restored = expanded.CreateResizedClone_NearestNeighbour(
          original.Width,
          original.Height
        ) ;
        restored.LinearArrayValues.Should().BeEquivalentTo(original.LinearArrayValues) ;
      }

      {
        var expanded = original.CreateResizedClone_NearestNeighbour(
          original.Width  * 3,
          original.Height * 2
        ) ;
        expanded.LinearArrayValues.Should().BeEquivalentTo(
          new byte[]{
            0,0,0,1,1,1,2,2,2,
            0,0,0,1,1,1,2,2,2,
            3,3,3,4,4,4,5,5,5,
            3,3,3,4,4,4,5,5,5
          }
        ) ;
        var restored = expanded.CreateResizedClone_NearestNeighbour(
          original.Width,
          original.Height
        ) ;
        restored.LinearArrayValues.Should().BeEquivalentTo(original.LinearArrayValues) ;
      }

      {
        var testImage = new LinearArrayHoldingPixelBytes(3,3,new byte[]{0,0,0,0,100,0,0,0,0}) ;
        byte referenceValue = testImage.MaxValue ;
        var clone = testImage.CreateNormalisedClone(referenceValue) ;
        testImage.ApplyNormalisationInPlace(referenceValue) ;
        testImage.LinearArrayValues.Should().BeEquivalentTo(new byte[]{0,0,0,0,255,0,0,0,0}) ;
        clone.LinearArrayValues.Should().BeEquivalentTo(new byte[]{0,0,0,0,255,0,0,0,0}) ;
      }

      {
        var testImage = new LinearArrayHoldingPixelBytes(
          width  : 5,
          height : 6,
          new byte[]{
            0,0,0,0,0,
            0,0,0,0,0,
            0,0,0,0,0,
            0,1,2,3,0,
            0,4,5,6,0,
            0,0,0,0,0
          }
        ) ;
        var region = testImage.CreateCloneOfRegion(
          topLeftX : 1, 
          topLeftY : 3, 
          width    : 3, 
          height   : 2
        ) ;
        region.LinearArrayValues.Should().BeEquivalentTo(new byte[]{1,2,3,4,5,6}) ;
      }

    }
  }

}

