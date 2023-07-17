using SharpSearch.Indices;

namespace SharpSearch.Models;

public interface IModel
{
    public void SetIndex(IIndex index);

    public double CalculateScore(string t, Document d);
}