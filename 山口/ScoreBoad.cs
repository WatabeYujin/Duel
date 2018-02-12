using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerContents
{
    public Result.HitPosition hitPosition = Result.HitPosition.Null;
    public float hitTime = 0;
    public int life = 7;

    /// <summary>
    /// TextUI配列　0:HitRes 1:TimeRes 2:WinOrLoss
    /// </summary>
    public ResultEvent[] textsEevaluate = new ResultEvent[3];
    public ResultEvent hpText = null;
    public Image[] bodyImages = new Image[3];
    public ResultEvent[] bodyImageResult = new ResultEvent[3];
    
}

public class ScoreBoad : MonoBehaviour
{
    [SerializeField]
    private ResultEvent fadeImage;
    [SerializeField]
    private Animator uiArrow;

    private PhotonView photonView;
    private PhaseCheck phaseCheck;
    private bool[] resultCheck = new bool[2];

    [SerializeField]
    Transform player1UI, player2UI;

    PlayerContents Player1;
    PlayerContents Player2;

    string[] uiString = { "Draw", "Win!", "Loss..", "Miss..", "Good!", "Great!", "Exciting!", "",""," " };
    
    // textSet のデフォルトの値 {hitpos, time, victriyPlayer}の順
    float[] valueDef        = { -1, -1, 2 };
    float[] startTimeDef    = { 1.5f, 3, 6 };

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        phaseCheck = GetComponent<PhaseCheck>();

