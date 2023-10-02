//
// PixelManipulation.cs
//

using System.Collections.Generic ;
using System.Drawing ;

namespace Clf.Common.ImageProcessing
{

  // Experiments ...

  public static class PixelManipulation
  {

    // See also
    // http://ericw.ca/notes/bresenhams-line-algorithm-in-csharp.html
    // http://www.roguebasin.com/index.php/Bresenham%27s_Line_Algorithm

    public static void VisitPixelsOnLine (
      int                    x1,
      int                    y1,
      int                    x2,
      int                    y2,
      System.Action<int,int> action
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
            action(x1,y1) ;
            if ( x1 == x2 )
              break ;
            x1++ ;
            e += 2 * dy ;
            if ( e > dx )
            {
              e -= 2 * dx ;
              y1++ ;
            }
          }
        }
        else
        {
          for(;;)
          {
            action(x1,y1) ;
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
            action(x1,y1) ;
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
            action(x1,y1) ;
            if ( y1 == y2 )
              break ;
            y1-- ;
            e += dx * 2 ;
            if ( e > dy )
            {
              e -= 2 * dy ;
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
        int x = a ;
        a = b ;
        b = x ;
      }
    }

    //
    // https://www.codeproject.com/Articles/30686/Bresenham-s-Line-Algorithm-Revisited
    // https://en.wikipedia.org/wiki/Midpoint_circle_algorithm
    // https://www.javatpoint.com/computer-graphics-bresenhams-circle-algorithm
    //

    public static IEnumerable<Point> VisitPointsOnCircle ( Point midpoint, int radius )
    {
      Point returnPoint = new Point() ;

      int f = 1 - radius ;
      int ddF_x = 1 ;
      int ddF_y = -2 * radius ;
      int x = 0 ;
      int y = radius ;

      returnPoint.X = midpoint.X ;
      returnPoint.Y = midpoint.Y + radius ;
      yield return returnPoint ;
      returnPoint.Y = midpoint.Y - radius ;
      yield return returnPoint ;
      returnPoint.X = midpoint.X + radius ;
      returnPoint.Y = midpoint.Y ;
      yield return returnPoint ;
      returnPoint.X = midpoint.X - radius ;
      yield return returnPoint ;

      while ( x < y )
      {
        if ( f >= 0 )
        {
          y-- ;
          ddF_y += 2 ;
          f += ddF_y ;
        }
        x++ ;
        ddF_x += 2 ;
        f += ddF_x ;
        returnPoint.X = midpoint.X + x ;
        returnPoint.Y = midpoint.Y + y ;
        yield return returnPoint ;
        returnPoint.X = midpoint.X - x ;
        returnPoint.Y = midpoint.Y + y ;
        yield return returnPoint ;
        returnPoint.X = midpoint.X + x ;
        returnPoint.Y = midpoint.Y - y ;
        yield return returnPoint ;
        returnPoint.X = midpoint.X - x ;
        returnPoint.Y = midpoint.Y - y ;
        yield return returnPoint ;
        returnPoint.X = midpoint.X + y ;
        returnPoint.Y = midpoint.Y + x ;
        yield return returnPoint ;
        returnPoint.X = midpoint.X - y ;
        returnPoint.Y = midpoint.Y + x ;
        yield return returnPoint ;
        returnPoint.X = midpoint.X + y ;
        returnPoint.Y = midpoint.Y - x ;
        yield return returnPoint ;
        returnPoint.X = midpoint.X - y ;
        returnPoint.Y = midpoint.Y - x ;
        yield return returnPoint ;
      }
    }

    // https://www.javatpoint.com/computer-graphics-bresenhams-circle-algorithm
    // https://www.geeksforgeeks.org/bresenhams-circle-drawing-algorithm/
    // https://en.wikipedia.org/wiki/Midpoint_circle_algorithm
    // https://banu.com/blog/7/drawing-circles/

    static void VisitPointsOnCircle ( int xc, int yc, int r, System.Action<int,int,int> putpixel )  
    {  
      int x = 0 ;
      int y = r ;
      int d = 3 - ( 2 * r ) ;  
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
        putpixel (  x + xc,  y + yc, 1 ) ;  
        putpixel (  x + xc, -y + yc, 2 ) ;  
        putpixel ( -x + xc, -y + yc, 3 ) ;  
        putpixel ( -x + xc,  y + yc, 4 ) ;  
        putpixel (  y + xc,  x + yc, 5 ) ;  
        putpixel (  y + xc, -x + yc, 6 ) ;  
        putpixel ( -y + xc, -x + yc, 7 ) ;  
        putpixel ( -y + xc,  x + yc, 8 ) ;  
      }     
    }  
  
  }

}

