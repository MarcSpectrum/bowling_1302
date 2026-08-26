using System.Text;
using UnityEngine;
using UnityEngine.UI;

public sealed class BowlingHUD : MonoBehaviour
{
    [SerializeField] Text statusText, scoreText, feedbackText, resultsText, countdownText;
    [SerializeField] Image powerFill;
    [SerializeField] GameObject pausePanel, resultsPanel;
    float feedbackUntil;
    public void Refresh(BowlingGameManager game, BowlingBallController ball)
    {
        if(statusText) statusText.text=$"FRAME {game.Score.CurrentFrame}  •  ROLL {game.Score.CurrentRoll}  •  AIM {ball.AimAngle:+0;-0;0}°";
        if(powerFill) powerFill.fillAmount=ball.Charge01;
        if(scoreText)
        {
            var marks=game.Score.GetFrameMarks(); var totals=game.Score.GetCumulativeTotals(); var b=new StringBuilder();
            for(int i=0;i<10;i++) b.Append($"{i+1,2}\n{(string.IsNullOrEmpty(marks[i])?"·":marks[i]),3}\n{(totals[i]?.ToString()??"—"),3}  ");
            scoreText.text=b.ToString();
        }
        if(feedbackText && Time.unscaledTime>feedbackUntil) feedbackText.text="";
        if(resultsPanel && resultsPanel.activeSelf && countdownText) countdownText.text="NEW MATCH IN 5 SECONDS";
    }
    public void ShowFeedback(string value){if(feedbackText){feedbackText.text=value;feedbackUntil=Time.unscaledTime+2.5f;}}
    public void ShowPause(bool show){if(pausePanel)pausePanel.SetActive(show);}
    public void ShowResults(int score){if(resultsPanel)resultsPanel.SetActive(true);if(resultsText)resultsText.text=$"FINAL SCORE\n{score}";}
    public void HideOverlays(){if(pausePanel)pausePanel.SetActive(false);if(resultsPanel)resultsPanel.SetActive(false);}
}
