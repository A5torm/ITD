﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ITD.Content.Projectiles.Friendly.Melee.Snaptraps.Extra;
using Terraria.Localization;

namespace ITD.Content.Projectiles.Friendly.Melee.Snaptraps
{
    public class MrSnapkinsProjectile : ITDSnaptrap
    {
        public static LocalizedText OneTimeLatchMessage { get; private set; }

        int constantEffectFrames = 80;
        int constantEffectTimer = 0;
        public override void SetSnaptrapProperties()
        {
            OneTimeLatchMessage = Language.GetOrRegister(Mod.GetLocalizationKey($"Projectiles.{nameof(MrSnapkinsProjectile)}.OneTimeLatchMessage"));
            ShootRange = 16f * 14f;
            RetractAccel = 1.5f;
            ExtraFlexibility = 16f * 2f;
            FramesBetweenHits = 22;
            MinDamage = 20;
            MaxDamage = 50;
            FullPowerHitsAmount = 5;
            WarningFrames = 60;
            ChompDust = DustID.Titanium;
        }

        private void LaunchBowties()
        {
            if (Main.myPlayer == myPlayer.whoAmI)
            {
                for (int i = 0; i < 8; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2((float)Math.Cos(MathHelper.PiOver4 * i) * 2f, (float)Math.Sin(MathHelper.PiOver4 * i) * 2f), ModContent.ProjectileType<SnapkinsBowtie>(), MinDamage, 0.1f, myPlayer.whoAmI);
                }
            }
        }
        public override void OneTimeLatchEffect()
        {
            SoundEngine.PlaySound(snaptrapMetal, Projectile.Center);
            AdvancedPopupRequest popupSettings = new AdvancedPopupRequest
            {
                Text = OneTimeLatchMessage.Value,
                Color = Color.DarkSlateGray,
                DurationInFrames = 60 * 2,
                Velocity = Projectile.velocity,
            };
            PopupText.NewText(popupSettings, Projectile.Center + new Vector2(0f, -50f));
            LaunchBowties();
        }

        public override void ConstantLatchEffect()
        {
            constantEffectTimer++;
            if (constantEffectTimer >= constantEffectFrames)
            {
                constantEffectTimer = 0;
                LaunchBowties();
            }
        }

        public override void PostAI()
        {
            Projectile.spriteDirection = -Math.Sign((myPlayer.Center - Projectile.Center).X);
        }
    }
}