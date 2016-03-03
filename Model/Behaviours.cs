using System.Collections.Generic;

public class Behaviours
{
	public static float GetCostByType (Furniture.TYPE type)
	{
		switch (type) {
			case Furniture.TYPE.PROCESSOR: return 10f;
			case Furniture.TYPE.HOPPER: return 10f;
			case Furniture.TYPE.DEPOT: return 10f;
			case Furniture.TYPE.MINER: return 10f;
			case Furniture.TYPE.TERMINAL: return 10f;
			case Furniture.TYPE.WALL: return 2f;
			case Furniture.TYPE.NONE: return 0;
		}
		return 1f;
	}
}
/*s
	public class FurnitureBehaviour
	{
		public static float cost = 10f;
		protected float coolDown;
		protected Tile tile;

		public FurnitureBehaviour (Tile tile)
		{
			this.tile = tile;
			SetCoolDown();
			if (class.hasMethod ("Trigger"))  {
				tile.world.registerOnTick(Trigger);
			}
		}

		protected void SetCoolDown ()
		{
			coolDown = UnityEngine.Random.Range(5f, 6f); // reset cooldown
		}
	}

	public class Miner : FurnitureBehaviour
	{
		public static float cost = 10f;

		// This must be here because just be cause.. dammit C# is lame

		void Trigger (float time)
		{
			coolDown -= time;
			if (coolDown <= 0) {
				Job job = tile.world.createCustomJobAt(tile.X, tile.Y, 5f, Job.TYPE.MINE_WORK);
				job.registerOnCompleteCallback((Job j) => {
					//Debug.Log("Complete!");
				});
			}
		}
	}

	public class Terminal : FurnitureBehaviour
	{
		public static float cost = 10f;

		// This must be here because just be cause.. dammit C# is lame
		public Terminal (Tile t) : base(t) { }
		void Trigger (float time)
		{
			coolDown -= time;
			if (coolDown <= 0) {
				// Terminal has triggered, lets create a job to force a worker to move here
				tile.world.createCustomJobAt(tile.X, tile.Y, 5f, Job.TYPE.TERMINAL_WORK);
				SetCoolDown();
			}
		}
	}
*/