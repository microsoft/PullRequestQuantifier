namespace PullRequestQuantifier.Abstractions.Git.DiffParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using PullRequestQuantifier.Abstractions.Git.DiffParser.Models;

    internal class FileDiffHelper
    {
        private delegate void ParserAction(string line, Match m);

        public static IEnumerable<FileDiff> Parse(string input, string lineEnding = "\n")
        {
            var lines = StringHelper.SplitLines(input, lineEnding).ToList();

            if (!lines.Any())
            {
                return Enumerable.Empty<FileDiff>();
            }

            var files = new List<FileDiff>();
            var inDel = 0;
            var inAdd = 0;

            Chunk current = null;
            FileDiff file = null;

            int oldStart, newStart;
            int oldLines, newLines;

            ParserAction start = (line, m) =>
            {
                file = new FileDiff();
                files.Add(file);

                if (file.To == null && file.From == null)
                {
                    var fileNames = ParseFile(line);

                    if (fileNames != null)
                    {
                        file.From = fileNames[0];
                        file.To = fileNames[1];
                    }
                }
            };

            ParserAction restart = (line, m) =>
            {
                if (file == null || file.Chunks.Count != 0)
                {
                    start(null, null);
                }
            };

            ParserAction new_file = (line, m) =>
            {
                restart(null, null);
                file.Type = FileChangeType.Add;
                file.From = "/dev/null";
            };

            ParserAction deleted_file = (line, m) =>
            {
                restart(null, null);
                file.Type = FileChangeType.Delete;
                file.To = "/dev/null";
            };

            ParserAction index = (line, m) =>
            {
                restart(null, null);
                file.Index = line.Split(' ').Skip(1);
            };

            ParserAction from_file = (line, m) =>
            {
                restart(null, null);
                file.From = ParseFileFallback(line);
            };

            ParserAction to_file = (line, m) =>
            {
                restart(null, null);
                file.To = ParseFileFallback(line);
            };

            ParserAction chunk = (line, match) =>
            {
                inDel = oldStart = int.Parse(match.Groups[1].Value);
                oldLines = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : 0;
                inAdd = newStart = int.Parse(match.Groups[3].Value);
                newLines = match.Groups[4].Success ? int.Parse(match.Groups[4].Value) : 0;
                ChunkRangeInfo rangeInfo = new ChunkRangeInfo(
                    new ChunkRange(oldStart, oldLines),
                    new ChunkRange(newStart, newLines));

                current = new Chunk(line, rangeInfo);
                file.Chunks.Add(current);
            };

            ParserAction del = (line, match) =>
            {
                string content = DiffLineHelper.GetContent(line);
                current.Changes.Add(new LineDiff(type: LineChangeType.Delete, index: inDel++, content: content));
                file.Deletions++;
            };

            ParserAction add = (line, m) =>
            {
                string content = DiffLineHelper.GetContent(line);
                current.Changes.Add(new LineDiff(type: LineChangeType.Add, index: inAdd++, content: content));
                file.Additions++;
            };

            const string noeol = "\\ No newline at end of file";

            Action<string> normal = line =>
            {
                if (file == null)
                {
                    return;
                }

                string content = DiffLineHelper.GetContent(line);
                current.Changes.Add(new LineDiff(
                    oldIndex: line == noeol ? 0 : inDel++,
                    newIndex: line == noeol ? 0 : inAdd++,
                    content: content));
            };

            var schema = new Dictionary<Regex, ParserAction>
            {
                { new Regex(@"^diff\s"), start },
                { new Regex(@"^new file mode \d+$"), new_file },
                { new Regex(@"^deleted file mode \d+$"), deleted_file },
                { new Regex(@"^index\s[\da-zA-Z]+\.\.[\da-zA-Z]+(\s(\d+))?$"), index },
                { new Regex(@"^---\s"), from_file },
                { new Regex(@"^\+\+\+\s"), to_file },
                { new Regex(@"^@@\s+\-(\d+),?(\d+)?\s+\+(\d+),?(\d+)?\s@@"), chunk },
                { new Regex(@"^-"), del },
                { new Regex(@"^\+"), add }
            };

            Func<string, bool> parse = line =>
            {
                foreach (var p in schema)
                {
                    var m = p.Key.Match(line);
                    if (m.Success)
                    {
                        p.Value(line, m);
                        return true;
                    }
                }

                return false;
            };

            foreach (var line in lines)
            {
                if (!parse(line))
                {
                    normal(line);
                }
            }

            return files;
        }

        private static string[] ParseFile(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }

            return s
                .Split(' ')
                .Reverse().Take(2).Reverse()
                .Select(fileName => Regex.Replace(fileName, @"^(a|b)\/", string.Empty)).ToArray();
        }

        private static string ParseFileFallback(string s)
        {
            s = s.TrimStart('-', '+');
            s = s.Trim();

            // ignore possible time stamp
            var t = new Regex(@"\t.*|\d{4}-\d\d-\d\d\s\d\d:\d\d:\d\d(.\d+)?\s(\+|-)\d\d\d\d").Match(s);
            if (t.Success)
            {
                s = s.Substring(0, t.Index).Trim();
            }

            // ignore git prefixes a/ or b/
            return Regex.IsMatch(s, @"^(a|b)\/")
                ? s.Substring(2)
                : s;
        }
    }
}
