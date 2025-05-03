using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private SwordBack swordBack;

    private void Start()
    {
        if (swordBack != null)
        {
            swordBack.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (swordBack != null)
            {
                swordBack.ActivateSwordBack();
            }

            Destroy(gameObject);
        }
    }
}
