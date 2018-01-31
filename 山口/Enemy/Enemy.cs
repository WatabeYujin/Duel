using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    /*****************************************
    * クラス変数
    *****************************************/

    private PhotonView ePhotonview = null;
    private Renderer eRender = null;
    private Color eColor;

    [SerializeField]
    private Text elifeTxt;
    [SerializeField]
    private int eMaxlife = 100;
    [SerializeField]
    private int elife;

    /**********************************************
    * プロパティ
    **********************************************/
    public static Enemy Instance
    {       
        get; 
        private set; 
    }

    /**********************************************
    * クラス関数
    **********************************************/
    private void Awake()
    {
        ePhotonview = GetComponent<PhotonView>();

        if (Instance != null) return;
        else
        {
            Instance = this;
        }
    }

    // Use this for initialization
    void Start () {
        elife = eMaxlife;
        elifeTxt.text = elife.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        if (!ePhotonview.isMine)
        {
            return;
        }
	}
    //--------------------------------------------

    //ダメージ処理     
    public void DamageDeli(int i,string s)
    {
        ePhotonview.RPC("Damage", PhotonTargets.AllViaServer, i);
        ePhotonview.RPC("ColorChange", PhotonTargets.AllViaServer, s);               
    }

    [PunRPC]
    private void Damage(int damage)
    {
        Debug.Log(damage);
        elife -= damage;
        if(elife <= 0)
        {
            elife = eMaxlife;
        }
        elifeTxt.text = elife.ToString();
    }

    //弾が当たった部位を赤く点滅させる
    [PunRPC]
    private void ColorChange(string parts)
    {
        Debug.Log(parts);
        GameObject child = GameObject.FindGameObjectWithTag(parts);
        
        eRender = child.GetComponent<Renderer>();
        eColor = eRender.material.color;        

        eRender.material.color = Color.red;
        StartCoroutine(Pause());  
    }

    private IEnumerator Pause()
    {
        yield return new WaitForSeconds(0.1f);
        eRender.material.color = eColor;
        
    }
    //-------------------------------------------
}
