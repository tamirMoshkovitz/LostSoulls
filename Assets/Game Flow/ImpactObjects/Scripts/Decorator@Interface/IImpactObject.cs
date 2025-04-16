namespace Game_Flow.ImpactObjects.Scripts.Decorator_Interface
{
    public interface IImpactObject
    {
        public void Impact(UnityEngine.Vector3 direction);
        public void DrawGizmos();
    }
}