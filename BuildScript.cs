using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;
using UnityEditor.Build.Reporting;
namespace Container.Build
{
    public class BuildScript : MonoBehaviour
    {
        public static BuildScript instance;

        void Awake(){
            instance = this;
        }

        [MenuItem("Build/Alpha/All", false, priority:0)]
        public static void BuildAlphaAll(){
            Debug.Log("BuildAlphaAll");
            BuildAlpha(0);
        }
        [MenuItem("Build/Alpha/Steam", false, priority:0)]
        public static void BuildAlphaOnlySteam(){
            Debug.Log("BuildAlphaOnlySteam");
            BuildAlpha(1);

        }
        [MenuItem("Build/Alpha/SteamX", false, priority:0)]
        public static void BuildAlphaNoSteam(){
            Debug.Log("BuildAlphaNoSteam");
            BuildAlpha(2);

        }
        [MenuItem("Build/Live/All", false, priority:0)]
        public static void BuildLiveAll(){
            Debug.Log("BuildLiveAll");
            BuildLive(0);

        }
        [MenuItem("Build/Live/Steam", false, priority:0)]
        public static void BuildLiveOnlySteam(){
            Debug.Log("BuildLiveOnlySteam");
            BuildLive(1);

        }
        [MenuItem("Build/Live/SteamX", false, priority:0)]
        public static void BuildLiveNoSteam(){
            Debug.Log("BuildLiveNoSteam");
            BuildLive(2);

        }
        /// <summary>
        /// 0:all, 1:onlySteam, 2:noSteam
        /// </summary>
        /// <param name="steamOption"> 왜 안돼 </param>
        public static void BuildAlpha(int steamOption)
        {
            //List<string> defineStrArray = new List<string>();
            string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            HashSet<string> defines = new HashSet<string>();

            if (steamOption == 0){
                Debug.Log($"Build Start : alpha - steam both ");
            }

            else if(steamOption == 1){
                Debug.Log($"Build Start : alpha - steam included ");
                defines = new HashSet<string>(currentDefines.Split(';')) {
                    "STEAMWORKS_NET","alpha"
                };
            }
            else if(steamOption == 2){
                Debug.Log($"Build Start : alpha - steam excluded ");
                defines = new HashSet<string>(currentDefines.Split(';')) {
                    "STEAMWORKS_NET","alpha","DISABLESTEAMWORKS"
                };
            }


            string currentTimeText = DateTime.Now.ToString(("yyMMdd_HHmm"));
            string currentVersion = GetNonHotVersion().Replace(".", "");

            BuildPlayerOptions buildOption = new BuildPlayerOptions();
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            buildOption.scenes = GetBuildSceneList();
            buildOption.target = BuildTarget.StandaloneWindows64;
                //스팀미포함
                string fileName = string.Format("/Build/Alpha_SteamX/JID_Alpha_{0}_{1}/{2}.exe"
                ,currentVersion,currentTimeText,PlayerSettings.productName);

                buildOption.locationPathName = Application.persistentDataPath + fileName;

                string newDefines = string.Join(";", defines);
                if (newDefines != currentDefines) {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newDefines);
                }

                //instance.StartCoroutine(StartBuildPlayer(buildOption));
                //Debug.Log($"Build completed : alpha - steam excluded ");
                
                BuildReport report = BuildPipeline.BuildPlayer(buildOption);
                BuildSummary summary = report.summary;
                
                
                if (summary.result == BuildResult.Succeeded)
                {
                    Debug.Log($"Build succeeded: {summary.totalSize}bytes. {summary.totalTime}s");
                    Debug.Log(summary.outputPath);
                }

                if (summary.result == BuildResult.Failed)
                {
                    Debug.Log("Build failed");
                }
            //}




        }
        public static void BuildLive(int steamOption)
        {
            if (steamOption == 0){
                Debug.Log($"Build Start : live - steam both ");

            }
            else if(steamOption == 1){
                Debug.Log($"Build Start : live - steam included ");

            }
            else if(steamOption == 2){
                Debug.Log($"Build Start : live - steam excluded ");

            }

            string currentTimeText = DateTime.Now.ToString(("yyMMdd_HHmm"));
            string currentVersion = GetNonHotVersion().Replace(".", "");

            

            BuildPlayerOptions buildOption = new BuildPlayerOptions();
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            buildOption.scenes = GetBuildSceneList();
            buildOption.target = BuildTarget.StandaloneWindows64;

            if (steamOption == 0 || steamOption == 1){

            //스팀용
                string fileName = string.Format("/Build/Live/JID_Live_{0}_{1}/{2}.exe"
                ,currentVersion,currentTimeText,PlayerSettings.productName);

                buildOption.locationPathName = Application.persistentDataPath + fileName;
                
                string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                HashSet<string> defines = new HashSet<string>(currentDefines.Split(';')) {
                    "STEAMWORKS_NET"
                };
                defines.Remove("alpha");
                defines.Remove("DISABLESTEAMWORKS");

                string newDefines = string.Join(";", defines);
                if (newDefines != currentDefines) {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newDefines);
                }
                
                
                //PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, "STEAMWORKS_NET");
                //BuildPipeline.BuildPlayer(buildOption);
                instance.Invoke("StartBuildPlayer", 1f);
                Debug.Log($"Build completed : live - steam included ");
            }


