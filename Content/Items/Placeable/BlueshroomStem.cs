﻿using ITD.Content.Tiles.LayersRework;
using Terraria.ModLoader;

namespace ITD.Content.Items.Placeable
{
    public class Shale : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<BlueshroomStemTile>());
            Item.width = 12;
            Item.height = 12;
        }
    }
}