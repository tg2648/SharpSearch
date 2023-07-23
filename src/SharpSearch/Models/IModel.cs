using SharpSearch.Indices;

namespace SharpSearch.Models;

public interface IModel
{
    public IIndex? Index { get; set; }
    public double CalculateScore(string t, Document d);
}