using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

//**************************************************************************************************************

    void Start()
    {
        phaseCheck = GetComponent<PhaseCheck>();    //フェイズを確認するために最初にPhaseCheckを取得する
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
	}

    /// <summary>
    /// 戦闘フェイズ終了の処理
    /// </summary>
    void BattleFinish()
    {
        time = 0;
        for (int i = 0; i < 2; i++) playerShot[i] = false;
        Debug.Log("ここでお互いの残り弾を打てないようにする。");
        phaseCheck.PhaseProgress();
    }

    //**************************************************************************************************************

    /// <summary>
    /// 発砲を行ったかを取得する。
    /// </summary>
    /// <param name="PlayerCode">発砲したプレイヤー(0=1P,1=2P)</param>
    [PunRPC]
    public void ShotCheck(int PlayerCode)
    {
        if (phaseCheck.NowPhaseGet == PhaseCheck.Phase.BattlePhase)
        {
            playerShot[PlayerCode] = true;
        }
        if (playerShot[0] && playerShot[1]) BattleFinish();
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
