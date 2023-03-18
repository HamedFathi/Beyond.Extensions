namespace Beyond.Extensions.Internals.Trie;
internal class TrieNode
{
    internal bool IsWord { get; set; }
    internal Dictionary<char, TrieNode> Children { get; } = new Dictionary<char, TrieNode>();
}