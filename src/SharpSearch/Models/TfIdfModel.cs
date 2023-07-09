using System.Dynamic;
using SharpSearch.Indices;
using SharpSearch.Models;

class TfIdfModel : IModel
{
    private IIndex? _index;

    public void SetIndex(IIndex index)
    {
        _index = index;
    }

    private double GetTf(string t, Document d)
    {
        int rawTf = _index!.GetTermFrequency(t, d);
        return Math.Log10(1 + rawTf);
    }

    private double GetIdf(string t)
    {
        int N = _index!.GetInfo().DocumentCount;
        int df = _index!.GetDocumentFrequency(t);

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