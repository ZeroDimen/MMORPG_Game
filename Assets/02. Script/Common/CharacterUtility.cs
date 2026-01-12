using UnityEngine;

public static class CharacterUtility
{
    public static float GetDistanceToGround(Vector3 position, LayerMask layerMask, float maxDistance)
    {
        if (Physics.Raycast(position,
                Vector3.down, out RaycastHit hit,
                maxDistance, layerMask))
        {
            return hit.distance;
        }
        else
        {
            return maxDistance;
        }
    }
}