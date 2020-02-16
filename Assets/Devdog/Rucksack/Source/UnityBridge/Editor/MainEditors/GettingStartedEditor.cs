using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Devdog.General2.Editors;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Devdog.Rucksack.Editor
{
    public class GettingStartedEditor : GettingStartedEditorBase
    {
        [XmlRoot(ElementName="dependency")]
        public class Dependency {
            [XmlAttribute(AttributeName="name")]
            public string Name { get; set; }
            [XmlAttribute(AttributeName="minVersion")]
            public string MinVersion { get; set; }
        }

        [XmlRoot(ElementName="plugin")]
        public class Plugin {
            [XmlElement(ElementName="name")]
            public string Name { get; set; }
            [XmlElement(ElementName="description")]
            public string Description { get; set; }
            [XmlElement(ElementName="version")]
            public string Version { get; set; }

            private string _separateBuild;
            [XmlElement(ElementName="separateBuild")]
            public bool SeparateBuild {
                get { return bool.Parse(_separateBuild); }
                set { _separateBuild = value.ToString(); }
            }
			
            [XmlElement(ElementName="dependencies")]
            public List<Dependency> Dependencies { get; set; }
        }
		
        
        private const string MenuItemPath = RucksackConstants.ToolsPath + "Getting started";
        private static bool didReloadScripts = false;
        
        public GettingStartedEditor()
        {
            productName = RucksackConstants.ProductName;
            youtubeUrl = "https://www.youtube.com/watch?v=fQXOAHr50ag&list=PL_HIoK0xBTK43kHnGEkYVXw378BLt9OFk";
            reviewProductUrl = "https://www.assetstore.unity3d.com/en/#!/content/55292";
            documentationUrl = "http://rucksack-docs.readthedocs.io/en/latest/";
        }

        [MenuItem(MenuItemPath, false, 1)] // Always at bottom
        protected static void ShowWindowInternal()
        {
            window = GetWindow<GettingStartedEditor>();
            window.ShowWindow();
        }

        public override void ShowWindow()
        {
            window = GetWindow<GettingStartedEditor>();
            window.GetImages();
            window.ShowUtility();
        }

        [InitializeOnLoadMethod]
        protected static void InitializeOnLoadMethod()
        {
            if (didReloadScripts)
            {
                // Avoid showing the window every time the code is reloaded or recompiled.
                didReloadScripts = false;
                return;
            }
            
            if (EditorPrefs.GetBool("SHOW_" + RucksackConstants.ProductName + "_GETTING_STARTED_WINDOW", true))
            {
                window = GetWindow<GettingStartedEditor>();
                window.ShowWindow();
            }
        }

        [DidReloadScripts]
        protected static void DidReloadScripts()
        {
            didReloadScripts = true;
        }

        private void OnEnable()
        {
            // Fetch meta data from XML file
            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/Devdog/{RucksackConstants.ProductName}/{RucksackConstants.ProductName}.xml");
            var serializer = new XmlSerializer(typeof(Plugin));

            using (var reader = new StringReader(asset.text))
            {
                var p = serializer.Deserialize(reader);
                var plugin = p as Plugin;
                if (plugin != null)
                {
                    version = plugin.Version;
                
                    // TODO: Set links!
//                documentationUrl = Constants.ProductUrl;
//                forumUrl = Constants.DiscordUrl;
                }   
            }
        }
        
        protected override void DrawGettingStarted()
        {
            int i = 0;
            DrawBox(i++, 0, "Demo Scene", "Open the main demo scene", documentationIcon, () =>
            {
                EditorSceneManager.OpenScene($"Assets/Devdog/{RucksackConstants.ProductName}/Demo/UNetMultiplayer.unity", OpenSceneMode.Single);
                EditorSceneManager.OpenScene($"Assets/Devdog/{RucksackConstants.ProductName}/Demo/SurvivalUI_Partial.unity", OpenSceneMode.Additive);
            });
            
            DrawBox(i++, 0, "Documentation", "The official documentation has a detailed description of all components and code examples.", documentationIcon, () =>
            {
                Application.OpenURL(documentationUrl);
            });

            DrawBox(i++, 0, "Video tutorials", "The video tutorials cover all interfaces and a complete set up.", videoTutorialsIcon, () =>
            {
                Application.OpenURL(youtubeUrl);
            });

            DrawBox(i++, 0, "Discord", "Check out the " + productName + " discord for some community power.", forumIcon, () =>
            {
                Application.OpenURL(forumUrl);
            });

//            DrawBox(3, 0, "Integrations", "Combine the power of assets and enable integrations.", integrationsIcon, () =>
//            {
//                IntegrationHelperEditor.ShowWindow();
//            });

            DrawBox(i++, 0, "Rate / Review", "Like " + productName + "? Share the experience :)", reviewIcon, () =>
            {
                Application.OpenURL(reviewProductUrl);
            });

            base.DrawGettingStarted();
        }
        
    }
}