﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clf.Common.Helper_functions
{
  public static class UrlUtilities
  {
    public static string Separator { get; set; } = "/";

    public static string GetUrlString(params object[] ids)
    {
      var sb = new StringBuilder();
      for (int i = 0; i < ids.Length; i++)
      {
        var id = ids[i];
        var type = id.GetType();
        var stringValue = "";
        if (type.IsEnum)
          stringValue = Enum.GetName(type, id);
        else
          stringValue = id.ToString();

        if (stringValue.StartsWith(Separator))
          sb.Append(stringValue);
        else
          sb.Append(Separator).Append(stringValue);
      }
      return sb.ToString();
    }
  }
}
