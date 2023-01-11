using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;
using UnityEditor.Build.Reporting;
using Unity.EditorCoroutines.Editor;
namespace Container.Build
{
    public class BuildScript : MonoBehaviour
    {
        public static BuildScript instance;

        void Awake(){
            instance = this;
        }

        // [MenuItem("Build/Alpha/All", false, priority:-12)]
        // public static void BuildAlphaAll(){
        //     //Debug.Log("BuildAlphaAll");
        //     Build(0,true);
        //     Build(1,true);
        // }
        [MenuItem("SetSymbols/Alpha/Set Steam", false, priority:0)]
        public static void BuildAlphaOnlySteam(){
            //Debug.Log("BuildAlphaOnlySteam");
            Build(0);

        }
        [MenuItem("SetSymbols/Alpha/Set SteamX", false, priority:0)]
        public static void BuildAlphaNoSteam(){
            //Debug.Log("BuildAlphaNoSteam");
            Build(1);

        }
        #if STEAMWORKS_NET && alpha && !DISABLESTEAMWORKS
        [MenuItem("Build/Alpha/Build Steam", false, priority:12)]
        public static void BuildAlphaOnlySteamAndBuild(){
            //Debug.Log("BuildAlphaOnlySteam");
            Build(0,true);

        }
        #elif STEAMWORKS_NET && alpha && DISABLESTEAMWORKS
        [MenuItem("Build/Alpha/Build SteamX", false, priority:12)]
        public static void BuildAlphaNoSteamAndBuild(){
            //Debug.Log("BuildAlphaNoSteam");
            Build(1,true);

        }
        #endif
        // [MenuItem("Build/Live/All", false, priority:0)]
        // public static void BuildLiveAll(){
        //     //Debug.Log("BuildLiveAll");
        //     Build(2);
        //     Build(3);

        // }
        [MenuItem("SetSymbols/Live/Set Steam", false, priority:0)]
        public static void BuildLiveOnlySteam(){
            //Debug.Log("BuildLiveOnlySteam");
            Build(2);

        }
        [MenuItem("SetSymbols/Live/Set SteamX", false, priority:0)]
        public static void BuildLiveNoSteam(){
            //Debug.Log("BuildLiveNoSteam");
            Build(3);

        }
        #if STEAMWORKS_NET && live && !DISABLESTEAMWORKS
        [MenuItem("Build/Live/Steam", false, priority:12)]
        public static void BuildLiveOnlySteamAndBuild(){
            //Debug.Log("BuildLiveOnlySteam");
            Build(2,true);

        }
        #elif STEAMWORKS_NET && live && DISABLESTEAMWORKS
        [MenuItem("Build/Live/SteamX", false, priority:12)]
        public static void BuildLiveNoSteamAndBuild(){
            //Debug.Log("BuildLiveNoSteam");
            Build(3,true);

        }
        #endif
        /// <summary>
        /// 0:all, 1:onlySteam, 2:noSteam
        /// </summary>
        /// <param name="steamOption"> 왜 안돼 </param>
        public static void Build(int steamOption, bool isBuild = false)
        {
            //List<string> defineStrArray = new List<string>();
            string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            HashSet<string> defines = new HashSet<string>();

            string buildType = "none";
            bool forSteam = false;

            if(steamOption == 0){
                Debug.Log($"Build Start : Alpha - steam included ");
                defines = new HashSet<string>() {
                    "STEAMWORKS_NET","alpha"
                };
                defines.Remove("DISABLESTEAMWORKS");
                buildType = "Alpha";
                forSteam = true;
            }
            else if(steamOption == 1){
                Debug.Log($"Build Start : Alpha - steam excluded ");
                defines = new HashSet<string>() {
                    "STEAMWORKS_NET","alpha","DISABLESTEAMWORKS"
                };
                buildType = "Alpha";
                forSteam = false;
            }
            else if(steamOption == 2){
                Debug.Log($"Build Start : Live - steam included ");
                defines = new HashSet<string>() {
                    "STEAMWORKS_NET","live"
                };
                buildType = "Live";
                forSteam = true;
            }
            else if(steamOption == 3){
                Debug.Log($"Build Start : Live - steam excluded ");
                defines = new HashSet<string>() {
                    "STEAMWORKS_NET","live","DISABLESTEAMWORKS"
                };
                buildType = "Live";
                forSteam = false;
            }


            string currentTimeText = DateTime.Now.ToString(("yyMMdd_HHmm"));
            string currentVersion = GetNonHotVersion().Replace(".", "");

            BuildPlayerOptions buildOption = new BuildPlayerOptions();
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            buildOption.scenes = GetBuildSceneList();
            buildOption.target = BuildTarget.StandaloneWindows64;

            string fileName = string.Format($"/Build/{buildType}_{(forSteam ? "Steam" : "SteamX")}/JID_{buildType}_{currentVersion}_{currentTimeText}/{PlayerSettings.productName}.exe");

            buildOption.locationPathName = Application.persistentDataPath + fileName;
            //buildOption.options = BuildOptions.DetailedBuildReport;

            string newDefines = string.Join(";", defines);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, newDefines);

            Debug.Log(newDefines);

            Unity.CodeEditor.CodeEditor.CurrentEditor.SyncAll();
            //AssetDatabase.SaveAssets();

            //Build(0, true);
            //=useDelay(buildOption);

            if(isBuild){

                BuildReport report = BuildPipeline.BuildPlayer(buildOption);
                BuildSummary summary = report.summary;

                report.mak
                
                
                if (summary.result == BuildResult.Succeeded)
                {
                    Debug.Log($"Build succeeded: {summary.totalSize* 0.000001f}bytes. {summary.totalTime.Seconds}s");
                    Debug.Log(summary.outputPath);
                }

                if (summary.result == BuildResult.Failed)
                {
                    Debug.Log("Build failed");
                }
            }





        }


        static void useDelay(BuildPlayerOptions buildOption)
        {
            object obj = new object();
            EditorCoroutineUtility.StartCoroutine(IEDelayEditor(buildOption), obj);
        }
        
        static IEnumerator IEDelayEditor(BuildPlayerOptions buildOption)
        {
            Debug.Log("Wait 1 second");
            yield return new EditorWaitForSeconds(20f);
            Debug.Log("After 1 second");
            
            BuildReport report = BuildPipeline.BuildPlayer(buildOption);
            BuildSummary summary = report.summary;
            
            
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Build succeeded: {summary.totalSize* 0.000001f}MB, for {summary.totalTime.Seconds}s");
                Debug.Log(summary.outputPath);
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }
        }
        public static void RealBuild(){
            
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