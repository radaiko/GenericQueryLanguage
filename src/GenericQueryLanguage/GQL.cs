namespace GenericQueryLanguage;

public class GQL<T> {

  private string[] Filters;
  private static readonly string[] Splitters = ["AND", "OR"];
  private static readonly string Sorter = "OrderBy";
  private static readonly string[] Assigners = ["is", "is not", "contains", "does not contain", "starts with", "ends with"];

  public GQL() {
    Filters = typeof(T).GetProperties()
      .Where(p => p.CanRead && p.GetGetMethod().IsPublic)
      .Select(p => p.Name)
      .ToArray();
  }

  public List<string> SuggestNextInput(string query) {
    var suggestions = new List<string>();

    if (string.IsNullOrWhiteSpace(query)) {
      return Filters.ToList();
    }

    var directions = Enum.GetNames(typeof(ESortingDirection)).ToList();
    var parts = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    var lastPart = parts.LastOrDefault()?.ToLower() ?? string.Empty;

    if (lastPart.Contains("=") || directions.Any(d => d == lastPart)) return [];
    if (lastPart.Contains("=")) return [];
    if (lastPart == "orderby") return directions;
    if (lastPart == "and" || lastPart == "or") return Filters.Where (f => !parts.Any(p => p == f)).ToList();

    // also check if the lastpart is finished
    var isStartOfFilter = Filters.Where(f => f.StartsWith(lastPart, StringComparison.InvariantCultureIgnoreCase) && !f.Equals(lastPart, StringComparison.InvariantCultureIgnoreCase));
    var isStartOfSorter = Sorter.StartsWith(lastPart) && !Sorter.Equals(lastPart, StringComparison.InvariantCultureIgnoreCase);
    var isStartOfSplitter = Splitters.Where(s => s.StartsWith(lastPart, StringComparison.InvariantCultureIgnoreCase) && !s.Equals(lastPart, StringComparison.InvariantCultureIgnoreCase));


    if (isStartOfFilter.Any()) return isStartOfFilter.ToList();
    if (isStartOfSorter) return new List<string> { Sorter };
    if (isStartOfSplitter.Any()) return isStartOfSplitter.ToList();


    if (Splitters.Any(s => query.EndsWith(s, StringComparison.OrdinalIgnoreCase))) {
      return Filters.Concat(new[] { Sorter }).ToList();
    }

    if (query.Contains("AND", StringComparison.OrdinalIgnoreCase) || query.Contains("OR", StringComparison.OrdinalIgnoreCase)) {
      var index = query.LastIndexOf("AND", StringComparison.OrdinalIgnoreCase);
      if (index == -1) {
        index = query.LastIndexOf("OR", StringComparison.OrdinalIgnoreCase);
      }

      if (index != -1) {
        lastPart = query.Substring(index + 3).Trim().ToLower();
      }
    }

    if (Filters.Any(filter => lastPart.StartsWith(filter.ToLower(), StringComparison.OrdinalIgnoreCase) && !lastPart.Contains("="))) {
      suggestions.Add("=");
    }
    else if (lastPart == "") suggestions.Add("=");
    else if (Filters.Any(filter => lastPart == (filter.ToLower() + "="))) {
      suggestions.Add("");
    }
    else if (lastPart.StartsWith("order", StringComparison.OrdinalIgnoreCase)) {
      suggestions.Add("By");
    }

    if (!suggestions.Any() && parts.Length > 1) {
      suggestions.AddRange(Splitters);
      suggestions.Add(Sorter);
    }

    return suggestions.Distinct().ToList();
  }

  public List<T> FilterAndSort(List<T> input, string query) {
    if (input.Count == 0) return [];
    // TODO: Implement filtering and sorting
    return [];
  }
}

public enum ESortingDirection {
  asc
  , desc
}