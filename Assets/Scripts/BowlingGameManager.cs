using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GamePhase { Ready, Charging, BallRolling, PinsSettling, Scoring, Resetting, GameOver, Paused }

public sealed class BowlingGameManager : MonoBehaviour
{
    [SerializeField] BowlingBallController ball;
    [SerializeField] PinDeckController deck;
    [SerializeField] BowlingHUD hud;
    [SerializeField] Transform deadZone;
    public GamePhase Phase { get; private set; }
    public BowlingScore Score { get; } = new();
    public event Action<GamePhase> PhaseChanged;
    float rollStarted, stillTime; GamePhase prePause; ArcadeAudio arcadeAudio;

    void Awake() { BowlingSettings.LoadAndApply(); arcadeAudio=GetComponent<ArcadeAudio>(); }
    void Start()
    {
        ball.ChargeStarted += BeginCharge; ball.Launched += BeginRoll;
        RestartMatch();
    }
    void OnDestroy() { if(ball!=null){ball.ChargeStarted-=BeginCharge;ball.Launched-=BeginRoll;} }
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) TogglePause();
        if (Phase == GamePhase.Charging) ball.TickCharging();
        if (Phase == GamePhase.BallRolling)
        {
            bool rear = deadZone != null && ball.transform.position.z >= deadZone.position.z;
            bool still = ball.Body.linearVelocity.sqrMagnitude < .04f;
            stillTime = still ? stillTime + Time.deltaTime : 0;
            if (rear || stillTime >= 1f || Time.time-rollStarted >= 12f) StartCoroutine(SettleAndScore());
        }
        hud?.Refresh(this, ball);
    }
    void BeginCharge() => SetPhase(GamePhase.Charging);
    void BeginRoll(float power) { rollStarted=Time.time; stillTime=0; SetPhase(GamePhase.BallRolling); arcadeAudio?.PlayLaunch(); hud?.ShowFeedback(power>.85f?"MAX POWER!":"ROLLING..."); }
    IEnumerator SettleAndScore()
    {
        SetPhase(GamePhase.PinsSettling); yield return new WaitForSeconds(2f); SetPhase(GamePhase.Scoring);
        int knocked = Mathf.Clamp(deck.CountFallenPins(),0,Score.MaxPinsForNextRoll()); int frameBefore=Score.CurrentFrame, rollBefore=Score.CurrentRoll;
        Score.RecordRoll(knocked); arcadeAudio?.PlayPins(knocked); hud?.ShowFeedback(knocked==10?"STRIKE!": knocked==0?"GUTTER": knocked+" PINS");
        if (Score.IsGameOver) { SetPhase(GamePhase.GameOver); hud?.ShowResults(Score.FinalScore); yield return new WaitForSeconds(5f); if(Phase==GamePhase.GameOver) RestartMatch(); yield break; }
        SetPhase(GamePhase.Resetting);
        bool frameComplete = Score.CurrentFrame != frameBefore;
        bool tenthBonus = frameBefore==10 && (knocked==10 || (rollBefore==2 && Score.Rolls[^2]+knocked==10));
        if (frameComplete || knocked==10 || tenthBonus) deck.Rerack(); else deck.PrepareSecondRoll();
        ball.ResetBall(); yield return new WaitForSeconds(.65f); SetPhase(GamePhase.Ready);
    }
    public void RestartMatch()
    {
        StopAllCoroutines(); Time.timeScale=1; Score.Reset(); deck.Rerack(); ball.ResetBall(); hud?.HideOverlays(); SetPhase(GamePhase.Ready);
    }
    public void TogglePause()
    {
        if (Phase==GamePhase.GameOver) return;
        if (Phase==GamePhase.Paused) { Time.timeScale=1; hud?.ShowPause(false); SetPhase(prePause); }
        else { prePause=Phase; Time.timeScale=0; hud?.ShowPause(true); SetPhase(GamePhase.Paused); }
    }
    public void MainMenu() { Time.timeScale=1; StopAllCoroutines(); SceneManager.LoadScene("MainMenu"); }
    void SetPhase(GamePhase next) { Phase=next; ball.SetControlsEnabled(next==GamePhase.Ready); PhaseChanged?.Invoke(next); }
}
