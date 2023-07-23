using SharpSearch.Indices;

namespace SharpSearch.Models;

public class TfIdfModel : IModel
{
    public IIndex? Index { get; set; }

    private double GetTf(string t, Document d)
    {
        int rawTf = Index!.GetTermFrequency(t, d);
        return Math.Log10(1 + rawTf);
    }

    private double GetIdf(string t)
    {
        int N = Index!.GetInfo().DocumentCount;
        int df = Index!.GetDocumentFrequency(t);

        return Math.Log10(N / df);
    }

    private double CalculateTfIdf(string t, Document d)
    {
        return GetTf(t, d) * GetIdf(t);
    }

    public double CalculateScore(string t, Document d)
    {
        return CalculateTfIdf(t, d);
    }
}