﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ITD.Content.Projectiles;
using Terraria.Map;
using ITD.Content.NPCs.Bosses;
using ITD.Utilities;
namespace ITD.Content.Projectiles.Hostile
{
    //TODO: MUST BE RE-WRITTEN ENTIRELY, ELSE REMOVE BEFORE RELEASE
    public class CosmicRay : Deathray
    {
        public override string Texture => "ITD/Content/Projectiles/Hostile/CosmicRay";
        public CosmicRay() : base(120) { }
        private Vector2 spawnPos;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void AI()
        {
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            /*if (Main.npc[(int)Projectile.ai[1]].active && Main.npc[(int)Projectile.ai[1]].type == ModContent.\1Type<\2>\(\))
            {
                Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center;
            }
            else
            {
                Projectile.Kill();
                return;
            }*/
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            NPC CosJel = Main.npc[(int)Projectile.ai[1]];
            Player player = Main.player[CosJel.target];
            player.GetITDPlayer().Screenshake = 20;
            if (CosJel.active && CosJel.type == ModContent.NPCType<CosmicJellyfish>())
            {
                if (CosJel != null && !CosJel.dontTakeDamage)
                {
                    CosJel.velocity =  -(Vector2.Normalize(new Vector2(player.Center.X, player.Center.Y) - new Vector2(CosJel.Center.X, CosJel.Center.Y))) * 2f;
                    Projectile.Center = CosJel.Center;
                }
                else
                {
                    Projectile.Kill();
                    return;
                }
            }
            else
            {
                Projectile.Kill();
            }
            if (Projectile.localAI[0] == 0f)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/Zombie_104") { Volume = 0.5f }, Projectile.Center);
                spawnPos = Projectile.Center;
            }
            
            if (Main.expertMode||Main.masterMode)
            {
                Projectile.velocity = Projectile.velocity.ToRotation().AngleLerp(CosJel.DirectionTo(Main.player[CosJel.target].Center + Main.player[CosJel.target].velocity * 6).ToRotation(), 0.01f).ToRotationVector2();
                Projectile.rotation = Projectile.velocity.ToRotation() - (float)Math.PI / 6;
            }
            float num801 = 2f;
            if (Main.expertMode)
            {
                num801 = 3f;
            }
            else if (Main.masterMode)
            {
                num801 = 5f;
            }
            Projectile.localAI[0] += 1f;

            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * num801 * 6f;
            if (Projectile.scale > num801)
            {
                Projectile.scale = num801;
            }

            if (Projectile.localAI[0] > maxTime / 2 && Projectile.scale < num801)
            {
                if (Projectile.ai[0] > 0)
                {
                    Projectile.ai[0] = 0;
                }
            }

            //float num804 = Projectile.velocity.ToRotation();
            //num804 += Projectile.ai[0];
            //Projectile.rotation = num804 - 1.57079637f;
            //float num804 = Main.npc[(int)Projectile.ai[1]].ai[3] - 1.57079637f;
            //if (Projectile.ai[0] != 0f) num804 -= (float)Math.PI;
            //Projectile.rotation = num804;
            //num804 += 1.57079637f;
            //Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = (float)Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            //Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 3000f, array3);
            for (int i = 0; i < array3.Length; i++)
                array3[i] = 3000f;
            float num807 = 0f;
            int num3;
            for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
            {
                num807 += array3[num808];
                num3 = num808;
            }
            num807 /= num805;
            float amount = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num807, amount);
            Vector2 vector79 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
            for (int num809 = 0; num809 < 2; num809 = num3 + 1)
            {
                float num810 = Projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * 1.57079637f;
                float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector80 = new Vector2((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
                int num812 = Dust.NewDust(vector79, 0, 0, 244, vector80.X, vector80.Y, 0, default(Color), 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
                num3 = num809;
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 value29 = Projectile.velocity.RotatedBy(1.5707963705062866, default(Vector2)) * ((float)Main.rand.NextDouble() - 0.5f) * (float)Projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, 244, 0f, 0f, 100, default(Color), 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

            Projectile.position -= Projectile.velocity;
            Projectile.rotation = Projectile.velocity.ToRotation() - 1.57079637f;
        }
    }
}