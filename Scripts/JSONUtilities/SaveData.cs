using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//Responsible for saving player inventory/health/etc.

//From tutorial: https://www.youtube.com/watch?v=Wu4SGitck7M
//"JSONUtility - A Better Way to Save Persistent Data in Unity" by Game Dev By Kaupenjoe
public class SaveData
{
    private int currentHP;
    private int maxHP;

    private List<Weapon> weapons;

    private static SaveData _instance = null;

    public static SaveData Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new SaveData();
            }

            return _instance;
        }
    }

    // We set the current HP and max HP to 100
    private SaveData()
    {
        this.currentHP = 100;
        this.maxHP = 100;
        this.weapons = new List<Weapon>();
    }
}