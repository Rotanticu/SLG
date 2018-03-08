using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using maho;
using System.IO;

public class FastUnitSelection : MonoBehaviour {

    public HexMapCamera Camera;
    public HexUnit selectUnit;
   
    public bool Moved = true;
    Vector3 Vector3, TempVector3;
    public List<Fleet> SelectFleet;
    public HexGrid Grid;
    public SelectionUnit SelectUnitPrefab;
    public RectTransform listContent;
    public GamePlayUI GamePlayUI;

    public void LoadSheet()
    {
        for(int i = 0; i < listContent.childCount; i++)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }

        for (int i = 0; i < Grid.fleetUnits.Count; i++)
        {
            if (Grid.fleetUnits[i].camp == 0)
            {
                SelectFleet.Add(Grid.fleetUnits[i]);
            }
        }
        for(int i = 0; i < SelectFleet.Count; i++)
        {
            
            SelectionUnit SelectionUnit = Instantiate(SelectUnitPrefab);
            SelectionUnit.FastSclectList = this;
            SelectionUnit.transform.SetParent(listContent, false);
            SelectionUnit.Initialization();
            SelectionUnit.index = i;
            SelectionUnit.text.text = SelectFleet[i].Name;
            //SelectionUnit.image = SelectFleet[i].mapShipList[0].index
            
        }
        Begin();
    }

    void Begin()
    {
        if(SelectFleet[0])
        {
            Move(0);
        }
    }
    public void Move(int index)
    {
        selectUnit = SelectFleet[index];
        GamePlayUI.KEySelection(SelectFleet[index]);
        
        Vector3.y = Camera.transform.position.y;
        Vector3.x = selectUnit.Location.transform.position.x;
        Vector3.z = -selectUnit.Location.transform.position.y;
        Moved = false;
    }
    
    public void  UnitCamera()
    {
        

        TempVector3.x = Mathf.Lerp(Camera.transform.position.x, Vector3.x, 5*Time.deltaTime);
        TempVector3.y = Mathf.Lerp(Camera.transform.position.y, Vector3.y, 5*Time.deltaTime);
        TempVector3.z = Mathf.Lerp(Camera.transform.position.z, Vector3.z, 5*Time.deltaTime);
        Camera.transform.position = TempVector3;


    }
    void Key()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (9 < SelectFleet.Count)
            {
                Move(9);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (0 < SelectFleet.Count)
            {
                Move(0);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (1 < SelectFleet.Count)
            {
                Move(1);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (2 < SelectFleet.Count)
            {
                Move(2);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (3 < SelectFleet.Count)
            {
                Move(3);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (4 < SelectFleet.Count)
            {
                Move(4);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (5 < SelectFleet.Count)
            {
                Move(5);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (6 < SelectFleet.Count)
            {
                Move(6);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            if (7 < SelectFleet.Count)
            {
                Move(7);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (8 < SelectFleet.Count)
            {
                Move(8);
            }
        }

    }
 
    private void Update()
    {
        Key();
        if (!Moved )
        {
          
            if (-1 > Camera.transform.position.z - Vector3.z || Camera.transform.position.z - Vector3.z > 1)
            {
                
                UnitCamera();
            }
            else
            {
                Moved = true;
            }
            
        }
    }
}
