using ITD.Particles;
using ITD.Particles.Projectiles;
using ITD.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace ITD.Content.Projectiles.Friendly.Ranger;

public class ResonanceBar : ModProjectile
{
    public VertexStrip TrailStrip = new();

    public static readonly SoundStyle MetalPipeSound = new SoundStyle("ITD/Content/Sounds/MetalPipeFalling");

    private static readonly Stack<Projectile> SharedStack = new();
    private static readonly HashSet<int> SharedProcessed = new();

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        ProjectileID.Sets.DontAttachHideToAlpha[Projectile.type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.width = 56;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 1200;
        Projectile.alpha = 0;
        Projectile.light = 0.5f;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.extraUpdates = 1; // 120 ticks per second
        Projectile.hide = true;
    }

    private bool isStuck = false;
    private bool doCollsion = false;
    public int DamageTime = 0;
    private readonly List<int> connectedProjectiles = new();

    public bool isFalling;
    public bool isStickingToGround;

    // --- NEW: Cascading Collapse Variables ---
    public bool isCrumbling = false;
    public int crumbleDelay = 0;

    Vector2 spawnvel;
    float spawnrotation;

    public bool IsStickingToTarget
    {
        get => Projectile.ai[0] == 1f;
        set => Projectile.ai[0] = value ? 1f : 0f;
    }

    public int TargetWhoAmI
    {
        get => (int)Projectile.ai[1];
        set => Projectile.ai[1] = value;
    }

    public int ProjectileWhoAmI
    {
        get => (int)Projectile.ai[2];
        set => Projectile.ai[2] = value;
    }

    public override bool? CanDamage()
    {
        return (!IsStickingToTarget && !isStuck) || DamageTime > 0;
    }

    public override void AI()
    {
        if (IsStickingToTarget)
        {
            StickyAI();
        }
        else
        {
            NormalAI();
        }

        if (Projectile.velocity != Vector2.Zero)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }

