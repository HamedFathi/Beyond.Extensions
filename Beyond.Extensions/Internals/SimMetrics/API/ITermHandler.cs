namespace Beyond.Extensions.Internals.SimMetrics.API;

internal interface ITermHandler
{
    void AddWord(string termToAdd);

    bool IsWord(string termToTest);

    void RemoveWord(string termToRemove);

    int NumberOfWords { get; }

    string ShortDescriptionString { get; }

    StringBuilder WordsAsBuffer { get; }
}