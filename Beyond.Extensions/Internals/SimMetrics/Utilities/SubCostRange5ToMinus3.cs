using Beyond.Extensions.Internals.SimMetrics.API;

namespace Beyond.Extensions.Internals.SimMetrics.Utilities;

internal sealed class SubCostRange5ToMinus3 : AbstractSubstitutionCost
{
    private const int CharApproximateMatchScore = 3;
    private const int CharExactMatchScore = 5;
    private const int CharMismatchMatchScore = -3;
    private Collection<string>[] _approx = new Collection<string>[7];

    public SubCostRange5ToMinus3()
    {
        _approx[0] = new Collection<string>();
        _approx[0].Add("d");
        _approx[0].Add("t");
        _approx[1] = new Collection<string>();
        _approx[1].Add("g");
        _approx[1].Add("j");
        _approx[2] = new Collection<string>();
        _approx[2].Add("l");
        _approx[2].Add("r");
        _approx[3] = new Collection<string>();
        _approx[3].Add("m");
        _approx[3].Add("n");
        _approx[4] = new Collection<string>();
        _approx[4].Add("b");
        _approx[4].Add("p");
        _approx[4].Add("v");
        _approx[5] = new Collection<string>();
        _approx[5].Add("a");
        _approx[5].Add("e");
        _approx[5].Add("i");
        _approx[5].Add("o");
        _approx[5].Add("u");
        _approx[6] = new Collection<string>();
        _approx[6].Add(",");
        _approx[6].Add(".");
    }

    public override double MaxCost => 5.0;

    public override double MinCost => -3.0;

    public override string ShortDescriptionString => "SubCostRange5ToMinus3";

    public override double GetCost(string firstWord, int firstWordIndex, string secondWord, int secondWordIndex)
    {
        if (firstWord != null && secondWord != null)
        {
            if (firstWord.Length <= firstWordIndex || firstWordIndex < 0)
            {
                return -3.0;
            }
            if (secondWord.Length <= secondWordIndex || secondWordIndex < 0)
            {
                return -3.0;
            }
            if (firstWord[firstWordIndex] == secondWord[secondWordIndex])
            {
                return 5.0;
            }
            var ch = firstWord[firstWordIndex];
            var item = ch.ToString().ToLowerInvariant();
            var ch2 = secondWord[secondWordIndex];
            var str2 = ch2.ToString().ToLowerInvariant();
            for (var i = 0; i < _approx.Length; i++)
            {
                if (_approx[i].Contains(item) && _approx[i].Contains(str2))
                {
                    return 3.0;
                }
            }
        }
        return -3.0;
    }
}