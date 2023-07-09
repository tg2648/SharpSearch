using SharpSearch.Models;

namespace SharpSearch.Utilities;

class QueryResultPrinter
{
    public static void PrintResults(string query, IEnumerable<DocumentScore> results, int n = 10)
    {
        Console.WriteLine($"Query results for \"{query}\":");

        int idx = 1;
        foreach (DocumentScore score in results.Take(n))
        {
            Console.WriteLine($"{idx++}. [{score.Score}] {score.Path}");
        }
    }
}