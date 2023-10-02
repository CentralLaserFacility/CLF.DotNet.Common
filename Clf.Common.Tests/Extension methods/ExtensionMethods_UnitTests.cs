//
// ExtensionMethods_UnitTests.cs
//

using Xunit ;
using FluentAssertions ;
using static FluentAssertions.FluentActions ;

using Clf.Common.ExtensionMethods ;
using System.Linq ;

namespace Common_UnitTests
{

  public sealed class ExtensionMethods_UnitTests
  {

    // Need a lot more tests for the parsing methods !!

    [Fact]
    public void ParsedAs_WorksAsExpected ( )
    {

      "123".ParsedAs<int>().Should().Be(123) ;
      "0xff".ParsedAs<int>().Should().Be(255) ;
      "0b011".ParsedAs<int>().Should().Be(3) ;

      "123".ParsedAs<byte>().Should().Be(123) ;
      "0xff".ParsedAs<byte>().Should().Be(255) ;
      "0b011".ParsedAs<byte>().Should().Be(3) ;

      "true".ParsedAs<bool>().Should().Be(true) ;
      "false".ParsedAs<bool>().Should().Be(false) ;

      "t".ParsedAs<bool>().Should().Be(true) ;
      "f".ParsedAs<bool>().Should().Be(false) ;

      "1".ParsedAs<bool>().Should().Be(true) ;
      "0".ParsedAs<bool>().Should().Be(false) ;

      "123.0".ParsedAs<double>().Should().Be(123.0) ;

      "null".ParsedAs<int?>().Should().BeNull() ;
      "".ParsedAs<int?>().Should().BeNull() ;
      "null".ParsedAs<double?>().Should().BeNull() ;
      "".ParsedAs<double?>().Should().BeNull() ;

    }

    [Fact]
    public void ParsedAsNullable_WorksAsExpected ( )
    {

      "123".ParsedAsNullableValue<int>().Should().Be(123) ;

      "true".ParsedAsNullableValue<bool>().Should().Be(true) ;
      "false".ParsedAsNullableValue<bool>().Should().Be(false) ;

      "1".ParsedAsNullableValue<bool>().Should().Be(true) ;
      "0".ParsedAsNullableValue<bool>().Should().Be(false) ;

      // Null values

      "".ParsedAsNullableValue<int>().Should().Be(null) ;
      "null".ParsedAsNullableValue<int>().Should().Be(null) ;

      "".ParsedAsNullableValue<bool>().Should().Be(null) ;
      "null".ParsedAsNullableValue<bool>().Should().Be(null) ;

    }

    [Fact]
    public void ParsedAsNullableEx_WorksAsExpected ( )
    {

      "123".ParsedAs<int?>().Should().Be(123) ;

      "true".ParsedAs<bool?>().Should().Be(true) ;
      "false".ParsedAs<bool?>().Should().Be(false) ;

      "1".ParsedAs<bool?>().Should().Be(true) ;
      "0".ParsedAs<bool?>().Should().Be(false) ;

      // Null values

      "null".ParsedAs<int?>().Should().Be(null) ;

      "null".ParsedAs<bool?>().Should().Be(null) ;
      "null".ParsedAs<bool?>().Should().Be(null) ;

    }

    [Fact]
    public void ValueIs_WorksAsexpected ( )
    {
      123.ValueIs(123).Should().BeTrue() ;
      0.ValueIs(123).Should().BeFalse() ;
    }

  }

}
