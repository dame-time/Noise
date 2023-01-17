using UnityEngine;

namespace Noise.Generators
{
    public abstract class NoiseType : ScriptableObject
    {
        public virtual void Initialize() { }
    }
}
