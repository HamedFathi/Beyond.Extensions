// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedType.Global

using Beyond.Extensions.ByteArrayExtended;
using Beyond.Extensions.Enums;
using Beyond.Extensions.StreamExtended;
using Beyond.Extensions.StringExtended;
using Beyond.Extensions.Types;

namespace Beyond.Extensions.JsonExtended;

public static class JsonExtensions
{
    public static T? Deserialize<T>(this byte[] data, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(data.ToText(), options);
    }

    public static T? Deserialize<T>(this Stream stream, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(stream.ToText(), options);
    }

    public static string FlattenJson(this string jsonString)
    {
        var json = JsonSerializer.Deserialize<JsonElement>(jsonString);
        var flattenedJson = new Dictionary<string, object>();
        json.FlattenJsonElement(string.Empty, flattenedJson);
        return JsonSerializer.Serialize(flattenedJson);
    }
    public static T? FromJson<T>(this string jsonText, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(jsonText, options);
    }

    public static string? GetJsonPropertyValue(this string jsonText, string propertyName)
    {
        return string.IsNullOrEmpty(jsonText) ? null : jsonText.ToJsonNode()?[propertyName]?.AsValue().ToString();
    }

    public static IEnumerable<string> GetKeys(this JsonDocument jsonDocument, string separator = ".")
    {
        var jsonElement = jsonDocument.RootElement;
        return jsonElement.GetKeys(separator);
    }

