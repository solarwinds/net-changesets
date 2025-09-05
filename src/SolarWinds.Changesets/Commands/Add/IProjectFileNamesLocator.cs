namespace SolarWinds.Changesets.Commands.Add;

/// <summary>
/// Defines a contract for locating project file names within a specified directory.
/// </summary>
public interface IProjectFileNamesLocator
{
    /// <summary>
    /// Retrieves project file names located in the specified directory.
    /// </summary>
    /// <param name="startDirectoryFullPath">The full path of the directory to search for project files.</param>
    /// <returns>An array of strings representing the names of the project files found in the directory.</returns>
    string[] GetProjectFileNames(string startDirectoryFullPath);
}
