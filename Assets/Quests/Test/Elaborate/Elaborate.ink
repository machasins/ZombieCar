VAR char_name = "Wealthy Charles"
VAR char_sprite = "test_charles"

VAR quest_0 = false
VAR quest_1 = false
VAR quest_2 = false
VAR quest_3 = false
VAR quest_4 = false

{quest_4: -> finished}
{quest_3: -> defeated}
{quest_2: -> fighting}
{quest_1: -> collected}
{quest_0: -> accepted}

- Hello there old chap!
- Would you mind assisting me in some menial tasks?
* [Of course.]
    ~ quest_0 = true
    Wonderful. <> -> accepted
* [I'm busy.]
    Hmm. Very well, but I assure you the compensation is well worth the effort!
-> END

=== accepted ===
- I need some things collected from around town. There's three things in total. A <b>light blue box</b>, some <b>green circles</b>, and a <b>purple triangle</b>.
- The <b>light blue box</b> should be to the north, by some trees.
- The <b>green circles</b> are in a pile to the west, guarded by an insect.
- Finally, the <b>purple triangle</b> is to the southeast, surrounded by fences.
- I wish you luck on your journey, chap.
-> END

=== collected ===
- Ah, I see you have the things I requested. Most wonderful indeed, I'll just take those off of you right now.
- If I could request one more thing of you...
* [Of course.]
    ~ quest_2 = true
    Wonderful. <> -> fighting
* [I'm busy.]
    Hmm. Very well, but I assure you the compensation is well worth the effort!
-> END

=== fighting ===
- There are some bandits harrassing the town. I need you to take care of them. They are located to the northwest, the south, and the east.
- I wish you luck, chap.
-> END

=== defeated ===
- Ah, I see you have done all I have asked. Thank you, chap.
- Here is your promised compensation. I hope it will be to your liking.
- The best of luck to you in your future endeavors.
~ quest_4 = true
-> END

=== finished ===
- Thank you again, chap. Best of luck to you.
-> END