using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    float hitTime;
    int playerId;
    int life = 7;
    int enemyLife = 7;
    int bulletSpeed;

    bool isStanby = false;
    bool isShot = false;
    bool isEndHitCheck = false;
    bool isResultView = false;
    bool isEndResult = false;
    bool isBattleStart = false;

    GameCommonData.HitPosition hitPos = GameCommonData.HitPosition.Null;
    
    public float HitTime
    {
        get { return hitTime; }
        set { hitTime = value; }
    }
    public int PlayerId
    {
        get { return playerId; }
    }
    public int Life
    {
        get { return life; }
        set { life = value; }
    }
    public int EnemyLife
    {
        get { return enemyLife; }
        set { enemyLife = value; }
    }
    public int BulletSpeed
    {
        get { return bulletSpeed; }
        set { bulletSpeed = value; }
    }

    public bool IsStanby
    {
        get { return isStanby; }
        set { isStanby = value; }
    }
    public bool IsShot
    {
        get { return isShot; }
        set { isShot = value; }
    }
    public bool IsEndHitCheck
    {
        get { return isEndHitCheck; }
        set { isEndHitCheck = value; }
    }
    public bool IsResultView
    {
        get { return isResultView; }
        set { isResultView = value; }
    }
    public bool IsEndResult
    {
        get { return isEndResult; }
        set { isEndResult = value; }
    }
    public bool IsBattleStart
    {
        get { return isBattleStart; }
        set { isBattleStart = value; }
    }

    public GameCommonData.HitPosition HitPos
    {
        get { return hitPos; }
        set { hitPos = value; }
    }

    public PlayerData(int p, int speed)
    {
        playerId = p;
        bulletSpeed = speed;
    }

    public void ResetFlag()
    {
        isStanby = false;
        isShot = false;
        isEndHitCheck = false;
        isResultView = false;
        isEndResult = false;
        isBattleStart = false;

        hitPos = GameCommonData.HitPosition.Null;
        hitTime = 0;
    }
}

public class GameCommonData : MonoBehaviour {
    #region enumの定義
    public enum HitPosition                             //命中箇所のenum
    {
        Null,
        Legs,
        Arms_Body,
        Head
    }

    [HideInInspector]
    public enum VictoryPlayer                           //勝者のenum
    {
        None = -1,
        Player1,
        Player2
    }
    public VictoryPlayer victoryPlayer = VictoryPlayer.None;


    [HideInInspector]
    public enum Phase                               //フェイズのenmu
    {
        StandbyPhase,                               //準備
        CointossPhase,                              //コイントス
        BattlePhase,                                //射撃
        ResultPhase,                                //リザルト
        GameSetPhase,                               //ゲームセット
    }
    [SerializeField]
    private Phase nowPhase = Phase.StandbyPhase;  //現在のフェイズ

    #endregion

    [HideInInspector]
    public PhotonView photonView;
    [HideInInspector]
    public Battle battle;
    [HideInInspector]
    public Cointoss cointoss;
    [HideInInspector]
    public Result result;
    [HideInInspector]
    public Standby standby;
    [HideInInspector]
    public ScoreBoard scoreBoard;
    
    public int defBulletSpeed;
    public int foulBulletSpeed;

    [SerializeField]
    private int roundCount; //現在のラウンド数

    public PlayerData player1P;
    public PlayerData player2P;

    public PlayerData myData;


    // Use this for initialization
    void Start () {
        photonView = GetComponent<PhotonView>();
        battle = GetComponent<Battle>();
        cointoss = GetComponent<Cointoss>();
        result = GetComponent<Result>();
        standby = GetComponent<Standby>();
        scoreBoard = GetComponent<ScoreBoard>();

        player1P = new PlayerData(1, defBulletSpeed);
        player2P = new PlayerData(2, defBulletSpeed);

        if (PhotonNetwork.player.ID == 1) myData = player1P;
        else myData = player2P;
        PassPlayerUpdata();
    }
	
    public void PassPlayerUpdata()
    {
        
        photonView.RPC("PlayerUpdata", PhotonTargets.AllViaServer, DataDisassemblyInt(myData),DataDisassemblyFloat(myData),DataDisassemblyBool(myData));
    }

    /// <summary>
    /// Playerの更新
    /// </summary>
    /// <param name="data"> 1P　2P　のMyDataが来る </param>
    [PunRPC]
    public void PlayerUpdata(int[] i, float ht, bool[] b)
    {
        PlayerData data = SetPlayerData(i, ht, b);
        if (data.PlayerId == player1P.PlayerId) player1P = data;
        else player2P = data;
    }

    public int[] DataDisassemblyInt(PlayerData pd)
    {
        int[] i = new int[5];
        i[0] = pd.PlayerId;
        i[1] = pd.Life;
        i[2] = pd.EnemyLife;
        i[3] = pd.BulletSpeed;
        i[4] = (int)pd.HitPos;
        return i;
    }

    public float DataDisassemblyFloat(PlayerData pd)
    {
        return pd.HitTime;
    }

    public bool[] DataDisassemblyBool(PlayerData pd)
    {
        bool[] b = new bool[6];
        b[0] = pd.IsStanby;
        b[1] = pd.IsShot;
        b[2] = pd.IsEndHitCheck;
        b[3] = pd.IsResultView;
        b[4] = pd.IsEndResult;
        b[5] = pd.IsBattleStart;
        return b;
    }

    public PlayerData SetPlayerData(int[] i, float ht, bool[] b)
    {
        PlayerData pd = new PlayerData(i[0],i[3]);
        pd.Life = i[1];
        pd.EnemyLife = i[2];
        pd.BulletSpeed = i[3];
        pd.HitPos = (HitPosition)i[4];

        pd.HitTime = ht;

        pd.IsStanby = b[0];
        pd.IsShot = b[1];
        pd.IsEndHitCheck = b[2];
        pd.IsResultView = b[3];
        pd.IsEndResult = b[4];
        pd.IsBattleStart = b[5];

        return pd;
    }

    // Update is called once per frame
    void Update ()
    {


#if UNITY_EDITOR

        if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))//デバッグ中（UnityEditor）でのみフェイズを進めるられるように
        {
            PhaseProgress();
        }
#endif
    }

    /////////////////////////////////////////////////////////////

    /// <summary>
    /// 現在のフェーズの状況の取得を行う
    /// </summary>
    public Phase GetNowPhase
    {
        get { return nowPhase; }
        // set { roundPhase = value; }
    }
    /// <summary>
    /// フェーズの指定変更
    /// </summary>
    public Phase SetNowPhase
    {
        set { nowPhase = value; }
    }
    /// <summary>
    /// フェイズの進行を行う
    /// </summary>
    public void PhaseProgress()
    {
        if (nowPhase == Phase.ResultPhase)
        {
            SetNowPhase = Phase.StandbyPhase;
            NextRound();
        }
        else nowPhase++;
        Debug.Log("<color=blue>" + nowPhase + RoundCountChack.ToString() + "</color>");
    }

    ////////////////////////////////////////////////////

    /// <summary>
    /// ラウンド数を取得する
    /// </summary>
    public int RoundCountChack
    {
        get { return roundCount; }
    }

    public void NextRound()
    {
        roundCount++;
        if (roundCount >= 6)
        {
            SetNowPhase = Phase.GameSetPhase;
        }
        Debug.Log("roundCount:" + roundCount);
    }

}
