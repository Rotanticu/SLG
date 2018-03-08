using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using maho;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace maho
{
   public class FightShip : MapShip
    
    {
        public Text text;
        public Image image;
        public Slider slider;
        public bool attacked = false;
        public bool dead = false;
         void FixedUpdate()
        {
            text = transform.GetChild(0).GetComponent<Text>();
            image = transform.GetChild(1).GetComponent<Image>();

            slider = transform.GetChild(2).GetComponent<Slider>();

           // text = transform.Find("Text").GetComponent<Text>();
            // image = transform.Find("Image").GetComponent<Image>();
             //slider = transform.Find("Slider").GetComponent<Slider>();
            slider.maxValue = MaxDefence;
            slider.value = defence;
            
        }
        public void RemoteAttack(FightShip ship)
        {
            ship.defence = ship.defence - attack;
            if (ship.defence < 0)
            {
                ship.defence = 0;
                ship.Die();
            }

            ship.slider.value = ship.defence ;
        }
        public void MediumRangeAttack(FightShip ship)
        {
            ship.defence = ship.defence - attack;
            if (ship.defence < 0)
            { 
                ship.defence = 0;
                ship.Die();

            }
            ship.slider.value = ship.defence;

        }
        public void ShortRangeAttack(FightShip ship)
        {
            ship.defence = ship.defence - attack;
            if (ship.defence < 0)
            { 
                ship.defence = 0;
                ship.Die();

            }
            ship.slider.value = ship.defence;

        }

         public IEnumerator  Fire(int attackTurn, FightShip beFiredShip)
        {
            if(!beFiredShip.dead)
            { 
            switch (attackTurn)
            {
                case (1):
                    RemoteAttack(beFiredShip);
                    break;
                case (2):
                    MediumRangeAttack(beFiredShip);
                    break;
                case (3):
                   ShortRangeAttack(beFiredShip);
                    break;
            }
            attacked = true;
            yield return new WaitForSeconds(1 / 2f);
            image.color = Color.white;
            }
        }
        public void Die()
        {
            text.text = "Dead";
            dead = true;
        }



        public int CompareTo(FightShip other)
        {
            //等于返回0
            int re = other.speed.CompareTo(speed);
            if (0 == re)
            {
                //id相同再比较Name
                return camp.CompareTo(other.camp);
            }
            return re;
        }

        /*
        public int Compare(FightShip x, FightShip y)
        {
            int re = y.speed.CompareTo(x.speed);
            if (0 == re)
            {
                return x.camp.CompareTo(y.camp);
            }
            return re;
        }
        */
    }
}

