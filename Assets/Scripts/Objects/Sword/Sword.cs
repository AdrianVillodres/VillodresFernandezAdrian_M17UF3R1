using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private SwordBack swordBack;
    private MainCharacter character;

    private void Start()
    {
        character = FindAnyObjectByType<MainCharacter>();
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

            character.objectPicked = true;
            Destroy(gameObject);
        }
    }
}
