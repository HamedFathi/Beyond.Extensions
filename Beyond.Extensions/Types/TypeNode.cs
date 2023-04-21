namespace Beyond.Extensions.Types;

public class TypeNode
{
    public Type Type { get; set; }
    public List<TypeNode> Children { get; set; }

    public TypeNode(Type type)
    {
        Type = type;
        Children = new List<TypeNode>();
    }
}