using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Practice : MonoBehaviour {
    /// <summary>
    /// 練習モード時のスクリプト
    /// 現状、命中箇所と各キャラクターの準備完了状態の取得のみ
    /// </summary>

    public enum HitPosition                         //命中箇所のenum
    {
        Null,
        Regs,
        Arms_Body,
        Hed
    }
    private HitPosition hitPosition;                //命中箇所を取得する
    [SerializeField]
    private bool[] playerReady = new bool[2];       //各プレイヤー1の状態

    /////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// ダメージを与えた箇所を取得・あるいは出力する
    /// </summary>
    public HitPosition PracticeDamage
    {
        set {
            hitPosition = value;
        }
        get
        {
            return hitPosition;
        }
    }

    /// <summary>
    /// 準備が完了したかの取得を行う
    /// </summary>
    /// <param name="PlayerCode">準備の完了したプレイヤー(0=1P,1=2P)</param>
    /// <param name="isReady">準備完了しているかどうか</param>
    public void ReadyChack(int PlayerCode, bool isReady)
    {
        //どのプレイヤーが準備完了か否かを取得する。
        playerReady[PlayerCode] = playerReady[PlayerCode] != isReady ? isReady : !isReady;
    }
}
