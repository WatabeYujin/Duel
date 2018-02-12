using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battle : MonoBehaviour{
    /// <summary>
    /// 撃ち合いを行うフェイズ
    /// </summary>

    [SerializeField]
    private bool[] playerShot = new bool[2];    //各プレイヤーの発砲状況
    [SerializeField]
    private float time = 0;                     //経過時間
    [SerializeField]
    private AudioSource coinFallSound;          //コインが落ちた音を鳴らすAudioSource
    private PhaseCheck phaseCheck;              //現在のフェイズを取得
    private Result result;
    private PhotonView photonView;


    //**************************************************************************************************************

    public bool PlayerShot
    {
        get { return playerShot[PhotonNetwork.player.ID -1]; }
    }

    void Start()
    {
        phaseCheck = GetComponent<PhaseCheck>();    //フェイズを確認するために最初にPhaseCheckを取得する
        result = GetComponent<Result>();
        photonView = GetComponent<PhotonView>();

        for (int i = 0; i < playerShot.Length; i++) playerShot[i] = false;
    }

	void Update () {
        if (phaseCheck.NowPhaseGet == PhaseCheck.Phase.BattlePhase)
        {
            time += Time.deltaTime;
            if (time >= 5)
            {
                if(coinFallSound) coinFallSound.Play();
                BattleFinish();
            }
        }
        if(phaseCheck.NowPhaseGet == PhaseCheck.Phase.StandbyPhase)
        {
            time = 0;
        }
    }

    /// <summary>
    /// 戦闘フェイズ終了の処理
    /// </summary>
    public void BattleFinish()
    {
        Debug.Log("<color=blue>BattleFin</color>" + phaseCheck.NowPhaseGet);
        if(phaseCheck.NowPhaseGet == PhaseCheck.Phase.BattlePhase)
        {
            result.PassHitCheck(PhotonNetwork.player.ID - 1, Result.HitPosition.Null);
            for (int i = 0; i < 2; i++) playerShot[i] = false;
            
        } 
    }

    //**************************************************************************************************************


    public void PassShotCheck(int playerCode)
    {
        photonView.RPC("ShotCheck", PhotonTargets.AllViaServer, playerCode);
    }
    /// <summary>
    /// 発砲を行ったかを取得する。
    /// </summary>
    /// <param name="PlayerCode">発砲したプレイヤー(1=1P,2=2P)</param>
    [PunRPC]
    public void ShotCheck(int playerCode)
    {
        if (phaseCheck.NowPhaseGet == PhaseCheck.Phase.BattlePhase)
        {
            playerShot[playerCode] = true;
            Debug.Log(playerShot[playerCode]);
        }
        
    }

    /// <summary>
    /// 経過時間を取得する
    /// </summary>
    public float GetTime
    {
        get
        {
            return time;
        }
    }
}
