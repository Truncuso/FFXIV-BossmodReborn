﻿namespace BossMod.Endwalker.Quest.Endwalker;

class AkhMorn : Components.GenericBaitAway
{
    private DateTime _activation;

    public AkhMorn() : base(centerAtTarget: true) { }

    public override void AddGlobalHints(BossModule module, GlobalHints hints)
    {
        if (_activation != default)
            hints.Add($"Tankbuster x{NumExpectedCasts(module)}");
    }

    public override void AddAIHints(BossModule module, int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (_activation != default)
            hints.PredictedDamage.Add((new(1), _activation));
    }

    public override void OnCastStarted(BossModule module, Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.AkhMorn)
        {
            CurrentBaits.Add(new(module.PrimaryActor, module.Raid.Player()!, new AOEShapeCircle(4)));
            _activation = spell.NPCFinishAt;
        }
    }

    public override void OnCastFinished(BossModule module, Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.AkhMorn)
            ++NumCasts;
    }

    public override void OnEventCast(BossModule module, Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.AkhMornVisual)
        {
            ++NumCasts;
            if (NumCasts == NumExpectedCasts(module))
            {
                CurrentBaits.Clear();
                NumCasts = 0;
                _activation = default;
            }
        }
    }

    private int NumExpectedCasts(BossModule module) => module.PrimaryActor.IsDead ? 8 : 6;
}
