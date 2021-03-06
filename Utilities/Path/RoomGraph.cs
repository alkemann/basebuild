﻿using UnityEngine;
using System.Collections.Generic;

// Placeholder pending a model
class Room
{

}

namespace Path {
	class RoomGraph : Graphs<Room>
	{
		Dictionary<Room, Node<Room>> nodes;

		public RoomGraph(World world)
		{
			nodes = new Dictionary<Room, Node<Room>> ();
		}

		public override Stack<Room> search (Room from, Room target)
		{
			
			return aStarSearch(
				nodes,
				from,
				target,
				(Room one, Room two) => {
					return 1;
				}
			);
		}
	}
}
