using Beyond.Extensions.Internals.SimMetrics.API;

namespace Beyond.Extensions.Internals.SimMetrics.Utilities;

public sealed class DummyStopTermHandler : ITermHandler
{
    public void AddWord(string termToAdd)
    {
    }

    public bool IsWord(string termToTest)
    {
        return false;
    }

    public void RemoveWord(string termToRemove)
    {
    }

    public int NumberOfWords => 0;

    public string ShortDescriptionString => "DummyStopTermHandler";

    public StringBuilder WordsAsBuffer => new StringBuilder();
}