using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoundCount : MonoBehaviour {
    /// <summary>
    /// 現在のラウンド数を管理するスクリプト
    /// </summary>

    [SerializeField]
    private int roundCount; //現在のラウンド数
    VictoryJadge victoryJadge;
    
    /////////////////////////////////////////////////////////////////////////

    void Start()
    {
        victoryJadge = GetComponent<VictoryJadge>();

    }

/////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// ラウンド数を取得する
    /// </summary>
    public int RoundCountChack
    {
        get{ return roundCount; }
    }

    public int NextRound()
    {
        roundCount++;
        if (roundCount <= 6)
        {
            if (victoryJadge.Jadge() != 2) victoryJadge.GameSet();
        }
        return roundCount;
    }
    
}
