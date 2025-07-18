﻿using Terraria.Audio;
using System;

using ITD.Content.Projectiles.Friendly.Misc;

namespace ITD.Content.Items.Accessories.Movement.Jumps
{
    public class StormInABottle : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 24;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Gray;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<StormPlayer>().hasStormJump = true;
        }
    }

    public class StormPlayer : ModPlayer
    {
        public const int TornadoDuration = 180;
        public const int LightningInterval = 60;
        public const float TornadoHeight = 200f;
        public const float MaxLightningDistance = 300f;

        public bool hasStormJump;
        public bool canDoubleJump;
        public bool waitDoubleJump;
        public int tornadoTimer;
        public int lightningTimer;
        public Vector2 tornadoBase;

        public override void ResetEffects() => hasStormJump = false;

        public override void PreUpdate()
        {
            if (!hasStormJump)
            {
                canDoubleJump = false;
            }

            HandleDoubleJump();
            UpdateTimers();
        }

        private void HandleDoubleJump()
        {
            if ((Player.velocity.Y == 0f || Player.sliding || Player.autoJump && Player.justJumped) && hasStormJump)
            {
                canDoubleJump = true;
                waitDoubleJump = true;
            }
            else if (canDoubleJump && Player.controlJump && !waitDoubleJump)
            {
                PerformDoubleJump();
            }

            waitDoubleJump = Player.controlJump;
        }

        private void PerformDoubleJump()
        {
            if (Player.jump > 0) return;

            Player.velocity.Y = -Player.jumpSpeed * 2f * Player.gravDir;
            Player.jump = (int)(Player.jumpHeight * 2f);
            canDoubleJump = false;

            SoundEngine.PlaySound(new SoundStyle($"ITD/Content/Sounds/StormInABottle{Main.rand.Next(1, 3)}"), Player.position);
            InitializeTornado();
        }

        private void InitializeTornado()
        {
            tornadoTimer = TornadoDuration;
            lightningTimer = LightningInterval;
            tornadoBase = Player.Bottom + new Vector2(0, 40);
            AdjustTornadoBaseToGround();
        }

        public void AdjustTornadoBaseToGround()
        {
            for (int i = 0; i < 20; i++)
            {
                Point tilePt = (tornadoBase + new Vector2(0, i * 16)).ToTileCoordinates();
                if (Main.tile[tilePt.X, tilePt.Y].HasTile && Main.tileSolid[Main.tile[tilePt.X, tilePt.Y].TileType])
                {
                    tornadoBase.Y = tilePt.Y * 16 - 8;
                    break;
                }
            }
        }

        private void UpdateTimers()
        {
            if (tornadoTimer > 0)
            {
                tornadoTimer--;
                Tornado();

                if (Main.myPlayer == Player.whoAmI && --lightningTimer <= 0)
                {
                    Lightning();
                    lightningTimer = LightningInterval;
                }
            }
            else
            {
                lightningTimer = 0;
            }
        }

        private void Tornado()
        {
            float progress = (float)tornadoTimer / TornadoDuration;
            for (int i = 0; i < 20; i++)
            {
                CreateTornadoDust(progress);
            }
        }

        private void CreateTornadoDust(float progress)
        {
            float heightProgress = Main.rand.NextFloat();
            float angle = Main.rand.NextFloat() * MathHelper.TwoPi;
            float radius = MathHelper.Lerp(10f, 100f, heightProgress) * progress;
            float height = TornadoHeight * heightProgress * progress;

            Vector2 offset = new Vector2((float)Math.Cos(angle) * radius, -height);
            Vector2 position = tornadoBase + offset;
            Vector2 velocity = new Vector2(-offset.X * 0.05f, Main.rand.NextFloat(-1f, 1f));

            if (Main.rand.NextFloat() < 0.01f)
            {
                CreateElectricSpark(position, velocity, progress);
            }
            else
            {
                CreateSmokeDust(position, velocity, progress);
            }
        }

        private void CreateElectricSpark(Vector2 position, Vector2 velocity, float progress)
        {
            Dust spark = Dust.NewDustPerfect(
                position,
                DustID.Electric,
                velocity * 2f,
                0,
                Color.White,
                Main.rand.NextFloat(0.5f, 1f) * progress
            );

            spark.noGravity = true;
            spark.fadeIn = 0.5f;
        }

        private void CreateSmokeDust(Vector2 position, Vector2 velocity, float progress)
        {
            Color dustColor = Color.Lerp(Color.DarkGray, Color.Gray, Main.rand.NextFloat() * 0.3f);

            Dust dust = Dust.NewDustPerfect(
                position,
                DustID.Smoke,
                velocity,
                0,
                dustColor * progress,
                Main.rand.NextFloat(1.5f, 2.5f) * progress
            );

            dust.noGravity = true;
            dust.fadeIn = Main.rand.NextFloat(0.8f, 1.2f) * progress;
        }

        private void Lightning()
        {
            float progress = (float)tornadoTimer / TornadoDuration;
            Vector2 lightningStart = CalculateLightningStart(progress);

			NPC newTarget = null;
			float reach = 600;
			
			foreach (var npc in Main.ActiveNPCs)
			{
				if (!npc.friendly && npc.CanBeChasedBy())
				{
					float distance = Vector2.Distance(npc.Center, lightningStart);
					if (distance < reach)
					{
						reach = distance;
						newTarget = npc;
					}
				}
			}
			if (newTarget != null)
			{
				Projectile newZap = Main.projectile[Projectile.NewProjectile(Player.GetSource_FromThis(), newTarget.Center, new Vector2(), ModContent.ProjectileType<Zap>(), 300, 0, Player.whoAmI, newTarget.whoAmI, lightningStart.X, lightningStart.Y)];
				newZap.scale = 1.33f;
			}
        }

        private Vector2 CalculateLightningStart(float progress)
        {
            float heightProgress = Main.rand.NextFloat();
            float angle = Main.rand.NextFloat() * MathHelper.TwoPi;
            float radius = MathHelper.Lerp(10f, MathHelper.Lerp(10f, 100f, progress), heightProgress);
            float height = TornadoHeight * progress * heightProgress;

            return tornadoBase + new Vector2((float)Math.Cos(angle) * radius, -height);
        }
    }
}