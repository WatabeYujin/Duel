using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*このフェーズから次のフェーズに移る条件*/
//WaitGunでコライダーに両プレイヤーの銃が入ったらコイントスへ

/// <summary>
/// 準備完了などを判断するフェイズ
/// </summary>
public class Standby : MonoBehaviour{
    
    //[SerializeField]
    //private bool[] playerReady = new bool[2];       //各プレイヤー1の状態
    
    //private PhaseCheck phaseCheck;                  //現在のフェイズを取得するため
    //private PhotonView photonView;
    private GameCommonData gameData;
    private float stanbyTimer;          // スタンバイフェイズに入ってからの経過時間
    private bool wait = false;          // 経過待ちかどうか

    //***********************************************************************************************************************************

    void Start()
    {
        gameData = GetComponent<GameCommonData>();

    }

    /// <summary>
    /// 各プレイヤーが準備完了だった場合、フェイズを進行させる。
    /// </summary>
    void Update () {
        //if (phaseCheck.NowPhaseGet == PhaseCheck.Phase.StandbyPhase && playerReady[0] && playerReady[1] && wait)
        if (gameData.GetNowPhase == GameCommonData.Phase.StandbyPhase && gameData.player1P.IsStanby && gameData.player2P.IsStanby)
        {
            wait = false;
            stanbyTimer = 0;
            gameData.PhaseProgress();
        }
        if(gameData.GetNowPhase == GameCommonData.Phase.StandbyPhase)
        {
            stanbyTimer += Time.deltaTime;
            if(stanbyTimer >= 3.0f) wait = true;
        }
    }

    

    //***********************************************************************************************************************************

    //public void PassReadyCheck(int playerCode,bool isReady)
    //{
    //    //if (phaseCheck.NowPhaseGet == PhaseCheck.Phase.StandbyPhase && !wait) StartCoroutine(WaitStandby());
    //    gameData.photonView.RPC("ReadyCheck", PhotonTargets.AllViaServer, playerCode, isReady);
    //}
    ///// <summary>
    ///// 準備が完了したかの取得を行う
    ///// </summary>
    ///// <param name="PlayerCode">準備の完了したプレイヤー(0=1P,1=2P)</param>
    ///// <param name="isReady">準備完了しているかどうか</param>
    //[PunRPC]
    //public void ReadyCheck(int playerCode, bool isReady)
    //{
    //    //どのプレイヤーが準備完了か否かを取得する。
    //    playerReady[playerCode] = playerReady[playerCode] != isReady ? isReady : !isReady;
    //    Debug.Log("<color=green>" + "Player" + (playerCode + 1) + ":" + playerReady[playerCode] + "</color>");
    //}
}
