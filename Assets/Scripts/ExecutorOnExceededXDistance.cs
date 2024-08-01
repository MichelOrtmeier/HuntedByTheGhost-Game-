using UnityEngine;

public abstract class ExecutorOnExceededXDistance : MonoBehaviour
{
    [SerializeField] Transform referenceTransform;
    [SerializeField] int xDistanceExceededByPlayerBetweenKeySpawns = 50;
    [SerializeField] int xDistanceVariation = 10;

    Vector3 referencePosition;
    int xDistanceBeforeNextExecution;

    protected virtual void Start()
    {
        referencePosition = referenceTransform.position;
        xDistanceBeforeNextExecution = GetRandomXDistanceBeforeNextSpawn();
    }

    private int GetRandomXDistanceBeforeNextSpawn()
    {
        return Random.Range(xDistanceExceededByPlayerBetweenKeySpawns - xDistanceVariation, xDistanceExceededByPlayerBetweenKeySpawns + xDistanceVariation);
    }

    protected virtual void Update()
    {
        if (XDistanceIsExceeded())
        {
            OnXDistanceIsExceeded();
            referencePosition = referenceTransform.position;
        }
    }

    private bool XDistanceIsExceeded()
    {
        return referenceTransform.position.x - referencePosition.x > xDistanceBeforeNextExecution;
    }

    protected abstract void OnXDistanceIsExceeded();
}