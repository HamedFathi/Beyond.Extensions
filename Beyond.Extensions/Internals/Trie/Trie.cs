namespace Beyond.Extensions.Internals.Trie;

internal class Trie
{
    private readonly TrieNode _root = new TrieNode();

    internal void AddWord(string word)
    {
        var node = _root;

        foreach (char c in word)
        {
            if (!node.Children.ContainsKey(c))
                node.Children[c] = new TrieNode();

            node = node.Children[c];
        }

        node.IsWord = true;
    }

    internal bool Search(string word, bool ignorecase = false)
    {
        var node = _root;
        word = ignorecase ? word.ToLower() : word;
        foreach (char c in word)
        {
            var cc = ignorecase ? Char.ToLower(c) : c;
            if (!node.Children.ContainsKey(cc))
                return false;

            node = node.Children[cc];
        }

        return node.IsWord;
    }
}