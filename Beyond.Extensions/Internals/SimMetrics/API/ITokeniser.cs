namespace Beyond.Extensions.Internals.SimMetrics.API;

internal interface ITokeniser
{
    string Delimiters { get; }

    string ShortDescriptionString { get; }

    ITermHandler StopWordHandler { get; set; }

    Collection<string> Tokenize(string word);

    Collection<string> TokenizeToSet(string word);
}