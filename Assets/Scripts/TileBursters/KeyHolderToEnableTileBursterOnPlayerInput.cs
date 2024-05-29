using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Collider2D))]
public class KeyHolderToEnableTileBursterOnPlayerInput : MonoBehaviour
{
    [SerializeField] TileAroundObjectBursterToEnable tileBurster;
    [SerializeField] GameObject player;
    [SerializeField] int keys = 0;

    public void OnPlayerBurstsTileBlock(InputAction.CallbackContext context)
    {
        if (context.performed && keys > 0)
        {
            tileBurster.Enable();
            keys--;
        }
        else if(!context.performed)
        {
            tileBurster.Disable();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == player)
        {
            keys++;
        }
    }
}
