//
// ExtensionMethods.cs
//

using System.Collections.Generic ;
using System.Diagnostics.CodeAnalysis ;
using System.Linq ;

namespace Clf.Common.ExtensionMethods
{

  public static class IEnumerable_ExtensionMethods
  {

    public static bool IsEmpty<T>(this IEnumerable<T>? sequence)
    => sequence?.Any() == false;

    public static void ForEachItem<T>(this IEnumerable<T> sequence, System.Action<T> action)
    {
      foreach (var item in sequence)
      {
        action(item);
      }
    }

    public static void ForEachItem<T>(this IEnumerable<T> sequence, System.Action<T, int> action)
    {
      int j = 0;
      foreach (var item in sequence)
      {
        action(item, j++);
      }
    }

    public static IEnumerable<T> WhereNonNull<T>(this IEnumerable<T?>? items) where T : class
    => (
      items == null
      ? Enumerable.Empty<T>()
      : items.Where(
          item => item != null
        ).Select(
          item => item!
        )
    );

    public static IEnumerable<T> WithAdditionalItems<T>(this T item, IEnumerable<T> others)
    => (
      new[]{
        item
      }.Concat(
        others
      )
    );

    public static IEnumerable<TCollectionItem> WithOptionalFilterApplied<TCollectionItem>(
      this IEnumerable<TCollectionItem> collection, System.Func<TCollectionItem, bool>? filter) => (filter is null ? collection : collection.Where(filter)
     );

    public static string AsSingleLine(this IEnumerable<string> strings, string separator = ",") => (string.Join(separator,strings));

    public static TItem GetSingleMatchingItem<TItem>(
      this IEnumerable<TItem> sequence,
      System.Func<TItem, bool> filter
    )
    where TItem : class
    {
      TItem? firstMatchingItemFound = default;
      List<TItem>? additionalMatchingItems = default;
      foreach (var element in sequence)
      {
        if (filter(element))
        {
          if (firstMatchingItemFound == default)
          {
            firstMatchingItemFound = element;
          }
          else
          {
            // We've found an additional item !
            // additionalMatchingItems = additionalMatchingItems ?? new List<TCollectionItem>() ;
            // Can use a COMPOUND ASSIGNMENT !!!
            additionalMatchingItems ??= new List<TItem>();
            additionalMatchingItems.Add(element);
          }
        }
      }
      if (additionalMatchingItems != default)
      {
        // We found too many items !
        additionalMatchingItems.Add(firstMatchingItemFound!);
        var allItems_toShowInDebugger = sequence.ToArray();
        // System.Diagnostics.Debugger.Break() ;
        throw new System.ApplicationException("Expected single item ; more than one item found");
      }
      else if (firstMatchingItemFound == default)
      {
        // We found no item !
        var allItems_toShowInDebugger = sequence.ToArray();
        // System.Diagnostics.Debugger.Break() ;
        throw new System.ApplicationException("Expected single item ; zero items found");
      }
      else
      {
        return firstMatchingItemFound;
      }
    }

    public static TCollectionItem? GetSingleMatchingItemOrDefault<TCollectionItem>(
      this IEnumerable<TCollectionItem> collection,
      System.Func<TCollectionItem, bool> filter
    )
    where TCollectionItem : class
    {
      TCollectionItem? firstMatchingItemFound = default;
      List<TCollectionItem>? additionalMatchingItems = default;
      foreach (var element in collection)
      {
        if (filter(element))
        {
          if (firstMatchingItemFound == default)
          {
            firstMatchingItemFound = element;
          }
          else
          {
            // We've found an additional item !
            additionalMatchingItems ??= new List<TCollectionItem>();
            additionalMatchingItems.Add(element);
          }
        }
      }
      if (additionalMatchingItems != default)
      {
        // We found too many items !
        additionalMatchingItems.Add(firstMatchingItemFound!);
        var allItems_toShowInDebugger = collection.ToArray();
        // System.Diagnostics.Debugger.Break() ;
        // throw new System.ApplicationException("Expected single item ; more than one item found") ;
        return default;
      }
      else if (firstMatchingItemFound == default)
      {
        // We found no item !
        var allItems_toShowInDebugger = collection.ToArray();
        return default;
      }
      else
      {
        return firstMatchingItemFound;
      }
    }

    public static bool HasSingleMatchingItem<TItem>(
      this IEnumerable<TItem> collection,
      System.Func<TItem, bool> filter,
      [NotNullWhen(true)] out TItem? singleMatchingItem
    )
    where TItem : class
    => (
      singleMatchingItem = GetSingleMatchingItemOrDefault(
        collection,
        filter
      )
    ) != default;

    public static string BuildItemNamesList<T>(
      this IEnumerable<T> items,
      string enclosingBrackets = "[]",
      System.Func<T, string>? itemToStringFunc = null
    ) => (
      // Aha ! x.ToString() now returns a 'string?' that can be null !!!
      items.Select(
        item => itemToStringFunc?.Invoke(item) ?? item?.ToString() ?? "(null)"
      ).OrderBy(
        name => name
      ).ToDelimitedListInBrackets(
        enclosingBrackets: enclosingBrackets // !!!!!!!!!
      )
    );

    public static string ToDelimitedListInBrackets( // NOT NEEDED ...
      this IEnumerable<string> itemNames,
      string itemsSeparator = ",",
      string enclosingBrackets = "[]"
    ) => (
      itemNames.ToDelimitedList(
        itemsSeparator
      ).EnclosedInBrackets(
        enclosingBrackets
      )
    );

    public static string ToDelimitedSummaryString(
      this IEnumerable<object?> items,
      string itemsSeparator = ","
    ) => (
      string.Join(
        itemsSeparator,
        items.WhereNonNull().Select(
          item => item.ToString()
        )
      )
    );

    public static int? IndexOfItem<T>(
      this IEnumerable<T> collection,
      T itemExpectedToBePresent
    )
    {
      var index = System.Array.IndexOf(
        collection.ToArray(),
        itemExpectedToBePresent
      );
      return (
        index == -1
        ? (int?)null
        : index
      );
    }

    public static IEnumerable<string> WhereNotNullOrEmpty(
      this IEnumerable<string?> items
    )
    {
      return items.Where(
        item => !string.IsNullOrEmpty(item)
      ).Select(
        s => s!
      );
    }
  }
}
