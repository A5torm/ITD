﻿using System;
using System.Collections.Generic;

using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader.IO;

using ITD.Systems;
using ITD.Content.NPCs;
using ITD.Content.Items.Weapons.Melee;
using ITD.Content.Dusts;
using ITD.Physics;
using ITD.Content.Projectiles.Friendly.Misc;
using ITD.Utilities;
using ITD.Networking;
using ITD.Networking.Packets;
using ITD.Content.Buffs.EquipmentBuffs;
using ITD.Content.UI;
using ITD.Content.Items.DevTools;
using ITD.Content.Projectiles;
using ITD.Content.TileEntities;
using ITD.Content.Projectiles.Friendly.Mage;
using ITD.Content.Projectiles.Friendly.Summoner;
using ITD.Content.Projectiles.Friendly.Ranger;
using ITD.Content.Projectiles.Friendly.Melee;

namespace ITD.Players
{
    public class ITDPlayer : ModPlayer
    {
        public float[] itemVar = new float[4];
		int heldItem;

        public int charge = 0;
        public float recoilFront = 0f;
		public float recoilBack = 0f;

        public Vector2 MousePosition { get; set; }
        private Vector2 mousePosPrev;
		
		public int dashTime = 0;
		public Vector2 dashVelocity;
		
        bool prevTime = false;
        bool curTime = false;

        public bool ZoneDeepDesert;
        public bool ZoneBlueshroomsUnderground;
        public bool ZoneBlueshroomsSurface;
		public bool ZoneCatacombs;

		public bool necrosis = false;
		public bool soulRot = false;
        public bool melomycosis = false;

		public float blockChance = 0f;
		public bool dreadBlock = false;

        public bool razedWine = false;
        public int razedCooldown = 0;

        public bool portableLab = false;

        public bool soulTalisman = false;
        public bool soulTalismanEffect = false;
        public int soulTalismanStack = 0;
        public int soulTalismanTally = 0;

        public bool wickedHeartVisual = false;
        public bool wickedHeart = false;
        public bool wickedHeartEffect = false;
        public int wickedHeartStack = 0;
        public int wickedHeartStorage = 0;
        public int wickedHeartCooling = 0;

        public bool setAlloy_Melee = false;
        public bool setAlloy_Ranged = false;
        public bool setAlloy_Magic = false;

        public bool orionsRingEffect = false;
        public int orionsRingStack = 0;
        public bool setAlloy { get { return setAlloy_Melee ||  setAlloy_Ranged || setAlloy_Magic; } }

        public int DebuffCount;
		
        //Drawlayer nonsense
        public int frameCounter = 0;
        public int frameEffect = 0;
        public bool CosJellSuffocated;
        public const int CosJellEscapeMax = 10;
        public int CosJellEscapeCurrent;
        //shakeDuration
        public int shakeDuration;
        public float shakeIntensityX;
        public float shakeIntensityY;
        public bool shakeDecay;
        //Recruitment data
        public Guid guid = Guid.Empty;
        //WorldNPC stuff
        public int TalkWorldNPC = -1;
        //cool tools
        public bool selectBox = false;
        public Point16 selectTopLeft;
        public Point16 selectBottomRight;
        public Rectangle selectBounds;
        public SimpleTileDataType tileDataSelection = SimpleTileDataType.None | SimpleTileDataType.Tile | SimpleTileDataType.Wall | SimpleTileDataType.Liquid | SimpleTileDataType.Wiring;
        public override void Load()
        {
            On_Player.ToggleInv += On_Player_ToggleInv;
            On_Player.OpenChest += On_Player_OpenChest;
        }

        private static void On_Player_OpenChest(On_Player.orig_OpenChest orig, Player self, int x, int y, int newChest)
        {
            if (newChest != -1)
            {
                var anchor = self.tileEntityAnchor;
                TileEntity te = anchor.GetTileEntity();
                if (te != null && te is ITDChestTE chest && chest.OpenedBy == self.whoAmI)
                    chest.Close(self, true);
            }
            orig(self, x, y, newChest);
        }

        private static void On_Player_ToggleInv(On_Player.orig_ToggleInv orig, Player self)
        {
            if (Main.playerInventory)
            {
                var anchor = self.tileEntityAnchor;
                TileEntity te = anchor.GetTileEntity();
                if (te != null && te is ITDChestTE chest && chest.OpenedBy == self.whoAmI)
                    chest.Close(self);
            }
            orig(self);
        }

