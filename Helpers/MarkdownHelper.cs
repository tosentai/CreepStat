using Markdig;

namespace CreepStat.Helpers
{
    public static class MarkdownHelper
{
    public static string MarkdownToHtml(string markdown)
    {
        return Markdown.ToHtml(markdown);
    }
}
}