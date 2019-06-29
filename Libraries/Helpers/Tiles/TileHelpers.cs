using System;
using Terraria;


namespace MonsterAlertness.Libraries.Helpers.Tiles {
	public class TileHelpers {
		public static bool IsTileSolid( Tile tile, bool isPlatformSolid = false, bool isActuatedSolid = false ) {
			if( tile == null || !tile.active() ) { return false; }
			if( !Main.tileSolid[tile.type] ) { return false; }

			bool isTopSolid = Main.tileSolidTop[tile.type];
			bool isPassable = tile.inActive();

			if( !isPlatformSolid && isTopSolid ) { return false; }
			if( !isActuatedSolid && isPassable ) { return false; }

			return true;
		}
	}
}
