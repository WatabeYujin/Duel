using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultView : MonoBehaviour{
    /// <summary>
    /// リザルト結果を表示するスクリプト
    /// Result.csより情報を受け取っている
    /// </summary>
    
    [SerializeField]
    private ResultEvent hitPositionText;                            //命中箇所を表示させるText
    [SerializeField]
    private ResultEvent[] playerHitTimeText = new ResultEvent[2];   //YourHitTimeと表示させるtextとプレイヤーの着弾までの時間をあらわすtext
    [SerializeField]
    private ResultEvent[] enemyHitTimeText = new ResultEvent[2];    //EnemyHitTimeと表示させるtextと敵の着弾までの時間を表示させるText
    [SerializeField]
    private ResultEvent victoryText;                                //勝敗を表示させるText
    [SerializeField]
    private ResultEvent[] lifeText = new ResultEvent[2];            //両者のライフ数のtext
    [SerializeField]
    private ResultEvent fadeImage;                                  //resultの上にフェードインで表示する看板の画像

    //***********************************************************************************************************************************

    /// <summary>
    /// リザルトを開く
    /// </summary>
    /// <param name="HitPos">命中箇所</param>
    /// <param name="HitTime">命中までの時間</param>
    /// <param name="Victory">勝者</param>
    /// <param name="AfterLife">変動した後のライフ</param>
    public void ViewOpen(Result.HitPosition HitPos,float HitTime,Result.VictoryPlayer Victory,int AfterLife)
    {
        StartCoroutine(ViewClose());

        switch (HitPos)
        {
            case Result.HitPosition.Null:

                hitPositionText.SetText("Miss...",-1,1.5f);
                break;
            case Result.HitPosition.Legs:
                hitPositionText.SetText("Hit!Legs!", -1, 1.5f);
                break;
            case Result.HitPosition.Arms_Body:
                hitPositionText.SetText("Great!  Arms or Body!", -1, 1.5f);
                break;
            case Result.HitPosition.Head:
                hitPositionText.SetText("Exciting!  HeadShot!", -1, 1.5f);
                break;
        }
        if (HitPos != Result.HitPosition.Null)
        {
            playerHitTimeText[0].SetText("YourHitTime is ... ",-1,2.5f);
            playerHitTimeText[1].SetText(HitTime.ToString() + "s!", -1f,3);
        }
        else for(int i=0;i<2;i++) playerHitTimeText[i].SetText(" ", 0,2.5f);
        if (HitPos != Result.HitPosition.Null)
        {
            enemyHitTimeText[0].SetText("EnemyHitTime is ...", -1, 4);
            enemyHitTimeText[1].SetText(HitTime.ToString() + "s!", -1f, 4.5f);
        }
        else for (int i = 0; i < 2; i++) enemyHitTimeText[i].SetText(" ", 0, 4f);
        switch (Victory)
        {
            case Result.VictoryPlayer.None:
                victoryText.SetText("Draw",2,6f);
                break;
            case Result.VictoryPlayer.Player1:
                victoryText.SetText("Victory!!!",2, 6f);
                lifeText[0].SetText(null, -1f, 8);
                lifeText[1].SetText(AfterLife.ToString(),-1f,8);
                break;
            default:
                victoryText.SetText("Lose...",2, 6f);
                lifeText[0].SetText(AfterLife.ToString(), -1f, 8);
                lifeText[1].SetText(null, -0.5f, 8);
                break;
        }
    }

    /// <summary>
    /// リザルトを閉じる
    /// </summary>
    private IEnumerator ViewClose()
    {
        yield return new WaitForSeconds(9.0f);

        fadeImage.CloseEvent();
        hitPositionText.Invoke("TextDelete", 3);
        for (int i = 0; i < 2; i++) {
            playerHitTimeText[i].Invoke("TextDelete", 3);
            enemyHitTimeText[i].Invoke("TextDelete", 3);
        }
        victoryText.Invoke("TextDelete", 3);
    }
}
