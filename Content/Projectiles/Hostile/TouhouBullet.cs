﻿
using ITD.Content.NPCs.Bosses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteelSeries.GameSense;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ITD.Content.Projectiles.Hostile
{
    public class TouhouBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tofu");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;

        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC CosJel = Main.npc[(int)Projectile.ai[0]];
            if (CosJel.active && CosJel.type == ModContent.NPCType<CosmicJellyfish>())
            {
                Vector2 CorePos = new Vector2(CosJel.Center.X, CosJel.Center.Y - 80);

                if (Projectile.ai[1] != 2)
                    Projectile.velocity = (CorePos - Projectile.Center).SafeNormalize(Vector2.Zero) * 2f;
                else
                {
                    Projectile.velocity = (CorePos - Projectile.Center).SafeNormalize(Vector2.Zero) * 0.05f;
                }
            }
            else
            {
                Projectile.Kill();
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        public override bool? CanDamage()
        {
            if (Projectile.ai[1] == 2)
            {
                if (Projectile.ai[2] >= 30)
                    return true;
                else return false;
            }
            return true;
        }
        public override void AI()
        {

            NPC CosJel = Main.npc[(int)Projectile.ai[0]];
            if (CosJel.active && CosJel.type == ModContent.NPCType<CosmicJellyfish>())
            {
                Vector2 CorePos = new Vector2(CosJel.Center.X, CosJel.Center.Y - 100);

                if (Vector2.Distance(Projectile.Center, CorePos) < 30)
                {
                    if (Projectile.ai[1] == 2)
                    {
                        Projectile.Kill();
                        CosJel.localAI[0]++;
                    }
                    Projectile.Kill();
                }
            }
            if (Projectile.ai[1] == 2)
            {
                if (Projectile.ai[2]++ >= 30)
                {
                    Projectile.extraUpdates = 1;
                    Projectile.velocity *= 1.03f;
                }
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            Main.instance.DrawCacheProjsBehindNPCs.Add(index);

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D effectTexture = TextureAssets.Extra[98].Value;
            Vector2 effectOrigin = effectTexture.Size() / 2f;
            lightColor = Lighting.GetColor((int)player.Center.X / 16, (int)player.Center.Y / 16);

            Main.EntitySpriteDraw(effectTexture, Projectile.Center, new Rectangle?(Projectile.Hitbox), new Color(120, 184, 255, 50) * 0.05f * Projectile.timeLeft, Projectile.rotation, effectOrigin, Projectile.scale, SpriteEffects.None, 0f);
            return true;
        }
    }
}

