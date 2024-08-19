﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;

namespace ITD.Content.Items.Accessories.Movement
{
    public class ImpulseShackles : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			ImpulseShacklesPlayer modPlayer = player.GetModPlayer<ImpulseShacklesPlayer>();
			
            if (player.jump <= 0 && player.velocity.Y == 0f)
            {
                modPlayer.impulseShackles = true;
				modPlayer.impulseJump =  0f;
            }
            else
            {
                modPlayer.impulseShackles = false;
            }
			
			if (player.controlJump && player.releaseJump)
            {
				if (modPlayer.impulseJump > 0)
				{
					modPlayer.impulseJump =  0f;
				}
				else if (modPlayer.impulseShackles)
				{
					modPlayer.impulseJump =  3f;
					
					for (int i = 0; i < 20; i++)
					{
						int dust = Dust.NewDust(player.MountedCenter+new Vector2(-44f, player.height/2), 80, 0, 255, 0f, 0f, 0, default, 1.5f);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].velocity.X = 0;
					}
					SoundEngine.PlaySound(SoundID.Item91, player.Center);
				}
            }
			
			if (modPlayer.impulseJump > 0)
			{				
				int dust = Dust.NewDust(player.MountedCenter - new Vector2(4f, 0) - player.velocity, 0, 0, 255, 0, 0, 100, default, 1.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity = player.velocity/2f;
			
				player.jumpSpeedBoost += modPlayer.impulseJump;
				
				player.runAcceleration *= 2f;
				player.maxRunSpeed *= 2f;
				player.runSlowdown *= 2f;
				
				modPlayer.impulseJump *= 0.95f;
			}
        }
    }
    public class ImpulseShacklesPlayer : ModPlayer
    {
        public bool impulseShackles = false;
		public float impulseJump = 0f;
    }
}