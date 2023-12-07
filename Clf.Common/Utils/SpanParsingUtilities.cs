//
// SpanParsingUtilities.cs
//

using System.Collections.Generic;

namespace Clf.Common.Utils
{


  public static class SpanParsingUtilities
  {

    //
    // Hmm, because 'Span' is a very special type, you can't
    // have arrays of Span<T> and you can't use Span<T> as a type parameter.
    //
    //

    public record struct SpanDescriptor ( int StartOffset, int Length )
    {
      public System.ReadOnlySpan<T> ApplyTo<T> ( System.ReadOnlySpan<T> span )
      {
        return span.Slice(
          StartOffset,
          Length
        ) ;
      }
    }

    public static IReadOnlyList<SpanDescriptor> GetDistinctLineSpanDescriptors ( 
      this System.ReadOnlySpan<char> span_hostingTextLines 
    ) {
      //
      // Suppose we have 7 characters in a Span :
      //
      //     0    1    2    3    4    5    6
      //   +----+----+----+----+----+----+----+
      //   |  a |  b | \r | \n |  c |  d |  e |
      //   +----+----+----+----+----+----+----+
      //     |                   |
      //     0,2                 4,3
      //     
      // We want to find the specifications of Spans that
      // represent individual lines, ie that end with '\r\n'.
      //
      // When we find the '\r' at #2, we'll return a span
      // that starts at 0 and has a length of 2.
      // Then we bump 'iStart' to the position just past
      // the '\n', ie to index 4.
      //
      // If the sequence had ended with a '\r\n', we would have
      // seen the '\r' and our 'iStart' would have been
      // set to '9'. In that case, iStart would have matched
      // the number of elements, and it would not be necessary 
      // to add a further span_hostingTextLines.
      //
      //     0    1    2    3    4    5    6    7    8
      //   +----+----+----+----+----+----+----+----+----+
      //   |  a |  b | \r | \n |  c |  d |  e | \r | \n |
      //   +----+----+----+----+----+----+----+----+----+
      //     |                   |
      //     0,2                 4,3
      //     
      // In the original case shown above, when we fall out of the loop
      // our 'iStart' is NOT equal to the sequence length, so it's
      // necessary to return an additional span_hostingTextLines that represents
      // the three trailing elements.
      //

      int iStart = 0 ;
      var spans = new List<SpanDescriptor>() ;
      for ( int i = 0 ; i < span_hostingTextLines.Length ; i++ ) 
      {
        if ( span_hostingTextLines[i] == '\r' ) 
        {
          // We've found a delimiter.
          int spanLength = i - iStart ;
          spans.Add(
            new SpanDescriptor(
              iStart,
              spanLength
            )
          ) ;
          iStart = i + 2 ;
        }
      }
      if ( iStart == span_hostingTextLines.Length ) 
      {
        // No trailing elements, because the
        // sequence ended with '\r\n'
      }
      else
      {
        int trailingSpanLength = span_hostingTextLines.Length - iStart ;
        spans.Add(
          new SpanDescriptor(
            iStart,
            trailingSpanLength
          )
        ) ;
      }
      return spans ;
    }

    public static IReadOnlyList<SpanDescriptor> GetSpanDescriptorsForSegmentedLine ( 
      this System.ReadOnlySpan<char> span, 
      char                           delimiter,
      int                            offset = 0
    ) {
      //
      // Suppose we have 6 characters in a Span :
      //
      //     0   1   2   3   4   5 
      //   +---+---+---+---+---+---+
      //   | a | b | , | c | d | e |
      //   +---+---+---+---+---+---+
      //     |           |
      //     0,2         3,3
      //     
      // We want to find the specifications of Spans that
      // represent individual segments delimited by ','.
      //
      // When we find the ',' at #2, we'll return
      // a span that starts at 0 and has a length of 2.
      // Then we bump 'iStart' to the position just past
      // the delimiter, ie to index 3.
      //
      // If the sequence had ended with a ',', we would have
      // seen the final ',' and our 'iStart' would have been
      // set to '7'. In that case, iStart would have matched
      // the number of elements, and it would not be necessary 
      // to add a further span.
      //
      //     0   1   2   3   4   5   6
      //   +---+---+---+---+---+---+---+
      //   | a | b | , | c | d | e | , |
      //   +---+---+---+---+---+---+---+
      //     |           |
      //     0,2         3,3
      //     
      // In the original case shown above, when we fall out of the loop
      // our 'iStart' is NOT equal to the sequence length, so it's
      // necessary to return an additional span that represents
      // the three trailing elements.
      //

      int iStart = 0 ;
      var spans = new List<SpanDescriptor>() ;
      for ( int i = 0 ; i < span.Length ; i++ ) 
      {
        if ( span[i] == delimiter ) 
        {
          // We've found a delimiter.
          int spanLength = i - iStart ;
          if ( spanLength != 0 )
          {
            spans.Add(
              new SpanDescriptor(
                iStart + offset,
                spanLength
              )
            ) ;
          }
          iStart = i + 1 ;
        }
      }
      if ( iStart == span.Length ) 
      {
        // No trailing elements, because the
        // sequence ended with ','
      }
      else
      {
        int trailingSpanLength = span.Length - iStart ;
        if ( trailingSpanLength != 0 )
        {
          spans.Add(
            new SpanDescriptor(
              iStart + offset,
              trailingSpanLength
            )
          ) ;
        }
      }
      return spans ;
    }

  }

}

