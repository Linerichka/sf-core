using System.IO;
using System.Text;
using SFramework.Core.Runtime;
using UnityEngine;

namespace SFramework.Core.Editor
{
    public class SFDatabaseCodeGenerator
    {
        private const string characters_template = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
        private const string numbers_template = "0123456789";
        private const string symbols_template = ".-_$#@*()[]{}+:?!&',^=<>~`";
        private const int max_characters = 256;


        public void Generate(SFGenerationData[] generationDatas)
        {
            foreach (var generationData in generationDatas)
            {
                _Generate(generationData);
            }
        }

        private void _Generate(SFGenerationData generationData)
        {
            var sbTrans = new StringBuilder();

            sbTrans.AppendLine();
            sbTrans.AppendLine("namespace SFramework.Generated");
            sbTrans.AppendLine("{");
            sbTrans.AppendLine($"{indents(1)}public static class {generationData.FileName}"); //Filename
            sbTrans.AppendLine($"{indents(1)}{{");

            foreach (var property in generationData.Properties)
            {
                var propertyName = formatPropertyName(property.Replace("/", "__"));

                if (string.IsNullOrWhiteSpace(propertyName)) continue;

                sbTrans.AppendLine($"{indents(2)}public static readonly string {propertyName} = \"{property}\";");
            }

            sbTrans.AppendLine($"{indents(1)}}}");
            sbTrans.AppendLine("}");


            var generatedScriptFilePath = getPathToGeneratedFile(generationData.FileName);

            var filePath = Application.dataPath + generatedScriptFilePath.Substring("Assets".Length);
            var fileText = sbTrans.ToString();

            var dirPath = new FileInfo(filePath).DirectoryName;

            if (dirPath != null && !Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            File.WriteAllText(filePath, fileText, Encoding.UTF8);
        }

        private string getPathToGeneratedFile(string fileName)
        {
            return !SFCoreSettings.Instance(out var settings)
                ? string.Empty
                : $"Assets/{settings.GeneratorScriptsPath}/{fileName}.cs";
        }

        private string indents(int amount)
        {
            var result = string.Empty;

            for (int i = 0; i < amount; i++)
            {
                result += "    ";
            }

            return result;
        }

        private string formatPropertyName(string property, bool allowFullLength = false)
        {
            property = getValidPropertyName(property);

            if (string.IsNullOrEmpty(property))
                return string.Empty;

            // C# IDs can't start with a number
            if (numbers_template.IndexOf(property[0]) >= 0)
                property = "_" + property;

            if (!allowFullLength && property.Length > max_characters)
                property = property.Substring(0, max_characters);

            // Remove invalid characters
            var chars = property.ToCharArray();
            for (int i = 0, imax = chars.Length; i < imax; ++i)
                if (characters_template.IndexOf(chars[i]) < 0)
                    chars[i] = '_';
            return new string(chars);
        }

        private string getValidPropertyName(string text, bool allowCategory = false)
        {
            if (text == null)
                return null;
            return removeNonASCIICharacters(text, allowCategory);
        }

        private string removeNonASCIICharacters(string text, bool allowCategory = false)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Remove Non-Letter/Digits and collapse all extra spaces into a single space
            var current = 0;
            var output = new char[text.Length];
            var skipped = false;

            foreach (var cc in text.Trim().ToCharArray())
            {
                var c = ' ';
                if ((allowCategory && (cc == '\\' || cc == '\"' || (cc == '/'))) ||
                    char.IsLetterOrDigit(cc) ||
                    symbols_template.IndexOf(cc) >= 0)
                {
                    c = cc;
                }

                if (char.IsWhiteSpace(c))
                {
                    if (!skipped)
                    {
                        if (current > 0)
                            output[current++] = ' ';

                        skipped = true;
                    }
                }
                else
                {
                    skipped = false;
                    output[current++] = c;
                }
            }

            return new string(output, 0, current);
        }
    }
}