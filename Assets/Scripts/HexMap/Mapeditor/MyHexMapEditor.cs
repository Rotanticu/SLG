using maho;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
namespace maho {
public class MyHexMapEditor : MonoBehaviour {
    public HexGrid hexGrid;
        //public Color[] colors;
        // private Color activeColor;
        int activeTerrainTypeIndex;
        int activeUnitCamp;
        public bool 编辑模式;
       public GameObject UI;
        //public HexUnit unitPrefab;

        HexCell previousCell;
        //HexCell searchFromCell, searchToCell;

    // Use this for initialization
    void Awake()
    {
        //SelectColor(0);

    }
    void Update()
    {
        if (编辑模式)
            {
                
                UI.SetActive(编辑模式);
            if (!EventSystem.current.IsPointerOverGameObject())
            {

                    if (Input.GetMouseButton(0))
                {
                    HandleInput();
                    return;
                }
                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    CreateUnit();
                    return;
                }
                if(Input.GetKeyDown(KeyCode.Q))
                {
                    DestroyUnit();
                    return;
                }
                
            }
            previousCell = null;
            }
            else
            {
                UI.SetActive(编辑模式);
            }

        }
        //获取鼠标位置
     HexCell GetCellUnderCursor ()
{
    Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
    if (Physics.Raycast(inputRay, out hit))
    {
        return hexGrid.GetCell(hit.point);
    }
    return null;
}
        void CreateUnit()
        {
            HexCell cell = GetCellUnderCursor();
            if (cell && !cell.FleetUnit)
            {
                //HexUnit unit = Instantiate(HexUnit.unitPrefab);
                // unit.transform.SetParent(hexGrid.transform, false);
                //unit.Location = cell;
                //cell.Unit = hexGrid.unitPrefab;
                if (activeUnitCamp == 0)
                {
                    Fleet newFleet = Instantiate(hexGrid.unitPrefabs[0]);
                    hexGrid.AddFleetUnit(newFleet, cell,activeUnitCamp);
                    cell.FleetUnit.camp = 0;
                    for(int i = 0; i<newFleet.shipTypeIndex.Count; i++)
                    {
                        MapShip newShip = Instantiate(hexGrid.shipPrefabs[newFleet.shipTypeIndex[i]]);
                        newFleet.mapShipList.Add(newShip);
                        newShip.transform.SetParent(newFleet.transform);

                    }
                }
                if (activeUnitCamp == 1)
                {
                    Fleet newFleet = Instantiate(hexGrid.unitPrefabs[1]);
                    hexGrid.AddFleetUnit(newFleet, cell, activeUnitCamp);
                    cell.FleetUnit.camp = 1;
                    for (int i = 0; i < newFleet.shipTypeIndex.Count; i++)
                    {
                        MapShip newShip = Instantiate(hexGrid.shipPrefabs[newFleet.shipTypeIndex[i]]);
                        newFleet.mapShipList.Add(newShip);
                        newShip.transform.SetParent(newFleet.transform);
                    }

                }
                if (activeUnitCamp == 2)
                {
                    Fleet newFleet = Instantiate(hexGrid.unitPrefabs[2]);
                    hexGrid.AddFleetUnit(newFleet, cell, activeUnitCamp);
                    cell.FleetUnit.camp = 2;
                    for (int i = 0; i < newFleet.shipTypeIndex.Count; i++)
                    {
                        MapShip newShip = Instantiate(hexGrid.shipPrefabs[newFleet.shipTypeIndex[i]]);
                        newFleet.mapShipList.Add(newShip);
                        newShip.transform.SetParent(newFleet.transform);
                    }

                }


            }
        }
        void DestroyUnit()
        {
            HexCell cell = GetCellUnderCursor();
            if (cell && cell.FleetUnit)
            {
                hexGrid.RemoveFleetUnit((Fleet)cell.FleetUnit);
               cell.FleetUnit.Die();

            }
        }
        //旧版的获取鼠标代码，牵扯到寻路，暂时未优化
           void HandleInput()
       {
           Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
           RaycastHit hit;
               if (Physics.Raycast(inputRay, out hit))
               {
                   HexCell currentCell = hexGrid.GetCell(hit.point);
                       EditCell(hexGrid.GetCell(hit.point));
                  /*  else if (Input.GetKey(KeyCode.LeftShift)&& searchToCell != currentCell)
                    { 

                        if (searchFromCell)
                        {
                            searchFromCell.DisableHighlight();
                        }

                     searchFromCell = currentCell;
                     searchFromCell.EnableHighlight(Color.blue);

                        if (searchToCell)
                        {
                            hexGrid.FindPath(searchFromCell, searchToCell);
                        }
                    }
                    else if (searchFromCell && searchFromCell != currentCell)
                    {
                        searchToCell = currentCell;
                        hexGrid.FindPath(searchFromCell, searchToCell);
                    }*/
                    previousCell = currentCell;
               }
               else
               {
                   previousCell = null;
               }
           }

        void EditCell(HexCell cell)
        {
            //cell.color = activeColor;
            if(cell)
            {
                if(activeTerrainTypeIndex >= 0)
                {
                    cell.TerrainTypeIndex = activeTerrainTypeIndex;
                }
            }
            hexGrid.Refresh();
            
        }
        //  public void SelectColor (int index)
        // {
        //     activeColor = colors[index];
        // }
        public void SetTerrainTypeIndex(int index)
        {
            activeTerrainTypeIndex = index;
        }
        public void setUnitCamp(int camp)
        {
            activeUnitCamp = camp;
        }

        /*
        public void Save()
        {
            Debug.Log(Application.streamingAssetsPath + "\\Save");
            string path = Path.Combine(Application.streamingAssetsPath + "\\Save", "test.map");
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(2);
                hexGrid.Save(writer);
            }
            //writer.Close();
        }

        public void Load()
        {
            string path = Path.Combine(Application.streamingAssetsPath + "\\Save", "test.map");
            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
            {
                int header = reader.ReadInt32();
                if (header == 2)
                {
                    hexGrid.Load(reader);
                    
                    
                }
                else
                {
                    Debug.LogWarning("无法识别的地图格式" + header);
                }
            }
        }
        */
    }
}