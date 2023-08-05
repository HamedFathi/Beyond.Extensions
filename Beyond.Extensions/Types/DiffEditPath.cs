namespace Beyond.Extensions.Types;

public class DiffEditPath
{
    // Constructor to create a new EditPath instance with the provided values.
    public DiffEditPath(int endpoint, IEnumerable<DiffChange> editHistory)
    {
        Endpoint = endpoint;
        EditHistory = new List<DiffChange>(editHistory);
    }

    public List<DiffChange> EditHistory { get; set; }

    // Properties to store the endpoint and the list of changes in the edit history.
    public int Endpoint { get; set; }
}