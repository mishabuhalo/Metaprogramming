﻿using CSSFormater.Constants;
using CSSFormater.FormaterConfigurationModels;
using CSSFormater.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            BlankLinesValidation(fileTokens);
            BracesPlacementValidation(fileTokens);
            AlignValuesValidation(fileTokens);
            QuoteMarksValidation(fileTokens);
            ClosingBracketsValidation(fileTokens);
            SingleLineBlocksValidation(fileTokens);
            SpacesValidation(fileTokens);
            HexColorsValidation(fileTokens);
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


            for (; i < fileTokens.Count - 1; ++i)
            {
                if (fileTokens[i].TokenValue == "{")
                {
                    i++;
                    while (fileTokens[i + 1].TokenValue != "}")
                    {
                        if (fileTokens[i].TokenType == TokenTypes.NewLine)
                        {
                            i++;
                            var lineNumber = fileTokens[i].LineNumber + 1;
                            var currentIndent = 0;
                            var tabsCount = 0;
                            while (fileTokens[i].TokenType == TokenTypes.Tab || fileTokens[i].TokenType == TokenTypes.WhiteSpace)
                            {
                                if (fileTokens[i].TokenType == TokenTypes.Tab)
                                {
                                    if (!useTabCharacter)
                                    {
                                        verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = "You should use white spaces instead of tabs.", ErrorType = VerificationErrorTypes.TabsAndIndentsError });
                                    }
                                    currentIndent += tabSize;
                                    tabsCount++;
                                }
                                else
                                {
                                    if (fileTokens[i].TokenValue.Length == tabSize && useSmartTabs)
                                    {
                                        verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = $"Here should be tab instead of {tabSize} white spaces", ErrorType = VerificationErrorTypes.TabsAndIndentsError });
                                    }
                                    currentIndent += fileTokens[i].TokenValue.Length;
                                }

                                i++;
                            }
                            if (currentIndent == indentSize)
                            {
                                if (tabsCount != neededTabsCount && useTabCharacter)
                                {
                                    verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = "Invalid tabs count.", ErrorType = VerificationErrorTypes.TabsAndIndentsError });
                                }
                            }
                            else if (currentIndent > indentSize)
                            {
                                verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = $"Indent is bigger then should be on {currentIndent - indentSize}.", ErrorType = VerificationErrorTypes.TabsAndIndentsError });
                            }
                            else
                            {
                                verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = $"Indent is less then should be on {indentSize - currentIndent}.", ErrorType = VerificationErrorTypes.TabsAndIndentsError });
                            }

                            if (fileTokens[i].TokenType == TokenTypes.NewLine)
                            {
                                if (!keepIndentsOnEmptyLines)
                                {
                                    verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = "You should not keep indentes on empty lines", ErrorType = VerificationErrorTypes.TabsAndIndentsError });
                                }
                            }
                        }
                        else
                            i++;
                    }
                }
            }
        }

        private void BlankLinesValidation(List<Token> fileTokens)
        {
            var maximumBlankLinesInCode = _configuration.BlankLines.MaximumBlankLinesInCode;
            var minimumBlankLinesAroundTopLevelBlocks = _configuration.BlankLines.MinimumBlankLinesAroundTopLevelBlock;

            var lastClosedBracketToken = fileTokens.Where(x => x.TokenValue == "}").Last();

            var lastIndexOfClosingBracket = fileTokens.LastIndexOf(lastClosedBracketToken);

            var countOfNextEmptyLines = 0;
            for (int i = lastIndexOfClosingBracket; i < fileTokens.Count; ++i)
            {
                if (fileTokens[i].TokenType == TokenTypes.NewLine)
                    countOfNextEmptyLines++;
            }
            if (maximumBlankLinesInCode < countOfNextEmptyLines - 1)
                verificationErrors.Add(new VerificationError { ErrorLineNumber = lastClosedBracketToken.LineNumber + 1, ErrorMessage = $"You exceeded the limit of extra blank lines to be kept on {countOfNextEmptyLines - maximumBlankLinesInCode}.", ErrorType = VerificationErrorTypes.BlankLinesError });

            var j = 0;
            for (; j < fileTokens.Count - 1; ++j)
            {
                if (fileTokens[j].TokenValue == "}" && j != lastIndexOfClosingBracket)
                {
                    j++;
                    var lineNumber = fileTokens[j + 1].LineNumber;
                    var currentBlankLinesCount = 0;
                    while (fileTokens[j].TokenValue != "{" && j < fileTokens.Count)
                    {
                        if (fileTokens[j].TokenType == TokenTypes.NewLine)
                        {
                            currentBlankLinesCount++;
                        }
                        j++;
                    }

                    if (currentBlankLinesCount - 1 < minimumBlankLinesAroundTopLevelBlocks)
                        verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = $"You ddid not reach the minimum number of blank lines around top level blocks on {minimumBlankLinesAroundTopLevelBlocks - currentBlankLinesCount + 1}.", ErrorType = VerificationErrorTypes.BlankLinesError });
                }
            }
        }

        private void BracesPlacementValidation(List<Token> fileTokens)
        {
            var bracesPlacementType = _configuration.Other.BracesPlacement;

            for (int i = 1; i < fileTokens.Count; ++i)
            {
                if (fileTokens[i].TokenValue == "{")
                {
                    if (fileTokens[i - 1].TokenType == TokenTypes.NewLine && bracesPlacementType != BracesPlacementTypes.NextLine)
                        verificationErrors.Add(new VerificationError { ErrorLineNumber = fileTokens[i].LineNumber, ErrorMessage = "You should place bracket at the end of line", ErrorType = VerificationErrorTypes.BracesReplacementError });
                    if (fileTokens[i - 1].TokenType != TokenTypes.NewLine && bracesPlacementType != BracesPlacementTypes.EndOfLine)
                        verificationErrors.Add(new VerificationError { ErrorLineNumber = fileTokens[i].LineNumber + 1, ErrorMessage = "You should place bracket at the next line", ErrorType = VerificationErrorTypes.BracesReplacementError });
                }
            }
        }

        private void AlignValuesValidation(List<Token> fileTokens)
        {
            var alignValuesType = _configuration.Other.AlignValues;

            int i = 0;

            for (; i < fileTokens.Count - 1; ++i)
            {
                if (fileTokens[i].TokenValue == "{")
                {
                    i++;
                    var maxStartIndex = GetMaximalStartIndexOfAttributeValueInBlock(fileTokens, i);
                    while (fileTokens[i].TokenValue != "}")
                    {
                        if (fileTokens[i].TokenValue == ":")
                        {
                            var lineNumber = fileTokens[i].LineNumber;

                            if (alignValuesType == AlignValuesTypes.DoNotAlign)
                            {
                                if (fileTokens[i + 1].TokenType != TokenTypes.WhiteSpace && _configuration.Other.Spaces.AfterColon)
                                {
                                    verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber + 1, ErrorMessage = "Between ':' and attribute value must be a white space!", ErrorType = VerificationErrorTypes.AlignValuesError });
                                }
                                else
                                {
                                    if (fileTokens[i + 1].TokenValue.Length != 1 && _configuration.Other.Spaces.AfterColon)
                                        verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber + 1, ErrorMessage = "Between ':' and attribute value must be only one white space!", ErrorType = VerificationErrorTypes.AlignValuesError });

                                }
                            }

                            if (alignValuesType == AlignValuesTypes.OnValue)
                            {
                                if (fileTokens[i + 1].TokenType != TokenTypes.WhiteSpace)
                                {
                                    if (fileTokens[i + 1].Position != maxStartIndex)
                                    {
                                        var offset = maxStartIndex - fileTokens[i + 1].Position;

                                        var message = $"Between ':' and attribute value must be more on {offset} white spaces!";

                                        verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber + 1, ErrorMessage = message, ErrorType = VerificationErrorTypes.AlignValuesError });

                                    }
                                }
                                else
                                {
                                    if (fileTokens[i + 2].Position != maxStartIndex)
                                    {
                                        var offset = maxStartIndex - fileTokens[i + 2].Position;
                                        var message = $"Between ':' and attribute value must be more on {offset} white spaces!";
                                        verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber + 1, ErrorMessage = message, ErrorType = VerificationErrorTypes.AlignValuesError });

                                    }
                                }
                            }
                            if (alignValuesType == AlignValuesTypes.OnColon)
                            {
                                int identifierPosition = 0;
                                if (fileTokens[i + 1].TokenType != TokenTypes.WhiteSpace && _configuration.Other.Spaces.AfterColon)
                                {
                                    verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber + 1, ErrorMessage = "Between ':' and attribute value must be a white space!", ErrorType = VerificationErrorTypes.AlignValuesError });
                                    identifierPosition = fileTokens[i + 1].Position;
                                }
                                else
                                {
                                    if(fileTokens[i+1].TokenValue.Length >1 && _configuration.Other.Spaces.AfterColon)
                                        verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber + 1, ErrorMessage = "Between ':' and attribute value must be only one white space!", ErrorType = VerificationErrorTypes.AlignValuesError });

                                    identifierPosition = fileTokens[i + 2].Position;
                                }

                                if(fileTokens[i-1].TokenType!= TokenTypes.WhiteSpace)
                                {
                                    verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber + 1, ErrorMessage = "Between ':' and attribute value must be at least one white space!", ErrorType = VerificationErrorTypes.AlignValuesError });
                                }

                                if (identifierPosition != maxStartIndex)
                                {
                                    var offset = maxStartIndex - identifierPosition;
                                    var message = $"Between attribute and ':' must be more on {offset} white spaces!";
                                    verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber + 1, ErrorMessage = message, ErrorType = VerificationErrorTypes.AlignValuesError });
                                }

                            }
                        }
                        i++;
                    }
                }
            }
        }

        private void QuoteMarksValidation(List<Token> fileTokens)
        {
            var stringTokens = fileTokens.Where(token => token.TokenType == TokenTypes.String).ToList();
            var quotesMarksType = _configuration.Other.QuoteMarks;
            for(int i = 0; i < stringTokens.Count; ++i)
            {
                if(quotesMarksType == QuoteMarksTypes.Single && (!stringTokens[i].TokenValue.StartsWith('\'') && !stringTokens[i].TokenValue.EndsWith('\'')))
                {
                    verificationErrors.Add(new VerificationError { ErrorLineNumber = stringTokens[i].LineNumber +1, ErrorMessage = "String value should be wrapped in single quotes", ErrorType = VerificationErrorTypes.QuoteMarksError });
                }

                if (quotesMarksType == QuoteMarksTypes.Double && (!stringTokens[i].TokenValue.StartsWith('\"') && !stringTokens[i].TokenValue.EndsWith('\"')))
                {
                    verificationErrors.Add(new VerificationError { ErrorLineNumber = stringTokens[i].LineNumber +1, ErrorMessage = "String value should be wrapped in double quotes", ErrorType = VerificationErrorTypes.QuoteMarksError });
                }
            }
        }

        private void ClosingBracketsValidation(List<Token> fileTokens)
        {
            var alignClosingBracketsWithProperies = _configuration.Other.AlignClosingBraceWithProperties;

            for(int i = 1; i < fileTokens.Count; ++i)
            {
                if(fileTokens[i].TokenValue == "}")
                {
                    if (alignClosingBracketsWithProperies)
                    {
                        if (fileTokens[i - 1].TokenType == TokenTypes.NewLine)
                        {
                            verificationErrors.Add(new VerificationError { ErrorLineNumber = fileTokens[i].LineNumber + 1, ErrorMessage = "You should insert indent before closing bracket", ErrorType = VerificationErrorTypes.ClosingBraceAligmentError });
                        }
                    }
                    else
                    {
                        if(fileTokens[i-1].TokenType != TokenTypes.NewLine)
                        {
                            verificationErrors.Add(new VerificationError { ErrorLineNumber = fileTokens[i].LineNumber + 1, ErrorMessage = "You should delete indent before closing bracket", ErrorType = VerificationErrorTypes.ClosingBraceAligmentError });
                        }
                    }
                }
            }
        }

        private void SingleLineBlocksValidation(List<Token> fileTokens)
        {
            var keepSingleLineBlocks = _configuration.Other.KeepSingleLineBlocks;

            for(int i = 0; i < fileTokens.Count-1; ++i)
            {
                if(fileTokens[i].TokenValue == "{")
                {
                    var openBracketLineNumber = fileTokens[i].LineNumber +1;
                    var openBracketIndex = i;
                    var attributesCount = 0;
                    i++;
                    while(fileTokens[i].TokenValue!="}" && i < fileTokens.Count)
                    {
                        if (fileTokens[i].TokenValue == ":")
                            attributesCount++;
                        i++;
                    }
                    if(attributesCount == 1)
                    {
                        if(fileTokens[openBracketIndex+1].TokenType!=TokenTypes.NewLine && !keepSingleLineBlocks)
                        {
                            verificationErrors.Add(new VerificationError { ErrorLineNumber = openBracketLineNumber, ErrorMessage = "You should not keep single line block", ErrorType = VerificationErrorTypes.SingleLineBlockError });
                        }
                        if(fileTokens[openBracketIndex+1].TokenType==TokenTypes.NewLine && keepSingleLineBlocks)
                        {
                            verificationErrors.Add(new VerificationError { ErrorLineNumber = openBracketLineNumber, ErrorMessage = "You should keep single line block", ErrorType = VerificationErrorTypes.SingleLineBlockError });
                        }
                    }
                }
            }
        }

        private void SpacesValidation(List<Token> fileTokens)
        {
            var spacesAfterColon = _configuration.Other.Spaces.AfterColon;
            var spaceBeforeOpeningBracket = _configuration.Other.Spaces.BeforeOpeningBrace;
            bool isBracketOpened = false; 
            for(int i = 0; i < fileTokens.Count-1; ++i)
            {
                if (fileTokens[i].TokenValue == "{")
                    isBracketOpened = true;

                if (fileTokens[i].TokenValue == "}")
                    isBracketOpened = false;

                if (fileTokens[i].TokenValue == ":" && spacesAfterColon && isBracketOpened)
                {
                    if (fileTokens[i + 1].TokenType != TokenTypes.WhiteSpace)
                        verificationErrors.Add(new VerificationError { ErrorLineNumber = fileTokens[i].LineNumber + 1, ErrorMessage = "You should place space after colon", ErrorType = VerificationErrorTypes.SpaceAfterColonError });

                }

                if (_configuration.Other.BracesPlacement == BracesPlacementTypes.EndOfLine)
                {
                    if (fileTokens[i].TokenValue == "{" && spaceBeforeOpeningBracket)
                    {
                        if (fileTokens[i - 1].TokenType != TokenTypes.WhiteSpace)
                            verificationErrors.Add(new VerificationError { ErrorLineNumber = fileTokens[i].LineNumber + 1, ErrorMessage = "You should place space before openning space", ErrorType = VerificationErrorTypes.SpaceBeforeOpeningBracketError });

                    }
                }
            }
        }
        
        private void HexColorsValidation(List<Token> fileTokens)
        {
            var hexColorLowerCase = _configuration.Other.HexColors.ConvertHexColorsToLowerCase;
            var hexColorsLongFormat = _configuration.Other.HexColors.ConvertHexColorsFormatToLongFormat;
            bool isBracketOpened = false;

            for(int i = 0; i < fileTokens.Count -1; ++i)
            {
                if (fileTokens[i].TokenValue == "{")
                    isBracketOpened = true;

                else if (fileTokens[i].TokenValue == "}")
                    isBracketOpened = false;

                if(fileTokens[i].TokenValue == "#" && isBracketOpened)
                {
                    var hexColor = fileTokens[i + 1].TokenValue;
                    if (hexColor.Length == 6 && !hexColorsLongFormat)
                        verificationErrors.Add(new VerificationError { ErrorLineNumber = fileTokens[i].LineNumber + 1, ErrorMessage = "Hex collor should be in short format", ErrorType = VerificationErrorTypes.HexColorFormatError });
                    else if(hexColor.Length == 3 && hexColorsLongFormat)
                        verificationErrors.Add(new VerificationError { ErrorLineNumber = fileTokens[i].LineNumber + 1, ErrorMessage = "Hex collor should be in long format", ErrorType = VerificationErrorTypes.HexColorFormatError });

                    if(hexColorLowerCase && !IsStringInLowerCase(hexColor))
                        verificationErrors.Add(new VerificationError { ErrorLineNumber = fileTokens[i].LineNumber + 1, ErrorMessage = "Hex collor should be in lower case", ErrorType = VerificationErrorTypes.HexColorCaseError });

                    else if (!hexColorLowerCase && !IsStringInUpperCase(hexColor))
                        verificationErrors.Add(new VerificationError { ErrorLineNumber = fileTokens[i].LineNumber + 1, ErrorMessage = "Hex collor should be in upper case", ErrorType = VerificationErrorTypes.HexColorCaseError });

                }
            }
        }

        private int GetMaximalStartIndexOfAttributeValueInBlock(List<Token> fileTokens, int startBlockIndex)
        {
            int i = startBlockIndex;
            var maximumIndex = 0;
            while (i < fileTokens.Count - 2 && fileTokens[i].TokenValue != "}")
            {

                if (fileTokens[i].TokenValue == ":")
                {
                    var tabsCount = GetCountOfTabsInLineBeforeAttributeValue(fileTokens, fileTokens[i]);
                    var tabsOffset = tabsCount == 0 ? (tabsCount * _configuration.TabsAndIndents.TabSize - 1) : 0;
                    if (fileTokens[i + 1].TokenType == TokenTypes.WhiteSpace)
                    {
                        if (fileTokens[i + 2].Position - tabsOffset > maximumIndex)
                            maximumIndex = fileTokens[i + 2].Position - tabsOffset;
                    }
                    else
                    {
                        if (fileTokens[i + 1].Position - tabsOffset > maximumIndex)
                            maximumIndex = fileTokens[i + 1].Position - tabsOffset;
                    }
                }

                i++;
            }

            return maximumIndex;
        }
        private int GetCountOfTabsInLineBeforeAttributeValue(List<Token> fileTokens, Token token)
        {
            return fileTokens.Where(x => x.LineNumber == token.LineNumber && x.TokenType == TokenTypes.Tab && x.Position < token.Position).Count();
        }

        private bool IsStringInLowerCase(string value)
        {
            for(int i = 0; i< value.Length; ++i)
            {
                if (!Char.IsLower(value[i]))
                    return false;
            }
            return true;
        }

        private bool IsStringInUpperCase(string value)
        {
            for (int i = 0; i < value.Length; ++i)
            {
                if (!Char.IsUpper(value[i]))
                    return false;
            }
            return true;
        }
    }
}
