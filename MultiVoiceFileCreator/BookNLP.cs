namespace MultiVoiceFileCreator
{
    public class Rootobject
    {
        public Character[] characters { get; set; }
    }

    public class Character
    {
        public Agent[] agent { get; set; }
        public int count { get; set; }
        public G g { get; set; }
        public int id { get; set; }
        public Mentions mentions { get; set; }
        public Mod[] mod { get; set; }
        public Patient[] patient { get; set; }
        public Poss[] poss { get; set; }
    }

    public class G
    {
        public string argmax { get; set; }
        public Inference inference { get; set; }
        public float max { get; set; }
        public float total { get; set; }
    }

    public class Inference
    {
        public float hehimhis { get; set; }
        public float sheher { get; set; }
        public float theythemtheir { get; set; }
        public float xexemxyrxir { get; set; }
        public float zezemzirhir { get; set; }
    }

    public class Mentions
    {
        public Common[] common { get; set; }
        public Pronoun[] pronoun { get; set; }
        public Proper[] proper { get; set; }
    }

    public class Common
    {
        public int c { get; set; }
        public string n { get; set; }
    }

    public class Pronoun
    {
        public int c { get; set; }
        public string n { get; set; }
    }

    public class Proper
    {
        public int c { get; set; }
        public string n { get; set; }
    }

    public class Agent
    {
        public int i { get; set; }
        public string w { get; set; }
    }

    public class Mod
    {
        public int i { get; set; }
        public string w { get; set; }
    }

    public class Patient
    {
        public int i { get; set; }
        public string w { get; set; }
    }

    public class Poss
    {
        public int i { get; set; }
        public string w { get; set; }
    }
}