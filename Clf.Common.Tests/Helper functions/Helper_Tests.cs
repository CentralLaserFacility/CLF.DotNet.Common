//
// Helper_Tests.cs
//

using Xunit ;
using FluentAssertions ;

namespace Common_UnitTests
{

  public class Helper_Tests
  {

    [Fact]
    public void CreateArrayOfObjects_WorksAsExpected ( )
    {
      // Hmm, it's be nice to be able to see this ...
      System.Console.WriteLine("Written by System.Console.WriteLine") ;
      System.Diagnostics.Debug.WriteLine("Written by System.Diagnostics.Debug.WriteLine") ;
      {
        int[] int_array = Clf.Common.Helpers.CreateArrayOfObjects(4,123) ;
        int_array.Should().BeEquivalentTo(new int[]{123,123,123,123}) ;
      }
      {
        double[] array = Clf.Common.Helpers.CreateArrayOfObjects(4,123.0) ;
        array.Should().BeEquivalentTo(new double[]{123.0,123.0,123.0,123.0}) ;
      }
      {
        string[] array = Clf.Common.Helpers.CreateArrayOfObjects(4,"hello") ;
        array.Should().BeEquivalentTo(new string[]{"hello","hello","hello","hello"}) ;
      }
      {
        // The type of the array can be inferred
        var array = Clf.Common.Helpers.CreateArrayOfObjects(4,"hello") ;
        array.Should().BeEquivalentTo(new string[]{"hello","hello","hello","hello"}) ;
      }
      {
        // The type of the array can be specified
        var array = Clf.Common.Helpers.CreateArrayOfObjects<string?>(4,null) ;
        array.Should().BeEquivalentTo(new string?[]{null,null,null,null}) ;
      }
      {
        // The type of the array can be specified
        var array = Clf.Common.Helpers.CreateArrayOfObjects<int?>(4,null) ;
        array.Should().BeEquivalentTo(new int?[]{null,null,null,null}) ;
      }
    }
    
  }

}
