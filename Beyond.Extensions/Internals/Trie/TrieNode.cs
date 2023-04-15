namespace Beyond.Extensions.Internals.Trie;

internal class TrieNode
{
    internal Dictionary<char, TrieNode> Children { get; } = new Dictionary<char, TrieNode>();
    internal bool IsWord { get; set; }
}