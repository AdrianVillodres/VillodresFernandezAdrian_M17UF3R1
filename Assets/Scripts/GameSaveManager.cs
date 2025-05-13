using UnityEngine;

public class GameSaveManager : MonoBehaviour
{
    public static void SaveGame(Vector3 playerPosition, Quaternion playerRotation, bool hasObject, bool enemyKilled)
    {
        PlayerPrefs.SetFloat("PlayerPosX", playerPosition.x);
        PlayerPrefs.SetFloat("PlayerPosY", playerPosition.y);
        PlayerPrefs.SetFloat("PlayerPosZ", playerPosition.z);

        PlayerPrefs.SetFloat("PlayerRotX", playerRotation.x);
        PlayerPrefs.SetFloat("PlayerRotY", playerRotation.y);
        PlayerPrefs.SetFloat("PlayerRotZ", playerRotation.z);
        PlayerPrefs.SetFloat("PlayerRotW", playerRotation.w);

        PlayerPrefs.SetInt("HasObject", hasObject ? 1 : 0);
        PlayerPrefs.SetInt("EnemyKilled", enemyKilled ? 1 : 0);

        PlayerPrefs.Save();
    }

    public static void LoadGame(out Vector3 playerPosition, out Quaternion playerRotation, out bool hasObject, out bool enemyKilled)
    {
        
        float x = PlayerPrefs.GetFloat("PlayerPosX", 0);
        float y = PlayerPrefs.GetFloat("PlayerPosY", 0);
        float z = PlayerPrefs.GetFloat("PlayerPosZ", 0);
        playerPosition = new Vector3(x, y, z);

        
        float rx = PlayerPrefs.GetFloat("PlayerRotX", 0);
        float ry = PlayerPrefs.GetFloat("PlayerRotY", 0);
        float rz = PlayerPrefs.GetFloat("PlayerRotZ", 0);
        float rw = PlayerPrefs.GetFloat("PlayerRotW", 1);
        playerRotation = new Quaternion(rx, ry, rz, rw);

        
        hasObject = PlayerPrefs.GetInt("HasObject", 0) == 1;
        enemyKilled = PlayerPrefs.GetInt("EnemyKilled", 0) == 1;
    }

    public static bool HasSavedData()
    {
        return PlayerPrefs.HasKey("PlayerPosX");
    }
}
