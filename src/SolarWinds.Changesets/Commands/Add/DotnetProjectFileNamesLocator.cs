namespace SolarWinds.Changesets.Commands.Add;

/// <summary>
/// Provides functionality to locate .NET project file names (.csproj and .fsproj) within a specified directory.
/// </summary>
internal sealed class DotnetProjectFileNamesLocator : IProjectFileNamesLocator
{
    /// <summary>
    /// The file extension patterns used to identify .NET project files.
    /// </summary>
    private static readonly string[] s_projectFileExtensions = ["*.csproj", "*.fsproj"];

    /// <summary>
    /// Retrieves the names of all .NET project files (without extensions) located in the specified directory and its subdirectories.
    /// </summary>
    /// <param name="startDirectoryFullPath">The full path of the directory to search.</param>
    /// <returns>An array of project file names without their extensions.</returns>
    public string[] GetProjectFileNames(string startDirectoryFullPath)
    {
        return s_projectFileExtensions
            .SelectMany(extension => Directory.GetFiles(startDirectoryFullPath, extension, SearchOption.AllDirectories))
            .Select(projectFilePath => Path.GetFileNameWithoutExtension(projectFilePath))
            .ToArray()
            ;
    }
}
