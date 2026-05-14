using System.Runtime.CompilerServices;

namespace CodexSwitchUI.Tests;

internal static class TestRepository
{
    public static string FindRoot([CallerFilePath] string sourceFilePath = "")
    {
        foreach (var startDirectory in CandidateStartDirectories(sourceFilePath))
        {
            var current = new DirectoryInfo(startDirectory);
            while (current is not null)
            {
                if (IsRepositoryRoot(current.FullName))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }
        }

        throw new DirectoryNotFoundException("Could not locate CodexSwitchUI repository root.");
    }

    private static IEnumerable<string> CandidateStartDirectories(string sourceFilePath)
    {
        yield return AppContext.BaseDirectory;
        yield return Environment.CurrentDirectory;

        var sourceDirectory = Path.GetDirectoryName(sourceFilePath);
        if (!string.IsNullOrWhiteSpace(sourceDirectory))
        {
            yield return sourceDirectory;
        }
    }

    private static bool IsRepositoryRoot(string directory)
    {
        return File.Exists(Path.Combine(directory, "CodexSwitchUI.slnx"))
            && Directory.Exists(Path.Combine(directory, "src", "CodexSwitchUI"));
    }
}
