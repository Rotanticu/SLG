using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using maho;
using UnityEngine.UI;

public class SelectionUnit : MonoBehaviour {

    public FastUnitSelection FastSclectList;
    public Text text;
    public Image image;
    public int index;
    // Use this for initialization
    public void Select ()
    {
        FastSclectList.Move(index);

    }

    // Update is called once per frame
    void Update () {
		
	}

    public void Initialization ()
    {
        text = transform.Find("Text").GetComponent<Text>();
        image = transform.Find("Image").GetComponent<Image>();
    }
}

