using ITD.Content.Buffs.MinionBuffs;
using ITD.Content.Projectiles.Hostile;
using ITD.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace ITD.Content.Projectiles.Friendly.Summoner
{
    public class IncendipedeMinionHead : ITDProjectile
    {
        public const int SpacingBetween = 5;

        public ref float SineTimer => ref Projectile.ai[0];
        public bool Wall;

        private NPC HomingTarget
        {
            get => Projectile.ai[1] == 0 ? null : Main.npc[(int)Projectile.ai[1] - 1];
            set => Projectile.ai[1] = value == null ? 0 : value.whoAmI + 1;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 10 * SpacingBetween;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.stepSpeed = 4f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead || !player.active)
                player.ClearBuff(ModContent.BuffType<IncendipedeMinionBuff>());
            if (player.HasBuff(ModContent.BuffType<IncendipedeMinionBuff>()))
                Projectile.timeLeft = 2;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<IncendipedeMinionTail>()] == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<IncendipedeMinionTail>(), Projectile.damage, Projectile.knockBack, player.whoAmI);

            if (Projectile.Distance(player.Center) > 1000f)
            {
                Projectile.Center = player.Center;
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
            }

            Wall = BigHitboxTiles.Points().Any(p =>
            {
                Tile t = Framing.GetTileSafely(p);
                return t.WallType != WallID.None || (t.HasUnactuatedTile && Main.tileSolid[t.TileType] && !Main.tileSolidTop[t.TileType]);
            });

            Projectile.tileCollide = !Wall;

            float maxDetectRadius = 800f;
            if (player.HasMinionAttackTargetNPC)
            {
                HomingTarget = Main.npc[player.MinionAttackTargetNPC];
            }
            else
            {
                HomingTarget ??= Projectile.FindClosestNPC(maxDetectRadius);
            }

            if (HomingTarget != null && (!HomingTarget.active || HomingTarget.life <= 0 || !HomingTarget.CanBeChasedBy()))
            {
                HomingTarget = null;
            }

            if (HomingTarget != null)
            {
                if (Wall)
                {
                    Vector2 attackOffset = new Vector2((float)Math.Cos(SineTimer * 0.04f) * 150f, (float)Math.Sin(SineTimer * 0.08f) * 80f);
                    WallMovement(HomingTarget.Center + attackOffset);
                }
                else
                {
                    Vector2 attackOffset = new Vector2((float)Math.Sin(SineTimer * 0.02f) * 200f, 0f);
                    GroundMovement(HomingTarget.Center + attackOffset);
                }

                if (Collision.CanHitLine(Projectile.Center, 1, 1, HomingTarget.Center, 1, 1))
                {
                    Projectile.ai[2]++;
                }
                else
                {
                    Projectile.ai[2] = 0;
                }

                if (Projectile.ai[2] > 120)
                {
                    if (Projectile.ai[2] % 4 == 0 && Main.myPlayer == Projectile.owner)
                    {
                        Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                        direction = direction.RotatedByRandom(MathHelper.ToRadians(10));
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction * 8, ModContent.ProjectileType<IncendipedeBreath>(), Projectile.originalDamage / 2, 0, Projectile.owner);
                        Main.projectile[p].friendly = true;
                        Main.projectile[p].hostile = false;
                    }
                    if (Projectile.ai[2] > 160)
                        Projectile.ai[2] = 0;
                }
            }
            else
            {
                Projectile.ai[2] = 0;

                if (Wall)
                {
                    Vector2 idleOffset = new Vector2((float)Math.Cos(SineTimer * 0.04f) * 150f, (float)Math.Sin(SineTimer * 0.08f) * 80f);
                    WallMovement(player.Center + idleOffset);
                }
                else
                {
                    Vector2 idleOffset = new Vector2((float)Math.Sin(SineTimer * 0.02f) * 200f, 0f);
                    GroundMovement(player.Center + idleOffset);
                }
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }
        }

        private void WallMovement(Vector2 targetPos)
        {
            Vector2 toTargetNorm = (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero);
            float speed = 2f;
            float sineAmplitude = 32f;
            float sineFrequency = 0.1f;
            Vector2 perpendicular = toTargetNorm.RotatedBy(Math.PI / 2d);
            float sineOffset = (float)Math.Sin(SineTimer * sineFrequency) * sineAmplitude;

            Projectile.velocity = toTargetNorm * speed + perpendicular * sineOffset / 16f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Math.Abs(Projectile.velocity.X) > 0.1f)
            {
                Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            }

            SineTimer++;
        }

        private void GroundMovement(Vector2 targetPos)
        {
            Projectile.velocity.Y += 0.2f;
            int directionX = Projectile.Center.X < targetPos.X ? 1 : -1;
            float xSpeed = 3f;

            Projectile.velocity.X = Math.Clamp(Projectile.velocity.X + (directionX * 0.1f), -xSpeed, xSpeed);

            if (Math.Abs(Projectile.velocity.X) > 0.5f)
            {
                Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            }

            Projectile.rotation = 0f;

            Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);

            SineTimer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            int frameX = Wall ? 0 : tex.Width / 2;
            Rectangle frame = new(frameX, Projectile.frame * (tex.Height / Main.projFrames[Type]), tex.Width / 2, tex.Height / Main.projFrames[Type]);
            Vector2 origin = new(tex.Width / 4, tex.Height / Main.projFrames[Type] / 2);
            Vector2 offset = new(!Wall ? -8f * Projectile.spriteDirection : 0f, Projectile.gfxOffY);

            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + offset, frame, lightColor, Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }
    }

    public class IncendipedeMinionBody : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead || !player.active || !player.HasBuff(ModContent.BuffType<IncendipedeMinionBuff>()))
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;

            Projectile head = Main.projectile.FirstOrDefault(p => p.active && p.owner == Projectile.owner && p.type == ModContent.ProjectileType<IncendipedeMinionHead>());
            if (head != null)
            {
                var bodies = Main.projectile.Where(p => p.active && p.owner == Projectile.owner && p.type == Type).OrderBy(p => p.whoAmI).ToList();
                int myID = bodies.IndexOf(Projectile) + 1;
                int spacingIndex = myID * IncendipedeMinionHead.SpacingBetween;

                if (spacingIndex < head.oldPos.Length && head.oldPos[spacingIndex] != Vector2.Zero)
                {
                    Projectile.position = head.oldPos[spacingIndex];
                    Projectile.rotation = head.oldRot[spacingIndex];
                    if (spacingIndex > 0)
                        Projectile.spriteDirection = (Projectile.position - head.oldPos[spacingIndex - 1]).X > 0 ? 1 : -1;

                    if (head.ai[1] != 0)
                    {
                        Projectile.ai[0]++;
                        if (Projectile.ai[0] > 180)
                        {
                            Projectile.ai[0] = Main.rand.Next(-30, 30);
                            if (Main.myPlayer == Projectile.owner)
                            {
                                Vector2 dir = (Projectile.position - head.oldPos[spacingIndex - 1]).SafeNormalize(Vector2.UnitX);
                                Vector2 perp = dir.RotatedBy(Main.rand.NextBool() ? MathHelper.PiOver2 : -MathHelper.PiOver2) * 4f;

                                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perp, ModContent.ProjectileType<IncendipedeFireSpike>(), Projectile.originalDamage / 2, 1f, Projectile.owner);
                                Main.projectile[p].friendly = true;
                                Main.projectile[p].hostile = false;
                            }
                        }
                    }
                    else
                    {
                        Projectile.ai[0] = myID * 10;
                    }
                }
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Tile t = Framing.GetTileSafely(Projectile.Center.ToTileCoordinates());
            bool Wall = t.WallType != WallID.None || (t.HasUnactuatedTile && Main.tileSolid[t.TileType] && !Main.tileSolidTop[t.TileType]);
            int frameX = Wall ? 0 : tex.Width / 2;
            Rectangle frame = new(frameX, Projectile.frame * (tex.Height / Main.projFrames[Type]), tex.Width / 2, tex.Height / Main.projFrames[Type]);
            Vector2 origin = new(tex.Width / 4, tex.Height / Main.projFrames[Type] / 2);
            Vector2 offset = new(0, Projectile.gfxOffY);

            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + offset, frame, lightColor, Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }
    }

    public class IncendipedeMinionTail : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead || !player.active || !player.HasBuff(ModContent.BuffType<IncendipedeMinionBuff>()))
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;

            Projectile head = Main.projectile.FirstOrDefault(p => p.active && p.owner == Projectile.owner && p.type == ModContent.ProjectileType<IncendipedeMinionHead>());
            if (head != null)
            {
                int bodyCount = player.ownedProjectileCounts[ModContent.ProjectileType<IncendipedeMinionBody>()];
                int spacingIndex = (bodyCount + 1) * IncendipedeMinionHead.SpacingBetween;

                if (spacingIndex < head.oldPos.Length && head.oldPos[spacingIndex] != Vector2.Zero)
                {
                    Projectile.position = head.oldPos[spacingIndex];
                    Projectile.rotation = head.oldRot[spacingIndex];
                    if (spacingIndex > 0)
                        Projectile.spriteDirection = (Projectile.position - head.oldPos[spacingIndex - 1]).X > 0 ? 1 : -1;

                    if (head.ai[1] != 0)
                    {
                        Projectile.ai[0]++;
                        if (Projectile.ai[0] > 180)
                        {
                            Projectile.ai[0] = Main.rand.Next(-20, 20);
                            if (Main.myPlayer == Projectile.owner && spacingIndex > 0)
                            {
                                Vector2 dir = (Projectile.position - head.oldPos[spacingIndex - 1]).SafeNormalize(Vector2.UnitX);
                                Vector2 perp = dir.RotatedBy(MathHelper.PiOver2) * 4f;
                                Vector2 behind = -dir * 4f;

                                int p1 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perp, ModContent.ProjectileType<IncendipedeFireSpike>(), Projectile.originalDamage / 2, 1f, Projectile.owner);
                                Main.projectile[p1].friendly = true; Main.projectile[p1].hostile = false;

                                int p2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -perp, ModContent.ProjectileType<IncendipedeFireSpike>(), Projectile.originalDamage / 2, 1f, Projectile.owner);
                                Main.projectile[p2].friendly = true; Main.projectile[p2].hostile = false;

                                int p3 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, behind, ModContent.ProjectileType<IncendipedeFireSpike>(), Projectile.originalDamage / 2, 1f, Projectile.owner);
                                Main.projectile[p3].friendly = true; Main.projectile[p3].hostile = false;
                            }
                        }
                    }
                    else
                    {
                        Projectile.ai[0] = 0;
                    }
                }
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= Main.projFrames[Type])
                    Projectile.frame = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Tile t = Framing.GetTileSafely(Projectile.Center.ToTileCoordinates());
            bool Wall = t.WallType != WallID.None || (t.HasUnactuatedTile && Main.tileSolid[t.TileType] && !Main.tileSolidTop[t.TileType]);
            int frameX = Wall ? 0 : tex.Width / 2;
            Rectangle frame = new(frameX, Projectile.frame * (tex.Height / Main.projFrames[Type]), tex.Width / 2, tex.Height / Main.projFrames[Type]);
            Vector2 origin = new(tex.Width / 4, tex.Height / Main.projFrames[Type] / 2);
            Vector2 offset = new(0, Projectile.gfxOffY);

            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + offset, frame, lightColor, Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }
    }
}