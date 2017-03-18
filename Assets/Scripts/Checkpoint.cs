using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script
{
    class Checkpoint
    {
        private Zone zone;
        private Vector3 respawn;

        public Checkpoint(Vector2 vector1, Vector2 vector2, Vector3 r)
        {
            this.zone = new Zone(vector1, vector2);
            this.respawn = r;
        }

        // Renvoi appartenance qu vec a la zone
        public bool CheckInZone(Vector3 vec)
        {
            return zone.AreBelong(vec);
        }

        public Vector3 GetRespawn()
        {
            return respawn;
        }
    }
}
