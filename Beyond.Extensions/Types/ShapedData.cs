namespace Beyond.Extensions.Types;

public class ShapedData
{
    public ShapedData()
    {
        Values = new List<List<DataField>>();
    }

    // ReSharper disable once CollectionNeverQueried.Global
    public List<List<DataField>> Values { get; }
}