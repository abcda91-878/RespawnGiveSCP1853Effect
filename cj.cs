using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.Handlers;
using Exiled.Events.EventArgs;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

namespace RespawnGiveSCP1853Effect
{
    // 简单配置类（可扩展）
    public sealed class Config : Exiled.API.Interfaces.IConfig
    {
        public bool IsEnabled { get; set; } = true;
        // 自定义效果标识
        public string EffectId { get; set; } = "SCP1853";
        public bool Debug { get; set; } = false;
    }

    public class RespawnGiveSCP1853Effect : Plugin<Config>
    {
        // 记录哪些玩家当前拥有 SCP-1853 效果（按 UserId）
        private readonly HashSet<string> active = new HashSet<string>();

        public override string Name => "RespawnGiveSCP1853Effect";
        public override string Author => "GitHub Copilot";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(5, 0, 0);

        public override void OnEnabled()
        {
            // 部分 EXILED 版本中没有 Server.RespawningTeam 事件，改为在玩家生成时检查并赋予效果
            Exiled.Events.Handlers.Player.Spawned += OnPlayerSpawned;
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;
            Exiled.API.Features.Log.Info("RespawnGiveSCP1853Effect 已启用");
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnPlayerSpawned;
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
            active.Clear();
            Exiled.API.Features.Log.Info("RespawnGiveSCP1853Effect 已禁用");
        }




        // 玩家实际生成时，如果在待赋予列表中则赋予自定义“效果”（存在于本插件的跟踪中），直到死亡移除
        private void OnPlayerSpawned(SpawnedEventArgs ev)
        {
            try
            {
                var player = ev.Player;
                if (player == null) return;

                // 仅对 NTF/MTF/Chaos 生效：通过 Role.Type.ToString() 字符串检查以避免不同 EXILED/PlayerRoles 版本中枚举成员名差异
                var roleName = player.Role.Type.ToString().ToLowerInvariant();
                if (roleName.Contains("ntf") || roleName.Contains("mtf") || roleName.Contains("chaos"))
                {
                    if (active.Add(player.UserId))
                    {
                        player.Broadcast(5, $"已赋予效果: {Config.EffectId} （直到死亡移除）");
                        Exiled.API.Features.Log.Debug($"RespawnGiveSCP1853Effect: 给玩家 {player.UserId} 赋予 {Config.EffectId}");
                    }
                }
            }
            catch (Exception ex)
            {
                Exiled.API.Features.Log.Error($"RespawnGiveSCP1853Effect: OnPlayerSpawned 异常: {ex}");
            }
        }

        // 死亡时移除效果标记
        private void OnPlayerDied(DiedEventArgs ev)
        {
            try
            {
                var userId = ev.Player?.UserId;
                if (userId == null) return;

                if (active.Remove(userId))
                    Exiled.API.Features.Log.Debug($"RespawnGiveSCP1853Effect: 移除玩家 {userId} 的 {Config.EffectId} 效果（死亡）");
            }
            catch (Exception ex)
            {
                Exiled.API.Features.Log.Error($"RespawnGiveSCP1853Effect: OnPlayerDied 异常: {ex}");
            }
        }

        // 公开查询方法，供其他插件检查某玩家是否拥有该效果
        public bool HasEffect(string userId)
        {
            return active.Contains(userId);
        }
    }
}
