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
#### What i consider to be bad
- I want to improve upon the selection of the item pool in the shop, now it just gets a specified random amount of items from the shop, however i want to implement a probability table, where it gives better options the further along the maps you come. The chance of getting rarer items should increase based on which level you are in. And a guaranteed stat item in addition to weapon item.
- The Shop UI, it is very basic and needs improvement.
<br>![](https://github.com/AxelJacobsen/FolkHorror/blob/main/Reports/Images/daniel_shop_example.png)<br>
*Current Shop UI*
- The item system, if i could remake something I would probably want to implement wider scale systems for items if i could.
- The custom sprites i've made for the game are something i enjoyed making, however it took too much time in addition to other projects from other courses. And it was very time consuming making a different animation based on character orientation and form depiction. If i could do this again i would focus on implementation before asset creation.
#### Reflection
Originally i was in charge of assets however i quickly found out that a lot of the assets i made were probably not going to be used the way they were intended to be used, and the production of such a quantity of assets would require a lot more time than originally planned. Which is why i was put in charge of other parts of the game instead, seeing as we needed such things to be implemented. 
I learned in coalition with other classes how to organize and be better at merging, getting things done and sprint meetings. I gained better mastery at using GIT and SourceTree. 
I learned that by using my time smartly i could implement features at a satisfactory speed, and i enjoyed doing things other than drawing assets. Using Unity as a game engine worked out great, except the parts where the Build version was vastly different to the one compiled in Unity. Bugs that were not there suddely appeared and they had to be fixed. Our group was very good at communicating what we needed from each other and how to help each other. This made my experience with the course very much enjoyable. I only regret not working on implementation before asset creation.

