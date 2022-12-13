# Individual Report - Daniel
### Score weighting
|Description | My Weight |
|----|----|
|Gameplay video | 10 |
|Code video | 0 |
|Good Code  | 25 |
|Bad Code | 20 |
|Development process | 20 |
|Reflection | 20 |
#### What i've learned
- I have learned how to code in C#, and how to translate my existing knowledge to this language.
- I learned key concepts related to UI design and anchoring. Adding components based on prefabs and how to communicate through scenes using tags.
- I also learned how to make key scripts related to managing shop items and sound components for the game.
- i have learned how to add simple elements to the UI, such as a Health bar and a Coin amount holder.
- I have also learned that using one single canvas for all UI is considered bad practice. You should consider elements that are dynamic and static when using canvas.
<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_ui_structure.png)<br>
*How our UI is currently setup in NeverUnload.Unity*
- I have also learned that audioSources have a max allowed played sounds, therefore a handler for audioSources has to be implemented in order to manage sound.
- I have also learned that assets such as character assets and such should be the last priority, this is because the major systems related to the assets are not developed yet, and should not be developed around existing assets, the assets should be created preferably for the system itself.

#### What i consider to be good
- I consider the implementation of SoundManager and ShopManager to be good
	- SoundManager is very modular and supports the implementation of sound settings sliders when needed.
		- Has handling for when you want to play sound in 3d space or when you want to play sounds when handling UI.
<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_soundmanager_example.png)<br>
*Snippet from ShopManager.cs Script, it shows an example of how you would play a sound for UI* 
- ShopManager is simple and very customizable.
- I consider the implementation of currency handling to be good aswell.
	- very simple and clean.
	- When it comes to the code quality, I've read Clean Code by Robert C. Martin, and one of his suggestions is to reduce large complex if statements into multiple variables with names according to their use case.
<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_coinhandler.png)<br>
*Snippet from PlayerController.cs Script, it shows the simple implementaion of incrementing, removing and getting the amount of coins stored in the PlayerController Script* 
- The conceptual planning of the product.
	- Concept scene(note that this was before we began coding)
	<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_concept_art.jpg)<br>
	*Shows planned implementation of how a character might look like in an example scene*
	- Color overlay
	<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_grass_overlay_concept.png)<br>
	*Shows planned implementation of how the color overlay might look like using my grass texture*
	<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_grass_light_overlay_concept.png)<br>
	*Shows planned implementation of how the color overlay might look like using my grass texture, with a lightoverlay*
	<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_biome_concept_grass.jpg)<br>
	*Shows planned implementation of how the color overlay might look like using my grass texture, with a different biome*
	<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_biome_concept_grass_2.jpg)<br>
	*Shows planned implementation of how the color overlay might look like using my grass texture, with a different biome*
- The custom assets. I'm a little proud as its the first assets ive made.
<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_idle_back.png)<br>
<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_idle_front.png)<br>
<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_side_left-sheet.png)<br>
<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_side_right-sheet.png)<br>
*All character idle animations.*
<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_heartIcon.png)<br>
<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_heartSingle.png)<br>
*Heart icons*
<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_border.png)<br>
*Frames for HP and items*
<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_boreal.png)<br>
*Custom trees, meant to be used with a color overlay*
<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_grass.png)<br>
*Grass texture, meant to be used with a color overlay*
<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/portal.png)<br>
*Portal sprite, replaces old placeholder*
<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/bush.png)<br>
*Bush sprite, replaces old placeholder*
<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/ItemPng.png)<br>
*Shop sprite, replaces old placeholder*
#### What i consider to be bad
- I want to improve upon the selection of the item pool in the shop, now it just gets a specified random amount of items from the shop, however i want to implement a probability table, where it gives better options the further along the maps you come. The chance of getting rarer items should increase based on which level you are in. And a guaranteed stat item in addition to weapon item.
- The Shop UI, it is very basic and needs improvement.
<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_shop_example.png)<br>
*Current Shop UI*
- The item system, if i could remake something I would probably want to implement wider scale systems for items if i could.
- The custom sprites i've made for the game are something i enjoyed making, however it took too much time in addition to other projects from other courses. And it was very time consuming making a different animation based on character orientation and form depiction. If i could do this again i would focus on implementation before asset creation.
#### Reflection
I was originally in charge of assets, however i quickly noticed how much time it would take to implement an abstract amount of assets for an abstract concept which was not yet implemented. By the time i had started implementing assets i quickly noticed that the planned functionality of our game was vastly different from what was actually implemented, in the form of simplifying what we originally wanted to implement. This means that we set the bar way too high in the beginning, where we wanted to implement features before we layed out the groundwork.
> What i would do differently on this part the next time i would take on such a project, would be to downsize out scope first, sometimes simpler is better and by focusing the scope we can achieve greater efficiency. 
After i was put in charge of other things than assets i quickly noticed that the workflow improved seeing as the implementation of a defined needed functionality flowed better than implementing abstract assets. 
> I realized that the implementation of assets should be dragged out to the later stages of development, the reason being that you should lay out the groundwork of the game first, then add more and more to it later.

What would be helpful to do in the very beginning would to lay out a development phase/stage diagram where we would split the development into different stages, sort of like a roadmap for devs.
> first lay out the groundwork of the application, setup systems that are required such as map creation, enemy spawning, player systems, item systems, etc. Then do the auxillary systems, Sound system, Shop system, Dialogue and etc. Lastly add all the extra implementations such as Bosses, special items, special interractions, randomized interractions, custom assets, etc. Planning in such a way would make the workflow better.

I am very happy with how things went, because i learned valuable lessions during this project, i realize that you can be a lot more effective if you preplan everything into stages of development, and if you simplyfy and abstract systems, you may expand upon them later when you need it. It is smart to plan for everything, however with such a strict schedule as the one we had, we should have taken things to consideration.
The group i worked with were very fast at their implementations, however i also realized that working on multiple projects in parallel took quite a toll the development time aswell. Therefore as i've stated earlier a roadmap would be very useful for realizing where we need to be compared to where we are.

