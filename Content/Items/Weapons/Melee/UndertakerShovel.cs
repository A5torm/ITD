using ITD.Content.Projectiles;
using ITD.Content.Projectiles.Friendly.Melee;
using ITD.Content.Projectiles.Friendly.Ranger;
using ITD.Content.Projectiles.Friendly.Summoner;
using ITD.Systems;
using ITD.Utilities;
using Terraria.Audio;
using Terraria.DataStructures;

namespace ITD.Content.Items.Weapons.Melee;

public class UndertakerShovel : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
    }
    public override void SetDefaults()
    {
        Item.damage = 20;
        Item.DamageType = DamageClass.Melee;
        Item.width = 38;
        Item.height = 38;
        Item.useTime = 33;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 5f;
        Item.value = 10000;
        Item.rare = ItemRarityID.Blue;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = false;
        Item.shootSpeed = 8;
        Item.shoot = ModContent.ProjectileType<UndertakerGhostProj>();
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return false;
    }
    public override void MeleeEffects(Player player, Rectangle hitbox)
    {
        MiscHelpers.GetPointOnSwungItemPath(player, 30f, 30f, 0.2f + 0.8f * Main.rand.NextFloat(), 
            player.GetAdjustedItemScale(Item), out Vector2 pos, out Vector2 spinningpoint);
        Vector2 value = spinningpoint.RotatedBy((double)(1.57079637f * player.direction * player.gravDir), default);
        Dust.NewDustPerfect(pos, DustID.CrimsonTorch, new Vector2?(value * 4f), 100, default, 1.5f).noGravity = true;
        SoundEngine.PlaySound(SoundID.Item15, player.position);

        if (player.itemAnimation == 1f)
        {
            Vector2 position = player.Center + new Vector2(60f * player.direction, player.height * 0.5f);
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), position, new Vector2((Main.rand.NextFloat(1,2))
                * player.direction, Main.rand.NextFloat(-6,-2)),
                ModContent.ProjectileType<UndertakerGhostProj>(), 20, 5f, player.whoAmI);
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(position, DustID.CrimsonTorch, new Vector2((Main.rand.NextFloat(1, 2))
                * player.direction, Main.rand.NextFloat(-6, -2)), 0, default, 1.5f).noGravity = true;

                Dust.NewDustPerfect(position, DustID.Crimson, new Vector2((Main.rand.NextFloat(1, 2))
                * player.direction, Main.rand.NextFloat(-6, -2)), 0, default, 1.5f).noGravity = true;
            }
        }
    }
}