using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGlobal : MonoBehaviour
{


    private static MapGlobal MapDate = null;
    public static GameObject obj;

    public string path;
    public int mapHeader;
    public int width, height;
    public int[] cells;
    public int unitCount;
    public Fleet[] Fleets;
    public int attacker, defender;

    [System.Serializable]
    public struct Fleet
    {
        public int CoordinatesX;
        public int CoordinatesZ;
        public bool moved;
        public int camp;
        public int fleetTypeIndex;
        public int mapIndex;

        public int shipCount;
        public MapShip[] MapShip;    

    }

    [System.Serializable]

    public struct MapShip
    {
        public int shipTypeIndex;

        public int attack;
        public int defence;
        public int maxDefence;
        public int speed;
    }


    public static MapGlobal getInstance()
    {
        if (MapDate == null)
        {

            obj = new GameObject("MapDate");//创建一个带名字的对象  
            MapDate = obj.AddComponent(typeof(MapGlobal)) as MapGlobal;
            //LoadFight(attackingFleet, attackedFleet);
            DontDestroyOnLoad(obj);
        }
        return MapDate;
    }
    public  void Clear(MapGlobal MapGlobal)
    {
        MapGlobal.path = null;
        MapGlobal.width = 0;
        MapGlobal.height = 0;
        FightClear();
        MapGlobal.cells = new int[0];
        MapGlobal.unitCount = 0;
        MapGlobal.Fleets = new Fleet[0];
    }

    public void FightClear()
    {
        attacker = int.MaxValue;
        defender = int.MaxValue;

    }

}

