# Game Design Document

---

### 1. Title Page
**1.1. Game Name**

    TBD.

**1.2. Tag Line**

    N/A.

**1.3. Team**

    Christopher Tye, Mitch Carrillo, Tristian Ruiz, Chase Hartley.

**1.4. Date of Last Update**
    
    02/01/2023.

---

### 2. Game Overview
**2.1. Game Concept**

    Revival of old arcade beat 'em ups with modern gameplay loops like a combo gauge you build up to spend on your ability.

**2.2. Target Audience**

    Indie game enthusiasts and CS491 students.

**2.3. Genre(s)**

    Beat 'em up, fighting game, fantasy, side-scrolling.

**2.4. Game Flow Summary**

    Zones are broken up into arenas and rooms. 
    Arenas are set regions where you must defeat the enemies to continue, with the camera being sat in one position. 
    Rooms are the rest of the gameworld you traverse outside of combat, with the camera follwing the player.

**2.5. Look and Feel**

    Cartoon high fantasy art style, inspired by games like World of Warcraft.
    The player character, Vira, is a brooding drunkard who easily resolves conflicts with her fists.

---

### 3. Gameplay
**3.1. Objectives**

    Reach the end of the zone and defeat the miniboss before your health reaches zero.

**3.2. Game Progression**

    New zones become more difficult, with more enemies as well as new ones.

**3.3. Play Flow**

    The average flow for a player will be defeating enemies, building combo gauge, spending it on powerful abilities, repeat.

**3.4. Mission/Challenge Structure**

    N/A

**3.5. Puzzle Structure**

    N/A

---

### 4. Mechanics
**4.1. Rules**

    The game world is an on rails experience. The player must follow the path forward, akin to a Super Mario Bros. level.
    If the player gets hit when possessing a combo upgrade, the upgrade is lost/spent to the preceeding tier.

**4.2. Model of the Game Universe**

    The player is the catalyst of the world. Enemies won't spawn until Vira enters an arena.
    Enemies won't interact with one another in their AI, they just care about the player.
    3D terrain in the level is treated as a true obstacle to pass, whether it be player or AI.

**4.3. Physics**

    While the characters exist within a 3D world, their reactions to each other are determined on a 2D plane.
    There is gravity to pull the player down, but no way to jump or move upwards besides a ramp.
    2D characters are set to ignore physics impulses not from the player, but this can be changed on the fly for stylistic choices.

**4.4. Economy**

    Beating up an enemies builds up the players combo gauge. The combo gauge acts as a wallet.
    Keep your gauge level and invest, and you recieve upgrades accordingly so long as you remain at that tier.
    Spend your gauge level, and you lose the long term upgrades for a powerful, short term burst ability.
    The players health is a commodity, so spend your gauge for damage and you're temporarily losing the safety blanket the gauge provides you.

**4.5. Character Movement in the Game**

    2D chracters move along a slice of a 3D world.
    They are able to move up and down along this slice fighting enemies as well as interacting with world objects.

**4.6. Objects**

    Potentially wooden boards you can destroy to make a passageway.
    No other current plans.

**4.7. Actions**

    The keyboard is the main input, though controller could be supported with correct implementation.
    The player interacts with the world solely with violence.
    The mouse is used to interface with game menus.

    Left click - Punch
    Right click - Kick
    E - Counter
    Space - Ability

**4.8. Combat**

    The enemies always approach the player when not attacking.
    Simple enemies have one attack, but boss enemies may have a special attack as well.
    Vira is very powerful, and exerts this power on the lesser enemies. The player should feel like they are in control of the situation.
    Somewhat chaotic and quick.

**4.9. Screen Flow**

    Arenas contain enemies, and the player must defeat them to move forward. While in these arenas, the players position doesn't controll the camera.
    Outside of arenas, the screen scrolls along with the players movement.
    Since this is a 3D world, objects can be employed in the foreground between the camera and player to give a greater sense of depth.

**4.10. Game Options**

    Volume settings, possibly a level lock.

**4.11. Replaying and Saving**

    Completing a zone saves that progress. Settings also saved between sessions.

**4.12. Cheats and Easter Eggs**

    N/A

---

### 5. Story and Narrative
**5.1. Back Story**

    Our main character, Vira, is a drunkard. Not known to be a very compassionate being, Vira can be quite hot headed.
    At her new favorite bar in town (after she destroyed the last one for running out of ale), she once again runs the joint dry.
    Threatening the bartender, she demands a new keg of ale by the time she wakes up from her nap, or she'll tear the place down.
    Upon waking up, shes told that the main villian has stolen the keg, and thus Vira must now rip and tear through anyone in her way to get to it.

**5.2. Plot Elements**

    Keg of ale moving through the zones of the gameworld. Final boss at end having a pint.

**5.3. Game Story Progression**

    The story progresses at the end of each zone, where you find out that your princess (keg of ale) is in another castle (zone).

**5.4. Cutscenes**

    N/A

---

### 6. Game World
**6.1. General Look and Feel**

    High fantasy cartoon world. It's a serene place, though you fight many enemies, they're just natives who got in the way.

**6.2. Areas**

    Highmountain Alps - The beginning of our journey, a beautiful landscape wiht sparse tree lines and open fields beside great mountains. Leads into covered forest.

    Covered Forest - A magical fae forest, with hues of blue. A bit more densely populated, so more enemies. Leads to basalt lakes.

    Basalt Lakes - The home of the main villian. Developed with stone bridges and amenities, but volcanoes stretch through the background replenishing the lakes.

---

### 7. Characters
**7.1. Each Character**

    Player Character (Vira) - 

    Main Villian (Goblin Mech) -

    Native Population -

**7.2. AI Use in Opponents and Enemies**

    The AI is simple for fodder enemies. They're meant to be a little dull, so the best they can do is follow Vira until they're close enough to attack.
    Bosses may have more abilties that make them inherenitly smarter.

**7.3. Non-Combat and Friendly Characters**

    The bartender at the beginning of the game isn't exactly a friend, but its the best Vira's got. We don't see him for long.

---