        Player1 = ContentsSet(player1UI);
        Player2 = ContentsSet(player2UI);
        //Result.HitPosition[] e = { Result.HitPosition.Legs, Result.HitPosition.Null };
        //ViewOpen(Result.VictoryPlayer.Player1, e, valueDef, 6);
    }

    PlayerContents ContentsSet(Transform pUi)
    {
        PlayerContents pc = new PlayerContents { };
        GameObject hitBody = pUi.FindChild("HitBody").gameObject;
        GameObject textSet = pUi.FindChild("TextSet").gameObject;
        pc.hpText = pUi.FindChild("HPSet/HP").GetComponent<ResultEvent>();
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
        if (resultCheck[0] && resultCheck[1])
        {
            for (int i = 0; i < resultCheck.Length; i++)
            {
                resultCheck[i] = false;
            }
            photonView.RPC("ViewClose", PhotonTargets.AllViaServer);
        }
    }

    public void PassViewOpen(Result.VictoryPlayer Victory, Result.HitPosition[] HitPos, float[] HitTime, int AfterLife = 0)
    {
        Debug.Log(Victory + "," + HitPos[0] + "," + HitTime[0] + "," + AfterLife);
        Debug.Log(Victory + "," + HitPos[1] + "," + HitTime[1] + "," + AfterLife);
        uiArrow.SetBool("isStart", true);
        photonView.RPC("ViewOpen", PhotonTargets.AllViaServer, Victory, HitPos, HitTime, AfterLife);
    }
    /// <summary>
    /// リザルトを開く
    /// </summary>
    /// <param name="HitPos">命中箇所 0 = 1P 1 = 2P</param>
    /// <param name="HitTime">命中までの時間 0 = 1P 1 = 2P</param>
    /// <param name="Victory">勝者</param>
    /// <param name="AfterLife">変動した後のライフ</param>
    [PunRPC]
    public void ViewOpen(Result.VictoryPlayer Victory, Result.HitPosition[] HitPos, float[] HitTime, int AfterLife = 0)
    {
        Player1.hitTime = HitTime[0];
        Player2.hitTime = HitTime[1];
        Player1.hitPosition = HitPos[0];
        Player2.hitPosition = HitPos[1];

        // uiStringの7,8番目はヒットタイムが入る
        uiString[7] = (Mathf.Floor(Player1.hitTime * 100) / 100).ToString() + "s!";
        uiString[8] = (Mathf.Floor(Player2.hitTime * 100) / 100).ToString() + "s!";

        // ここにuiStringを見て値をセットするとその文字が出る
        //                  {hitpos, time, victriyPlayer}の順
        int[] stringNum1P = null;
        int[] stringNum2P = null;
        int p1 = (int)Player1.hitPosition + 3;
        int p2 = (int)Player2.hitPosition + 3;
        Debug.Log(p1 + " " + p2);
        switch (Victory)
        {
            case Result.VictoryPlayer.None: // 引き分け
                stringNum1P = new int[] { p1, 7, 0 };
                stringNum2P = new int[] { p2, 8, 0 };
                break;
            case Result.VictoryPlayer.Player1:
                stringNum1P = new int[] { p1, 7, 1 };
                stringNum2P = new int[] { p2, 9, 2 };
                Player2.hitPosition = Result.HitPosition.Null;
                Player2.life = AfterLife;
                break;
            case Result.VictoryPlayer.Player2:
                stringNum1P = new int[] { p1, 9, 2 };
                stringNum2P = new int[] { p2, 8, 1 };
                Player1.hitPosition = Result.HitPosition.Null;
                Player1.life = AfterLife;
                break;
            default:
                Debug.LogError("Victoryエラー");
                break;
        }
        StartCoroutine(BodyBrenk(Player1));
        StartCoroutine(BodyBrenk(Player2));
        ScoreTextUISet(Player1, stringNum1P);
        ScoreTextUISet(Player2, stringNum2P);
    }

    public void PassResultCheck(int playerCode)
    {
        Debug.Log(playerCode);
        photonView.RPC("ResultSCheck", PhotonTargets.AllViaServer, playerCode);
    }

    public void ScoreTextUISet(PlayerContents player, int[] textnum)
    {
        for (int i = 0; i < 3; i++)
        {
            player.textsEevaluate[i].SetText(uiString[textnum[i]], valueDef[i], startTimeDef[i]);
        }
        player.hpText.SetText(player.life.ToString(), -1, 8);
    }

    IEnumerator BodyBrenk(PlayerContents player)
    {
        for(int i = 0; i < 3; i++)
        {
            player.bodyImageResult[i].SetImage();
        }
        if (player.hitPosition == Result.HitPosition.Null) yield break;
        for (int i = 0; i < 3; i++)
        {
            Debug.Log(player.hitPosition);
            player.bodyImages[(int)player.hitPosition - 1].color = Color.red;
            yield return new WaitForSeconds(0.5f);
            player.bodyImages[(int)player.hitPosition - 1].color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }
        player.bodyImages[(int)player.hitPosition - 1].color = Color.red;
    }

/// <summary>
/// リザルトを送るためのチェック
/// </summary>
/// <param name="playerCode">プレイヤーのID</param>
[PunRPC]
    public void ResultSCheck(int playerCode)
    {
        if (phaseCheck.NowPhaseGet == PhaseCheck.Phase.ResultPhase)
        {
            if (!resultCheck[playerCode])
            {
                resultCheck[playerCode] = true;
                Debug.Log("Player" + (playerCode + 1) + ":" + resultCheck[playerCode]);
            }
        }
    }

    /// <summary>
    /// リザルトを閉じる
    /// </summary>
    [PunRPC]
    public void ViewClose()
    {
        fadeImage.CloseEvent();
        for(int i = 0; i < 3; i++)
        {
            Player1.textsEevaluate[i].Invoke("TextDelete", 1);
            Player2.textsEevaluate[i].Invoke("TextDelete", 1);
            Player1.bodyImageResult[i].Invoke("ImageDelete", 1);
            Player2.bodyImageResult[i].Invoke("ImageDelete", 1);
        }
        Player1.hpText.Invoke("TextDelete", 1);
        Player2.hpText.Invoke("TextDelete", 1);

        uiArrow.SetBool("isStart", false);
        

        if (phaseCheck.NowPhaseGet == PhaseCheck.Phase.ResultPhase)
        {
            phaseCheck.PhaseProgress();
        }
    }
}