using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GPSTesting : MonoBehaviour
{
    public int level;
    public Text logtext;
    public Text outputtext;
    public InputField levelfield;

    public void OnvalueCHnagedLevel(string field)
    {
        level = int.Parse(levelfield.text);
    }
}
