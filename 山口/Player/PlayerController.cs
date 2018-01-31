using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour 
{
    /************************************************
     * クラス変数
     ***********************************************/

    private PhotonView myPhotonView = null;

    [SerializeField]
    private float moveSpeed = 10.0f;
    [SerializeField]
    private string bulletName = null;
    [SerializeField]
    private Transform bulletSpawn;

    /************************************************
     * クラス関数
     ***********************************************/

    void Awake()
    {
        myPhotonView = GetComponent<PhotonView>();
    }
	// Use this for initialization
	void Start () 
    {
        
        bulletName = "Bullet";
              
	}

    // Update is called once per frame
    void Update()
    {
        //持ち主でないのなら制御させない
        if (!myPhotonView.isMine)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            myPhotonView.RPC("Shoot", PhotonTargets.AllViaServer, PhotonNetwork.player.ID);
        }

        


        PlayerMove();
    }   
    //--------------------------------------------
    
    // 弾を撃ったプレイヤーと弾を生成したプレイヤーのIDが同じ場合、Bulletの処理を行う
    [PunRPC]
    private void Shoot(int playerID)
    {
        var obj = GameObject.Instantiate(Resources.Load(bulletName), bulletSpawn.position, transform.rotation) as GameObject;
        var bullet = obj.GetComponent<Bullet>();

        // Bulletに持ち主を判別させる
        bullet.Initialize(playerID == PhotonNetwork.player.ID);
    }

    //移動
    private void PlayerMove()
    {
        Vector3 pos = transform.position;
       

        pos.x += Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
      
        pos.z += Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        
        transform.position = pos;
    }
    //-------------------------------------
   
}
