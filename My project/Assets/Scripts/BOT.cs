using UnityEngine;

public class BOT : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100f;
    private float currHealth;

    void Awake()
    {
        currHealth = maxHealth;     
    }

    public void TakeDamage(float damage)
    {
        currHealth -= damage;
        Debug.Log(transform.name + " HP " + currHealth);
    }
}
