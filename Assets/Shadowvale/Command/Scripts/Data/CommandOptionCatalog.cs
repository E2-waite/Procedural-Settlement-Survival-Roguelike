using Shadowvale.Command.Data;
using Shadowvale.Command.Runtime;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Shadowvale.Command.Data
{
    /// <summary>
    /// Container for storing command options
    /// </summary>
    [CreateAssetMenu(fileName = "CommandCatalog", menuName = "Scriptable Objects/CommandCatalog")]
    public class CommandOptionCatalog : ScriptableObject
    {
        [SerializeField] public CommandOption[] options;

        public CommandOption GetOption(CommandType type)
        {
            if (options == null || (int)type >= options.Length) return null;
            return options[(int)type];
        }
    }
}

#if UNITY_EDITOR
namespace Shadowvale.Command.Editor
{
    [CustomEditor(typeof(CommandOptionCatalog))]
    public class CommandOptionCatalogEditor : UnityEditor.Editor
    {
        private const string OptionsDirectory = "Assets/Command/Data/Options";

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            if (GUILayout.Button("Populate Options"))
            {
                PopulateOptions();
            }
        }

        private void PopulateOptions()
        {
            CommandOptionCatalog catalog =
                (CommandOptionCatalog)target;

            string[] guids = AssetDatabase.FindAssets(
                "t:CommandOption",
                new[] { OptionsDirectory });

            List<CommandOption> options = new List<CommandOption>();

            // Get CommandOption objects from directory
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                CommandOption option =
                    AssetDatabase.LoadAssetAtPath<CommandOption>(path);

                if (option != null)
                    options.Add(option);
            }

            // Assign CommandOption objects by CommandType
            catalog.options = new CommandOption[(int)CommandType.None];
            foreach(CommandOption option in options)
            {
                if (option == null || 
                    option.commandType == CommandType.None ||
                    catalog.options.Length <= (int)option.commandType || 
                    catalog.options[(int)option.commandType] != null) 
                    continue;

                catalog.options[(int)option.commandType] = option;
            }
            EditorUtility.SetDirty(catalog);
        }
    }
}
#endif