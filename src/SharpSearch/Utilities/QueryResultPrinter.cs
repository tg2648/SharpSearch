using SharpSearch.Models;

namespace SharpSearch.Utilities;

public class QueryResultPrinter
{
    private const int DEFAULT_LIMIT = 10;

    public static void PrintResults(string query, IEnumerable<DocumentScore> results, int n = DEFAULT_LIMIT)
    {
        Console.WriteLine($"Query results for \"{query}\":");

        int idx = 1;
        foreach (DocumentScore score in results.Take(n))
        {
            Console.WriteLine($"{idx++}. [{Math.Round(score.Score, 2)}] {score.Path}");
        }
    }
}