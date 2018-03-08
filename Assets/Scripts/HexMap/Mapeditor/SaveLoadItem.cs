using UnityEngine;
using UnityEngine.UI;

public class SaveLoadItem : MonoBehaviour
{

    public SaveLoadMenu menu;
    public StartLoad startMenu;

    public string MapName
    {
        get
        {
            return mapName;
        }
        set
        {
            mapName = value;
             transform.GetChild(0).GetComponent<Text>().text ="  " + value;
        }
    }

    string mapName;

    public void Select()
    {
        if(menu == null)
        {
            StartSelect();
        }
        else if(startMenu == null)
        {
            menu.SelectItem(mapName);
        }
    }
    public void StartSelect()
    {
        startMenu.SelectItem(mapName);
    }


}