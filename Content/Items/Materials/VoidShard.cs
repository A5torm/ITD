﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace ITD.Content.Items.Materials
{
    public class VoidShard : ModItem
    {
        public override void SetStaticDefaults()
        { 
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 24;
			Item.rare = ItemRarityID.Blue;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(silver: 1);
        }
    }
}