using ITD.Content.Items.Materials;
using ITD.Content.Projectiles.Friendly.Ranger.Ammo;
using Terraria.DataStructures;

namespace ITD.Content.Items.Weapons.Ranger;

public class FungalBow : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
    }
    public override void SetDefaults()
    {
        Item.damage = 12;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 38;
        Item.height = 60;
        Item.useTime = 36;
        Item.useAnimation = 36;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.knockBack = 2;
        Item.value = Item.sellPrice(silver: 50);
        Item.rare = ItemRarityID.Blue;
        Item.UseSound = SoundID.Item5;
        Item.shoot = ProjectileID.WoodenArrowFriendly;
        Item.shootSpeed = 8f;
        Item.useAmmo = AmmoID.Arrow;
        Item.autoReuse = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        type = ModContent.ProjectileType<ParasiteArrow>();
        Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }
}
