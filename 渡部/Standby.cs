using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Standby : MonoBehaviour{
    /// <summary>
    /// 準備完了などを判断するフェイズ
    /// </summary>

    [SerializeField]
    private bool[] playerReady = new bool[2];       //各プレイヤー1の状態
    private PhaseCheck phaseCheck;                  //現在のフェイズを取得するため

//***********************************************************************************************************************************

    void Start()
    {
        phaseCheck = GetComponent<PhaseCheck>();    //フェイズを確認するために最初にPhaseCheckを取得する
    }

    /// <summary>
    /// 各プレイヤーが準備完了だった場合、フェイズを進行させる。
    /// </summary>
    void Update () {
        if (phaseCheck.NowPhaseGet == PhaseCheck.Phase.StandbyPhase && playerReady[0] && playerReady[1]) phaseCheck.PhaseProgress();
    }


//***********************************************************************************************************************************


    /// <summary>
    /// 準備が完了したかの取得を行う
    /// </summary>
    /// <param name="PlayerCode">準備の完了したプレイヤー(0=1P,1=2P)</param>
    /// <param name="isReady">準備完了しているかどうか</param>
    [PunRPC]
    public void ReadyCheck(int PlayerCode,bool isReady)
    {
        if (phaseCheck.NowPhaseGet == PhaseCheck.Phase.StandbyPhase)
        {
            //どのプレイヤーが準備完了か否かを取得する。
            playerReady[PlayerCode] = playerReady[PlayerCode] != isReady ? isReady : !isReady;
        }
    }
}
