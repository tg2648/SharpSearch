namespace SharpSearch.Indices;

/// <summary>
///     Information/statistics about the index
/// </summary>
/// <param name="DocumentCount">How many documents are in the index</param>
public record class IndexInfo(int DocumentCount);
