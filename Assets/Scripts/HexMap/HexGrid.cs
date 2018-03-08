using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
namespace maho { 
    /*非常重要的类，负责网格的所用工作
    *包括网格生成，网格序号，和搜索寻路*/
public class HexGrid : MonoBehaviour {
        public int height;
        public int width;
        //获取细胞预制体和细胞文本预制体，在unity的Inspector设置
        public HexCell cellPrefab;
        public Text cellLabelPrefab;
        int searchFrontierPhase;
        GameObject gridCanvas;
        HexCell currentPathForm, currentPathTo;
        bool currentPathExists;

        public   HexCell[] cells;
        public Color[] colors;
        public Fleet [] unitPrefabs;
        public MapShip[] shipPrefabs;
        public List<Fleet> fleetUnits = new List<Fleet>();
        public GamePlayUI gamePlayUI;
        public FastUnitSelection FastUnitSelection;
        

        HexMesh hexMesh;
        HexCellPriorityQueue searchFrontier;

       // public Color defaultColor = Color.white;

            public bool HasPath
        {
            get
            {
                return currentPathExists;
            }
        }
        
        

        void Start()
        {
            if (MapGlobal.getInstance().mapHeader == 0 && MapGlobal.getInstance().path != null)
            {
                ReadMapFile(MapGlobal.getInstance().path);
                Load(MapGlobal.getInstance());

            }

            else if (MapGlobal.getInstance().mapHeader != 0)
            {
                Load(MapGlobal.getInstance());
            }
            
            hexMesh.Triangulate(cells);
        }
        
        public void Awake()
        {
            HexMetrics.colors = colors;
            hexMesh = GetComponentInChildren<HexMesh>();
            HexUnit.unitPrefab = unitPrefabs[0];
            gridCanvas = GameObject.Find("HexCanvas");
            //CreateMap(width,height);



        }

        public void CreateMap(int width, int height)
        {
            ClearPath();
            for(int i = 0;i<cells.Length;i++)
            {
                cells[i].SetLabel(null);
                cells[i].DisableHighlight();
                if(cells[i].FleetUnit)
                {
                    cells[i].FleetUnit.Die();
                }
                Destroy(cells[i].gameObject);
            }
            ClearUnits();
            cells = new HexCell[width * height];
            
            for (int z = 0, i = 0; z < width; z++)
            {
                for (int x = 0; x < height; x++)
                {
                    CreateCell(x, z, i++);
                }
            }
            Refresh();

        }
        //创建网格，也负责让网格互相记录自己的邻居
        void CreateCell(int x, int z, int i)
        {
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f);

            HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
            cell.transform.SetParent(gridCanvas.transform, false);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            //cell.color = defaultColor;
            if (x > 0)
            {
                cell.SetNeighbor(HexDirection.W, cells[i - 1]);
            }
            if (z > 0)
            {
                if ((z & 1) == 0)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - height]);
                    if (x > 0)
                    {
                        cell.SetNeighbor(HexDirection.SW, cells[i - height - 1]);
                    }
                }
                else
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - height]);
                    if (x < height - 1)
                    {
                        cell.SetNeighbor(HexDirection.SE, cells[i - height + 1]);
                    }
                }
            }

            Text label = Instantiate<Text>(cellLabelPrefab);
            label.rectTransform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
            cell.uiRect = label.rectTransform;

            
        }
        //很重要的函数，通过position（即由ScreenPointToRay得到的鼠标点击位置）选中网格，返回网格序号
        public HexCell GetCell(Vector3 position)
        {
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            int index = coordinates.X + coordinates.Z * height + coordinates.Z / 2;
            //Debug.Log("touched at position = " + position + ". coordinates = " + coordinates.ToString() + ". index = " + index + ", 地形type = " + cells[index].TerrainTypeIndex );
            return cells[index];
            
        }
        public HexCell GetCell(HexCoordinates coordinates)
        {
            int z = coordinates.Z;
            if (z < 0 || z >= width)
            {
                return null;
            }
            int x = coordinates.X + z / 2;
            if (x < 0 || x >= height)
            {
                return null;
            }
            return cells[x + z * height];
        }
        //重新三角化，刷新网格
        public void Refresh()
        {
            hexMesh.Triangulate(cells);
        }
        public void AddFleetUnit(Fleet fleet, HexCell location,int camp)
        {
            fleetUnits.Add(fleet);
            fleet.transform.SetParent(transform, false);
            fleet.Location = location;
            fleet.camp = camp;
            location.FleetUnit.camp = camp;
            Vector3 y = fleet.transform.position;
            y.y = 2;
            fleet.transform.position = y;
           
            
            
        }
        public void RemoveFleetUnit(Fleet fleetUnit)
        {
            fleetUnits.Remove(fleetUnit);
        }
        void ClearUnits()
        {
            for (int i = 0; i < fleetUnits.Count; i++)
            {
                fleetUnits[i].Die();
            }
            fleetUnits.Clear();
        }

        public void Save(MapGlobal MapGlobal)
        {
            MapGlobal.height = height;
            MapGlobal.width = width;

            MapGlobal.cells = new int[height * width];
            for (int i = 0; i < cells.Length; i++)
            {
                MapGlobal.cells[i] = cells[i].terrainTypeIndex;
            }

            MapGlobal.unitCount = fleetUnits.Count;

            MapGlobal.Fleets = new MapGlobal.Fleet[fleetUnits.Count];
            for (int i = 0; i < fleetUnits.Count; i++)
            {
                fleetUnits[i].location.coordinates.Save(MapGlobal,i);
                MapGlobal.Fleets[i].moved =  fleetUnits[i].location.FleetUnit.moved;
                MapGlobal.Fleets[i].camp = fleetUnits[i].location.FleetUnit.camp;
                MapGlobal.Fleets[i].fleetTypeIndex = fleetUnits[i].location.FleetUnit.fleetTypeIndex;
                MapGlobal.Fleets[i].mapIndex = fleetUnits[i].location.FleetUnit.mapIndex;
                MapGlobal.Fleets[i].shipCount = fleetUnits[i].mapShipList.Count;

                MapGlobal.Fleets[i].MapShip = new MapGlobal.MapShip[fleetUnits[i].mapShipList.Count];
                for (int j = 0; j < fleetUnits[i].mapShipList.Count; j++)
                {
                    MapGlobal.Fleets[i].MapShip[j].maxDefence = fleetUnits[i].mapShipList[j].MaxDefence;
                    MapGlobal.Fleets[i].MapShip[j].defence = fleetUnits[i].mapShipList[j].defence;
                    MapGlobal.Fleets[i].MapShip[j].attack = fleetUnits[i].mapShipList[j].attack;
                    MapGlobal.Fleets[i].MapShip[j].speed = fleetUnits[i].mapShipList[j].speed;
                    MapGlobal.Fleets[i].MapShip[j].shipTypeIndex = fleetUnits[i].mapShipList[j].shipTypeIndex;

                }
            }

        }
        public void SaveMapFile(BinaryWriter writer, MapGlobal MapGlobal)
        {
            writer.Write(MapGlobal.height);
            writer.Write(MapGlobal.width);
            for (int i = 0; i < MapGlobal.cells.Length; i++)
            {
                writer.Write(MapGlobal.cells[i]);
            }
            writer.Write(MapGlobal.unitCount);

            for (int i = 0; i < MapGlobal.unitCount; i++)
            {
                writer.Write(MapGlobal.Fleets[i].CoordinatesX);
                writer.Write(MapGlobal.Fleets[i].CoordinatesZ);
                writer.Write(MapGlobal.Fleets[i].moved);
                writer.Write(MapGlobal.Fleets[i].camp);
                writer.Write(MapGlobal.Fleets[i].fleetTypeIndex);
                writer.Write(MapGlobal.Fleets[i].mapIndex);
                writer.Write(MapGlobal.Fleets[i].shipCount);

                for (int j = 0; j < MapGlobal.Fleets[i].MapShip.Length; j++)
                {
                    writer.Write(MapGlobal.Fleets[i].MapShip[j].maxDefence);
                    writer.Write(MapGlobal.Fleets[i].MapShip[j].defence);
                    writer.Write(MapGlobal.Fleets[i].MapShip[j].attack);
                    writer.Write(MapGlobal.Fleets[i].MapShip[j].speed);
                    writer.Write(MapGlobal.Fleets[i].MapShip[j].shipTypeIndex);

                }
            }
        }
        public void Load(MapGlobal MapGlobal)
        {
            ClearPath();
            ClearUnits();
            int x , z ;
            x = MapGlobal.height;
            z = MapGlobal.width;
            width = z;
            height = x;
            CreateMap(z, x);
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Load(MapGlobal, i);
            }
            Refresh();
            int unitCount = MapGlobal.unitCount;
            for (int i = 0; i < unitCount; i++)
            {
                Fleet.FleetShipsLoad(MapGlobal, this,i);
            }
            gamePlayUI.activeCampUnits();
            if(FastUnitSelection)
            {
                FastUnitSelection.LoadSheet();
            }
            this.Save(MapGlobal.getInstance());
        }

        public void ReadMapFile(string path)
        {
            BinaryReader reader = new BinaryReader(File.OpenRead(path));

            MapGlobal.getInstance().mapHeader = reader.ReadInt32();

            MapGlobal.getInstance().height = reader.ReadInt32();
            MapGlobal.getInstance().width = reader.ReadInt32();

            MapGlobal.getInstance().cells = new int[MapGlobal.getInstance().width * MapGlobal.getInstance().height];

            for (int i = 0; i < MapGlobal.getInstance().width * MapGlobal.getInstance().height; i++)
            {
                MapGlobal.getInstance().cells[i] = reader.ReadInt32(); ;
            }


            MapGlobal.getInstance().unitCount = reader.ReadInt32();

            MapGlobal.getInstance().Fleets = new MapGlobal.Fleet[MapGlobal.getInstance().unitCount];

            for (int i = 0; i < MapGlobal.getInstance().unitCount; i++)
            {
                MapGlobal.getInstance().Fleets[i].CoordinatesX = reader.ReadInt32();
                MapGlobal.getInstance().Fleets[i].CoordinatesZ = reader.ReadInt32();
                MapGlobal.getInstance().Fleets[i].moved = reader.ReadBoolean();
                MapGlobal.getInstance().Fleets[i].camp = reader.ReadInt32();
                MapGlobal.getInstance().Fleets[i].fleetTypeIndex = reader.ReadInt32();
                MapGlobal.getInstance().Fleets[i].mapIndex = reader.ReadInt32();
                MapGlobal.getInstance().Fleets[i].shipCount = reader.ReadInt32();

                MapGlobal.getInstance().Fleets[i].MapShip = new MapGlobal.MapShip[MapGlobal.getInstance().Fleets[i].shipCount];
                for (int j = 0; j < MapGlobal.getInstance().Fleets[i].MapShip.Length; j++)
                {
                    MapGlobal.getInstance().Fleets[i].MapShip[j].maxDefence = reader.ReadInt32();
                    MapGlobal.getInstance().Fleets[i].MapShip[j].defence = reader.ReadInt32();
                    MapGlobal.getInstance().Fleets[i].MapShip[j].attack = reader.ReadInt32();
                    MapGlobal.getInstance().Fleets[i].MapShip[j].speed = reader.ReadInt32();
                    MapGlobal.getInstance().Fleets[i].MapShip[j].shipTypeIndex = reader.ReadInt32();

                }

            }
        }


        public List<HexCell> GetPath()
        {
            if(!currentPathExists)
            {
                return null;
            }
            List<HexCell> path = ListPool<HexCell>.Get();
            for(HexCell c = currentPathTo; c != currentPathForm; c = c.PathFrom)
            {
                path.Add(c);
            }
            path.Add(currentPathForm);
            path.Reverse();
            return path;
        }
       
        /*public void ColorCell(Vector3 position,Color color)
        {
            position = transform.InverseTransformPoint(position);
            HexCoordinates coordinates = HexCoordinates.FromPosition(position);
            int index = coordinates.X + coordinates.Z * height + coordinates.Z / 2;
            HexCell cell = cells[index];
            //cell.color = color;
            hexMesh.Triangulate(cells);
           
        }*/
        //以下是搜索和寻路算法
        //通过neighbor获取每一个网格的相邻网格，实现树状网络搜索
        
        public void FindPath(HexCell fromCell, HexCell toCell,int step)
        {
            //StopAllCoroutines();
            //StartCoroutine(Search(fromCell, toCell,step));
            ClearPath();
            currentPathForm = fromCell;
            currentPathTo = toCell;
            //Search(fromCell, toCell, step);
            currentPathExists = Search(fromCell, toCell, step);
            if(currentPathExists)
            {
                ShowPath(step);
            }
            
        }

        void ShowPath(int step)
        {
            if(currentPathExists)
            {
                HexCell current = currentPathTo;
                while(current!= currentPathForm)
                {
                    int turn = (current.Distance - 1) / step;
                    current.SetLabel(turn.ToString());
                    current.EnableHighlight(Color.yellow);
                    current = current.PathFrom;
                }
            }

            currentPathForm.EnableHighlight(Color.blue);
            currentPathTo.EnableHighlight(Color.red);
        }
        void ClearPath()
        {
            if (currentPathExists)
            {
                HexCell current = currentPathTo;
                while (current != currentPathForm)
                {
                    current.SetLabel(null);
                    current.DisableHighlight();
                    current = current.PathFrom;
                }
                current.DisableHighlight();
                currentPathExists = false;
            }

            currentPathForm = currentPathTo = null;
        }
       bool  Search(HexCell fromCell, HexCell toCell,int step)
        {
            searchFrontierPhase += 2;
            if(searchFrontier == null)
            {
                searchFrontier = new HexCellPriorityQueue();
            }
            else
            {
                searchFrontier.Clear();
            }

            /*
            for (int i = 0; i < cells.Length; i++)
            {
                //cells[i].Distance = int.MaxValue;
                cells[i].DisableHighlight();
                cells[i].SetLabel(null);
            }
            fromCell.EnableHighlight(Color.blue);
            //toCell.EnableHighlight(Color.red);
            */

            //WaitForSeconds delay = new WaitForSeconds(1 / 300f);
            //List<HexCell> frontier = new List<HexCell>();
            //frontier.Add(fromCell);

            fromCell.SearchPhase = searchFrontierPhase;
            fromCell.Distance = 0;
            searchFrontier.Enqueue(fromCell);
            while (searchFrontier.Count > 0)
            {
               // yield return delay;
                HexCell current = searchFrontier.Dequeue();
                current.SearchPhase += 1;
                //frontier.RemoveAt(0);

                if (current == toCell)
                {
                    return true;
                    //current = current.PathFrom;
                    /*
                    while (current != fromCell)
                    {
                        int turn = current.Distance / step;
                        current.SetLabel(turn.ToString());
                        current.EnableHighlight(Color.white);
                        current = current.PathFrom;
                    }
                    toCell.EnableHighlight(Color.red);
                    break;
                    */
                }
                //int currentTurn = ( current.Distance - 1) / step;
                for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    HexCell neighbor = current.GetNeighbor(d);
                    if (neighbor == null||neighbor.SearchPhase>searchFrontierPhase)
                    {
                        continue;
                    }
                   if(neighbor.FleetUnit)
                    {
                        continue;
                    }
                    
                    int distance = current.Distance;
                    if(neighbor.TerrainTypeIndex == 2)
                    {
                        continue;
                    }
                    else
                    {
                        distance += 5;
                    }
                    //int turn = (distance - 1) / step;


                   // if (neighbor.Distance == int.MaxValue)
                   if(neighbor.SearchPhase<searchFrontierPhase)
                    {
                        neighbor.SearchPhase = searchFrontierPhase;
                        neighbor.Distance = distance;
                        //neighbor.SetLabel(turn.ToString());
                        neighbor.PathFrom = current;
                        neighbor.SearchHeuristic =
                        neighbor.coordinates.DistanceTo(toCell.coordinates);
                        //frontier.Add(neighbor);
                        searchFrontier.Enqueue(neighbor);
                    }
                    else if (distance < neighbor.Distance)
                    {
                        int oldPriority = neighbor.SearchPriority;
                        neighbor.Distance = distance;
                        //neighbor.SetLabel(turn.ToString());
                        neighbor.PathFrom = current;
                        searchFrontier.Change(neighbor, oldPriority);
                    }
                   // frontier.Sort(
                   //     (x, y) => x.SearchPriority.CompareTo(y.SearchPriority)
                    //);
                }
            }
            return false;
        }

    }

}
    
     