    private void NormalAI()
    {
        if (!IsStickingToTarget)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        DamageTime--;

        for (int i = 0; i < Main.maxProjectiles; i++)
        {
            Projectile other = Main.projectile[i];

            if (other.active && other.type == Projectile.type && other.whoAmI != Projectile.whoAmI)
            {
                if (Vector2.DistanceSquared(Projectile.Center, other.Center) < 1000000f)
                {
                    if (RotatedRectangleCollision(Projectile, other))
                    {
                        if (other.ai[0] != 0)
                        {
                            Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.Center, -Projectile.velocity / 2, ModContent.Find<ModGore>(Mod.Name, "ResonanceBarGore").Type);

                            Player player = Main.LocalPlayer;
                            float power = 6 * Utils.GetLerpValue(800f, 0f, Projectile.Distance(player.Center), true);
                            player.GetITDPlayer().BetterScreenshake(6, power, power, false);

                            Projectile Blast = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                                ModContent.ProjectileType<ResonanceBlast>(), Projectile.damage * 2, Projectile.knockBack);

                            Blast.ai[1] = 60f;
                            Blast.localAI[1] = Main.rand.NextFloat(0.18f, 0.3f);
                            Blast.netUpdate = true;
                            Blast.CritChance = Projectile.CritChance;
                            other.Kill();
                            Projectile.Kill();
                            return;
                        }

                        if (!doCollsion)
                        {
                            Player player = Main.LocalPlayer;
                            float power = 4 * Utils.GetLerpValue(800f, 0f, Projectile.Distance(player.Center), true);
                            player.GetITDPlayer().BetterScreenshake(6, power, power, false);
                            doCollsion = true;
                            OnCollision(other);
                        }
                        break;
                    }
                }
            }
        }

        if (isStickingToGround) return;

        // --- NEW: Cascading Sequence ---
        if (isCrumbling)
        {
            if (crumbleDelay > 0)
            {
                crumbleDelay--;
            }
            else
            {
                // Timer is up! Fall natively.
                Projectile.tileCollide = true;
                isStickingToGround = false;
                isFalling = true;
                Projectile.velocity.Y += 10f;
                isCrumbling = false;
            }
        }
        else if (Projectile.velocity == Vector2.Zero)
        {
            if (Projectile.timeLeft % 10 == 0 && !HasGroundConnectionInStructure())
            {
                MakeStructureFall();
            }
        }
    }

    private void StickyAI()
    {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        int npcTarget = TargetWhoAmI;

        if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage)
        {
            Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
            Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
        }
        else
        {
            Projectile.Kill();
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (!isFalling)
        {
            isStickingToGround = true;
        }
        else
        {
            Gore gore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.Center + new Vector2(0, -20), new Vector2(0, -10), ModContent.Find<ModGore>(Mod.Name, "ResonanceBarGore1").Type);
            gore.timeLeft = 180;
            Projectile.Kill();
        }

        isStuck = true;
        Projectile.velocity = Vector2.Zero;
        connectedProjectiles.Clear();
        FindConnectedBars(Projectile);

        return false;
    }

    private void OnCollision(Projectile other)
    {
        if (other.ai[0] != 0)
        {
            other.Kill();
            Projectile.Kill();
            return;
        }

        connectedProjectiles.Clear();
        FindConnectedBars(Projectile);
        Projectile.velocity = Vector2.Zero;

        foreach (int projIndex in connectedProjectiles)
        {
            Projectile bar = Main.projectile[projIndex];

            if (bar.ModProjectile is ResonanceBar barProj)
            {
                barProj.DamageTime = 10;
                ParticleEmitter part = ParticleSystem.NewSingleParticle<ResonanceParticle>(bar.Center, Vector2.Zero, barProj.spawnrotation, lifetime: 40);
                part.additive = true;
                part.tag = bar;
                isStuck = true;

                if (!barProj.isStuck)
                {
                    barProj.isStuck = true;
                    bar.velocity = Vector2.Zero;
                    bar.tileCollide = false;
                }
            }
        }
    }

    private static bool RotatedRectangleCollision(Projectile proj1, Projectile proj2)
    {
        Span<Vector2> corners1 = stackalloc Vector2[4];
        Span<Vector2> corners2 = stackalloc Vector2[4];

        GetRotatedCorners(proj1, corners1);
        GetRotatedCorners(proj2, corners2);

        if (!OverlapOnAxis(corners1, corners2, corners1[1] - corners1[0])) return false;
        if (!OverlapOnAxis(corners1, corners2, corners1[3] - corners1[0])) return false;
        if (!OverlapOnAxis(corners1, corners2, corners2[1] - corners2[0])) return false;
        if (!OverlapOnAxis(corners1, corners2, corners2[3] - corners2[0])) return false;

        return true;
    }

    private static bool OverlapOnAxis(ReadOnlySpan<Vector2> shape1, ReadOnlySpan<Vector2> shape2, Vector2 axis)
    {
        axis = Vector2.Normalize(axis);
        ProjectMinMax(shape1, axis, out float min1, out float max1);
        ProjectMinMax(shape2, axis, out float min2, out float max2);

        return max1 >= min2 && max2 >= min1;
    }

    private static void ProjectMinMax(ReadOnlySpan<Vector2> shape, Vector2 axis, out float min, out float max)
    {
        min = float.MaxValue;
        max = float.MinValue;

        foreach (Vector2 point in shape)
        {
            float projection = Vector2.Dot(point, axis);
            min = Math.Min(min, projection);
            max = Math.Max(max, projection);
        }
    }

    private static void GetRotatedCorners(Projectile proj, Span<Vector2> corners)
    {
        float halfWidth = proj.width * 0.5f;
        float halfHeight = proj.height * 0.5f;
        float rot = (proj.ModProjectile is ResonanceBar barProj) ? barProj.spawnrotation : proj.rotation;

        float cos = (float)Math.Cos(rot);
        float sin = (float)Math.Sin(rot);
        float cx = proj.Center.X;
        float cy = proj.Center.Y;

        for (int i = 0; i < 4; i++)
        {
            float px = (i == 0 || i == 3) ? -halfWidth : halfWidth;
            float py = (i == 0 || i == 1) ? -halfHeight : halfHeight;
            corners[i] = new Vector2(cx + px * cos - py * sin, cy + px * sin + py * cos);
        }
    }

    private bool HasGroundConnectionInStructure()
    {
        connectedProjectiles.Clear();
        FindConnectedBars(Projectile);

        foreach (int projIndex in connectedProjectiles)
        {
            if (Main.projectile[projIndex].ModProjectile is ResonanceBar bar && bar.isStickingToGround)
                return true;
        }
        return false;
    }

    private void MakeStructureFall()
    {
        connectedProjectiles.Clear();
        FindConnectedBars(Projectile);

        // Reverse the list so the edges of the structure fall first, creating a cascading crumble towards the root.
        connectedProjectiles.Reverse();

        int currentDelay = 0;

        foreach (int projIndex in connectedProjectiles)
        {
            Projectile bar = Main.projectile[projIndex];
            if (bar.active && bar.ModProjectile is ResonanceBar barProj && !barProj.isCrumbling && !barProj.isFalling)
            {
                barProj.isCrumbling = true;
                barProj.crumbleDelay = currentDelay;

                // Because you have extraUpdates = 1 (120 ticks per second), a delay of 6 means ~20 bars fall per second.
                // Adjust this number to make it crumble slower/faster!
                currentDelay += 6;
            }
        }
    }

    private void FindConnectedBars(Projectile start)
    {
        SharedStack.Clear();
        SharedProcessed.Clear();

        SharedStack.Push(start);
        SharedProcessed.Add(start.whoAmI);
        connectedProjectiles.Add(start.whoAmI);

        while (SharedStack.Count > 0)
        {
            Projectile current = SharedStack.Pop();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];

                if (!other.active || other.type != Projectile.type ||
                    other.whoAmI == current.whoAmI || SharedProcessed.Contains(other.whoAmI))
                {
                    continue;
                }

                if (Vector2.DistanceSquared(current.Center, other.Center) < 1000000f)
                {
                    if (RotatedRectangleCollision(current, other))
                    {
                        SharedStack.Push(other);
                        SharedProcessed.Add(other.whoAmI);
                        connectedProjectiles.Add(other.whoAmI);
                    }
                }
            }
        }
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
        spawnrotation = Projectile.rotation;
        spawnvel = Projectile.velocity;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!isStuck || Projectile.velocity != Vector2.Zero)
        {
            IsStickingToTarget = true;
            TargetWhoAmI = target.whoAmI;
            Projectile.rotation = spawnrotation;
            Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
        }
        Projectile.netUpdate = true;
    }

    public override void OnKill(int timeLeft)
    {
        if (Main.myPlayer == Projectile.owner)
        {
            SoundEngine.PlaySound(MetalPipeSound, Projectile.Center);
        }
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        if (IsStickingToTarget)
        {
            int npcIndex = TargetWhoAmI;
            if (npcIndex >= 0 && npcIndex < Main.maxNPCs && Main.npc[npcIndex].active)
            {
                behindNPCsAndTiles.Add(index);
                return;
            }
        }
        behindNPCsAndTiles.Add(index);
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        width = height = 10;
        return true;
    }

    private Color StripColors(float progressOnStrip)
    {
        Color result = Color.Lerp(new Color(243, 162, 63), new Color(227, 98, 43), Utils.GetLerpValue(0f, 0.7f, progressOnStrip, true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip, false));
        result.A /= 2;
        return result * 0.5f;
    }

    private float StripWidth(float progressOnStrip)
    {
        return MathHelper.Lerp(6f, 5f, Utils.GetLerpValue(0f, 0.2f, progressOnStrip, true)) * Utils.GetLerpValue(0f, 0.07f, progressOnStrip, true);
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return new Color(255, 170, 90);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        GameShaders.Misc["LightDisc"].Apply(null);
        TrailStrip.PrepareStrip(Projectile.oldPos, Projectile.oldRot, StripColors, StripWidth, Projectile.Size * 0.5f - Main.screenPosition, Projectile.oldPos.Length, true);
        TrailStrip.DrawTrail();

        Main.pixelShader.CurrentTechnique.Passes[0].Apply();

        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Rectangle rectangle = texture.Frame(1, 1);
        Vector2 position = Projectile.Center - Main.screenPosition;
        Main.EntitySpriteDraw(texture, position, rectangle, lightColor, spawnrotation, rectangle.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }
}