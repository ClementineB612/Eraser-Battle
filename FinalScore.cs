using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalScore : MonoBehaviour
{
   public TMPro.TextMeshProUGUI final;
    void Start()
    {
        final.text = "Game Over! Final Score: " + PlayerPrefs.GetInt("FinalScore");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
