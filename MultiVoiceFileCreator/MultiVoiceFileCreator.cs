using AnyAscii;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Web;
using static Constants;

namespace MultiVoiceFileCreator
{
    public class MultiVoiceFileCreator
    {
        #region Properties

        /// <summary>
        /// CreationParms
        /// </summary>
        public CreationParms Parms { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="parms"></param>
        public MultiVoiceFileCreator(CreationParms parms)
        {
            Parms = parms;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generate files
        /// </summary>
        public void Execute()
        {
            try
            {
                GenerateReadableHTML();
            }
            catch (Exception ex)
            {
                var test = ex;
                throw;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates HTML that can be Read by Edge or the TextToSpeechAPI app
        /// </summary>
        /// <returns></returns>
        private void GenerateReadableHTML()
        {
            if (Parms.Method == Method.MultiVoice && !File.Exists(Parms.BookFileName))
                Parms.Method = Method.BookNLP;

            var html = string.Empty;
            if (Parms.URL.Contains("http"))
            {
                HttpClient client = new();
                html = client.GetStringAsync(Parms.URL).GetAwaiter().GetResult();
            }
            else
                html = new StreamReader(Parms.URL).ReadToEnd();

            // Decode characters
            html = System.Web.HttpUtility.HtmlDecode(html);

            if (Parms.Chapter.HasValue)
            {
                var chapters = html.Split("class=\"heading\"");
                html = $"<p class=\"heading\" {chapters[Parms.Chapter.Value]}";
            }

            ReplacementCharacters.ToList().ForEach(f => html = html.Replace(f.Key, f.Value));

            // Load the html as a html Document and remove empty nodes
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            doc.DocumentNode.RemoveEmptyNodes();

            // Look for divs that we don't care about
            var nodes = doc.DocumentNode.SelectNodes("//div");
            if (nodes.SafeAny())
                foreach (var tag in nodes)
                {
                    if (tag.Attributes["id"] != null &&
                        (tag.Attributes["id"].Value.Compare("preface") ||
                        tag.Attributes["id"].Value.ContainsString("endnotes") ||
                        tag.Attributes["id"].Value.Compare("afterword")))
                        tag.Remove();
                    else if (tag.Attributes["class"] != null && (tag.Attributes["class"].Value.Compare("endnote-link")))
                        tag.Remove();
                }

            // Remove head
            doc.DocumentNode.SelectNodes("//head").RemoveNodes();

            // Add Chapter the header nodes and the title to the very first chapter
            nodes = doc.DocumentNode.SelectNodes("//p");
            var i = Parms.Chapter.HasValue ? Parms.Chapter.Value : 1;
            if (nodes.SafeAny())
                foreach (var tag in nodes)
                {
                    if (tag.Attributes["class"] != null && (tag.Attributes["class"].Value.Compare("heading") || tag.Attributes["class"].Value.Compare("toc-heading")))
                    {
                        tag.InnerHtml = $"{(Parms.IncludeChapterNumbers ? $"Chapter {i}; " : string.Empty)}{tag.InnerHtml}";
                        i++;
                    }
                }

            // Remove any images
            doc.DocumentNode.SelectNodes("//img").RemoveNodes();

            // Remove any formatting found in headers and footers
            doc.DocumentNode.SelectNodes("//blockquote").RemoveNodes();

            // Remove any formatting found in headers and footers
            doc.DocumentNode.SelectNodes("//style").RemoveNodes();

            // Remove all class and id attributes
            doc.DocumentNode.SelectNodes("//h1").RemoveAttributes("class", "id");
            doc.DocumentNode.SelectNodes("//body").RemoveAttributes("class", "id");
            doc.DocumentNode.SelectNodes("//p").RemoveAttributes("class", "id", "align");

            // Reset the html document after all our adjustments
            html = doc.DocumentNode.OuterHtml;
            doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Decode HTML, Remove anything but p tags, get rid of
            html = HttpUtility.HtmlDecode(doc.DocumentNode.OuterHtml).ToString();
            var pattern = @"(?!</?p>)<.*?>";
            html = Regex.Replace(html, pattern, " ");

            // Unicode to ASCII transliteration
            html = html.Transliterate();

            // Clean up html string
            html = html.EmptyLineRemover();
            if (Parms.Method != Method.BookNLP)
                FormattingReplacements.ToList().ForEach(f => html = html.Replace(f.Key, f.Value));
            var regex = new Regex(@"([" + Environment.NewLine + "\t])+");
            html = regex.Replace(html, "");

            // Handle custom replacements that have compiled over the time
            CasedCommonReplacements.ToList().ForEach(f => html = html.Replace(f.Key, f.Value));

            // Reset the html document after all our adjustments
            doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Remove any empty nodes before we start working
            doc.DocumentNode.RemoveEmptyNodes();

            html = doc.DocumentNode.OuterHtml;

            // Write the file out
            WriteFile(html);
        }

        /// <summary>
        /// Write the html to a file
        /// </summary>
        /// <param name="html"></param>
        private void WriteFile(string html)
        {
            html = html.HTMLCleanUp();
            html = html.Replace("</p><p>", "|||").Replace("<p>", "").Replace("</p>", "");
            var paragraphs = html.Split("|||").ToList();
            var finalParagraphs = new List<string>();

            if (Parms.Method == Method.MultiVoice)
            {
                var tagger = new MultiVoiceTagger(Parms, paragraphs.ToList());
                tagger.Execute();
                return;
            }

            if (Parms.Method != Method.Edge && Parms.Method != Method.BookNLP)
            {
                var i = 0;
                var output = new List<string>();
                foreach (var item in paragraphs)
                {
                    if (i == 0)
                    {
                        output.VoiceSplit1000();
                        output.VoiceSplitNarrator();
                        if (!Parms.Chapter.HasValue || Parms.Chapter.Value == 1)
                        {
                            output.Add(Parms.Intro);
                            output.VoiceSplit1500();
                            output.VoiceSplitNarrator();
                        }
                        output.Add(item);
                        output.VoiceSplit1500();
                        i++;
                    }
                    else
                    {
                        if (item.Contains("~~~") || item.Contains(":white_circle:"))
                            output.VoiceSplit2000();
                        else
                        {
                            if (!output.Last().Contains(VOICE_SPLIT))
                                output.VoiceSplit1200();
                            output.VoiceSplitNarrator();
                            output.Add(item);
                        }
                    }
                }
                if (!Parms.Chapter.HasValue || Parms.LastChapter)
                    output.LastChapter();

                output.VoiceSplit2500();
                output.ForEach(f => finalParagraphs.Add(f.DictionaryReplacer(Parms.Method).AddPTags()));
            }

            // Do replacements for pronunciation
            if (!finalParagraphs.Any() && Parms.Method != Method.BookNLP)
                paragraphs.ForEach(f => finalParagraphs.Add(f.DictionaryReplacer(Parms.Method).AddPTags()));
            else if (!finalParagraphs.Any())
            {
                foreach (var paragraph in paragraphs)
                {
                    finalParagraphs.Add(paragraph.Trim());
                    finalParagraphs.Add(string.Empty);
                }
            }

            // Create the directory if it doesn't exist and write the HTML out
            if (!Directory.Exists(Parms.OutputDirectory))
                Directory.CreateDirectory(Parms.OutputDirectory);
            File.WriteAllText(Parms.HTMLFileName, string.Join("\r\n", finalParagraphs));
        }

        #endregion
    }
}