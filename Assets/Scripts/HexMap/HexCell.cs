using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
namespace maho {
    /*HexCell类是生成六边形网格的基本细胞
     * 定义了六边形颜色，地图UI，引擎坐标，距离等等*/
    public class HexCell : MonoBehaviour
    {
        public HexCoordinates coordinates;
        // public Color color;
        public int terrainTypeIndex;
        public RectTransform uiRect;
        int distance;
        public int SearchPhase { get; set; }
        public HexCell NextWithSamePriority { get; set; }
        
        [SerializeField] HexCell[] neighbors;
        //获取相邻格Neighbor
        public HexCell GetNeighbor(HexDirection direction)
        {
            return neighbors[(int)direction];
        }
        public void SetNeighbor(HexDirection direction, HexCell cell)
        {
            neighbors[(int)direction] = cell;
            cell.neighbors[(int)direction.Opposite()] = this;
        }
        public Color color
        {
            get
            {
                return HexMetrics.colors[terrainTypeIndex];
            }
        }
        public int TerrainTypeIndex
        {
            get
            {
                return terrainTypeIndex;
            }
            set
            {
                if (terrainTypeIndex != value)
                {
                    terrainTypeIndex = value;
                   //Refresh();
                }
            }
        }
      // void Refresh()
       // {
           // HexGrid hexGrid = new HexGrid();
          // hexGrid.Refresh();
       // }
        public Vector3 Position
        {
            get
            {
                return transform.localPosition;
            }
        }
        public Fleet FleetUnit { get; set; }
        public HexCell PathFrom { get; set; }
        public int Distance
        {
            get
            {
                return distance;
            }
            set
            {
                distance = value;
               // UpdateDistanceLabel();
            }
        }

        //void UpdateDistanceLabel()
       // {
          //  UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
           // label.text = ((distance == int.MaxValue) ? "" : distance.ToString());
       // }
        public void SetLabel(string text)
        {
            UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
            label.text = text;
        }
        public void Save(BinaryWriter writer)
        {
            writer.Write(terrainTypeIndex);
        }

        public void Load(MapGlobal MapGlobal,int i)
        {
            terrainTypeIndex = MapGlobal.cells[i];
        }
        //启发式寻路算法，把距离加上启发式距离确定搜索优先级
        public int SearchHeuristic { get; set; }

        public int SearchPriority
        {
            get
            {
                return distance + SearchHeuristic*4;
            }
        }
        //Highlight（）负责显示路径
        public void DisableHighlight()
        {
            Image highLight = uiRect.GetChild(0).GetComponent<Image>();
            highLight.enabled = false;
        }
        public void EnableHighlight(Color color)
        {
            Image highlight = uiRect.GetChild(0).GetComponent<Image>();
            highlight.color = color;
            highlight.enabled = true;
        }
    }
   
}