﻿// ReSharper disable StringLiteralTypo

namespace Beyond.Extensions.Internals.Pluralizer.Rules;

internal static class IrregularRules
{
    private static readonly Dictionary<string, string> Dictionary = new()
    {
        // Pronouns.
        { "I", "we" },
        { "me", "us" },
        { "he", "they" },
        { "she", "they" },
        { "them", "them" },
        { "myself", "ourselves" },
        { "yourself", "yourselves" },
        { "itself", "themselves" },
        { "herself", "themselves" },
        { "himself", "themselves" },
        { "themself", "themselves" },
        { "is", "are" },
        { "was", "were" },
        { "has", "have" },
        { "this", "these" },
        { "that", "those" },
        // Words ending in with a consonant and `o`.
        { "echo", "echoes" },
        { "dingo", "dingoes" },
        { "volcano", "volcanoes" },
        { "tornado", "tornadoes" },
        { "torpedo", "torpedoes" },
        // Ends with `us`.
        { "genus", "genera" },
        { "viscus", "viscera" },
        // Ends with `ma`.
        { "stigma", "stigmata" },
        { "stoma", "stomata" },
        { "dogma", "dogmata" },
        { "lemma", "lemmata" },
        { "schema", "schemata" },
        { "anathema", "anathemata" },
        // Other irregular rules.
        { "ox", "oxen" },
        { "axe", "axes" },
        { "die", "dice" },
        { "yes", "yeses" },
        { "foot", "feet" },
        { "eave", "eaves" },
        { "goose", "geese" },
        { "tooth", "teeth" },
        { "quiz", "quizzes" },
        { "human", "humans" },
        { "proof", "proofs" },
        { "carve", "carves" },
        { "valve", "valves" },
        { "looey", "looies" },
        { "thief", "thieves" },
        { "groove", "grooves" },
        { "pickaxe", "pickaxes" },
        { "whiskey", "whiskies" }
    };

    internal static Dictionary<string, string> GetIrregularPlurals()
    {
        var result = new Dictionary<string, string>();
        foreach (var item in Dictionary.Reverse())
            if (!result.ContainsKey(item.Value))
                result.Add(item.Value, item.Key);

        return result;
    }

    internal static Dictionary<string, string> GetIrregularSingulars()
    {
        return Dictionary;
    }
}