VAR char_name = "Old Man Jenkins"
VAR char_sprite = ""

VAR quest_2 = false
VAR quest_1 = false
VAR quest_0 = false

{quest_2: -> finished}
{quest_1: -> collected}
{quest_0: -> accepted}

- Hey there.
- Could you get me that there <b>blue box</b>?
* [Sure.]
    Cool. It's right over there.
    ~ quest_0 = true
* [I'm busy.]
    Whatever.
-> END

=== accepted ===
- It's just northwest of 'ere.
- Can't miss it mate.
-> END

=== collected ===
- Oh, is that the <b>blue box</b>?
- Thanks for that. Here's a little something for your time.
~ quest_2 = true
-> END

=== finished ===
- That's all I needed. Thanks for all your help!
-> END