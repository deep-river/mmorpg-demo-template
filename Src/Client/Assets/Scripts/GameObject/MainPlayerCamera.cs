﻿using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public Camera camera;
    public Transform viewPoint;

    public GameObject player;
	
	// Update is called once per frame
	void Update () {
		
	}

    private void LateUpdate()
    {
        if (player == null)
            player = User.Instance.CurrentCharacterObject;

        this.transform.position = player.transform.position;
        this.transform.rotation = player.transform.rotation;
    }
}
