using maho;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class StartLoad : MonoBehaviour
{

    public Text actionButtonLabel;
    public InputField nameInput;
    public RectTransform listContent;
    public SaveLoadItem itemPrefab;

    public void Open()
    {

            actionButtonLabel.text = "读取";
        FillList();
        gameObject.SetActive(true);

    }

    public void Close()
    {
        gameObject.SetActive(false);

    }
    string GetSelectedPath()
    {
        string mapName = nameInput.text;
        if (mapName.Length == 0)
        {
            return null;
        }
        return Path.Combine(Application.streamingAssetsPath + "\\Save", mapName + ".map");
    }
    public void SelectItem(string name)
    {
        nameInput.text = name;
    }
    void FillList()
    {
        for (int i = 0; i < listContent.childCount; i++)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }
        string[] paths =

            Directory.GetFiles(Application.streamingAssetsPath + "\\Save", "*.map");
        Array.Sort(paths);
        for (int i = 0; i < paths.Length; i++)
        {

            SaveLoadItem item = Instantiate(itemPrefab);
            item.startMenu = this;
            item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
            item.transform.SetParent(listContent, false);

        }
    }
    public void Action()
    {


        string path = GetSelectedPath();
        if (path == null)
        {
            return;
        }
        else
        {

            Load(path);
        }
        Close();
    }

    void Load(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File does not exist " + path);
            return;
        }
        //      string path = Path.Combine(Application.streamingAssetsPath + "\\Save", "test.map");
        using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
        {
            int header = reader.ReadInt32();
            if (header <= 7)
            {
                MapGlobal.getInstance().path = path;
                SceneManager.LoadScene("Start 1");


            }
            else
            {
                Debug.LogWarning("Unknown map format " + header);
            }
        }
    }
    public void Delete()
    {
        string path = GetSelectedPath();
        //path = path.Substring(2);
        if (path == null)
        {
            return;
        }
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        nameInput.text = "";
        FillList();
    }


}
