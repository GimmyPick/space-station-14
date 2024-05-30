﻿using Content.Server.GameTicking;
using Content.Shared._Sunrise.SunriseCCVars;
using Content.Shared.CombatMode.Pacification;
using Content.Shared.Mindshield.Components;
using Robust.Server.Player;
using Robust.Shared.Configuration;

namespace Content.Server._Sunrise.PeacefulRoundEnd;

public sealed class PeacefulRoundEndSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    private bool _isEnabled = false;

    public override void Initialize()
    {
        base.Initialize();
        _cfg.OnValueChanged(SunriseCCVars.PeacefulRoundEnd, v => _isEnabled = v, true);
        SubscribeLocalEvent<RoundEndTextAppendEvent>(OnRoundEnded);
    }

    private void OnRoundEnded(RoundEndTextAppendEvent ev)
    {
        if (!_isEnabled) return;
        foreach (var session in _playerManager.Sessions)
        {
            if (!session.AttachedEntity.HasValue) continue;

            var entityId = session.AttachedEntity.Value;
            if (HasComp<MindShieldComponent>(entityId))
            {
                continue;
            }
            EnsureComp<PacifiedComponent>(entityId);
        }
    }
}
