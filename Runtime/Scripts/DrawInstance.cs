using UnityEngine;

namespace DebugDrawer
{
    /// <summary>
    /// Base class for all drawable instances
    /// </summary>
    internal class DrawInstance : IPoolable
    {
        internal Color Color;
        internal bool BeenDrawn;
        protected float ExpirationTime;
        internal uint DrawLayers;

        internal void SetDuration(float duration)
        {
            ExpirationTime = Time.time + duration;
            BeenDrawn = false;
        }

        internal virtual bool IsExpired()
        {
            return (Time.time > ExpirationTime && BeenDrawn);
        }

        public virtual void Reset()
        {
            BeenDrawn = false;
            Color = default;
            ExpirationTime = 0f;
            DrawLayers = 0;
        }
    }

    internal interface IPoolable
    {
        void Reset();
    }

    /// <summary>
    /// Specialized instance for meshes
    /// </summary>
    internal class DrawMeshInstance : DrawInstance
    {
        internal Matrix4x4 Transform;
        internal float length;

        public override void Reset()
        {
            base.Reset();
            Transform = Matrix4x4.identity;
            length = 0.2f;
        }
    }
}