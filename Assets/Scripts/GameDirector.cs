﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [SerializeField] private List<GameObject> _enemies = new List<GameObject> ();

    public List<GameObject> Enemies
    {
        get { return _enemies; }
    }

    private void OnEnable()
    {
        EnemyController.OnEnemyDeath += RemoveEnemyFromChain;
    }

    private void Start()
    {
        DisplayEnemies();       
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            // If correct Enemy dies, remove it from chain
            // Else resurrect Enemy
            //RemoveEnemyFromChain(0);
            DisplayEnemies();
        }
    }

    private void RemoveEnemyFromChain(int position)
    {
        _enemies.RemoveAt(position);
    }

    private void DisplayEnemies()
    {
        // Display chain of Enemies in the console
        foreach(GameObject obj in _enemies)
            Debug.Log(obj.name);

        Debug.LogWarning("**************");
    }

    private void OnDisable()
    {
        EnemyController.OnEnemyDeath -= RemoveEnemyFromChain;
    }
}