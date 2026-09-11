using UnityEditor;

namespace Shadowvale.SpriteSytem.Editor
{
    public static class Toolbar
    {
        public static void Draw()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            ToolbarFileMenu.Draw();
            ToolbarEditMenu.Draw();

            EditorGUILayout.EndHorizontal();
        }
    }
}