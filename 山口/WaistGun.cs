using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// スタンバイフェイズ時の腰の判定のスクリプト
/// </summary>
public class WaistGun : MonoBehaviour {
    /***********************************
     * クラス変数
     **********************************/
    //private bool isStandby = false;
    private Standby standby;
    private PhaseCheck phaseCheck;

    /***********************************
     * プロパティ
     **********************************/
    /*public bool IsStandby
    {
        get { return isStandby; }
    }*/
	
    /***********************************
     * クラス関数
     **********************************/
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "alpha" && standby == null)
        {
            GameObject battleController = GameObject.Find("BatteleController");
            standby = battleController.GetComponent<Standby>();
            phaseCheck = battleController.GetComponent<PhaseCheck>();


        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Gun" && phaseCheck.NowPhaseGet == PhaseCheck.Phase.StandbyPhase)
        {
            standby.PassReadyCheck(PhotonNetwork.player.ID - 1, true);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Gun")
        {
            standby.PassReadyCheck(PhotonNetwork.player.ID - 1, false);
        }
    }
}
