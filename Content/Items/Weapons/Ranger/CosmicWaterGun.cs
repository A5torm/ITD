using ITD.Content.Items.Placeable.LayersRework;
using ITD.Content.Projectiles.Friendly.Ranger;
using ITD.Content.Projectiles.Friendly.Ranger.Ammo;
using Terraria.DataStructures;

namespace ITD.Content.Items.Weapons.Ranger;

public class CosmicWaterGun : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
    }
    public override void SetDefaults()
    {
        Item.damage = 10;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 56;
        Item.height = 28;
        Item.useTime = 10;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.knockBack = 2;
        Item.value = Item.sellPrice(silver: 50);
        Item.rare = ItemRarityID.Orange;
        Item.shoot = ModContent.ProjectileType<HoneyGunProj>();
        Item.shootSpeed = 10f;
        Item.autoReuse = true;
    }
    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-24f, -6f);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = velocity.SafeNormalize(Vector2.Zero) * 60f;
        position += new Vector2(-8f * player.direction, -6f);
        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
        }
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(4));
        newVelocity *= Main.rand.NextFloat(0.9f, 1.1f);

        Projectile.NewProjectileDirect(source, position, newVelocity, ModContent.ProjectileType<CosmicWaterProj>(), damage, knockback, player.whoAmI);
        return false;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.LavaBucket, 1);
        recipe.AddIngredient(ItemID.Obsidian, 10);
        recipe.AddTile(TileID.Anvils);
        recipe.Register();
    }
}
