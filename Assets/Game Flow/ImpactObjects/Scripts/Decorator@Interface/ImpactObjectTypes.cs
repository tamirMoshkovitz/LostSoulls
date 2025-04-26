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
        FourBlocksSquareGrid = 10,
        MovingShader = 11,
        OpenClose = 12
    }
    public static class ImpactObjectFactory
    {
        public static IImpactObject CreateImpactObject(ImpactObjectTypes type, IImpactObject inner, MonoImpactObject mono,ImpactObjectStats stats, Grid grid)
        {
            return type switch
            {
                ImpactObjectTypes.Straight => new StraightImpactObject(inner,mono,stats),
                ImpactObjectTypes.Backwards => new BackwardImpactObject(inner,mono,stats),
                ImpactObjectTypes.Right => new RightImpactObject(inner,mono,stats),
                ImpactObjectTypes.Left => new LeftImpactObject(inner,mono,stats),
                ImpactObjectTypes.Soul => new SoulImpactObject(inner,mono,stats),
                ImpactObjectTypes.OneBlockGrid => new OneBlockGridImpactObject(inner,mono,stats,grid),
                ImpactObjectTypes.TwoBlockHorizontalGrid => new TwoBlockHorizontalGridImpactObject(inner,mono,stats,grid),
                ImpactObjectTypes.TwoBlockVerticalGrid => new TwoBlockVerticalGridImpactObject(inner,mono,stats,grid),
                ImpactObjectTypes.ThreeBlockHorizontalGrid => new ThreeBlockHorizontalGridImpactObject(inner,mono,stats,grid),
                ImpactObjectTypes.ThreeBlockVerticalGrid => new ThreeBlockVerticalGridImpactObject(inner,mono,stats,grid),
                ImpactObjectTypes.FourBlocksSquareGrid => new FourBlocksSquareGridImpactObject(inner,mono,stats,grid),
                ImpactObjectTypes.MovingShader => new MovingShaderImpactObject(inner,mono,stats),
                ImpactObjectTypes.OpenClose => new OpenCloseImpactObject(inner,mono,stats,false),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}