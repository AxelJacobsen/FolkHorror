## Weighting
|Description        |Score|
|-------------------|-----|
|Gameplay video     |10   |
|Code video         |5    |
|Good code          |25   |
|Bad code           |30   |
|Development process|25   |
|Reflection         |25   |

## Good code
	Map material

## Bad Code

	MoveObject
		Brute force
	Load from binary

## Reflection
**Initial thoughts**
When entering this project i was super excited to work with it. It was fresh to see results with every change, and interesting to learn about Unity's built in functionality. After settling upon an idea

**The process**
Once development started it went quite smoothly. We had weekly meetings and progress was good. Since i had started with a relatively complete product as base, a significant portion of early work went to understanding the code so that i could alter it more efficiently later. Some red flags showed up here with how cumbersome the code was to manouver. However, i had a clear idea in my mind on how i wanted to result to look. I managed to implement a system for seperating sections of the map into seperate meshes, and then turn those meshes into a group of objects. When this was complete things started to bog down. Due to other projects delivery dates being stacked against us, we couldnt hold several of the planned meetings for some time, and individual progress was slow. 

We had a grace period of a few days before MVP submition which let us knit out seperate pieces together. However, i was growing dissatisfied with what i had created. With my code becoming increasingly tangled with old code unintended for this program the, map generator was showing cracks. It performed fine, but every small change required a cascade of small alterations to ensure everything was running smothly. The biggest issue i faced, was portal spawning (later object spawning). Placing two points on a big square is easy. Placing two points in a polygon is cumbersome, but not too hard. Placing two points in polygon, in seperate corners randomized each time while avoiding spawning too close to an enemy or inside a tree however. Turned out to be a formidable challenge. This demanded significant time when i had none, and showed little results leading to long and frustrating nights. Nonetheless, the hill was climbed, and the game worked as intended. 

Personally, i found Unity decent to work with. It has many quirks that i was not aware of when starting out, but was great for quickly developing a simple, working game. 

**In Hindsight**
