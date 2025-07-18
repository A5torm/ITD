﻿namespace ITD.Content.Items.Armor.Alloy
{
    [AutoloadEquip(EquipType.Body)]
    public class AlloyChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 22;
            Item.value = Item.sellPrice(silver: 10);
            Item.rare = ItemRarityID.Green;
            Item.defense = 2;
        }
    }
}
