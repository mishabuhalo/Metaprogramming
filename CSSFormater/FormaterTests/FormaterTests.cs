using CSSFormater;
using CSSFormater.FormaterConfigurationModels;
using CSSFormater.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace FormaterTests
{
    [TestClass]
    public class FormaterTests
    {
        [TestMethod]
        public void FirstTest()
        {
            var testFilePath = "../../../TestsFiles/1.css";
            var resultFilePath = "../../../ResultFiles/1.css";

            Configuration configuration = null;

            configuration = ConfigurationReadingService.ReadConfiguration();


            Lexer lexer = new Lexer();
            lexer.Lex(testFilePath);
            var tokens = lexer.GetTokens();
            FormatVerificationService formatVerificationService = new FormatVerificationService(configuration, tokens);

            formatVerificationService.VerifyTokens(true);

            var formatedTokens = formatVerificationService.GetFormatedTokens();

            var resultFileText = File.ReadAllText(resultFilePath).Replace(Environment.NewLine, "\n");

            string formatedText ="";

            for(int i = 0; i < formatedTokens.Count; ++i)
            {
                formatedText += formatedTokens[i].TokenValue;
            }

            Assert.AreEqual(resultFileText, formatedText);
        }

        [TestMethod]
        public void SecondTest()
        {
            var testFilePath = "../../../TestsFiles/2.css";
            var resultFilePath = "../../../ResultFiles/2.css";

            Configuration configuration = null;

            configuration = ConfigurationReadingService.ReadConfiguration();


            Lexer lexer = new Lexer();
            lexer.Lex(testFilePath);
            var tokens = lexer.GetTokens();
            FormatVerificationService formatVerificationService = new FormatVerificationService(configuration, tokens);

            formatVerificationService.VerifyTokens(true);

            var formatedTokens = formatVerificationService.GetFormatedTokens();

            var resultFileText = File.ReadAllText(resultFilePath).Replace(Environment.NewLine, "\n");

            string formatedText = "";

            for (int i = 0; i < formatedTokens.Count; ++i)
            {
                formatedText += formatedTokens[i].TokenValue;
            }

            Assert.AreEqual(resultFileText, formatedText);
        }

        [TestMethod]
        public void ThirdTest()
        {
            var testFilePath = "../../../TestsFiles/3.css";
            var resultFilePath = "../../../ResultFiles/3.css";

            Configuration configuration = null;

            configuration = ConfigurationReadingService.ReadConfiguration();


            Lexer lexer = new Lexer();
            lexer.Lex(testFilePath);
            var tokens = lexer.GetTokens();
            FormatVerificationService formatVerificationService = new FormatVerificationService(configuration, tokens);

            formatVerificationService.VerifyTokens(true);

            var formatedTokens = formatVerificationService.GetFormatedTokens();

            var resultFileText = File.ReadAllText(resultFilePath).Replace(Environment.NewLine, "\n");

            string formatedText = "";

            for (int i = 0; i < formatedTokens.Count; ++i)
            {
                formatedText += formatedTokens[i].TokenValue;
            }

            Assert.AreEqual(resultFileText, formatedText);
        }
    }
}
