namespace Beyond.Extensions.Internals.PropertyPathResolver;

public class SelectionEnumerator : IEnumerator<object>
{
    private readonly Selection _selection;
    private int _currentIndex;

    public SelectionEnumerator(Selection selection)
    {
        _selection = selection;
        _currentIndex = -1;
    }

    public object? Current
    {
        get
        {
            lock (_selection)
            {
                if (_currentIndex == -1) return null;
                if (_currentIndex >= _selection.Entries.Count)
                    throw new InvalidOperationException("The enumerator is past the last element. Call Reset to start again from the first one");
                return _selection.Entries.ElementAt(_currentIndex);
            }
        }
    }

    public void Dispose()
    {
    }

    public bool MoveNext()
    {
        _currentIndex++;
        return _currentIndex < _selection.Entries.Count;
    }

    public void Reset()
    {
        _currentIndex = -1;
    }
}