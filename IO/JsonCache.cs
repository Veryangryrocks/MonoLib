using System.Text.Json;
using MonoLib;

namespace MonoLib.IO;

public static class JsonCache
{
    private static string _rootDirectory;
    private static Dictionary<(Type, string), object> _objectCache = new();
    private static Dictionary<string, string> _jsonCache = new();
    private static HashSet<string> _pathsSet;

    public static void Load(string rootDirectory)
    {
        _rootDirectory = Path.GetFullPath(PathUtil.NormalizeDirectory(rootDirectory));
        _pathsSet = GetPaths();
    }
    
    private static HashSet<string> GetPaths()
    {
        List<string> pathsList = new();
        foreach (string file in Directory.GetFiles(_rootDirectory, "*", SearchOption.AllDirectories))
        {
            string relativePath = PathUtil.NormalizePath(Path.GetRelativePath(_rootDirectory, file));
            
            if (Path.GetExtension(relativePath) != ".json")
                continue;

            pathsList.Add(relativePath);
        }

        Console.WriteLine($"JsonCache found {pathsList.Count} json file(s) in {_rootDirectory}");
        return pathsList.ToHashSet();
    }

    public static bool TryGetJson(string relativePath, out string json)
    {
        json = default;
        relativePath = PathUtil.NormalizePath(relativePath);

        if (_rootDirectory == null)
            throw new NullReferenceException("JsonManager has not been initialized.");

        if (!_pathsSet.Contains(relativePath))
            return false;

        if (_jsonCache.TryGetValue(relativePath, out string cachedJson))
        {
            json = cachedJson;
            return true;
        }

        json = File.ReadAllText(Path.Combine(_rootDirectory, relativePath));
        return true;
    }

    public static string GetJson(string relativePath)
    {
        relativePath = PathUtil.NormalizePath(relativePath);

        if (_rootDirectory == null)
            throw new InvalidOperationException("JsonManager has not been initialized.");

        if (!_pathsSet.Contains(relativePath))
            throw new InvalidOperationException(nameof(relativePath));

        if (_jsonCache.TryGetValue(relativePath, out string cachedJson))
            return cachedJson;

        return File.ReadAllText(Path.Combine(_rootDirectory, relativePath));
    }

    public static bool TryGet<T>(string relativePath, out T obj)
    {
        obj = default;
        relativePath = PathUtil.NormalizePath(relativePath);

        if (!_pathsSet.Contains(relativePath))
            return false;
        
        if (_objectCache.TryGetValue((typeof(T), relativePath), out object cachedObj) && cachedObj is T)
        {
            obj = (T)cachedObj;
            return true;
        }
        
        if (!TryGetJson(relativePath, out string json))
            return false;
        
        obj = JsonSerializer.Deserialize<T>(json);
        _objectCache[(typeof(T), relativePath)] = obj;
        return true;
    }

    public static T Get<T>(string relativePath)
    {
        relativePath = PathUtil.NormalizePath(relativePath);

        if (!_pathsSet.Contains(relativePath))
            throw new KeyNotFoundException(nameof(relativePath));

        if (_objectCache.TryGetValue((typeof(T), relativePath), out object cachedObj))
            return (T)cachedObj;
        
        string json = GetJson(relativePath);

        T obj = JsonSerializer.Deserialize<T>(json);
        _objectCache[(typeof(T), relativePath)] = obj;
        return obj;
    }
}