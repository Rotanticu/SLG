using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using maho;
using UnityEngine.UI;
using System;

namespace maho
{ 

public class CreateNewMap : MonoBehaviour {

        public HexGrid grid;
        public InputField 宽;
        public InputField 高;
        public void Open()
        {
            gameObject.SetActive(true);
        }
        public void Close()
        {
            gameObject.SetActive(false);
        }
        public void CreateMap()
        {
            int width, height;
            width = Convert.ToInt32(宽.text);
            height = Convert.ToInt32(高.text);
            grid.width = width;
            grid.height = height;

            grid.CreateMap(width, height);
            Close();
        }
}
}
