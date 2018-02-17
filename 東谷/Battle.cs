using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battle : MonoBehaviour{
    /// <summary>
    /// 撃ち合いを行うフェイズ
    /// </summary>

    //[SerializeField]
    //private bool[] playerShot = new bool[2];    //各プレイヤーの発砲状況
    [SerializeField]
    private float time = 0;                     //経過時間
    [SerializeField]
    private AudioSource SE;          //コインが落ちた音を鳴らすAudioSource
    //private PhaseCheck phaseCheck;              //現在のフェイズを取得
    //private Result result;
    //private PhotonView photonView;
    private GameCommonData gameData;
    public Text battleImage;

    //**************************************************************************************************************

    //public bool PlayerShot
    //{
    //    get { return playerShot[PhotonNetwork.player.ID -1]; }
    //}

    void Start()
    {
        //phaseCheck = GetComponent<PhaseCheck>();    //フェイズを確認するために最初にPhaseCheckを取得する
        //result = GetComponent<Result>();
        //photonView = GetComponent<PhotonView>();
        gameData = GetComponent<GameCommonData>();

        //gameData.player1P.IsShot = false;
        //gameData.player2P.IsShot = false;
        //gameData.PassPlayerUpdata();
    }

	void Update () {
        if (gameData.GetNowPhase == GameCommonData.Phase.BattlePhase)
        {
            time += Time.deltaTime;

            if (time >= 0.5f) battleImage.enabled = false;
            else battleImage.enabled = true;

            if (time >= 5)
            {
                // 自分が撃ってなければ
                if (!gameData.myData.IsShot || !gameData.myData.IsEndHitCheck)
                {
                    HitCheck(GameCommonData.HitPosition.Null);
                    gameData.myData.IsShot = true;
                    gameData.PassPlayerUpdata();
                }
                // 両方のヒットチェックが終わっている場合
                if(gameData.player1P.IsEndHitCheck && gameData.player2P.IsEndHitCheck)
                {
                    SE.clip = Resources.Load("Audio/SE/coinDrop") as AudioClip;
                    SE.Play();
                    //Debug.Log(time);
                    BattleFinish();
                }
            }
        }
        //if(gameData.GetNowPhase == GameCommonData.Phase.StandbyPhase)
        //{
        //    time = 0;

        //    gameData.player1P.IsShot = false;
        //    gameData.player2P.IsShot = false;
        //    gameData.PassPlayerUpdata();
        //}
    }

    /// <summary>
    /// 戦闘フェイズ終了の処理
    /// </summary>
    public void BattleFinish()
    {
        time = 0;

        gameData.player1P.IsShot = false;
        gameData.player2P.IsShot = false;
        gameData.PassPlayerUpdata();

        gameData.PhaseProgress();
    }

    //**************************************************************************************************************


    //public void PassShotCheck(int playerCode)
    //{
    //    photonView.RPC("ShotCheck", PhotonTargets.AllViaServer, playerCode);
    //}
    /// <summary>
    /// 発砲を行ったかを取得する。
    /// </summary>
    /// <param name="PlayerCode">発砲したプレイヤー(1=1P,2=2P)</param>
    //[PunRPC]
    //public void ShotCheck(int playerCode)
    //{
    //    if (gameData.GetNowPhase == GameCommonData.Phase.BattlePhase)
    //    {
    //        playerShot[playerCode] = true;
    //        Debug.Log(playerShot[playerCode]);
    //    }
    //}

    public void HitCheck(GameCommonData.HitPosition _hitPos)
    {
        // hitcheck前の場合のみ
        if (!gameData.myData.IsEndHitCheck)
        {
            gameData.myData.HitPos = _hitPos;
            gameData.myData.HitTime = time;
            gameData.myData.IsEndHitCheck = true;
            gameData.PassPlayerUpdata();
        }

        //Debug.Log("<color=red>InHitCheck</color>" + phaseCheck.NowPhaseGet);
        //if (!playerReady[playerCode] && phaseCheck.NowPhaseGet == PhaseCheck.Phase.BattlePhase)
        //{
        //    Debug.Log("playerReady = false");
        //    hitPos[playerCode] = HitPos;
        //    hitTime[playerCode] = battle.GetTime;

        //    playerReady[playerCode] = true;

        //    if (HitPos == HitPosition.Null) hitTime[playerCode] = 5;
        //}
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
