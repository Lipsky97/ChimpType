using System.Text;

namespace ChimpType.Services
{
    public class TextService
    {
        private readonly string[] _wordFiles;
        private readonly string[] _quoteFiles;
        public TextService(IWebHostEnvironment env) 
        {
            var textPath = Path.Combine(env.ContentRootPath, "Resources");
            _wordFiles = Directory.GetFiles(Path.Combine(textPath, "Words"), "*.txt");
            _quoteFiles = Directory.GetFiles(Path.Combine(textPath, "Quotes"), "*.txt");
        }

        public string GetRandomWords(int amount)
        {
            var words = new List<string>(amount);
            var rng = Random.Shared;

            while (words.Count < amount)
            {
                using var reader = File.OpenText(_wordFiles[0]);
                string? word;
                int needed = amount - words.Count;

                while (needed > 0 && (word = ReadNextWord(reader)) != null)
                {
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        words.Add(word);
                        needed--;
                    }
                }
            }

            words = words.Shuffle().ToList();
            return string.Join(" ", words);
        }

        public string GetQuote(QuoteLength length)
        {
            if (_quoteFiles.Length == 0) return string.Empty;

            int idx = Random.Shared.Next(_quoteFiles.Length);
            var quote = File.ReadAllText(_quoteFiles[idx]);
            var quoteArr = quote.Split(". ");

            switch (length)
            {
                case QuoteLength.Short:
                    quoteArr = quoteArr.Take(1).ToArray();
                    break;
                case QuoteLength.Mid:
                    quoteArr = quoteArr.Take(5).ToArray();
                    break;
                case QuoteLength.Long:
                    quoteArr = quoteArr.Take(10).ToArray();
                    break;
            }

            quote = string.Join(". ", quoteArr);

            return quote + ".";
        }

        private static string? ReadNextWord(StreamReader reader)
        {
            var sb = new StringBuilder(20);
            int ch;

            // Skip non-word chars (spaces, newlines, punctuation)
            while ((ch = reader.Peek()) != -1 && !IsWordChar((char)ch))
                reader.Read();

            // Read word
            while ((ch = reader.Peek()) != -1 && IsWordChar((char)ch))
            {
                sb.Append((char)reader.Read());
            }

            return sb.Length > 0 ? sb.ToString().ToLowerInvariant() : null;
        }

        private static bool IsWordChar(char c) =>
            (c >= 'a' && c <= 'z') || (c >= 'A' && 'Z' <= c) || c == '\'';
        private static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = Random.Shared.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }

    public enum QuoteLength
    {
        Short,
        Mid,
        Long,
        ExtraLong
    }

    public static class StringBuilderExtentions
    {
        public static int WordCount(this StringBuilder sb)
        {
            int count = 0;
            int pos = 0;
            while ((pos = sb.ToString().IndexOf(' ', pos)) >= 0)
            {
                count++;
                pos++;
            }

            return count;
        }
    }
}
