namespace Beyond.Extensions.Types;

[Serializable]
public class TypeNode
{
    public TypeNode(Type type)
    {
        Type = type;
        Children = new List<TypeNode>();
    }

    public List<TypeNode> Children { get; set; }
    public Type Type { get; set; }
}