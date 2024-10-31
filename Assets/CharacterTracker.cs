using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Psychonaut
{
    public class CharacterTracker : MonoBehaviour
    {
        [SerializeField] PlayerController Player;
        public Transform Player_Location;
    void Update()
        {
            Player_Location = Player.transform;
        }
    }
}

