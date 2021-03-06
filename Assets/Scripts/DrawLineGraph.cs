﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DrawLineGraph : MonoBehaviour
{
    GameTime gameTime;

    public float graphWidth;
    public float graphHeight;
    LineRenderer newLineRenderer;
    List<int> decibels;
    int vertexAmount = 50;
    float xInterval;

    GameObject parentCanvas;

    // Use this for initialization
    void Start()
    {
        gameTime = GameObject.Find("EventManager").GetComponent<GameTime>();

        parentCanvas = GameObject.Find("MainInformation");
        graphWidth = gameObject.GetComponent<RectTransform>().rect.width;
        graphHeight = gameObject.GetComponent<RectTransform>().rect.height;
        newLineRenderer = gameObject.GetComponent<LineRenderer>();
        newLineRenderer.positionCount = vertexAmount;

        xInterval = graphWidth / vertexAmount;
    }

    private void Update()
    {
        List<int> tmpTime = new List<int>();
        tmpTime.Add(gameTime.getTime());

        Draw(tmpTime);
    }

    //Display 1 minute of data or as much as there is.
    public void Draw(List<int> decibels)
    {
        if (decibels.Count == 0)
            return;

        float x = 0;

        for (int i = 0; i < vertexAmount && i < decibels.Count; i++)
        {
            int _index = decibels.Count - i - 1;

            float y = decibels[_index] * (graphHeight / 130); //(Divide grapheight with the maximum value of decibels.
            x = i * xInterval;

            newLineRenderer.SetPosition(i, new Vector3(x - graphWidth / 2, y - graphHeight / 2, 0));
        }
    }
}
