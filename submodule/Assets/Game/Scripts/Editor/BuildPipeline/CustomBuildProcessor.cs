using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System;
using UnityEditor.iOS.Xcode;
using System.IO;

public class CustomBuildProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 999;

    public void OnPostprocessBuild(BuildReport report)
    {
        switch (report.summary.platform)
        {
            case BuildTarget.Android:
                AndroidPostProcessBuild(report.summary.outputPath);
                break;
            case BuildTarget.iOS:
                IosPostProcessBuild(report.summary.outputPath);
                break;
        }
    }

    private void IosPostProcessBuild(string pathToBuildProject)
    {
#if UNITY_IOS
        PlistElementDict TryGetDictElement(PlistElementDict elementDict, string key)
        {
            return elementDict.values.ContainsKey(key)
                ? elementDict.values[key].AsDict()
                : elementDict.CreateDict(key);
        }
        const string k_TrackingDescription = "Your data will be used to provide you a better and personalized ad experience.";
        var plistPath = pathToBuildProject + "/Info.plist";
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
        PlistElementDict rootDict = plist.root;
        // App transport security settings
        PlistElementDict allowsDict = plist.root.CreateDict("NSAppTransportSecurity");
        allowsDict.SetBoolean("NSAllowsArbitraryLoads", true);

        //SKAdNetwork support
        PlistElementArray adsList = plist.root.CreateArray("SKAdNetworkItems");
        List<string> sdkIdList = GetListSdkNetworkIds();
        for (int i = 0; i < sdkIdList.Count; i++) {
            PlistElementDict adDict = adsList.AddDict();
            adDict.SetString("SKAdNetworkIdentifier", sdkIdList[i]);
        }

        //Universal SKAN Reporting
        rootDict.SetString("NSAdvertisingAttributionReportEndpoint", "https://appsflyer-skadnetwork.com/");
        // Export Compliance
        rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);
        // Tracking 
        rootDict.SetString("NSUserTrackingUsageDescription", k_TrackingDescription);

        // Update Info.plist
        File.WriteAllText(plistPath, plist.WriteToString()); 

        var projPath = PBXProject.GetPBXProjectPath(pathToBuildProject);
        var project = new PBXProject();
        project.ReadFromFile(projPath);
        var mainTargetGuid = project.GetUnityMainTargetGuid();
        foreach (var targetGuid in new[] { mainTargetGuid, project.GetUnityFrameworkTargetGuid() })
        {
            project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
            project.SetBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
        }
        project.SetBuildProperty(mainTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
        project.WriteToFile(projPath);
#endif
    }

    private void AndroidPostProcessBuild(string pathToBuildProject)
    {
        Debug.Log($"pathToBuildProject = ${pathToBuildProject}");
    }

    private List<string> GetListSdkNetworkIds () {
        return new List<string> {
            "su67r6k2v3.skadnetwork",
            "f7s53z58qe.skadnetwork",
            "2u9pt9hc89.skadnetwork",
            "hs6bdukanm.skadnetwork",
            "8s468mfl3y.skadnetwork",
            "5l3tpt7t6e.skadnetwork",
            "c6k4g5qg8m.skadnetwork",
            "v72qych5uu.skadnetwork",
            "44jx6755aq.skadnetwork",
            "prcb7njmu6.skadnetwork",
            "m8dbw4sv7c.skadnetwork",
            "3rd42ekr43.skadnetwork",
            "4fzdc2evr5.skadnetwork",
            "t38b2kh725.skadnetwork",
            "f38h382jlk.skadnetwork",
            "424m5254lk.skadnetwork",
            "ppmx28t8ap.skadnetwork",
            "av6w8kgt66.skadnetwork",
            "cp8zw746q7.skadnetwork",
            "4468km3ulz.skadnetwork",
            "e5fvkxwrpn.skadnetwork",
            "22mmun2rn5.skadnetwork",
            "s39g8k73mm.skadnetwork",
            "yclnxrl5pm.skadnetwork",
            "3qy4746246.skadnetwork",
            "k674qkevps.skadnetwork",
            "kbmxtkgpxc.skadnetwork",
            "9nlqeag3gk.skadnetwork",
            "32z4fx6l9h.skadnetwork",
            "252b5q8x7y.skadnetwork",
            "kbd757ywx3.skadnetwork",
            "cstr6suwn9.skadnetwork",
            "7fmhfwg9en.skadnetwork",
            "6yxyv74ff7.skadnetwork",
            "wzmmz9fp6w.skadnetwork",
            "mqn7fxpca7.skadnetwork",
            "4pfyvq9l8r.skadnetwork",
            "tl55sbb4fm.skadnetwork",
            "mlmmfzh3r3.skadnetwork",
            "klf5c3l5u5.skadnetwork",
            "9t245vhmpl.skadnetwork",
            "9rd848q2bz.skadnetwork",
            "7ug5zh24hu.skadnetwork",
            "7rz58n8ntl.skadnetwork",
            "ejvt5qm6ak.skadnetwork",
            "5lm9lj6jb7.skadnetwork",
            "mtkv5xtk9e.skadnetwork",
            "6g9af3uyq4.skadnetwork",
            "uw77j35x4d.skadnetwork",
            "u679fj5vs4.skadnetwork",
            "rx5hdcabgc.skadnetwork",
            "g28c52eehv.skadnetwork",
            "cg4yq2srnc.skadnetwork",
            "275upjj5gd.skadnetwork",
            "wg4vff78zm.skadnetwork",
            "qqp299437r.skadnetwork",
            "294l99pt4k.skadnetwork",
            "2fnua5tdw4.skadnetwork",
            "ydx93a7ass.skadnetwork",
            "523jb4fst2.skadnetwork",
            "cj5566h2ga.skadnetwork",
            "r45fhb6rf7.skadnetwork",
            "g2y4y55b64.skadnetwork",
            "n6fk4nfna4.skadnetwork",
            "n9x2a789qt.skadnetwork",
            "pwa73g5rt2.skadnetwork",
            "74b6s63p6l.skadnetwork",
            "44n7hlldy6.skadnetwork",
            "gta9lk7p23.skadnetwork",
            "84993kbrcf.skadnetwork",
            "pwdxu55a5a.skadnetwork",
            "6964rsfnh4.skadnetwork",
            "a7xqa6mtl2.skadnetwork",
            "c3frkrj4fj.skadnetwork",
            "3qcr597p9d.skadnetwork",
            "3sh42y64q3.skadnetwork",
            "5a6flpkh64.skadnetwork",
            "p78axxw29g.skadnetwork",
            "zq492l623r.skadnetwork",
            "24t9a8vw3c.skadnetwork",
            "54nzkqm89y.skadnetwork",
            "578prtvx9j.skadnetwork",
            "6xzpu9s2p8.skadnetwork",
            "79pbpufp6p.skadnetwork",
            "9b89h5y424.skadnetwork",
            "feyaarzu9v.skadnetwork",
            "ggvn48r87g.skadnetwork",
            "glqzh8vgby.skadnetwork",
            "ludvb6z3bs.skadnetwork",
            "xy9t38ct57.skadnetwork",
            "zmvfpc5aq8.skadnetwork",
            "4dzt52r2t5.skadnetwork",
            "4w7y6s5ca2.skadnetwork",
            "5tjdwbrq8w.skadnetwork",
            "6p4ks3rnbw.skadnetwork",
            "737z793b9f.skadnetwork",
            "97r2b46745.skadnetwork",
            "b9bk5wbcq9.skadnetwork",
            "bxvub5ada5.skadnetwork",
            "dzg6xy7pwj.skadnetwork",
            "f73kdq92p3.skadnetwork",
            "hdw39hrw9y.skadnetwork",
            "lr83yxwka7.skadnetwork",
            "mls7yz5dvl.skadnetwork",
            "mp6xlyr22a.skadnetwork",
            "rvh3l7un93.skadnetwork",
            "s69wq72ugq.skadnetwork",
            "w9q455wk68.skadnetwork",
            "x44k69ngh6.skadnetwork",
            "x8uqf25wch.skadnetwork",
            "y45688jllp.skadnetwork",
            "238da6jt44.skadnetwork",
            "x2jnk7ly8j.skadnetwork",
            "v9wttpbfk9.skadnetwork",
            "n38lu8286q.skadnetwork",
            "9g2aggbj52.skadnetwork",
            "wzmmZ9fp6w.skadnetwork",
            "nu4557a4je.skadnetwork",
            "7953jerfzd.skadnetwork",
            "9yg77x724h.skadnetwork",
            "n66cz3y3bx.skadnetwork",
            "v4nxqhlyqp.skadnetwork",
            "24zw6aqk47.skadnetwork",
            "cs644xg564.skadnetwork",
            "9vvzujtq5s.skadnetwork",
            "a8cz6cu7e5.skadnetwork",
            "r26jy69rpl.skadnetwork",
            "dbu4b84rxf.skadnetwork",
            "3l6bd9hu43.skadnetwork",
            "488r3q3dtq.skadnetwork",
            "52fl2v3hgk.skadnetwork",
            "6v7lgmsu45.skadnetwork",
            "89z7zv988g.skadnetwork",
            "8m87ys6875.skadnetwork",
            "hb56zgv37p.skadnetwork",
            "m297p6643m.skadnetwork",
            "m5mvw97r93.skadnetwork",
            "vcra2ehyfk.skadnetwork",
            "ecpz2srf59.skadnetwork",
            "gvmwg8q7h5.skadnetwork",
            "nzq8sh4pbs.skadnetwork",
            "pu4na253f3.skadnetwork",
            "v79kvwwj4g.skadnetwork",
            "yrqqpx2mcb.skadnetwork",
            "z4gj7hsk7h.skadnetwork"
        };
    }
}
