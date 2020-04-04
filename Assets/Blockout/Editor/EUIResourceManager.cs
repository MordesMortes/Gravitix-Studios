using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace RadicalForge.Blockout
{
    public struct EUISectionContent
    {
        public EUISectionContent(string name_, GUIContent content_)
        {
            name = name_;
            content = content_;
        }

        public string name;
        public GUIContent content;
    }

    public class EUIResourceManager
    {
        private static EUIResourceManager m_instance;
        public static EUIResourceManager Instance{
            get{
                if(m_instance == null)
                {
                    m_instance = new EUIResourceManager();
                    m_instance.Init();
                }
                return m_instance;
            }
        }

        private Dictionary<string, Texture2D> m_blockoutTextures = new Dictionary<string, Texture2D>();
        private GUISkin m_skin;
        public GUISkin Skin { get{return m_skin; }}
        private Dictionary<string, GUIContent> m_blockoutGUIContent = new Dictionary<string, GUIContent>();

        public Texture2D[] GridTextures;
        public Texture2D[] GridIcons;
        public Texture2D[] AreaCommentTextures;
        public Texture2D[] AreaCommentIcons;
        public Texture2D[] PinCommentIcons;
        public Texture2D[] ThemeIcons;

        public ThemeDefinition[] BlockoutThemes;
        public ThemeDefinition UserTheme;

        /// <summary>
        /// Initialize the Editor UI Resource Manager.
        /// - Finds all Blockout textures based on pro / free skin
        /// - Load the editor UI GUI skin
        /// </summary>
        public void Init()
        {
            var textures = Resources.LoadAll<Texture2D>("Blockout/UI_Icons").Where(x => x.name.Contains("Button_Long") || EditorGUIUtility.isProSkin ? x.name.Contains("Light") : !x.name.Contains("Light")).ToArray();
            foreach (var tex in textures)
            {
                if(!m_blockoutTextures.ContainsKey(tex.name))
                    m_blockoutTextures.Add(tex.name, tex);
            }
            m_skin = Resources.Load<GUISkin>(EditorGUIUtility.isProSkin ? "Blockout/UI_Icons/BlockoutEditorSkinLight" : "Blockout/UI_Icons/BlockoutEditorSkin");
            GridTextures = Resources.LoadAll<Texture2D>("Blockout/BlockoutTextures").ToArray();
            GridIcons = Resources.LoadAll<Texture2D>("Blockout/UI_Icons").ToList().Where(x => x.name.Contains("Icon_Blockout")).ToArray();
            ThemeIcons =  Resources.LoadAll<Texture2D>("Blockout/UI_Icons").Where(x => x.name.Contains("Icon_Theme")).ToArray();

            AreaCommentTextures = Resources.LoadAll("Blockout/Area Comment Textures", typeof(Texture2D))
                                           .Cast<Texture2D>()
                                           .ToArray();
            PinCommentIcons = Resources.LoadAll("Blockout/Pins", typeof(Texture2D))
                                       .Where(x => EditorGUIUtility.isProSkin
                                                       ? x.name.Contains("Light")
                                                       : !x.name.Contains("Light"))
                                       .Cast<Texture2D>().OrderBy(x => x.name).ToArray();
            AreaCommentIcons = Resources.LoadAll("Blockout/UI_Icons", typeof(Texture2D)).Cast<Texture2D>()
                                               .Where(x => x.name.Contains("Blockout_Comment_Texture_Icon")).ToArray();

            BlockoutThemes = Resources.LoadAll<ThemeDefinition>("Blockout/BlockoutMaterials").ToArray();

            UserTheme = Resources.Load("Blockout/BlockoutMaterials/Blockout_Theme_User") as ThemeDefinition;
            var l = BlockoutThemes.ToList();
            l.Remove(UserTheme);
            l.TrimExcess();
            BlockoutThemes = l.ToArray();


        }

        /// <summary>
        /// Register a collection of EUI Content resources if that content doesnt exist
        /// This is usually called from a Blockout Editor Section
        /// </summary>
        /// <param name="sectionContent">The Collection of Editor UI Content</param>
        public void RegisterGUIContent(EUISectionContent[] sectionContent)
        {
            foreach (var content in sectionContent)
            {
                if(!m_blockoutGUIContent.ContainsKey(content.name))
                    m_blockoutGUIContent.Add(content.name, content.content);
            }
        }

        /// <summary>
        /// Get a texture from the Editor UI Manager by a name or partial name
        /// </summary>
        /// <param name="name">The full or partial name of the texture</param>
        /// <returns>An Editor Texture 2D</returns>
        public Texture2D GetTexture(string name)
        {
            var result = m_blockoutTextures.FirstOrDefault(x => x.Key.Contains(name)).Value;
            if(!result)
            {
                var alt = Resources.LoadAll<Texture2D>("").Where(x => x.name.Contains(name)).ToList();
                if(alt.Count == 0)
                    Debug.LogError("Unable to find texture with name: " + name);
                else
                {
                    m_blockoutTextures.Add(name, alt[0]);
                }
                result = alt.Count > 0 ? alt[0] : Texture2D.whiteTexture;
            }
            return result;
        }


        /// <summary>
        /// Get a GUIContent resource based on a name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GUIContent GetContent(string name)
        {
            var result = m_blockoutGUIContent.FirstOrDefault(x => x.Key.Contains(name)).Value;
            if(result == null)
            {
                Debug.LogError("Unable to load GUIContent to content: " + name);
                result = new GUIContent("NULL");
            }

            return result;
        }
        
    }
}