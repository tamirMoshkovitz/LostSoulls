using System;
using Game_Flow.ImpactObjects.Scripts.Types;
using Game_Flow.ImpactObjects.Scripts.UnityMonoSOScripts;

namespace Game_Flow.ImpactObjects.Scripts.Decorator_Interface
{
    public enum ImpactObjectTypes
    {
        StraightAndBackwards = 0,
        RightAndLeft = 1,
        Soul = 2
    }
    public static class ImpactObjectFactory
    {
        public static IImpactObject CreateImpactObject(ImpactObjectTypes type, IImpactObject inner, MonoImpactObject mono,ImpactObjectStats stats)
        {
            return type switch
            {
                ImpactObjectTypes.StraightAndBackwards => new StraightAndBackwardImpactObject(inner,mono,stats),
                ImpactObjectTypes.RightAndLeft => new RightAndLeftImpactObject(inner,mono,stats),
                ImpactObjectTypes.Soul => new SoulImpactObject(inner,mono,stats),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}