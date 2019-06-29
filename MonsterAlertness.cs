using Microsoft.Xna.Framework;
using MonsterAlertness.Libraries.Helpers.Tiles;
using Terraria;
using Terraria.ModLoader;


namespace MonsterAlertness {
	class MonsterAlertnessMod : Mod {
		public MonsterAlertnessMod() {
		}
	}




	class MonsterAlertnessPlayer : ModPlayer {
		public bool IsLit = false;



		////////////////

		public override bool CloneNewInstances => false;



		////////////////

		public override void DrawEffects( PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright ) {
			this.IsLit = r >= 0.125f && g >= 0.125f && b >= 0.125f;
		}
	}




	class MonsterAlertnessNPC : GlobalNPC {
		public static int SeesPlayer( NPC npc ) {
			for( int i=0; i<Main.player.Length; i++ ) {
				Player plr = Main.player[i];
				if( plr == null || !plr.active ) { continue; }

				if( !plr.dead ) {
					var myplayer = Main.LocalPlayer.GetModPlayer<MonsterAlertnessPlayer>();
					if( !myplayer.IsLit ) {
						continue;
					}

					bool sees = Utils.PlotTileLine( npc.Center, Main.LocalPlayer.Center, 1f, (x, y) => {
						return !TileHelpers.IsTileSolid( Main.tile[x, y] );
					} );
					if( sees ) {
						return i;
					}
				}

				if( Main.netMode == 0 ) { break; }
			}

			return -1;
		}



		////////////////

		private int OldAiStyle = -1;
		private int SeesPlayerWho = -1;
		private bool IsLit = false;



		////////////////

		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;



		////////////////

		public override void SetDefaults( NPC npc ) {
			if( npc.friendly ) { return; }
			if( npc.noTileCollide ) { return; }
			if( npc.boss ) { return; }

			this.OldAiStyle = npc.aiStyle;
			npc.aiStyle = 0;
		}


		////////////////

		public override bool PreAI( NPC npc ) {
			if( this.OldAiStyle == -1 ) {
				return base.PreAI( npc );
			}

			if( this.IsLit ) {
				this.SeesPlayerWho = MonsterAlertnessNPC.SeesPlayer( npc );

				if( this.SeesPlayerWho != -1 ) {
					npc.aiStyle = this.OldAiStyle;
					this.OldAiStyle = -1;
				}
			}

			return base.PreAI( npc );
		}


		////////////////

		public override void DrawEffects( NPC npc, ref Color drawColor ) {
			this.IsLit = drawColor.R >= 32 && drawColor.G >= 32 && drawColor.B >= 32;
		}
	}
}
