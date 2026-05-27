namespace CodexSwitchUI.Docs.Docs;

internal static class DocsCodeSamples
{
    public static string Load(string relativePath)
    {
        foreach (var root in CandidateRoots())
        {
            var path = Path.Combine(root, "Examples", "Axaml", relativePath);
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
        }

        return $"<!-- Missing AXAML sample: {relativePath} -->";
    }

    private static IEnumerable<string> CandidateRoots()
    {
        var yielded = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var root in CandidateRootsCore())
        {
            if (yielded.Add(root))
            {
                yield return root;
            }
        }
    }

    private static IEnumerable<string> CandidateRootsCore()
    {
        yield return AppContext.BaseDirectory;

        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "CodexSwitchUI.Docs.csproj")))
            {
                yield return current.FullName;
            }

            var nestedDocsProject = Path.Combine(current.FullName, "src", "CodexSwitchUI.Docs", "CodexSwitchUI.Docs.csproj");
            if (File.Exists(nestedDocsProject))
            {
                yield return Path.GetDirectoryName(nestedDocsProject)!;
            }

            current = current.Parent;
        }
    }
}
