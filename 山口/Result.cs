using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    ScoreBoad scoreBoad;

    /// <summary>
    /// 撃ち合いの結果を処理するフェイズ
    /// </summary>
    private PhotonView photonView;
    private Battle battle;
    private bool[] playerReady = new bool[2];           //各プレイヤーの状態
    private int[] life = new int[2] {7,7};             //各プレイヤーのHP
    [HideInInspector]
    public enum HitPosition                             //命中箇所のenum
    {
        Null,
        Legs,
        Arms_Body,
        Head
    }
    private HitPosition[] hitPos = new HitPosition[2];  //命中させた箇所

    private float[] hitTime = new float[2] {6,6};             //着弾までの時間

    [HideInInspector]
    public enum VictoryPlayer                           //勝者のenum
    {
        None = -1,
        Player1,
        Player2
    }
    private VictoryPlayer victoryPlayer = VictoryPlayer.None;            //勝利プレイヤー

    private AudioMixer audioMixer;                      //音量調節に使用
    private PhaseCheck phaseCheck;                      //フェイズ状況の取得
    //private ResultView resultView;                      //結果表示用

    //**********************************************************************************************************

    void Start()
    {
        audioMixer = Resources.Load("Audio/AudioMixer") as AudioMixer;  //音量の調節を可能にするため
        phaseCheck = GetComponent<PhaseCheck>();                        //フェイズを確認するために最初にPhaseCheckを取得する
        //resultView = GetComponent<ResultView>();                        //結果表示をするために最初にResultViewを取得する
        photonView = GetComponent<PhotonView>();
        battle = GetComponent<Battle>();

        scoreBoad = GetComponent<ScoreBoad>();

        for (int i = 0; i < 2; i++) life[i] = 7;
        for (int i = 0; i < 2; i++) playerReady[i] = false;
    }

    void Update () {

        //Debug.Log(playerReady[0] + " " + playerReady[1]);

        if (phaseCheck.NowPhaseGet == PhaseCheck.Phase.BattlePhase &&
            playerReady[0] && playerReady[1])
        {
            battle.BattleFinish();
            phaseCheck.PhaseProgress();
            for (int i = 0 ; i < 2 ; i ++ ) playerReady[i] = false;

            if(hitPos[0] == HitPosition.Null && hitPos[1] == HitPosition.Null) victoryPlayer = VictoryPlayer.None;
            else
            {
                if(hitTime[0] < hitTime[1]) // 1PWin
                {
                    victoryPlayer = VictoryPlayer.Player1;
                }
                else
                {
                    victoryPlayer = VictoryPlayer.Player2;
                }
            }
            Invoke("ResetViewSet", 3);

            ////[どのプレイヤーが,0命中箇所,1命中時の時間]
            //if ((int)hitTime[0] == -1 && (int)hitTime[1] == -1) victoryPlayer = VictoryPlayer.None;  //命中時間が同じ・両方撃っていない・両方外れた場合引き分け（勝者無し）
            //else if (hitTime[0] < hitTime[1] ||
            //         (int)hitTime[1] == -1) victoryPlayer = VictoryPlayer.Player1;               //1Pのほうが命中が早い・2Pが外れた場合1Pの勝ち
            //else victoryPlayer = VictoryPlayer.Player2;                                     //その他の場合2Pの勝ち
            //Invoke("ResetViewSet", 3);                                                      //3秒後にリザルト表示
        }
    }

    /// <summary>
    /// リザルトを表示させフェイズを進める
    /// </summary>
    private void ResetViewSet()
    {
        int _AfterLife = 0;
        Debug.Log(victoryPlayer);
        if (victoryPlayer != VictoryPlayer.None)
        {
            _AfterLife = life[(int)victoryPlayer];
            switch (hitPos[(int)victoryPlayer])
            {
                case HitPosition.Head:
                    life[(int)victoryPlayer] = 0;
                    _AfterLife = 0;
                    break;
                case HitPosition.Arms_Body:
                    life[(int)victoryPlayer] -= 3;
                    _AfterLife = life[(int)victoryPlayer];
                    break;
                case HitPosition.Legs:
                    life[(int)victoryPlayer] -= 1;
                    _AfterLife = life[(int)victoryPlayer];
                    break;
                case HitPosition.Null:
                    break;

            }
            //resultView.PassViewOpen(victoryPlayer, hitPos[(int)victoryPlayer], hitTime[(int)victoryPlayer], _AfterLife);
            scoreBoad.PassViewOpen(victoryPlayer, hitPos, hitTime, _AfterLife);
        }
        else // VictoryPlayer.None
        {
            scoreBoad.PassViewOpen(victoryPlayer, hitPos, hitTime, _AfterLife);
            //resultView.PassViewOpen(victoryPlayer);
        }
        audioMixer.SetFloat("BGM", -20);                                    //BGMを元に戻す
    }

//**********************************************************************************************************

    public void PassHitCheck(int playerCode,HitPosition hitPos)
    {
        if (!playerReady[playerCode] && phaseCheck.NowPhaseGet == PhaseCheck.Phase.BattlePhase)
        {
            photonView.RPC("HitCheck", PhotonTargets.AllViaServer, playerCode, hitPos);
        }
            
    }
    /// <summary>
    /// 命中した情報を入力する。
    /// </summary>
    /// <param name="playerCode">命中させたプレイヤー(0=1P,1=2P)</param>
    /// <param name="HitPos">命中させた箇所</param>
    /// <param name="HitTime">命中時の時間(外れたり撃ってない場合は-1)</param>
    [PunRPC]
    public void HitCheck(int playerCode,HitPosition HitPos)
    {
        Debug.Log("<color=red>InHitCheck</color>" + phaseCheck.NowPhaseGet);
        if (!playerReady[playerCode] && phaseCheck.NowPhaseGet == PhaseCheck.Phase.BattlePhase)
        {
            Debug.Log("playerReady = false");
            hitPos[playerCode] = HitPos;
            hitTime[playerCode] = battle.GetTime;
            
            playerReady[playerCode] = true;

            if (HitPos == HitPosition.Null) hitTime[playerCode] = 5;
        }
    }

    /// <summary>
    /// 現在の体力を取得する
    /// </summary>
    /// <param name="TargetPlayer">0=1P,1=2P</param>
    /// <returns></returns>
    public int GetHP(int TargetPlayer)
    {
        return life[TargetPlayer];
    }

    /// <summary>
    /// 5ラウンド経過後の
    /// </summary>
    /// <returns>1P勝利=0,2P勝利=1,体力が同じ=2</returns>
    public int Judge()
    {
        if (life[0] == life[1]) return 2;
        else if (life[0] > life[1]) return 0;
        else return 1;
    }

    public void GameSet()
    {
        phaseCheck.NowPhaseGet = PhaseCheck.Phase.GameSetPhase;
        Debug.Log(phaseCheck.NowPhaseGet);
    }
}
