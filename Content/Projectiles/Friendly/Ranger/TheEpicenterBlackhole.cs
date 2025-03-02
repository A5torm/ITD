﻿using Terraria;
using System;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;

using ITD.Content.Dusts;
using Terraria.DataStructures;
using System.Threading;
using ITD.Content.Items.Weapons.Summoner;
using ITD.Utilities;
using ITD.Content.Items.Weapons.Ranger;
using ITD.PrimitiveDrawing;
using Terraria.Graphics.Shaders;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;
using Terraria.Graphics;
using ITD.Particles;
using ITD.Particles.Misc;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ITD.Content.Projectiles.Friendly.Ranger
{

    public class TheEpicenterBlackhole : ModProjectile
    {
        public MiscShaderData Shader = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile").UseProjectionMatrix(true);

        public VertexStrip TrailStrip = new VertexStrip();
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 120; Projectile.height = 120;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1800;
            Shader.UseImage0("Images/Extra_" + 192);
            Shader.UseImage1("Images/Extra_" + 194);
            Shader.UseImage2("Images/Extra_" + 190);
            Shader.UseSaturation(-4f);
            Shader.UseOpacity(2f);
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        
        private NPC HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }
        Vector2 spawnMousePos;
        int CurrentBulletCount = 0;
        int totalDamage = 0;
        readonly int MaxBulletCount = 100;
        readonly int Time = 60;
        int TimeBeforeRetract;
        bool Retracting;
        public override void OnSpawn(IEntitySource source)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                SoundStyle blackholeSpawn = new SoundStyle("ITD/Content/Sounds/NPCSounds/Bosses/CosjelBlackholeStart") with
                {
                    Volume = 1.25f,
                    PitchVariance = 0.1f,
                    MaxInstances = 1,
                    SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
                };
                SoundEngine.PlaySound(blackholeSpawn, player.Center);
                spawnMousePos = Main.MouseWorld;
            }
        }
        public override bool? CanDamage()
        {
            return false;
        }

        Player player => Main.player[Projectile.owner];
        public override void AI()
        {
            Projectile.spriteDirection = (Projectile.velocity.X < 0).ToDirectionInt();
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 * 2;
            else
                Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.timeLeft = 5;
            if (CurrentBulletCount < 100)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile other = Main.projectile[i];
                    if (i != Projectile.whoAmI &&
                        other.active &&
                        other.GetGlobalProjectile<ITDInstancedGlobalProjectile>().isFromTheEpicenter == true
                        &&
                        other.owner == player.whoAmI
                        && Math.Abs(Projectile.Center.X - other.position.X)
                        + Math.Abs(Projectile.Center.Y - other.position.Y) < 90)
                    {
                        ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.Excalibur, new ParticleOrchestraSettings
                        {
                            PositionInWorld = Projectile.Center,
                        }, Projectile.whoAmI);
                        other.active = false;
                        other.netUpdate = true;
                        Projectile.localAI[1] += 10f;
                        CurrentBulletCount++;
                    }
                }
            }
            if (Projectile.localAI[1] > 0f)
                Projectile.localAI[1] -= 5f;
            if (Projectile.localAI[1] > 100f)
                Projectile.localAI[1] = 100f;
            Projectile.rotation += 0.05f;
            if (!Retracting)
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, spawnMousePos, 0.2f);
            }
            else
            {
                if (Projectile.Distance(player.Center) <= 30)
                {
                    Projectile.Kill();
                }
                Projectile.Center = Vector2.Lerp(Projectile.Center, player.Center, 0.2f);
            }

            if (Projectile.Distance(spawnMousePos) >= 40)
            {
            }
            else
            {
                if (TimeBeforeRetract++ >= 120)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        if (Main.mouseRight ||
                            player.HeldItem.ModItem is not TheEpicenter && TimeBeforeRetract >= 600 ||
                            TimeBeforeRetract >= 2000
                            )
                        {
                            Retracting = true;
                        }
                    }
                }
                Projectile.velocity *= 0.95f;
                HomingTarget ??= Projectile.FindClosestNPC(1000);

                if (HomingTarget == null)
                    return;
                if (CurrentBulletCount > 0)
                {
                    if (Projectile.localAI[0]++ % 12 == 0)
                    {
                        CurrentBulletCount--;
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center,
                                (HomingTarget.Center - Projectile.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.ToRadians(4)) * 20f,
                                ModContent.ProjectileType<TheEpicenterSpark>(), (int)(Projectile.damage * 1.25f), Projectile.knockBack, Projectile.owner, 1);
                            proj.tileCollide = false;
                        }
                        Projectile.localAI[1] += 10f;
                        for (int i = 0; i < 10; i++)
                        {
                            int dust = Dust.NewDust(Projectile.Center, 1, 1, ModContent.DustType<StarlitDust>(), 0f, 0f, 0, default, 1.5f);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].velocity *= 2f;
                        }
                    }
                }
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        private Color StripColors(float progressOnStrip)
        {
            Color result = Color.Lerp(Color.Black, Color.White, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip, false));
            result.A /= 2;
            return result * Projectile.Opacity;
        }
        private float StripWidth(float progressOnStrip)
        {
            return 120f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D effectTexture = TextureAssets.Extra[98].Value;
            Vector2 effectOrigin = effectTexture.Size() / 2f;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            float scaleX = 1f;
            float scaleY = 2.5f;
            Rectangle rectangle = texture.Frame(1, 1);
            Player player = Main.player[Projectile.owner];
            lightColor = Lighting.GetColor((int)player.Center.X / 16, (int)player.Center.Y / 16);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            if (Projectile.Distance(spawnMousePos) >=40)
            {
                Shader.Apply(null);
                TrailStrip.PrepareStrip(Projectile.oldPos, Projectile.oldRot, StripColors, StripWidth, Projectile.Size * 0.5f - Main.screenPosition, Projectile.oldPos.Length, true);
                TrailStrip.DrawTrail();
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
                Main.EntitySpriteDraw(effectTexture, drawPosition, null, Color.White, Projectile.velocity.ToRotation() + MathHelper.PiOver2, effectTexture.Size() / 2f, new Vector2(scaleX, scaleY), SpriteEffects.None, 0);
            }
            else
            {
                GameShaders.Misc["Blackhole"].UseImage0(TextureAssets.Extra[193]);
                GameShaders.Misc["Blackhole"].UseColor(new Color(133, 50, 88));
                GameShaders.Misc["Blackhole"].UseSecondaryColor(Color.Beige);
                GameShaders.Misc["Blackhole"].Apply();
                float scaleUp = 200 + Projectile.localAI[1];
                square.Draw(Projectile.Center - Main.screenPosition, size: new 
                    Vector2(scaleUp, scaleUp));
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            }
            return false;
        }
        private static SimpleSquare square = new SimpleSquare();

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = (Projectile.Center.X < target.Center.X).ToDirectionInt();
        }
    }
}