using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NonPublishablePackages.UI;
using UnityEditor;
using UnityEngine;

namespace NonPublishablePackages
{
    [InitializeOnLoad]
    public class NonPublishablePackagesObserver
    {
        [MenuItem("Assets/Non Publishable Packages/Add package to database")]
        public static void AddRegistrytoDatabase(MenuCommand command)
        {
            // Dictionary<string, NonPublishableregistry> storedRegistries =
            //     new Dictionary<string, NonPublishableregistry>();
            // LoadDependencyRegistry(storedRegistries);
            Debug.Log(command.context);
            Debug.Log(string.Join(" || ",Selection.objects.Select(x=>AssetDatabase.GetAssetPath(x))));
            //WIP Still need a fornt end to create new registries, please add them manually at the json document
            //if(storedRegistries.Values.Any(X=> X.projectRootPath == ))
        }
        
        private const string DEPENDENCY_REGISTRY_PATH = "Resources/NonPublishablePackagesDependencies.json";

        private static Dictionary<string, NonPublishableregistry> loadedRegistries;

        private static Dictionary<string, bool> registryState;

        public static Dictionary<string, bool> RegistryState => registryState;
        public static Dictionary<string, NonPublishableregistry> LoadedRegistries => loadedRegistries;

        private static bool initialized = false;
        
        static NonPublishablePackagesObserver()
        {
            EditorApplication.update += OneTimeInitialize;
        }

        private static void OneTimeInitialize()
        {
            if(initialized) return;
            initialized = true;
            if (!CheckExtraPackagesIntegrity())
            {
                NonPublishablePackagesWindow.ShowWindow();
            }
        }
        
        public static bool CheckExtraPackagesIntegrity()
        {
            LoadDependencyRegistry();
            registryState = new Dictionary<string, bool>();
            bool ret = true;
            foreach (KeyValuePair<string,NonPublishableregistry> record in loadedRegistries)
            {
                bool result = CheckExtraPackageIntegrity(record.Value.projectRootPath);
                if (!result)
                {
                    ret = result;
                }

                registryState[record.Key] = result;
            }

            return ret;
        } 

        private static bool CheckExtraPackageIntegrity(string path)
        {
            string p = Application.dataPath + "/" + path;
            return Directory.Exists(p);
        }
        
        private static void LoadDependencyRegistry(Dictionary<string,NonPublishableregistry> outDictionary = null)
        {
            using StreamReader stream = new StreamReader($"{Application.dataPath}/{DEPENDENCY_REGISTRY_PATH}");
            string json = stream.ReadToEnd();
            if (outDictionary is null)
            {
                loadedRegistries = JsonConvert.DeserializeObject<Dictionary<string, NonPublishableregistry>>(json);
            }
            else
            {
                outDictionary = JsonConvert.DeserializeObject<Dictionary<string, NonPublishableregistry>>(json);
            }
        }
    }
}
