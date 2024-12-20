﻿using static Constants;

namespace MultiVoiceFileCreator
{
    public class CreationParms
    {
        #region Declarations

        public const string Characters = "Characters";

        #endregion

        #region Properties

        public string URL { get; set; }
        public string BaseDirectory { get; set; }
        public bool IncludeChapterNumbers { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int? Chapter { get; set; }
        public bool LastChapter { get; set; }
        public Method Method { get; set; }
        public bool SkipDictionary { get; set; }
        public string OutputDirectory
        {
            get
            {
                var outputDirectory = BaseDirectory;
                if (string.IsNullOrWhiteSpace(BaseDirectory))
                    outputDirectory = Path.Combine(@"C:\sandbox\Epub", Author, Title);
                if (Method == Method.Lines)
                    outputDirectory = Path.Combine(outputDirectory, Characters);
                if (Chapter.HasValue)
                    outputDirectory = Path.Combine(outputDirectory, $"Chapter_{Chapter.Value:00}");
                return outputDirectory;
            }
        }
        public string FileName
        {
            get
            {
                if (Chapter.HasValue)
                    return $"Chapter_{Chapter.Value:00}";
                return $"{Title}";
            }
        }
        public string HTMLFileName
        {
            get
            {
                if (Method == Method.MultiVoice)
                    return Path.Combine(OutputDirectory, $"Tagged_{FileName}.html");
                if (Method == Method.Lines)
                    return Path.Combine(OutputDirectory, $"{FileName}");
                if (Method == Method.BookNLP)
                    return Path.Combine(OutputDirectory, $"BookNLP_{FileName}.txt");
                return Path.Combine(OutputDirectory, $"{FileName}.html");
            }
        }
        public string BookFileName => Path.Combine(OutputDirectory.Replace(Characters, ""), $"{FileName}.book");
        public string QuotesFileName => Path.Combine(OutputDirectory.Replace(Characters, ""), $"{FileName}.quotes");
        public string Intro => $"This is the multi-voice text to speech podfic reading of, {Title} by {Author}, Composed by Burning Aurora, Kaleana, and Flower-Hawk";

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="url"></param>
        /// <param name="baseDirectory"></param>
        /// <param name="includeChapterNumbers"></param>
        /// <param name="title"></param>
        /// <param name="author"></param>
        /// <param name="chapter"></param>
        /// <param name="method"></param>
        public CreationParms(
            string url,
            string baseDirectory,
            string includeChapterNumbers,
            string title,
            string author,
            string chapter,
            string lastChapter,
            string method,
            string skipDictionary)
        {
            URL = url;
            BaseDirectory = baseDirectory;
            IncludeChapterNumbers = string.IsNullOrWhiteSpace(includeChapterNumbers) || includeChapterNumbers.ToBoolean();
            Title = title;
            Author = author;
            Chapter = !string.IsNullOrEmpty(chapter) ? Convert.ToInt32(chapter) : (int?)null;
            LastChapter = !string.IsNullOrWhiteSpace(lastChapter) && lastChapter.ToBoolean();
            Method = Enum.Parse<Method>(method);
            SkipDictionary = !string.IsNullOrWhiteSpace(skipDictionary) && skipDictionary.ToBoolean();

            if (Method.InList(Method.Lines, Method.MultiVoice) && !File.Exists(BookFileName))
                Method = Method.BookNLP;
        }

        #endregion
    }
}