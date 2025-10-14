---
applyTo: "**/*.cs"
---

# Instructions for .NET C# code

C# code (besides unit tests) should adhere to the following guidelines:
- Follow SOLID principles
- Use dependency injection
- Use appropriate configuration formats (JSON, YAML, environment variables)
- Use .NET 8.0 or later features
- Use modern C# features (e.g., records, pattern matching)
- Be well-documented with XML comments
- Ensure high performance and low memory usage
- Be unit tested with a focus on high coverage
- Use async/await for I/O operations
- Use `StringBuilder` for concatenation only in scenarios involving heavy concatenation in loops or performance-critical code
- Use `StringComparison.Ordinal` or `StringComparison.InvariantCulture` for string comparisons, depending on the context
- Avoid using `dynamic` or reflection unless absolutely necessary, such as for JSON deserialization or COM interop
- Use `var` when the type is obvious from the context, but avoid it for complex or ambiguous types
- Use regex source generators with `[GeneratedRegex]` attribute instead of `new Regex()` for compile-time regex validation and better performance

## Unit Testing Approach

When writing or modifying unit tests:

- Use NUnit as the testing framework
- Use Moq for creating mocked objects
- Structure tests according to Arrange-Act-Assert pattern
- Use descriptive test names that indicate the behavior being tested
- Write focused tests that verify a single behavior
- Use appropriate test categories where needed
- Test both success and failure paths
- Test edge cases and boundary conditions
- Ensure tests are deterministic and don't depend on machine-specific configurations
- Use test data builders or fixtures for complex object creation
- Use FluentAssertions (if available) or standard Assert methods

### Naming Conventions

Follow these naming conventions for consistency:

- **Classes and interfaces**: Use PascalCase (e.g., `UserService`, `IRepository`)
- **Methods and properties**: Use PascalCase (e.g., `GetUserAsync`, `UserName`)
- **Variables and private fields**: Use camelCase (e.g., `userName`, `_logger`)
- **Constants**: Use PascalCase (e.g., `MaxRetryCount`) or ALL_CAPS for legacy compatibility
- **Namespaces**: Use PascalCase and follow the pattern `CompanyName.ProjectName.FeatureName`

### Mocking Guidelines

When mocking dependencies:

- Mock interfaces, not concrete classes
- Set up only the methods that will be called in the test
- Be specific about argument matching to catch interface changes
- Use Verify() to confirm that methods were called with expected parameters
- Use appropriate Times constraints (Times.Once(), Times.Exactly(n), etc.)
- Use It.IsAny<T>() only when the exact value doesn't matter
- For async methods, use ReturnsAsync() instead of Returns(Task.FromResult())
- Set up sequential calls when testing behavior that changes over multiple calls
- Clean up mocks in teardown if they hold resources

### Test Data

When creating test data:

- Use constants for common test values
- Use factory methods for creating complex test objects
- Consider using test data builders for complex object graphs
- Use meaningful values that clearly indicate their purpose
- Create custom test data for specific test scenarios
- Use realistic data values when testing parsing or formatting
- Separate test data creation from test logic
- Consider using in-memory test data for integration tests
- Use appropriate data types and ranges
- Document non-obvious test data choices

### Testing Resource Cleanup

When testing resource cleanup:

- Verify that IDisposable.Dispose() is called
- Test both normal and exceptional paths
- Verify that resources are cleaned up in the right order
- Use "using" blocks or try-finally constructs in tests
- Test that resources are cleaned up even after exceptions
- Consider using custom assertions for resource cleanup
- Test memory leaks where appropriate
- Test for resource exhaustion where appropriate
- Verify cancellation behavior
- Test graceful shutdown scenarios

## Documentation guidelines

For public types, provide comprehensive examples on how to use the class.

### XML Documentation

Use XML comments for public APIs:

```csharp
/// <summary>
/// Processes user data asynchronously.
/// </summary>
/// <param name="userData">The user data to process.</param>
/// <param name="cancellationToken">Token to cancel the operation.</param>
/// <returns>A task representing the processing result.</returns>
/// <exception cref="ArgumentNullException">Thrown when userData is null.</exception>
/// <example>
/// <code>
/// var result = await processor.ProcessAsync(userData, CancellationToken.None);
/// </code>
/// </example>
public async Task<ProcessResult> ProcessAsync(UserData userData, CancellationToken cancellationToken = default)
{
    // Method implementation here
}
```

## Repository structure

The following are instructions on how to name folders and files and how the basic repository structure should look like 

- Each project (CSPROJ) should be in its own folder.
- Projects with the implementation code should be under "src" folder.
- Testing projects should be under "tests" folder.
- The repository's root should contain a README.md where you can find basic info about what's in the repository, how to start with local development, how to test, how CI/CD works, what the contribution rules are, etc.
- Each project (component) should have its own README.md describing the specifics of the project, internal architecture, dependencies, etc

### Code Examples

#### Async/Await Pattern
```csharp
public async Task<User> GetUserAsync(int userId, CancellationToken cancellationToken = default)
{
    using var httpClient = _httpClientFactory.CreateClient();
    var response = await httpClient.GetAsync($"/api/users/{userId}", cancellationToken);
    response.EnsureSuccessStatusCode();
    
    var content = await response.Content.ReadAsStringAsync(cancellationToken);
    return JsonSerializer.Deserialize<User>(content);
}
```

#### String Comparison Example
```csharp
// For culture-sensitive comparisons
if (userInput.Equals(expectedValue, StringComparison.InvariantCultureIgnoreCase))

// For ordinal comparisons (recommended for most cases)
if (fileName.EndsWith(".txt", StringComparison.Ordinal))
```

#### StringBuilder Usage
```csharp
// Use StringBuilder for heavy concatenation in loops
var builder = new StringBuilder();
for (int i = 0; i < items.Count; i++)
{
    builder.AppendLine($"Item {i}: {items[i]}");
}
return builder.ToString();
```

#### Regex Source Generators
```csharp
using System.Text.RegularExpressions;

public partial class MyService
{
    // Use regex source generators for compile-time validation and better performance
    [GeneratedRegex(@"\b(query|mutation)\b\s+(\w+)")]
    private static partial Regex OperationNameRegex();
    
    // For culture-sensitive patterns, specify options
    [GeneratedRegex(@"[a-zA-Z_:][a-zA-Z0-9_:]*", RegexOptions.IgnoreCase)]
    private static partial Regex IdentifierRegex();
    
    public string ExtractOperationName(string query)
    {
        var match = OperationNameRegex().Match(query);
        return match.Success ? match.Groups[2].Value : "unknown";
    }
}
```

## General guidelines

Update the respective README.md files (both root-level and project-level) to reflect changes made in the code, including new features, breaking changes, and updated dependencies.

! Important: Write testable code. Cover the written code with unit tests.
! Important: After you implement the code and tests, always run `dotnet build`, `dotnet test`, and `dotnet format` to validate your changes and ensure code style consistency.
