using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{


    public GameObject gameOverPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayerDied()
    {
    

       
        ShowGameOverUI();

        // หยุดเวลา
        Time.timeScale = 0f;

    }

    // เรียกใช้เมื่อต้องการเปิด UI Game Over
    public void ShowGameOverUI()
    {
        // เปิด UI Game Over
        gameOverPanel.SetActive(true);
    }
}
