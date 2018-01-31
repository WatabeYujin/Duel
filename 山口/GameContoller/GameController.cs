using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour {

    //ゲームのログを保存しておく。shot1,shot2,hit1,hit2の時間を保存。
    private Dictionary<string, float> gameLog = new Dictionary<string, float>();

    private bool isGameStart;
    private float gameTime;


    // Use this for initialization

    #region //シングルトン
    public static GameController Instance { private set; get; }
    private void Awake()
    {
        if (Instance != null) return;
        else
        {
            Instance = this;
        }
    }
    #endregion

    void Start()
    {
        
    }
	
	// Update is called once per frame
	void Update () {
        if (isGameStart)
        {
            gameTime += Time.deltaTime;
        }
	}

    public void ShootTime(int i)
    {
        string key = "shot" + i.ToString();
        if (gameLog.ContainsKey(key) == false)
        {
            //myPhotonView.RPC("SpornBullet", PhotonTargets.AllViaServer, i);
            gameLog.Add("shot" + i.ToString(), gameTime);
        }
        Debug.Log(gameLog[key]);
    }

    /*[PunRPC]
    private void SpornBullet(int i)
    {
        GameObject cloneBullet = PhotonNetwork.Instantiate(objName, emp[i].position, Quaternion.identity, 0);
        cloneBullet.GetComponent<Rigidbody>().AddForce(emp[i].right * 1000);
    }*/

    public void GameStart()
    {
        isGameStart = true;
    }

    [PunRPC]
    public void HitLog(string s,int i)
    {
        Debug.Log(i);
        if (i == 1)
        {
            gameLog.Add(s + i.ToString(), gameTime);
        }
        else
        {
            gameLog.Add(s + i.ToString(), gameTime);
        }
        Debug.Log(gameLog[s + i.ToString()] + " : " + gameTime);
    }

}
