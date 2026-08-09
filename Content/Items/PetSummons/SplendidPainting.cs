using ITD.Content.Buffs.PetBuffs;
using ITD.Content.Projectiles.Friendly.Pets;

namespace ITD.Content.Items.PetSummons;

public class SplendidPainting : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
    }
    public override void SetDefaults()
    {
        Item.damage = 0;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.UseSound = SoundID.Item2;
        Item.useAnimation = 20;
        Item.useTime = 20;
        Item.rare = ItemRarityID.Blue;
        Item.noMelee = true;
        Item.height = 32;
        Item.width = 32;
        Item.shoot = ModContent.ProjectileType<SplendidPaintingPet>();
        Item.buffType = ModContent.BuffType<SplendidPaintingBuff>();
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
        {
            player.AddBuff(Item.buffType, 3600);
        }
        return true;
    }
}