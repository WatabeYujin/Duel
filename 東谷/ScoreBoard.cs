using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerContents
{
    //public Result.HitPosition hitPosition = Result.HitPosition.Null;
    //public float hitTime = 0;
    //public int life = 7;

    /// <summary>
    /// TextUI配列　0:HitRes 1:TimeRes 2:WinOrLoss
    /// </summary>
    public ResultEvent[] textsEevaluate = new ResultEvent[3];
    public ResultEvent hpText = null;
    public Text resultText = null;
    public Image[] bodyImages = new Image[3];
    public ResultEvent[] bodyImageResult = new ResultEvent[3];
    
}

public class ScoreBoard : MonoBehaviour
{
    [SerializeField]
    private ResultEvent fadeImage;
    [SerializeField]
    private Animator uiArrow;

    private GameCommonData gameData;
    private bool[] resultCheck = new bool[2];

    [SerializeField]
    Transform player1UI, player2UI;

    PlayerContents Player1UI;
    PlayerContents Player2UI;

    string[] uiString = { "Draw", "Win!", "Lose..", "Miss..", "Good!", "Great!", "Exciting!", "",""," " };
    
    // textSet のデフォルトの値 {hitpos, time, victriyPlayer}の順
    float[] valueDef        = { -1, -1, 2 };
    float[] startTimeDef    = { 1.5f, 3, 6 };

    public bool viewSet;

    void Start()
    {
        gameData = GetComponent<GameCommonData>();

        Player1UI = ContentsSet(player1UI);
        Player2UI = ContentsSet(player2UI);
        //Result.HitPosition[] e = { Result.HitPosition.Legs, Result.HitPosition.Null };
        //ViewOpen(Result.VictoryPlayer.Player1, e, valueDef, 6);
    }

    PlayerContents ContentsSet(Transform pUi)
    {
        PlayerContents pc = new PlayerContents { };
        GameObject hitBody = pUi.FindChild("HitBody").gameObject;
        GameObject textSet = pUi.FindChild("TextSet").gameObject;
        pc.hpText = pUi.FindChild("HPSet/HP").GetComponent<ResultEvent>();
        pc.resultText = pUi.FindChild("ResultText").GetComponent<Text>();
        for (int i = 0; i < 3; i++)
        {
            pc.bodyImages[i] = hitBody.transform.GetComponentsInChildren<Image>()[i];
            pc.textsEevaluate[i] = textSet.transform.GetComponentsInChildren<ResultEvent>()[i];
            pc.bodyImageResult[i] = pc.bodyImages[i].GetComponent<ResultEvent>();
        }
        return pc;
    }

    void Update()
    {
        if (gameData.GetNowPhase == GameCommonData.Phase.ResultPhase)
        {
            if (gameData.player1P.IsEndResult && gameData.player2P.IsEndResult)
            {
                viewSet = false;
                gameData.myData.IsEndResult = false;
                gameData.PassPlayerUpdata();
                ViewClose();
            }
            else if (gameData.player1P.IsResultView && gameData.player2P.IsResultView && !viewSet)
            {
                viewSet = true;
                uiArrow.SetBool("isStart", true);

                ViewOpen();
                gameData.myData.ResetFlag();
                gameData.PassPlayerUpdata();
            }
        }
        if(gameData.GetNowPhase == GameCommonData.Phase.GameSetPhase)
        {
            GameSet();
        }
    }
    /// <summary>
    /// リザルトを開く
    /// </summary>
    /// <param name="HitPos">命中箇所 0 = 1P 1 = 2P</param>
    /// <param name="HitTime">命中までの時間 0 = 1P 1 = 2P</param>
    /// <param name="Victory">勝者</param>
    /// <param name="AfterLife">変動した後のライフ</param>
    //[PunRPC]
    public void ViewOpen(/*Result.VictoryPlayer Victory, Result.HitPosition[] HitPos, float[] HitTime, int AfterLife = 0*/)
    {
        //Player1.hitTime = HitTime[0];
        //Player2.hitTime = HitTime[1];
        //Player1.hitPosition = HitPos[0];
        //Player2.hitPosition = HitPos[1];

        // uiStringの7,8番目はヒットタイムが入る
        uiString[7] = (Mathf.Floor(gameData.player1P.HitTime * 100) / 100).ToString() + "s!";
        uiString[8] = (Mathf.Floor(gameData.player2P.HitTime * 100) / 100).ToString() + "s!";

        // ここにuiStringを見て値をセットするとその文字が出る
        //                  {hitpos, time, victriyPlayer}の順
        int[] stringNum1P = null;
        int[] stringNum2P = null;
        int p1 = (int)gameData.player1P.HitPos + 3;
        int p2 = (int)gameData.player2P.HitPos + 3;
        switch (gameData.victoryPlayer)
        {
            case GameCommonData.VictoryPlayer.None: // 引き分け
                stringNum1P = new int[] { p1, 7, 0 };
                stringNum2P = new int[] { p2, 8, 0 };
                break;
            case GameCommonData.VictoryPlayer.Player1:
                stringNum1P = new int[] { p1, 7, 1 };
                stringNum2P = new int[] { p2, 9, 2 };
                gameData.player2P.HitPos = GameCommonData.HitPosition.Null;
                //gameData.player2P.Life = gameData.player1P.EnemyLife;
                break;
            case GameCommonData.VictoryPlayer.Player2:
                stringNum1P = new int[] { p1, 9, 2 };
                stringNum2P = new int[] { p2, 8, 1 };
                gameData.player1P.HitPos = GameCommonData.HitPosition.Null;
                //gameData.player1P.Life = gameData.player2P.EnemyLife;
                break;
            default:
                Debug.LogError("Victoryエラー");
                break;
        }

        Debug.Log(gameData.player1P.EnemyLife + "  " + gameData.player2P.EnemyLife);
        gameData.PassPlayerUpdata();
        StartCoroutine(BodyBrenk(Player1UI, gameData.player1P));
        StartCoroutine(BodyBrenk(Player2UI, gameData.player2P));
        ScoreTextUISet(Player1UI, stringNum1P, gameData.player2P.EnemyLife);
        ScoreTextUISet(Player2UI, stringNum2P, gameData.player1P.EnemyLife);
    }

