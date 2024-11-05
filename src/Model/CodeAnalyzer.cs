using System.IO;
using System.Text.RegularExpressions;

namespace ClassMethodAnalyzer.Model
{
    public static class CodeAnalyzer
    {
        public static string Analyze(string folderPath)
        {
            var results = new List<string>();

            // フォルダ内の全ファイルを再帰的に取得し、bin/objフォルダを除外
            foreach (var filePath in GetCSharpFiles(folderPath))
            {
                var code = File.ReadAllText(filePath);
                var classNames = ExtractClassNames(code);
                var methodsByClass = ExtractMethodsByClass(code, classNames);

                foreach (var className in methodsByClass.Keys)
                {
                    foreach (var methodName in methodsByClass[className].Methods)
                    {
                        results.Add($"{className}.{methodName}");
                    }
                    if (methodsByClass[className].Constructor != null)
                    {
                        results.Add($"{className}.{methodsByClass[className].Constructor}");
                    }
                }
            }

            return string.Join("\n", results);
        }

        private static IEnumerable<string> GetCSharpFiles(string folderPath)
        {
            foreach (var file in Directory.GetFiles(folderPath, "*.cs", SearchOption.TopDirectoryOnly))
            {
                yield return file;
            }

            foreach (var directory in Directory.GetDirectories(folderPath))
            {
                // binまたはobjフォルダをスキップ
                if (directory.EndsWith("bin", StringComparison.OrdinalIgnoreCase) ||
                    directory.EndsWith("obj", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // サブディレクトリを再帰的に取得
                foreach (var file in GetCSharpFiles(directory))
                {
                    yield return file;
                }
            }
        }

        private static List<string> ExtractClassNames(string code)
        {
            var classNames = new List<string>();
            var classPattern = @"\bclass\s+(\w+)";
            var matches = Regex.Matches(code, classPattern);

            foreach (Match match in matches)
            {
                classNames.Add(match.Groups[1].Value);
            }

            return classNames;
        }

        private static Dictionary<string, ClassMethods> ExtractMethodsByClass(string code, List<string> classNames)
        {
            var methodsByClass = new Dictionary<string, ClassMethods>();
            foreach (var className in classNames)
            {
                // クラス全体を取得
                var methodPattern = $@"{className}.*?\{{((?:.*?\n)*?)\}}";
                var classMatch = Regex.Match(code, methodPattern, RegexOptions.Singleline);

                if (classMatch.Success)
                {
                    var methods = new List<string>();
                    string constructor = null;
                    var methodPatternInClass = @"\b(?:public|private|protected|internal)?\s*(?:void|int|string|bool|float|double|var)\s+(\w+)\s*\(";
                    var constructorPattern = $@"\b{className}\s*\(";

                    var classBody = classMatch.Groups[1].Value;

                    // メソッドを抽出
                    var methodMatches = Regex.Matches(classBody, methodPatternInClass);
                    foreach (Match methodMatch in methodMatches)
                    {
                        methods.Add(methodMatch.Groups[1].Value);
                    }

                    // コンストラクタを抽出
                    var constructorMatch = Regex.Match(classBody, constructorPattern);
                    if (constructorMatch.Success)
                    {
                        constructor = className; // コンストラクタ名はクラス名と同じ
                    }

                    methodsByClass[className] = new ClassMethods(methods, constructor);
                }
            }

            return methodsByClass;
        }
    }
}
