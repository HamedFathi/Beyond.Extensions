using Beyond.Extensions.Internals.SimMetrics.API;

namespace Beyond.Extensions.Internals.SimMetrics.Utilities;

internal class TokeniserQGram3 : AbstractTokeniserQGramN
{
    public TokeniserQGram3()
    {
        base.StopWordHandler = new DummyStopTermHandler();
        base.TokenUtilities = new TokeniserUtilities<string>();
        base.CharacterCombinationIndex = 0;
        base.QGramLength = 3;
    }

    public override string ShortDescriptionString => "TokeniserQGram3";

    public override Collection<string> Tokenize(string word)
    {
        return base.Tokenize(word, false, base.QGramLength, base.CharacterCombinationIndex);
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(base.SuppliedWord))
        {
            return string.Format("{0} : not word passed for tokenising yet.", this.ShortDescriptionString);
        }
        return string.Format("{0} - currently holding : {1}.{2}The method is using a QGram length of {3}.", new object[] { this.ShortDescriptionString, base.SuppliedWord, Environment.NewLine, Convert.ToInt32(base.QGramLength) });
    }
}