# Hybrid Crops
An expansion to Stardew Valley that adds the ability to create hybrid crops.

## Devlog
### 1.0.0
9/17/2022
  * Added extra details that were never uploaded to GitHub.

9/7/2021
  * Added more crop trait ideas.

8/28/2021
  * Reworked hybrid success chance formula. Seeds no longer have an impact on success rate, as they just allow you to get more seeds as output. Crops are now the main "bonus" item, and they provide a much higher bonus when they are a pair. This discourages cheap crops being used as a low-cost high-reward bonus when hybriding with expensive crops.
  * Each crop pair now provides an extra output seed upon hybrid success. Input crops are still destroyed after every hybrid attempt.
  * Hybrid process can be attempted without using crops (only seeds), but the process takes an extra day and has a -50% success chance.
  * Added some more random ideas for extra content.

8/27/2021
  * Initial code created.
  * Started developing plan for structure and implementation. Gathering modding resources relevant to what will be necessary for the mod.
  * Added more detailed planning notes

## Development Plan/Details
### Overall Plan
  * 1) Develop Hybrid Seed Maker machine
    * Basic interface
    * Full functionality
      * Only allow seeds/crops to be inserted; both sides must be different types
      * Time delay between hybrid process start -> end
      * Lock inputs once hybrid starts
      * Allow halting of the hybrid process, unlocking all inputs. Resets time delay. (AKA completely new hybrid process)
      * (?) Wiggle animation / provide light when hybrid in process
      * (?) Dialogue bubble with output item above machine if successful; red X if failure
    * First test "recipe" should take in two seed/crop types and, for testing purposes only, output a pre-existing item (maybe ancient seeds for now?)
  * 2) Validate that Hybrid Seed Maker is functioning perfectly for the first test above
  * Pre-3) Consider creating framework/api for hybrid crops
    * Simplify future development process
    * Allow collaboration
    * Support other mods/allow support by other mods
  * Pre-3.1) Consider creating framework/api for crop traits
    * Would help when adding more traits/inheritance perks than originally thought of
    * One thing that comes to mind is a "Winter" perk/prefix by hybriding with winter forage
      * Spring/summer/fall traits could also be obtained by hybriding with their respective crops/forage
    * Extra perks; think Tinkers' Construct
      * Glowing/Radiant/Irradiant: faintly glows in the dark, and glows brightly when ready to harvest; on sunny days, increases growth rate of self/self & surrounding crops/self & surrounding crops, and drops solar essence on harvest
      * Palatable/Cultured/Elite: produces silver/gold/iridium quality artisan good instead of normal quality
      * Premier/Superior/Perfect: harvested crop is at minimum silver/gold/iridium quality
      * Tasty: harvested crop provides extra health/energy
      * Absorbant: grows faster on rainy days
      * Fragile: can be harvested with scythe
      * June Drop: "harvests itself" when ready, leaving crop(s) on the floor (ensure this still counts for harvest quests)
      * Friendly: harvested crop has higher value when sold in-person rather than shipping, and provides extra friendship bonus when gifting
      * Quenched/Hydrant: after being watered initially, self-sustainably waters itself/self & surrounding crops
    * Other special, unique behavior...
    * Discovered traits should be shown in the Hybrid Encyclopedia. Undiscovered should be grayed out.
  * 3) Create the first hybrid crop
    * Crop details like price, growth, etc.
    * Temporary sprites for now (unless bored and want to pixel art lol)
  * 4) Define the "recipe" for the first hybrid crop to be created using the Hybrid Seed Maker. Validate all functionality works perfectly.
    * Debug success chance to validate success chance formula
  * 5) Create a few more hybrid crops with varying traits to validate that inheritance works perfectly in all cases.
  * 6) (?) Create the first hybrid fruit tree
    * Details like fruit price, season, etc.
    * Temporary sprites for now (unless bored and want to pixel art again lol)
  * 7) Define the "recipe" for the first hybrid fruit tree to be created using the Hybrid Seed Maker. Validate all functionality works perfectly.
  * 8) From this point on, foundation has been laid, so all work should be based off previously-completed code.
  * 9) Make reddit post asking for feedback, ideas, and opinions about what would be good to include and what would not. Better to get this feedback now rather than after everything has been made solely to my liking. 
  * 10) With community feedback in mind, make any desired changes, and start creating more hybrids!
  * 11) Attempt implementation of extra content and/or random ideas listed here.


### Hybrid Seed Maker
Large machine located in Demetrius' lab after completing Hybrid Crops storyline.

Takes in two different types of seeds (and crops, optionally). After some time, has a chance to produce seeds that are a hybrid between both of the input seed types.

