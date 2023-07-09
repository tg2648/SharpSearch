using SharpSearch.Indices;

namespace SharpSearch.Models;

interface IModel
{
    public void SetIndex(IIndex index);

    public double CalculateScore(string t, Document d);
}