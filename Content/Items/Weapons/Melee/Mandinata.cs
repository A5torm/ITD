using ITD.Content.Items.Materials;
using ITD.Content.Projectiles.Friendly.Melee;
using ITD.Systems;
using ITD.Utilities;
using Terraria.Audio;
using Terraria.DataStructures;

namespace ITD.Content.Items.Weapons.Melee;

public class Mandinata : ModItem
{
	public int directionCycle = 0;
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
        ItemID.Sets.Spears[Item.type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 21;
        Item.DamageType = DamageClass.Melee;
        Item.width = 40;
        Item.height = 40;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.UseSound = SoundID.Item20;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 7;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.shootSpeed = 32f;
        Item.shoot = ModContent.ProjectileType<MandinataProjectile>();
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
		float direction = 1;
        directionCycle = ++directionCycle % 2;
        if (directionCycle == 0)
            direction = -1;
        velocity.Normalize();

        float adjustedItemScale = player.GetAdjustedItemScale(player.inventory[player.selectedItem]);
		
        Projectile.NewProjectile(source, position, velocity.RotatedBy(-2 * direction) * adjustedItemScale * 32f, type, damage, knockback, player.whoAmI, direction);
        return false;
    }

    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[Item.shoot] < 1;
    }

    public override bool? UseItem(Player player)
    {
        if (!Main.dedServ && Item.UseSound.HasValue)
        {
            SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
        }

        return null;
    }

    public override void AddRecipes()
    {
        CreateRecipe(1)
            .AddIngredient(ModContent.ItemType<EmberlionMandible>(), 1)
            .AddIngredient(ModContent.ItemType<EmberlionSclerite>(), 4)
            .AddIngredient(ItemID.IronBar, 6)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