        public override void ResetEffects()
        {
            //shakeDuration
            if (shakeDuration > 0)
                shakeDuration--;
            //Suffocrap
            if (CosJellSuffocated)
            {
                Player.velocity *= 0;
                Player.RemoveAllGrapplingHooks();
                if (CosJellEscapeCurrent <= 0)
                {
                    CosJellSuffocated = false;
                    Player.immune = true;//probably op
                    Player.immuneTime = 180;
                }
            }
            if ((Player.controlJump && Player.releaseJump))
            {
                CosJellEscapeCurrent--;

                if (CosJellEscapeCurrent <= 0)
                {
                    CosJellEscapeCurrent = 0;
                }
            }

            if (heldItem != Player.inventory[Player.selectedItem].type)
			{
				itemVar = new float[4];
                charge = 0;
                heldItem = Player.inventory[Player.selectedItem].type;
			}
			if (recoilFront > 0f)
				recoilFront -= 0.02f;
			if (recoilBack > 0f)
				recoilBack -= 0.02f;

			necrosis = false;
			soulRot = false;
            melomycosis = false;

			blockChance = 0f;
			dreadBlock = false;

            razedWine = false;

            if (!soulTalisman)
            {
                Player.ClearBuff(ModContent.BuffType<SoulTalismanBuff>());
                soulTalismanStack = 0;
                soulTalismanTally = 0;
            }
            if (!soulTalismanEffect)
            {
                Player.ClearBuff(ModContent.BuffType<SoulTalismanBuff>());
                soulTalismanStack = 0;
            }
            if (!wickedHeart)
            {
                Player.ClearBuff(ModContent.BuffType<WickedHeartBuff>());
                wickedHeartStack = 0;
                wickedHeartStorage = 0;
            }
            if (!wickedHeart)
            {
                Player.ClearBuff(ModContent.BuffType<WickedHeartBuff>());
                wickedHeartStack = 0;
            }
            wickedHeartEffect = false;
            wickedHeart = false;
            soulTalismanEffect = false;
            soulTalisman = false;
            portableLab = false;
						
            setAlloy_Melee = false;
            setAlloy_Ranged = false;
            setAlloy_Magic = false;
        }
		public override void UpdateDead()
        {
            soulTalismanEffect = false;
            soulTalismanTally = 0;
            soulTalismanStack = 0;
            CosJellSuffocated = false;
            CosJellEscapeCurrent = 0;
            if (shakeDuration > 0)
                shakeDuration--; 
            itemVar = new float[4];
		}
        public override void UpdateBadLifeRegen()
        {
			if (necrosis)
			{
				if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
				
				Player.lifeRegenTime = 0;
				 
				Player.lifeRegen -= 10;
			}
			if (soulRot)
			{
				if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
				
				Player.lifeRegenTime = 0;
				 
				Player.lifeRegen -= 20;
			}
            if (melomycosis)
            {
                int defenseCalc = Player.statDefense / 5;
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;

                Player.lifeRegen -= 5 + defenseCalc;
            }
		}
		
		public override void SetControls()
        {
			if (dashTime > 0)
            {				
                Player.controlLeft = false;
                Player.controlRight = false;
                Player.controlJump = false;
                Player.controlDown = false;
                Player.controlUseItem = false;
                Player.controlUseTile = false;
                Player.controlHook = false;
                Player.controlMount = false;
            }
		}

        public override void PostUpdateEquips()
        {
            if (dashTime > 0)
            {
                dashTime--;
                if (dashVelocity != Collision.TileCollision(Player.position, dashVelocity, Player.width, Player.height, true, true, (int)Player.gravDir))
                    dashTime = 0;

                Player.velocity = dashVelocity;
                if (dashVelocity.Y > Player.maxFallSpeed)
                    Player.maxFallSpeed = dashVelocity.Y;

                Player.immune = true;
                Player.itemAnimation = 0;
                Player.itemTime = 0;
                Player.GoingDownWithGrapple = true; // go through platforms

                //dash over
                if (dashTime == 0)
                {
                    Player.velocity *= 0.5f;
                    Player.fallStart = (int)(Player.position.Y / 16f);
                }
            }

            if (setAlloy)
            {
                Player.endurance += 0.02f;
                if (setAlloy_Melee)
                    Player.GetDamage(DamageClass.Melee) += 0.1f;
                if (setAlloy_Ranged)
                    Player.GetDamage(DamageClass.Ranged) += 0.08f;
                if (setAlloy_Magic)
                    Player.GetDamage(DamageClass.Magic) += 0.06f;
            }
            if (razedWine)
            {
                if (razedCooldown > 0)
                {
                    razedCooldown--;
                }
            }
            if (soulTalisman)
            {
                if (soulTalismanStack >= 4)
                {
                    soulTalismanStack = 4;
                }
            }
            if (soulTalismanEffect)
            {
                Player.GetDamage<GenericDamageClass>() += 0.04f * (1 + soulTalismanStack);
                Player.GetAttackSpeed<GenericDamageClass>() += 0.03f * (1 + soulTalismanStack);
                Player.endurance += 0.01f * (1 + soulTalismanStack);
            }
            if (orionsRingEffect)
            {
                Player.GetDamage<GenericDamageClass>() += 0.15f * (1 + orionsRingStack);
            }

            if (wickedHeartEffect)
            {
                Player.GetDamage<GenericDamageClass>() += 0.05f * (1 + wickedHeartStack);
            }
        }
        
