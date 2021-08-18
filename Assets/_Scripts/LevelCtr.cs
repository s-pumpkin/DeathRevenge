using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelCtr : MonoBehaviour
{
    private static LevelCtr _instance;
    public static LevelCtr instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<LevelCtr>();
            return _instance;
        }
    }

    [System.Serializable]
    public class Levels
    {
        public GameObject[] mouster;
        public GameObject Door;
        public bool openDoor;
    }

    public Levels[] levels;
    private int CurrLevel = 0;

    private void Awake()
    {
        _instance = this;
    }

    public void RemoveMonster(GameObject go)
    {
        Levels _levels = levels[CurrLevel];

        List<GameObject> monstersList = new List<GameObject>(_levels.mouster);
        monstersList.Remove(go);
        _levels.mouster = monstersList.ToArray();
        if (_levels.mouster.Length == 0)
        {
            nextLevel();

            _levels.Door.GetComponent<Collider>().enabled = false;
            ParticleSystem.MainModule main = _levels.Door.GetComponent<ParticleSystem>().main;
            main.loop = false;
            Destroy(_levels.Door, 5f);
        }

    }

    public void nextLevel()
    {
        CurrLevel += 1;
        if (CurrLevel >= levels.Length)
            return;
        Levels _levels = levels[CurrLevel];
        foreach (GameObject go in _levels.mouster)
            go.SetActive(true);
    }
}
