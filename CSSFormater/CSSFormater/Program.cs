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
                                PrintSuccessVerifiactionMesssage();
                            }
                            else
                            {
                                PrintErrorParametersMessage();
                                return;
                            }
                        }
                        else if(arguments[i+1] == "-directory")
                        {
                            if (i + 2 < arguments.Length)
                            {
                                var directoryPath = arguments[i + 2];
                                HandleDirectoryVerification(directoryPath);
                                PrintSuccessVerifiactionMesssage();
                                return;
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
                                PrintSuccessVerifiactionMesssage();
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
                if(arguments[i] == "-format")
                {
                    if (i + 1 < arguments.Length)
                    {
                        if (arguments[i + 1] == "-project")
                        {
                            if (i + 2 < arguments.Length)
                            {
                                var projectPath = arguments[i + 2];
                                HandleProjectVerification(projectPath, true);
                                PrintSuccessVerifiactionMesssage(true);
                            }
                            else
                            {
                                PrintErrorParametersMessage();
                                return;
                            }
                        }
                        else if (arguments[i + 1] == "-directory")
                        {
                            if (i + 2 < arguments.Length)
                            {
                                var directoryPath = arguments[i + 2];
                                HandleDirectoryVerification(directoryPath, true);
                                PrintSuccessVerifiactionMesssage(true);
                                return;
                            }
                            else
                            {
                                PrintErrorParametersMessage();
                                return;
                            }
                        }
                        else if (arguments[i + 1] == "-file")
                        {
                            if (i + 2 < arguments.Length)
                            {
                                var filePath = arguments[i + 2];
                                HandleFileVerification(filePath, true);
                                PrintSuccessVerifiactionMesssage(true);
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
                else
                {
                    PrintErrorParametersMessage();
                    return;
                }
            }

        }

        private static void PrintSuccessVerifiactionMesssage(bool isFormated = false)
        {
            Console.WriteLine("Verification completed, all verification errors has been written to file error.log in project folder");
            if(isFormated)
            {
                Console.WriteLine("Files modified to coe style seted in configurations.");
            }
        }

        private static void HandleFileVerification(string filePath, bool shouldFormat = false)
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
            FormatVerificationService formatVerificationService = new FormatVerificationService(configuration, tokens);

            formatVerificationService.VerifyTokens(shouldFormat);

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

            if(shouldFormat)
            {
                using (StreamWriter streamWriter = File.CreateText(filePath))
                {
                    var formatedTokens = formatVerificationService.GetFormatedTokens();
                    for(int i = 0; i <formatedTokens.Count; ++i)
                    {
                        
                        streamWriter.Write(formatedTokens[i].TokenValue);
                    }
                }
            }
        }


        private static void HandleDirectoryVerification(string directoryPath, bool shouldFormat = false)
        {
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"Directory {directoryPath} does not exist.");
                return;
            }

            var filePaths = Directory.GetFiles(directoryPath, "*.css");

            for (int i = 0; i < filePaths.Length; ++i)
            {
                HandleFileVerification(filePaths[i], shouldFormat);
            }
           
        }

        private static void HandleProjectVerification(string projectPath, bool shouldFormat = false)
        {
            if (!Directory.Exists(projectPath))
            {
                Console.WriteLine($"Project directory {projectPath} does not exist.");
                return;
            }

            var filePaths = Directory.GetFiles(projectPath, "*.css", SearchOption.AllDirectories);

            for (int i = 0; i < filePaths.Length; ++i)
            {
                HandleFileVerification(filePaths[i], shouldFormat);
            }
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
