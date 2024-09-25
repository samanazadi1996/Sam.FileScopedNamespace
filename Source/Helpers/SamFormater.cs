using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FileScopedNamespace.Helpers;


internal static class SamFormater
{
    public static bool TryFormat(List<string> lines, out string result)
    {
        var isFileScoped = lines.Any(p => p.Trim().Contains("namespace")) && lines.Any(p => p.Trim().StartsWith("namespace") && p.Trim().EndsWith(";"));
        if (!isFileScoped)
        {
            var code = AddSemiToNamespace(lines);
            code = RemoveBracketsAfterNamespace(code);
            result = FormatCode(code);
            return true;
        }

        result = string.Empty;

        return false;
    }

    static string AddSemiToNamespace(List<string> lines)
    {
        StringBuilder result = new StringBuilder();
        bool namespaceFound = false;

        foreach (var line in lines)
        {
            string temp = line;

            if (!namespaceFound && !string.IsNullOrEmpty(line) && !line.Trim().EndsWith(";"))
            {
                if (line.Trim().StartsWith("namespace"))
                {
                    temp += ";";
                    namespaceFound = true;
                }
            }

            result.AppendLine(temp);
        }

        return result.ToString();
    }

    static string RemoveBracketsAfterNamespace(string input)
    {
        // پیدا کردن اولین نیم‌فضای نام
        int namespaceIndex = input.IndexOf("namespace");
        if (namespaceIndex == -1)
        {
            Console.WriteLine("No namespace found.");
            return input;
        }

        // پیدا کردن اولین براکت باز بعد از نیم‌فضای نام
        int firstOpenBracketIndex = input.IndexOf('{', namespaceIndex);
        if (firstOpenBracketIndex == -1)
        {
            Console.WriteLine("No opening bracket found after namespace.");
            return input;
        }

        // پیدا کردن آخرین براکت بعد از اولین براکت باز
        int bracketCounter = 1;
        int i = firstOpenBracketIndex + 1;
        for (; i < input.Length; i++)
        {
            if (input[i] == '{')
            {
                bracketCounter++;
            }
            else if (input[i] == '}')
            {
                bracketCounter--;
                if (bracketCounter == 0)
                {
                    break;
                }
            }
        }

        // حذف براکت‌ها
        if (i < input.Length)
        {
            input = input.Remove(i, 1);
            input = input.Remove(firstOpenBracketIndex, 1);
        }
        else
        {
            Console.WriteLine("No closing bracket found for the opening bracket after namespace.");
        }

        return input;
    }

    static string FormatCode(string code)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
        SyntaxNode root = syntaxTree.GetRoot();

        SyntaxNode formattedNode = Microsoft.CodeAnalysis.Formatting.Formatter.Format(root, new AdhocWorkspace());
        return formattedNode.ToFullString();
    }

}
