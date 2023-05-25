using Beyond.Extensions.Internals.SimMetrics.API;

namespace Beyond.Extensions.Internals.SimMetrics.Utilities;

internal sealed class SubCostRange5ToMinus3 : AbstractSubstitutionCost
{
    private const int charApproximateMatchScore = 3;
    private const int charExactMatchScore = 5;
    private const int charMismatchMatchScore = -3;
    private Collection<string>[] approx = new Collection<string>[7];

    public SubCostRange5ToMinus3()
    {
        approx[0] = new Collection<string>();
        approx[0].Add("d");
        approx[0].Add("t");
        approx[1] = new Collection<string>();
        approx[1].Add("g");
        approx[1].Add("j");
        approx[2] = new Collection<string>();
        approx[2].Add("l");
        approx[2].Add("r");
        approx[3] = new Collection<string>();
        approx[3].Add("m");
        approx[3].Add("n");
        approx[4] = new Collection<string>();
        approx[4].Add("b");
        approx[4].Add("p");
        approx[4].Add("v");
        approx[5] = new Collection<string>();
        approx[5].Add("a");
        approx[5].Add("e");
        approx[5].Add("i");
        approx[5].Add("o");
        approx[5].Add("u");
        approx[6] = new Collection<string>();
        approx[6].Add(",");
        approx[6].Add(".");
    }

    public override double MaxCost => 5.0;

    public override double MinCost => -3.0;

    public override string ShortDescriptionString => "SubCostRange5ToMinus3";

    public override double GetCost(string firstWord, int firstWordIndex, string secondWord, int secondWordIndex)
    {
        if ((firstWord != null) && (secondWord != null))
        {
            if ((firstWord.Length <= firstWordIndex) || (firstWordIndex < 0))
            {
                return -3.0;
            }
            if ((secondWord.Length <= secondWordIndex) || (secondWordIndex < 0))
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
            for (var i = 0; i < approx.Length; i++)
            {
                if (approx[i].Contains(item) && approx[i].Contains(str2))
                {
                    return 3.0;
                }
            }
        }
        return -3.0;
    }
}