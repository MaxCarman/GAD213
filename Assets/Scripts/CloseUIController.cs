using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CloseUIController : MonoBehaviour
{
    //Delcare variables
    public GameObject playerOwner; //The player object that 'owns' this ui. The ui follows this reference, and is declared in the instatiation.
    public Image staminaMarker;
    public TextMeshProUGUI staminaText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Teleport to playerOwner
        if (playerOwner != null)
        {
            transform.position = playerOwner.transform.position;
        }
    }
}
