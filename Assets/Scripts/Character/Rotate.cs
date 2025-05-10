using System.Collections;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    private MainCharacter character;

    [SerializeField] private Transform model;

    Vector3[] directions = new Vector3[]
    {
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1),
        new Vector3(1, 0, 0),
        new Vector3(1, 0, -1),
        new Vector3(0, 0, -1),
        new Vector3(-1, 0, -1),
        new Vector3(-1, 0, 0),
        new Vector3(-1, 0, 1)
    };

    private void Awake()
    {
        character = GetComponent<MainCharacter>();
    }

    private void Update()
    {
        try
        {
            if (character.isDancing)
                return;
        }
        catch
        {
        }

        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if (moveDir != Vector3.zero)
        {
            moveDir.Normalize();
            RotateTo8Direction(moveDir);
        }
    }

    void RotateTo8Direction(Vector3 moveDir)
    {
        float maxDot = -Mathf.Infinity;
        Vector3 bestDir = Vector3.zero;

        foreach (Vector3 dir in directions)
        {
            float dot = Vector3.Dot(moveDir, dir.normalized);
            if (dot > maxDot)
            {
                maxDot = dot;
                bestDir = dir;
            }
        }

        Quaternion targetRotation = Quaternion.LookRotation(bestDir);
        model.rotation = targetRotation;
    }
}
