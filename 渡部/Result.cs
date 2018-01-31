using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Result : MonoBehaviour
{
    /// <summary>
    /// 撃ち合いの結果を処理するフェイズ
    /// </summary>
    
    private bool[] playerReady = new bool[2];           //各プレイヤーの状態
    private int[] life = new int[2] { 7,7};             //各プレイヤーのHP
    [HideInInspector]
    public enum HitPosition                             //命中箇所のenum
    {
        Null,
        Legs,
        Arms_Body,
        Head
    }
    private HitPosition[] hitPos = new HitPosition[2];  //命中させた箇所

    private float[] hitTime = new float[2];             //着弾までの時間

    [HideInInspector]
    public enum VictoryPlayer                           //勝者のenum
    {
        None,
        Player1,
        Player2
    }
    private VictoryPlayer victoryPlayer = 0;            //勝利プレイヤー

    private AudioMixer audioMixer;                      //音量調節に使用
    private PhaseCheck phaseCheck;                      //フェイズ状況の取得
    private ResultView resultView;                      //結果表示用

    //**********************************************************************************************************

    void Start()
    {
        audioMixer = Resources.Load("Audio/AudioMixer") as AudioMixer;  //音量の調節を可能にするため
        phaseCheck = GetComponent<PhaseCheck>();                        //フェイズを確認するために最初にPhaseCheckを取得する
        resultView = GetComponent<ResultView>();                        //結果表示をするために最初にResultViewを取得する
    }

    void Update () {
        if (phaseCheck.NowPhaseGet == PhaseCheck.Phase.ResultPhase &&
            playerReady[0] && playerReady[1])
        {
            for(int i = 0 ; i < 2 ; i ++ ) playerReady[i] = false;
            //[どのプレイヤーが,0命中箇所,1命中時の時間]
            if (hitTime[0] == hitTime[1] ||
               (hitTime[0] == -1 && hitTime[1] == -1)) victoryPlayer = VictoryPlayer.None;  //命中時間が同じ・両方撃っていない・両方外れた場合引き分け（勝者無し）
            else if (hitTime[0] < hitTime[1] ||
                     hitTime[1] == -1) victoryPlayer = VictoryPlayer.Player1;               //1Pのほうが命中が早い・2Pが外れた場合1Pの勝ち
            else victoryPlayer = VictoryPlayer.Player2;                                     //その他の場合2Pの勝ち
            Invoke("ResetViewSet", 3);                                                      //3秒後にリザルト表示
        }
    }

    /// <summary>
    /// リザルトを表示させフェイズを進める
    /// </summary>
    private void ResetViewSet()
    {
        int _AfterLife = life[(int)victoryPlayer - 1];
        if (victoryPlayer != VictoryPlayer.None)
        {
            switch (hitPos[(int)victoryPlayer - 1])
            {
                case HitPosition.Head:
                    life[(int)victoryPlayer - 1] = 0;
                    _AfterLife = 0;
                    break;
                case HitPosition.Arms_Body:
                    life[(int)victoryPlayer - 1] -= 3;
                    _AfterLife = life[(int)victoryPlayer - 1] - 3;
                    break;
                case HitPosition.Legs:
                    life[(int)victoryPlayer - 1] -= 1;
                    _AfterLife = life[(int)victoryPlayer - 1] - 1;
                    break;
            }
        }
        Debug.Log("ここでプレイヤーIDを入力する。デバッグのため0（プレイヤー1）");
        int PlayerID = 0;
        resultView.ViewOpen(hitPos[PlayerID], hitTime[PlayerID],victoryPlayer, _AfterLife);
        Debug.Log("ここでリザルトを表示し、ダメ―ジの処理を与える。");
        audioMixer.SetFloat("BGM", -20);                                    //BGMを元に戻す
        phaseCheck.PhaseProgress();
    }

//**********************************************************************************************************

    /// <summary>
    /// 命中した情報を入力する。
    /// </summary>
    /// <param name="PlayerCode">命中させたプレイヤー(0=1P,1=2P)</param>
    /// <param name="HitPos">命中させた箇所</param>
    /// <param name="HitTime">命中時の時間(外れたり撃ってない場合は-1)</param>
    [PunRPC]
    public void HitCheck(int PlayerCode,HitPosition HitPos, float HitTime)
    {
        hitPos[PlayerCode] = HitPos;
        hitTime[PlayerCode] = HitTime;
        playerReady[PlayerCode] = true;
    }
}
