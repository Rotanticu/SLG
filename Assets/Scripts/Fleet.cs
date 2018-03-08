using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using maho;
using System.IO;
using UnityEngine;

namespace maho
{
    public class Fleet : HexUnit
    {
        public int shipCount;
        public List<int> shipTypeIndex = new List<int>();

        public List<FightShip> fightShipList = new List<FightShip>();
        public List<MapShip> mapShipList = new List<MapShip>();
        //舰队的图鉴编号
        public int fleetTypeIndex;

        public int mapIndex;

        public string Name;
        //阵营，camp=0我军，camp=1敌军，camp=2中立。
        public int camp;

        public int attack;

        public int defence;

        public int step;

        /*
        public void FleetSave(BinaryWriter writer)
        {
            location.coordinates.Save(writer);
            writer.Write(location.FleetUnit.moved);
            writer.Write(location.FleetUnit.camp);
            writer.Write(location.FleetUnit.fleetTypeIndex);
            writer.Write(location.FleetUnit.mapIndex);


            for (int i = 0; i < mapShipList.Count; i++)
            {
                writer.Write(mapShipList[i].MaxDefence);
                writer.Write(mapShipList[i].defence);
                writer.Write(mapShipList[i].attack);
                writer.Write(mapShipList[i].speed);

            }
        }
        */
        public static void FleetShipsLoad(MapGlobal MapGlobal, HexGrid grid,int i)
        {
            if(MapGlobal.Fleets[i].MapShip.Length != 0)
            { 
            HexCoordinates coordinates = HexCoordinates.Load(MapGlobal,i);
            int camp = MapGlobal.Fleets[i].camp;
            int fleetTypeIndex = MapGlobal.Fleets[i].fleetTypeIndex;
            Fleet newFleet = Instantiate(grid.unitPrefabs[fleetTypeIndex]);
            newFleet.mapIndex = i;
            newFleet.moved = MapGlobal.Fleets[i].moved;
            newFleet.shipCount = MapGlobal.Fleets[i].shipCount;
            newFleet.CampLoad(MapGlobal, grid, newFleet, coordinates,camp, i);
            }

        }

        public void CampLoad(MapGlobal MapGlobal, HexGrid grid,Fleet newFleet, HexCoordinates coordinates,int camp,int i)
        {
            grid.AddFleetUnit(newFleet, grid.GetCell(coordinates), camp);
            for (int j = 0; j < MapGlobal.Fleets[i].MapShip.Length; j++)
            {
                MapShip newShip = Instantiate(grid.shipPrefabs[MapGlobal.Fleets[i].MapShip[j].shipTypeIndex]);
                newFleet.mapShipList.Add(newShip);
                newShip.transform.SetParent(newFleet.transform);
                newShip.MaxDefence = MapGlobal.Fleets[i].MapShip[j].maxDefence;
                newShip.defence = MapGlobal.Fleets[i].MapShip[j].defence;
                newShip.attack = MapGlobal.Fleets[i].MapShip[j].attack;
                newShip.speed = MapGlobal.Fleets[i].MapShip[j].speed;
                newShip.shipTypeIndex = MapGlobal.Fleets[i].MapShip[j].shipTypeIndex;
            }
        }



    }
}
