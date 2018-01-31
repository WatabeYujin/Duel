using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryJadge : MonoBehaviour {

    int[] hp = new int[2];

    ////////////////////////////////////////////////////

    void Start()
    {
        for (int i = 0; i < 2; i++) hp[i] = 7;
    }

    ////////////////////////////////////////////////////
    
    /// <summary>
    /// 5ラウンド経過後の
    /// </summary>
    /// <returns>1P勝利=0,2P勝利=1,体力が同じ=2</returns>
    public int Jadge()
    {
        if (hp[0] == hp[1]) return 2;
        else if (hp[0] > hp[1]) return 0;
        else return 1;
    }

    /// <summary>
    /// ダメージを受ける処理
    /// </summary>
    /// <param name="TargetPlayer">回復させる</param>
    /// <param name="Damage"></param>
    public void Damage(int TargetPlayer,int Damage)
    {
        hp[TargetPlayer] -= Damage;
    }

    /// <summary>
    /// 現在の体力を取得する
    /// </summary>
    /// <param name="TargetPlayer">0=1P,1=2P</param>
    /// <returns></returns>
    public int GetHP(int TargetPlayer)
    {
        return hp[TargetPlayer];
    }

    public void GameSet()
    {

    }
}
