namespace SolarWinds.Changesets.Commands.Add;

/// <summary>
/// Provides functionality to locate C# project file names within a specified directory.
/// </summary>
internal sealed class CsharpProjectFileNamesLocator : IProjectFileNamesLocator
{
    /// <summary>
    /// The file extension pattern used to identify C# project files.
    /// </summary>
    private const string ProjectFileExtension = "*.csproj";

    /// <summary>
    /// Retrieves the names of all C# project files (without extensions) located in the specified directory and its subdirectories.
    /// </summary>
    /// <param name="startDirectoryFullPath">The full path of the directory to search.</param>
    /// <returns>An array of project file names without their extensions.</returns>
    public string[] GetProjectFileNames(string startDirectoryFullPath)
    {
        return Directory
            .GetFiles(startDirectoryFullPath, ProjectFileExtension, SearchOption.AllDirectories)
            .Select(csprojectFilePath => Path.GetFileNameWithoutExtension(csprojectFilePath))
            .ToArray()
            ;
    }
}
