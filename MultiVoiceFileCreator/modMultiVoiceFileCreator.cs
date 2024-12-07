namespace MultiVoiceFileCreator
{
    public class modMultiVoiceFileCreator
    {
        #region Properties

        private static CreationParms _creationParms;
        private static bool _all;

        #endregion

        #region Handler

        private static void Main(string[] args)
        {
            ParseArgs(args.FirstOrDefault());

            // Run the App
            if (_all)
            {
                var chapters = _creationParms.Chapter.GetValueOrDefault(0);
                for (int i = 1; i <= chapters; i++)
                {
                    _creationParms.Chapter = i;
                    _creationParms.LastChapter = i >= chapters;
                    var MultiVoiceFileCreator = new MultiVoiceFileCreator(_creationParms);
                    MultiVoiceFileCreator.Execute();
                }
            }
            else
            {
                var MultiVoiceFileCreator = new MultiVoiceFileCreator(_creationParms);
                MultiVoiceFileCreator.Execute();
            }
        }

        #endregion

        #region Private Methods

        private static void ParseArgs(string txtFile)
        {
            var args = File.ReadAllLines(Path.Combine(@"C:\sandbox\MultiVoiceFileCreator\MultiVoiceFileCreator\Fics", $"{txtFile}.txt"));
            _creationParms = new CreationParms(
                args.GetParm("url"),
                args.GetParm("baseDirectory"),
                args.GetParm("includeChapterNumber"),
                args.GetParm("title"),
                args.GetParm("author"),
                args.GetParm("chapter"),
                args.GetParm("lastChapter"),
                args.GetParm("method"),
                args.GetParm("skipDictionary"));
            _all = args.Contains("/all");
        }

        #endregion
    }
}