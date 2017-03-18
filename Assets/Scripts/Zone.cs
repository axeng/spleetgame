using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script
{
    class Zone
    {
        private Vector2 vec1;   // Point en bas a gauche de la zone
        private Vector2 vec2;   // Point en haut a droite de la zone

        public Zone(Vector2 vector1, Vector2 vector2)
        {
            this.vec1 = vector1;
            this.vec2 = vector2;
        }

        // check if the vector3 position is inside the zone
        public bool AreBelong(Vector3 position) 
        {
            return position.x > vec1.x && position.z > vec1.y && position.x < vec2.x && position.z < vec2.y;
        }
    }
}
