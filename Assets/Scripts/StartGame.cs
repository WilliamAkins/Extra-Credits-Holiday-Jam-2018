using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    private Button btnStart;
    private ManageMainInfo manageMainInfo;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;

        manageMainInfo = GameObject.Find("EventManager").GetComponent<ManageMainInfo>();
        manageMainInfo.setGamePaused(true);

        btnStart = gameObject.transform.Find("Panel/btnStart").GetComponent<Button>();
        btnStart.onClick.AddListener(delegate { btnBuyPressed(); });
    }

    // Update is called once per frame
    void Update() {}

    private void btnBuyPressed()
    {
        Debug.Log("Removing help");

        if (btnStart != null)
        {
            btnStart.onClick.RemoveAllListeners();
            btnStart = null;
        }

        Time.timeScale = 1;
        manageMainInfo.setGamePaused(false);

        Destroy(gameObject);
    }
}