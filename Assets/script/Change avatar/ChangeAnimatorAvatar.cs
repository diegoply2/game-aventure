using UnityEngine;

public class ChangeAnimatorAvatar : MonoBehaviour
{
    public Avatar newAvatar; // Glisse ton nouvel Avatar ici

    void Start()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null && newAvatar != null)
        {
            animator.avatar = newAvatar;
            Debug.Log("✅ Nouvel avatar assigné !");
        }
        else
        {
            Debug.LogError("❌ Impossible d'assigner l'avatar !");
        }
    }
}
