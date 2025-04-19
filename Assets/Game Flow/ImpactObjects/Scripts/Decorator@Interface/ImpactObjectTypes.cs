using System;
using Game_Flow.ImpactObjects.Scripts.Types;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;

namespace Game_Flow.ImpactObjects.Scripts.Decorator_Interface
{
    public enum ImpactObjectTypes
    {
        Straight = 0,
        Backwards = 1,
        Right = 2,
        Left = 3,
        Soul = 4
    }
    public static class ImpactObjectFactory
    {
        public static IImpactObject CreateImpactObject(ImpactObjectTypes type, IImpactObject inner, MonoImpactObject mono,ImpactObjectStats stats)
        {
            return type switch
            {
                ImpactObjectTypes.Straight => new StraightImpactObject(inner,mono,stats),
                ImpactObjectTypes.Backwards => new BackwardImpactObject(inner,mono,stats),
                ImpactObjectTypes.Right => new RightImpactObject(inner,mono,stats),
                ImpactObjectTypes.Left => new LeftImpactObject(inner,mono,stats),
                ImpactObjectTypes.Soul => new SoulImpactObject(inner,mono,stats),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}