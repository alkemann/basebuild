using UnityEngine;
using System.Collections.Generic;
using System;

public class Astroid
{
	public Tile tile { get; protected set; }

	float resources_left = 100f;
	Action<Astroid> cbOnChange;

	public Astroid (Tile tile)
	{
		this.tile = tile;
	}

	public float mine (float amount)
	{
		float mined = Mathf.Max(amount, resources_left);
		this.resources_left -= mined;
		return mined;
	}

	public void RegisterOnChange (Action<Astroid> cb)
	{
		cbOnChange += cb;
	}
}

