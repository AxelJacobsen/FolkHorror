## Score weighting
|Description | Weight |
|----|----|
|Gameplay video | 15 |
|Code video | 0 |
|Good Code  | 30 |
|Bad Code | 30 |
|Development process | 15 |
|Reflection | 10 |

## Good code
### Flexible AI
![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Sequence%2001.gif)<br>
*Trolls fighting amongst each other*<br>
When creating the enemy AI I made it as flexible as I could. I wanted enemies to be able to fight each other, utilize any weapon or item, and behave uniquely. For this I made one base class called [BaseEnemy](../Assets/Scripts/Characters/Enemies/BaseEnemyAI.cs), and three (four, including the boss) archetypes that inherited from the base class:<br>
- [Red Troll](../Assets/Scripts/Characters/Enemies/RedTrollAI.cs): This is your melee enemy. It stampedes towards its enemies, pushing them back and dealing damage.
- [Green Troll](../Assets/Scripts/Characters/Enemies/GreenTrollAI.cs): This is your ranged enemy. It shoots at its enemies from afar and runs away if they get too close, but it can only run for a couple of seconds before getting tired and stopping.
- [Nisse](../Assets/Scripts/Characters/Enemies/NisseAI.cs): This is your "annoying" enemy. It walks around until it finds an enemy, at which point it starts screaming and running for its allies, alerting them. However, it only tells its allies where it *last* saw someone, so in the meantime they may have moved.

![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Sequence%2001_1.gif)<br>
*A "Nisse" alerting other enemies about the player's presence*<br>

![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Pasted%20image%2020221212225348.png)<br>
All enemies have four states (shown in the picture above), but the way they act in these states varies wildly. This is achieved through virtualizing their behavioral functions.<br>

![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Pasted%20image%2020221212225958.png)<br>
*Aggressive behavior of the Red Troll. [RedTrollAI.cs](../Assets/Scripts/Characters/Enemies/RedTrollAI.cs), line 24-68.*<br>

An enemy can be charmed, turning it into an ally for the charmer. Enemies can even charm each other if they have the items for it. Enemies can equip any item or weapon the player can use.<br>

### Passive items
A passive item is an item that applies its effects automatically. Passive items can alter a character's stats in any way and have triggers that are called when their user does something specific.<br>
![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Pasted%20image%2020221212230945.png)<br>
*The triggers an item can use. [Item.cs](../Assets/Scripts/Items/Item.cs), line 166-169.*<br>

Creating a new passive item with the system I made is very easy. The code below shows the *entire* script for an item that charms opponents, doubles the users speed, and halves their maximum health.<br>
![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Pasted%20image%2020221212231342.png)<br>

Stats are calculated as a stateless function, meaning if you equip and drop an item you will have the same stats before and after. <br>

### Weapon modularity
![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Sequence%2001_2.gif)<br>
*An arrow bouncing on two enemies*<br>

![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Sequence%2001_3.gif)<br>
*An arrow chaining between multiple enemies*<br>

![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Sequence%2001_4.gif)<br>
*An arrow piercing two enemies, but no more*<br>

![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Pasted%20image%2020221213003210.png)<br>
Projectiles can bounce, chain and pierce - any number of times for each effect, with -1 being infinite. The code above is located in [SimpleProjectile.cs](../Assets/Scripts/Projectiles/SimpleProjectile.cs), line 88-128.<br>

## Bad code
### CharacterStats
Occam's razor states that:<br>
>"Entities should not be multiplied beyond necessity"
>(Barry, C. M. (27 May 2014). ["Who sharpened Occam's Razor?"](https://www.irishphilosophy.com/2014/05/27/who-sharpened-occams-razor/). _Irish Philosophy_.)

Which is a useful principle to keep in mind when creating code for games. Unnecessarily complex code often clutters the project and pulls focus away from more important aspects, such as creating code that works. When creating the code for character stats I ended up putting way too much effort into making it general, which in the end was not worth the time, concidering these are the only two stats we ended up using:<br>
![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Pasted%20image%2020221213004717.png])<br>

I wanted items to be able to alter character stats in any way, not just by adding/subtracting/multiplying/dividing, so to represent a *change* in stats I made a delegate, pictured below.<br>
![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Pasted%20image%2020221213005229.png)<br>

To handle a whole list of items altering a character's stats, I made a separate function. The reasoning for this was to keep character stats *stateless*.<br>
![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Pasted%20image%2020221213005336.png)<br>

At this point I wanted to compare two statblocks in case we were going to display how picking up an item would change the player's stats, which we never actually ended up doing. Still, to do this I had to compare every field in the class, so I used reflection to fetch all serializable, non-inherited, public variables.<br>
![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Pasted%20image%2020221213005824.png)<br>

Since I was already going to subtract every stat from the corresponding stat in a different stat-block when doing this, I thought I'd make it more general by allowing for *any* mathematical operation. The resulting function was too general and difficult to name, imaged below. It also had to take into concideration that some fields would be integers, not floats.<br>
![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Pasted%20image%2020221213011027.png)<br>

To make the class easier to use I added two shorthands for adding and subtracting statblocks, and since I already was doing all of this complicated stuff I decided to use operator overloading instead of just calling them Add() and Subtract().<br>
![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Pasted%20image%2020221213011152.png)<br>

In the end none of the *four last functions* imaged was used, but atleast I learned a lot about C#...<br>

### EffectEmitter
A central part of game creation when using an engine is knowing its features. When I was creating Effects such as bleed, stun, charm, etc. I wanted particles to spring out of the affected character. These particles would behave differently. Some would have a starting velocity, some would spawn on the ground beneath the character, some would spin in circles. Unity has an in-built particle system, but I did not know this at the time, and ended up creating my own similar system.<br>

![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Pasted%20image%2020221213020939.png)<br>
*The fields used by EffectEmitter. [EffectEmitter.cs](../Assets/Scripts/Effects/EffectEmitter.cs), line 6-34.*<br>

![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Pasted%20image%2020221213022247.png)<br>
*The fields used by [Unity's Particle](https://docs.unity3d.com/ScriptReference/ParticleSystem.Particle.html). Notice the similarities with EffectEmitter, marked with red.*<br>

In creating this system, I effectively remade what Unity already had built-in. This did, however, allow for some interesting effects.<br>
![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Sequence%2001_6.gif)<br>
*Stars circling around the character indicates that it is stunned.*<br>

## Reflection
### Learned
During the project I have gained a lot of experience doing remote group work. As our main communication medium we used Discord, this worked great as it allows for sharing your screen, having dedicated chat- and/or voice channels, and much more. Each week we held a scrum meeting where we categorized Git issues, marking them as "Backlog", "In progress", "Deadline X", "In review", or "Done", and merged our branches.<br>
![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Pasted%20image%2020221213030320.png)<br>
*Our Git issue board at the end of the project.*<br>

In addition I have learned to use the Unity documentation much more effectively, and intend to keep creating games in Unity as a hobby. I do, however, have a lot of things I would do different if I were to do this project again. <br>

### What I would do different
To start off I would set my expectations lower. Having ambition is good, but it's important to set an accurate scope when working with a limited timespan. Otherwise it gets difficult to distribute time, and ultimately reach the very goals you're aiming for. I would also make a roadmap, since during this project I felt it was difficult to estimate how far off from *finished* we were. With a roadmap it's also easier to segment the project, in turn making it more clear where challenges lie.<br>

### Testing
Testing was done manually before each merge. Almost every pull request was reviewed by another person before merged. <br>
![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/Pasted%20image%2020221213041743.png)<br>
