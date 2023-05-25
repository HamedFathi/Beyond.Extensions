using Beyond.Extensions.Internals.SimMetrics.API;

namespace Beyond.Extensions.Internals.SimMetrics.Utilities;

internal class TokeniserQGram3 : AbstractTokeniserQGramN
{
    public TokeniserQGram3()
    {
        StopWordHandler = new DummyStopTermHandler();
        TokenUtilities = new TokeniserUtilities<string>();
        CharacterCombinationIndex = 0;
        QGramLength = 3;
    }

    public override string ShortDescriptionString => "TokeniserQGram3";

    public override Collection<string> Tokenize(string word)
    {
        return base.Tokenize(word, false, QGramLength, CharacterCombinationIndex);
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(SuppliedWord))
        {
            return string.Format("{0} : not word passed for tokenising yet.", ShortDescriptionString);
        }
        return string.Format("{0} - currently holding : {1}.{2}The method is using a QGram length of {3}.", ShortDescriptionString, SuppliedWord, Environment.NewLine, Convert.ToInt32(QGramLength));
    }
}