using Newtonsoft.Json;

namespace MultiVoiceFileCreator
{
    public class MultiVoiceTagger
    {
        #region Properties

        /// <summary>
        /// Parms for all input and output info
        /// </summary>
        public CreationParms Parms { get; private set; }

        /// <summary>
        /// Paragraphs of the fic
        /// </summary>
        public List<string> Paragraphs { get; }

        /// <summary>
        /// Narration Characters
        /// </summary>
        public Dictionary<int, string> Characters { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="parms"></param>
        /// <param name="paragraphs"></param>
        public MultiVoiceTagger(CreationParms parms, List<string> paragraphs)
        {
            Parms = parms;
            Paragraphs = paragraphs;
            Characters = new Dictionary<int, string>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generate the Multi-Voice Tagged file
        /// </summary>
        public void Execute()
        {
            // Make sure the directory we're writing to exists
            if (!Directory.Exists(Parms.OutputDirectory))
                Directory.CreateDirectory(Parms.OutputDirectory);

            // Get the characters to populate the tags
            DeriveCharacters();

            var bookNLPQuotes = File.ReadAllLines(Parms.QuotesFileName);
            var quotes = bookNLPQuotes.Skip(1).Take(bookNLPQuotes.Length - 1).Select(f => new Quote(f, Characters)).ToList();
            var batchedQuotes = quotes.GroupBy(f => f.Name).Select(f => f.Key).ToList().ToDictionary(f => f, f => new List<string>());
            var taggedParagraphs = new List<string>();
            var insertIntro = true;
            var previousWasQuote = false;
            foreach (var paragraph in Paragraphs)
            {
                if (insertIntro)
                {
                    taggedParagraphs.VoiceSplit1000();
                    taggedParagraphs.VoiceSplitNarrator();
                    if (!Parms.Chapter.HasValue || paragraph.Contains("Chapter 1") || paragraph.Contains("Chapter One"))
                    {
                        if (!Parms.Chapter.HasValue || Parms.Chapter.Value == 1)
                        {
                            taggedParagraphs.Add(Parms.Intro);
                            taggedParagraphs.VoiceSplit1500();
                            taggedParagraphs.VoiceSplitNarrator();
                        }
                    }
                    taggedParagraphs.Add(paragraph);
                    taggedParagraphs.VoiceSplit1500();
                    insertIntro = false;
                    continue;
                }

                var lines = paragraph.Replace("\"", "|").Split("|").Where(f => !string.IsNullOrWhiteSpace(f)).ToList();
                var formattedLines = lines.Select(f => f.ToPunctuationlessString()).ToList();
                formattedLines = formattedLines.Select(f => f.Replace("\r\n", "").ToUpper()).ToList();

                var applicableQuotes = quotes.Where(f => formattedLines.Contains(f.Line))?.ToList();
                if (applicableQuotes?.Any() == true)
                {
                    if (previousWasQuote)
                        taggedParagraphs.VoiceSplit700();
                    var i = 0;
                    previousWasQuote = true;
                    foreach (var line in lines)
                    {
                        var upperLine = line.ToPunctuationlessString().ToUpper();
                        var quote = applicableQuotes.Where(f => f.Line == upperLine).FirstOrDefault();
                        if (quote != null)
                        {
                            if (!taggedParagraphs.Last().Contains(Constants.VOICE_SPLIT))
                                taggedParagraphs.VoiceSplit1200();
                            taggedParagraphs.Add($"{Constants.VOICE_SPLIT} {quote.Name}");
                            taggedParagraphs.Add(line);

                            // Pop line and quote from lists
                            batchedQuotes[quote.Name].Add(line);
                            quotes.RemoveAt(0);
                            applicableQuotes.RemoveAt(0);
                        }
                        else
                        {
                            if (!taggedParagraphs.Last().Contains(Constants.VOICE_SPLIT))
                                taggedParagraphs.VoiceSplit1200();
                            taggedParagraphs.VoiceSplitNarrator();
                            taggedParagraphs.Add(line);
                        }
                        if (i != lines.Count - 1)
                            taggedParagraphs.VoiceSplit400();
                        i++;
                    }
                }
                else
                {
                    if (paragraph.Contains("~~~") || paragraph.Contains(":white_circle:"))
                        taggedParagraphs.VoiceSplit2000();
                    else
                    {
                        if (!taggedParagraphs.Last().Contains(Constants.VOICE_SPLIT))
                            taggedParagraphs.VoiceSplit1200();
                        taggedParagraphs.VoiceSplitNarrator();
                        taggedParagraphs.Add(paragraph);
                    }
                    previousWasQuote = false;
                }
            }
            if (Parms.LastChapter)
                taggedParagraphs.LastChapter();

            taggedParagraphs.VoiceSplit2500();

            var finalParagraphs = new List<string>();
            taggedParagraphs.ForEach(f => finalParagraphs.Add(f.DictionaryReplacer(Parms.Method).AddPTags()));

            if (Parms.Method == Constants.Method.MultiVoice)
                File.WriteAllText(Parms.HTMLFileName, string.Join("\r\n", finalParagraphs));
            else if (Parms.Method == Constants.Method.Lines)
                GenerateCharacterFiles(batchedQuotes);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Read the book file and pull out character ids and Names
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void DeriveCharacters()
        {
            var bookNLPString = File.ReadAllText(Parms.BookFileName);
            var bookNLP = JsonConvert.DeserializeObject<Rootobject>(bookNLPString);
            if (bookNLP == null)
                throw new Exception($"{Parms.BookFileName} is null!!! Oh No!!!");
            foreach (var character in bookNLP.characters)
            {
                if (character == null)
                    continue;
                var name = character.mentions?.proper?.FirstOrDefault()?.n;
                if (!string.IsNullOrWhiteSpace(name))
                    Characters.Add(character.id, name);
            }
        }

        /// <summary>
        /// Writes out each characters lines
        /// </summary>
        /// <param name="quotes"></param>
        private void GenerateCharacterFiles(Dictionary<string, List<string>> quotes)
        {
            foreach (var quote in quotes)
                File.WriteAllText($"{Parms.HTMLFileName}_{quote.Key}.txt", string.Join("\r\n", quote.Value));
        }

        #endregion

        #region Private Classes

        private class Quote
        {
            public string CharId { get; set; }
            public string Name { get; set; }
            public string Line { get; set; }

            public Quote(string bookNLPLine, Dictionary<int, string> characters)
            {
                var quoteLine = bookNLPLine.Split("\t");
                CharId = quoteLine[^2];
                Name = characters.ContainsKey(Convert.ToInt32(CharId)) ? characters[Convert.ToInt32(CharId)] : "Unknown";
                if (Name.Compare("Moony"))
                    Name = "Remus";
                Line = quoteLine.Last().ToPunctuationlessString().ToUpper();
            }
        }

        #endregion
    }
}