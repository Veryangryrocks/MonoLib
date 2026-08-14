

namespace MonoLib.IO;

public static class PathUtil
{
    public static string NormalizePath(string path)
    {
        path = path.Replace('/', '\\');

        while (path.Contains("\\\\"))
            path = path.Replace("\\\\", "\\");

        return path;
    }

    public static string NormalizeDirectory(string directory)
    {
        directory = directory.Replace('/', '\\');

        while (directory.Contains("\\\\"))
            directory = directory.Replace("\\\\", "\\");
        
        return directory.EndsWith('\\') ? directory : directory += '\\';
    }
}