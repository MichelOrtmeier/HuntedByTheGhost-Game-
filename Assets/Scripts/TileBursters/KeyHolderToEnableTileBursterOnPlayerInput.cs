using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Collider2D))]
public class KeyHolderToEnableTileBursterOnPlayerInput : MonoBehaviour
{
    [SerializeField] TileAroundObjectBursterToEnable tileBurster;
    [SerializeField] GameObject player;
    [SerializeField] int keys = 0;
    [SerializeField] TMP_Text numberOfKeysVisualisation;
    [SerializeField] Canvas keyCanvas;

    private void Start()
    {
        keyCanvas.gameObject.SetActive(false);
    }

    public void OnPlayerBurstsTileBlock(InputAction.CallbackContext context)
    {
        if (context.performed && keys > 0)
        {
            tileBurster.Enable();
            DecreaseKeysByOne();
        }
        else if(!context.performed)
        {
            tileBurster.Disable();
        }
    }

    private void DecreaseKeysByOne()
    {
        keys--;
        ShowNumberOfKeys();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == player)
        {
            keys++;
            ShowNumberOfKeys();
        }
    }

    private void ShowNumberOfKeys()
    {
        if(!keyCanvas.gameObject.activeSelf)
            keyCanvas.gameObject.SetActive(true);
        numberOfKeysVisualisation.text = keys.ToString();
    }
}
