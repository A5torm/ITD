using ITD.Particles;
using ITD.Particles.Projectiles;
using ITD.Systems;
using ITD.Utilities;
using System;
using Terraria.GameContent;

namespace ITD.Content.Projectiles.Hostile.MotherWisp;

public class WispFireBreath : ModProjectile
{
    public override string Texture => ITD.BlankTexture;
    public ParticleEmitter emitter;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 1;
        ProjectileID.Sets.TrailCacheLength[Type] = 30;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.aiStyle = 0;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 180;
        emitter = ParticleSystem.NewEmitter<WispFlame>(ParticleEmitterDrawCanvas.WorldOverProjectiles);
        emitter.tag = Projectile;
    }

    public override bool? CanDamage()
    {
        return true;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return new Color(255, 255, 255);
    }
    private float rayPosY//be accurate
    {
        get => Projectile.ai[1];
        set => Projectile.ai[1] = value;
    }
    public override void AI()
    {
        if (emitter != null)
            emitter.keptAlive = true;

        Projectile.hide = true;
        if (Main.rand.NextBool(2))
            emitter?.Emit(Projectile.Center + Main.rand.NextVector2Square(-Projectile.width / 4, Projectile.width / 4), Projectile.velocity * 0.2f, Projectile.velocity.ToRotation() - MathHelper.PiOver2, 20);
                
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }
    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
 
        return false;
    }
}