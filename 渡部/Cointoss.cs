using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Cointoss : MonoBehaviour{
    /// <summary>
    /// コイントスを行うフェイズ。コイントスまでの時間はcoinWait+waitRandamRange[0]～waitRandamRange[1]です。
    /// フライング撃ちをしてしまった場合はこのスクリプト内のBattleStart()を呼び出すこと。
    /// </summary>

    [SerializeField]
    private float coinWait =5 ;                             //何秒間たったらコイントスを行うか
    [SerializeField]
    private int[] waitRandamRange = new int[2] { -1, 2 };   //coinWaitに対しここの数値を足した秒待機させる
    private float lastWaitTime;                             //最終的なコイントス待機時間
    [SerializeField]
    private float time;                                     //経過時間
    [SerializeField]
    private AudioSource heartBeat;                          //心拍音のAudioSource
    [SerializeField]
    private AudioSource cointossSound;                      //コイントスのAudioSource
    private bool heatBeatPlayed=false;                      //心拍音を再生させるか
    private PhaseCheck phaseCheck;                          //現在のフェイズを取得
    private AudioMixer audioMixer;                          //環境音を下げるため

//************************************************************************************************

    void Start()
    {
        audioMixer = Resources.Load("Audio/AudioMixer") as AudioMixer;                  //音量の調節を行えるようにする為
        phaseCheck = GetComponent<PhaseCheck>();                                        //フェイズを確認するために最初にPhaseCheckを取得する
        lastWaitTime = coinWait + Random.Range(waitRandamRange[0], waitRandamRange[1]); //コイントスするまでの乱数を決定する
    }

    void Update () {
        if (phaseCheck.NowPhaseGet == PhaseCheck.Phase.CointossPhase)
        {
            //コイントスフェイズに移行した場合
            time += Time.deltaTime;
            if (!heatBeatPlayed && time > lastWaitTime - 3) PlayHeartBeat();                              //開始二秒前になったら心拍音を鳴らす
            if (time > lastWaitTime)
            {
                if(cointossSound) cointossSound.Play();
                BattleStart();                                                                          //ラウンドを進ませる
            }
        }
	}

    /// <summary>
    /// 心拍音を再生して環境音（BGS）を低下
    /// </summary>
    void PlayHeartBeat()
    {
        heatBeatPlayed = true;
        audioMixer.SetFloat("BGS", Mathf.Clamp((lastWaitTime * 45 - 135) - (time * 45), -80, -20));
        if(heartBeat) heartBeat.Play();
    }


//************************************************************************************************

    /// <summary>
    /// 音量を調節し、フェイズを進行させる。
    /// フライング撃ちを行った場合もここを呼び出すこと
    /// </summary>
    public void BattleStart()
    {
        if(heartBeat) heartBeat.Stop();                                                 //心拍音（のSE）を止める
        audioMixer.SetFloat("BGS", -20);                                                //環境音の音量を戻す
        audioMixer.SetFloat("BGM", -80);                                                //BGMを消す
        time = 0;
        lastWaitTime = coinWait + Random.Range(waitRandamRange[0], waitRandamRange[1]); //次のコイントスするまでの乱数を決定する
        phaseCheck.PhaseProgress();       
    }
    
}
