using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using maho;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace maho { 
    
public class FightMgr : MonoBehaviour {
        
        public FightShip[] shipPrefabs;
        public int attackerIndex, defenderIndex;
        public RectTransform weContent;
        public RectTransform enemyContent;
        public Text combatDistance;
        public Text 开战;
        public List<FightShip> fightFleet = new List<FightShip>();
        public List<FightShip> weFleet = new List<FightShip>();
        public List<FightShip> enemyFleet = new List<FightShip>();

        public FightShip selectShip, attackingShip;
        public int attackTurn = 1;
        private void Awake()
        {

            LoadFleet(MapGlobal.getInstance().attacker, MapGlobal.getInstance().defender,MapGlobal.getInstance());
           
        }
        private void Update()
        {
            {
                if(attackingShip)
                { 
                UIClear();
                UpDateSelectShip();
                if(attackingShip&&Input.GetMouseButtonDown(0))
                    {
                        if(!attackingShip.attacked&&GetShip().camp != attackingShip.camp)
                       StartCoroutine(attackingShip.Fire(attackTurn, GetShip()));
                        if (attackingShip.attacked)
                            ActionQueue();
                    }
                }
            }
        }

        void UIClear()
        {
            for(int i = 0;i<fightFleet.Count;i++)
            {
                Image image = fightFleet[i].transform.GetComponent<Image>();
                Color color = new Color(1, 1, 1, 0.3f);
                image.color = color;
            }
        }

        void UpDateSelectShip()
        {
            if(GetShip())
            { 
            FightShip selectShip =  GetShip();
            Image image = selectShip.transform.GetComponent<Image>();
            image.color = Color.red;
            }

        }
        FightShip GetShip()
        {
            

            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit))
            {
                FightShip selectShip = hit.transform.GetComponent<FightShip>();
                return selectShip;
            }
            else

            return null;
        }


        void LoadFleet(int attackerIndex,int defenderIndex,MapGlobal MapGlobal)
        {
            this.attackerIndex = attackerIndex;
            this.defenderIndex = defenderIndex;
            for (int i = 0; i < MapGlobal.Fleets.Length; i++)
            {
                if (MapGlobal.Fleets[i].mapIndex == attackerIndex)
                {
                    MapGlobal.Fleet attacker = MapGlobal.Fleets[i];
                    for (int j = 0; j < attacker.MapShip.Length;j++)
                    {
                        FightShip ship = Instantiate(shipPrefabs[attacker.MapShip[j].shipTypeIndex]);
                        ship.attack = attacker.MapShip[j].attack;
                        ship.defence = attacker.MapShip[j].defence;

                        ship.MaxDefence = attacker.MapShip[j].maxDefence;

                        ship.speed = attacker.MapShip[j].speed;

                        ship.camp = attacker.camp;

                        ship.shipTypeIndex = attacker.MapShip[j].shipTypeIndex;

                        //text.text = ship.hullNumber;
                        //image = ship.headPortrait;
                        if (ship.camp ==0||ship.camp == 2)
                        {
                            weFleet.Add(ship);
                        }
                        else if(ship.camp == 1)
                        {
                            enemyFleet.Add(ship);
                        }
                        fightFleet.Add(ship);
                        ship.transform.SetParent(weContent, false);
                    }
                }
                else if(MapGlobal.Fleets[i].mapIndex == defenderIndex)
                {
                    MapGlobal.Fleet defender = MapGlobal.Fleets[i];

                    for (int j = 0; j < defender.MapShip.Length; j++)
                    {
                        FightShip ship = Instantiate(shipPrefabs[defender.MapShip[j].shipTypeIndex]);
                        ship.attack = defender.MapShip[j].attack;
                        ship.defence = defender.MapShip[j].defence;

                        ship.MaxDefence = defender.MapShip[j].maxDefence;

                        ship.speed = defender.MapShip[j].speed;

                        ship.camp = defender.camp;

                        ship.shipTypeIndex = defender.MapShip[j].shipTypeIndex;


                        //text.text = ship.hullNumber;
                        //image = ship.headPortrait;
                        if (ship.camp == 0 && ship.camp == 2)
                        {
                            weFleet.Add(ship);
                        }
                        else if (ship.camp == 1)
                        {
                            enemyFleet.Add(ship);
                        }
                        fightFleet.Add(ship);
                        ship.transform.SetParent(enemyContent, false);
                    }
                }
            }
            fightFleet.Sort((x,y) => x.CompareTo(y));
        }
        public void Retreat()
        {
            UpDateFleets(MapGlobal.getInstance(), attackerIndex, weFleet);
            UpDateFleets(MapGlobal.getInstance(), attackerIndex, enemyFleet);

            UpDateFleets(MapGlobal.getInstance(), defenderIndex, weFleet);

            UpDateFleets(MapGlobal.getInstance(), defenderIndex, enemyFleet);

            SceneManager.LoadScene("Start 1");
        }

        public void ActionQueue()
        {
            for (int i = 0; i < fightFleet.Count; i++)
            {
                if (!fightFleet[i].attacked)
                {
                    attackingShip = fightFleet[i];

                    if (attackingShip.camp == 0)
                    {
                        attackingShip.image.color = Color.green;
                        return;
                    }
                    if (attackingShip.camp == 1)
                    {
                        attackingShip.image.color = Color.green;
                        enemyAI();
                    }
                }
            }
        }

        List<FightShip> ClearDead(List<FightShip> fleet)
        {
            for (int i = 0; i < fleet.Count; i++)
            {
                if (fleet[i].dead)
                {
                    fleet.Remove(fleet[i]);
                }
            }

            if (fleet.Count == 0)
            {
                return null;
            }
            else
            {
                return fleet;
            }
        }

        void UpDateFleets(MapGlobal MapGlobal,int Index,List<FightShip> fleet)
        {
            bool b = false;
            b = (ClearDead(fleet) != null);
            if (b)
            {
                List<FightShip> clearFleet = ClearDead(fleet);
            MapGlobal.MapShip[] tempFleet = new MapGlobal.MapShip[clearFleet.Count];


            for (int i = 0; i < clearFleet.Count; i++)
            {
                tempFleet[i].attack = clearFleet[i].attack;
                tempFleet[i].defence = clearFleet[i].defence;
                tempFleet[i].maxDefence = clearFleet[i].MaxDefence;
                tempFleet[i].speed = clearFleet[i].speed;
                tempFleet[i].shipTypeIndex = clearFleet[i].shipTypeIndex;
            }
                for (int i = 0; i < MapGlobal.Fleets.Length; i++)
            {
                if (MapGlobal.Fleets[i].mapIndex == Index && MapGlobal.Fleets[i].camp == fleet[0].camp)
                {
                    MapGlobal.Fleets[i].MapShip = tempFleet;
                }
            }
            }
            else
            {
                MapGlobal.Fleet[] tempFleets = new MapGlobal.Fleet[MapGlobal.Fleets.Length - 1];
                for (int i = 0, j = 0; i < MapGlobal.Fleets.Length; i++)
                {
                    if (MapGlobal.Fleets[i].mapIndex != Index)
                    {
                        tempFleets[j] = MapGlobal.Fleets[i];
                        j++;
                    }
                }
                MapGlobal.Fleets = tempFleets;
                MapGlobal.unitCount = MapGlobal.Fleets.Length;
            }
        }

        void NewFleet(int index,int camp,MapGlobal MapGlobal)
        {

            for (int i = 0; i < MapGlobal.Fleets.Length; i++)
            {

                if (MapGlobal.Fleets[i].mapIndex == index)
                {
                    if (MapGlobal.Fleets[i].camp == 0)
                    {
                        MapGlobal.MapShip[] tempMapShip = new MapGlobal.MapShip[weFleet.Count];
                        for (int j = 0; j < tempMapShip.Length; j++)
                        {
                            tempMapShip[j].attack = weFleet[j].attack;
                            tempMapShip[j].defence = weFleet[j].defence;
                            tempMapShip[j].maxDefence = weFleet[j].MaxDefence;
                            tempMapShip[j].speed = weFleet[j].speed;
                            tempMapShip[j].shipTypeIndex = weFleet[j].shipTypeIndex;
                        }
                        MapGlobal.Fleets[i].MapShip = tempMapShip;
                    }
                }
            }

            }
        public  void  RemoteAttackTurn()
        {
            switch (attackTurn)
            {
                case (1):
                    combatDistance.text = "远程战";
                    break;
                case (2):
                    combatDistance.text = "中程战";
                    break;
                case (3):
                    combatDistance.text = "近程战";
                    break;
                case (4):
                    return;

            }

            //            WaitForSeconds delay = new WaitForSeconds(1f);
            for (int i= 0; i < fightFleet.Count;i++)
            {
 //               yield return delay;
                if (!fightFleet[i].attacked)
                { 
                if (fightFleet[i].camp == 0)
                {
                        attackingShip = fightFleet[i];
                        attackingShip.image.color = Color.green;
                        return;
                    //fightFleet[i].RemoteAttack(enemyFleet[Random.Range(0, enemyFleet.Count)]);                  
                }
                if (fightFleet[i].camp == 1)
                {
                        attackingShip = fightFleet[i];
                        attackingShip.image.color = Color.green;
                        enemyAI();
                        //Fire(attackingShip, weFleet[Random.Range(0, weFleet.Count)]);
                    }
                }
            }
            attackingShip = null;
            开战.text = "追击";
            for (int i = 0; i < fightFleet.Count; i++)
            {
                fightFleet[i].attacked = false;
            }

            switch (attackTurn)
            {
                case (1):
                    combatDistance.text = "远程战结束！";
                    attackTurn++;
                    break;
                case (2):
                    combatDistance.text = "中程战结束";
                    attackTurn++;

                    break;
                case (3):
                    combatDistance.text = "近程战结束";
                    attackTurn++;
                    break;
                case (4):
                    break;
            }


        }

        void enemyAI( )
        {
            int r = Random.Range(0, weFleet.Count);
            StartCoroutine(attackingShip.Fire(attackTurn, weFleet[r]));
        }

    }
}
