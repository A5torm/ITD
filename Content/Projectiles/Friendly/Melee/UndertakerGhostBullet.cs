using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;

namespace ITD.Content.Projectiles.Friendly.Melee;

public class UndertakerGhostBullet : ModProjectile
{
    public VertexStrip TrailStrip = new();

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 28;
        Projectile.aiStyle = 1;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.noEnchantments = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 40;
        Projectile.alpha = 0;
        Projectile.light = 0.5f;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.extraUpdates = 0;
        AIType = ProjectileID.Bullet;
    }

    Vector2 spawnvel;
    public override void OnSpawn(IEntitySource source)
    {
    }
    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (Projectile.timeLeft <= 20)
        {
            Projectile.velocity *= 0.8f;
            Projectile.alpha += 5;
        }
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return true;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        for (int i = 0; i < 12; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width / 4, Projectile.height / 4, DustID.CrimsonTorch, Projectile.velocity.X / 1.5f, Projectile.velocity.Y / 1.5f, 60, default, Main.rand.NextFloat(1f, 1.5f));
            dust.noGravity = true;
            dust.velocity *= 5f;
        }
    }
    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 12; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width / 4, Projectile.height / 4, DustID.CrimsonTorch, Projectile.velocity.X / 1.5f, Projectile.velocity.Y / 1.5f, 60, default, Main.rand.NextFloat(1f, 1.5f));
            dust.noGravity = true;
            dust.velocity *= 5f;
        }
        SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
    }
    private Color StripColors(float progressOnStrip)
    {
        Color result = new Color(255, 44, 44);
        result.A /= 2;
        return result * 0.5f;
    }
    private float StripWidth(float progressOnStrip)
    {
        return MathHelper.Lerp(6, 0.1f, progressOnStrip * 1.25f);
    }
    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
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
        Main.EntitySpriteDraw(texture, position, rectangle, lightColor, Projectile.rotation + MathHelper.PiOver2, rectangle.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }
}