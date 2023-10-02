using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clf.Common.Arithmatic
{
  public static class Converters
  {
    public static string GetPrecisionText(object? value, int precision)
    {
      if (value is null)
        return "null";
      else if (value is System.Exception exception)
        return $"EXCEPTION : {exception.Message}";
      else if (value.GetType().IsArray == true && value.GetType().GetElementType() == typeof(byte))
      {
        var data = (value as byte[]).Where(x => x != 0).ToArray();
        return System.Text.Encoding.Default.GetString(data);
      }      
      else if (precision >= 0 && (value is double || value is float))
        return String.Format(CultureInfo.InvariantCulture, $"{{0:F{precision}}}", value);
      else
        return value?.ToString() ?? "null";
    }
    
     //If count is null then it will return actual number of bytes without padding/ truncating
     public static byte[] GetByteArrayFromString(string value, int? count=null)
     {
       var bytes = Encoding.ASCII.GetBytes(value);
       if (count != null)
         Array.Resize(ref bytes, count.Value);
      return bytes;
    }
    public static double GetPrecisionDoubleFromString(string value, int precision)
    {
      if (Double.TryParse(value, out var result))
      {
        return precision >= 0 ? Math.Round(result, precision) : result;
      }
      else
      {
        // TODO: Throw/Log Exception
        return double.NaN;
      }
    }

    public static double GetPrecisionDouble(double value, int precision)
    {
      return precision >= 0 ? Math.Round(value, precision) : value;
    }

    public static double GetDoubleFromObject(object? value, int precision = -1)
    {
      if (value is short || value is int || value is long || value is float || value is double)
      {
        return precision >= 0 ? Math.Round(Convert.ToDouble(value), precision) : Convert.ToDouble(value);
      }
      else if (value is string)
        return Double.TryParse((string)value, out var result) ? result : double.NaN;
      else
        return double.NaN;
    }
  }
}
