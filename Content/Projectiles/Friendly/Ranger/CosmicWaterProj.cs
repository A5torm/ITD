using ITD.Content.Buffs.Debuffs;
using ITD.Content.Projectiles.Friendly.Misc;
using ITD.Particles;
using ITD.Particles.Misc;
using ITD.Particles.Projectiles;
using Terraria.Audio;
using Terraria.GameContent;

namespace ITD.Content.Projectiles.Friendly.Ranger;

public class CosmicWaterProj : ModProjectile
{
    public override string Texture => ITD.BlankTexture;
    public ParticleEmitter emitter;
    public ParticleEmitter emitter2;
    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.friendly = true;
        Projectile.hostile = true;
        Projectile.alpha = 255;
        Projectile.penetrate = 1;
        Projectile.MaxUpdates = 3;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.timeLeft = 60;
        emitter = ParticleSystem.NewEmitter<Particles.Projectiles.TheEpicenterFlash>(ParticleEmitterDrawCanvas.WorldUnderProjectiles);
        emitter.tag = Projectile;
        emitter2 = ParticleSystem.NewEmitter<BeanMist>(ParticleEmitterDrawCanvas.WorldUnderProjectiles);
        emitter2.tag = Projectile;
    }

    public override void AI()
    {
        Projectile.ai[0]++;
        if (Projectile.ai[0] > 15f)
        {
            Projectile.velocity.Y += 0.1f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }
        if (emitter != null)
            emitter.keptAlive = true;
        if (emitter2 != null)
            emitter2.keptAlive = true;
        if (Projectile.ai[0] > 1f)
        {
            int dustDensity = 2;

            for (int j = 0; j < dustDensity; j++)
            {
                Vector2 lerpedPosition = Vector2.Lerp(Projectile.oldPosition, Projectile.position, j / (float)dustDensity);
                lerpedPosition += new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-1.5f, 1.5f));
                emitter?.Emit(lerpedPosition, 
                    Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30)) * 0.4f);
                emitter2?.Emit(lerpedPosition, Projectile.velocity * 0.2f);

            }
        }
    }
    public override bool CanHitPlayer(Player target)
    {
        return false;
    }
    public override void OnKill(int timeLeft)
    {

    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {


    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity *= 0.4f;
        return true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpriteBatch sb = Main.spriteBatch;
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
        return false;
    }
}