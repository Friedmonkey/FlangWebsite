using FriedLang;
using FriedLanguage.BuiltinType;
using FriedLanguage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlangWebsiteConsole
{
    public class webIO : LanguageExtention
    {
        public override List<FlangMethod> InjectMethods()
        {
            List<FlangMethod> methods = new()
            {
                new FlangMethod("print", WebPrint, "string message"),
            };

            return methods;
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
        static void Main(string[] args)
        {
        }
    }
}
