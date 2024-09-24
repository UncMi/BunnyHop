using System.Collections;
using System.Collections.Generic;
using System.IO;
using static System.IO.Path;
using static System.IO.Directory;
//using static UnityEditor.AssetDatabase;

using UnityEngine;
using UnityEditor;

public class Setup 
{
    // [MenuItem("Tools/Setup/Create Default Folders")]
    public static void CreateDefaultFolders()
    {
            Folders.CreateDefault("_Project", "Animation", "Art", "Materials", "Prefabs", "ScriptableObjects", "Scripts", "Settings");
            //Refresh();
    }

    static class Folders
    {
        public static void CreateDefault(string root, params string[] folders)
        {
            var fullpath = Path.Combine(Application.dataPath, root);
            if (!Directory.Exists(fullpath))
            {
                Directory.CreateDirectory(fullpath);
            }
            foreach (var folder in folders)
            {
                CreateSubFolders(fullpath, folder);
            }
        }
        private static void CreateSubFolders(string rootPath, string folderHierarchy)
        {
            var folders = folderHierarchy.Split('/');
            var currentPath = rootPath;
            foreach (var folder in folders)
            {
                currentPath = Path.Combine(currentPath, folder);
                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }
            }
        }
    }
}
