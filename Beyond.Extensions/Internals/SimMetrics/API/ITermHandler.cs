namespace Beyond.Extensions.Internals.SimMetrics.API;

internal interface ITermHandler
{
    int NumberOfWords { get; }

    string ShortDescriptionString { get; }

    StringBuilder WordsAsBuffer { get; }

    void AddWord(string termToAdd);

    bool IsWord(string termToTest);

    void RemoveWord(string termToRemove);
}