# Hybrid Crops
An expansion to Stardew Valley that adds the ability to create hybrid crops.

## Devlog
### 1.0.0
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
    * Potentially add more traits/inheritance perks than originally thought of
      * One thing that comes to mind is a "Winter" perk/prefix by hybriding with winter forage
      * Other special, unique behavior like above...
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
  * 8) From this point on, foundation has been laid, so all work should be based off previously-completed code. Now create more hybrids!


### Hybrid Seed Maker
Large machine located in Demetrius' lab after completing Hybrid Crops storyline.

Takes in two different types of seeds (and crops, optionally). After some time, has a chance to produce seeds that are a hybrid between both of the input seed types.

Benefits to hybrid crops:
  * A hybrid of crop A and crop B sells for the price of A + B. (May need to be adjusted for balancing reasons)
  * Grows at the average of A and B growth speed. (May need to be adjusted for balancing reasons)
    * Example: (A=8d, B=12d) => AB Hybrid = 10d
  * If re-harvestable, harvest speed is a weighted average of A and B harvest speed. (May need to be adjusted for balancing reasons)
    * Examples:
      * (A = 1ea x $50 / 2 days, B = 3ea x $50 / 4 days) => --------------------------- TODO: Create weighted average formula for re-harvest time
      * (A = 1ea x $200 / NO_REHARVEST, B = 2.5ea x $10 / 3 days) => 3 days
  * Inherits traits from parent crops, such as: season, yield multiple at harvest, regrow, grows on trellis, etc.
  * Complete the Hybrid Encyclopedia - a new collection for shipping every implemented hybrid crop.

Details:
  * Input a maximum of 5 seeds and 5 crops (minimum 1 seed and 0 crops) for both types, per hybrid attempt
  * Combine two different crops and/or their seeds for a chance of recieving a hybrid of the two crops in return.
    * Percent chance for hybrid shown via:
      * (Red -> Green) scaled colored-text, with text something like: ("Not likely" -> "Extremely likely").
        * (?) Maybe "IMPOSSIBLE" for hybrid crops that are not yet implemented
      * Maybe a brown slider nib inside red -> green gradient as well?
    * Adding more crops/seeds increases chance for hybrid success
    * Can add up to 4 extra of each (total = 5 crops and 5 seeds), for both crop types.
		  * Each extra crop/seed adds +12.5% to success rate --------------------------- TODO: Crops should give more success rate, but get destroyed upon hybrid attempt
			  * Total = +100% for either type
				* If additional opposing-type crop/seed is also added, total bonus increased by +12.5% for each "pair".
				* Grand total = +300% if both sides have 5 crops and 5 seeds.
				* Examples (#seeds,#crops):
					* (1,1),(1,1) => (0%)    + (0%)    + (0% bonus)    = +0%
					* (3,2),(1,1) => (37.5%) + (0%)    + (0% bonus)    = +37.5%
					* (2,2),(5,5) => (25%)   + (100%)  + (25% bonus)   = +150%
					* (4,4),(4,4) => (75%)   + (75%)   + (75% bonus)   = +225%
					* (1,5),(5,2) => (50%)   + (62.5%) + (12.5% bonus) = +125%
					* (5,5),(1,1) => (100%)  + (0%)    + (0% bonus)    = +100%
					* (5,1),(5,1) => (100%)  + (0%)    + (50% bonus)   = +150%
		* Current luck amount when starting hybrid process also slightly affects success chance
		* Hybrid process takes 2 in-game days (put in on 1st, results on 4th)
		* Successful hybrid process takes all input and converts total number of input seeds/crops to same number of output hybrid seeds.
			* i.e. 2 seed A & 4 crop A + 3 seed B => 9 hybrid AB seeds
		* Failed hybrid process returns all seeds back (crops are lost)
	* Uses a 2x2 machine located in Demetrius' lab
	* Can interact with the machine to open interface:
		* Two slots (one for seeds, one for crops) on left and right side of interface.
		* One slot on bottom for output hybrid seed (or maybe a "trough" instead of a single slot)
			* Becomes two slots upon hybrid process failure, holding returned seeds for both types
			* Once returned seeds are taken, reverts back to a single slot

### Hybrid Crops Storyline
Quest involving Wizard and Demetrius that provides a backstory leading up to unlocking the main mod content — the ability to create hybrid crops.

Prerequisites:
  * Spring Year 2 or later
  * Friendship with Wizard and Demetrius

### Config
Hybrid Attempt
  * Time needed
    * for people who don't want to wait
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


