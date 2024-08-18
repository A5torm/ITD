﻿using ITD.Content.Tiles.Furniture.Catacombs;
using Terraria.ModLoader;

namespace ITD.Content.Items.Placeable
{
    public class BlueCatacombChair : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CatacombChairTile>());
            Item.width = 12;
            Item.height = 30;
        }
    }
    public class GreenCatacombChair : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CatacombChairTile>(), 1);
            Item.width = 12;
            Item.height = 30;
        }
    }
    public class PinkCatacombChair : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CatacombChairTile>(), 2);
            Item.width = 12;
            Item.height = 30;
        }
    }
}