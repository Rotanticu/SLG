using maho;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class SaveLoadMenu : MonoBehaviour {

    public HexGrid hexGrid;
    bool saveMode;
    public Text actionButtonLabel;
    public InputField nameInput;
    public RectTransform listContent;
    public SaveLoadItem itemPrefab;
    
    public void Open(bool saveMode)
    {
        this.saveMode = saveMode;
        if (saveMode)
        {
            
            actionButtonLabel.text = "保存";
        }
        else
        {
            
            actionButtonLabel.text = "读取";
        }
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
            item.menu = this;
            item.MapName =Path.GetFileNameWithoutExtension(paths[i]);
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
        if (saveMode)
        {
            Save(path);
        }
        else
        {
            Load(path);
        }
        Close();
    }
    void Save(string path)
    {
        //      string path = Path.Combine(Application.streamingAssetsPath + "\\Save", "test.map");
        using (
            BinaryWriter writer =
            new BinaryWriter(File.Open(path, FileMode.Create))
        )
        {
            writer.Write(7);
            MapGlobal.getInstance().Clear(MapGlobal.getInstance());
            hexGrid.Save(MapGlobal.getInstance());
            hexGrid.SaveMapFile(writer,MapGlobal.getInstance());
        }
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
                hexGrid.ReadMapFile(path);
                hexGrid.Load(MapGlobal.getInstance());
                
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
