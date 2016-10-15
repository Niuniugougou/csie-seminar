﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OpenCVForUnity;
using UnityEngine.UI;
using System;

public class TeachWindowEvent : MonoBehaviour
{
    public GameLevelIndex _levelIndex;

    public LevelBeenTeachFlag[] _levelBeenTeachFlag;

    void Start()
    {

    }

    void Update()
    {

    }

    public bool IsBeenTeach(int index)
    {
        return _levelBeenTeachFlag[index]._isBeenTeach;
    }

    public void EnterTeachModeEvent()
    {
        this.GetComponent<ObjectMoveInEffect>().SmoothMoveInButtonEffect();
    }

    public void ExitTeachModeEvent()
    {
        this.GetComponent<ObjectMoveOutEffect>().SmoothMoveOutButtonEffect();
    }
}


