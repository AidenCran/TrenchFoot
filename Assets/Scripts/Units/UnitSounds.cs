using UnityEngine;

namespace Units
{
    public static class UnitSounds
    {
        public static readonly AudioClip ShootSound = Resources.Load<AudioClip>("SFX/Unit Rifle Single Shot");
        public static readonly AudioClip DieSound = Resources.Load<AudioClip>("SFX/Unit Rifle Death");
    }
}