using GenericQueryLanguage;

namespace GenericQueryLanguageTests;

[TestClass]
public sealed class GQLTests {
  public TestContext TestContext { get; set; }
  
  [TestMethod]
  [DataRow("", new[] { "Language", "Title", "Authors", "Year", "FileType", "ISBN-10", "ISBN-13", "SortingType" })]
  [DataRow("T", new[] { "Title" })]
  [DataRow("Ti", new[] { "Title" })]
  [DataRow("Aut", new[] { "Authors" })]
  [DataRow("ISBN", new[] { "ISBN-10", "ISBN-13" })]
  [DataRow("I", new[] { "ISBN-10", "ISBN-13" })]
  [DataRow("Title", new[] { "=" })]
  [DataRow("Title ", new[] { "=" })]
  [DataRow("Title =", new string[]{})]
  [DataRow("Title = ", new string[]{})]
  [DataRow("Authors", new[] { "=" })]
  [DataRow("Authors ", new[] { "=" })]
  [DataRow("Authors is", new string[]{})]
  [DataRow("Authors is not", new string[]{})]
  [DataRow("Authors is ", new string[]{})]
  [DataRow("Authors is not ", new string[]{})]
  [DataRow("Title is test python ", new[] { "AND", "OR", "OrderBy" })]
  [DataRow("Title is test python AND ", new[] { "Language", "Authors", "Year", "FileType", "ISBN-10", "ISBN-13", "SortingType" })]
  [DataRow("Title is test python AND Au", new[] { "Authors" })]
  [DataRow("(Language is en or Language is de) and Title is testing python for dummy's and SortingType is title OrderBy desc", new string[] { })]
  [DataRow("(Language is en or Language is de) and Title is testing python for dummy's and SortingType is title ", new[] { "AND", "OR", "OrderBy" })]
  public void TestSuggestion(string query, string[] expected) {
    var gql = new GQL<Dummy>();
    var result = gql.SuggestNextInput(query);
    TestContext.WriteLine($"Query: {query}");
    TestContext.WriteLine($"Result: \"{string.Join(", ", result)}\"");
    TestContext.WriteLine($"Expected: \"{string.Join(", ", expected)}\"");
    CollectionAssert.AreEqual(expected, result);
  }
}