    public static IEnumerable<string> GetKeys(this JsonElement jsonElement, string separator = ".")
    {
        var queue = new Queue<(string ParentPath, JsonElement element)>();
        queue.Enqueue(("", jsonElement));
        while (queue.Any())
        {
            var (parentPath, element) = queue.Dequeue();
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    parentPath = parentPath == ""
                        ? "$" + separator
                        : parentPath + separator;
                    foreach (var nextEl in element.EnumerateObject())
                    {
                        queue.Enqueue(($"{parentPath}{nextEl.Name}", nextEl.Value));
                    }
                    yield return parentPath.Trim(separator) + "-object";
                    break;
                case JsonValueKind.Array:
                    foreach (var (nextEl, i) in element.EnumerateArray().Select((js, i) => (js, i)))
                    {
                        if (string.IsNullOrEmpty(parentPath))
                            parentPath = "$" + separator;
                        queue.Enqueue(($"{parentPath}[{i}]", nextEl));
                    }
                    yield return parentPath.Trim(separator) + "-array";
                    break;
                case JsonValueKind.String:
                    var isDate = DateTime.TryParse(element.ToString(), out _);
                    var type = isDate ? "-date" : "-string";
                    yield return parentPath.Trim(separator) + type;
                    break;
                case JsonValueKind.Number:
                    yield return parentPath.Trim(separator) + "-number";
                    break;
                case JsonValueKind.Undefined:
                    yield return parentPath.Trim(separator) + "-undefined";
                    break;
                case JsonValueKind.Null:
                    yield return parentPath.Trim(separator) + "-null";
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    yield return parentPath.Trim(separator) + "-boolean";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public static IEnumerable<string> GetPaths(this JsonDocument jsonDocument, string separator = ".")
    {
        var jsonElement = jsonDocument.RootElement;
        return jsonElement.GetKeys(separator).Select(x => x.Substring(0, x.LastIndexOfAny(new[] { '-' })));
    }

    public static IEnumerable<string> GetPaths(this JsonElement jsonElement, string separator = ".")
    {
        return jsonElement.GetKeys(separator).Select(x => x.Substring(0, x.LastIndexOfAny(new[] { '-' })));
    }

    public static bool? IsOpenApiDocument(string swaggerJsonText)
    {
        try
        {
            var parsedJson = JsonNode.Parse(swaggerJsonText);

            var openapi = parsedJson?["openapi"];
            var swagger = parsedJson?["swagger"];
            var info = parsedJson?["info"];
            var title = parsedJson?["info"]?["title"];
            var version = parsedJson?["info"]?["version"];
            var paths = parsedJson?["paths"];

            var status = (openapi != null || swagger != null)
                         && info != null
                         && version != null
                         && paths != null
                         && title != null;

            return status;
        }
        catch
        {
            return null;
        }
    }

    public static dynamic? ToDynamic(this string jsonText, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<dynamic>(jsonText, options);
    }

    public static string ToFormattedJson(this string jsonText)
    {
        jsonText = jsonText.Trim().Trim(',');
        var parsedJson = JsonDocument.Parse(jsonText, new JsonDocumentOptions { AllowTrailingCommas = true });
        return JsonSerializer.Serialize(parsedJson, new JsonSerializerOptions { WriteIndented = true });
    }

    public static string ToIndentedJson<T>(this T obj)
    {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    public static string ToJson<T>(this T obj, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize(obj, options);
    }

    public static JsonArray? ToJsonArray(this JsonElement jsonElement)
    {
        return jsonElement.Deserialize<JsonArray>();
    }

    public static JsonArray? ToJsonArray(this JsonNode jsonNode)
    {
        try
        {
            return jsonNode.AsArray();
        }
        catch
        {
            return null;
        }
    }

    public static JsonElement ToJsonElement(this JsonNode jsonNode)
    {
        var element = jsonNode.Deserialize<JsonElement>();
        return element;
    }

    public static JsonNode? ToJsonNode(this JsonElement jsonElement)
    {
        return jsonElement.Deserialize<JsonNode>();
    }

    public static JsonNode? ToJsonNode(this string json)
    {
        return JsonNode.Parse(json);
    }

    public static JsonObject? ToJsonObject(this JsonElement jsonElement)
    {
        return jsonElement.Deserialize<JsonObject>();
    }

    public static JsonObject? ToJsonObject(this JsonNode jsonNode)
    {
        try
        {
            return jsonNode.AsObject();
        }
        catch
        {
            return null;
        }
    }

    public static object? ToObject(this string jsonText, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<object>(jsonText, options);
    }

    public static void Walk(this JsonElement jsonElement, Action<JsonData> action, string separator = ".")
    {
        var queue = new Queue<(string ParentPath, JsonElement element)>();
        queue.Enqueue(("", jsonElement));
        while (queue.Any())
        {
            var (parentPath, element) = queue.Dequeue();
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    parentPath = parentPath == ""
                        ? "$" + separator
                        : parentPath + separator;
                    foreach (var nextEl in element.EnumerateObject())
                    {
                        queue.Enqueue(($"{parentPath}{nextEl.Name}", nextEl.Value));
                    }
                    action(new JsonData()
                    {
                        Key = parentPath.Trim(separator),
                        Kind = JsonDataValueKind.Object,
                        Value = element
                    });
                    break;

                case JsonValueKind.Array:
                    foreach (var (nextEl, i) in element.EnumerateArray().Select((js, i) => (js, i)))
                    {
                        if (string.IsNullOrEmpty(parentPath))
                            parentPath = "$" + separator;
                        queue.Enqueue(($"{parentPath}[{i}]", nextEl));
                    }
                    action(new JsonData()
                    {
                        Key = parentPath.Trim(separator),
                        Kind = JsonDataValueKind.Array,
                        Value = element
                    });
                    break;

                case JsonValueKind.String:
                    var isDate = DateTime.TryParse(element.GetString(), out _);
                    action(new JsonData()
                    {
                        Key = parentPath.Trim(separator),
                        Kind = isDate ? JsonDataValueKind.DateTime : JsonDataValueKind.String,
                        Value = element
                    });
                    break;

                case JsonValueKind.Number:
                    action(new JsonData()
                    {
                        Key = parentPath.Trim(separator),
                        Kind = JsonDataValueKind.Number,
                        Value = element.GetDouble()
                    });
                    break;

                case JsonValueKind.Undefined:
                    action(new JsonData()
                    {
                        Key = parentPath.Trim(separator),
                        Kind = JsonDataValueKind.Undefined,
                        Value = element
                    });
                    break;

                case JsonValueKind.Null:
                    action(new JsonData()
                    {
                        Key = parentPath.Trim(separator),
                        Kind = JsonDataValueKind.Null,
                        Value = element
                    });
                    break;

                case JsonValueKind.True:
                case JsonValueKind.False:
                    action(new JsonData()
                    {
                        Key = parentPath.Trim(separator),
                        Kind = JsonDataValueKind.Boolean,
                        Value = element.GetBoolean()
                    });
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private static void FlattenJsonElement(this JsonElement json, string parentKey, Dictionary<string, object> flattenedJson)
    {
        foreach (JsonProperty property in json.EnumerateObject())
        {
            string key = string.IsNullOrEmpty(parentKey) ? property.Name : $"{parentKey}.{property.Name}";

            switch (property.Value.ValueKind)
            {
                case JsonValueKind.Object:
                    FlattenJsonElement(property.Value, key, flattenedJson);
                    break;
                default:
                    flattenedJson[key] = property.Value.ToString();
                    break;
            }
        }
    }
}