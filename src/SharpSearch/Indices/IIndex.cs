using SharpSearch.Models;

namespace SharpSearch.Indices;

public interface IIndex
{
    /// <summary>
    ///     A model that is responsible for calculating document scores
    /// </summary>
    public IModel? Model { get; set; }

    /// <summary>
    ///     Adds file or directory to the index.
    /// </summary>
    public void Add(string path);

    /// <summary>
    ///     Removes file or directory from the index.
    /// </summary>
    public void Remove(string path);

    /// <summary>
    ///     Re-index files that have been modified since they were added to the index.
    /// </summary>
    public void Refresh();

    /// <summary>
    ///     Returns information/statistics about the index
    /// </summary>
    public IndexInfo GetInfo();

    /// <summary>
    ///     Returns documents ranked against the provided query.
    /// </summary>
    public IEnumerable<DocumentScore> CalculateDocumentScores(string query);

    /// <summary>
    ///     Returns the number of times term `t` appears in document `d`.
    /// </summary>
    public int GetTermFrequency(string t, Document d);

    /// <summary>
    ///     Returns the number of documents where term `t` appears at least once.
    /// </summary>
    public int GetDocumentFrequency(string t);
}