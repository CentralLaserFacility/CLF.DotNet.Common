using Clf.Common.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Clf.Common.ExtensionMethods
{
  public static class EnumExtensions
  {
    public static string GetDisplayName(this Enum enumValue)
    {
      string? displayName;
      DisplayAttribute? displayAttribute = enumValue.GetType()
                      .GetMember(enumValue.ToString())
                      .First()
                      .GetCustomAttribute<DisplayAttribute>();
                
     if (displayAttribute == null)
        return string.Empty;
      else
         displayName = displayAttribute.GetName();
      displayName ??= string.Empty;
      
      return displayName;
    }
  }
}
