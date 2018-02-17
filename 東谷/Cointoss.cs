using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// コイントスを行うフェイズ。コイントスまでの時間はcoinWait+waitRandamRange[0]～waitRandamRange[1]です。
/// フライング撃ちをしてしまった場合はこのスクリプト内のBattleStart()を呼び出すこと。
/// </summary>
public class Cointoss : MonoBehaviour{
    
    [SerializeField]
    private float coinWait =5 ;                             //何秒間たったらコイントスを行うか
    [SerializeField]
    private int[] waitRandamRange = new int[2] { -1, 2 };   //coinWaitに対しここの数値を足した秒待機させる
    private float lastWaitTime;                             //最終的なコイントス待機時間
    [SerializeField]
    private float time;                                     //経過時間
    [SerializeField]
    private AudioSource SE;                          //心拍音のAudioSource
    [SerializeField]
    private bool heatBeatPlayed=false;                      //心拍音を再生させるか
    //private PhaseCheck phaseCheck;                          //現在のフェイズを取得
    [SerializeField]
    private AudioMixer audioMixer;                          //環境音を下げるため
    //private PhotonView photonView;

    private GameCommonData gameData;

//************************************************************************************************

    void Start()
    {
        //audioMixer = Resources.Load("Audio/AudioMixer") as AudioMixer;                  //音量の調節を行えるようにする為
        lastWaitTime = coinWait + Random.Range(waitRandamRange[0], waitRandamRange[1]); //コイントスするまでの乱数を決定する
        gameData = GetComponent<GameCommonData>();
    }

    void Update () {
        if (gameData.GetNowPhase == GameCommonData.Phase.CointossPhase)
        {

            if(gameData.player1P.IsBattleStart || gameData.player2P.IsBattleStart)
            {
                BattleStart();
                gameData.player1P.IsBattleStart = false;
                gameData.player2P.IsBattleStart = false;
                gameData.PassPlayerUpdata();
            }

            //コイントスフェイズに移行した場合
            if (!gameData.player1P.IsStanby || !gameData.player2P.IsStanby)      // コイントスフェーズ中に どちらかがスタンバイから外れたら
            {
                if (!gameData.player1P.IsStanby) gameData.player1P.BulletSpeed = gameData.foulBulletSpeed;
                else gameData.player2P.BulletSpeed = gameData.foulBulletSpeed;
                gameData.myData.IsBattleStart = true;
                gameData.PassPlayerUpdata();
            }
            time += Time.deltaTime;
            if (!heatBeatPlayed && time > lastWaitTime - 3) PlayHeartBeat();                              //開始二秒前になったら心拍音を鳴らす
            if (time > lastWaitTime)
            {
                SE.clip = Resources.Load("Audio/SE/coin") as AudioClip;
                SE.Play();
                gameData.myData.IsBattleStart = true;
                gameData.PassPlayerUpdata();
                time = 0;//ラウンドを進ませる
            }

        }
	}

    /// <summary>
    /// 心拍音を再生して環境音（BGS）を低下
    /// </summary>
    void PlayHeartBeat()
    {
        heatBeatPlayed = true;
        audioMixer.SetFloat("BGM", Mathf.Clamp((time * -20), -80, -20));
        //audioMixer.SetFloat("BGM", -80);
        SE.clip = Resources.Load("Audio/SE/heart1") as AudioClip;
        SE.Play();
    }


//************************************************************************************************

    //public void PassBattleStart()
    //{
    //    photonView.RPC("BattleStart", PhotonTargets.AllViaServer);
    //}
    ///// <summary>
    ///// 音量を調節し、フェイズを進行させる。
    ///// フライング撃ちを行った場合もここを呼び出すこと
    ///// </summary>
    //[PunRPC]
    public void BattleStart()
    {
        heatBeatPlayed = false;
        SE.Stop();                                                 //心拍音（のSE）を止める
        audioMixer.SetFloat("BGS", -20);                                                //環境音の音量を戻す
        audioMixer.SetFloat("BGM", -80);                                                //BGMを消す
        time = 0;
        lastWaitTime = coinWait + Random.Range(waitRandamRange[0], waitRandamRange[1]); //次のコイントスするまでの乱数を決定する
        gameData.PhaseProgress();       
        
    }
    
}
