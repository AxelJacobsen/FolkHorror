using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passiveitem : Item
{
    // Public vars
    public AlterStats alterStats;
    
    void Start()
    {
        base.Start();

		/*
		List<AlterStats> changeList = new List<AlterStats>();

		changeList.Add(c =>
		{
			c.Speed -= 3f;
			c.MaxHealth += 5;
			return c;
		}
		);

		changeList.Add(c =>
		{
			c.Speed *= 2f;
			c.MaxHealth /= 2;
			return c;
		}
		);

		CharacterStats newStats = CalculateStats(changeList);
		CharacterStats changed = newStats.CalculateDiff(this);

		print(newStats.Speed);
		print(newStats.MaxHealth);
		print(changed.Speed);
		print(changed.MaxHealth);*/
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
