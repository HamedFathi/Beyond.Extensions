﻿namespace Beyond.Extensions.Internals.SimMetrics.API;

internal interface ITokeniser
{
    Collection<string> Tokenize(string word);

    Collection<string> TokenizeToSet(string word);

    string Delimiters { get; }

    string ShortDescriptionString { get; }

    ITermHandler StopWordHandler { get; set; }
}