        public void BetterScreenshake(int dur, float powerX, float powerY, bool Decay)
        {
            shakeDuration = dur;
            shakeIntensityX = powerX; shakeIntensityY = powerX;
            shakeDecay = Decay;
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (info.PvP && info.DamageSource.TryGetCausingEntity(out Entity entity))
            {
                if (entity is Projectile projectile && projectile.ModProjectile is ITDProjectile itdprojectile)
                {
                    if (projectile.owner == Main.myPlayer)
                        itdprojectile.OnHitPlayer(Player, info);
                }
            }
        }
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (proj.ModProjectile is ITDProjectile itdprojectile)
            {
                itdprojectile.ModifyHitPlayer(Player, ref modifiers);
            }
        }
        public override void PreUpdate()
        {			
            ITDSystem system = ModContent.GetInstance<ITDSystem>();
            ZoneBlueshroomsSurface = system.bluegrassCount > 50 && Player.ZoneOverworldHeight;
            ZoneBlueshroomsUnderground = system.bluegrassCount > 50 && (Player.ZoneDirtLayerHeight || Player.ZoneRockLayerHeight);
            ZoneDeepDesert = system.deepdesertTileCount > 50 && Player.ZoneRockLayerHeight;

            if (!Main.dedServ)
                Player.ManageSpecialBiomeVisuals("BlackMold", melomycosis);

            /*
            foreach (var pairs in ITDSystem.recruitmentData)
            {
                Main.NewText(pairs.Key, Color.Blue);
                Main.NewText(pairs.Value, Color.Yellow);
            }
            */

            UpdateMouse();

            prevTime = curTime;
            curTime = Main.dayTime;

            NaturalSpawns.CosmicJellyfishSpawn(curTime, prevTime, Player);
                //Suffocation Here
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (selectBox)
            {
                Point16 tileCoords16 = MousePosition.ToTileCoordinates16();
                if (selectTopLeft == Point16.Zero)
                    selectTopLeft = tileCoords16;

                selectBottomRight = tileCoords16;
            }
            ProcessCustomToolInput();
            // see if player just right clicked on an ITDNPC to call OnRightClick
            if (Main.mouseRight && Main.mouseRightRelease)
            {
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (npc.Hitbox.Contains(MousePosition.ToPoint()) && npc.ModNPC is ITDNPC itdNPC)
                    {
                        itdNPC.OnRightClick(Player);
                    }
                }
            }
        }
        public void ProcessCustomToolInput()
        {
            if (Player.HeldItem.ModItem is DevTool d)
            {
                d.ProcessInput(Main.keyState);
            }
        }
        public void DrawSelectBox(SpriteBatch sb, Asset<Texture2D> tex)
        {
            if (!selectBox)
                return;
            MiscHelpers.DynamicRectangle(selectTopLeft.ToPoint(), selectBottomRight.ToPoint(), out var slTopLeft, out var slBottomRight);
            Rectangle rect2 = new(slTopLeft.X, slTopLeft.Y, slBottomRight.X + 1 - slTopLeft.X, slBottomRight.Y + 1 - slTopLeft.Y);
            selectBounds = rect2;
            rect2 = rect2.ToWorldRectangle();
            rect2.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
            ITDUIElement.DrawAdjustableBox(sb, tex.Value, rect2, Player.shirtColor);
        }
        public void DrawSpecialPreviews(SpriteBatch sb)
        {
            if (Player.HeldItem.ModItem is DevTool d)
            {
                d.DrawSpecialPreviews(sb, Player);
            }
        }
        public void UpdateMouse()
        {
            if (Player.IsLocalPlayer())
            {
                mousePosPrev = MousePosition;
                MousePosition = Main.MouseWorld;
                if (Main.netMode == NetmodeID.MultiplayerClient && mousePosPrev != MousePosition)
                {
                    NetSystem.SendPacket(new MousePositionPacket(Player));
                }
            }
        }
        public override void ModifyScreenPosition()
        {
            if (shakeDuration > 0)
            {
                float configIntensity = ITD.ClientConfig.ScreenshakeIntensity / 100f;
                if (configIntensity > 0)
                {
                    if (shakeDecay)
                    {
                        shakeIntensityY = float.Lerp(shakeIntensityY, 0, 0.01f) * configIntensity;

                        shakeIntensityX = float.Lerp(shakeIntensityX, 0, 0.01f) * configIntensity;
                    }
                    Main.screenPosition += Main.rand.NextVector2Circular(shakeIntensityX, shakeIntensityY);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (soulTalisman)
            {
                soulTalismanTally++;
                if (soulTalismanTally >= 20)
                {
                    soulTalismanTally = 0;
                    soulTalismanEffect = true;
                    Player.AddBuff(ModContent.BuffType<SoulTalismanBuff>(), 600);
                }
            }

            if (wickedHeart && wickedHeartEffect == false)
            {
                wickedHeartStorage += damageDone;
                if (wickedHeartStorage >= 100)
                {
                    wickedHeartStack += 1;
                    wickedHeartStorage = 0;
                    CombatText.NewText(Player.getRect(), new Color(145, 145, 145), (wickedHeartStack + "/5"), true, false);
                }

                if (wickedHeartStack >= 5)
                {
                    wickedHeartStack = 0;
                    wickedHeartEffect = true;
                    Player.AddBuff(ModContent.BuffType<WickedHeartBuff>(), 1200);
                }
            }
        }

        public override void PostUpdate()
        {
            if (wickedHeartEffect && wickedHeartCooling >= 1)
            {
                wickedHeartCooling -= 1;
                if (wickedHeartCooling <= 0)
                {
                    wickedHeartCooling = 0;
                }
            }
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (wickedHeart)
            {
                if (Player.HeldItem.DamageType == DamageClass.Ranged && wickedHeartEffect && wickedHeartCooling< 1)
                {
                    Projectile rangedAttack = Projectile.NewProjectileDirect(source, position, velocity,
                    ModContent.ProjectileType<WickedHeartR>(), (int)(damage * 2.5), knockback, Player.whoAmI);

                    wickedHeartCooling = 30;

                    return false;
                }
                else
                {
                    wickedHeartCooling -= 1;
                    if (wickedHeartCooling <= 0)
                    {
                        wickedHeartCooling = 0;
                    }

                    return true;
                }
            }

            return true;
        }

        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            if (wickedHeart)
            {
                if (Player.HeldItem.DamageType == DamageClass.Melee && wickedHeartEffect && wickedHeartCooling < 1)
                {
                    Vector2 mousePosition = Main.MouseWorld;
                    Vector2 direction = mousePosition - Player.position;
                    direction.Normalize();
                    float speed = 6f;

                    Vector2 shootTowardsMouse = direction * speed;
                    Projectile meleeAttack = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.position, shootTowardsMouse,
                    ModContent.ProjectileType<WickedHeartMelee>(), (int)(Player.HeldItem.damage * 0.75), 3f, Player.whoAmI);

                    wickedHeartCooling = 66;
                }
            }
        }

        public override void OnConsumeMana(Item item, int manaConsumed)
        {
            if (Player.HeldItem.DamageType == DamageClass.Magic && wickedHeartEffect && wickedHeartCooling < 1)
            {
                Vector2 mousePosition = Main.MouseWorld;
                Vector2 direction = mousePosition - Player.position;
                direction.Normalize();
                float speed = 6f;
                Vector2 shootTowardsMouse = direction * speed;

                Projectile magicAttack = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, shootTowardsMouse,
                ModContent.ProjectileType<WickedHeartM>(), (int)(Player.HeldItem.damage * 0.33), 0f, Player.whoAmI);

                wickedHeartCooling = 90;
            }
        }

        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (wickedHeart)
            {
                if (Player.HeldItem.DamageType == DamageClass.Summon && wickedHeartCooling < 1)
                {
                    Projectile summonAttack = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), victim.Center, Vector2.Zero,
                    ModContent.ProjectileType<WickedHeartS>(), (int)(Player.HeldItem.damage * 0.5), 0f, Player.whoAmI);

                    wickedHeartCooling = 90;
                }
            }
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
			if (modifiers.Dodgeable && Main.rand.NextFloat(1f) < blockChance) // Chance to block attacks
			{
				SoundEngine.PlaySound(SoundID.NPCHit4, Player.Center);
				modifiers.DisableSound();
				modifiers.SetMaxDamage(1);
				if (dreadBlock) // Dread Shell block ability
				{
					for (int i = 0; i < Main.maxNPCs; i++)
					{
					NPC target = Main.npc[i];
						if (target.CanBeChasedBy() && target.Distance(Player.Center) < 200)
						{
							int damage = Main.DamageVar(200, Player.luck);
							target.StrikeNPC(new NPC.HitInfo
							{
								Damage = damage,
								Knockback = 2f,
								HitDirection = target.Center.X < Player.Center.X ? -1 : 1,
								Crit = false
							});
							target.AddBuff(BuffID.Confused, 300, false);
						}
					}
					for (int i = 0; i < 60; i++)
					{
						Vector2 offset = new Vector2();
						double angle = Main.rand.NextDouble() * 2d * Math.PI;
						offset.X += (float)(Math.Sin(angle) * 200);
						offset.Y += (float)(Math.Cos(angle) * 200);
						Vector2 spawnPos = Player.Center + offset - new Vector2(4, 0);
						Dust dust = Main.dust[Dust.NewDust(
							spawnPos, 0, 0,
							DustID.LifeDrain, 0, 0, 100, default, 1.5f
							)];
						dust.noGravity = true;
					}
				}
			}
            if (soulTalismanEffect)
            {
                BetterScreenshake(10, 5, 5, true);
                Projectile Blast = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
        ModContent.ProjectileType<SoulTalismanBlast>(), (int)Player.GetDamage(DamageClass.Generic).ApplyTo(80 * (soulTalismanStack + 1)), 20f, Player.whoAmI);
                Blast.ai[1] = 80 * (soulTalismanStack + 1);
                Blast.localAI[1] = Main.rand.NextFloat(0.1f, 0.3f);
                Blast.netUpdate = true;
                soulTalismanEffect = false;
            }
		}
		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
			if (damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                if (necrosis)
                    damageSource = Player.DeathByLocalization("Necrosis");
				if (soulRot)
                    damageSource = Player.DeathByLocalization("SoulRot");
                if (melomycosis)
                    damageSource = Player.DeathByLocalization("Melomycosis");
            }
			return true;
		}
        public NPC[] GetNearbyNPCs(float distance, bool ignoreFriendly = true)
        {
            List<NPC> npcs = [];
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.WithinRange(Player.Center, distance))
                {
                    if (!npc.CanBeChasedBy() && ignoreFriendly)
                        continue;
                    npcs.Add(npc);
                }
            }
            return [.. npcs];
        }
        public override void SaveData(TagCompound tag)
        {
            tag["ITD:playerGuid"] = guid.ToByteArray();
        }
        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("ITD:playerGuid"))
                guid = new Guid(tag.GetByteArray("ITD:playerGuid"));
            // idk if it's possible for a guid to be generated with all zeros or what the chance for that is but uhhhh
            while (guid == Guid.Empty)
                guid = Guid.NewGuid();
        }
        public override void OnEnterWorld()
        {
            // for good measure
            while (guid == Guid.Empty)
                guid = Guid.NewGuid();
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetSystem.SendPacket(new PlayerJoinedPacket(Player));
            }
        }
        private sealed class ITDPlayerSystem : ModSystem // there's no OnExitWorld hook :death:
        {
            public override void PreSaveAndQuit()
            {
                Player player = Main.CurrentPlayer;
                NaturalSpawns.LeaveWorld();
                PhysicsMethods.ClearAll();
            }
            public override void PostDrawTiles()
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                Asset<Texture2D> tex = ModContent.Request<Texture2D>("ITD/Content/SelectBox");
                //Main.spriteBatch.Draw(tex.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
                foreach (var plr in Main.ActivePlayers)
                {
                    plr.GetITDPlayer().DrawSelectBox(Main.spriteBatch, tex);
                }
                Player p = Main.LocalPlayer;
                p.GetITDPlayer().DrawSpecialPreviews(Main.spriteBatch);
                Main.spriteBatch.End();
            }
        }
		public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (heldItem == ModContent.ItemType<WormholeRipper>())
            {
                if (itemVar[0] > 0)
				{
					int dustType = ModContent.DustType<StarlitDust>();
					if (itemVar[0] == 3)
                    {
                        dustType = 204;
                    }
					int dust = Dust.NewDust(Player.MountedCenter - new Vector2(4f, 40f), 0, 0, dustType, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1f + itemVar[0] * 0.5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity.X = 0f;
					Main.dust[dust].velocity.Y = -3f;
					drawInfo.DustCache.Add(dust);
				}
            }
			
			if (necrosis)
			{
				r = 0.7f;
				g = 0.4f;
				b = 0.9f;
			}
			
			if (soulRot)
			{
				r = 0.4f;
				g = 0.7f;
				b = 0.9f;
			}
        }
    }
}
