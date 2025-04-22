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
        Soul = 4,
        OneBlockGrid = 5,
        TwoBlockHorizontalGrid = 6,
        TwoBlockVerticalGrid = 7,
        ThreeBlockHorizontalGrid = 8,
        ThreeBlockVerticalGrid = 9,
        FourBlocksSquareGrid = 10
    }
    public static class ImpactObjectFactory
    {
        public static IImpactObject CreateImpactObject(ImpactObjectTypes type, IImpactObject inner, MonoImpactObject mono,ImpactObjectStats stats, GridVisualizer gridVisualizer)
        {
            return type switch
            {
                ImpactObjectTypes.Straight => new StraightImpactObject(inner,mono,stats),
                ImpactObjectTypes.Backwards => new BackwardImpactObject(inner,mono,stats),
                ImpactObjectTypes.Right => new RightImpactObject(inner,mono,stats),
                ImpactObjectTypes.Left => new LeftImpactObject(inner,mono,stats),
                ImpactObjectTypes.Soul => new SoulImpactObject(inner,mono,stats),
                ImpactObjectTypes.OneBlockGrid => new OneBlockGridImpactObject(inner,mono,stats,gridVisualizer),
                ImpactObjectTypes.TwoBlockHorizontalGrid => new TwoBlockHorizontalGridImpactObject(inner,mono,stats,gridVisualizer),
                ImpactObjectTypes.TwoBlockVerticalGrid => new TwoBlockVerticalGridImpactObject(inner,mono,stats,gridVisualizer),
                ImpactObjectTypes.ThreeBlockHorizontalGrid => new ThreeBlockHorizontalGridImpactObject(inner,mono,stats,gridVisualizer),
                ImpactObjectTypes.ThreeBlockVerticalGrid => new ThreeBlockVerticalGridImpactObject(inner,mono,stats,gridVisualizer),
                ImpactObjectTypes.FourBlocksSquareGrid => new FourBlocksSquareGridImpactObject(inner,mono,stats,gridVisualizer),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}