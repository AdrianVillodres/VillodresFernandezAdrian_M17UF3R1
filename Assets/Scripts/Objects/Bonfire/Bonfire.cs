using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonfire : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MainCharacter character = other.GetComponent<MainCharacter>();
            if (character != null)
            {
                bool enemyKilled = GameObject.FindWithTag("Enemy") == null;
                GameSaveManager.SaveGame(character.transform.position, character.transform.rotation, character.objectPicked, enemyKilled);
                Debug.Log("Partida guardada al tocar el punto de guardado.");
            }
        }
    }
}
