using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageHandler : MonoBehaviour
{
    private TextMeshProUGUI txtMessages;

    private bool msgShown = false;

    private float timer = 0.0f;

    private Color getMessageColour(string msgType)
    {
        switch (msgType) {
            case "good":
                return new Color(0.1490196f, 0.8509804f, 0.1490196f); //Green
            case "bad":
                return new Color(0.8509804f, 0.1490196f, 0.1490196f); //Red
            case "neutral":
                return Color.white;
            default:
                return Color.white;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        txtMessages = GameObject.Find("MainInformation/TopPanel/NewsSlider/txtNews").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        //after 3 seconds remove the message
        if (msgShown)
        {
            timer += Time.deltaTime;

            if (timer >= 5.0f)
            {
                txtMessages.text = "";
                msgShown = false;
                timer = 0.0f;
            }
        }
    }

    public void createMessage(string msgText, string msgType)
    {
        //show the message and set its text and color
        txtMessages.text = msgText;
        txtMessages.color = getMessageColour(msgType);

        msgShown = true;
    }
}