﻿using ITD.Content.Dusts;
using ITD.Content.Projectiles.Friendly.Melee;
using ITD.Utilities;
using ITD.Utilities.EntityAnim;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static ITD.Utilities.ITDShapes;

namespace ITD.Content.Items.Weapons.Melee
{
    public class Flyswatter : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = Item.height = 34;
            Item.damage = 24;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 12.5f;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<FlyswatterHeldProjectile>();
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
        }
    }
}
