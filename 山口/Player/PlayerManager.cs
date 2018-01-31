using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour 
{
    /************************************************
     * クラス変数
     ***********************************************/

    private PhotonView pmphotonView = null;
    private Color color = Color.white;
    private Renderer render = null;

    /************************************************
     * 定数
     ***********************************************/

    private readonly Color[] MaterialColors = new Color[]
    {
        Color.white, Color.red, Color.green, Color.blue, Color.yellow,
    };

    /************************************************
     * クラス関数
     ***********************************************/

    void Awake()
    {
        pmphotonView = GetComponent<PhotonView>();
        render = GetComponent<Renderer>();
    }
	// Use this for initialization
	void Start () 
    {
        int ownerId = pmphotonView.ownerId;
        color = MaterialColors[ownerId];
        render.material.color = color;
	}
    //----------------------------------
}
