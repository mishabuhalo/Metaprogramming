using CSSFormater.Exceptions;
using CSSFormater.Services;
using System;

namespace CSSFormater
{
    class Program
    {
        static void Main(string[] args)
        {
            var testFilePath = "test.css";

            try
            {
                var configuration = ConfigurationReadingService.ReadConfiguration();
            }
            catch (ConfigurationConstructionException e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            Lexer lexer = new Lexer();

            lexer.Lex(testFilePath);

            lexer.PrintAllTokens();
        }
    }
}