#### Benefits to hybrid crops:
  * A hybrid of crop A and crop B sells for the price of A + B. (May need to be adjusted for balancing reasons. Maybe average of both prices?)
  * Grows at the average of A and B growth speed. (May need to be adjusted for balancing reasons)
    * Example: (A=8d, B=12d) => AB Hybrid = 10d
  * If re-harvestable, harvest speed is a weighted average of A and B harvest speed. (May need to be adjusted for balancing reasons)
    * Examples:
      * (A = 1ea x $50 / 2 days, B = 3ea x $50 / 4 days) =>
        * ***TODO***: *Create weighted average formula for re-harvest time*
      * (A = 1ea x $200 / NO_REHARVEST, B = 2.5ea x $10 / 3 days) => 3 days
  * Inherits traits from parent crops, such as: season, yield multiple at harvest, regrow, grows on trellis, etc.
  * Complete the Hybrid Encyclopedia - a new collection for shipping every implemented hybrid crop.
  * A hybrid crop can be used in place of any of its parent crops in cooking recipes.

#### Hybriding details:
  * Input a maximum of 5 seeds and 5 crops (minimum 1 seed and 0 crops) for both types, per hybrid attempt.
  * Combine two different seeds and their crops for a chance of recieving a hybrid of the two seeds in return.
    * Percent chance for hybrid shown via:
      * (Red -> Green) scaled colored-text, with text something like: ("Not likely" -> "Extremely likely").
        * (?) Maybe "IMPOSSIBLE" for hybrid crops that are not yet implemented
      * Maybe a brown slider nib inside red -> green gradient as well?
    * Adding crops increases chance for hybrid success
      * If hyriding with no crops (only seeds), success chance is reduced by -50%, and process takes an extra day until complete.
        * This should be reflected on the interface, informing players that using crops is better and faster.
      * This is to allow players to not *have to* spend anything for their hybrids, but on average will take much longer to obtain.
    * Can add up to 4 extra of each (total = 5 crops and 5 seeds), for both crop types.
      * Each extra crop adds +6.25% to success rate
      * Total = +25% for either type
      * If additional opposing-type crop is also added, total bonus increased by +62.5% for each "pair".
      * Grand total = +300% if both sides have 5 crops.
      * Examples:
          | (#seeds, #crops) x2 |   | left |   | right |   | pair bonus |   | total |
          |---------------------|---|------|---|-------|---|------------|---|-------|
          | (1,**1**), (1,**1**) | => | (0%)    | + | (0%)    | + | (0% bonus)    | = | +0% |
          | (5,**1**), (5,**1**) | => | (0%)  | + | (0%)    | + | (0% bonus)   | = | +0% |
          | (3,**2**), (1,**1**) | => | (6.25%) | + | (0%)    | + | (0% bonus)    | = | +6.25% |
          | (2,**2**), (5,**5**) | => | (6.25%)   | + | (25%)  | + | (62.5% bonus)   | = | +93.75% |
          | (4,**4**), (4,**4**) | => | (18.75%)   | + | (18.75%)   | + | (187.5% bonus)   | = | +225% |
          | (5,**5**), (1,**1**) | => | (25%)  | + | (0%)    | + | (0% bonus)    | = | +25% |
      * Current luck amount when starting hybrid process also slightly affects success chance
      * Hybrid process takes 2 in-game days (begin on 1st, results on 4th)
        * Displayed by a progress bar when hybriding is in process
          * gray background, bar is a gradient from blue (start) -> green (finish)
        * While hybriding is in process, if player inputs 10 (?) solar essence, speeds up the process by a day
      * Successful hybrid process takes all input seeds and converts them to same number of output hybrid seeds. Each crop pair adds an extra seed.
        * i.e. (2 seed A, 4 crop A) + (3 seed B, 2 crop A) => 2 + 3 + (2) = 7 hybrid AB seeds
      * Failed hybrid process returns all seeds back (crops are lost)

#### Interaction/Interface:
  * Uses a 2x2 machine located in Demetrius' lab
  * Can interact with the machine to open interface:
    * On left and right side of interface, either:
      * Two slots (one for seeds, one for crops) with stacking up to max. total, or
        * **Fancy idea**: *Upon placing a stack of ```X``` items into their respective slot, if the machine currently has ```Y``` spaces for that slot, take ```Y``` amount from the stack (or the entire stack ```X``` if ```Y >= X```), updating the machine's current space for that slot to ```Y - Min(X, Y)```. Each item gets visually "placed" into the machine and can be seen upon entering the interface. For ease of use (and easier for controllers), add a "Return all" button to the interface that attempts to return all the items from the machine into your backpack. (If no button, return each item upon clicking/selecting them in the interface.)*
      * Ten slots (five for seeds, five for crops)... wouldn't work with + max. total input config option
    * One slot on bottom for output hybrid seed
      * Or maybe a "trough" instead of a single slot, where all items go after hybrid process completes, success or failure.
    * One slot on bottom-left for solar essence input
      * **Fancy idea**: When solar essence is put in, visually place them nearby and show them expanding and glowing, before being absorbed by the machine. For each solar essence absorbed, increase the "glow" of the interface (I'm imagining glass tubes with green liquid running around the border/frame that will go from forest green to neon green). This should "grow" the blue/green progress bar to indiciate that the hybriding process has been sped up.

#### Random ideas:
  * Ancient fruit is many genetic variations behind all crops, so hybriding with it is very difficult, but IS possible.
    * Maybe a 2nd storyline quest that unlocks this?
  * Ultimate goal of genetically recreating the Genesis Seed, unlocking ability to grow Eden Fruit (new best crop - I'm imagining a golden apple).
    * Maybe by hybriding two different ancient fruit hybrids together, therefore making the eden fruit an AB-AC hybrid?
  * Purchase max. input amount upgrade from Demetrius
    * Maybe multiple tiers?
  * Goal to discover 25 (?) hybrids for Demetrius, tracked with the Hybrid Encyclopedia.
    * Each new hybrid found during this time will prompt the player to talk with Demetrius or interact with the Hybrid Encyclopedia.
    * Doing so will display a scientific research paper, originally completely blank, that gets a new detail added with each new hybrid.
    * Alongside the paper being filled out, each new completed "section" will prompt Demetrius to reward the player with money to continue funding your research.
    * When the paper is complete, player is told to talk with Demetrius.
      * He has successfully published your combined research! Scientists had been in conversation with him for a while now, so they were just waiting for the completed results and findings.
      * He was rewarded handsomely for the publication, as it will be further researched around the world to benefit global agriculture practices.
      * He then rewards the player with a final large sum of money as compensation for the help.
      * The project is officially out of his hands now, so any further hybriding is your choice!
      * Maybe he offers a max. input amount upgrade at this point?
    * Overall, this side goal is just a way to provide satisfaction to completionist players, as well as provide more incentive towards experimentation and discovering new hybrids.
    * If Wizard ever sees the completed paper, he alludes back to the myths and legends and says: this research paper is as if all of those supposed myths and legends had just been validated and explained, a story carried on through time. It truly is a magic of it's own.
      * Maybe leads him to explain the connection with Ancient Fruit, and allow for hybriding with them?
  * Maybe Hybrid Crops Storyline can be started as early as Winter Year 1?
    * Wizard was perplexed at the fruit that sprouted through the winter snow.
    * Quest instead rewards the player with hybrid seeds of a Winter ___ crop.
      * Same as the original crop, but with Winter added as one of its habitable seasons.
      * Maybe this Winter hybrid trait can be passed on using Winter forage items? Or hybriding with Winter ___ hybrid crops? (A-BC Hybrid)

### Hybrid Crops Storyline
Quest involving Wizard and Demetrius that provides a backstory leading up to unlocking the main mod content — the ability to create hybrid crops.

#### Prerequisites:
  * Year 2+ Spring, Summer, or Fall
			 * (?) Year 1 Winter instead, if planning on implementing Traits.
  * 3 hearts with Wizard and Demetrius
#### Condition:
		* Receive letter from wizard about a mysterious fruit he has stumbled upon. Wants you to come visit him.
		* Given quest: Visit the wizard to find out more about the mysterious fruit he found.
#### Wizard
		* Has unknown fruit from which he can sense a faint feeling of ... magic? No, something else.
		* In any case, no other crop gives off this feeling, as far as he's aware. Maybe that potion he made yesterday is still affecting him...
		* No, that can't be it. There's definitely something unique about this crop, but even his magic skills can't figure it what it is.
		* As much as it pains him to say it, his magic senses aren't helping this time. He can't figure it out.
		* He says to try bringing it to a human scientist and see if they have anything to say about it.
		* He is obviously embarassed at his inability. OPTIONS:
			 * It's okay, I can't even sense anything at all (cheers him up)
			 * Magic and science might have to work together (reassures him)
			 * Must feel weird to not be able to figure this out (still embarassed)
		* Quest updated: Give the mysterious fruit to Demetrius to research.
#### Demetrius
		* What's this? A fruit? Never seen anything like it. Is this a gift of some sort? OPTIONS:
			 * I'd like your help in finding out what it is (interested)
			 * Not really, but it could be if you'd like (laugh)
			 * Of course, it's called "more work for you" (annoyed)
		* Alright. I'm usually pretty busy, but if you give me a few days to work on it I can let you know what I find.
		* Quest updated: Visit Demetrius in 3 (counts down every day) days to find out more about the mysterious fruit.
#### Demetrius (3 days left)
		* I'll start looking at that fruit you gave me in just a little bit.
#### Demetrius (<= 2 days left)
		* I'm still looking at that fruit you gave me. I have to say, it has really sparked my curiosity.
#### Demetrius (after time is up)
		* Hey {NAME}, I finished looking at that fruit you gave me. What a bizarre plant species! I ended up running a few tests on its seeds.
		* Using a makeshift phytogenic insolator, I managed to induce premature blooming and flowering via cotyledon expansion, and...
		* Well, uh... (notices his scientific talk, embarassed) The seeds were able to produce fruit quite quickly.
		* Oddly, however, my test resulted in the seeds producing Ancient Fruit; nothing like the fruit you originally gave me.
		* My only hypothesis is that there must be a genetic link... perhaps from a time far before documented horticulture.
		* (Something moves/makes a magical noise in an obscure spot in the room)
		* You can take the fruit, if you want. There's plenty more of them growing from these seeds that I can continue my experiments with.
		* Quest updated: Tell the wizard what Demetrius has discovered from his experiment.
		* Receive rewards:
			 * 1x Ancient Fruit
#### Wizard
		* Before you say anything, the wizard cuts you off explaining that he has been reading up on some old stories relating to what Demetrius said.
		* (He must have been listening in on your conversation. That explains the weird noise.)
		* He has read them all before, but just wanted a refresher to be sure. Many stories have been written about the Genesis Seed.
		* In those stories, a miraculous plant was said to bloom, seed, pass on, and grow again for eons.
		* It flourished in all of its eventual environments, evolving and adapting over time to fit in wherever it may be.
		* It was told to be one of the most important pieces in bringing life and vibrance to the world.
		* All this time, he thought it was just a legend, or a metaphor for something... there was never any link found validating that story.
		* But thanks to Demetrius (and of course, himself) that link may have just been found!
		* The Ancient Fruit that sprouted from his experiement must be closely derived from the Genesis Seed... the very same of which those stories tell.
		* If you could convince Demetrius to let you join in on his experimentation, the wizard would be very interested in any results found.
		* Quest updated: Convince Demetrius to let you join in with his research on the mysterious seeds.
#### Demetrius
		* Hi {NAME}, have you come to check in on my experiments?
		* I've tried splicing the mysterious fruit seeds with multiple different crop seeds, but they all end up sprouting into Ancient Fruit.
		* You know, I never asked... where'd you get that fruit from, anyway? OPTIONS:
			 * It randomly grew on my farm. (suspicious, skeptical look)
			 * I found it in the forest. (interested look)
			 * Some strange guy gave it to me. (concerned look. Hopes you're not talking about Linus.)
		* So anyway, I've been thinking. These experiments I've been doing so far... I've been able to nail down the *process*, but not any results yet.
		* I know that these specific seeds are indeed interesting... But this process could definitely be applied to other seeds too!
		* I need to look into performing more of these tests, but with different types of seeds and crops. That should get me some real results.
		* (After this change of thought-process, you offer your help.) OPTIONS:
			 * I could help with your research too!
			 * It just so happens that I deal with a lot of crops myself!
		* That's great! If you wouldn't mind helping me, we really could learn a lot from this. I could even be published!
			 * I'd be happy to help! (confident)
			 * I think you meant "we". (embarassed)
		* I'll have to give you access to some of the equipment Maru and I built to perform these tests so far.
		* (Warp to Demetrius' lab)
		* Considering its functionality, I suppose you could call this machine a Hybrid Seed Maker.
		* It uses two different types of seeds or crops, and after some time splicing the genetic structure together, there may be a hybrid seed created!
		* It doesn't always work perfectly, though. As you can imagine, there are a lot of potential combinations, with some being more viable than others.
		* Feel free to experiment with any seeds and crops you have. I'll be very interested in your findings!
		* Quest complete! Rewards:
			 * **If craftable**:
			   * 1x Hybrid Seed Maker
			   * (?) Recipe for Hybrid Seed Maker (maybe recipe comes later in a letter?)
			 * **If not**: Access to the Hybrid Seed Maker in Demetrius' lab.
			 * Gifting hybrid crops to Wizard, Demetrius, or Maru are all loved gifts.

### Config
Hybrid Attempt
  * Time needed
    * for people who don't want to wait
  * No crop success chance penalty
    * how much to decrease success chance by when only hybriding with seeds
  * No crop time penalty
    * how much time to add to hybrid process when only hybriding with seeds
  * Global success chance multiplier
    * to make succesfully hybriding more/less likely
  * Max amount of input seeds
    * allows more input seeds and consequently more output seeds
  * Crops count towards output seed amount
    * makes each input crop provide an additional output seed
  * Additional seed/crop success chance
    * modify the additional success chance obtained from using multiple seeds/crops
  * Matching seed/crop pair success chance bonus
    * modify the additional success chance bonus obtained from using matching pair(s) of seeds/crops

Quest Prerequisites
  * have access to the quest without waiting for Year 2, or
  * skip the story and quest entirely, instantly gaining access to hybriding.


