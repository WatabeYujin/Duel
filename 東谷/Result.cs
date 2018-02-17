using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// 撃ち合いの結果を処理するフェイズ
/// </summary>
public class Result : MonoBehaviour
{
    GameCommonData gameData;
    private AudioMixer audioMixer;                      //音量調節に使用

    //**********************************************************************************************************

    void Start()
    {
        audioMixer = Resources.Load("Audio/AudioMixer") as AudioMixer;  //音量の調節を可能にするため
        
        gameData = GetComponent<GameCommonData>();
    }

    void Update()
    {
        if (gameData.GetNowPhase == GameCommonData.Phase.ResultPhase)
        {
            if (!gameData.myData.IsResultView && !gameData.scoreBoard.viewSet)
            {
                //両方外してる場合
                if (gameData.player1P.HitPos == GameCommonData.HitPosition.Null &&
                    gameData.player2P.HitPos == GameCommonData.HitPosition.Null)
                {
                    gameData.victoryPlayer = GameCommonData.VictoryPlayer.None;
                }
                else // 少なくともどちらかが当たっている場合
                {
                    if (gameData.player1P.HitTime < gameData.player2P.HitTime) // 1PWin
                    {
                        gameData.victoryPlayer = GameCommonData.VictoryPlayer.Player1;
                        if(gameData.myData.PlayerId == 1) ResetViewSet();
                    }
                    else
                    {
                        gameData.victoryPlayer = GameCommonData.VictoryPlayer.Player2;
                        if(gameData.myData.PlayerId == 2) ResetViewSet();
                    }
                }
                gameData.myData.IsResultView = true;
                
                gameData.PassPlayerUpdata();
            }
        }
    }
        
    /// <summary>
    /// HPの変動をセット
    /// </summary>
    private void ResetViewSet()
    {
        Debug.Log(gameData.victoryPlayer);
        if (gameData.victoryPlayer != GameCommonData.VictoryPlayer.None)
        {
            //_AfterLife = life[(int)victoryPlayer];
            // 自分の分のみを見る
            switch (gameData.myData.HitPos)
            {
                case GameCommonData.HitPosition.Head:
                    gameData.myData.EnemyLife = 0;
                    break;
                case GameCommonData.HitPosition.Arms_Body:
                    gameData.myData.EnemyLife -= 3;
                    break;
                case GameCommonData.HitPosition.Legs:
                    gameData.myData.EnemyLife -= 1;
                    break;
                case GameCommonData.HitPosition.Null:
                    break;

            }
        }
        audioMixer.SetFloat("BGM", -20);                                    //BGMを元に戻す
    }
}
