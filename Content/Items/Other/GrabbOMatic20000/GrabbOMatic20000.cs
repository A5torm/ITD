﻿using ITD.Kinematics;
using ITD.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ITD.Content.Items.Other.GrabbOMatic20000
{
    public class GrabbOMatic20000 : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
        }
        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            player.GetModPlayer<GrabbOMaticPlayer>().Active = true;
        }
    }
    public class GrabbOMaticPlayer : ModPlayer
    {
        private const string toTex = "ITD/Content/Items/Other/GrabbOMatic20000/";
        private static readonly Asset<Texture2D> armTex = ModContent.Request<Texture2D>(toTex + "SegBase");
        private static readonly Asset<Texture2D> armMidTex = ModContent.Request<Texture2D>(toTex + "SegMid");
        private static readonly Asset<Texture2D> handTex = ModContent.Request<Texture2D>(toTex + "SegHand");
        public bool Active;
        public KineChain arm = null;
        public override void ResetEffects()
        {
            Active = false;
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (!Active)
                return;
            float[][] armP =
                [
                    KineChain.CreateKineSegment(42f),
                    KineChain.CreateKineSegment(42f),
                    KineChain.CreateKineSegment(42f),
                ];
            arm ??= new KineChain(Player.Center.X, Player.Center.Y, armP);
            arm.basePoint = Player.Center;
            arm.GenUpdate(Player.GetITDPlayer().MousePosition);
            arm.Draw(Main.spriteBatch, Main.screenPosition, Color.White, Player.direction == 1, armMidTex.Value, handTex.Value, armTex.Value);
        }
    }
}
