using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Compilation;

namespace Secrets.Misc
{
    public sealed class AddNamespaceAssetProcessor : AssetModificationProcessor
    {
        private static readonly Regex _rootFoldersRegex = new(@"Assets\/Scripts\/.+?\/(.*)");

        // https://stackoverflow.com/questions/39461801/unity-add-default-namespace-to-script-template
        // https://forum.unity.com/threads/filenotfoundexception-when-reading-asset-contents-in-onwillcreateasset.1399756/
        public static void OnWillCreateAsset(string path)
        {
            if (!path.EndsWith(".cs.meta"))
            {
                return;
            }

            var originalFilePath = AssetDatabase.GetAssetPathFromTextMetaFilePath(path);
            var folderPath = Path.GetDirectoryName(originalFilePath).Replace('\\', '/');
            var match = _rootFoldersRegex.Match(folderPath);

            var file = File.ReadAllText(originalFilePath);
            var rootNamespace = CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(originalFilePath);

            var namespaceString = new StringBuilder(rootNamespace);
            if (match.Success)
            {
                // Then we are in some module folder.
                namespaceString.Append('.');
                namespaceString.Append(match.Groups[1]);
                namespaceString.Replace('/', '.');
            }

            file = file.Replace("#NAMESPACE#", namespaceString.ToString());

            File.WriteAllText(originalFilePath, file);
            AssetDatabase.Refresh();
        }
    }
}