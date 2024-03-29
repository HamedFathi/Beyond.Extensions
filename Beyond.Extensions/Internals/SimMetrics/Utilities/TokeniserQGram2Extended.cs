﻿namespace Beyond.Extensions.Internals.SimMetrics.Utilities;

internal class TokeniserQGram2Extended : TokeniserQGram2
{
    public override string ShortDescriptionString => "TokeniserQGram2Extended";

    public override Collection<string> Tokenize(string word)
    {
        return base.Tokenize(word, true, QGramLength, CharacterCombinationIndex);
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