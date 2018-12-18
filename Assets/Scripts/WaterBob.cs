using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBob : MonoBehaviour
{
    private float timer = 0.0f;

    Vector3 initPos;

    private float maxMove = 0.3f;
    private float minMove = -0.3f;

    private float curMove = 0.0f;

    private bool moveUp;

    // Start is called before the first frame update
    void Start()
    {
        initPos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveUp)
            curMove += 0.2f * Time.deltaTime;
        else
            curMove -= 0.2f * Time.deltaTime;

        gameObject.transform.position = new Vector3(initPos.x, initPos.y + curMove, initPos.z);

        if (curMove >= maxMove || curMove <= minMove)
            moveUp = !moveUp;
    }
}
