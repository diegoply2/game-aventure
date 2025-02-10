using UnityEngine;

public class TreeColliderAdder : MonoBehaviour
{
    public float sizeFactor = 2f; // Facteur pour ajuster le rayon du Collider

    void Start()
    {
        AddColliderToTree();
    }

    void AddColliderToTree()
    {
        // Vérifie si le CapsuleCollider existe déjà
        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CapsuleCollider>();
        }

        // Ajuste le rayon du CapsuleCollider en fonction de l'arbre
        float treeRadius = Mathf.Max(transform.localScale.x, transform.localScale.z); // Prendre la plus grande échelle de l'arbre
        collider.radius = treeRadius * sizeFactor;

        // Ajuste la hauteur du CapsuleCollider en fonction de l'échelle de l'arbre
        float treeHeight = transform.localScale.y;
        collider.height = treeHeight;

        // Centrer le collider à la hauteur de l'arbre
        collider.center = new Vector3(0, treeHeight / 2, 0); // Centré sur la hauteur de l'arbre

        // Assurer que le collider est bien configuré pour éviter que le joueur passe à travers
        collider.isTrigger = false;  // Vérifie que le collider n'est pas un trigger (permet les collisions physiques)
    }
}

