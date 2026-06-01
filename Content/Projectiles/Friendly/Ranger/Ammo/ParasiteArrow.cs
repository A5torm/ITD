using ITD.Content.Projectiles.Friendly.Misc;
using ITD.Particles;
using ITD.Particles.Projectiles;
using Terraria.Audio;
using Terraria.GameContent;

namespace ITD.Content.Projectiles.Friendly.Ranger.Ammo;

public class ParasiteArrow : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(1);
    }

    public override void AI()
    {
    }

    public override void OnKill(int timeLeft)
    {

    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        for (int g = 0; g < 2; g++)
        {
            var goreSpawnPosition = Projectile.Center;
            Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, 912, 1f);
            gore.scale = 1f;
            gore.velocity.X += 1f;
            gore.velocity.Y += 1f;
            gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, 912, 1f);
            gore.scale = 1.5f;
            gore.velocity.X -= 1f;
            gore.velocity.Y += 1f;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        for (int g = 0; g < 2; g++)
        {
            var goreSpawnPosition = Projectile.Center;
            Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, 912, 1f);
            gore.scale = 1f;
            gore.velocity.X += 1f;
            gore.velocity.Y += 1f;
            gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, 912, 1f);
            gore.scale = 1.5f;
            gore.velocity.X -= 1f;
            gore.velocity.Y += 1f;
        }
        return true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return true;
    }
}