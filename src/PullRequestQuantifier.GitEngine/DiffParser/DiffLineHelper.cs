namespace PullRequestQuantifier.GitEngine.DiffParser
{
    public static class DiffLineHelper
    {
        public static string GetContent(string line)
        {
            string content = line.Substring(1);
            return content;
        }
    }
}
