﻿using ITD.Systems;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ITD.Content.Projectiles.Friendly.Misc;

namespace ITD.Content.Items.Favors.Prehardmode
{
    public class BloodPact : Favor
    {
        public override int FavorFatigueTime => 0;
        public override bool IsCursedFavor => true;

        private int lifeConsumed;
        public override void SetStaticDefaults()
        {

        }
        public override void SetFavorDefaults()
        {
            Item.width = Item.height = 32;
        }
        public override string GetBarStyle()
        {
            return "BloodPactBarStyle";
        }
        public override string GetChargeSound()
        {
            return "SoapBubbles";
        }

        public override bool UseFavor(Player player)
        {
            return true;
        }

        public override void UpdateFavor(Player player, bool hideVisual)
        {
            int bloodPactSpirit = ModContent.ProjectileType<BloodPactSpirit>();
            if (player.ownedProjectileCounts[bloodPactSpirit] > 0)
                return;
            if (FavorPlayer.UseFavorKey.Current)
            {
                player.GetModPlayer<FavorPlayer>().bloodPact = true;
                lifeConsumed += 1;
            }
            if (FavorPlayer.UseFavorKey.JustReleased)
            {
                // I'm using Projectile.ai[0] here in the newProjectile call as timeLeft, if you wanna change the amount of time relative to lifeConsumed the projectile should exist.
                Projectile.NewProjectile(Item.GetSource_FromThis(), player.Center, Vector2.Zero, bloodPactSpirit, lifeConsumed, 0f, player.whoAmI, lifeConsumed * 10);
                lifeConsumed = 0;
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = tooltips.First(x => x.Name == "Tooltip0");
            string hotkeyText = string.Format(line.Text, FavorPlayer.FavorKeybindString);
            line.Text = hotkeyText;
        }
    }
}