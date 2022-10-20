using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    public Sprite[] pictures;

    // Use this for initialization
    void OnEnable()
    {
        if (LevelManager.THIS != null)
        {
            var num = (int)((float)LevelManager.Instance.currentLevel / 20f - 0.01f);
            GetComponent<Image>().sprite = pictures[num >= 5 ? 0 : num];
            //GetComponent<Image>().sprite = pictures[(int)((float)LevelManager.Instance.currentLevel / 20f - 0.01f)];
        }
    }
}