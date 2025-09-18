using System.Runtime.InteropServices;
using AwesomeAssertions;
using SolarWinds.Changesets.Shared;

namespace SolarWinds.Changesets.Tests;

[TestFixture]
internal sealed class ProcessExecutorTests
{
    private ProcessExecutor _processExecutor;
    private string _executable = string.Empty;
    private char _argumentsTemplate;

    [SetUp]
    public void SetUp()
    {
        _processExecutor = new ProcessExecutor();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _executable = "cmd";
            _argumentsTemplate = '/';
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            _executable = "/bin/bash";
            _argumentsTemplate = '-';
        }
        else
        {
            throw new PlatformNotSupportedException("Unsupported OS platform.");
        }
    }

    [Test]
    public async Task Execute_ReceivesOutput_WhenProcessExecutesSuccessfully()
    {
        string expectedOutput = "Hello Changesets";
        string arguments = $"{_argumentsTemplate}c \"echo {expectedOutput}\"";

        ProcessOutput processOutput = await _processExecutor.Execute(_executable, arguments, Environment.CurrentDirectory);

        processOutput.ExitCode.Should().Be(0);
        processOutput.Output.First().Should().Be(expectedOutput);
    }

    [Test]
    public async Task Execute_ReceivesNoZeroExitCode_WhenProcessExecutionFails()
    {
        string command = "bla";
        string arguments = $"{_argumentsTemplate}c \"{command}\"";
        string expectedOutput = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
            "'bla' is not recognized" : "command not found";

        ProcessOutput processOutput = await _processExecutor.Execute(_executable, arguments, Environment.CurrentDirectory);

        processOutput.ExitCode.Should().BeGreaterThan(0);

        string.Join(' ', processOutput.Output).Should().Contain(expectedOutput);
    }
}
