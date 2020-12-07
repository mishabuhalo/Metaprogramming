using CSSFormater;
using CSSFormater.FormaterConfigurationModels;
using CSSFormater.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            var tokens = GetTokensFromTestFile(testFilePath);
            FormatVerificationService formatVerificationService = new FormatVerificationService(GetConfiguration(), tokens);

            formatVerificationService.VerifyTokens(true);

            var formatedTokens = formatVerificationService.GetFormatedTokens();

            var resultFileText = File.ReadAllText(resultFilePath).Replace(Environment.NewLine, "\n");

            string formatedText = GetFormatedText(formatedTokens);

            Assert.AreEqual(resultFileText, formatedText);
        }

        [TestMethod]
        public void SecondTest()
        {
            var testFilePath = "../../../TestsFiles/2.css";
            var resultFilePath = "../../../ResultFiles/2.css";

            var tokens = GetTokensFromTestFile(testFilePath);
            FormatVerificationService formatVerificationService = new FormatVerificationService(GetConfiguration(), tokens);

            formatVerificationService.VerifyTokens(true);

            var formatedTokens = formatVerificationService.GetFormatedTokens();

            var resultFileText = File.ReadAllText(resultFilePath).Replace(Environment.NewLine, "\n");

            string formatedText = GetFormatedText(formatedTokens);

            Assert.AreEqual(resultFileText, formatedText);
        }

        [TestMethod]
        public void ThirdTest()
        {
            var testFilePath = "../../../TestsFiles/3.css";
            var resultFilePath = "../../../ResultFiles/3.css";

            var tokens = GetTokensFromTestFile(testFilePath);
            FormatVerificationService formatVerificationService = new FormatVerificationService(GetConfiguration(), tokens);

            formatVerificationService.VerifyTokens(true);

            var formatedTokens = formatVerificationService.GetFormatedTokens();

            var resultFileText = File.ReadAllText(resultFilePath).Replace(Environment.NewLine, "\n");

            string formatedText = GetFormatedText(formatedTokens);

            Assert.AreEqual(resultFileText, formatedText);
        }

        [TestMethod]
        public void HexColorsConvertionTest()
        {
            var testFilePath = "../../../TestsFiles/HexColorsConvertionTest.css";
            var resultFilePath = "../../../ResultFiles/HexColorsConvertionTest.css";

            var tokens = GetTokensFromTestFile(testFilePath);

            FormatVerificationService formatVerificationService = new FormatVerificationService(GetConfiguration(), tokens);

            formatVerificationService.HexColorsToString();

            var formatedTokens = formatVerificationService.GetFormatedTokens();

            var resultFileText = File.ReadAllText(resultFilePath).Replace(Environment.NewLine, "\n");

            string formatedText = GetFormatedText(formatedTokens);

            Assert.AreEqual(resultFileText, formatedText);
        }


        [TestMethod]
        public void BlanckLinesTest()
        {
            var testFilePath = "../../../TestsFiles/BlankLinesTest.css";
            var resultFilePath = "../../../ResultFiles/BlankLinesTest.css";

            var tokens = GetTokensFromTestFile(testFilePath);

            FormatVerificationService formatVerificationService = new FormatVerificationService(GetConfiguration(), tokens);

            formatVerificationService.BlankLinesValidation(true);

            var formatedTokens = formatVerificationService.GetFormatedTokens();

            var resultFileText = File.ReadAllText(resultFilePath).Replace(Environment.NewLine, "\n");

            string formatedText = GetFormatedText(formatedTokens);

            Assert.AreEqual(resultFileText, formatedText);
        }


        [TestMethod]
        public void AlignValuesValidationTest()
        {
            var testFilePath = "../../../TestsFiles/AlignValuesTest.css";
            var resultFilePath = "../../../ResultFiles/AlignValuesTest.css";

            var tokens = GetTokensFromTestFile(testFilePath);

            FormatVerificationService formatVerificationService = new FormatVerificationService(GetConfiguration(), tokens);

            formatVerificationService.AlignValuesValidation(true);

            var formatedTokens = formatVerificationService.GetFormatedTokens();

            var resultFileText = File.ReadAllText(resultFilePath).Replace(Environment.NewLine, "\n");

            string formatedText = GetFormatedText(formatedTokens);

            Assert.AreEqual(resultFileText, formatedText);
        }


        [TestMethod]
        public void QuoteMarksValidationTest()
        {
            var testFilePath = "../../../TestsFiles/QuoteMarksTest.css";
            var resultFilePath = "../../../ResultFiles/QuoteMarksTest.css";

            var tokens = GetTokensFromTestFile(testFilePath);

            FormatVerificationService formatVerificationService = new FormatVerificationService(GetConfiguration(), tokens);

            formatVerificationService.QuoteMarksValidation(true);

            var formatedTokens = formatVerificationService.GetFormatedTokens();

            var resultFileText = File.ReadAllText(resultFilePath).Replace(Environment.NewLine, "\n");

            string formatedText = GetFormatedText(formatedTokens);

            Assert.AreEqual(resultFileText, formatedText);
        }


        [TestMethod]
        public void ClosingBracketsValidationTest()
        {
            var testFilePath = "../../../TestsFiles/ClosingBracketTest.css";
            var resultFilePath = "../../../ResultFiles/ClosingBracketTest.css";

            var tokens = GetTokensFromTestFile(testFilePath);

            FormatVerificationService formatVerificationService = new FormatVerificationService(GetConfiguration(), tokens);

            formatVerificationService.ClosingBracketsValidation(true);

            var formatedTokens = formatVerificationService.GetFormatedTokens();

            var resultFileText = File.ReadAllText(resultFilePath).Replace(Environment.NewLine, "\n");

            string formatedText = GetFormatedText(formatedTokens);

            Assert.AreEqual(resultFileText, formatedText);
        }

        [TestMethod]
        public void SingleLineBlocksValidationTest()
        {
            var testFilePath = "../../../TestsFiles/SingleLineTest.css";
            var resultFilePath = "../../../ResultFiles/SingleLineTest.css";

            var tokens = GetTokensFromTestFile(testFilePath);

            FormatVerificationService formatVerificationService = new FormatVerificationService(GetConfiguration(), tokens);

            formatVerificationService.SingleLineBlocksValidation(true);

            var formatedTokens = formatVerificationService.GetFormatedTokens();

            var resultFileText = File.ReadAllText(resultFilePath).Replace(Environment.NewLine, "\n");

            string formatedText = GetFormatedText(formatedTokens);

            Assert.AreEqual(resultFileText, formatedText);
        }

        [TestMethod]
        public void BracesPlacementValidationTest()
        {
            var testFilePath = "../../../TestsFiles/BracesPlacementTest.css";
            var resultFilePath = "../../../ResultFiles/BracesPlacementTest.css";

            var tokens = GetTokensFromTestFile(testFilePath);

            FormatVerificationService formatVerificationService = new FormatVerificationService(GetConfiguration(), tokens);

            formatVerificationService.BracesPlacementValidation(true);

            var formatedTokens = formatVerificationService.GetFormatedTokens();

            var resultFileText = File.ReadAllText(resultFilePath).Replace(Environment.NewLine, "\n");

            string formatedText = GetFormatedText(formatedTokens);

            Assert.AreEqual(resultFileText, formatedText);
        }

        [TestMethod]
        public void SpacesValidationTest()
        {
            var testFilePath = "../../../TestsFiles/SpacesTest.css";
            var resultFilePath = "../../../ResultFiles/SpacesTest.css";

            var tokens = GetTokensFromTestFile(testFilePath);

            FormatVerificationService formatVerificationService = new FormatVerificationService(GetConfiguration(), tokens);

            formatVerificationService.SpacesValidation(true);

            var formatedTokens = formatVerificationService.GetFormatedTokens();

            var resultFileText = File.ReadAllText(resultFilePath).Replace(Environment.NewLine, "\n");

            string formatedText = GetFormatedText(formatedTokens);

            Assert.AreEqual(resultFileText, formatedText);
        }

        [TestMethod]
        public void TabsAndIndentsVerificationTest()
        {
            var testFilePath = "../../../TestsFiles/TabsAndIndentsTest.css";
            var resultFilePath = "../../../ResultFiles/TabsAndIndentsTest.css";

            var tokens = GetTokensFromTestFile(testFilePath);

            FormatVerificationService formatVerificationService = new FormatVerificationService(GetConfiguration(), tokens);

            formatVerificationService.TabsAndIndentsVerification(true);

            var formatedTokens = formatVerificationService.GetFormatedTokens();

            var resultFileText = File.ReadAllText(resultFilePath).Replace(Environment.NewLine, "\n");

            string formatedText = GetFormatedText(formatedTokens);

            Assert.AreEqual(resultFileText, formatedText);
        }


        private List<Token> GetTokensFromTestFile (string testFilePath)
        {
            Lexer lexer = new Lexer();
            lexer.Lex(testFilePath);
            return lexer.GetTokens();
        }


        private Configuration GetConfiguration()
        {
            return  ConfigurationReadingService.ReadConfiguration();
        }

        private string GetFormatedText(List<Token> formatedTokens)
        {
            string formatedText = "";

            for (int i = 0; i < formatedTokens.Count; ++i)
            {
                formatedText += formatedTokens[i].TokenValue;
            }
            return formatedText;
        }
    }
}
