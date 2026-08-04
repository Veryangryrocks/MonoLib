using Microsoft.Xna.Framework.Content;

namespace MonoLib;

public static class ContentCache
{
    private static ContentManager _contentManager;
    private static string _mgcbPath;
    private static Dictionary<string, object> _contentCache = new();
    private static HashSet<string> _pathsSet;
    public static void Load(ContentManager contentManager, string mgcbPath)
    {
        _contentManager = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
        _mgcbPath = Path.GetFullPath(PathUtil.NormalizePath(mgcbPath));

        if (!File.Exists(_mgcbPath))
            throw new FileNotFoundException(_mgcbPath);
        
        _pathsSet = GetPaths();
    }

    private static HashSet<string> GetPaths()
    {
        var lines = File.ReadAllLines(_mgcbPath)
            .Where(line => line.StartsWith("/build:", StringComparison.OrdinalIgnoreCase))
            .Select(line => line.Substring(7).Trim())
            .Select(path => Path.ChangeExtension(PathUtil.NormalizePath(path), null))
            .Distinct()
            .ToList();

        Console.WriteLine($"ContentCache found {lines.Count} asset(s) in {_mgcbPath}");
        return lines.ToHashSet();
    }

    public static bool TryGet<T>(string relativePath, out T obj)
    {
        obj = default;
        relativePath = PathUtil.NormalizePath(relativePath);

        if (_contentManager is null)
            throw new NullReferenceException("ContentCache has not been initialized.");
        
        if (!_pathsSet.Contains(relativePath))
            return false;
        
        if (_contentCache.TryGetValue(relativePath, out object cachedObj) && cachedObj is T)
        {
            obj = (T)cachedObj;
            return true;
        }

        obj = _contentManager.Load<T>(relativePath);
        _contentCache[relativePath] = obj;
        return true;
    }

    public static T Get<T>(string relativePath)
    {
        relativePath = PathUtil.NormalizePath(relativePath);

        if (_contentManager is null)
            throw new NullReferenceException("ContentCache has not been initialized.");
        
        if (!_pathsSet.Contains(relativePath))
            throw new KeyNotFoundException(relativePath);
        
        if (_contentCache.TryGetValue(relativePath, out object cachedObj) && cachedObj is T)
            return (T)cachedObj;

        T obj = _contentManager.Load<T>(relativePath);
        _contentCache[relativePath] = obj;
        return obj;
    }
}