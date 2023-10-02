//
// SpanExperiments.cs
//

using FluentAssertions ;

namespace Clf.Common.ImageProcessing
{

  public class SpanExperiments
  {

    public static void Experiment_01 ( )
    {
      int[] myArray = new int[]{0,1,2,3} ;
      System.Span<int> mySpan = new System.Span<int>(myArray,1,2) ;
      mySpan[0] = 100 ;
      myArray[1].Should().Be(100) ;
    }

    public unsafe static void Experiment_02 ( )
    {
      byte[] myByteArray = new byte[]{0,1,2,3,4,5,6,7} ;
      fixed ( byte * p = myByteArray )
      {
        System.Span<int> mySpan = new System.Span<int>(p,2) ;
        mySpan[0] = 0x44332211 ;
      }
      myByteArray.Should().BeEquivalentTo(
        new byte[]{0x11,0x22,0x33,0x44,4,5,6,7}
      ) ;
    }

    public unsafe static void Experiment_03 ( )
    {
      byte[] myByteArray = new byte[]{9,9,9,9,0,1,2,3,4,5,6,7} ;
      fixed ( byte * p = &myByteArray[4] )
      {
        System.Span<int> mySpan = new System.Span<int>(p,2) ;
        mySpan[0] = 0x44332211 ;
      }
      myByteArray.Should().BeEquivalentTo(
        new byte[]{9,9,9,9,0x11,0x22,0x33,0x44,4,5,6,7}
      ) ;
    }

    public unsafe static void Experiment_04 ( )
    {
      byte[] myByteArray = new byte[]{9,9,9,9,0,1,2,3,4,5,6,7} ;
      fixed ( byte * p = &myByteArray[4] )
      {
        int * pInt = (int *) p ;
        *pInt = 0x44332211 ;
      }
      myByteArray.Should().BeEquivalentTo(
        new byte[]{9,9,9,9,0x11,0x22,0x33,0x44,4,5,6,7}
      ) ;
    }

    public unsafe static void Experiment_05 ( )
    {
      byte[] myByteArray = new byte[]{9,9,9,9,0,1,2,3,4,5,6,7} ;
      fixed ( byte * p = &myByteArray[4] )
      {
        // byte[] shadow = System.Array.CreateInstance(typeof(byte),3) ;
      }
      myByteArray.Should().BeEquivalentTo(
        new byte[]{9,9,9,9,0x11,0x22,0x33,0x44,4,5,6,7}
      ) ;
    }

  }

}

