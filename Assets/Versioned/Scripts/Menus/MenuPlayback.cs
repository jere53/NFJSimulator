using System;
using SimpleFileBrowser;
using UnityEngine;

public class MenuPlayback : MonoBehaviour
{
    public PlaybackManager PlaybackManager;
    
    public void OnButtonPlayPressed()
    {

        string initialPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        if(FileBrowser.IsOpen) return;
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Grabaciones", ".nfj"));
        FileBrowser.SetDefaultFilter(".nfj");
        FileBrowser.ShowLoadDialog(ComenzarPlayback, null, FileBrowser.PickMode.Files,
            false, initialPath);
    }

    private void ComenzarPlayback(string[] paths)
    {
        string path = paths[0];
        PlaybackManager.BeginPlayback(FileBrowserHelpers.GetDirectoryName(path) + "\\"
            , FileBrowserHelpers.GetFilename(path));
    }
}
