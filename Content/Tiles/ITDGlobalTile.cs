﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ITD.Content.Dusts;
using ITD.Content.Tiles.BlueshroomGroves;

namespace ITD.Content.Tiles
{
    public class ITDGlobalTile : GlobalTile
    {
        public override void PlaceInWorld(int i, int j, int type, Item item)
        {
            //Main.NewText($"{i},{j}");
        }
    }
}