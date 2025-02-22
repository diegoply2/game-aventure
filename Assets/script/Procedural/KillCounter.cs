using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KillCounter : MonoBehaviour
{
    public static KillCounter instance;  // Singleton pour accéder facilement au compteur
    public TMP_Text killCounterText;  // Référence au texte UI
    private int killCount = 0;  

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateKillCounter();
    }

    public void AddKill()
    {
        killCount++;
        UpdateKillCounter();
    }

    void UpdateKillCounter()
    {
        if (killCounterText != null)
            killCounterText.text = "Kills: " + killCount;
    }
}
