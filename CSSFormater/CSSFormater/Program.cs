using CSSFormater.Exceptions;
using CSSFormater.FormaterConfigurationModels;
using CSSFormater.Services;
using System;

namespace CSSFormater
{
    class Program
    {
        static void Main(string[] args)
        {
            var testFilePath = "test.css";
            Configuration configuration = null;
            try
            {
                configuration = ConfigurationReadingService.ReadConfiguration();
            }
            catch (ConfigurationConstructionException e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            Lexer lexer = new Lexer();

            lexer.Lex(testFilePath);

            FormatVerificationService formatVerificationService = new FormatVerificationService(configuration);

            formatVerificationService.VerifyFileTokens(lexer.Tokens);
            lexer.PrintAllTokens();
        }
    }
}
