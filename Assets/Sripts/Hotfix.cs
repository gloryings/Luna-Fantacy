using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Hotfix : MonoBehaviour
{
    Text txtError = null;
    Text txtDownloadURL = null;
    Text txtDownloadProgress = null;
    Text txtErrorHdiff = null;
    string zipDiffPath = null;
    string zipDiffURL = "http://10.10.12.4:8000/unity/Patch.patch";
    string newApkPath = null;
    void Start()
    {
        txtError = GameObject.Find("Canvas/Panel_Main/Button_Upgrade/Text_Error").GetComponent<Text>();
        txtDownloadProgress = GameObject.Find("Canvas/Panel_Main/Button_Download/Text_Progress").GetComponent<Text>();
        txtDownloadURL = GameObject.Find("Canvas/Panel_Main/Button_Hotfix/Text_DownloadURL").GetComponent<Text>();
        txtErrorHdiff = GameObject.Find("Canvas/Panel_Main/Button_Upgrade_Hdiff/Text_Error").GetComponent<Text>();
        zipDiffPath = Application.temporaryCachePath + "/Patch.patch";
        newApkPath = Application.temporaryCachePath + "/temp_new.apk";

        Button btn = GetComponent<Button>();
        if (btn.name == "Button_Download")
            btn.onClick.AddListener(downloadPatClick);
        else if (btn.name == "Button_Hotfix")
            btn.onClick.AddListener(hotUpdateClick);
        else if (btn.name == "Button_Upgrade")
            btn.onClick.AddListener(installUpdateClick);
        else if (btn.name == "Button_Upgrade_Hdiff")
            btn.onClick.AddListener(installUpdateHdiffClick);
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using (var patchClass = new AndroidJavaClass("com.github.sisong.HotUnity"))
            {
                patchClass.CallStatic("hotUnity", context);
            }
        }
#endif

    }

    private WWW downloadOperation = null;
    void Update()
    {
        if (downloadOperation != null)
        {
            if (!downloadOperation.isDone)
                txtDownloadProgress.text = ((int)(downloadOperation.progress * 100.0)).ToString() + "%";
            else
                txtDownloadProgress.text = "100%";
        }
    }
    private IEnumerator doDownload(string url, string localPath)
    {
        downloadOperation = new WWW(url);
        yield return downloadOperation;

        Byte[] b = downloadOperation.bytes;
        File.WriteAllBytes(localPath, b);
    }

    public void downloadPatClick()
    {
        Debug.Log("downloadPatClick");
        if (File.Exists(zipDiffPath)) File.Delete(zipDiffPath);
        Debug.Log("download URL: " + zipDiffURL);
        StartCoroutine(doDownload(zipDiffURL, zipDiffPath));
    }

    public void hotUpdateClick()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("hotUpdateClick");
        if (!File.Exists(zipDiffPath)) {
            txtError.text = "hot update error, zipDiffPath not exists ";
            return;
        }
        using (AndroidJavaClass HotUnity = new AndroidJavaClass("com.github.sisong.HotUnity"))
        {
            int patchResult=HotUnity.CallStatic<int>("apkPatch",zipDiffPath,1,"");
            if (patchResult==0) { //ok
                File.Delete(zipDiffPath);
                HotUnity.CallStatic("restartApp");
            }else{ //error
                txtError.text="hot update error, HotUnity.apkPatch() result "+patchResult.ToString();
            }
        }
#endif
    }
    public void installUpdateClick()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("installUpdateClick");
        if (!File.Exists(zipDiffPath)) {
            txtError.text = "install update error, zipDiffPath not exists ";
            return;
        }
        //using (AndroidJavaClass HotUnity = new AndroidJavaClass("com.github.sisong.HotUnity"))
        //{
        //    if (File.Exists(newApkPath)) File.Delete(newApkPath);

        //    //int patchResult = HotUnity.CallStatic<int>("apkPatch", zipDiffPath, 3, newApkPath);
        //    //if (patchResult == 0) { //ok
        //    //    File.Delete(zipDiffPath);
        //    //    HotUnity.CallStatic("installApk", newApkPath);
        //    //}else{ //error
        //    //    txtError.text = "install update error, HotUnity.apkPatch() result " + patchResult.ToString();
        //    //}

        //    // 创建回调代理
        //    var callback = new PatchCallbackProxy((resultCode) =>
        //    {
        //        if (resultCode == 0)
        //        {
        //            Debug.Log("Patch success!");
        //            // 调用安装
        //            HotUnity.CallStatic("installApk", newApkPath);
        //        }
        //        else
        //        {
        //            Debug.LogError("Patch failed with code: " + resultCode);
        //            txtError.text = "install update error, HotUnity.apkPatchAsync() result " + resultCode;
        //        }
        //    });

        //     HotUnity.CallStatic("apkPatchAsync", zipDiffPath, 3, newApkPath, callback); //异步调用
        //}
        
        AndroidJavaClass hotUnity = new AndroidJavaClass("com.github.sisong.HotUnity");

        var callback = new PatchCallbackProxy((resultCode) =>
        {
            if (resultCode == 0)
            {
                Debug.Log("Patch success!");
                hotUnity.CallStatic("installApk", newApkPath);
            }
            else
            {
                Debug.LogError("Patch failed with code: " + resultCode);
                txtErrorHdiff.text = "install update error, apkPatchAsync() result " + resultCode;
            }
        });

        hotUnity.CallStatic("apkPatchAsync", zipDiffPath, 3, newApkPath, callback); //异步调用
        
        //StartCoroutine(DoInstallUpdateAsync());
