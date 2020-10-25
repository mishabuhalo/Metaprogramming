using CSSFormater.Exceptions;
using CSSFormater.FormaterConfigurationModels;
using CSSFormater.Services;
using System;
using System.IO;
using System.Text;

namespace CSSFormater
{
    class Program
    {

        static void Main(string[] args)
        {
            ReadConsoleParameters(args);
        }

        private static void ReadConsoleParameters(string[] arguments)
        {
            for (int i = 0; i < arguments.Length; ++i)
            {
                if(arguments[i] == "-H" || arguments[i] == "--Help")
                {
                    PrintHelp();
                }
                if(arguments[i] == "-verify")
                {
                    if (i+1< arguments.Length)
                    {
                        if(arguments[i+1] == "-project")
                        {
                            if(i+2 < arguments.Length)
                            {
                                var projectPath = arguments[i + 2];
                                HandleProjectVerification(projectPath);
                            }
                            else
                            {
                                PrintErrorParametersMessage();
                                return;
                            }
                        }
                        else if(arguments[i+i] == "-directory")
                        {
                            if (i + 2 < arguments.Length)
                            {
                                var directoryPath = arguments[i + 2];
                                HandleDirectoryVerification(directoryPath);
                            }
                            else
                            {
                                PrintErrorParametersMessage();
                                return;
                            }
                        }
                        else if(arguments[i+1] == "-file")
                        {
                            if (i + 2 < arguments.Length)
                            {
                                var filePath = arguments[i + 2];
                                HandleFileVerification(filePath);
                            }
                            else
                            {
                                PrintErrorParametersMessage();
                                return;
                            }
                        }
                        else
                        {
                            PrintErrorParametersMessage();
                            return;
                        }
                    }
                    else
                    {
                        PrintErrorParametersMessage();
                        return;
                    }
                }
            }
        }

        private static void HandleFileVerification(string filePath)
        {
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
            lexer.Lex(filePath);
            var tokens = lexer.GetTokens();
            FormatVerificationService formatVerificationService = new FormatVerificationService(configuration);

            formatVerificationService.VerifyFileTokens(tokens);

            var verificationErrors = formatVerificationService.GetVerificationErrors();

            if(verificationErrors.Count>0)
            {
                if(!File.Exists("errors.log"))
                {
                    using (StreamWriter streamWriter = File.CreateText("errors.log"))
                    {
                        for(int i = 0; i < verificationErrors.Count; ++i)
                        {
                            streamWriter.WriteLine(filePath + ": " + verificationErrors[i].ToString());
                        }
                    }
                }
                else
                {
                    using (StreamWriter streamWriter = File.AppendText("errors.log"))
                    {
                        for (int i = 0; i < verificationErrors.Count; ++i)
                        {
                            streamWriter.WriteLine(filePath + ": " +verificationErrors[i].ToString());
                        }
                    }
                }
            }
        }

        private static void HandleDirectoryVerification(string directoryPath)
        {
            throw new NotImplementedException();
        }

        private static void HandleProjectVerification(string projectPath)
        {
            throw new NotImplementedException();
        }


        private static void PrintErrorParametersMessage()
        {
            Console.WriteLine("Invalid parameters syntax. Please use -H or -Help to get help.");
        }

        private static void PrintHelp()
        {
            var helpMessage = "dotnet run [arguments]\nArguments:\n-h, --help Show help message\n-verify -(project|directory|file) \"path\"  Verify file project or directory and write errors to log file";

            Console.WriteLine(helpMessage);
        }
    }
}
