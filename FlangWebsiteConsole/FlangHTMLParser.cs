using System.Linq;

namespace FlangWebsiteConsole
{
    public class FlangHTMLParser : AnalizerBase<char>
    {
        public FlangHTMLParser() : base('\0') { }
        public (string FinalText, string FinalCode) Parse(string input)
        {
            string FinalText = string.Empty;
            string FinalCode = string.Empty;
            this.Analizable = input.ToList();
            bool isFlang = false;

            while (Current != '\0')
            {
                if (isFlang)
                {
                    if (Find(")>"))
                    {
                        isFlang = false;
                        continue;
                    }
                    else
                    {
                        FinalCode += Current;
                        Position++;
                    }
                }
                else
                {
                    if (Find("<(flang"))
                    {
                        isFlang = true;
                        //FinalText += Current;
                        Position++;
                        FinalCode += $" POSITION = {FinalText.Length}; ";
                        continue;
                    }
                    else
                    {
                        FinalText += Current;
                        Position++;
                    }
                }
            }


            return (FinalText, FinalCode);
        }
        public bool Find(string find)
        {
            for (int i = 0; i < find.Length; i++)
            {
                if (Peek(i) == find[i])
                {
                    continue;
                }
                else return false;
            }
            Position += find.Length;
            return true;
        }
    }
}
