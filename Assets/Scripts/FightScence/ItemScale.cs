using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemScale : MonoBehaviour {

    // Use this for initialization
    public int 每页显示数量;
    void Start()
    {
        Scale();
    }
    public void Scale()
    {
        
        var grid = transform.Find("Content").GetComponent<GridLayoutGroup>();
        var rect = GetComponent<RectTransform>().rect;
        if(grid.transform.childCount > 1)
        { 
        grid.cellSize = new Vector2(rect.width, rect.height / (每页显示数量 + 1));
        grid.spacing = new Vector2(0, rect.height / ((每页显示数量 + 1) * (grid.transform.childCount - 1)));
        }
        else
        {
            grid.cellSize = new Vector2(rect.width, rect.height / (每页显示数量 + 1));
            grid.spacing = new Vector2(0, 0);

        }

    }
    // Update is called once per frame
    void Update () {
		
	}
}