    //public void PassResultCheck(int playerCode)
    //{
    //    Debug.Log(playerCode);
    //    photonView.RPC("ResultSCheck", PhotonTargets.AllViaServer, playerCode);
    //}

    public void ScoreTextUISet(PlayerContents player, int[] textnum, int life)
    {
        for (int i = 0; i < 3; i++)
        {
            player.textsEevaluate[i].SetText(uiString[textnum[i]], valueDef[i], startTimeDef[i]);
        }
        player.hpText.SetText(life.ToString(), -1, 8);
    }

    IEnumerator BodyBrenk(PlayerContents player, PlayerData pd)
    {
        for(int i = 0; i < 3; i++)
        {
            player.bodyImageResult[i].SetImage();
        }
        if (pd.HitPos == GameCommonData.HitPosition.Null) yield break;
        for (int i = 0; i < 3; i++)
        {
            Debug.Log(pd.HitPos);
            player.bodyImages[(int)pd.HitPos - 1].color = Color.red;
            yield return new WaitForSeconds(0.5f);
            player.bodyImages[(int)pd.HitPos - 1].color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }
        player.bodyImages[(int)pd.HitPos - 1].color = Color.red;
    }

/// <summary>
/// リザルトを送るためのチェック
/// </summary>
/// <param name="playerCode">プレイヤーのID</param>
//[PunRPC]
//    public void ResultSCheck(int playerCode)
//    {
//        if (gameData.GetNowPhase == GameCommonData.Phase.ResultPhase)
//        {
//            if (!resultCheck[playerCode])
//            {
//                resultCheck[playerCode] = true;
//                Debug.Log("Player" + (playerCode + 1) + ":" + resultCheck[playerCode]);
//            }
//        }
//    }

    /// <summary>
    /// リザルトを閉じる
    /// </summary>
    //[PunRPC]
    public void ViewClose()
    {
        fadeImage.CloseEvent();
        for(int i = 0; i < 3; i++)
        {
            Player1UI.textsEevaluate[i].Invoke("TextDelete", 1);
            Player2UI.textsEevaluate[i].Invoke("TextDelete", 1);
            Player1UI.bodyImageResult[i].Invoke("ImageDelete", 1);
            Player2UI.bodyImageResult[i].Invoke("ImageDelete", 1);
        }
        Player1UI.hpText.Invoke("TextDelete", 1);
        Player2UI.hpText.Invoke("TextDelete", 1);

        uiArrow.SetBool("isStart", false);
        

        if (gameData.GetNowPhase == GameCommonData.Phase.ResultPhase)
        {
            Debug.Log(gameData.player1P.Life + "  " + gameData.player2P.Life);
            Debug.Log(gameData.player1P.EnemyLife + "  " + gameData.player2P.EnemyLife);
            if (gameData.player1P.EnemyLife <= 0 || gameData.player2P.EnemyLife <= 0)
            {
                Debug.Log("gameSet");
                gameData.SetNowPhase = GameCommonData.Phase.GameSetPhase;
            }
            else
            {
                gameData.PhaseProgress();
            }
        }
    }

    /// <summary>
    /// ゲームセット
    /// </summary>
    private void GameSet()
    {
        //Debug.Log(gameData.player1P.Life + " " + gameData.player2P.Life);
        if (gameData.GetNowPhase == GameCommonData.Phase.GameSetPhase)
        {
            if(gameData.player1P.EnemyLife == gameData.player2P.EnemyLife)
            {
                Player1UI.resultText.text = uiString[0];
                Player2UI.resultText.text = uiString[0];
                Player1UI.resultText.color = Color.green;
                Player2UI.resultText.color = Color.green;
            }
            else if (gameData.player1P.EnemyLife < gameData.player2P.EnemyLife)
            {
                Player1UI.resultText.text = uiString[1];
                Player2UI.resultText.text = uiString[2];
                Player1UI.resultText.color = Color.red;
                Player2UI.resultText.color = Color.blue;
            }
            else if(gameData.player1P.EnemyLife > gameData.player2P.EnemyLife)
            {
                Player1UI.resultText.text = uiString[2];
                Player2UI.resultText.text = uiString[1];
                Player1UI.resultText.color = Color.blue;
                Player2UI.resultText.color = Color.red;
            }
        }
    }
}