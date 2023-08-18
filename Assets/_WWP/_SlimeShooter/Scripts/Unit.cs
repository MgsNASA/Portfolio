using System;

namespace WWP.Game
{
    public interface Unit
    {
        public event Action<Unit, bool> Died;
        public MyTransform Transform { get; }
        public void TakeHit(HitConfig hitConfig);
    }
}
