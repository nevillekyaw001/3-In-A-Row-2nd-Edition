using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PubScale.SdkOne
{
    public class PubScaleWindow : EditorWindow
    {
        public enum Category
        {
            SETUP,
            SETTINGS,
            SUPPORT
        }

        public Category category = Category.SETUP;

        private List<Category> _categories;
        private List<string> _categoryLabels;
        private Category _categorySelected;



        const string WindowTitle = "PubScale : " + PubEditorUX.packageName;
        GUIStyle textStyle;
        GUIStyle linkStyle;
        Texture image;
        bool initialized = false;


        static PubScaleSettings psSettings = null;

        float defaultEditorLabelWidth = 60;

        PubEditorUXState startState = new PubEditorUXState();

        [MenuItem("Window/PubScale SDK/Main", priority = -10)]
        public static void OpenWindow()
        {
            if (psSettings == null)
                LoadPubScaleSettings();

            PubScaleWindow wnd = GetWindow<PubScaleWindow>(false);
            wnd.titleContent = new GUIContent(WindowTitle);
            wnd.minSize = new Vector2(400, 400);
            wnd.maxSize = wnd.minSize;
            wnd.Show();

            psSettings.IsFirstTimeUsingTheAsset = false;

            EditorUtility.SetDirty(psSettings);
            AssetDatabase.SaveAssets();

            //  Debug.Log("Is Pro Skin - " + EditorGUIUtility.isProSkin);
        }

        private void OnEnable()
        {
            if (_categories == null)
            {
                InitCategories();
            }
            // if (_categorizedItems == null)
            // {
            //     InitContent();
            // }
            InitStyles();
        }

        private GUIStyle _tabStyle;
        private void InitStyles()
        {
            _tabStyle = new GUIStyle();
            _tabStyle.alignment = TextAnchor.MiddleCenter;
            _tabStyle.fontSize = 16;
            Texture2D tabNormal = (Texture2D)Resources.Load("Tab_Normal");
            Texture2D tabSelected = (Texture2D)Resources.Load("Tab_Pub");
            Font tabFont = (Font)Resources.Load("Oswald-Regular");
            _tabStyle.font = tabFont;
            _tabStyle.fixedHeight = 40;
            _tabStyle.normal.background = tabNormal;
            _tabStyle.normal.textColor = Color.grey;
            _tabStyle.onNormal.background = tabSelected;
            _tabStyle.onNormal.textColor = Color.black;
            _tabStyle.onFocused.background = tabSelected;
            _tabStyle.onFocused.textColor = Color.black;
            _tabStyle.border = new RectOffset(18, 18, 20, 4);
        }

        private void InitCategories()
        {
            _categories = PubEditorUX.GetListFromEnum<Category>();
            _categoryLabels = new List<string>();
            foreach (Category category in _categories)
            {
                _categoryLabels.Add(category.ToString());
            }
        }

        private void DrawTabs()
        {

            int index = (int)_categorySelected;
            EditorGUILayout.Space();

            index = GUILayout.Toolbar(index, _categoryLabels.ToArray(), _tabStyle);
            _categorySelected = _categories[index];
        }


        static void LoadPubScaleSettings()
        {
            PubEditorUX.CheckResourcesFolderInCommon();

            psSettings = AssetDatabase.LoadAssetAtPath<PubScaleSettings>(PubEditorUX.PackageSettingsPath);

            if (psSettings == null)
            {
                psSettings = PubEditorUX.CreateAndSavePubScaleSettings();
            }

            PubScaleSettings.Instance = (PubScaleSettings)AssetDatabase.LoadAssetAtPath(PubEditorUX.PackageSettingsPath, typeof(PubScaleSettings));

            //  Selection.activeObject = PubScaleSettings.Instance;
        }

        public void OnGUI()
        {
            if (psSettings == null)
                LoadPubScaleSettings();


            defaultEditorLabelWidth = EditorGUIUtility.labelWidth;

            if (!initialized)
            {
                string[] s = AssetDatabase.FindAssets(PubEditorUX.imageName + " t:Texture");

                if (s.Length > 0)
                {
                    image = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(s[0]));
                }

                textStyle = new GUIStyle(EditorStyles.label);
                textStyle.wordWrap = true;
                textStyle.margin = new RectOffset(20, 20, 20, 20);
                textStyle.alignment = TextAnchor.UpperLeft;
                linkStyle = new GUIStyle(textStyle);
                linkStyle.hover.textColor = linkStyle.normal.textColor * 0.5f;
                initialized = true;
            }

            float imageHeight = image.height / 4;
            float imageWidth = image.width / 4;

            float margin = 20;

            if (image != null)
            {
                GUI.DrawTexture(new Rect(EditorGUIUtility.currentViewWidth / 2 - imageWidth / 2, 5, imageWidth, imageHeight), image, ScaleMode.ScaleToFit);
            }

            EditorGUILayout.Space();

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {

                EditorGUIUtility.labelWidth = PubEditorUX.DEF_LABEL_WIDTH;

                GUILayout.BeginArea(new Rect(20, imageHeight + 10, position.width - margin * 2, position.height - imageHeight + 2 * margin));

                DrawTabs();

                // EditorGUILayout.Space();
                // EditorGUILayout.Space();
                // EditorGUILayout.Space();

                bool changesDetected = false;

                int selection = (int)_categorySelected;


                using (new EditorGUILayout.VerticalScope(GUI.skin.button))
                {
                    EditorGUILayout.LabelField("", "v" + PubScaleSDK.PluginVersion, PubEditorUX.PubTitleStyle, GUILayout.ExpandWidth(true));

                    EditorGUILayout.Space();

                    if (selection == 0)
                    {
                        EditorGUIUtility.labelWidth = PubEditorUX.DEF_LABEL_WIDTH - 40;

                        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                        {
                            string pre = psSettings.AppId;
                            psSettings.AppId = EditorGUILayout.TextField("PubScale APP ID:", psSettings.AppId);

                            if (IsStringChanged(pre, psSettings.AppId))
                                changesDetected = true;
                        }

                        EditorGUIUtility.labelWidth = PubEditorUX.DEF_LABEL_WIDTH + 40;
                        EditorGUILayout.Space();
                    }
                    else if (selection == 1)
                    {

                        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                        {
                            PubEditorUX.DisplayHeading("NATIVE ADS");

                            EditorGUILayout.BeginHorizontal();

                            EditorGUIUtility.labelWidth = PubEditorUX.DEF_LABEL_WIDTH - 60;

                            bool preBooleanSettingVal = false;

                            preBooleanSettingVal = psSettings.UseTestMode;
                            psSettings.UseTestMode = EditorGUILayout.Toggle("Use Test Mode:", psSettings.UseTestMode);

                            if (preBooleanSettingVal != psSettings.UseTestMode)
                                changesDetected = true;

                            // if (psSettings.UseTestMode == false)
                            // {
                            //     GUILayout.FlexibleSpace();

                            //     EditorGUIUtility.labelWidth = PubEditorUX.DEF_LABEL_WIDTH + 30;

                            //     preBooleanSettingVal = psSettings.ShowLogsForLiveAdUnits;
                            //     psSettings.ShowLogsForLiveAdUnits = EditorGUILayout.Toggle("Enable Plugin Logs in Release:", psSettings.ShowLogsForLiveAdUnits);

                            //     if (preBooleanSettingVal != psSettings.ShowLogsForLiveAdUnits)
                            //         changesDetected = true;
                            // }

                            EditorGUIUtility.labelWidth = PubEditorUX.DEF_LABEL_WIDTH + 50;


                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space();

                            EditorGUIUtility.labelWidth = PubEditorUX.DEF_LABEL_WIDTH;


#if UNITY_IOS
                        string preFallbackIOS = psSettings.Fallback_NativeAdID_IOS;
                        psSettings.Fallback_NativeAdID_IOS = EditorGUILayout.TextField("Fallback Native ID IOS: ", psSettings.Fallback_NativeAdID_IOS);

                        if (IsStringChanged(preFallbackIOS, psSettings.Fallback_NativeAdID_IOS))
                            changesDetected = true;
#else

                            string preFallbackAnd = psSettings.Fallback_NativeAdID_Android;
                            psSettings.Fallback_NativeAdID_Android = EditorGUILayout.TextField("Fallback Native ID Android: ", psSettings.Fallback_NativeAdID_Android);

                            if (IsStringChanged(preFallbackAnd, psSettings.Fallback_NativeAdID_Android))
                                changesDetected = true;
#endif

                            EditorGUIUtility.labelWidth = PubEditorUX.DEF_LABEL_WIDTH;

                            PubEditorUX.DisplayTip("Used in case there is delay in ad config from server");

                            EditorGUILayout.Space();

                        }
                    }
                    else if (selection == 2)
                    {


                        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                        {
                            EditorGUILayout.Space();

                            if (GUILayout.Button("Integration Docs"))
                            {
                                Application.OpenURL("https://greedygame.github.io/sdkone-unity_native_ads_plugin/");
                            }

                            EditorGUILayout.Space();
                            EditorGUILayout.Space();

                            if (GUILayout.Button("Scripting API"))
                            {
                                Application.OpenURL("https://github.com/GreedyGame/sdkone-unity_native_ads_plugin/wiki");
                            }

                            EditorGUILayout.Space();
                            EditorGUILayout.Space();

                            if (GUILayout.Button("Website"))
                            {
                                Application.OpenURL("https://pubscale.com/ad-revenue-optimization-platform");
                            }

                            EditorGUILayout.Space();
                        }

                    }
                    else
                    {
                        EditorGUILayout.LabelField(@"COMING SOON...", textStyle);
                    }

                }

                GUILayout.EndArea();


                if (changesDetected)
                {
                    SaveSettingsData();
                }
            }

            EditorGUIUtility.labelWidth = defaultEditorLabelWidth;
        }




        bool IsStringChanged(string s1, string s2)
        {
            if (string.CompareOrdinal(s1, s2) != 0)
                return true;
            else
                return false;
        }


        void SaveSettingsData()
        {
            if (psSettings != null)
            {
                EditorUtility.SetDirty(psSettings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }



    }
}
