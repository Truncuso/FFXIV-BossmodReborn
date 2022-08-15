﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BossMod.RealmReborn.Trial.T02TitanN
{
    public enum OID : uint
    {
        Boss = 0xF6, // x1
        Helper = 0x1B2, // x4
        TitansHeart = 0x5E3, // Unknown type, spawn during fight
        GraniteGaol = 0x5A4, // spawn during fight
    };

    public enum AID : uint
    {
        AutoAttack = 872, // Boss->player, no cast
        Tumult = 642, // Boss->self, no cast, raidwide
        RockBuster = 641, // Boss->self, no cast, range 11.25 ?-degree cone cleave
        Geocrush = 651, // Boss->self, 4.5s cast, range 27 aoe with ? falloff
        Landslide = 650, // Boss->self, 3.0s cast, range 40.25 width 6 rect aoe with knockback 15
        RockThrow = 645, // Boss->player, no cast, visual for granite gaol spawn
        GraniteSepulchre = 28799, // GraniteGaol->self, 15.0s cast, oneshot target if gaol not killed
        EarthenFury = 652, // Boss->self, no cast, wipe if heart not killed, otherwise just a raidwide
        WeightOfTheLand = 644, // Boss->self, 3.0s cast, visual
        WeightOfTheLandAOE = 973, // Helper->location, 3.5s cast, range 6 puddle
    };

    class Hints : BossComponent
    {
        private DateTime _heartSpawn;

        public override void AddGlobalHints(BossModule module, GlobalHints hints)
        {
            var nail = module.Enemies(OID.TitansHeart).FirstOrDefault();
            if (_heartSpawn == new DateTime() && nail != null && nail.IsTargetable)
            {
                _heartSpawn = module.WorldState.CurrentTime;
            }
            if (_heartSpawn != new DateTime() && nail != null && nail.IsTargetable && !nail.IsDead)
            {
                hints.Add($"Heart enrage in: {Math.Max(62.6f - (module.WorldState.CurrentTime - _heartSpawn).TotalSeconds, 0.0f):f1}s");
            }
        }
    }

    class RockBuster : Components.Cleave
    {
        public RockBuster() : base(ActionID.MakeSpell(AID.RockBuster), new AOEShapeCone(11.25f, 60.Degrees())) { } // TODO: verify angle
    }

    class Geocrush : Components.SelfTargetedAOEs
    {
        public Geocrush() : base(ActionID.MakeSpell(AID.Geocrush), new AOEShapeCircle(18)) { } // TODO: verify falloff
    }

    class Landslide : Components.SelfTargetedAOEs
    {
        public Landslide() : base(ActionID.MakeSpell(AID.Landslide), new AOEShapeRect(40, 3)) { }
    }

    class WeightOfTheLand : Components.LocationTargetedAOEs
    {
        public WeightOfTheLand() : base(ActionID.MakeSpell(AID.WeightOfTheLandAOE), 6) { }
    }

    class T02TitanNStates : StateMachineBuilder
    {
        public T02TitanNStates(BossModule module) : base(module)
        {
            TrivialPhase()
                .ActivateOnEnter<Hints>()
                .ActivateOnEnter<RockBuster>()
                .ActivateOnEnter<Geocrush>()
                .ActivateOnEnter<Landslide>()
                .ActivateOnEnter<WeightOfTheLand>();
        }
    }

    public class T02TitanN : BossModule
    {
        private List<Actor> _gaol;
        private List<Actor> _heart;

        public T02TitanN(WorldState ws, Actor primary) : base(ws, primary, new ArenaBoundsCircle(new(-0, 0), 20)) // note: initial area is size 25, but it becomes smaller at 75%
        {
            _gaol = Enemies(OID.GraniteGaol);
            _heart = Enemies(OID.TitansHeart);
        }

        public override bool FillTargets(BossTargets targets, int pcSlot)
        {
            if (!targets.AddIfValid(_gaol) && !targets.AddIfValid(_heart))
                targets.AddIfValid(PrimaryActor);
            return true;
        }
    }
}