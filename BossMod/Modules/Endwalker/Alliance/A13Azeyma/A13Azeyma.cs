﻿namespace BossMod.Endwalker.Alliance.A13Azeyma;

class WardensWarmth(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.WardensWarmthAOE), 6);
class FleetingSpark(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.FleetingSpark), new AOEShapeCone(60, 135.Degrees()));
class SolarFold(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.SolarFoldAOE), new AOEShapeCross(30, 5));
class Sunbeam(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Sunbeam), new AOEShapeCircle(9), 14);
class SublimeSunset(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.SublimeSunsetAOE), 40); // TODO: check falloff

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "veyn, Malediktus", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 866, NameID = 11277, SortOrder = 5)]
public class A13Azeyma(WorldState ws, Actor primary) : BossModule(ws, primary, NormalCenter, NormalBounds)
{
    public static readonly WPos NormalCenter = new(-750, -750);
    public static readonly WPos TriangleCenter = new(-750, -753.5f);
    public static readonly ArenaBoundsCircle NormalBounds = new(30);
    public static readonly ArenaBoundsComplex TriangleBounds = new([new TriangleE(TriangleCenter, 12)]);
}
