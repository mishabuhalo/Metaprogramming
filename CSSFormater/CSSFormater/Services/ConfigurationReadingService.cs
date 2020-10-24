using CSSFormater.Exceptions;
using CSSFormater.FormaterConfigurationModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace CSSFormater.Services
{
    public static class ConfigurationReadingService
    {
        private static string ConfigurationFolderName = "Configuration";
        private static string ConfigurationFileName = "config.json";
        private readonly static List<string> BracesPlacement = new List<string>() { "End of line", "Next line" };
        private readonly static List<string> AlignValues = new List<string>() { "Do not align", "On colon", "On value" };
        private readonly static List<string> QuoteMarks = new List<string>() { "Double", "Single"};

        public static Configuration ReadConfiguration()
        {
            var json = File.ReadAllText($"../../../{ConfigurationFolderName}/{ConfigurationFileName}");

            Configuration configuration = null;
            try
            {
                configuration = JsonSerializer.Deserialize<Configuration>(json);
            }
            catch(Exception e)
            {
                if(e is ArgumentNullException)
                    throw new ConfigurationConstructionException("Configuration file is missing!");
                else if (e is JsonException)
                    throw new ConfigurationConstructionException("The JSON is invalid!");
                else if (e is NotSupportedException)
                    throw new ConfigurationConstructionException("Invalid configuration file!");
            }

            ValidateConfiguration(configuration);

            return configuration;
        }


        private static void ValidateConfiguration(Configuration configuration)
        {
            if (!configuration.TabsAndIndents.UseTabCharacter && configuration.TabsAndIndents.SmartTabs)
                throw new ConfigurationConstructionException("Invalid configuration file. Smart tabs can't be true if Use tab characters is false");
            if (!BracesPlacement.Contains(configuration.Other.BracesPlacement))
            {
                string allBracesPlacements = "";
                BracesPlacement.Select(x => allBracesPlacements += $"{x}\n").Count();
                throw new ConfigurationConstructionException($"Invalid configuration file. Braces placement property must be one of the following:\n{allBracesPlacements}");
            }

            if(!AlignValues.Contains(configuration.Other.AlignValues))
            {
                string allAlignValues = "";
                AlignValues.Select(x => allAlignValues += $"{x}\n").Count();
                throw new ConfigurationConstructionException($"Invalid configuration file. Align values property must be one of the following:\n{allAlignValues}");
            }

            if (!QuoteMarks.Contains(configuration.Other.QuoteMarks))
            {
                string allQuoteMarks = "";
                QuoteMarks.Select(x => allQuoteMarks += $"{x}\n").Count();
                throw new ConfigurationConstructionException($"Invalid configuration file. Quote marks property must be one of the following:\n{allQuoteMarks}");
            }

            if(configuration.Other.HexColors.ConvertHexColorsToLowerCase && configuration.Other.HexColors.ConvertHexColorsToUpperCase)
            {
                throw new ConfigurationConstructionException($"Invalid configuration file. Converting hex colors to upper case and lower case can't be true at one time, please set true only one of this options.");
            }

            if (configuration.Other.HexColors.ConvertHexColorsFormatToLongFormat && configuration.Other.HexColors.ConvertHexColorsFormatToShortFormat)
            {
                throw new ConfigurationConstructionException($"Invalid configuration file. Converting hex colors format to short and long formats can't be true at one time, please set true only one of this options.");
            }

            if(configuration.Arrangement.SortCssPropertiesByCustomOrder && configuration.Arrangement.SortCssPropertiesByName)
            {
                throw new ConfigurationConstructionException($"Invalid configuration file. Sorting CSS properties by name and custom value cant't be true at one time, please set true only one of this options.");
            }

        }

    }
}
