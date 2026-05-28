using Avalonia.Controls;

namespace CodexSwitchUI.Docs.Docs;

internal sealed record DocsCategory(string Title, IReadOnlyList<DocsPage> Pages);

internal sealed record DocsStateCase(string State, string Surface, string Contract);

internal sealed record DocsEventCase(string Input, string Expected);

internal sealed record DocsCodeSnippet(string Title, string SamplePath);

internal sealed record DocsExampleCase(
    string Title,
    string Description,
    string SamplePath,
    Func<Control> BuildPreview,
    IReadOnlyList<DocsCodeSnippet>? AdditionalCodeSamples = null)
{
    public IReadOnlyList<DocsCodeSnippet> CodeSamples
    {
        get
        {
            var samples = new List<DocsCodeSnippet> { new(SamplePath, SamplePath) };
            if (AdditionalCodeSamples is not null)
            {
                samples.AddRange(AdditionalCodeSamples);
            }

            return samples;
        }
    }
}

internal sealed record DocsPage(
    string Id,
    string Category,
    string Title,
    string Description,
    string SamplePath,
    Func<Control> BuildPreview,
    IReadOnlyList<DocsExampleCase> Examples,
    IReadOnlyList<string> Sections,
    IReadOnlyList<string> BehaviorNotes,
    IReadOnlyList<DocsStateCase> StateCases,
    IReadOnlyList<DocsEventCase> EventCases);
