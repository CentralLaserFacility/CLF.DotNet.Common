using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clf.Common.Drawing
{
  public record Position(int X, int Y)
  {
    public static readonly Position Origin = new Position(0,0);
  }
}