#endif
    }

    public void onPatchResult(string result)
    {
        int ret = int.Parse(result);
        Debug.Log("Patch result: " + ret);

        if (ret == 0)
        {
            File.Delete(zipDiffPath); // 删除 patch
#if UNITY_ANDROID && !UNITY_EDITOR
                using (AndroidJavaClass HotUnity = new AndroidJavaClass("com.github.sisong.HotUnity"))
                {
                    HotUnity.CallStatic("installApk", newApkPath); // 安装
                }
#endif
        }
        else
        {
            txtError.text = "install update error, apkPatchAsync() result = " + ret;
        }
    }

    private IEnumerator DoInstallUpdate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        yield return null; // 下一帧开始执行，避免阻塞 UI

        using (AndroidJavaClass HotUnity = new AndroidJavaClass("com.github.sisong.HotUnity"))
        {
            if (File.Exists(newApkPath)) File.Delete(newApkPath);

            int patchResult = HotUnity.CallStatic<int>("apkPatch", zipDiffPath, 3, newApkPath);
            if (patchResult == 0) { //ok
                File.Delete(zipDiffPath);
                HotUnity.CallStatic("installApk", newApkPath);
            }else{ //error
                txtError.text = "install update error, HotUnity.apkPatch() result " + patchResult.ToString();
            }
        }
#else
        yield return null;
#endif
    }

    public void installUpdateHdiffClick()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("installUpdateHdiffClick");
        if (!File.Exists(zipDiffPath)) {
            txtErrorHdiff.text = "install update error, zipDiffPath not exists ";
            return;
        }
        //using (AndroidJavaClass HotUnity = new AndroidJavaClass("com.github.sisong.HotUnity"))
        //{
        //    if (File.Exists(newApkPath)) File.Delete(newApkPath);

        //    // 创建回调代理
        //    var callback = new PatchCallbackProxy((resultCode) =>
        //    {
        //        if (resultCode == 0)
        //        {
        //            Debug.Log("Patch success!");
        //            // 调用安装
        //            HotUnity.CallStatic("installApk", newApkPath);
        //        }
        //        else
        //        {
        //            Debug.LogError("Patch failed with code: " + resultCode);
        //            txtErrorHdiff.text = "install update error, HotUnity.apkPatchAsync() result " + resultCode;
        //        }
        //    });

        //    long cacheMemory = 64L * 1024 * 1024; // 64MB
        //    HotUnity.CallStatic("hdiffPatchAsync", zipDiffPath, newApkPath,  cacheMemory, callback); //异步调用
        //}


        AndroidJavaClass hotUnity = new AndroidJavaClass("com.github.sisong.HotUnity");

        var callback = new PatchCallbackProxy((resultCode) =>
        {
            if (resultCode == 0)
            {
                Debug.Log("Patch success!");
                hotUnity.CallStatic("installApk", newApkPath);
            }
            else
            {
                Debug.LogError("Patch failed with code: " + resultCode);
                txtErrorHdiff.text = "install update error, hdiffPatchAsync() result " + resultCode;
            }
        });
        long cacheMemory = 64L * 1024 * 1024; // 64MB
        hotUnity.CallStatic("hdiffPatchAsync", zipDiffPath, newApkPath,  cacheMemory, callback); //异步调用
#endif
    }
}

/// <summary>
/// Java 回调接口代理类（对应 Java 中的 PatchCallback）
/// </summary>
public class PatchCallbackProxy : AndroidJavaProxy
{
    private readonly Action<int> onResult;

    public PatchCallbackProxy(Action<int> onResultCallback)
           : base("com.github.sisong.HotUnity$PatchCallback") // <-- 这里必须是内部类名，带$
    {
        this.onResult = onResultCallback;
    }

    // Java 会调用这个方法
    public void onPatchResult(int resultCode)
    {
        onResult?.Invoke(resultCode);
    }
}
