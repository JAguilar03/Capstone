using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    public GameObject Player;
    private GameObject PlayerHitbox;

    void Start() {
        PlayerHitbox = Player.transform.GetChild(4).gameObject;
    }

    // Called on the attacking frames of Player Melee animation
    public void StartMelee() {
        // Toggle Enemy Attack Hitbox on 
        PlayerHitbox.SetActive(true);

        // move hitbox in positon
        PlayerHitbox.transform.localPosition = new Vector3(1f, 0, 0);
    }

    // Called on the frame after attacking frames
    public void EndMelee() {
        // Toggle Enemy Attack Hitbox off
        PlayerHitbox.SetActive(false);

        // move hitbox out of positon
        PlayerHitbox.transform.localPosition = new Vector3(-1f, 0, 0);
    }
}
