using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GameFlowManager : MonoBehaviour
{
    public static bool Playing = true;

    public string minigameId = "Battery";
    public float timeLimit = 120f;        // tiempo total
    public float threeStarsTime = 40f;    // 3 estrellas si terminas en este tiempo o menos
    public float twoStarsTime = 80f;      // 2 estrellas hasta aquí; después, 1
    public int pointsPerConnection = 100;
    public int pointsPerError = 50;
    public int pointsPerSecondLeft = 10;
    public TMP_Text timeText, scoreText, resultText;

    [Header("Multijugador")]
    public bool waitForStart = true;      // espera a que MatchStarter llame a BeginGame()
    public float autoStartSeconds = 0f;   // solo para probar sin red (en multijugador déjalo en 0)
    public TMP_Text rivalText;            // opcional: marcador del rival

    float timeLeft;
    int score, errors, lastCount;
    bool finished;
    float nextReport, nextRival;

    float Used => timeLimit - timeLeft;   // incluye lo que restaron los errores
    int Score => Mathf.Max(0, score);

    void OnEnable()
    {
        GameEvents.OnPenalty += AddPenalty;
        GameEvents.OnMinigameCompleted += OnCompleted;
        TerminalLogic.OnConnectionsChanged += OnConnections;
    }

    void OnDisable()
    {
        GameEvents.OnPenalty -= AddPenalty;
        GameEvents.OnMinigameCompleted -= OnCompleted;
        TerminalLogic.OnConnectionsChanged -= OnConnections;
    }

    void Start()
    {
        timeLeft = timeLimit;
        score = errors = lastCount = 0;
        finished = false;

        if (waitForStart)
        {
            Playing = false;
            SetInteractions(false);
            resultText.color = Color.white;
            resultText.text = "Esperando a que ambos estén listos...";
            if (autoStartSeconds > 0f) Invoke(nameof(BeginGame), autoStartSeconds);
        }
        else
        {
            Playing = true;
            resultText.text = "";
        }
        UpdateUI();
    }

    // Lo llama MatchStarter cuando los dos jugadores presionaron "Listo"
    public void BeginGame()
    {
        CancelInvoke(nameof(BeginGame));
        timeLeft = timeLimit;
        score = errors = lastCount = 0;
        finished = false;
        resultText.text = "";
        SetInteractions(true);
        Playing = true;
        UpdateUI();
    }

    void Update()
    {
        UpdateRival();
        ReportProgress();

        if (!Playing) return;
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f) Lose();
        UpdateUI();
    }

    void OnConnections(int n)
    {
        if (!Playing) return;
        score += (n - lastCount) * pointsPerConnection;
        lastCount = n;
        UpdateUI();
    }

    void AddPenalty(float seconds)
    {
        if (!Playing) return;
        errors++;
        score -= pointsPerError;
        timeLeft -= seconds;
        if (timeLeft <= 0f) Lose();
        UpdateUI();
    }

    void OnCompleted(string id)
    {
        if (Playing && id == minigameId) Win();
    }

    void Win()
    {
        Playing = false;
        finished = true;
        score += Mathf.RoundToInt(timeLeft) * pointsPerSecondLeft;
        int stars = Used <= threeStarsTime ? 3 : Used <= twoStarsTime ? 2 : 1;

        resultText.color = Color.green;
        resultText.text = $"¡Completado!  {stars} estrellas\nPuntaje: {Score}\nTiempo: {Format(Used)}  |  Errores: {errors}";
        LockInteractions();
        UpdateUI();
        Report();
        Debug.Log($"Victoria: {stars} estrellas, {Score} puntos");
    }

    void Lose()
    {
        if (!Playing) return;
        Playing = false;
        finished = true;
        timeLeft = 0f;

        resultText.color = Color.red;
        resultText.text = $"¡Se acabó el tiempo!  0 estrellas\nPuntaje: {Score}\nErrores: {errors}";
        LockInteractions();
        UpdateUI();
        Report();
        Debug.Log($"Derrota: {Score} puntos");
    }

    // Al terminar, nada más se puede agarrar
    void LockInteractions() { SetInteractions(false); }

    void SetInteractions(bool on)
    {
        foreach (var g in FindObjectsByType<XRGrabInteractable>())
            g.enabled = on;
    }

    // ---------- Multijugador ----------
    void ReportProgress()
    {
        if (Time.time < nextReport) return;
        nextReport = Time.time + 0.25f;
        Report();
    }

    void Report()
    {
        if (PlayerScore.Local != null)
            PlayerScore.Local.Report(Score, lastCount, timeLeft, finished);
    }

    void UpdateRival()
    {
        if (rivalText == null || Time.time < nextRival) return;
        nextRival = Time.time + 0.3f;

        PlayerScore rival = null;
        foreach (var p in FindObjectsByType<PlayerScore>(FindObjectsSortMode.None))
            if (!p.HasStateAuthority) { rival = p; break; }

        rivalText.text = rival == null
            ? ""
            : $"Rival: {rival.Points} pts | {rival.Cables} cables | " +
              (rival.Finished ? "Terminó" : Format(rival.TimeLeft));
    }

    void UpdateUI()
    {
        timeText.text = $"Tiempo: {Format(timeLeft)}";
        timeText.color = timeLeft < 15f ? Color.red : Color.white;
        scoreText.text = $"Puntos: {Score}";
    }

    static string Format(float t)
    {
        int s = Mathf.CeilToInt(Mathf.Max(0f, t));
        return $"{s / 60:00}:{s % 60:00}";
    }
}