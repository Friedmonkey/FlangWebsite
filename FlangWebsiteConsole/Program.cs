using FriedLang;
using FriedLanguage.BuiltinType;
using FriedLanguage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlangWebsite;
using FriedLang.NativeLibraries;
using System.IO;

namespace FlangWebsiteConsole
{
    public class webIO : IO
    {
        //public override List<FlangMethod> InjectMethods()
        //{
        //    List<FlangMethod> methods = new()
        //    {
        //        new FlangMethod("print", WebPrint, "string message"),
        //    };

        //    return methods;
        //}
        public override void Intercept()
        {
            InterRemoveMethod("read");
            InterReplaceMethod("print",WebPrint);
        }
        public static FValue WebPrint(Scope scope, List<FValue> arguments)
        {
            FValue val = arguments.FirstOrDefault();

            FValue Text = scope.Get("TEXT");
            FValue Pos = scope.Get("POSITION");
            if (Text is FDictionary Dict)
            {
                if (Pos is FInt Position)
                {
                    FValue ret = Dict.Idx(Position); //if it already exists get that otherwise return fnull
                    if (ret is not FNull)
                    {
                        ret = Dict.SetIndex(Position, new FString(ret.SpagToCsString() + val.SpagToCsString())); //concatenate old and new
                    }
                    else
                    {
                        ret = Dict.SetIndex(Position, val); //if it doest exist then make it exist
                    }
                    scope.SetAdmin("TEXT", Dict); //update it
                    return ret;
                }
            }
            return FValue.Null;
        }
        public static FValue PrintBlue(Scope scope, List<FValue> arguments)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(arguments.First().SpagToCsString());
            Console.ResetColor();
            return arguments.First();
        }
    }
    internal class Program
    {
        public static List<string> suffixes = new() { "", ".html", ".flang", "index.html", "index.flang", "/index.html", "/index.flang" };
        public static Website website;
        static void Main(string[] args)
        {
            website = new Website();

            website.onVisit += Website_onVisit;
            website.SetPort(7902);
            website.Start();
        }
        private static WebResponse Website_onVisit(Website sender, WebsiteContext context)
        {
            string filePath = Path.Combine("www", context.Request.Url.AbsolutePath.Substring(1));
            retry:
            foreach (string suffix in suffixes)
            {
                string pathWithSuffix = filePath + suffix;
                if (File.Exists(pathWithSuffix))
                {
                    if (suffix.Contains("flang") || pathWithSuffix.EndsWith(".flang"))
                        return ParseFlang(File.ReadAllText(pathWithSuffix), context);
                    else
                        return WebResponse.FromFile(pathWithSuffix);
                }
            }
            if (filePath != Path.Combine("www", "fallback"))
            { 
                filePath = Path.Combine("www", "fallback");
                goto retry;
            }

            filePath = Path.Combine("www", context.Request.Url.AbsolutePath.Substring(1));

            Console.WriteLine($"{filePath} not found");
            return WebResponse.FromGenerateError("File not found", $"{filePath} not found");
        }
        private static WebResponse ParseFlang(string input,WebsiteContext context)
        {
            //var lines = Regex.Split(input, "\r\n|\r|\n");
            FlangHTMLParser parser = new FlangHTMLParser();
            (var FinalText, var FinalCode) = parser.Parse(input);

            //Flang.ImportNative
            FLang Flang = new FLang();

            Flang.ImportNative<Lang>("lang");
            Flang.ImportNative<webIO>("io");
            Flang.AddVariable("POSITION", 0);
            Flang.AddVariable("PAGE", context.Page);
            Flang.AddVariable("URL", context.Request.Url);
            Flang.AddVariable("CONTENT", context.Request.ContentRaw);
            Flang.AddDictionary<string, object>("GET", context.Request.QueryString.ToDictionary());
            Flang.AddDictionary<string, object>("POST",context.Request.ContentDict);
            Flang.AddDictionary<int, string>("TEXT", new Dictionary<int, string>());

            string Code = $"""
            import native io;
            import native lang;
            {FinalCode}
            return TEXT;
""";
            object output = Flang.RunCode(Code, false);
            List<(object, object)> outputDict = FLang.ListFromFriedDictionary(output);

            if (outputDict is null)
            {
                return WebResponse.FromGenerateError("An Error occured",Flang.LastError);
            }
            else
            {
                int backtrack = 0;
                for (int i = 0; i < outputDict.Count(); i++)
                {
                    var (key, value) = outputDict.ElementAt(i);
                    if (key is not int index)
                        continue;
                    FinalText = FinalText.Insert(index + backtrack, value.ToString());
                    backtrack += value.ToString().Length;
                }
                return new WebResponse(FinalText);
            }

        }
    }
}
