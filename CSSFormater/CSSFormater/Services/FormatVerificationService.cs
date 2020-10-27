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
        private List<Token> _tokens;

        public FormatVerificationService(Configuration configuration, List<Token> tokens)
        {
            _configuration = configuration;
            verificationErrors = new List<VerificationError>();
            _tokens = tokens;
        }
        public void VerifyTokens(bool shouldFormat = false)
        {
            TabsAndIndentsVerification(shouldFormat);
            BlankLinesValidation(shouldFormat);
            BracesPlacementValidation( shouldFormat);
            AlignValuesValidation(shouldFormat);
            QuoteMarksValidation(shouldFormat);
            ClosingBracketsValidation(shouldFormat);
            SingleLineBlocksValidation(shouldFormat);
            SpacesValidation(shouldFormat);
            HexColorsValidation(shouldFormat);

            verificationErrors = verificationErrors.OrderBy(x => x.ErrorLineNumber).ToList();
        }

        public List<VerificationError> GetVerificationErrors()
        {
            return verificationErrors;
        }

        private void TabsAndIndentsVerification(bool shouldFormat = false)
        {
            int i = 0;
            var useTabCharacter = _configuration.TabsAndIndents.UseTabCharacter;
            var useSmartTabs = _configuration.TabsAndIndents.SmartTabs;
            var tabSize = _configuration.TabsAndIndents.TabSize;
            var indentSize = _configuration.TabsAndIndents.Indent;
            var keepIndentsOnEmptyLines = _configuration.TabsAndIndents.KeepIndentsOnEmtyLines;
            var neededTabsCount = indentSize == tabSize ? 1 : indentSize % tabSize;


            for (; i < _tokens.Count - 1; ++i)
            {
                if (_tokens[i].TokenValue == "{")
                {
                    i++;
                    while (i < _tokens.Count - 2 && _tokens[i + 1].TokenValue != "}")
                    {
                        if (_tokens[i].TokenType == TokenTypes.NewLine)
                        {
                            i++;
                            var lineNumber = _tokens[i].LineNumber + 1;
                            var currentIndent = 0;
                            var tabsCount = 0;
                            while (_tokens[i].TokenType == TokenTypes.Tab || _tokens[i].TokenType == TokenTypes.WhiteSpace && i < _tokens.Count - 1)
                            {
                                if (_tokens[i].TokenType == TokenTypes.Tab)
                                {
                                    if (!useTabCharacter)
                                    {
                                        if (shouldFormat)
                                        {
                                            _tokens[i].TokenType = TokenTypes.WhiteSpace;
                                            _tokens[i].TokenValue = new string(' ', tabSize);
                                        }
                                        verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = "You should use white spaces instead of tabs.", ErrorType = VerificationErrorTypes.TabsAndIndentsError });
                                    }
                                    currentIndent += tabSize;
                                    tabsCount++;
                                }
                                else
                                {
                                    if (_tokens[i].TokenValue.Length == tabSize && useSmartTabs)
                                    {
                                        if (shouldFormat)
                                        {
                                            _tokens[i].TokenType = TokenTypes.Tab;
                                        }
                                        verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = $"Here should be tab instead of {tabSize} white spaces", ErrorType = VerificationErrorTypes.TabsAndIndentsError });
                                    }
                                    currentIndent += _tokens[i].TokenValue.Length;
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
                                if (shouldFormat)
                                {
                                    _tokens[i - 1].TokenValue = new string(' ', indentSize);
                                    _tokens[i - 1].TokenType = TokenTypes.WhiteSpace;

                                }
                                verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = $"Indent is bigger then should be on {currentIndent - indentSize}.", ErrorType = VerificationErrorTypes.TabsAndIndentsError });
                            }
                            else
                            {
                                if (shouldFormat)
                                {
                                    _tokens[i - 1].TokenValue = new string(' ', indentSize);
                                    _tokens[i - 1].TokenType = TokenTypes.WhiteSpace;
                                }
                                verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = $"Indent is less then should be on {indentSize - currentIndent}.", ErrorType = VerificationErrorTypes.TabsAndIndentsError });
                            }

                            if (_tokens[i].TokenType == TokenTypes.NewLine)
                            {
                                if (!keepIndentsOnEmptyLines)
                                {
                                    if (shouldFormat)
                                    {
                                        _tokens.RemoveAt(i - 1);
                                        i--;
                                    }
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

        private void BlankLinesValidation(bool shouldFormat = false)
        {
            var maximumBlankLinesInCode = _configuration.BlankLines.MaximumBlankLinesInCode;
            var minimumBlankLinesAroundTopLevelBlocks = _configuration.BlankLines.MinimumBlankLinesAroundTopLevelBlock;

            var lastClosedBracketToken = _tokens.Where(x => x.TokenValue == "}").Last();

            var lastIndexOfClosingBracket = _tokens.LastIndexOf(lastClosedBracketToken);

            var countOfNextEmptyLines = 0;
            for (int i = lastIndexOfClosingBracket; i < _tokens.Count - 1; ++i)
            {
                if (_tokens[i].TokenType == TokenTypes.NewLine)
                    countOfNextEmptyLines++;
            }
            if (maximumBlankLinesInCode < countOfNextEmptyLines - 1)
            {
                if (shouldFormat)
                {
                    var countOfEmptyLinesToDelete = countOfNextEmptyLines - maximumBlankLinesInCode;
                    _tokens.RemoveRange(lastIndexOfClosingBracket + 1, countOfNextEmptyLines);
                }
                verificationErrors.Add(new VerificationError { ErrorLineNumber = lastClosedBracketToken.LineNumber + 1, ErrorMessage = $"You exceeded the limit of extra blank lines to be kept on {countOfNextEmptyLines - maximumBlankLinesInCode}.", ErrorType = VerificationErrorTypes.BlankLinesError });
            }
            var j = 0;
            for (; j < _tokens.Count - 1; ++j)
            {
                if (_tokens[j].TokenValue == "}" && j != lastIndexOfClosingBracket)
                {
                    j++;
                    var lineNumber = _tokens[j + 1].LineNumber;
                    var currentBlankLinesCount = 0;
                    while (_tokens[j].TokenValue != "{" && j < _tokens.Count - 1)
                    {
                        if (_tokens[j].TokenType == TokenTypes.NewLine)
                        {
                            currentBlankLinesCount++;
                        }
                        j++;
                    }

                    if (currentBlankLinesCount - 1 < minimumBlankLinesAroundTopLevelBlocks)
                    {
                        verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber, ErrorMessage = $"You ddid not reach the minimum number of blank lines around top level blocks on {minimumBlankLinesAroundTopLevelBlocks - currentBlankLinesCount + 1}.", ErrorType = VerificationErrorTypes.BlankLinesError });
                        if(shouldFormat)
                        {
                            for (int k = 0; k < minimumBlankLinesAroundTopLevelBlocks - currentBlankLinesCount; k++)
                                _tokens.Insert(j + 1, new Token { TokenType = TokenTypes.NewLine, TokenValue = "\n" });
                        }
                    }
                }
            }
        }

        private void BracesPlacementValidation(bool shouldFormat = false)
        {
            var bracesPlacementType = _configuration.Other.BracesPlacement;

            for (int i = 1; i < _tokens.Count - 1; ++i)
            {
                if (_tokens[i].TokenValue == "{")
                {
                    if (_tokens[i - 1].TokenType == TokenTypes.NewLine && bracesPlacementType != BracesPlacementTypes.NextLine)
                    {
                        if(shouldFormat)
                        {
                            _tokens.RemoveAt(i - 1);
                            i--;
                        }
                        verificationErrors.Add(new VerificationError { ErrorLineNumber = _tokens[i].LineNumber, ErrorMessage = "You should place bracket at the end of line", ErrorType = VerificationErrorTypes.BracesReplacementError });
                    }
                    if (_tokens[i - 1].TokenType != TokenTypes.NewLine && bracesPlacementType != BracesPlacementTypes.EndOfLine)
                    {
                        if(shouldFormat)
                        {
                            _tokens.Insert(i - 1, new Token { TokenType = TokenTypes.NewLine, TokenValue = "\n" });
                        }
                        verificationErrors.Add(new VerificationError { ErrorLineNumber = _tokens[i].LineNumber + 1, ErrorMessage = "You should place bracket at the next line", ErrorType = VerificationErrorTypes.BracesReplacementError });
                    }
                }
            }
        }

        private void AlignValuesValidation(bool shouldFormat = false)
        {
            var alignValuesType = _configuration.Other.AlignValues;

            int i = 0;

            for (; i < _tokens.Count - 1; ++i)
            {
                if (_tokens[i].TokenValue == "{")
                {
                    i++;
                    var maxStartIndex = GetMaximalStartIndexOfAttributeValueInBlock(i);
                    while (_tokens[i].TokenValue != "}" && i < _tokens.Count)
                    {
                        if (_tokens[i].TokenValue == ":")
                        {
                            var lineNumber = _tokens[i].LineNumber;

                            if (alignValuesType == AlignValuesTypes.DoNotAlign)
                            {
                                if (_tokens[i + 1].TokenType != TokenTypes.WhiteSpace && _configuration.Other.Spaces.AfterColon)
                                {
                                    if(shouldFormat)
                                    {
                                        _tokens.Insert(i + 1, new Token { TokenType = TokenTypes.WhiteSpace, TokenValue = " " });
                                    }
                                    verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber + 1, ErrorMessage = "Between ':' and attribute value must be a white space!", ErrorType = VerificationErrorTypes.AlignValuesError });
                                }
                                else
                                {
                                    if (_tokens[i + 1].TokenValue.Length != 1 && _configuration.Other.Spaces.AfterColon)
                                    {
                                        if(shouldFormat)
                                        {
                                            _tokens[i + 1].TokenValue.Substring(1);
                                        }
                                        verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber + 1, ErrorMessage = "Between ':' and attribute value must be only one white space!", ErrorType = VerificationErrorTypes.AlignValuesError });
                                    }

                                }
                            }

                            if (alignValuesType == AlignValuesTypes.OnValue)
                            {
                                if (_tokens[i + 1].TokenType != TokenTypes.WhiteSpace)
                                {
                                    if (_tokens[i + 1].Position != maxStartIndex)
                                    {
                                        var offset = maxStartIndex - _tokens[i + 1].Position;

                                        var message = $"Between ':' and attribute value must be more on {offset} white spaces!";

                                        if(shouldFormat)
                                        {
                                            _tokens.Insert(i + 1, new Token { TokenType = TokenTypes.WhiteSpace, TokenValue = new string(' ', offset) });
                                        }

                                        verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber + 1, ErrorMessage = message, ErrorType = VerificationErrorTypes.AlignValuesError });

                                    }
                                }
                                else
                                {
                                    if (_tokens[i + 2].Position != maxStartIndex)
                                    {
                                        var offset = maxStartIndex - _tokens[i + 2].Position;
                                        var message = $"Between ':' and attribute value must be more on {offset} white spaces!";
                                        if(shouldFormat)
                                        {
                                            _tokens.Insert(i + 2, new Token { TokenType = TokenTypes.WhiteSpace, TokenValue = new string(' ', offset) });
                                        }
                                        verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber + 1, ErrorMessage = message, ErrorType = VerificationErrorTypes.AlignValuesError });

                                    }
                                }
                            }
                            if (alignValuesType == AlignValuesTypes.OnColon)
                            {
                                int identifierPosition = 0;
                                var identifireIndex = 0;
                                if (_tokens[i + 1].TokenType != TokenTypes.WhiteSpace && _configuration.Other.Spaces.AfterColon)
                                {
                                    if (shouldFormat)
                                    {
                                        _tokens.Insert(i + 1, new Token { TokenType = TokenTypes.WhiteSpace, TokenValue = " " });
                                    }
                                    verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber + 1, ErrorMessage = "Between ':' and attribute value must be a white space!", ErrorType = VerificationErrorTypes.AlignValuesError });
                                    identifierPosition = _tokens[i + 1].Position;
                                    identifireIndex = i + 1;
                                }
                                else
                                {
                                    if (_tokens[i + 1].TokenValue.Length > 1 && _configuration.Other.Spaces.AfterColon)
                                    {
                                        if (shouldFormat)
                                        {
                                            _tokens[i + 1].TokenValue.Substring(1);
                                        }

                                        verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber + 1, ErrorMessage = "Between ':' and attribute value must be only one white space!", ErrorType = VerificationErrorTypes.AlignValuesError });
                                    }
                                    identifierPosition = _tokens[i + 2].Position;
                                    identifireIndex = i + 2;
                                }

                                if (_tokens[i - 1].TokenType != TokenTypes.WhiteSpace)
                                {
                                    if (shouldFormat)
                                    {
                                        _tokens.Insert(i - 1, new Token { TokenType = TokenTypes.WhiteSpace, TokenValue = " " });
                                        i++;
                                    }
                                    verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber + 1, ErrorMessage = "Between ':' and attribute value must be at least one white space!", ErrorType = VerificationErrorTypes.AlignValuesError });
                                }

                                if (identifierPosition != maxStartIndex)
                                {
                                    var offset = maxStartIndex - identifierPosition;
                                    var message = $"Between attribute and ':' must be more on {offset} white spaces!";

                                    if (shouldFormat)
                                    {
                                        _tokens.Insert(identifireIndex - 1, new Token { TokenType = TokenTypes.WhiteSpace, TokenValue = new string(' ', offset) });
                                    }
                                    verificationErrors.Add(new VerificationError { ErrorLineNumber = lineNumber + 1, ErrorMessage = message, ErrorType = VerificationErrorTypes.AlignValuesError });
                                }

                            }
                        }
                        i++;
                    }
                }
            }
        }

        private void QuoteMarksValidation(bool shouldFormat = false)
        {
            var stringTokens = _tokens.Where(token => token.TokenType == TokenTypes.String).ToList();
            var quotesMarksType = _configuration.Other.QuoteMarks;
            for (int i = 0; i < stringTokens.Count; ++i)
            {
                if (quotesMarksType == QuoteMarksTypes.Single && (!stringTokens[i].TokenValue.StartsWith('\'') && !stringTokens[i].TokenValue.EndsWith('\'')))
                {
                    if(shouldFormat)
                    {
                        _tokens[i].TokenValue.Replace('\"', '\'');
                    }
                    verificationErrors.Add(new VerificationError { ErrorLineNumber = stringTokens[i].LineNumber + 1, ErrorMessage = "String value should be wrapped in single quotes", ErrorType = VerificationErrorTypes.QuoteMarksError });
                }

                if (quotesMarksType == QuoteMarksTypes.Double && (!stringTokens[i].TokenValue.StartsWith('\"') && !stringTokens[i].TokenValue.EndsWith('\"')))
                {
                    if (shouldFormat)
                    {
                        _tokens[i].TokenValue.Replace('\'', '\"');
                    }
                    verificationErrors.Add(new VerificationError { ErrorLineNumber = stringTokens[i].LineNumber + 1, ErrorMessage = "String value should be wrapped in double quotes", ErrorType = VerificationErrorTypes.QuoteMarksError });
                }
            }
        }

        private void ClosingBracketsValidation(bool shouldFormat = false)
        {
            var alignClosingBracketsWithProperies = _configuration.Other.AlignClosingBraceWithProperties;

            for (int i = 1; i < _tokens.Count; ++i)
            {
                if (_tokens[i].TokenValue == "}")
                {
                    if (alignClosingBracketsWithProperies)
                    {
                        if (_tokens[i - 1].TokenType == TokenTypes.NewLine)
                        {
                            if(shouldFormat)
                            {
                                _tokens.Insert(i - 1, new Token { TokenType = TokenTypes.WhiteSpace, TokenValue =  new string(' ', _configuration.TabsAndIndents.Indent) });
                            }
                            verificationErrors.Add(new VerificationError { ErrorLineNumber = _tokens[i].LineNumber + 1, ErrorMessage = "You should insert indent before closing bracket", ErrorType = VerificationErrorTypes.ClosingBraceAligmentError });
                        }
                    }
                    else
                    {
                        if (_tokens[i - 1].TokenType != TokenTypes.NewLine)
                        {
                            if(shouldFormat)
                            {
                                _tokens.RemoveAt(i - 1);
                                i--;
                            }
                            verificationErrors.Add(new VerificationError { ErrorLineNumber = _tokens[i].LineNumber + 1, ErrorMessage = "You should delete indent before closing bracket", ErrorType = VerificationErrorTypes.ClosingBraceAligmentError });
                        }
                    }
                }
            }
        }

        private void SingleLineBlocksValidation(bool shouldFormat = false)
        {
            var keepSingleLineBlocks = _configuration.Other.KeepSingleLineBlocks;

            for (int i = 0; i < _tokens.Count - 1; ++i)
            {
                if (_tokens[i].TokenValue == "{")
                {
                    var openBracketLineNumber = _tokens[i].LineNumber + 1;
                    var openBracketIndex = i;
                    var attributesCount = 0;
                    i++;
                    while (_tokens[i].TokenValue != "}" && i < _tokens.Count)
                    {
                        if (_tokens[i].TokenValue == ":")
                            attributesCount++;
                        i++;
                    }
                    if (attributesCount == 1)
                    {
                        if (_tokens[openBracketIndex + 1].TokenType != TokenTypes.NewLine && !keepSingleLineBlocks)
                        {
                            if(shouldFormat)
                            {
                                _tokens.Insert(openBracketIndex + 1, new Token { TokenType = TokenTypes.NewLine, TokenValue = "\n" });
                            }
                            verificationErrors.Add(new VerificationError { ErrorLineNumber = openBracketLineNumber, ErrorMessage = "You should not keep single line block", ErrorType = VerificationErrorTypes.SingleLineBlockError });
                        }
                        if (_tokens[openBracketIndex + 1].TokenType == TokenTypes.NewLine && keepSingleLineBlocks)
                        {
                            if(shouldFormat)
                            {
                                _tokens.RemoveAt(i - 1);
                                i--;
                            }
                            verificationErrors.Add(new VerificationError { ErrorLineNumber = openBracketLineNumber, ErrorMessage = "You should keep single line block", ErrorType = VerificationErrorTypes.SingleLineBlockError });
                        }
                    }
                }
            }
        }

        private void SpacesValidation(bool shouldFormat = false)
        {
            var spacesAfterColon = _configuration.Other.Spaces.AfterColon;
            var spaceBeforeOpeningBracket = _configuration.Other.Spaces.BeforeOpeningBrace;
            bool isBracketOpened = false;
            for (int i = 0; i < _tokens.Count - 1; ++i)
            {
                if (_tokens[i].TokenValue == "{")
                    isBracketOpened = true;

                if (_tokens[i].TokenValue == "}")
                    isBracketOpened = false;

                if (_tokens[i].TokenValue == ":" && spacesAfterColon && isBracketOpened)
                {
                    if (_tokens[i + 1].TokenType != TokenTypes.WhiteSpace)
                    {
                        if(shouldFormat)
                        {
                            _tokens.Insert(i + 1, new Token { TokenType = TokenTypes.WhiteSpace, TokenValue = " " });
                        }
                        verificationErrors.Add(new VerificationError { ErrorLineNumber = _tokens[i].LineNumber + 1, ErrorMessage = "You should place space after colon", ErrorType = VerificationErrorTypes.SpaceAfterColonError });
                    }

                }

                if (_configuration.Other.BracesPlacement == BracesPlacementTypes.EndOfLine)
                {
                    if (_tokens[i].TokenValue == "{" && spaceBeforeOpeningBracket)
                    {
                        if (_tokens[i - 1].TokenType != TokenTypes.WhiteSpace)
                        {
                            if(shouldFormat)
                            {
                                _tokens.Insert(i - 1, new Token { TokenType = TokenTypes.WhiteSpace, TokenValue = " " });
                            }
                            verificationErrors.Add(new VerificationError { ErrorLineNumber = _tokens[i].LineNumber + 1, ErrorMessage = "You should place space before openning space", ErrorType = VerificationErrorTypes.SpaceBeforeOpeningBracketError });
                        }

                    }
                }
            }
        }

        private void HexColorsValidation(bool shouldFormat = false)
        {
            var hexColorLowerCase = _configuration.Other.HexColors.ConvertHexColorsToLowerCase;
            var hexColorsLongFormat = _configuration.Other.HexColors.ConvertHexColorsFormatToLongFormat;
            bool isBracketOpened = false;

            for (int i = 0; i < _tokens.Count - 1; ++i)
            {
                if (_tokens[i].TokenValue == "{")
                    isBracketOpened = true;

                else if (_tokens[i].TokenValue == "}")
                    isBracketOpened = false;

                if (_tokens[i].TokenValue == "#" && isBracketOpened)
                {
                    var hexColor = _tokens[i + 1].TokenValue;
                    if (hexColor.Length == 6 && !hexColorsLongFormat)
                    {
                        verificationErrors.Add(new VerificationError { ErrorLineNumber = _tokens[i].LineNumber + 1, ErrorMessage = "Hex collor should be in short format", ErrorType = VerificationErrorTypes.HexColorFormatError });
                    }
                    else if (hexColor.Length == 3 && hexColorsLongFormat)
                    {
                        verificationErrors.Add(new VerificationError { ErrorLineNumber = _tokens[i].LineNumber + 1, ErrorMessage = "Hex collor should be in long format", ErrorType = VerificationErrorTypes.HexColorFormatError });
                    }

                    if (hexColorLowerCase && !IsStringInLowerCase(hexColor))
                    {
                        if(shouldFormat)
                        {
                            _tokens[i].TokenValue.ToLower();
                        }
                        verificationErrors.Add(new VerificationError { ErrorLineNumber = _tokens[i].LineNumber + 1, ErrorMessage = "Hex collor should be in lower case", ErrorType = VerificationErrorTypes.HexColorCaseError });
                    }
                    else if (!hexColorLowerCase && !IsStringInUpperCase(hexColor))
                    {
                        if(shouldFormat)
                        {
                            _tokens[i].TokenValue.ToUpper();
                        }
                        verificationErrors.Add(new VerificationError { ErrorLineNumber = _tokens[i].LineNumber + 1, ErrorMessage = "Hex collor should be in upper case", ErrorType = VerificationErrorTypes.HexColorCaseError });
                    }
                }
            }
        }

        private int GetMaximalStartIndexOfAttributeValueInBlock(int startBlockIndex)
        {
            int i = startBlockIndex;
            var maximumIndex = 0;
            while (i < _tokens.Count - 2 && _tokens[i].TokenValue != "}")
            {

                if (_tokens[i].TokenValue == ":")
                {
                    var tabsCount = GetCountOfTabsInLineBeforeAttributeValue(_tokens[i]);
                    var tabsOffset = tabsCount == 0 ? (tabsCount * _configuration.TabsAndIndents.TabSize - 1) : 0;
                    if (_tokens[i + 1].TokenType == TokenTypes.WhiteSpace)
                    {
                        if (_tokens[i + 2].Position - tabsOffset > maximumIndex)
                            maximumIndex = _tokens[i + 2].Position - tabsOffset;
                    }
                    else
                    {
                        if (_tokens[i + 1].Position - tabsOffset > maximumIndex)
                            maximumIndex = _tokens[i + 1].Position - tabsOffset;
                    }
                }

                i++;
            }

            return maximumIndex;
        }
        private int GetCountOfTabsInLineBeforeAttributeValue(Token token)
        {
            return _tokens.Where(x => x.LineNumber == token.LineNumber && x.TokenType == TokenTypes.Tab && x.Position < token.Position).Count();
        }

        private bool IsStringInLowerCase(string value)
        {
            for (int i = 0; i < value.Length; ++i)
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
