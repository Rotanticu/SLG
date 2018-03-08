using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using maho;

namespace maho
{ 

public class GamePlayUI : MonoBehaviour {

        public HexGrid grid;
        HexCell currentCell;
        Fleet selectedUnit;
        public MyHexMapEditor MapEditor;
        public new Camera camera;
        public Canvas canvas;
         int roundIndex = 1;
        int activeCamp = 0;
        List<HexUnit> turnCamp = new List<HexUnit>();
        public Text 当前回合;
        public Button 结束回合;
        /* private void Awake()
         {
             for (int i = 0; i < grid.units.Count; i++)
             {
                 grid.units[i].camp = 0;
                 turnCamp.Add(grid.units[i]);

             }

         }*/
      
        void Update()
        {
            if (MapEditor==null || !MapEditor.编辑模式)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (!selectedUnit)
                        {
                            MouseDoSelection();
                          if(selectedUnit)
                              moveRange(selectedUnit);
                        }
                        else if (selectedUnit)
                        {
                        if (GetCellUnderCursor()&&GetCellUnderCursor().FleetUnit)
                          {
                              MouseDoSelection();
                              moveRange(selectedUnit);
                          }
                        else if (GetCellUnderCursor()&&!GetCellUnderCursor().FleetUnit)
                          {
                              if (selectedUnit.camp == 0)
                              {
                                  if (selectedUnit.step > GetCellUnderCursor().Distance)
                                  {
                                      DoMove(GetCellUnderCursor());
                                  }
                                  else if (selectedUnit.step < GetCellUnderCursor().Distance)
                                  {
                                      Refresh();
                                      selectedUnit = null;
                                  }
                              }
                            else if(selectedUnit.camp != 0)
                            {
                                Refresh();
                                selectedUnit = null;
                            }
                         }
                         }
                    }
                    if(Input.GetMouseButtonDown(1)
                        &&selectedUnit
                        && GetCellUnderCursor().FleetUnit.camp!=selectedUnit.camp
                        && GetCellUnderCursor().FleetUnit.camp!=2)
                    {
                        fight(selectedUnit, GetCellUnderCursor().FleetUnit);
                    }
                    else if (!selectedUnit)
                    {
                        Refresh();
                        UpdateMouseCell();
                    }
                    else if (selectedUnit)
                    {
                        moveRange(selectedUnit);
                        UpdateMouseCell();
                        DoPathfinding();
                    }
                }
            }
        }
        bool UpdateCurrentCell()
        {
            if (GetCellUnderCursor())
            {
                HexCell cell = GetCellUnderCursor();
            

            if (cell != currentCell)
            {
                currentCell = cell;
                currentCell.EnableHighlight(Color.red);
                return true;
            }
            return false;
        }
            return false;
        }
        void UpdateMouseCell()
        {
            if(GetCellUnderCursor())
            { 
            HexCell cell = GetCellUnderCursor();
            cell.EnableHighlight(Color.red);
            }
        }
        void MouseDoSelection()
        {
            UpdateCurrentCell();
            if (currentCell)
            {
                selectedUnit = currentCell.FleetUnit;
                
                
            }
        }
        public void KEySelection(Fleet fleet)
        {
            HexCell cell = fleet.Location;
            if(cell != currentCell)
            {
                currentCell = cell;
                currentCell.EnableHighlight(Color.red);
            }
            if (currentCell)
                selectedUnit = currentCell.FleetUnit;
            moveRange(selectedUnit);
        }
        void moveRange(Fleet unit)
        {
            Refresh();
            
            HexCell cell = unit.Location;
            List<HexCell> frontier = new List<HexCell>();
            cell.Distance = 0;
            frontier.Add(cell);
            while(frontier.Count > 0)
            {
                HexCell current = frontier[0];
                frontier.RemoveAt(0);
                for(HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    HexCell neighbor = current.GetNeighbor(d);
                    int distance = current.Distance;
                    if (neighbor == null || neighbor.FleetUnit)
                    {
                        continue;
                    }
                    if (neighbor.TerrainTypeIndex == 2)
                    {
                        continue;
                    }
                    
                    else
                    {
                        distance += 5;
                        if (distance > unit.step)
                        {
                            break;
                        }
                    }
                    if (neighbor.Distance == int.MaxValue)
                    {
                        neighbor.Distance = distance;
                        frontier.Add(neighbor);
                        neighbor.SetLabel(neighbor.Distance.ToString());
                        neighbor.EnableHighlight(Color.white);
                    }

                    
                    else if (distance < neighbor.Distance)
                    {
                        neighbor.Distance = distance;
                        neighbor.SetLabel(neighbor.Distance.ToString());
                        neighbor.EnableHighlight(Color.white);
                    }
                     
                     
                    frontier.Sort((x, y) => x.Distance.CompareTo(y.Distance));
                }
            }
        }
         void Refresh()
        {
            for (int i = 0; i < grid.cells.Length; i++)
            {
                grid.cells[i].Distance = int.MaxValue;
                grid.cells[i].SetLabel(" ");
                grid.cells[i].DisableHighlight();

            }
        }
        void DoPathfinding()
        {
            UpdateCurrentCell();
            if(currentCell)
            {
                grid.FindPath(selectedUnit.Location, currentCell, selectedUnit.step);
            }
        }
        HexCell GetCellUnderCursor()
        {
            if(Camera.main)
            { 
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                return grid.GetCell(hit.point);
            }
            }
            return null;

        }
        void DoMove(HexCell currentCell)
        {
            if(!selectedUnit.moved)
            {
                //selectedUnit.Location.FleetUnit = null;
                //selectedUnit.Location = currentCell;
                selectedUnit.travel(grid.GetPath());
                selectedUnit.moved = true;
            }
            else
            {
                Refresh();
                selectedUnit = null;
            }
        }
        public void Round()
        {
            for(int i = 0; i<turnCamp.Count;i++)
            {
                if (!turnCamp[i].moved)
                {
                    当前回合.text = "还有未移动单位！";
                    return;
                }
            }
            
            roundIndex++;
            当前回合.text = "第" + roundIndex.ToString() + "回合";
            for (int i = 0; i < turnCamp.Count; i++)
            {
                turnCamp[i].moved = false;
            }



        }
        public void activeCampUnits()
        {
            for (int i = 0; i < grid.fleetUnits.Count; i++)
            {
                if (grid.fleetUnits[i].camp == activeCamp)
                {
                    turnCamp.Add(grid.fleetUnits[i]);
                }
            }
        }
         void fight(Fleet attackerFleet,Fleet defenderFleet)
        {
            //Global.getInstance().LoadFight(attackerFleet, defenderFleet);
            SendFightFleet(attackerFleet, defenderFleet);
            grid.Save(MapGlobal.getInstance());
            loadingFightScence();
            grid.gameObject.SetActive(false);
            camera.gameObject.SetActive(false);
            
            canvas.gameObject.SetActive(false);
            
            
        }

        public void SendFightFleet(Fleet attacker, Fleet defender)
        {
            MapGlobal.getInstance().attacker = attacker.mapIndex;
            MapGlobal.getInstance().defender = defender.mapIndex;
        }
        public void loadingFightScence()
        {
            SceneManager.LoadSceneAsync("FightScence",LoadSceneMode.Additive);
        }

    }
}