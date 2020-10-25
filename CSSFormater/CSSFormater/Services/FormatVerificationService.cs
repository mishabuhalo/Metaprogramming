using CSSFormater.Constants;
using CSSFormater.FormaterConfigurationModels;
using CSSFormater.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSSFormater.Services
{
    public class FormatVerificationService
    {
        private List<VerificationError> verificationErrors;
        private Configuration _configuration;

        public FormatVerificationService(Configuration configuration)
        {
            _configuration = configuration;
            verificationErrors = new List<VerificationError>();
        }
        public void VerifyFileTokens(List<Token> fileTokens)
        {
            TabsAndIndentsVerification(fileTokens);
        }

        private void TabsAndIndentsVerification(List<Token> fileTokens)
        {
            int i = 0;
            var useTabCharacter = _configuration.TabsAndIndents.UseTabCharacter;
            var useSmartTabs = _configuration.TabsAndIndents.SmartTabs;
            var tabSize = _configuration.TabsAndIndents.TabSize;
            var indentSize = _configuration.TabsAndIndents.Indent;
            var keepIndentsOnEmptyLines = _configuration.TabsAndIndents.KeepIndentsOnEmtyLines;
            var neededTabsCount = indentSize == tabSize ? 1 : indentSize % tabSize;
            

            for (; i < fileTokens.Count -1; ++i)
            {
                if(fileTokens[i].TokenValue == "{")
                {
                    i++;
                    while(fileTokens[i+1].TokenValue != "}")
                    {
                        if (fileTokens[i].TokenType == TokenTypes.NewLine)
                        {
                            i++;
                            var lineNumber = fileTokens[i].LineNumber+1;
                            var currentIndent = 0;
                            var tabsCount = 0;
                            while (fileTokens[i].TokenType == TokenTypes.Tab || fileTokens[i].TokenType == TokenTypes.WhiteSpace)
                            {
                                if (fileTokens[i].TokenType == TokenTypes.Tab)
                                {
                                    if(!useTabCharacter)
                                    {
                                        verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = "Code style error. You should use white spaces instead of tabs.", ErrorType = VerificationErrorTypes.TabsAndIndentsError });
                                    }
                                    currentIndent += tabSize;
                                    tabsCount++;
                                }
                                else
                                {
                                    if(fileTokens[i].TokenValue.Length == tabSize && useSmartTabs)
                                    {
                                        verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = $"Code style error. Here should be tab instead of {tabSize} white spaces", ErrorType = VerificationErrorTypes.TabsAndIndentsError });
                                    }
                                    currentIndent += fileTokens[i].TokenValue.Length;
                                }

                                i++;
                            }
                            if(currentIndent==indentSize)
                            {
                                if(tabsCount!=neededTabsCount && useTabCharacter)
                                {
                                    verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = "Code style error. Invalid tabs count.", ErrorType = VerificationErrorTypes.TabsAndIndentsError });
                                }
                            }
                            else if(currentIndent>indentSize)
                            {
                                verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = $"Code style error. Indent is bigger then should be on {currentIndent-indentSize}.", ErrorType = VerificationErrorTypes.TabsAndIndentsError });
                            }
                            else
                            {
                                verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = $"Code style error. Indent is less then should be on {indentSize - currentIndent}.", ErrorType = VerificationErrorTypes.TabsAndIndentsError });
                            }

                            if(fileTokens[i].TokenType==TokenTypes.NewLine)
                            {
                                if(!keepIndentsOnEmptyLines)
                                {
                                    verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = "Code style error. You should not keep indentes on empty lines", ErrorType = VerificationErrorTypes.TabsAndIndentsError });
                                }
                            }
                        }
                        else
                            i++;
                    }
                }
            }
        }
    }
}
