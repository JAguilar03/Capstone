using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script handles the player's melee attack functionality. It controls the activation and positioning 
// of the player's hitbox during the melee attack animation. The hitbox is enabled when the player starts 
// their melee attack and is moved to the appropriate position relative to the player. After the attack finishes, 
// the hitbox is disabled and moved out of position. This allows the player to hit enemies during the attack animation 
// while keeping the hitbox inactive at other times to avoid unintended collisions.

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