            if (steamOption == 0 || steamOption == 2)
            {
                //스팀미포함
                string fileName = string.Format("/Build/Live_SteamX/JID_Live_{0}_{1}/{2}.exe"
                , currentVersion, currentTimeText, PlayerSettings.productName);

                buildOption.locationPathName = Application.persistentDataPath + fileName;
                
                      
                string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                Debug.Log($"currentDefines = {currentDefines}");
                HashSet<string> defines = new HashSet<string>(currentDefines.Split(';')) {
                    "STEAMWORKS_NET","DISABLESTEAMWORKS"
                };

                defines.Remove("alpha");

                foreach(string a in defines){
                Debug.Log($"defines = {a}");

                }

                string newDefines = string.Join(";", defines);
                if (newDefines != currentDefines) {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newDefines);
                }


                
                //BuildPipeline.BuildPlayer(buildOption);
                //instance.Invoke("StartBuildPlayer", 1f);
                instance.StartCoroutine(StartBuildPlayer(buildOption));
                //ins Invoke("StartBuildPlayer", 2f);
                Debug.Log($"Build completed : live - steam excluded ");
            }

        }
        
 
        static IEnumerator StartBuildPlayer(BuildPlayerOptions _buildOption){
            //Debug.Log("StartBuildPlayer");
            yield return new WaitForSeconds(1f);
            BuildReport report = BuildPipeline.BuildPlayer(_buildOption);
            BuildSummary summary = report.summary;
            
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }
            //yield return StartCoroutine(coroutine);
        }

        /// <summary>
        /// 현재 빌드세팅의 Scene리스트를 받아옴.
        /// Enable이 True인 것만 받아옴.
        /// </summary>
        /// <returns></returns>
        protected static string[] GetBuildSceneList()
        {
            EditorBuildSettingsScene[] scenes = UnityEditor.EditorBuildSettings.scenes;

            List<string> listScenePath = new List<string>();

            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i].enabled)
                    listScenePath.Add(scenes[i].path);
            }

            return listScenePath.ToArray();
        }

        [MenuItem("Build/Ver/check version", false, priority:12)]
        private static void CheckCurrentVersion()
        {
            Debug.Log("Build v" + PlayerSettings.bundleVersion);
        }
        [MenuItem("Build/Ver/build/+", false, priority:1)]
        private static void IncreaseBuild()
        {
            EditVersion(0, 0, 1,0);
        }
        [MenuItem("Build/Ver/build/-", false, priority:1)]
        private static void DecreaseBuild()
        {
            EditVersion(0, 0, -1,0);
        }
        [MenuItem("Build/Ver/hot/+", false, priority:1)]
        private static void IncreaseHot()
        {
            EditVersion(0, 0, 0,1);
        }
        [MenuItem("Build/Ver/hot/-", false, priority:1)]
        private static void DecreaseHot()
        {
            EditVersion(0, 0, 0,-1);
        }
        static void EditVersion(int majorIncr, int minorIncr, int buildIncr, int hotIncr)
        {
            string[] fullVerStr = PlayerSettings.bundleVersion.Split('-');
            string[] nonHotVerStr = fullVerStr[0].Split('.');
            string hotVerStr = fullVerStr[1];

            int MajorVersion = int.Parse(nonHotVerStr[0]) + majorIncr;
            int MinorVersion = int.Parse(nonHotVerStr[1]) + minorIncr;
            int Build = int.Parse(nonHotVerStr[2]) + buildIncr;
            int HotFixCounter = int.Parse(hotVerStr) + hotIncr;

            PlayerSettings.bundleVersion = MajorVersion.ToString("0") + "." +
                                        MinorVersion.ToString("0") + "." +
                                        Build.ToString("0") + "-" +
                                        HotFixCounter.ToString("0");

            CheckCurrentVersion();
        }
        static string GetNonHotVersion(){

            string[] fullVerStr = PlayerSettings.bundleVersion.Split('-');

            return fullVerStr[0];
        }
    }
    
}